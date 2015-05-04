namespace QuoteFlow.Api.Util
{
    /// <summary>
    /// A Supplier of objects of a single type. Semantically, this could be a
    /// Factory, Generator, Builder, Closure, Producer or something else entirely. No
    /// guarantees are implied by this interface. Implementations may return null if
    /// no objects are available, can optionally block until elements are available
    /// or throw <see cref="NoSuchElementException"/>.
    /// 
    /// Thread safety of a Supplier is not mandated by this interface, although
    /// serious care and consideration should be taken with any implementations that
    /// are not.
    /// </summary>
    public interface ISupplier<out T>
    {
        /// <summary>
        /// Produce an object. Retrieve an instance of the appropriate type. The
        /// returned object may or may not be a new instance, depending on the
        /// implementation.
        /// </summary>
        /// <returns> the product, may be null if there are no objects available. </returns>
        /// <exception cref="NoSuchElementException"> if the supply has been exhausted. </exception>
        T Get();
    }
}