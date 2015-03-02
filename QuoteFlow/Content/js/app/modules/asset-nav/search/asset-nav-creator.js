"use strict";

var $ = require('jquery');
var _ = require('underscore');

var AssetSearchManager = require('./asset-search-manager');
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
            initialAssetTableState: options.initialAssetTableState
        });
        searchPageModule.registerViewContainers({
            issueContainer: $(".asset-container"),
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

        var queryModule = QueryComponent.create({
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

        this.layoutSwitcherView = new LayoutSwitcherView({ searchPageModule: searchPageModule });
        this.layoutSwitcherView.setElement($el.find("#layout-switcher-toggle")).render();

        var issueModule = JIRA.Issues.Application.request("issueEditor");
        JIRA.Issues.Application.on("issueEditor:render", function(regions) {
            JIRA.Issues.Application.execute("pager:render", regions.pager);
        });
        JIRA.Issues.Application.commands.setHandler("returnToSearch", function() {
            JIRA.Issues.Application.execute("issueEditor:close");
        });

        // Initialize event bubbling
        JIRA.Issues.Application.on("issueEditor:saveSuccess", function (props) {
            JIRA.trigger(JIRA.Events.ISSUE_REFRESHED, [props.issueId]);
        });
        JIRA.Issues.Application.on("issueEditor:saveError", function (props) {
            if (!props.deferred) {
                JIRA.trigger(JIRA.Events.ISSUE_REFRESHED, [props.issueId]);
            }
        });

        JIRA.Issues.FocusShifter.init();

        var viewIssueData = issueModule.viewAssetData;

        var issueSearchManager = new AssetSearchManager({
            initialAssetTableState: options.initialAssetTableState,
            initialAssetIds: options.initialAssetIds
        });

        var searchModule = new SearchModule({
            searchPageModule: searchPageModule,
            queryModule: queryModule,
            assetSearchManager: issueSearchManager,
            initialSelectedAsset: options.initialSelectedAsset
        });

        var issueCacheManager = new JIRA.Issues.Cache.IssueCacheManager({
            searchResults: searchModule.getResults(),
            viewAssetData: viewIssueData
        });

        // TODO TF-693 - FullScreenIssue will detach these elements, so get a reference now before they're not discoverable.
        var issueNavToolsElement = $el.find(".saved-search-selector");
        // TODO TF-693 - Try to prevent FullScreenIssue from hacking and mutilating the DOM so much...
        var fullScreenIssue = new JIRA.Issues.FullScreenIssue({
            issueContainer: searchPageModule.issueContainer,
            searchContainer: searchPageModule.searchContainer,
            assetCacheManager: issueCacheManager
        });

        // Register Modules
        searchPageModule.registerSearch(searchModule);
        searchPageModule.registerSearchHeaderModule(searchHeaderModule);
        searchPageModule.registerFilterModule(filterModule);
        searchPageModule.registerQueryModule(queryModule);

        searchPageModule.registerFullScreenIssue(fullScreenIssue);
        searchPageModule.registerIssueSearchManager( issueSearchManager);
        searchPageModule.registerIssueCacheManager(issueCacheManager);
        searchPageModule.registerLayoutSwitcher(this.layoutSwitcherView);

        searchHeaderModule.registerSearch(searchModule);
        searchHeaderModule.createToolsView(issueNavToolsElement);

        // Router

        var issueNavRouter = this.issueNavRouter = new JIRA.Issues.IssueNavRouter({
            searchPageModule: searchPageModule,
            initialSessionSearchState: options.initialSessionSearchState
        });

        searchPageModule.registerIssueNavRouter(issueNavRouter);

        // Overrides

        JIRA.Issues.enhanceLinks.toIssueNav({
            searchPageModule: searchPageModule
        });

        JIRA.Issues.enhanceLinks.withPushState({
            router: issueNavRouter
        });

        JIRA.Issues.enhanceLinks.toIssue({
            searchPageModule: searchPageModule
        });

        JIRA.Issues.enhanceLinks.transformToAjax();

        JIRA.Issues.dialogCleaner(issueNavRouter);

        JIRA.Issues.Api.initialize({
            searchPageModule: searchPageModule
        });

        JIRA.Issues.IssueAPI.override({
            searchPageModule: searchPageModule
        });

        JIRA.Issues.IssueNavigatorAPI.override({
            searchPageModule: searchPageModule
        });

        /**
         * Used to defer the showing of issue dialogs until all promises are resolved.
         * We use this to ensure the dialog we are opening has the correct data.
         * If we are inline editing the summary then open the edit dialog, we want to be sure that the summary has been
         * updated on the server first, otherwise we will be showing stale data in the edit dialog.
         */
        JIRA.Dialogs.BeforeShowIssueDialogHandler.add(JIRA.Issues.Api.waitForSavesToComplete);

        JIRA.Issues.overrideIssueDialogs({
            getIssueId: _.bind(searchPageModule.getEffectiveIssueId, searchPageModule),
            isNavigator: true,
            updateAsset: function(dialog) {
                var issueUpdate = JIRA.Issues.Utils.getUpdateCommandForDialog(dialog);
                return searchPageModule.updateAsset(issueUpdate);
            }
        });

        // Keyboard shortcuts ?

        $(document).keydown(function (e) {
            var dialogIsVisible = $("div.aui-blanket").length > 0,
                wasSupportedKey = (e.which === $.ui.keyCode.ENTER || e.which === $.ui.keyCode.LEFT ||
                    e.which === $.ui.keyCode.UP || e.which === $.ui.keyCode.RIGHT || e.which === $.ui.keyCode.DOWN);

            if (!dialogIsVisible && wasSupportedKey) {
                var target = $(e.target),
                    targetIsValid = target.is(":not(:input)");

                if (target == undefined || targetIsValid) {
                    if (e.which === $.ui.keyCode.ENTER) {
                        if (target == undefined || target.is(":not(a)")) {
                            JIRA.Issues.Api.viewSelectedIssue();
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
        this.searchPageModule.on("change", changeLoginUrl);
        this.searchResults.on("change", changeLoginUrl);
        this.searchResults.getSelectedAsset().on("change", changeLoginUrl);

        return this;
    },

    /**
     * change the login url to the current state.
     */
    changeLoginUrl: function() {
        var url = JIRA.Issues.LoginUtils.redirectUrlToCurrent();
        $('.login-link').attr('href', url);
    }
};

module.exports = AssetNavigator