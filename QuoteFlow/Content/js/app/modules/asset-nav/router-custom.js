"use strict";

var _ = require('underscore');
var Backbone = require('backbone');
var BackboneQueryParams = require('backbone-query-parameters');
var Marionette = require('backbone.marionette');

var UrlSerializer = require('../../util/url-serializer');

/**
 * A mostly-custom router for handling routes that are used within the
 * asset table module.
 */
var AssetNavCustomRouter = Marionette.AppRouter.extend({

    initialize: function (options) {
        _.extend(this, options);
        _.bindAll(this, "_restoreSessionSearch", "_route");

        // this.route(/^(.*?)([\?]{1}.*)?$/, this._route);
        // this.route(/^(quote)\/([^\/\?]+)\/([^\/\?]+)\/(builder)(\?.*)?$/, this._route);

        // equivalent to: "quote/:id/:name/builder/?:query"
        // this.route(/^(quote\/)([^\/\?]+)\/([^\/\?]+)\/(builder\/)\?([^\/\?]+)(\?.*)?$/, this._route);
        this.route("quote/:id/:name/builder/:selectedAssetId?:query", this._route);

        // equivalent to "quote/:id/:name/builder/"
        this.route(/^(quote)\/([^\/\?]+)\/([^\/\?]+)\/(builder)\/?$/, this._route);

        // backbone-query-parameters supports clever decoding of values into arrays, but we don't want this.
        delete Backbone.Router.arrayValueSplit;
    },

    /**
     * Overwrite Marionette.AppRouter, now it fires an event each time the URL changes
     */
    navigate: function () {
        // this.searchPageModule.removeOpenTipsies();
        this.trigger("navigate");
        Marionette.AppRouter.prototype.navigate.apply(this, arguments);
    },

    /**
     * Navigate to a new state.
     *
     * @param {UrlSerializer.state} state
     */
    pushState: function (state) {
        this._setStatePermalink(state);
        this.navigate(UrlSerializer.getURLFromState(state), { trigger: false });
    },

    replaceState: function (state) {
        this._setStatePermalink(state);
        this.navigate(UrlSerializer.getURLFromState(state), { trigger: false, replace: true });
    },

    _restoreSessionSearch: function (quoteId, quoteName, selectedAssetId) {
        // todo: implement search saving before using this
        // var sessionSearch = this.initialSessionSearchState,
        //     url = UrlSerializer.getURLFromState(sessionSearch || this.searchPageModule.getState());

        debugger;
        var url = UrlSerializer.getURLFromState(this.searchPageModule.getState());
        this.navigate(url, { replace: true, trigger: true });
    },

    /**
     * The "catch-all" route that distinguishes search and issue fragments.
     *
     * @param {string} path The path component of the URL (relative to the root)
     * @param {object} query The decoded querystring params
     * @private
     */
    _route: function (quoteId, quoteName, query) {
        var fragment = Backbone.history.getFragment();

        if (QuoteFlow.application.ignorePopState) {
            // Workaround for Chrome bug firing a null popstate event on page load.
            // Backbone should fix this!
            // @see http://code.google.com/p/chromium/issues/detail?id=63040
            // @see also JRADEV-14804
            return;
        }

        // Remove ignored parameters (e.g. focusedCommentId).
        var state = UrlSerializer.getStateFromURL(fragment);

        if (!this._navigateToLoginIfNeeded(state)) {
            this._navigateUsingState(state);
        }
    },

    _navigateToLoginIfNeeded: function (state, history) {
        // if (!this.usePushState(history) && state.selectedAssetKey && !JIRA.Issues.LoginUtils.isLoggedIn()) {
        //     var instance = this;
        //
        //     var requestParams = {};
        //     if (state.filter != null) {
        //         requestParams.filterId = state.filter;
        //     }
        //
        //     jQuery.ajax({
        //         url: AJS.contextPath() + "/rest/issueNav/1/issueNav/anonymousAccess/" + state.selectedAssetKey,
        //         headers: { 'X-SITEMESH-OFF': true },
        //         data: requestParams,
        //         success: function () {
        //             instance._navigateUsingState(state);
        //         },
        //         error: function (xhr) {
        //             if (xhr.status === 401) {
        //                 instance._redirectToLogin(state);
        //             } else {
        //                 instance._navigateUsingState(state);
        //             }
        //         }
        //     });
        //
        //     return true;
        // }

        return false;
    },

    _navigateUsingState: function (state) {
        // if (QuoteFlow.application.request("assetEditor:canDismissComment")) {
        //     this._setStatePermalink(state);
        //     this.navigate(UrlSerializer.getURLFromState(state), { replace: true, trigger: false });
        //     this.searchPageModule.updateFromRouter(state);
        // }

        this._setStatePermalink(state);
        this.navigate(UrlSerializer.getURLFromState(state), { replace: true, trigger: false });
        this.searchPageModule.updateFromRouter(state);
    },

    _redirectToLogin: function (state) {
        var url = AJS.contextPath() + "/login.jsp?permissionViolation=true&os_destination=" +
            encodeURIComponent(UrlSerializer.getURLFromState(state));

        window.location.replace(url);
    },

    /**
     * Set the permalink for a given state into AJS.Meta to be rendered by the share plugin
     */
    _setStatePermalink: function (state) {
        // var viewIssueState = _.pick(state, "selectedIssueKey");
        // var baseUrl = AJS.Meta.get("jira-base-url");
        // if (!_.isEmpty(viewIssueState)) {
        //     AJS.Meta.set("viewissue-permlink",
        //         baseUrl + "/" + UrlSerializer.getURLFromState(viewIssueState)
        //     );
        // }
        // var issueNavState = _.omit(state, "selectedIssueKey");
        // if (!_.isEmpty(issueNavState)) {
        //     AJS.Meta.set("issuenav-permlink",
        //         baseUrl + "/" + UrlSerializer.getURLFromState(issueNavState)
        //     );
        // }
    }
});

module.exports = AssetNavCustomRouter;
