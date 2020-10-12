using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;

namespace TestWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var loggerConfig = new LoggerConfiguration();

            loggerConfig
                //.Enrich.WithExceptionDetails()
                .WriteTo.Graylog(new GraylogSinkOptions
                {
                    ShortMessageMaxLength = 50,
                    MinimumLogEventLevel = LogEventLevel.Information,
                    TransportType = Serilog.Sinks.Graylog.Core.Transport.TransportType.Http,
                    Facility = "testfactility",
                    HostnameOrAddress = "http://localhost",
                    Port = 12201
                });

            var logger = loggerConfig.CreateLogger();

            Log.Logger = logger;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Aeroclub Services Databroom API", Version = "v1"});
                
                string execXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string execXmlPath = Path.Combine(AppContext.BaseDirectory, execXmlFile);
                c.IncludeXmlComments(execXmlPath);
                c.CustomSchemaIds(x => x.FullName);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test service");
            });
        }
    }
}
