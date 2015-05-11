"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

/**
 * Represents current and preferred search modes (basic or jql)
 */
var AssetQueryStateModel = Brace.Model.extend({
    BASIC_SEARCH: "basic",
    ADVANCED_SEARCH: "advanced",

    namedAttributes: [
        "style",
        "searchMode",
        "preferredSearchMode",
        "jql",
        "without",
        "layoutSwitcher",
        "autocompleteEnabled",
        "advancedAutoUpdate",
        "basicAutoUpdate",
        "basicOrderBy"
    ],

    defaults: {
        searchMode: "basic",
        preferredSearchMode: "basic"
    },

    /**
     * Sets search mode
     * @param searchMode search mode (basic or advanced)
     */
    switchToSearchMode: function (searchMode) {
        this.setSearchMode(searchMode);
    },

    /**
     * Changes the preferred and actual search mode and saves the preferred search mode.
     */
    switchPreferredSearchMode: function (mode) {
        this.switchToSearchMode(mode);
        this.setPreferredSearchMode(mode);
        this._savePreferredSearchMode();
    },

    /**
     * Switches to whatever is the preferred search mode
     */
    switchToPreferredSearchMode: function () {
        this.switchToSearchMode(this.getPreferredSearchMode());
    },

    hasSearchButton: function () {
        return this.getStyle() !== "field";
    },

    /**
     * Should the more criteria button be subtly styled
     */
    hasSubtleMoreCriteria: function () {
        return this.getStyle() !== "field";
    },

    /**
     * Persists preferred search mode to the server
     */
    _savePreferredSearchMode: function () {
        jQuery.ajax({
            url: QuoteFlow.RootUrl + "/rest/querycomponent/latest/userSearchMode", // IssueTableResource (JIRA core)
            type: 'POST',
            data: {
                searchMode: this.getPreferredSearchMode()
            },
            error: _.bind(function (xhr) {
                if (QuoteFlow.displayFailSearchMessage) {
                    QuoteFlow.displayFailSearchMessage(xhr);
                }
            }, this),
            success: function () {
                console.log("quoteflow.search.mode.changed");
            }
        });
    }
});

module.exports = AssetQueryStateModel;
