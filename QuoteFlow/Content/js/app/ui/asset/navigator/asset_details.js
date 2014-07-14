QuoteFlow.UI.Asset.Navigator.AssetDetails = QuoteFlow.Views.Base.extend({
    el: ".detail-panel",

    templateName: 'asset/details',

    options: {},

    events: {
        "click .asset-list li": "assetClickHandler"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function (options) {
        this.model = new QuoteFlow.Model.Asset.Details();
        QuoteFlow.Vent.on('navigator:asset-details:load', this.loadAsset, this);

        this.model.bind("change", this.render, this);
    },

    postRenderTemplate: function() {
        console.log('uh oh');
    },

    /**
     * Fetches the asset details and renders the asset details view.
     */
    loadAsset: function (assetId) {
        this.$el.empty();
        //this.model = new QuoteFlow.Model.Asset.Details({ id: assetId });
        this.model.set({ id: assetId }, { silent: true });
        this.model.fetch();
    }
})