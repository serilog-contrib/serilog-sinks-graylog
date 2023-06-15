# Status

[![Build status](https://ci.appveyor.com/api/projects/status/yqi7f32mxrwtnrvv/branch/master?svg=true)](https://ci.appveyor.com/project/whir1/serilog-sinks-graylog-j9flc/branch/master)

### serilog-sinks-graylog

[![NuGet](https://img.shields.io/nuget/v/serilog.sinks.graylog.svg)](https://www.nuget.org/packages/serilog.sinks.graylog/)
[![Downloads](https://img.shields.io/nuget/dt/serilog.sinks.graylog.svg)](https://www.nuget.org/packages/serilog.sinks.graylog/)

### serilog-sinks-graylog-Batching

[![NuGet](https://img.shields.io/nuget/v/serilog.sinks.graylog.batching.svg)](https://www.nuget.org/packages/Serilog.Sinks.Graylog.Batching/)
[![Downloads](https://img.shields.io/nuget/dt/serilog.sinks.graylog.batching.svg)](https://www.nuget.org/packages/Serilog.Sinks.Graylog.Batching/)

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
          HostnameOrAddress = "localhost",
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
          HostnameOrAddress = "http://localhost",
          Port = 12201,
          TransportType = TransportType.Http,
      }
```

All options you can see at https://github.com/whir1/serilog-sinks-graylog/blob/master/src/Serilog.Sinks.Graylog.Core/GraylogSinkOptions.cs

You can create your own implementation of transports or converter and set it to options. But maybe i'll delete this feature in the future

PS this is my first package XD.

PPS I am sorry for my language, but my second language is C#
