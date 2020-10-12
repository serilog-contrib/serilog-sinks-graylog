using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace TestWebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger _logger;

        public WeatherForecastController()
        {
            _logger = Log.ForContext<WeatherForecastController>();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("burn")]
        public IEnumerable<BurnStat> LetsBurn()
        {
            var result = new List<BurnStat>();

            for (int i=0; i < 1000; i++)
            {
                var item = new BurnStat
                {
                    Today = DateTime.Now,
                    RandomPayload = Guid.NewGuid().ToString()
                };
                result.Add(item);
                
                _logger.Information("BurnInfo: {@Info}", item);
            }

            return result;
        }
    }

    public class BurnStat
    {
        public DateTime Today { get; set; }

        public string RandomPayload { get; set; }
    }
}
