﻿using System.Threading;

namespace QuoteFlow.Api.Util.Support
{
    public class AtomicInteger
    {
        private int value;

        public AtomicInteger()
            : this(0)
        {
        }

        public AtomicInteger(int value_)
        {
            Interlocked.Exchange(ref value, value_);
        }

        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref value);
        }

        public int GetAndIncrement()
        {
            return Interlocked.Increment(ref value) - 1;
        }

        public int DecrementAndGet()
        {
            return Interlocked.Decrement(ref value);
        }

        public int GetAndDecrement()
        {
            return Interlocked.Decrement(ref value) + 1;
        }

        public void Set(int value_)
        {
            Interlocked.Exchange(ref value, value_);
        }

        public int AddAndGet(int value_)
        {
            return Interlocked.Add(ref value, value_);
        }

        public int Get()
        {
            //LUCENE TO-DO read operations atomic in 64 bit
            return value;
        }

        public bool CompareAndSet(int expect, int update)
        {
            int rc = Interlocked.CompareExchange(ref value, update, expect);
            return rc == expect;
        }
    }
}