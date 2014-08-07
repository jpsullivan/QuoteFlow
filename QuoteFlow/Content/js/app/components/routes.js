QuoteFlow.Routes = {
    
    ///////////////////////////
    /// ASSET ROUTES
    ///////////////////////////
    asset: function (routeValues) {
        var assetId = routeValues.hash.id;
        var assetName = routeValues.hash.name;

        return '{0}asset/{1}/{2}'.f(QuoteFlow.RootUrl, assetId, QuoteFlow.Utilities.urlFriendly(assetName));
    },

    editAsset: function (routeValues) {
        var assetId = routeValues.hash.id;
        var assetName = routeValues.hash.name;

        return '{0}asset/{1}/{2}/edit'.f(QuoteFlow.RootUrl, assetId, QuoteFlow.Utilities.urlFriendly(assetName));
    },

    ///////////////////////////
    /// MANUFACTURER ROUTES
    //////////////////////////
    manufacturer: function(routeValues) {
        var manufacturerId = routeValues.hash.id;
        var manufacturerName = routeValues.hash.name;

        return '{0}manufacturer/{1}/{2}'.f(QuoteFlow.RootUrl, manufacturerId, QuoteFlow.Utilities.urlFriendly(manufacturerName));
    },

    editManufacturer: function(routeValues) {
        var manufacturerId = routeValues.hash.id;
        var manufacturerName = routeValues.hash.name;

        return '{0}manufacturer/{1}/{2}/edit'.f(QuoteFlow.RootUrl, manufacturerId, QuoteFlow.Utilities.urlFriendly(manufacturerName));
    }
}