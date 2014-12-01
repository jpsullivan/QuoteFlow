using System;
using System.Diagnostics;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace QuoteFlow.Models.Assets.Search.Parameters.Lucene.Sort
{
    /// <summary>
    /// This supplies a Low Memory variant of Lucene's StringOrdValComparator.
    /// We have this modified class as we believe that most searches in very large QuoteFlow installations
    /// will return a very small portion of the document index, either because they are over only one of
    /// many projects or they return only open issues, especially on dashboards and in GreenHopper and similar plugins as
    /// well as in general navigator searches.
    /// </summary>
    public class StringSortComparator : FieldComparatorSource
    {
        public StringSortComparator()
        {
        }

        public override FieldComparator NewComparator(string fieldname, int numHits, int sortPos, bool reversed)
        {
            return new StringOrdValComparator(numHits, fieldname);
        }

        public sealed class StringOrdValComparator : FieldComparator
		{
			internal int[] ords;
            internal int[] readerGen;

			internal int currentReaderGen = -1;
			internal string[] lookup;
			internal int[] order;

            internal int bottomOrd;
			internal bool bottomSameReader;
			internal string bottomValue;
			internal readonly int numHits;

            public string[] Values { get; internal set; }
            public int BottomSlot { get; internal set; }
            public string Field { get; private set; }

			public StringOrdValComparator(int numHits, string field)
			{
			    BottomSlot = -1;
			    /* Altassian patch */
				/* Only allocate small arrays initially */
				this.numHits = numHits;
				int initSize = Math.Min(1024, numHits);
				ords = new int[initSize];
				Values = new string[initSize];
				readerGen = new int[initSize];
				/* Altassian patch end */
				Field = field;
			}

			/// <summary>
			/// Atlassian patch to dynamically increase the size of the arrays here as we need them.
			/// </summary>
			/// <param name="slot"> Slot to make sure we have capacity to store. </param>
			internal void EnsureCapacity(int slot)
			{
				if (Values.Length <= slot)
				{
					int newSize = Math.Min(numHits, Values.Length * 2);
					if (newSize <= slot) // Just to re really sure we don't blow up here.
					{
						newSize = slot + 1;
					}

                    Values.CopyTo(Values, newSize);
                    ords.CopyTo(ords, newSize);
                    readerGen.CopyTo(readerGen, newSize);
				}
			}

            public override int Compare(int slot1, int slot2)
            {
                if (readerGen[slot1] == readerGen[slot2])
				{
					return ords[slot1] - ords[slot2];
				}

				string val1 = Values[slot1];
				string val2 = Values[slot2];
				if (val1 == null)
				{
					if (val2 == null)
					{
						return 0;
					}
					return -1;
				}
			    if (val2 == null)
			    {
			        return 1;
			    }
			    return String.Compare(val1, val2, StringComparison.Ordinal);
            }

            public override void SetBottom(int slot)
            {
                BottomSlot = slot;
    
				bottomValue = Values[BottomSlot];
				if (currentReaderGen == readerGen[BottomSlot])
				{
					bottomOrd = ords[BottomSlot];
					bottomSameReader = true;
				}
				else
				{
					if (bottomValue == null)
					{
						ords[BottomSlot] = 0;
						bottomOrd = 0;
						bottomSameReader = true;
						readerGen[BottomSlot] = currentReaderGen;
					}
					else
					{
                        int index = BinarySearch(lookup, bottomValue);
						if (index < 0)
						{
							bottomOrd = -index - 2;
							bottomSameReader = false;
						}
						else
						{
							bottomOrd = index;
							// exact value match
							bottomSameReader = true;
							readerGen[BottomSlot] = currentReaderGen;
							ords[BottomSlot] = bottomOrd;
						}
					}
				}
            }

            public override int CompareBottom(int doc)
            {
                Debug.Assert(BottomSlot != -1);
				if (bottomSameReader)
				{
					// ord is precisely comparable, even in the equal case
					return bottomOrd - this.order[doc];
				}
                // ord is only approx comparable: if they are not
                // equal, we can use that; if they are equal, we
                // must fallback to compare by value
                int order = this.order[doc];
                int cmp = bottomOrd - order;
                if (cmp != 0)
                {
                    return cmp;
                }

                string val2 = lookup[order];
                if (bottomValue == null)
                {
                    if (val2 == null)
                    {
                        return 0;
                    }
                    // bottom wins
                    return -1;
                }
                if (val2 == null)
                {
                    // doc wins
                    return 1;
                }
                return String.Compare(bottomValue, val2, StringComparison.Ordinal);
            }

            public override void Copy(int slot, int doc)
            {
                int ord = order[doc];

				/* Altassian patch */
				EnsureCapacity(slot);
				/* Altassian patch - end */
				ords[slot] = ord;
				Debug.Assert(ord >= 0);
				Values[slot] = lookup[ord];
				readerGen[slot] = currentReaderGen;
            }

            public override void SetNextReader(IndexReader reader, int docBase)
            {

                var currentReaderValues = FieldCache_Fields.DEFAULT.GetStringIndex(reader, Field);
				currentReaderGen++;
				order = currentReaderValues.order;
				lookup = currentReaderValues.lookup;
				Debug.Assert(lookup.Length > 0);
				if (BottomSlot != -1)
				{
					Bottom = BottomSlot;
				}
            }

            public int Bottom
            {
                set
                {
                    BottomSlot = value;

                    bottomValue = Values[BottomSlot];
                    if (currentReaderGen == readerGen[BottomSlot])
                    {
                        bottomOrd = ords[BottomSlot];
                        bottomSameReader = true;
                    }
                    else
                    {
                        if (bottomValue == null)
                        {
                            ords[BottomSlot] = 0;
                            bottomOrd = 0;
                            bottomSameReader = true;
                            readerGen[BottomSlot] = currentReaderGen;
                        }
                        else
                        {
                            int index = BinarySearch(lookup, bottomValue);
                            if (index < 0)
                            {
                                bottomOrd = -index - 2;
                                bottomSameReader = false;
                            }
                            else
                            {
                                bottomOrd = index;
                                // exact value match
                                bottomSameReader = true;
                                readerGen[BottomSlot] = currentReaderGen;
                                ords[BottomSlot] = bottomOrd;
                            }
                        }
                    }
                }
            }

            public override IComparable this[int slot]
            {
                get { return Values[slot]; }
            }
		}
    }
}