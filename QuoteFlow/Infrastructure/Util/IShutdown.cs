namespace QuoteFlow.Infrastructure.Util
{
    /// <summary>
    /// Used to shut something down.
    /// </summary>
    public interface IShutdown
    {
        /// <summary>
        /// Shutdown. Should not throw any exceptions.
        /// </summary>
        void Shutdown();
    }
}