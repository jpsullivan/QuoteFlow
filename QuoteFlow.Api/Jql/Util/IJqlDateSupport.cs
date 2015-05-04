using System;

namespace QuoteFlow.Api.Jql.Util
{
    /// <summary>
	/// Interface that helps with date parsing and validation in JQL.
	/// </summary>
	public interface IJqlDateSupport
	{
		/// <summary>
		/// Try to parse the passed date string using the formats that JQL understands. It will consider the user's time zone
		/// when parsing the date string.
		/// </summary>
		/// <param name="dateString"> the string to parse. Cannot be null. </param>
		/// <returns> the parsed date or null if it cant be parsed.  You did call <see cref="Validate(String)"/> right? </returns>
		/// <exception cref="IllegalArgumentException"> if the passed dateString is null. </exception>
		DateTime ConvertToDate(string dateString);

		/// <summary>
		/// Try to parse the passed date string using the formats that JQL understands. It will use the passed time zone
		/// when parsing the date string.
		/// </summary>
		/// <param name="dateString"> the string to parse. Cannot be null. </param>
		/// <param name="timeZone"> time zone to use when parsing. </param>
		/// <returns> the parsed date or null if it cant be parsed.  You did call <see cref="Validate(String)"/> right? </returns>
		/// <exception cref="IllegalArgumentException"> if the passed dateString is null. </exception>
		DateTime ConvertToDate(string dateString, TimeZone timeZone);

		/// <summary>
		/// Try to parse the passed in date string using the formats that JQL understands. It will consider the user's time
		/// zone when parsing the date string.
		/// <p/>
		/// It will eamine the single input string and use the implied precision to create the range
		/// <p/>
		/// If you provide only a year/month/day it will have a precision of 24 hours, ie from the start of the day to the
		/// end of the day
		/// <p/>
		/// If you supply year/month/day hour/minute, it will have a precision of 1 minute, ie from the start of the minute
		/// to the end of the minute.
		/// </summary>
		/// <param name="dateString"> the string to parse. Cannot be null. </param>
		/// <returns> the parsed datetime as a range using the implied precision.  Or null if the date cant be parsed. You did
		///         call <see cref="Validate(String)"/> right? </returns>
		/// <exception cref="IllegalArgumentException"> if the passed dateString is null. </exception>
		DateRange ConvertToDateRangeWithImpliedPrecision(string dateString);

		/// <summary>
		/// Converts the long to a date.
		/// </summary>
		/// <param name="dateLong"> the long to give back a date for . Cannot be null. </param>
		/// <returns> the parsed date. </returns>
		/// <exception cref="IllegalArgumentException"> if the passed in Long is null </exception>
		DateTime ConvertToDate(long? dateLong);

		/// <summary>
		/// Converts the long to a date range where both values equal each other.
		/// </summary>
		/// <param name="dateLong"> the long to give back a date for . Cannot be null. </param>
		/// <returns> the parsed date twice.  Mostly for symmetry to calling code since JQL can have both long and string
		///         representations of values </returns>
		/// <exception cref="IllegalArgumentException"> if the passed in Long is null </exception>
		DateRange ConvertToDateRange(long? dateLong);

		/// <summary>
		/// Converts a date into the index-friendly format.
		/// </summary>
		/// <param name="date"> the date </param>
		/// <returns> a string representing the date, ready for comparison to indexed values. </returns>
		string GetIndexedValue(DateTime date);

		/// <summary>
		/// Check to see if the passed string is a valid date according to JQL.
		/// </summary>
		/// <param name="dateString"> the string to check cannot be null. </param>
		/// <returns> true if the date is valid; false otherwise. </returns>
		/// <exception cref="IllegalArgumentException"> if the passed dateString is blank or null </exception>
		bool Validate(string dateString);

		/// <summary>
		/// Return a string representation of the passed date. This method should just convert the date into its parseable
		/// String representation. The user's time zone will be used when formatting the date string.
		/// </summary>
		/// <param name="date"> the date to convert. Cannot be null. </param>
		/// <returns> return the passed date as a string. </returns>
		/// <exception cref="IllegalArgumentException"> if the passed date is null. </exception>
		string GetDateString(DateTime date);

		/// <summary>
		/// Return a string representation of the passed date. This method should just convert the date into its parseable
		/// String representation. The passed time zone will be used when formatting the date string.
		/// </summary>
		/// <param name="date"> the date to convert. Cannot be null. </param>
		/// <param name="timeZone"> time zone to use. Cannot be null. </param>
		/// <returns> return the passed date as a string. </returns>
		/// <exception cref="IllegalArgumentException"> if the passed date is null. </exception>
		string GetDateString(DateTime date, TimeZone timeZone);

		/// <summary>
		/// Returns a boolean value indicating whether the passed date string representation has duration format, e.g., 4d 1h, -1w.
		/// </summary>
		/// <param name="dateString"> the string to parse. </param>
		/// <returns> true if the passed date string has duration format, false otherwise. </returns>
		bool IsDuration(string dateString);
	}
}