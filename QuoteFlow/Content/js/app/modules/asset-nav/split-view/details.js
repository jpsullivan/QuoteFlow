"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');
var URI = require('urijs');

var ApplicationAdapter = require('./services/application-adaptor');
var Results = require('../search/model/search-results');
var DetailsLayout = require('../details-layout/details-layout');
var DropdownFactory = require('../../../components/dropdown/factory');

var SplitScreenDetailsView = Marionette.Object.extend({
    _buildSearchResults: function (search) {
        this._destroySearchResults();

        var searchResults = search.getResults();
        var skus = searchResults._getAssetIdsToSkus();
        var ids = searchResults.get('assetIds');
        var assets = _.map(ids, function (id) {
            return {id: id, sku: skus[id]};
        });

        this.searchResults = new Results([], {
            totalRecordsInDB: searchResults.get('total'),
            pageSize: searchResults.get('pageSize'),
            totalRecordsInSearch: searchResults.get('assetIds').length,
            assets: assets,
            jql: search.getEffectiveJql(),
            allowNoSelection: true
        });
    },

    _destroySearchResults: function () {
        if (this.searchResults) {
            this.stopListening(this.searchResults);
            delete this.searchResults;
        }
    },

    _buildDetailsLayout: function (searchPageModule) {
        var currentQuery = new URI().query();
        var url = new URI(QuoteFlow.ApplicationPath + "assets/").query(currentQuery).removeQuery("startIndex");
        this.detailsLayout = new DetailsLayout({
            baseURL: url.toString(),
            shouldUpdateCurrentProject: false
        });
        var boundAdjustSize = _.bind(this.detailsLayout.adjustSize, this.detailsLayout);

        this.listenTo(this.detailsLayout, {
            "select": function (assetData) {
                if (assetData.id) {
                    var shouldReplaceCurrentURLInTheHistory = false;
                    var isThereAnAssetInTheCurrentURL = this.search.getResults().getSelectedAsset().getSku();
                    if (!isThereAnAssetInTheCurrentURL) {
                        shouldReplaceCurrentURLInTheHistory = true;
                    }
                    this.search.getResults().selectAssetById(assetData.id, {replace: shouldReplaceCurrentURLInTheHistory});
                }
            },
            "list:refresh": function () {
                debugger;
                this.search.refresh();
            },
            "list:sort": function (jql) {
                this.search.doSort(jql);
            },
            "list:update": function () {
            },
            "list:render": function () {
                this.trigger("render");
            },
            "empty": function () {
                this.trigger("render");
            },
            "destroy": function () {
                QuoteFlow.Interactive.offHorizontalResize(boundAdjustSize);
                QuoteFlow.Interactive.offVerticalResize(boundAdjustSize);
            },
            "list:select": function (event) {
            },
            "list:pagination": function () {
            },
            "editorLoaded": function (event) {
                // Danger, horrible code ahead!!
                //
                // The scenario: before, the element '.asset-container' had the scroll for the IssueEditor. Now, the
                // element with the scroll is it's parent '.detail-panel' (this helps heaps with the position of the
                // pager).
                //
                // The problem(s): design flaws in other components make supporting that scenario near impossible:
                //  *  Dropdowns in IssueEditor specify that they should get auto-closed when the user scrolls on
                //    '.asset-container'. That's wrong because it assumes that the scroll will always happen on
                //    that element. We can't change this without affecting all the users of the IssueEditor.
                //  * AJS.Dropdown creates an instance for every element marked as 'js-default-dropdown' that handles
                //    the logic for showing/hidding the dropdown... but does not provide external access to that
                //    instance. So there is no way to gracefuly close the dropdown, change its properties, etc.
                //    Changing this will require a change in JIRA core.
                //
                // The solution: when the IssueEditor is rendered, serach for all the elements with the scroll
                // attribute ('data-hide-on-scroll') and change it so it points to '.detail-panel'. Then, rebind
                // all the dialogs.
                this.container.find("[data-hide-on-scroll~='.asset-container']").each(function (idx, item) {
                    var $item = AJS.$(item);
                    $item.data("hasDropdown", false);
                    $item.off('click');
                    $item.attr("data-hide-on-scroll", $item.attr("data-hide-on-scroll").replace(".asset-container", ".detail-panel"));
                });
                DropdownFactory.bindGenericDropdowns(this.container);
            },
            "editorLoadedFromCache": function () {
            },
            "editor:saveSuccess": function (event) {
            },
            "linkToAsset": function (event) {
                searchPageModule.reset({
                    selectedAssetSku: event.assetSku
                });
            }
        });

        QuoteFlow.Interactive.onHorizontalResize(boundAdjustSize);
        QuoteFlow.Interactive.onVerticalResize(boundAdjustSize);
    },

    _destroyDetailsLayout: function () {
        this.detailsLayout.destroy();
        delete this.detailsLayout;
    },

    _buildSearch: function (search) {
        this.search = search;
        this.listenTo(this.search.getResults(), {
            "change:resultsId": this._loadSearch,
            "assetDeleted": function (assetData) {
                var deletedAsset = this.searchResults.get(assetData.id);
                this.searchResults.removeAndUpdateSelectionIfNeeded(deletedAsset);
            }
        });

        // These is *not* a regular Backbone event, we can't use listenTo.
        this.search.getResults().onAssetUpdated(this._handleRefreshAsset);

        this.listenTo(this.search.getResults().getSelectedAsset(), {
            "change": function () {
                var newAsset = this.search.getResults().getSelectedAsset().get('id');
                if (!newAsset) {
                    return;
                }
                if (!this.searchResults) {
                    return;
                }

                // The selected asset in this.search.getResults() has changed and it isn't
                // changed in our model: this means it has been changed 'outside' the DetailsLayout
                // (e.g. push state or API). In this case, we want reload our model to get the
                // correct page and select the asset.
                if (this.searchResults.selected && this.searchResults.selected.get('id') !== newAsset) {
                    this.detailsLayout.load(this.searchResults, newAsset);
                }
            }
        });
    },

    _destroySearch: function () {
        this.search.getResults().setTable(null, {silent: true});
        this.search.getResults().offAssetUpdated(this._handleRefreshAsset);
    },

    _loadSearch: function () {
        var assetSku;

        this._buildSearchResults(this.search);
        if (this.search.getResults().hasSelectedAsset()) {
            assetSku = this.search.getResults().getSelectedAsset().get('sku');
        } else if (this.search.getResults().get('startIndex') > 0) {
            assetSku = this.searchResults.getAssetSkuForIndex(this.search.getResults().get('startIndex'));
        } else if (this.search.getResults().hasHighlightedAsset()) {
            assetSku = this.search.getResults().getHighlightedAsset().get('sku');
        }

        this.detailsLayout.load(this.searchResults, assetSku);
    },

    _handleRefreshAsset: function (assetId) {
        this.detailsLayout.refreshAsset(assetId);
    },

    initialize: function (options) {
        this._handleRefreshAsset = _.bind(this._handleRefreshAsset, this);

        this._buildDetailsLayout(options.searchPageModule);
        this._buildSearch(options.search);

            // Create a container, we don't want to replace the existing markup
        var container = jQuery("<div></div>");
        options.searchContainer.find(".navigator-content").html('').append(container);
        this.container = this.detailsLayout.show(container);
        jQuery("body").addClass("page-type-split");

        options.fullScreenAsset.hide();

        ApplicationAdapter.init(this.detailsLayout);
    },

    render: function () {
        // This method is NOT called when the DetailsLayout is loaded initially. It will get called
        // if the uses switches from another layout (i.e. List View) to Details View.
        this._loadSearch();
    },

    close: function () {
        this.destroy();
    },

    onDestroy: function () {
        jQuery("body").removeClass("page-type-split");
        this._destroyDetailsLayout();
        this._destroySearch();
        this._destroySearchResults();
        ApplicationAdapter.destroy();
    },

    nextAsset: function () {
        this.detailsLayout.selectNext();
    },

    prevAsset: function () {
        this.detailsLayout.selectPrevious();
    },

    isLoading: function () {
        return this.detailsLayout.isLoading();
    },

    isAssetViewActive: function () {
        return false;
    }
});

module.exports = SplitScreenDetailsView;
