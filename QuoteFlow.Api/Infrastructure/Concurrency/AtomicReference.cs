using System;

namespace QuoteFlow.Api.Infrastructure.Concurrency
{
    public class AtomicReference<T>
    {
        protected T AtomicValue;

        public T Value
        {
            get
            {
                lock (this)
                {
                    return AtomicValue;
                }
            }
            set
            {
                lock (this)
                {
                    AtomicValue = value;
                }
            }
        }

        public AtomicReference()
        {
            AtomicValue = default(T);
        }

        public AtomicReference(T defaultValue)
        {
            AtomicValue = defaultValue;
        }

        public T GetAndSet(T value)
        {
            lock (this)
            {
                T ret = AtomicValue;
                AtomicValue = value;
                return ret;
            }
        }
    }

    public class Atomic<T> : AtomicReference<T> where T : IComparable
    {
        public Atomic() : base()
        {
        }

        public Atomic(T defaultValue) : base(defaultValue)
        {
        }

        public bool CompareAndSet(T expected, T newValue)
        {
            lock (this)
            {
                if (AtomicValue.CompareTo(expected) != 0)
                {
                    return false;
                }

                AtomicValue = newValue;
                return true;
            }
        }
    }
}
