using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using AzuriteEmulator.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AzuriteEmulator.Controllers
{
    [ApiController]
    [Route("api")]
    public class AzuriteEmulatorController:ControllerBase
    {
        private readonly string _blobContainerName;
        private readonly string _queueContainerName;
        private readonly string _tableContainerName;

        private readonly TableServiceClient _tableServiceClient;

        private readonly QueueServiceClient _queueServiceClient;

        private readonly BlobServiceClient _blobServiceClient;

        public AzuriteEmulatorController(TableServiceClient tableServiceClient, 
            QueueServiceClient queueServiceClient, 
            BlobServiceClient blobServiceClient)
        {
            _tableServiceClient = tableServiceClient;
            _queueServiceClient = queueServiceClient;
            _blobServiceClient = blobServiceClient;
            _blobContainerName = "pictures";
            _queueContainerName = "adminrequest";
            _tableContainerName = "employees";
        }
        [HttpPost("file/publish")]
        public async Task<IActionResult> PublishFile(IFormFile file, CancellationToken token)
        {
            
            var container = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            await container.CreateIfNotExistsAsync(cancellationToken: token);

            //publish
            var blobClient = container.GetBlobClient(file.FileName);

            await blobClient.UploadAsync(file.OpenReadStream(), cancellationToken: token);
            //await container.UploadBlobAsync(_blobContainerName, file.OpenReadStream(), cancellationToken: token);

            return Accepted();
        }
        [HttpGet("file/get")]
        public async Task<IActionResult> GetFile(string fileName, CancellationToken token)
        {
            var contaier = _blobServiceClient.GetBlobContainerClient(fileName);
            if(!await contaier.ExistsAsync(token))
            {
                return BadRequest($"Container does not exists: {_blobContainerName}");
            }

            var blobClient = contaier.GetBlobClient(fileName);

            var result = await blobClient.DownloadContentAsync(token);
            var contentType = result.Value.Details.ContentType;
            var content = result.Value.Content.ToArray();
            return File(content, contentType);
        }

        [HttpPost("queue/publish")]
        public async Task<IActionResult> UpdateQueue([FromBody]QueueModelDto model, CancellationToken token)
        {

            var queue = _queueServiceClient.GetQueueClient(_queueContainerName);
            await queue.CreateIfNotExistsAsync(cancellationToken: token);

            var messaqe = JsonSerializer.Serialize<QueueModelDto>(new QueueModelDto()
            {
                Id=model.Id,
                Name=model.Name,
                Description=model.Description

            });
            await queue.SendMessageAsync(messaqe, cancellationToken: token);

            return Accepted();
        }
        [HttpGet("queue/get")]
        public async Task<IActionResult> GetMessageFromQueue(CancellationToken token)
        {
            var queue = _queueServiceClient.GetQueueClient(_queueContainerName);
            if(!await queue.ExistsAsync(cancellationToken: token))
            {
                return BadRequest($"Invalid Queue name: {_queueContainerName}");
            }

            var list = new List<QueueModelDto>();   

            while (true)
            {
                var result = await queue.ReceiveMessageAsync(cancellationToken: token);
                if(result.Value == null)
                {
                    break;
                }
                list.Add(result.Value.Body.ToObjectFromJson<QueueModelDto>());
                await queue.DeleteMessageAsync(result.Value.MessageId, result.Value.PopReceipt, cancellationToken: token);
            }

            return Ok(list);   
        }
    }
}
