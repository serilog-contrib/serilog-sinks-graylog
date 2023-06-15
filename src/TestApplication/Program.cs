using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(args);
SelfLog.Enable(Console.WriteLine);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, config) =>
{

    var serilogSection = context.Configuration.GetSection("Serilog");
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
   {
       var seriloglogger = Log.ForContext<Program>();
       
       var forecast = Enumerable.Range(1, 5).Select(index =>
                                    new WeatherForecast
                                    (
                                        DateTime.Now.AddDays(index),
                                        Random.Shared.Next(-20, 55),
                                        summaries[Random.Shared.Next(summaries.Length)]
                                    ))
                                .ToArray();
       return forecast;
   })
   .WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}