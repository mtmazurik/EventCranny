﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CCA.Services.EventCranny
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost webHost = WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();                   
            webHost.Run();
        }
    }
}