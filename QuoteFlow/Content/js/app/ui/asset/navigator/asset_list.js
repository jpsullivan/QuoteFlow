QuoteFlow.UI.Asset.Navigator.AssetList = QuoteFlow.Views.Base.extend({
    el: ".list-results-panel",

    options: {},

    events: {
        "click .asset-list li": "assetClickHandler"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function (options) {},

    postRenderTemplate: function () { },

    /**
     * Selects an asset from the left navigation, loading its info
     * and rendering it on the details panel.
     */
    assetClickHandler: function (e) {
        e.preventDefault();
        var el = $(e.currentTarget);
        this.$('.asset-list li').removeClass('focused');
        el.addClass('focused');

        var assetId = parseInt(el.data('id'), 10);

        QuoteFlow.Vent.trigger('navigator:asset-details:load', assetId);
    }
})