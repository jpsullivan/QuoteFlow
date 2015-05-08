"use strict";

var Marionette = require('backbone.marionette');

/**
 * Renders a message that tells the user there are no assets, with an optional link to create an asset.
 *
 * @extends Marionette.ItemView
 *
 * @param {object} options Options
 * @param {boolean} options.quoteflowHasAssets Whether there are assets created in this QuoteFlow instance
 */
var EmptyResultsView = Marionette.ItemView.extend({
    template: JST["quote-builder/table/no-results"],

    serializeData: function () {
        var message;
        var hint;
        var cssClass;
        var linkType;
        //var createIssuePerm = JIRA.Issues.UserParms.get().createIssue;
        var createAssetPerm = true;

         if (this.options.quoteflowHasAssets === false) {
            message = "No assets have been created (yet)";
            hint = createAssetPerm ? "Be the first to <a>create an asset</a>" : null;
            cssClass = "empty-results-message";
            this.linkType = 'create';
        } else {
            message = "No assets were found to match your search";
            hint = createAssetPerm ?
                "Try modifying your search criteria or <a>creating a new asset</a>" :
                AJS.I18n.getText('Try modifying your search criteria');
            cssClass = "no-results-message";
            this.linkType = 'create';
        }

        return {
            message: message,
            hint: hint,
            cssClass: cssClass
        };
    },

    onRender: function () {
        this.$el.addClass("empty-results");

        var $links = this.$('.no-results-hint a');
        var isLoggedIn = true;
        if (!isLoggedIn) {
            //$links.attr('href', JIRA.Issues.LoginUtils.redirectUrlToCurrent()).addClass('login-link');
        } else {
            $links.addClass('create-issue').attr('href', AJS.contextPath() + "/secure/CreateIssue!default.jspa");
        }

        this.hidePending();
    },

    showPending: function () {
        this.$el.addClass('pending');
    },

    hidePending: function () {
        this.$el.removeClass("pending");
    }
});

module.exports = EmptyResultsView;
