using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CCA.Services.EventCranny.JsonHelpers;
using CCA.Services.EventCranny.Models;
using CCA.Services.EventCranny.Logging.Models;
using Microsoft.Extensions.Logging;
using CCA.Services.EventCranny.Service;
using Microsoft.AspNetCore.Hosting;

namespace CCA.Services.EventCranny.Controllers
{
    [Route("/queue/{queueName}")]
    public class EventCrannyQueueController : Controller
    {
        [HttpPost("event")]   //   POST /queue/{queue-name}/event     // send event  message-body JSON 
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult eventSend([FromServices]IEventCrannyService service, string queueName)
        {
            //_logger.LogInformation("PUT kill requested");
            // service.Send(postBody);      // to-do: get postBody and process throught to RabbitMQ
            return ResultFormatter.ResponseOK(queueName);
        }
    }
}
