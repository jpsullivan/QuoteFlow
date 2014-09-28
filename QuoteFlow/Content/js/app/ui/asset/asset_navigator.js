"use strict";

var $ = require('jquery');
var jqUi = require('jquery-ui');
var jqUiSidebar = require('jquery-ui-sidebar');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

// Data Layer
var AssetVarValueModel = require('../../models/asset_var_value');

// UI Components
var BaseView = require('../../view');
var AssetDetails = require('./navigator/asset_details');
var AssetList = require('./navigator/asset_list');
var Toolbar = require('./navigator/toolbar');


/**
 *
 */
var AssetNavigator = BaseView.extend({
    el: ".navigator-content",

    options: {},

    events: {},

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
        
        });
    },

    subviews: {
        ".navigator-search": "toolbar"
    },

    initialize: function(options) {
        _.bindAll(this, "adjustHeight");

        // initialize the resize triggers
        QuoteFlow.Utilities.initializeResizeHooks();
        QuoteFlow.Interactive.onVerticalResize(this.adjustHeight);

        // subviews
        this.toolbar = new Toolbar();
        this.assetList = new AssetList();
        this.assetDetails = new AssetDetails();

        this.initializeAssetListSidebar();
        this.adjustHeight();
    },

    postRenderTemplate: function() {},

    /**
     * Automatically adjusts the height of the list view
     * once the available window size changes (dev tools pops up or screen resizes).
     */
    adjustHeight: function() {
        var listPanel = this.$el.find(".list-panel");
        var offset = $('.list-content', this.assetList.$el).offset().top + listPanel.scrollTop(),
            outerHeight = this.$el.find(".pagination-container").outerHeight(),
            innerHeight = window.innerHeight;

        listPanel.css("height", innerHeight - offset - outerHeight);
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
});

module.exports = AssetNavigator;