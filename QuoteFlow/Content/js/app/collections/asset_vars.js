QuoteFlow.Collection.AssetVars = Backbone.Collection.extend({

    model: QuoteFlow.Model.AssetVar,

    url: function() {
        return QuoteFlow.RootUrl + "api/assetvar";
    }
});