QuoteFlow.UI.Asset.AssetNavigator = QuoteFlow.Views.Base.extend({

    el: ".navigator-content",

    options: {},

    events: {},

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function(options) {
        this.initializeAssetListSidebar();
    },

    postRenderTemplate: function () { },

    /**
     * Initializes the asset list view as a draggable
     * sidebar. Sets the width.
     */
    initializeAssetListSidebar: function() {
        this.$el.find(".list-results-panel").sidebar({
            id: "splitview",
            minWidth: function(b) {
                return 250;
            },
            maxWidth: _.bind(function() {
                return this.$el[0].clientWidth - 500;
            }, this),
            resize: this.applyResponsiveDesign
        });
    },

    /**
     * Sets the width (position) of the sidebar.
     */
    updateAssetListSidebarPosition: function() {
        var b = this.$el.find(".list-results-panel");
        b.sidebar("updatePosition");
    }
})