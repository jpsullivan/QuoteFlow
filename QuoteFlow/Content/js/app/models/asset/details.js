QuoteFlow.Model.Asset.Details = Backbone.Model.extend({

    urlRoot: function () {
        return QuoteFlow.RootUrl + "api/asset";
    },

    defaults: function () {
        return {
            Id: null,
            Name: "",
            SKU: "",
            Type: "",
            Description: "",
            LastUpdated: null,
            CreationDate: null,
            Cost: null,
            Markup: null,
            Price: null,
            Creator: {},
            Manufacturer: {},
            Comments: {}
        };
    }
})