using Serilog;
using Serilog.Debugging;
using Serilog.Exceptions;
using TestApplication;

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

app.MapPost("/simple_send", () =>
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
       seriloglogger.Information("SomeInf {@Forecast}", forecast);
       
       return forecast;
   })
   .WithName("simple_send");

app.MapPost("simple_exception", () =>
{
    var seriloglogger = Log.ForContext<Program>();
    try
    {
        throw new Exception("test");
    }
    catch (Exception ex)
    {
        seriloglogger.Error(ex, ex.Message);
    }

}).WithName("simple_exception");

app.MapPost("custom_exception", () =>
{
    var seriloglogger = Log.ForContext<Program>();
    try
    {
        throw new CustomException()
        {
            CustomExceptionValue = "test_custom_exception_value",
            Data = { ["SomeData"] = "SomeValue" }
        };
    }
    catch (Exception ex)
    {
        seriloglogger.Error(ex, ex.Message);
    }

}).WithName("custom_exception");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
