using System;

namespace QuoteFlow.Models.Assets.Fields
{
    public interface IField : IComparable
    {
        /// <summary>
        /// The unique id of the field
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The i18n key that is used to lookup the field's name when it is displayed
        /// </summary>
        string NameKey { get; }

        /// <summary>
        /// Returns i18n'ed name of the field.
        /// </summary>
        string Name { get; }
    }
}