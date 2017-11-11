using System;

namespace Serilog.Sinks.Graylog.Helpers
{
    public class LazyRetry<T>
    {
        private readonly Func<T> _valueFactory;
        private Lazy<T> _lazy;

        public LazyRetry(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
            _lazy = new Lazy<T>(valueFactory);
        }

        public T Value
        {
            get
            {
                try
                {
                    return _lazy.Value;
                }
                catch (Exception)
                {
                    _lazy = new Lazy<T>(_valueFactory);
                    throw;
                }
            }
        }
    }
}
