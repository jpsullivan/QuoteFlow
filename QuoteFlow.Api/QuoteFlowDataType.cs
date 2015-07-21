using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuoteFlow.Api
{
    /// <summary>
	/// Each data type can specify a collection of actual java types, represented via their 
	/// <see cref="Class"/>, that this type is.
	/// </summary>
	public class QuoteFlowDataType<T> : IQuoteFlowDataType where T : class
	{
		private readonly ICollection<Type> dataTypes;
		private readonly ICollection<string> stringTypes;

		public QuoteFlowDataType(ICollection<T> types)
		{
		    if (types == null)
		    {
		        throw new ArgumentNullException(nameof(types));
		    }

		    if (types.Count == 0)
		    {
		        throw new ArgumentException("Provided types cannot be empty", nameof(types));
		    }

		    dataTypes = new List<Type>((IList<Type>) types);
			var strTypes = new List<string>();
			foreach (var type in types)
			{
				strTypes.Add(type.ToString());
			}
			stringTypes = new Collection<string>(strTypes);
		}

		public ICollection<string> AsStrings()
		{
			return stringTypes;
		}

		public bool Matches(IQuoteFlowDataType otherType)
		{
		    if (otherType == null)
		    {
		        throw new ArgumentNullException(nameof(otherType));
		    }

		    if (otherType is QuoteFlowDataType<T>)
		    {
		        var other = (QuoteFlowDataType<T>) otherType;
		        if (dataTypes.Contains(typeof (object)) || other.dataTypes.Contains(typeof (object)))
		        {
		            return true;
		        }

		        return dataTypes.Any(type => other.dataTypes.Contains(type));
		    }

		    return false;
		}

		/// <summary>
		/// This is not provided on the interface, if you need it you must cast the object 
		/// to this implementation type.
		/// </summary>
		/// <returns>The <see cref="class"/>'s that this data type represents.</returns>
		public ICollection<Type> Types
		{
			get { return dataTypes; }
		}
	}

    public class QuoteFlowDataType : QuoteFlowDataType<Type>
    {
        public QuoteFlowDataType(Type type) : base(new Collection<Type>() { type })
        {
        }
    }
}