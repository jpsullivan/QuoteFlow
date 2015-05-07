"use strict";

var $ = require('jquery');
var _ = require('underscore');

var Marionette = require('backbone.marionette');

var AssetCacheManager = require('./cache/asset-cache-manager');
var AssetNavCustomRouter = require('../router-custom');
var AssetsApi = require('./assets-api');
var AssetSearchManager = require('./asset-search-manager');
var DialogCleaner = require('../../../util/dialog-cleanup');
var EnhanceLinks = require('./enhance-links');
var FullScreenAsset = require('./asset/full-screen-asset');
var LayoutSwitcherView = require('./layout-switcher');
var QueryComponent = require('../../../components/query.js');
var SearchHeaderModule = require('./search-header-module');
var SearchModule = require('./search-module');
var SearchPageModule = require('./search-page-module');

/**
 *
 */
var AssetNavCreator = {

    create: function($el, options) {
        var searchPageModule = this.searchPageModule = new SearchPageModule({}, {
          initialIssueTableState: options.initialIssueTableState
        });
        searchPageModule.registerViewContainers({
            assetContainer: $(".issue-container"),
            searchContainer: $(".navigator-container")
        });

        // Initialize Modules

        var searchHeaderModule = this.searchHeaderModule = new SearchHeaderModule({
            searchPageModule: searchPageModule
        });

//        var filterModule = new JIRA.Issues.FilterModule({
//            searchPageModule: searchPageModule,
//            systemFilters: searchPageModule.addOwnerToSystemFilters(options.systemFilters)
//        });

        var queryModule = QueryComponent().create({
            el: $el.find("form.navigator-search"),
            searchers: options.initialSearcherCollectionState,
            preferredSearchMode: "basic",
            layoutSwitcher: true,
            autocompleteEnabled: false,
            basicAutoUpdate: true
        });

//        JIRA.bind(JIRA.Events.ISSUE_TABLE_REORDER, function(e) {
//            if (!JIRA.Issues.Application.request("issueEditor:canDismissComment")) {
//                e.preventDefault();
//            }
//        });

      //  this.layoutSwitcherView = new LayoutSwitcherView({ searchPageModule: searchPageModule });
      //  this.layoutSwitcherView.setElement($el.find("#layout-switcher-toggle")).render();

        var issueModule = QuoteFlow.application.request("issueEditor");

        QuoteFlow.application.vent.on("assetEditor:render", function (regions) {
            QuoteFlow.application.execute("pager:render", regions.pager);
        });
        QuoteFlow.application.commands.setHandler("returnToSearch", function () {
            QuoteFlow.application.execute("assetEditor:close");
        });

        // Initialize event bubbling
        QuoteFlow.application.vent.on("assetEditor:saveSuccess", function (props) {
            QuoteFlow.application.vent.trigger(JIRA.Events.ISSUE_REFRESHED, [props.issueId]);
        });
        QuoteFlow.application.vent.on("assetEditor:saveError", function (props) {
            if (!props.deferred) {
                QuoteFlow.application.vent.trigger(JIRA.Events.ISSUE_REFRESHED, [props.issueId]);
            }
        });

        //FocusShifter.init();

        var issueSearchManager = new AssetSearchManager({
            initialIssueTableState: options.initialIssueTableState,
            initialAssetIds: options.initialAssetIds
        });

        var searchModule = new SearchModule({
            searchPageModule: searchPageModule,
            queryModule: queryModule,
            assetSearchManager: issueSearchManager,
            initialSelectedAsset: options.initialSelectedAsset
        });

//        var viewIssueData = issueModule.viewAssetData;
//        var issueCacheManager = new AssetCacheManager({
//            searchResults: searchModule.getResults(),
//            viewAssetData: viewIssueData
//        });
        var issueCacheManager = new AssetCacheManager({
            searchResults: searchModule.getResults()
        });

        // TODO: FullScreenAsset will detach these elements, so get a reference now before they're not discoverable.
        var issueNavToolsElement = $el.find(".saved-search-selector");
        // TODO: Try to prevent FullScreenAsset from hacking and mutilating the DOM so much...
        var fullScreenIssue = new FullScreenAsset({
            assetContainer: searchPageModule.assetContainer,
            searchContainer: searchPageModule.searchContainer,
            assetCacheManager: issueCacheManager
        });

        // Register Modules
        searchPageModule.registerSearch(searchModule);
        searchPageModule.registerSearchHeaderModule(searchHeaderModule);
        //searchPageModule.registerFilterModule(filterModule);
        searchPageModule.registerQueryModule(queryModule);

        searchPageModule.registerFullScreenAsset(fullScreenIssue);
        searchPageModule.registerIssueSearchManager(issueSearchManager);
        searchPageModule.registerIssueCacheManager(issueCacheManager);
        //searchPageModule.registerLayoutSwitcher(this.layoutSwitcherView);

        searchHeaderModule.registerSearch(searchModule);
        //searchHeaderModule.createToolsView(issueNavToolsElement);

        // Router

        debugger;
        var issueNavRouter = this.assetNavRouter = new AssetNavCustomRouter({
            searchPageModule: searchPageModule,
            initialSessionSearchState: options.initialSessionSearchState
        });

        searchPageModule.registerIssueNavRouter(issueNavRouter);

        // Overrides

        EnhanceLinks.toIssueNav({
            searchPageModule: searchPageModule
        });

        EnhanceLinks.withPushState({
            router: issueNavRouter
        });

        EnhanceLinks.toIssue({
            searchPageModule: searchPageModule
        });

        EnhanceLinks.transformToAjax();

        DialogCleaner(issueNavRouter);

        AssetsApi.initialize({
            searchPageModule: searchPageModule
        });

//        JIRA.Issues.IssueAPI.override({
//            searchPageModule: searchPageModule
//        });

//        JIRA.Issues.IssueNavigatorAPI.override({
//            searchPageModule: searchPageModule
//        });

        /**
         * Used to defer the showing of issue dialogs until all promises are resolved.
         * We use this to ensure the dialog we are opening has the correct data.
         * If we are inline editing the summary then open the edit dialog, we want to be sure that the summary has been
         * updated on the server first, otherwise we will be showing stale data in the edit dialog.
         */
        //JIRA.Dialogs.BeforeShowIssueDialogHandler.add(JIRA.Issues.Api.waitForSavesToComplete);

//        JIRA.Issues.overrideIssueDialogs({
//            getIssueId: _.bind(searchPageModule.getEffectiveIssueId, searchPageModule),
//            isNavigator: true,
//            updateAsset: function(dialog) {
//                var issueUpdate = JIRA.Issues.Utils.getUpdateCommandForDialog(dialog);
//                return searchPageModule.updateAsset(issueUpdate);
//            }
//        });

        debugger;

        // Keyboard shortcuts ?

        $(document).keydown(function (e) {
            var dialogIsVisible = $("div.aui-blanket").length > 0,
                wasSupportedKey = (e.which === $.ui.keyCode.ENTER || e.which === $.ui.keyCode.LEFT ||
                    e.which === $.ui.keyCode.UP || e.which === $.ui.keyCode.RIGHT || e.which === $.ui.keyCode.DOWN);

            if (!dialogIsVisible && wasSupportedKey) {
                var target = $(e.target),
                    targetIsValid = target.is(":not(:input)");

                if (_.isUndefined(target) || targetIsValid) {
                    if (e.which === $.ui.keyCode.ENTER) {
                        if (_.isUndefined(target) || target.is(":not(a)")) {
                            AssetsApi.viewSelectedIssue();
                        }
                    } else if (e.which === $.ui.keyCode.LEFT) {
                        searchPageModule.handleLeft();
                    } else if (e.which === $.ui.keyCode.RIGHT) {
                        searchPageModule.handleRight();
                    } else if (e.which === $.ui.keyCode.UP) {
                        if (searchPageModule.handleUp()) {
                            e.preventDefault();
                        }
                    } else if (e.which === $.ui.keyCode.DOWN) {
                        if (searchPageModule.handleDown()) {
                            e.preventDefault();
                        }
                    }
                }
            }
        });

        // Not such a crash hot idea; should remove it.
        this.searchResults = searchModule.getResults();

        // Create the on change bindings for updating the login link.
        this.searchPageModule.on("change", this.changeLoginUrl);
        this.searchResults.on("change", this.changeLoginUrl);
        this.searchResults.getSelectedAsset().on("change", this.changeLoginUrl);

        return this;
    },

    /**
     * change the login url to the current state.
     */
    changeLoginUrl: function() {
        // var url = JIRA.Issues.LoginUtils.redirectUrlToCurrent();
        // $('.login-link').attr('href', url);
    }
};

module.exports = AssetNavCreator;
