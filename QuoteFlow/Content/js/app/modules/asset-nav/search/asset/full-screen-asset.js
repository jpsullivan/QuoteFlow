"use strict";

var _ = require('underscore');

var Marionette = require('backbone.marionette');

/**
 * 
 */
var FullScreenAsset = Marionette.ItemView.extend({
    initialize: function (options) {
        _.extend(this, options);
        _.bindAll(this,
            "onLoadComplete",
            "showError");

        if (this.assetContainer.hasClass("hidden")) {
            this.assetContainer.detach();
            this.assetContainer.removeClass("hidden");
        }

        if (this.searchContainer.hasClass("hidden")) {
            this.searchContainer.detach();
            this.searchContainer.removeClass("hidden");
        }
    },

    bindSearchService: function (searchService) {
        this.listenTo(searchService, {
            "assetUpdated": function (assetId, entity, reason) {
                if (reason.action !== "inlineEdit" && reason.action !== "rowUpdate") {
                    return this.updateAsset(reason);
                }
            },
            "selectedAssetChanged": function (selectedAsset) {
                if (selectedAsset.hasAsset()) {
                    QuoteFlow.application.execute("assetEditor:abortPending");
                    //TODO Why do we need to debounce this?
                    //JIRA.Issues.Utils.debounce(this, "_loadIssue", issue);
                    this.show({
                        id: selectedAsset.getId(),
                        key: selectedAsset.getKey(),
                        pager: searchService.getPager()
                    });
                    this.updatePager(searchService.getPager());
                } else {
                    if (this.isVisible()) {
                        this.hide();
//                        QuoteFlow.trace("quoteflow.returned.to.search");
//                        QuoteFlow.trace("quoteflow.psycho.returned.to.search");
                        QuoteFlow.application.execute("analytics:trigger", 'kickass.returntosearch');
                    }
                    //TODO Why the full $navigatorContent is marked as pending?
                    //this.$navigatorContent.removeClass("pending");
                }
            }
        });
    },

    onBeforeDestroy: function () {
        this.stopListening();
        if (this.active) {
            if (this.stalker) {
                this.stalker.unstalk();
            }
            QuoteFlow.application.off("assetEditor:loadComplete", this.onLoadComplete);
            QuoteFlow.application.off("assetEditor:loadError", this.showError);
            QuoteFlow.application.execute("assetEditor:abortPending");
            this.active = false;
        }
    },

    activate: function () {
        if (!this.active) {
            QuoteFlow.application.on("assetEditor:loadComplete", this.onLoadComplete);
            QuoteFlow.application.on("assetEditor:loadError", this.showError);
            this.active = true;
        }
    },

    onLoadComplete: function (model, props) {
        this.stalker = AJS.$(".js-stalker", this.assetContainer).stalker();
        this._makeAssetVisible();
        if (props.isNewAsset) {
            this._scrollToTop();
        }
    },

    _makeAssetVisible: function () {
        QuoteFlow.application.execute("assetEditor:beforeShow");
        this._setBodyClasses({
            error: false,
            asset: true,
            search: true
        });

        if (!this.isVisible()) {
            this.assetContainer.insertBefore(this.searchContainer);
            this.searchContainer.detach();

            //QuoteFlow.trace("jira.psycho.issue.refreshed", { id: JIRA.Issues.Application.request("issueEditor:getIssueId") });
            QuoteFlow.application.trigger(JIRA.Events.NEW_CONTENT_ADDED, [this.assetContainer, JIRA.CONTENT_ADDED_REASON.pageLoad]);
        }
    },

    /**
     * Refresh the visible asset in response to an asset update.
     *
     * @param {object} assetUpdate An asset update object (see <tt>JIRA.Issues.Utils.getUpdateCommandForDialog</tt>).
     * @return {jQuery.Deferred} A deferred that is resolved after the asset has been refreshed.
     */
    updateAsset: function (assetUpdate) {
        var deferred;

        var isVisibleAsset = assetUpdate.key === QuoteFlow.application.request("assetEditor:getAssetKey");

        if (this.isVisible()) {
            deferred = QuoteFlow.application.request("assetEditor:refreshAsset", assetUpdate);
            deferred.done(function () {
                if (!isVisibleAsset && assetUpdate.message) {
                    JIRA.Issues.showNotification(assetUpdate.message, assetUpdate.key);
                }
            });
        } else {
            deferred = jQuery.Deferred().resolve().promise();
        }

        return deferred;
    },

    /**
     * Show error message for loading asset.
     */
    showError: function () {
        this._setBodyClasses({
            error: true,
            asset: false,
            search: false
        });
    },

    /**
     * @return {boolean} whether an asset is visible.
     */
    isVisible: function () {
        return this.assetContainer.closest("body").length > 0;
    },

    /**
     * Scroll to the top of the window.
     *
     * @private
     */
    _scrollToTop: function () {
        AJS.$(window).scrollTop(0);
    },

    _setBodyClasses: function (options) {
        AJS.$("body")
            .toggleClass("page-type-message", options.error)
            .toggleClass("navigator-issue-only", options.asset)
            .toggleClass("page-type-navigator", options.search);
    },

    hide: function () {
        this._setBodyClasses({
            error: false,
            asset: false,
            search: true
        });

        if (this.isVisible()) {
            this.searchContainer.insertBefore(this.assetContainer);
            this.assetContainer.detach();
            this.trigger("assetHidden");
        }
    },

    updatePager: function (pager) {
        QuoteFlow.application.execute("pager:update", pager);
    },

    /**
     * Load and show an asset.
     *
     * @param {object} asset The asset to show.
     * @param {number} asset.id The asset's ID.
     * @param {string} asset.key The asset's key.
     * @return {jQuery.Deferred} A deferred that is resolved when the asset is visible.
     */
    show: function (asset) {
        this.activate();
        QuoteFlow.application.execute("assetEditor:setContainer", this.assetContainer);
        return QuoteFlow.application.request("assetEditor:loadAsset", asset).always(_.bind(function () {
            AJS.$(".js-stalker", this.assetContainer).stalker();

            this.updatePager(asset.pager);
            // now is a good time to pre-fetch anything that needs to be pre-fetched
            this.assetCacheManager.prefetchAssets();
        }, this));
    }
});

module.exports = FullScreenAsset;