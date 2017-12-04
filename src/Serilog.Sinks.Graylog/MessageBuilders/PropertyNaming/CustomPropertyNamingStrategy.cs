using System;

namespace Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming
{
    /// <summary>
    /// Apply Func to property name
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming.IPropertyNamingStrategy" />
    public class CustomPropertyNamingStrategy : IPropertyNamingStrategy
    {
        private readonly Func<string, string> customConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyNamingStrategy"/> class.
        /// </summary>
        /// <param name="customConverter">The custom converter.</param>
        public CustomPropertyNamingStrategy(Func<string, string> customConverter)
        {
            this.customConverter = customConverter;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public string GetPropertyName(string property)
        {
            return customConverter(property);
        }
    }
}