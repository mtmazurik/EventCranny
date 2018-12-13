﻿using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Hosting;
using CCA.Services.EventCranny.Config;
using CCA.Services.EventCranny.Security;
using CCA.Services.EventCranny.Models;
using CCA.Services.EventCranny.Tasks;
using CCA.Services.EventCranny.Logging.Models;
using CCA.Services.EventCranny.Logging.Provider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CCA.Services.EventCranny.Service;

namespace CCA.Services.EventCranny
{
    public class Startup
    {
        private ILogger<Program> _logger;
        private IConfigurationRoot _configuration { get; }

        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)       // ctor
        {
            var builder = new ConfigurationBuilder()        
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }
        private void OnShutdown() // callback, applicationLifetime.ApplicationStopping triggers it
        {
           _logger.Log(LogLevel.Information, "EventCranny service stopped.");
        }

        public void ConfigureServices(IServiceCollection services)    // Add services to the ASPNETCore App. This gets called by the runtime. 
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .WithMethods("Get", "Post", "Put")
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // leverage Auth0.com FREE service for API Authentication (for now)
            services.AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               }).AddJwtBearer(options =>
               {
                   options.Authority = $"https://{_configuration["Auth0:Domain"]}/"; 
                   options.Audience = _configuration["Auth0:ApiIdentifier"]; 
               }
            );
 
            services.AddMvc(options =>
            {
                options.Filters.Add(new AllowAnonymousFilter());
            }).AddJsonOptions( options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });


            /* services.AddSingleton<IHostedService, TaskManager>();     */     // task manager  (for background processing)


            services.AddSwaggerGen(options =>                                   // Swagger - autodocument setup
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "EventCranny Service",
                    Version = "v1",
                    Description = "RESTful API, micro service called 'EventCranny'",
                    TermsOfService = "(C) 2018 Cloud Computing Associates (CCA)  All Rights Reserved."
                });
            });

            string  dbConnectionString = _configuration.GetConnectionString("EventCrannySvcRepository");


            services.AddTransient<IResponse, Response>();                       // Dependency injection (DI) - using ASPNETCore's built-in facility
            services.AddTransient<HttpClient>();
            services.AddTransient<IJsonConfiguration, JsonConfiguration>();
            services.AddTransient<IWorker, Worker>();
            services.AddTransient<IEventCrannyService, EventCrannyService>();
           
            // logger setup
            CustomLoggerDBContext.ConnectionString = _configuration.GetConnectionString("LoggerDatabase");
        }
       public void ConfigureLogging( ILoggingBuilder logging)
       {
            logging.ClearProviders();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
       }
        // Use this method to configure the HTTP request pipeline. This method gets called by the runtime. 
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddContext(LogLevel.Information, _configuration.GetConnectionString("LoggerDatabase"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger- autodocument
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventCranny Service");
            });

            app.UseAuthentication();    // JWT Auth - using ASPNETCore methodology

            app.UseCors("CorsPolicy");

            app.UseMvc();

            _logger = loggerFactory.CreateLogger<Program>();
            _logger.Log(LogLevel.Information, "EventCranny service started.");

            applicationLifetime.ApplicationStopping.Register( OnShutdown );
        }
    }
}