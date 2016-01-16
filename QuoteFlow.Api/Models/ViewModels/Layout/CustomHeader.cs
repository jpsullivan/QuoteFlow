using System;

namespace QuoteFlow.Api.Models.ViewModels.Layout
{
    /// <summary>
    /// A set of properties that can be used to render a custom header
    /// in place of the existing navigation, or something under the nav.
    /// </summary>
    public class CustomHeader
    {
        public CustomHeader()
        {
            HeaderImage = null;
            HeaderLink = null;
            BackgroundColor = string.Empty;
            PaddingCss = "0";
        }

        /// <summary>
        /// A path to an image file.
        /// </summary>
        public Uri HeaderImage { get; set; }

        /// <summary>
        /// If this has a value, it will wrap the image in a hyperlink
        /// that will redirect to the specified Uri on click.
        /// </summary>
        public Uri HeaderLink { get; set; }

        /// <summary>
        /// The background color that the header image will be 
        /// rendered with. Leave null/empty for transparent.
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// If you wish to have some extra padding along with the logo,
        /// do so here by specifying it as you would in CSS. 
        /// So for example, if you wanted 20px padding on the top and bottom,
        /// this should be "20px 0". No semicolons are needed.
        /// </summary>
        public string PaddingCss { get; set; }

        /// <summary>
        /// The border color that should be displayed below the custom header.
        /// No, you can't specify the border width, because I'm a really mean person.
        /// Death to >1px borders.
        /// </summary>
        public string BorderBottomColor { get; set; }
    }
}