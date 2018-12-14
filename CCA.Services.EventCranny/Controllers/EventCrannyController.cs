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
    [Route("/")]
    public class EventCrannyController : Controller
    {
        //private readonly ILogger<EventCrannyController> _logger;
        // public EventCrannyController( ILogger<EventCrannyController> logger )   
        //{
        //    _logger = logger;
        //}

        [HttpPut("kill")]   // PUT in killed state : container ASPNETCore is shut down completely -> no more logging via its host container
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult Kill([FromServices]IEventCrannyService service)
        {
            //_logger.LogInformation("PUT kill requested");
            return ResultFormatter.ResponseOK(service.kill());
        }
        [HttpGet("ping")]   // ping
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetPing()
        {
            //_logger.LogInformation("GET ping");
            return ResultFormatter.ResponseOK((new JProperty("Ping", "Success")));
        }
        [HttpGet("version")]   // service version (from compiled assembly version)
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetVersion()
        {
            //_logger.LogInformation("GET version");
            var assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();
            return ResultFormatter.ResponseOK((new JProperty("Version", assemblyVersion)));
        }
    }
}
