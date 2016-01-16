using System.ComponentModel;

namespace QuoteFlow.Api.Models.ViewModels.Layout
{
    /// <summary>
    /// A class for holding full-screen-specific layout options
    /// on the "_FullScreenWidthLayout", such as layout mode, 
    /// whether or not only the logo should be shown, etc.
    /// </summary>
    public class FullWidthLayoutViewModel
    {
        public FullWidthLayoutViewModel()
        {
            LogoOnly = false;
            LayoutType = LayoutType.Standard;
            ShowHeader = true;
            ShowFooter = true;
        }

        public FullWidthLayoutViewModel(LayoutType layoutType)
        {
            LogoOnly = false;
            LayoutType = layoutType;
            ShowHeader = true;
            ShowFooter = true;
        }

        public FullWidthLayoutViewModel(LayoutType layoutType, bool showFooter)
        {
            LogoOnly = false;
            LayoutType = layoutType;
            ShowHeader = true;
            ShowFooter = showFooter;
        }

        public FullWidthLayoutViewModel(bool logoOnly, LayoutType layoutType)
        {
            LogoOnly = logoOnly;
            LayoutType = layoutType;
            ShowHeader = true;
            ShowFooter = true;
        }

        public FullWidthLayoutViewModel(bool logoOnly, LayoutType layoutType, bool showHeader, bool showFooter)
        {
            LogoOnly = logoOnly;
            LayoutType = layoutType;
            ShowHeader = showHeader;
            ShowFooter = showFooter;
        }

        /// <summary>
        /// If true, only the logo is visible on the header. The 
        /// navigation links and the account sidebar are not shown.
        /// </summary>
        public bool LogoOnly { get; set; }

        /// <summary>
        /// The type of layout that should be displayed on the 
        /// full width layout.
        /// </summary>
        public LayoutType LayoutType { get; set; }

        /// <summary>
        /// Whether or not the header should be displayed. This should typically
        /// be disabled if a custom header is used within a view.
        /// </summary>
        public bool ShowHeader { get; set; }

        /// <summary>
        /// Whether or not the footer should be displayed.
        /// </summary>
        public bool ShowFooter { get; set; }

        /// <summary>
        /// The custom header properties if one should be displayed.
        /// </summary>
        public CustomHeader CustomHeader { get; set; }
    }

    public enum LayoutType
    {
        [Description("")]
        Standard,

        [Description("aui-page-focused aui-page-focused-small")]
        FocusedSmall,

        [Description("aui-page-focused aui-page-focused-medium")]
        FocusedMedium,

        [Description("aui-page-focused aui-page-focused-large")]
        FocusedLarge,

        [Description("aui-page-notification aui-page-size-small")]
        NotificationSmall,

        [Description("aui-page-notification aui-page-size-medium")]
        NotificationMedium,

        [Description("aui-page-notification aui-page-size-large")]
        NotificationLarge
    }
}