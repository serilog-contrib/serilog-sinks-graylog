namespace Serilog.Sinks.Graylog.MessageBuilders
{
    public class NoOpPropertyNamingStrategy : IPropertyNamingStrategy
    {
        public string GetPropertyName(string property)
        {
            return property;
        }
    }
}
