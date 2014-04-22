using System.Collections.Generic;

namespace QuoteFlow.Models.ViewModels
{
    public class NavigationMenuViewModel
    {
        public NavigationMenuViewModel(List<SubHeaderViewData> navigationMenu, List<SubHeaderViewData> navigationSubMenu)
        {
            NavigationMenu = navigationMenu;
            NavigationSubMenu = navigationSubMenu;
        }

        public List<SubHeaderViewData> NavigationMenu { get; private set; }
        public List<SubHeaderViewData> NavigationSubMenu { get; set; }
    }
}