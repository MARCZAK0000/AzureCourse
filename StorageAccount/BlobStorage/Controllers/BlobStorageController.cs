using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorage.Controllers
{
    [ApiController]
    [Route("api/blob")]
    public class BlobStorageController:ControllerBase
    {

        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageController(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            var containerName = "documents";
            var container = _blobServiceClient.GetBlobContainerClient(containerName); //Get Specify Blob Container

            await container.CreateIfNotExistsAsync(); //Create container if not exist 

            var blobClient = container.GetBlobClient(file.FileName);

            if(await blobClient.ExistsAsync()) 
            {
                throw new Exception("BadRequest");
            }

            await blobClient.UploadAsync(file.OpenReadStream());

            return Ok();

        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string FileName)
        {
            var containerName = "documents";

            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            if(! await container.ExistsAsync()) 
            {
                throw new Exception($"{containerName} does not exist");
            }

            var blobClient = container.GetBlobClient(FileName);

            if(! await blobClient.ExistsAsync()) 
            {
                throw new Exception($"{FileName} not found"); 
            }

            var result = await blobClient.DownloadContentAsync();
            var content = result.Value.Content.ToArray();
            var contentType = blobClient.GetPropertiesAsync().Result.Value.ContentType.ToString();
            return File(content, contentType);



        }
    }
}
