using System;

namespace QuoteFlow.Api.Models.ViewModels
{
    /// <summary>
    /// Provides a construct for getting and setting information about a particular sidebar. 
    /// The idea here is to be able to rely on this when determing if a menu item on the 
    /// sidebar should be selected.
    /// </summary>
    public class SidebarViewModel<T>
    {
        public string SelectedItem { get; set; }
        public string SelectedItemClass { get; set; }

        /// <summary>
        /// The class type that this model will be using to build links/info
        /// about the sidebar.  Such as <code>typeof(Quote)</code> for the Name property.
        /// </summary>
        public T Actor { get; set; }

        public SidebarViewModel(string selectedItem, T actor, string selectedItemClass = "aui-nav-selected")
        {
            SelectedItem = selectedItem;
            SelectedItemClass = selectedItemClass;
            Actor = actor;
        }

        public SidebarViewModel(string selectedItem, string selectedItemClass = "aui-nav-selected")
        {
            SelectedItem = selectedItem;
            SelectedItemClass = selectedItemClass;
        }
    }

    public static class SidebarViewModelExtensions
    {
        public static string GetMenuItemClass<T>(this SidebarViewModel<T> model, string expectedMenuItemName)
        {
            return String.Equals(model.SelectedItem, expectedMenuItemName, StringComparison.CurrentCultureIgnoreCase)
                ? model.SelectedItemClass
                : String.Empty;
        }
    }
}
