﻿namespace QuoteFlow.Infrastructure
{
    public static class RouteNames
    {
        public const string Home = "Home";
        public const string LogOn = "Login";
        public const string Register = "Register";

        public const string ExternalAuthentication = "ExternalAuthentication";
        public const string ExternalAuthenticationCallback = "ExternalAuthenticationCallback";
        public const string RemoveCredential = "RemoveCredential";
        public const string RemovePassword = "RemovePassword";
        public const string ConfirmAccount = "ConfirmAccount";
        public const string SubscribeToEmails = "SubscribeToEmails";
        public const string UnsubscribeFromEmails = "UnsubscribeFromEmails";

        public const string Error500 = "Error500";
        public const string Error404 = "Error404";

        public const string Dashboard = "Dashboard";

        public const string AssetNew = "Asset-New";

        public const string CatalogShow = "Catalog-Show";

        public const string Customers = "Contacts";
        public const string CustomerNew = "Contact-New";
        public const string CustomerQuotes = "Contact-ShowQuotes";
        public const string CustomerShow = "Contact-Show";

        public const string QuoteAccessControl = "Quote-AccessControl";
        public const string QuoteBuilder = "Quote-Builder";
        public const string QuoteBuilderWithSelectedAsset = "Quote-Builder-With-Sel-Asset";
        public const string QuoteChangeHistory = "Quote-ChangeHistory";
        public const string QuoteIndex = "Quote-Index";
        public const string QuoteLineItems = "Quote-LineItems";
        public const string QuoteShow = "Quote-Show";
    }
}