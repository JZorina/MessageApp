using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MessageQueues.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{

    private readonly ILogger<HomeController> _logger;

    [HttpPost]
    public async Task Post([FromQuery]string queueName, [FromBody]string message)
    {
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=messagequeuetestim;AccountKey=UrXqh4buaaHKdWSY/h2AL1SzDY2PvufAC+IphSTkWsLVrXigj8kYCiqAGxwWmdvJH+YXRWZjECRf+AStKSD94g==;EndpointSuffix=core.windows.net";
        var queueClient = new QueueClient(connectionString, queueName);
        queueClient.CreateIfNotExists();
        if (queueClient.Exists())
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            await queueClient.SendMessageAsync(serializedMessage);
        }
        else
        {
            Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
        }
    }


}
