"use strict";

var Marionette = require('backbone.marionette');

var AssetController = require('./controller');
var AssetRouter = require('./router');
var LayoutSwitcher = require('./search/layout-switcher');
var QueryComponent = require('../../components/query');
var SearchPageModule = require('./search/search-page-module');
var SearchHeaderModule = require('./search/search-header-module');

/**
 * 
 */
var AssetModule = Marionette.Module.extend({

    onStart: function (options) {
        var initializedOptions = this.init();
        return this.startMediator(options);
    },

    /**
     * Equivalent of AssetNavInit.js
     */
    init: function() {
        
    },

    startMediator: function (options) {
//        this.searchPageModule = new SearchPageModule({}, {
//            initialAssetTableState: options.initialAssetTableState
//        });
//        this.searchPageModule.registerViewContainers({
//            assetContainer: $(".asset-container"),
//            searchContainer: $('#.navigator-container')
//        });
//
//        // init modules
//        this.searchHeaderModule = new SearchHeaderModule({ searchPageModule: this.searchPageModule });
//        var queryModule = QueryComponent.create({
//            el: $el.find("form.navigator-search"),
//            searchers: options.initialSearcherCollectionState,
//            preferredSearchMode: "basic",
//            layoutSwitcher: true,
//            autocompleteEnabled: undefined,
//            basicAutoUpdate: true
//        });
//
////        JIRA.bind(JIRA.Events.ISSUE_TABLE_REORDER, function (e) {
////            if (!JIRA.Issues.Application.request("issueEditor:canDismissComment")) {
////                e.preventDefault();
////            }
////        });
//
//        this.layoutSwitcherView = new LayoutSwitcher({ searchPageModule: this.searchPageModule });
//        this.layoutSwitcherView.setElement($el.find("#layout-switcher-toggle")).render();
//
//        this.controller = new AssetController();
//        return new AssetRouter({
//            controller: this.controller,
//            searchPageModule: this.searchPageModule
//        });
    }
});

module.exports = AssetModule;