using System;
using System.Text.RegularExpressions;

using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Models.Search.Jql.Util
{
    /// <summary>
    /// A class that parses the Jql syntax (e.g. cf[1000]) for Custom Field identifiers.
    /// </summary>
    public sealed class JqlCustomFieldId
    {
        private static readonly Regex pattern = new Regex("\\s*cf\\s*\\[\\s*(\\d+)\\s*\\]\\s*", RegexOptions.IgnoreCase);
        private readonly long id;

        public JqlCustomFieldId(long id)
        {
            this.id = id;
        }

        public long Id
        {
            get
            {
                return id;
            }
        }

        public string JqlName
        {
            get
            {
                return ToString(id);
            }
        }

        public override string ToString()
        {
            return JqlName;
        }
        
        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var that = (JqlCustomFieldId) o;

            return id == that.id;

        }

        public override int GetHashCode()
        {
            return (int)(id ^ ((long)((ulong)id >> 32)));
        }

        public static string ToString(long id)
        {
            if (id < 0)
            {
                throw new ArgumentException("id should be >= 0");
            }

            return string.Format("cf[{0:D}]", id);
        }

        public static bool IsJqlCustomFieldId(string fieldName)
        {
            return ParseId(fieldName) >= 0;
        }

        public static JqlCustomFieldId ParseJqlCustomFieldId(string fieldName)
        {
            long fieldId = ParseId(fieldName);
            return fieldId >= 0 ? new JqlCustomFieldId(fieldId) : null;
        }

        public static long ParseId(string fieldName)
        {
            if (fieldName.IsNullOrEmpty())
            {
                return -1;
            }

            var matcher = pattern.Match(fieldName.Trim());
            if (matcher.Success)
            {
                try
                {
                    long longId = Convert.ToInt64(matcher.Groups[1]);
                    if (longId >= 0)
                    {
                        return longId;
                    }
                }
                catch (Exception ignored)
                {
                }
            }
            return long.MinValue;
        }
    }
}