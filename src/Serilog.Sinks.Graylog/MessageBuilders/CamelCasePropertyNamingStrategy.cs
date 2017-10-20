namespace Serilog.Sinks.Graylog.MessageBuilders
{
    public class CamelCasePropertyNamingStrategy : IPropertyNamingStrategy
    {
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

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);
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
