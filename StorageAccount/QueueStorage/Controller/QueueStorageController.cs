using Azure.Core;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace QueueStorage.Controller
{
    [ApiController]
    [Route("/api")]
    public class QueueStorageController:ControllerBase
    {
        private readonly QueueClient _queueStorage;
        public QueueStorageController(QueueClient queueStorage)
        {
            _queueStorage = queueStorage;
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish()
        {

            await _queueStorage.CreateIfNotExistsAsync();
            
            var message = JsonSerializer.Serialize<ReturnDto>(new ReturnDto()
            {
                Id = "0000",
                Name = "Message",
                Description = "TestMessage",
            });

            await _queueStorage.SendMessageAsync(message);

            return Ok(message);
        }
        [HttpGet("dequeue")]
        public async Task<IActionResult> Dequeue()
        {

            if(!await _queueStorage.ExistsAsync())
            {
                return BadRequest("There is no queue");
            }
            var listFromQueue = new List<ReturnDto>();
            while(true) 
            {
                var result = await _queueStorage.ReceiveMessageAsync();
                if(result.Value==null) 
                {
                    break;
                }
               
                listFromQueue.Add(result.Value.Body.ToObjectFromJson<ReturnDto>());
                await _queueStorage.DeleteMessageAsync(result.Value.MessageId, result.Value.PopReceipt);
                await Task.Delay(1000);
            }
            return Ok(listFromQueue); 
        }
    }


    public class ReturnDto
    {
        public string Id { get; set; }

        public string Name { get; set; }    

        public string Description { get; set; }
    }
}
