using System;
using System.Collections;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace QuoteFlow.Api.Asset.Search.Parameters.Lucene.Sort
{
    /// <summary>
	/// This Sort Comparator uses a mixed strategy to retrieve the term values.  If we are retrieving less thatn 1% of the
	/// documents in the index, we get each document value individually.  Once passed the 1% threshhold however we switch
	/// to reading through the terms dictionary in lucene, and builds up a list of ordered terms.  It then
	/// sorts the documents according to the order that they appear in the terms list.
	/// This latter approach, whilst very fast, does load the entire term dictionary into memory, and allocates a slot
	/// for every document value, which is why we do not use it all the time.
	/// 
	/// We believe that most searches in very large JIRA installations
	/// will return a very small portion of the document index, either because they are over only one of
	/// many projects or they return only open issues, especially on dashboards and in GreenHopper and similar plugins as
	/// well as in general navigator searches.
	/// </summary>
	public class MappedSortComparator : FieldComparatorSource
	{
		private readonly ILuceneFieldSorter<object> sorter;

		public MappedSortComparator(ILuceneFieldSorter<object> sorter)
		{
			this.sorter = sorter;
		}

		public override FieldComparator NewComparator(string fieldname, int numHits, int sortPos, bool reversed)
		{
			return new InternalFieldComparator(this, numHits, fieldname, sorter);
		}

		public sealed class InternalFieldComparator : FieldComparator
		{
			private readonly MappedSortComparator outerInstance;

			internal object[] values;
			internal readonly int numHits;
			internal readonly string field;
			internal ILuceneFieldSorter<object> sorter;
			internal object bottom;
			internal readonly IComparer comparator;
			internal int resultsCount;
			internal int fastDocThreshold;
			internal IValueFinder hungryValueFinder;
			internal IValueFinder lazyValueFinder;

			internal InternalFieldComparator(MappedSortComparator outerInstance, int numHits, string field, ILuceneFieldSorter<object> sorter)
			{
				this.outerInstance = outerInstance;
				this.numHits = numHits;
				int initSize = Math.Min(1024, numHits);
				this.values = new object[initSize];
				this.field = field;
				this.sorter = sorter;
				this.comparator = this.sorter.Comparator as IComparer;
			}

			public override int Compare(int slot1, int slot2)
			{
				object v1 = values[slot1];
				object v2 = values[slot2];
				if (v1 == v2)
				{
					return 0;
				}
				if (v1 == null)
				{
					return 1;
				}
				else if (v2 == null)
				{
					return -1;
				}
				return comparator.Compare(v1, v2);
			}

		    public override void SetBottom(int slot)
		    {
		        this.bottom = values[slot];
		    }

		    public override int CompareBottom(int doc)
			{
				object v2 = GetDocumentValue(doc);
				if (bottom == v2)
				{
					return 0;
				}
				if (bottom == null)
				{
					return 1;
				}
				else if (v2 == null)
				{
					return -1;
				}
				return comparator.Compare(bottom, v2);
			}

		    public override void Copy(int slot, int doc)
		    {
		        EnsureCapacity(slot);
				values[slot] = GetDocumentValue(doc);
		    }

			internal void EnsureCapacity(int slot)
			{
				if (values.Length <= slot)
				{
					int newSize = Math.Min(numHits, values.Length * 2);
					if (newSize <= slot) // Just to re really sure we don't blow up here.
					{
						newSize = slot + 1;
					}
				    
                    Array.Copy(values, values, newSize);
					//values = Arrays.copyOf(values, newSize);
				}
			}

			internal object GetDocumentValue(int doc)
			{
				// We have 2 strategies for getting the document values
				// If we get a large number of results we walk the Terms in the Index and only  convert terms to values
				// once per term and then we store these for each document.
				// If we get only a few results the above method is very wasteful of both memory and compute time.
				// Unfortunatly we don't know how many results we will get in advance so we use a pair of strategies and flip
				// strategies once we hit a reasonalbe threshhold value
				resultsCount++;
				try
				{
					if (resultsCount > fastDocThreshold)
					{
						return hungryValueFinder.GetValue(doc);
					}
					else
					{
						return lazyValueFinder.GetValue(doc);
					}
				}
				catch (IOException e)
				{
					throw new Exception(e.Message);
				}
			}

		    public override void SetNextReader(IndexReader reader, int docBase)
		    {
		        resultsCount = 0;
				fastDocThreshold = reader.NumDocs() / 500;
				lazyValueFinder = new LazyValueFinder(outerInstance, reader, field);
				hungryValueFinder = new HungryValueFinder(outerInstance, reader, field);
		    }

		    public override IComparable this[int slot]
		    {
                // We won't be able to pull the values from the sort
				// This is only used by FiledDoc instances, which we do not use.
		        get { return null; }
		    }
		}

		/// <summary>
		/// This makes a call into the QuoteFlowLuceneFieldFinder to retrieve values from the Lucence index.  It returns an array
		/// that is the same size as the number of documents in the reader and will have all null values if the field is not
		/// present, otherwise it has the values of the field within the document.
		/// <p/>
		/// Broken out as package level for unit testing reasons.
		/// </summary>
		/// <param name="field"> the name of the field to find </param>
		/// <param name="reader"> the Lucence index reader </param>
		/// <returns> an non null array of values, which may contain null values. </returns>
		/// <exception cref="IOException"> if stuff goes wrong </exception>
		internal virtual object[] GetLuceneValues(string field, IndexReader reader)
		{
			return QuoteFlowLuceneFieldFinder.Instance.GetCustom(reader, field, this);
		}

		/// <summary>
		/// Returns an object which, when sorted according by the comparator returned from  {@link
		/// LuceneFieldSorter#getComparator()} , will order the Term values in the correct order. <p>For example, if the
		/// Terms contained integer values, this method would return <code>new Integer(termtext)</code>.  Note that this
		/// might not always be the most efficient implementation - for this particular example, a better implementation
		/// might be to make a ScoreDocLookupComparator that uses an internal lookup table of int.
		/// </summary>
		/// <param name="termtext"> The textual value of the term. </param>
		/// <returns> An object representing <code>termtext</code> that can be sorted by {@link
		///         LuceneFieldSorter#getComparator()} </returns>
		/// <see cref= Comparable </seealso>
		/// <see cref= FieldComparator </seealso>
		public virtual object GetComparable(string termtext)
		{
			return sorter.GetValueFromLuceneField(termtext);
		}

		public virtual IComparer Comparator
		{
			get
			{
				return sorter.Comparator as IComparer;
			}
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || this.GetType() != o.GetType())
			{
				return false;
			}

			var that = (MappedSortComparator) o;

			return (sorter == null ? that.sorter == null : sorter.Equals(that.sorter));
		}

		public override int GetHashCode()
		{
			return (sorter != null ? sorter.GetHashCode() : 0);
		}

        internal interface IValueFinder
		{
			object GetValue(int doc);
		}

		private class HungryValueFinder : IValueFinder
		{
			private readonly MappedSortComparator outerInstance;

			internal readonly IndexReader reader;
			internal readonly string field;
			internal bool initialised = false;
			internal object[] currentDocumentValues;

			internal HungryValueFinder(MappedSortComparator outerInstance, IndexReader reader, string field)
			{
				this.outerInstance = outerInstance;
				this.reader = reader;
				this.field = field;
			}

		    object IValueFinder.GetValue(int doc)
		    {
		        if (!initialised)
				{
					currentDocumentValues = outerInstance.GetLuceneValues(field, reader);
					initialised = true;
				}
				return currentDocumentValues[doc];
		    }
		}

		private class LazyValueFinder : IValueFinder
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				comparator = outerInstance.Comparator;
			}

			private readonly MappedSortComparator outerInstance;

			internal readonly IndexReader reader;
			internal readonly string field;
			internal readonly FieldSelector fieldSelector;
			internal IComparer comparator;
			internal int lastDoc = -1;
			internal object lastValue = null;

			internal LazyValueFinder(MappedSortComparator outerInstance, IndexReader reader, string field)
			{
				this.outerInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				this.reader = reader;
				this.field = field;
				fieldSelector = new FieldSelectorResolver(this);
			}

			private class FieldSelectorResolver : FieldSelector
			{
				private readonly LazyValueFinder _outerInstance;

				public FieldSelectorResolver(LazyValueFinder outerInstance)
				{
					_outerInstance = outerInstance;
				}

			    public FieldSelectorResult Accept(string fieldName)
			    {
			        return _outerInstance.field.Equals(fieldName) ? FieldSelectorResult.LOAD_AND_BREAK : FieldSelectorResult.NO_LOAD;
			    }
			}

		    object IValueFinder.GetValue(int doc)
		    {
                // Once the queue is full a call to compareBottom() is immediately followed by a call to copy() for
                // qualifying results, so we do a little caching here
                if (doc == lastDoc)
                {
                    return lastValue;
                }

                try
                {
                    Document document = reader.Document(doc, fieldSelector);
                    IFieldable[] values = document.GetFieldables(field);
                    object comparable = null;
                    
                    foreach (IFieldable fieldable in values)
                    {
                        object value = outerInstance.GetComparable(fieldable.StringValue);
                        if (comparable == null || comparator.Compare(value, comparable) < 1)
                        {
                            comparable = value;
                        }
                    }

                    lastDoc = doc;
                    lastValue = comparable;
                    return comparable;
                }
                catch (IOException e)
                {
                    throw new Exception(e.Message);
                }
		    }
		}

	}
}