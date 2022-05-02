using Azure.Storage.Queues;
using MessageQueues.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MessageQueues.Controllers;

[ApiController]
[Route("api")]
public class MainController : BaseController
{
    private QueueClient queueClient;
    private string _connectionString;
    public IConfiguration _configuration;

    public MainController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetSection("Configs").GetSection("ConnectionString").Value;
    }
    [HttpPost("{queueName}")]
    [ProducesResponseType(200)]
    public async Task PostMessage(string queueName, [FromBody]MessageRequest request)
    {
        var queueClient = new QueueClient(_connectionString, queueName);
        queueClient.CreateIfNotExists();
        if (queueClient.Exists())
        {
            var serializedMessage = JsonSerializer.Serialize(request);
            await queueClient.SendMessageAsync(serializedMessage,null,TimeSpan.FromSeconds(-1));
        }
    }
    [HttpGet("{queueName}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetMessage(string queueName, [FromQuery]int timeout = 10)
    {
        queueClient = new QueueClient(_connectionString, queueName);
        if (queueClient.Exists())
        {
            var message = await GetMessageFromQueue();
            if(message is not null)
            {               
                return GetResponse(message, HttpStatusCode.OK);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(timeout));
                message = await GetMessageFromQueue();
                if (message is not null)
                {
                    return GetResponse(message, HttpStatusCode.OK);
                }
                else
                {
                     return NoContent();
                }
            }
        }
        else
        {
            return GetResponse("Queue not exist", HttpStatusCode.NotFound);
        }
    }

    private async Task<string?> GetMessageFromQueue()
    {
        var message = await queueClient.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
        if (message.Value is not null)
        {
            var messageData = JsonSerializer.Deserialize<MessageRequest>(message.Value.Body);
            await queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
            return messageData.Message;
        }
        return null;
    }


}
