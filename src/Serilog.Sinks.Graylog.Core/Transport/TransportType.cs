namespace Serilog.Sinks.Graylog.Core.Transport
{
    public enum TransportType
    {
        Udp,
        Http,
        Tcp,
        // Custom implementations of transports
        Custom
    }
}