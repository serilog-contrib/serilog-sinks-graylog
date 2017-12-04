using System;

namespace Serilog.Sinks.Graylog.Helpers
{
    public class LazyRetry<T>
    {
        private readonly Func<T> valueFactory;
        private Lazy<T> lazy;

        public LazyRetry(Func<T> valueFactory)
        {
            this.valueFactory = valueFactory;
            lazy = new Lazy<T>(valueFactory);
        }

        public bool Created { get; private set; }

        public T Value
        {
            get
            {
                try
                {
                    var result = lazy.Value;
                    Created = true;
                    return result;
                }
                catch (Exception)
                {
                    lazy = new Lazy<T>(valueFactory);
                    throw;
                }
            }
        }
    }
}
