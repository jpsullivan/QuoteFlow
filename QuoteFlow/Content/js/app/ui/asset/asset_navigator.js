QuoteFlow.UI.Asset.Navigator = QuoteFlow.Views.Base.extend({
    el: ".navigator-content",

    options: {},

    events: {},

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    subviews: {
        ".navigator-search": "toolbar"
    },

    initialize: function (options) {
        _.bindAll(this, "adjustHeight");

        // initialize the resize triggers
        QuoteFlow.Utilities.initializeResizeHooks();
        QuoteFlow.Interactive.onVerticalResize(this.adjustHeight);

        // subviews
        this.toolbar = new QuoteFlow.UI.Asset.Navigator.Toolbar();
        this.assetList = new QuoteFlow.UI.Asset.Navigator.AssetList();
        this.assetDetails = new QuoteFlow.UI.Asset.Navigator.AssetDetails();

        this.initializeAssetListSidebar();
        this.adjustHeight();
    },

    postRenderTemplate: function () { },

    /**
     * Automatically adjusts the height of the list view
     * once the available window size changes (dev tools pops up or screen resizes).
     */
    adjustHeight: function () {
        var b = this.$el.find(".list-panel");
        var d = 0,
            offset = $('.list-content', this.assetList.$el).offset().top + b.scrollTop(),
            outerHeight = this.$el.find(".pagination-container").outerHeight(),
            innerHeight = window.innerHeight;

        //        if (this.endOfStableSearchView && this.endOfStableSearchView.$el) {
        //            d = this.endOfStableSearchView.$el.outerHeight();
        //        }
        b.css("height", innerHeight - offset - d - outerHeight);
    },

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