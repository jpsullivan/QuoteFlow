QuoteFlow.Model.AssetVarValue = Backbone.Model.extend({

    url: function () {
        return QuoteFlow.RootUrl + "api/assetvarvalue";
    },

    defaults: function () {
        return {
            Id: null,
            AssetId: 0,
            VarValue: "",
            AssetVarId: 0,
            OrganizationId: 0
        }
    }
})