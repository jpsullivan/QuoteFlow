﻿QuoteFlow.UI.Asset.Navigator.AssetDetails = QuoteFlow.Views.Base.extend({
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
        _.bindAll(this, "adjustHeight");

        this.model = new QuoteFlow.Model.Asset.Details();
        this.model.bind("change", this.render, this);

        this.adjustHeight = _.debounce(this.adjustHeight);

        QuoteFlow.Interactive.onVerticalResize(this.adjustHeight);
        this.adjustHeight();

        QuoteFlow.Vent.on('navigator:asset-details:load', this.loadAsset, this);        
    },

    postRenderTemplate: function() {
        this.adjustHeight();
        this.showAsset = new QuoteFlow.UI.Asset.ShowAsset();
    },

    /**
     * Automatically adjusts the height of the details view
     * once the available window size changes (dev tools pops up or screen resizes).
     */
    adjustHeight: function() {
        _.defer(_.bind(function() {
            var container = this.getAssetContainer(), offset;
            if (container.length) {
                offset = container.length && container.offset().top;
                container.css("height", window.innerHeight - offset);
            }
        }, this));
    },

    /**
     * Returns the asset container element.
     */
    getAssetContainer: function() {
        return this.$el.find(".asset-container");
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