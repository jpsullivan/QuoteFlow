﻿using System;
using System.Collections.Generic;
using System.Text;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Jql.Query.Order
{
    /// <summary>
    /// A simple data bean representing a portion of the sort order (related to a clause) for a search query.
    /// 
    /// Together via the <see cref="OrderBy"/> these will determine the sorting order of the results
    /// returned by a <see cref="Query"/>.
    /// </summary>
    [Serializable]
    public class SearchSort
    {
        public readonly SortOrder? Order;
        public string Field { get; private set; }
        public readonly IEnumerable<Property> Property;

        /// <summary>
        /// Used to construct a search sort for a field with a direction.
        /// </summary>
        /// <param name="field">The field to sort by.</param>
        /// <param name="order">The order direction to sort by, if null the default order for the field will be used.</param>
        public SearchSort(string field, SortOrder order = SortOrder.None)
            : this(field, null, order)
        {
        }

        /// <summary>
        /// Used to construct a search sort for a field with a direction and optional property </summary>
        /// <param name="field"> to sort by. </param>
        /// <param name="property"> property associated with sort filed this should be taken into consideration when constructing sort field </param>
        /// <param name="order"> direction to sort by, if null the default order for the field will be used. </param>
        public SearchSort(string field, IEnumerable<Property> property, SortOrder order)
        {
            Field = field;
            Order = order;
            Property = property;
        }

        /// <summary>
        /// Used to construct a search sort for a field with a direction.
        /// 
        /// NOTE: it would be better if the order of these parameters was reversed but we are leaving it for backward compatibility. </summary>
        /// @deprecated use <see cref="#SearchSort(String, SortOrder)"/> instead.
        /// 
        /// <param name="order">The order of the sort, if null, will be the default order for the system, if not one of
        /// <see cref="SortOrder#ASC"/> or <see cref="SortOrder#DESC"/> it will default to <see cref="SortOrder#ASC"/>.</param>
        /// <param name="field">To sort by.</param>
        public SearchSort(string order, string field)
            : this(field, null, SortOrderHelpers.ParseString(order))
        {
        }

        public SearchSort(SearchSort copy)
        {
            Field = copy.Field;
            Order = copy.Order;
            Property = copy.Property;
        }

        public string GetOrder()
        {
            return (Order == null) ? null : Order.Value.ToString();
        }

        public virtual bool Reverse
        {
            get { return Order == SortOrder.DESC; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Field);

            if (Property != null)
            {
                foreach (var prop in Property)
                {
                    sb.Append(prop);
                }
            }

            return sb.ToString();
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

            SearchSort that = (SearchSort)o;

            if (!Field.Equals(that.Field))
            {
                return false;
            }
            if (Order != that.Order)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = Order != null ? Order.GetHashCode() : 0;
            result = 31 * result + Field.GetHashCode();
            return result;
        }

    }
}