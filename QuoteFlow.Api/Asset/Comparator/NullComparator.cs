using System;
using System.Collections;

namespace QuoteFlow.Api.Asset.Comparator
{
    /// <summary>
    /// This comparator compares two given objects and is null safe.
    /// </summary>
    public class NullComparator : IComparer
    {
        /// <summary>
        /// Compares two given objects. Returns 0 if both objects are null, 1 if o2
        /// is null, -1 if o1 is null. In case when both objects are not null,
        /// returns the result of o1.compareTo(o2) as long as o1 implements
        /// Comparable, otherwise returns 0.
        /// <br/>
        /// Note that if o1 is an instance of <see cref="Comparable"/> and o2 is not of
        /// the same type may result in <see cref="ClassCastException"/>.
        /// </summary>
        /// <param name="o1"> object to compare </param>
        /// <param name="o2"> object to compare </param>
        /// <returns> result of comparison </returns>
        /// <exception cref="ClassCastException"> if o1 is an instance of <see cref="Comparable"/> and o2 is not of the same type </exception>
        public virtual int Compare(object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return 0;
            }
            if (o1 == null)
            {
                return -1;
            }
            if (o2 == null)
            {
                return 1;
            }
            if (o1 is IComparable)
            {
                return ((IComparable)o1).CompareTo(o2);
            }
            return 0;
        }
    }

}