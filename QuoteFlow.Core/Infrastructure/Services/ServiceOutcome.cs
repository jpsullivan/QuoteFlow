using System;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Util;

namespace QuoteFlow.Core.Infrastructure.Services
{
    /// <summary>
    /// Generic service outcome that can optionally hold a value.
    /// </summary>
    public class ServiceOutcome<T> : IServiceResult, IServiceOutcome<T>
    {
        public IErrorCollection ErrorCollection { get; private set; }

        /// <summary>
		/// Convenience method that returns a new ServiceOutcome instance containing no errors, and with the provided
		/// returned value.
		/// </summary>
		/// @param <T> the type of the returned value</param>
		/// <param name="returnedValue"> the returned value </param>
		/// <returns> a new ServiceOutcome </returns>
		public static ServiceOutcome<T> Ok(T returnedValue)
		{
			return new ServiceOutcome<T>(new SimpleErrorCollection(), returnedValue);
		}

		/// <summary>
		/// Convenience method that returns a new ServiceOutcome instance with the errors from the passed outcome.
		/// </summary>
		/// @param <T> the type of the returned value </param>
		/// <param name="outcome"> the outcome whose errors we are taking. </param>
		/// <returns> a new ServiceOutcome </returns>
		public static ServiceOutcome<T> Error(ServiceOutcome<T> outcome)
		{
			return From(outcome.ErrorCollection);
		}

		/// <summary>
		/// Convenience method that returns a new ServiceOutcome instance containing the provided error message, and no
		/// return value.
		/// </summary>
		/// @param <T> the type of the returned value </param>
		/// <param name="errorMessage"> the error message to include in the ServiceOutcome </param>
		/// <returns> a new ServiceOutcome </returns>
		public static ServiceOutcome<T> Error(string errorMessage)
		{
		    var errors = new SimpleErrorCollection();
		    errors.AddErrorMessage(errorMessage);
		    return new ServiceOutcome<T>(errors);
		}

		/// <summary>
		/// Convenience method that returns a new ServiceOutcome containing the given errors and returned value.
		/// </summary>
		/// <param name="errorCollection"> an ErrorCollection </param>
		/// <param name="value"> the returned value </param>
		/// @param <T> the type of the returned value </param>
		/// <returns> a new ServiceOutcome instance </returns>
		public static ServiceOutcome<T> From(IErrorCollection errorCollection, T value)
		{
			return new ServiceOutcome<T>(errorCollection, value);
		}

		/// <summary>
		/// Convenience method that returns a new ServiceOutcome containing the given errors and null return value.
		/// </summary>
		/// <param name="errorCollection"> an ErrorCollection </param>
		/// @param <T> the type of the returned value </param>
		/// <returns> a new ServiceOutcome instance </returns>
		public static ServiceOutcome<T> From(IErrorCollection errorCollection)
		{
            return new ServiceOutcome<T>(errorCollection);
		}

		/// <summary>
		/// The wrapped result.
		/// </summary>
		private readonly T _value;

		/// <summary>
		/// Creates a new ServiceOutcome with the given errors. The returned value will be set to null.
		/// </summary>
		/// <param name="errorCollection"> an ErrorCollection </param>
		public ServiceOutcome(IErrorCollection errorCollection) : this(errorCollection, null)
		{
		}

		/// <summary>
		/// Creates a new ServiceOutcome with the given errors and returned value.
		/// </summary>
		/// <param name="errorCollection"> an ErrorCollection </param>
		/// <param name="value"> the wrapped value </param>
		public ServiceOutcome(IErrorCollection errorCollection, T value)
		{
			_value = value;
		}

		/// <summary>
		/// Returns the value that was returned by the service, or null.
		/// </summary>
		/// <returns> the value returned by the service, or null </returns>
		public virtual T ReturnedValue
		{
			get { return _value; }
		}

		/// <summary>
		/// Returns the value that was returned by the service, or null.
		/// </summary>
		/// <returns> the value returned by the service, or null </returns>
		public T Get()
		{
			return _value;
		}

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}