namespace Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming
{
    /// <summary>
    /// О_о
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming.IPropertyNamingStrategy" />
    public class CamelCasePropertyNamingStrategy : IPropertyNamingStrategy
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public string GetPropertyName(string property)
        {
            var camelCased = ToCamelCase(property);
            return camelCased;
        }

        private static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            var chars = s.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                var hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    break;
                }

                char c;
                c = char.ToLowerInvariant(chars[i]);
                chars[i] = c;
            }

            return new string(chars);
        }
    }
}
