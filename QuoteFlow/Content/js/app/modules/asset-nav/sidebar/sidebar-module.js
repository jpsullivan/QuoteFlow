"use strict";

var Brace = require('backbone-brace');

var SidebarController = require('./controller');
var SidebarModel = require('./sidebar-model');
var SidebarPanelView = require('./panel');

var QuoteSidebarModule = Brace.Evented.extend({

    initialize: function (options) {
        this._searchPageModule = options.searchPageModule;

        this.filterPanelModel = options.filterPanelModel || new SidebarModel({}, {
            searchPageModule: options.searchPageModule
        });

        this.filtersComponent = new SidebarController({
            lineItems: options.lineItems,
            searchPageModule: this._searchPageModule
        });

        this.filtersComponent.on("newFilter", function(newFilterModel){
            this._searchPageModule.filterModuleSaved(newFilterModel);

            if (QuoteFlow.application.request("assetEditor:canDismissComment")) {
                this._searchPageModule.setSessionSearch(newFilterModel);
            }
        }, this);

        this.filtersComponent.on("filterRemoved", function(args){
            this.trigger("filterRemoved", args);
        }, this);

        this.filtersComponent.on("filterSelected", this._onFilterSelected, this);

        this.filtersComponent.on("filterDiscarded", function(){
            this._searchPageModule.discardFilterChanges();
        }, this);

        this.filtersComponent.on("savedFilter", function(filterModel) {
            this._searchPageModule.setSessionSearch(filterModel);
            this.filtersComponent.highlightLineItem(filterModel);
        }, this);

        this._searchPageModule.on('change changeFilterProps', function() {
            this.filtersComponent.updateFilterHeader({
                model: this._searchPageModule.getFilter(),
                isDirty: this._searchPageModule.isDirty()
            });
        }, this);

        this.filterPanelModel.bind("change:activeLineItem", this._markFilterAsActive, this);
        this.filterPanelModel.set("activeLineItem", this._searchPageModule.getFilter());
        this._searchPageModule.bind("change:lineItem", function(searchPageModule, newFilter) {
            this.filterPanelModel.set("activeLineItem", newFilter);
        }, this);
    },

    /**
     * Retrieve system filter information via AJAX.
     * <p/>
     * System filter information is not present on the stand alone view issue page.
     *
     * @return {jQuery.Deferred} a deferred that is resolved after system filter information has been retrieved.
     */
    initSystemFilters: function() {
        return this.filtersComponent.fetchSystemFilters();
    },

    /**
     * Finds the filter with the given id. Attempts to fetch from the server if it does not exist. Returns a promise.
     */
    getFilterById: function(filterId) {
        return this.filtersComponent.getFilterById(filterId);
    },

    /**
     * Creates a FilterPanelView and renders it into the provided elements
     * @param els - a map of elements
     */
    createView: function(els) {
        this.filterPanelView = new SidebarPanelView({
            el: els.$filterPanelEl,
            model: this.filterPanelModel,
            searchPageModule: this._searchPageModule,
            easeOff: (!!jQuery.browser.msie && jQuery.browser.version <= 8) ? 500 : 0
        });

        this.filterPanelView.on("render", function() {
            this.filtersComponent.showLineItems(this.filterPanelView.$el.find(".quote-panel-line-items-container"));
            this._markFilterAsActive();
        }, this);

        this.filterPanelView.on("filterSelected", this._onFilterSelected, this);

        this.filterPanelView.render();

        return this.filterPanelView;
    },

    _markFilterAsActive: function() {
        var activeLineItem = this.filterPanelModel.get("activeLineItem");
        this.filtersComponent.highlightLineItem(activeLineItem);
    },

    _onFilterSelected: function(filterId) {
        if (filterId === null) {
            this._searchPageModule.resetToBlank({isNewSearch: true});
        } else {
            this._searchPageModule.resetToFilter(filterId);
        }
    }
});

module.exports = QuoteSidebarModule;
