using System;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    /// <summary>
    /// Simple helper class that generates navigator param and form names given a date field id.
    /// 
    /// @since v4.0
    /// </summary>
    public sealed class DateSearcherConfig
    {
        public const string AFTER_SUFFIX = ":after";
        public const string BEFORE_SUFFIX = ":before";
        public const string NEXT_SUFFIX = ":next";
        public const string PREVIOUS_SUFFIX = ":previous";
        public const string EQUALS_SUFFIX = ":equals";

        public DateSearcherConfig(string id, ClauseNames clauseNames, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("id cannot be blank.", "id");
            }

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("fieldName cannot be blank.", "fieldName");
            }

            if (clauseNames == null)
            {
                throw new ArgumentNullException("clauseNames");
            }

            Id = id;
            FieldName = fieldName;
            ClauseNames = clauseNames;
            AfterField = Id + AFTER_SUFFIX;
            BeforeField = Id + BEFORE_SUFFIX;
            NextField = Id + NEXT_SUFFIX;
            PreviousField = Id + PREVIOUS_SUFFIX;
            EqualsField = Id + EQUALS_SUFFIX;
        }

        public ClauseNames ClauseNames { get; private set; }

        /// <returns> the name of this date field e.g. <code>created</code> or <code>My Custom Date Field</code> </returns>
        public string FieldName { get; private set; }

        public string Id { get; private set; }

        public string[] AbsoluteFields
        {
            get { return new[] {AfterField, BeforeField, EqualsField}; }
        }

        public string AfterField { get; private set; }

        public string BeforeField { get; private set; }

        public string[] RelativeFields
        {
            get { return new[] {PreviousField, NextField}; }
        }

        public string NextField { get; private set; }

        public string PreviousField { get; private set; }

        public string EqualsField { get; private set; }
    }

}