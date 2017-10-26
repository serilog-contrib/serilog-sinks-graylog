namespace Serilog.Sinks.Graylog.MessageBuilders
{
    public interface IPropertyNamingStrategy
    {
        string GetPropertyName(string property);
    }
}
