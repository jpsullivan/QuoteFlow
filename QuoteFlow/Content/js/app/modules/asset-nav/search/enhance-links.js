"use strict";

var $ = require('jquery');
var _ = require('underscore');

var AssetsApi = require('./assets-api');
var SmartAjax = require('../../../components/ajax/smart-ajax');

/**
 * Enhance links outside KA, usually to load things via AJAX.
 * <p/>
 * Only works if the AJAX view issue dark feature is enabled.
 *
 * @param {object} options
 * @param {SearchPageModule} options.searchPageModule The application's SearchPageModule.
 */
var EnhanceLinks = {

    /**
     * Search for links that should alter the page state via pushState
     *
     * @param options {Object} Options used by the enhancer
     * @param options.selector {String} (Optional) Selector to use, defaults to .push-state
     * @param options.router {JIRA.Issues.IssueNavRouter} Main router instance in the page
     */
    withPushState: function (options) {
        options = _.defaults(options, {
            selector: ".push-state"
        });

        AJS.$(document).delegate(options.selector, "simpleClick", function (e) {
            if (this.href) {
                e.preventDefault();

                // Let's assume we're only going to be pushing relative links.
                var uriComponents = parseUri(this.href);
                var fragment = uriComponents.path;

                if (uriComponents.query) {
                    fragment += "?" + uriComponents.query;
                }

                // Backbone's fragment doesn't include the history root.
                fragment = fragment.replace(Backbone.history.options.root, "");

                // Force refreshing even if the URL hasn't changed.
                // https://github.com/documentcloud/backbone/issues/652
                if (Backbone.history.fragment == fragment ||
                    Backbone.history.fragment == decodeURIComponent(fragment)) {
                    Backbone.history.loadUrl(fragment);
                }
                else {
                    options.router.navigate(fragment, { trigger: true });
                }
            }
        });
    },

    /**
     * Search for links to the the issue navigator and enhances them
     *
     * @param options {Object} Options used by the enhancer
     * @param options.searchPageModule {JIRA.Issues.SearchPageModule} Main SearchPageModule instance in the page
     */
    toIssueNav: function (options) {
        var filterSelector = "a.filter-link[data-filter-id], a.filter-link[data-jql]",
            newSelector = "#issues_new_search_link_lnk";

        AJS.$(document).on("simpleClick", filterSelector, function (e) {
            e.preventDefault();

            var $anchor = AJS.$(e.target).closest("a"),
                filterId = $anchor.data("filter-id"),
                jql = $anchor.data("jql");

            options.searchPageModule.reset({
                filter: filterId,
                jql: jql
            });
        });

        // The "Search for Issues" link in the header.
        AJS.$(document).on("simpleClick", newSelector, function (e) {
            e.preventDefault();
            options.searchPageModule.resetToBlank();
        });
    },

    isIssueTableDropdown: function (e) {
        if (AJS.InlineLayer.current) {
            var $offsetTarget = jQuery(AJS.InlineLayer.current.offsetTarget());
            if ($offsetTarget.closest(".list-view table#issuetable").size()) {
                return true;
            }
        }
    },

    /**
     * Search for links to a particular issue and enhances them
     *
     * @param options {Object} Options used by the enhancer
     */
    toIssue: function (options) {
        // Enhance Attachment options links
        AJS.$(document).on("simpleClick",
        [
            "#attachment-sorting-options a",
            "#attachment-sorting-order-options a",
            "#subtasks-show-all",
            "#subtasks-show-open"
        ].join(','),
            function (e) {
                if (this.href) {
                    e.preventDefault();
                    e.stopPropagation();

                    // Capture the new ViewIssueQuery from the link href and update the issue
                    var uriComponents = parseUri(this.href);
                    if (uriComponents.queryKey) {
                        QuoteFlow.application.execute("assetEditor:updateAssetWithQuery", uriComponents.queryKey);

                        // Adjust checked marks on our radio group
                        // This is needed for IE8, as this browser won't render the new Attachments panels if the content
                        // is the same (i.e. the sort order has not changed)
                        AJS.$(this).parents('ul').find('a.aui-checked').removeClass('aui-checked');
                        AJS.$(this).addClass('aui-checked');
                    }
                }
            }
        );

        // Make issue links load via AJAX.
        var issueSelector = "a.issue-link[data-issue-key]",
            issueIsParentSelector = ".parentIssue",
            issueFromTableSelector = ".list-view table#issuetable a.issue-link[data-issue-key]";

        AJS.$(document).on("simpleClick", issueSelector, function (e) {
            e.preventDefault();
            e.stopPropagation();

            var searchResults = options.searchPageModule.searchResults;

            var assetSku = AJS.$(e.target).closest("a").data("asset-sku");
            var assetIdAsString = searchResults._getAssetIdForSku(assetSku);
            var assetId = parseInt(assetIdAsString, 10);

            // Check if it is a link from the results table
            var isFromResultsTable = AJS.$(e.target).is(issueFromTableSelector);
            var isLinkToParentIssue = AJS.$(e.target).is(issueIsParentSelector);
            var isIssueTableDropdown = EnhanceLinks.isIssueTableDropdown(issueFromTableSelector);
            if ((isFromResultsTable && !isLinkToParentIssue) || isIssueTableDropdown) {
                QuoteFlow.application.execute("analytics:trigger", "kickass.openIssueFromTable", {
                    assetId: assetIdAsString,
                    // these are 1-based indices
                    absolutePosition: searchResults.getPositionOfIssueInSearchResults(assetId) + 1,
                    relativePosition: searchResults.getPositionOfIssueInPage(assetId) + 1
                });
                options.searchPageModule.update({
                    selectedAssetSku: assetSku
                });
            } else {
                options.searchPageModule.reset({
                    selectedAssetSku: assetSku
                });
            }
        });
    },


    /**
     * Search for links that should be loaded via AJAX
     */
    transformToAjax: function () {
        // Enhance Attachment options links
        AJS.$(document).on("simpleClick", ".subtask-reorder a", function (e) {
            if (this.href) {
                e.preventDefault();
                e.stopPropagation();

                AJS.$.ajax({
                    type: "GET",
                    data: {
                        disableRedirect: true
                    },
                    url: this.href
                }).done(function () {
                    AssetsApi.refreshSelectedIssue({
                        reason: JIRA.Issues.Actions.UPDATE
                    });
                }).fail(function (xhr) {
                    JIRA.Messages.showErrorMsg(SmartAjax.buildSimpleErrorContent(xhr), {
                        closeable: true
                    });
                });
            }
        });
    }
};

module.exports = EnhanceLinks;
