using System;
using System.Linq;

namespace QuoteFlow.Api.Jql.Values
{
    public class ClauseValuesGeneratorResult
    {
        public virtual string Value { get; private set; }
        public virtual string[] DisplayNameParts { get; private set; }

		public ClauseValuesGeneratorResult(string value)
		{
		    if (value == null)
		    {
                throw new ArgumentNullException(nameof(value));
		    }

            Value = value;
            DisplayNameParts = new[] { value };
		}

		public ClauseValuesGeneratorResult(string value, string displayName)
		{
		    if (value == null)
		    {
                throw new ArgumentNullException(nameof(value));
		    }

		    if (displayName == null)
		    {
                throw new ArgumentNullException(nameof(displayName));
		    }

            Value = value;
            DisplayNameParts = new[] { displayName };
		}

		/// <summary>
		/// Use this if you want to have multiple portions of your display string that will be concatenated with
		/// a space in between. Each portion of this display part will be anaylized for the matching string
		/// and will have bold tags around the matching parts.
		/// </summary>
		/// <param name="value"> the value that will be used to complete. </param>
		/// <param name="displayNameParts"> the parts of the display name that will be used to show the user what matched,
		/// will be searched for matching string. </param>
        public ClauseValuesGeneratorResult(string value, string[] displayNameParts)
		{
		    if (value == null)
		    {
                throw new ArgumentNullException(nameof(value));
		    }

		    if (displayNameParts == null)
		    {
                throw new ArgumentNullException(nameof(displayNameParts));
		    }

            Value = value;
            DisplayNameParts = displayNameParts;
		}

		public override string ToString()
		{
		    return string.Format("Result {{ value='{0}'\'', displayNameParts={1} }}", DisplayNameParts, (DisplayNameParts == null ? null : DisplayNameParts.ToList()));
		}
    }
}