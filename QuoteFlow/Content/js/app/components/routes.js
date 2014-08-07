QuoteFlow.Routes = {
    
    ///////////////////////
    /// ASSET ROUTES
    //////////////////////
    asset: function (routeValues) {
        var assetId = routeValues.hash.id;
        var assetName = routeValues.hash.name;

        return '{0}asset/{1}/{2}'.f(QuoteFlow.RootUrl, assetId, QuoteFlow.Utilities.urlFriendly(assetName));
    },

    editAsset: function (routeValues) {
        var assetId = routeValues.hash.id;
        var assetName = routeValues.hash.name;

        return '{0}asset/{1}/{2}/edit'.f(QuoteFlow.RootUrl, assetId, QuoteFlow.Utilities.urlFriendly(assetName));
    }
}