using MessageQueues.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MessageQueues.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult GetResponse<T>(T data, HttpStatusCode statusCode)
        {
            return Ok(new ApiResponse<T>(statusCode.GetHashCode(), data));
        }
    }
}
