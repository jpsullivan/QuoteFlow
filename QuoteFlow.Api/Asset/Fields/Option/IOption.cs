using System.Collections;

namespace QuoteFlow.Api.Asset.Fields.Option
{
    /// <summary>
    /// An option interface to wrap around other objects for display in select lists.
    /// </summary>
    public interface IOption
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        string ImagePath { get; set; }

        /// <summary>
        /// Returns the HTML-encoded image path for this Option.
        /// </summary>
        /// <returns> an HTML-encoded image path </returns>
        string ImagePathHtml { get; set; }

        string CssClass { get; set; }

        /// <summary>
        /// Returns a list of dependent child options. Returns empty list if no children </summary>
        /// <returns>  List of <seealso cref="Option"/> objects. (empty list if no children) </returns>
        IList ChildOptions { get; set; }
    }
}