using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.EventCranny.Service
{
    public class EventCrannyService : IEventCrannyService
    {
        private IApplicationLifetime _applicationLifetime;
        public EventCrannyService(IApplicationLifetime applicationLifetime)     //ctor
        {
            _applicationLifetime = applicationLifetime;
        } 
        public string kill()
        {
            _applicationLifetime.StopApplication();
            return "EventCranny service stopped.";
        }
    }
}
