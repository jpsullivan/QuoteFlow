using System.Threading;

namespace QuoteFlow.Infrastructure.Concurrency
{
    public class AtomicReference<T> where T : class
    {
        private T _value;

        public AtomicReference() { }

        public AtomicReference(T value)
        {
            OptimisticSet(value);
        }

        public T CompareAndSet(T newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
        }

        public void OptimisticSet(T newValue)
        {
            while (_value == CompareAndSet(newValue)) ;
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            _value = value;
        }
    }
}