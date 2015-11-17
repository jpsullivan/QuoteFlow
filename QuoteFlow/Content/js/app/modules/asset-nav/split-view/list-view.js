"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var recurringPromise = require('jquery-recurring-promise');
var Utilities = require('../../../components/utilities');

/**
 * The split view "asset panel" (where the asset is rendered).
 */
var SplitScreenListView = Marionette.ItemView.extend({
    //inaccessibleIssueRowTemplate: JIRA.Templates.SplitView.inaccessibleIssueRow,
    template: JST["quote-builder/split-view/asset-list"],

    events: {
        "click .issue-list li": "_onClickAsset"
    },

    /**
     * @param {object} options
     * @param {SearchModule} options.search The view's interface to the rest of the application.
     */
    initialize: function (options) {
        _.bindAll(this,
            "_onSearchDone",
            "_onSearchFail");

        this.search = options.search;
        this.searchContainer = options.searchContainer;
        this.searchPromise = this._createSearchPromise({
            done: this._onSearchDone,
            fail: this._onSearchFail
        });

        this.searchResults = options.search.getResults();
        this.serverRendered = !!options.serverRendered && this._assetsRendered(options);

        this.addListener(this.searchResults, "highlightedAssetChange", this._onHighlightedAssetChange, this);
        this.addListener(this.searchResults, "assetUpdated", this._onAssetUpdated, this);
        this.addListener(this.searchResults, "startIndexChange", this.render, this);
    },

    _assetsRendered: function (options) {
        return !!options.searchContainer.find(".issue-list").length;
    },

    scrollToFocused: function () {
        var $assetPanel = this.searchContainer.find(".list-panel");
        var $focused = this.$el.find(".focused");
        if ($assetPanel.size() && $focused.size()) {
            $assetPanel.scrollTop($focused.position().top);
        }
    },

    /**
     * Prepare to be removed, deactivating all subviews.
     */
    onBeforeDestroy: function () {
        this.searchPromise.reset();
        //JIRA.Issues.BaseView.prototype.deactivate.apply(this, arguments);
    },

    /**
     * Load an asset into the detail panel after its row was clicked.
     *
     * @param {jQuery.Event} e The click event.
     * @private
     */
    _onClickAsset: function (e) {
        var assetId = AJS.$(e.target).closest("[data-id]").data("id"),
            isSelected = this.searchResults.getSelectedAsset().getId() === assetId;

        e.preventDefault();

        if (!isSelected) {
            this.searchResults.selectAssetById(assetId, { replace: true });

//            JIRA.Issues.Application.execute("analytics:trigger", "kickass.openIssueFromTable", {
//                issueId: assetId,
//                // these are 1-based indices
//                absolutePosition: this.searchResults.getPositionOfIssueInSearchResults(assetId) + 1,
//                relativePosition: this.searchResults.getPositionOfIssueInPage(assetId) + 1
//            });
        }
    },

    /**
     * Create the recurring search promise. The added deferreds trigger a render on done.
     *
     * @param {object} [options]
     * @param {function} [options.done] A done callback.
     * @param {function} [options.fail] A fail callback.
     * @return {jQuery.RecurringPromise} the recurring search promise.
     * @private
     */
    _createSearchPromise: function (options) {
        var promise = jQuery.RecurringPromise();

        options = options || {};
        options.done && promise.done(_.bind(options.done, this));
        options.fail && promise.fail(_.bind(options.fail, this));

        return promise;
    },

    getAssetById: function (id) {
        return this.$el.find("[data-id=" + id + "]");
    },

    /**
     * Highlight an asset in the list.
     *
     * @param {SimpleAsset} [asset] The asset to highlight; if falsey, the highlight is removed.
     * @private
     */
    _highlightAsset: function (asset) {
        this.$el.find(".focused").removeClass("focused");
        // A little trick to scroll to the correct item without having to implement our
        asset && this.getAssetById(asset.getId()).addClass("focused").scrollIntoView();
    },

    /**
     * Mark an asset in the list as being inaccessible.
     *
     * @param {number} id The asset ID.
     */
    markAssetInaccessible: function (id) {
        this.getAssetById(id).replaceWith(this.inaccessibleIssueRowTemplate({
            isHighlighted: this.searchResults.getHighlightedAsset().getId() === id,
            assetId: id
        }));
    },

    /**
     * Highlight the currently highlighted asset.
     *
     * @private
     */
    _onHighlightedAssetChange: function () {
        this._highlightAsset(this.searchResults.getHighlightedAsset());
    },

    _onAssetUpdated: function (id, entity) {
        if (entity.table[0] === null) {
            this.markAssetInaccessible(id);
        } else {
            this.getAssetById(id).html(JIRA.Templates.SplitView.issueRow(entity.table[0]));
        }
    },

    /**
     * Render the asset list. Called when a search request is fulfilled.
     *
     * @param {object} result The search payload, extended with additional information.
     * @param {boolean} [result.serverRendered] Whether the existing HTML was server rendered.
     * @private
     */
    _onSearchDone: function (result) {
        //Depending on the default layout, anonymous user may have their layout preference set to the other layout
        //if that's true, data given by the server on page load will not be compatible. Hence do a check here.
        if (result instanceof Array) {
            if (this.searchResults.hasAssets()) {
                this.$el.html(this.template({
                    highlightedID: this.searchResults.getHighlightedAsset().getId(),
                    assetIDs: this.searchResults.getPageAssetIds(),
                    assets: result
                }));
            }

            _.defer(_.bind(this.scrollToFocused, this));
        }
    },

    /**
     * Display an error after a search fails.
     * Called when an operation in <tt>searchPromise</tt> fails.
     *
     * @private
     */
    _onSearchFail: function () {
        var navigatorContent = this.searchContainer.find(".navigator-content");
        navigatorContent.html(JIRA.Templates.Issues.ComponentUtil.errorMessage({
            msg: AJS.I18n.getText("issue.nav.common.server.error")
        }));
    },

    render: function () {
        var serverRendered = this.serverRendered;
        this.serverRendered = false;

        if (this.searchResults.hasAssets()) {
            if (!this.searchResults.hasHighlightedAsset() && !this.searchResults.hasSelectedAsset()) {
                // Highlighting the asset sets the start index, causing another render. Return to avoid double rendering.
                this.searchResults.highlightFirstInPage();
                return;
            }

            // Pass the serverRendered value through so _onSearchDone knows whether to preserve HTML.
            return this.searchPromise.add(this.searchResults.getResultsForPage({
                jql: this.search.getEffectiveJql()
            }).then(function (result) {
                return _.extend(result, { serverRendered: serverRendered });
            }));
        } else {
            return jQuery.Deferred().resolve();
        }
    }
});

module.exports = SplitScreenListView;
