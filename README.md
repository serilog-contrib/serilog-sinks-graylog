# serilog-sinks-graylog

[![Build status](https://ci.appveyor.com/api/projects/status/jaxp1cti0yu5boq7?svg=true)](https://ci.appveyor.com/project/whir1/serilog-sinks-graylog)

codecov not working because Open cover not covering .Net Core projects

## What is this sink ?
The Serilog Graylog sink project is a sink (basically a writer) for the Serilog logging framework. Structured log events are written to sinks and each sink is responsible for writing it to its own backend, database, store etc. This sink delivers the data to Graylog2, a NoSQL search engine.

## Quick start

```powershell
Install-Package serilog.sinks.graylog
```
Register the sink in code.
```csharp
var loggerConfig = new LoggerConfiguration()
    .WriteTo.Graylog(new GraylogSinkOptions
      {
          HostnameOrAdress = "localhost",
          Port = 12201
      });
```
...or alternatively configure the sink in appsettings.json configuration like so:

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Graylog"],
    "MinimumLevel": "Debug",
    "WriteTo": [
    {
        "Name": "Graylog",
        "Args": {
            "hostnameOrAddress": "localhost",
            "port": "12201",
            "transportType": "Udp"
        }
    }
    ]
  }
}
```

Note that because of the limitations of the Serilog.Settings.Configuration package, you cannot configure IGelfConverter using json. 

by default udp protocol is using, if you want to use http define sink options like 

```csharp
new GraylogSinkOptions
      {
          HostnameOrAdress = "http://localhost",
          Port = 12201,
          TransportType = TransportType.Http,
      }
```

All options you can see at https://github.com/whir1/serilog-sinks-graylog/blob/master/src/Serilog.Sinks.Graylog/GraylogSinkOptions.cs

You can create your own implementation of transports or converter and set it to options. But maybe i'll delete this feature in the future

PS this is my first package XD.

PPS I am sorry for my language, but my second language is C#
