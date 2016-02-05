using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Comparator
{
    public class IdComparator : IComparer<string>
    {
        public static readonly IComparer<string> Comparator = new IdComparator();

        private IdComparator()
        {
        }

        public virtual int Compare(string key1, string key2)
        {
            if (key1 == key2)
            {
                return 0;
            }
            if (key1 == null)
            {
                return 1;
            }
            if (key2 == null)
            {
                return -1;
            }

            int index1 = key1.LastIndexOf('-');
            int index2 = key2.LastIndexOf('-');

            // issue key may not have project key
            // data imported from Bugzilla may not comply with atlassian issue key format
            // this added to make jira more fault tolerant
            if ((index1 == -1) && (index2 == -1))
            {
                return 0;
            }
            if (index1 == -1)
            {
                return 1;
            }
            if (index2 == -1)
            {
                return -1;
            }

            // compare the project part (up until the first '-')
            for (int i = 0; i < Math.Min(index1, index2); i++)
            {
                char c1 = key1[i];
                char c2 = key2[i];
                if (c1 != c2)
                {
                    // This is a different project, do unicode char comparison
                    return (c1 < c2) ? -1 : 1;
                }
            }

            // if one of the project parts is shorter than the other, that will be less than
            if (index1 != index2)
            {
                return (index1 < index2) ? -1 : 1;
            }

            // Same project, compare numbers
            return CompareNumPart(key1, key2);
        }

        private int CompareNumPart(string key1, string key2)
        {

            // As we know project part is the same, we can just compare using a
            // string comparator if they are the same length
            if (key1.Length == key2.Length)
            {
                return key1.CompareTo(key2);
            }
            // Else the longer one will be the bigger number
            return (key1.Length > key2.Length) ? 1 : -1;
        }
    }
}