namespace Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming
{
    /// <summary>
    /// Can transform property name
    /// </summary>
    public interface IPropertyNamingStrategy
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        string GetPropertyName(string property);
    }
}
