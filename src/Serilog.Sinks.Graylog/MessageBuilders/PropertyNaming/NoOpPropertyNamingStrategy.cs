namespace Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming
{
    /// <summary>
    /// Do nothing
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming.IPropertyNamingStrategy" />
    public class NoOpPropertyNamingStrategy : IPropertyNamingStrategy
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public string GetPropertyName(string property)
        {
            return property;
        }
    }
}
