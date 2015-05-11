"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

var SmartAjax = require('../../components/ajax/smart-ajax');

/**
 *
 */
var AssetSearcherModel = Brace.Model.extend({
    /**
     * id: searcher id
     * name: The name of the clause
     * viewHtml: html to display in the criteria selector button, or the text to display in the input for the text searcher.
     * editHtml: html to display in the edit dropdown
     * groupId: group id
     * groupName: group name
     * isSelected: whether or not the searcher has been selected as a criteria for a search
     * jql: jql representation of the clause
     * position: The searcher's position in the extended criteria view.
     * validSearcher: is entire searcher valid for current search context
     * lastViewed: the time the searcher was last viewed in milliseconds
     */
    namedAttributes: [
        "id",
        "name",
        "isShown",
        "viewHtml",
        "editHtml",
        "groupId",
        "groupName",
        "initParams",
        "isSelected",
        "jql",
        "position",
        "serializedParams",
        "validSearcher",
        "key",
        "lastViewed"
    ],

    /**
     * readyForDisplay: edit html has been retrieved and is ready to be displayed
     */
    namedEvents: ["readyForDisplay"],

    initialize: function () {},

    parse: function (json) {
        if (json.viewHtml) {
            json.viewHtml = this._cleanViewHtml(json.viewHtml);
        }
        return json;
    },

    /**
     * @param {Boolean} [forceUpdate=false] Force update of the JQL, even if autoupdate is disabled
     * @returns {*}
     */
    createOrUpdateClauseWithQueryString: function (forceUpdate) {
        return this.collection.createOrUpdateClauseWithQueryString(this.id, forceUpdate);
    },

    getQueryString: function () {
        var params = {};
        // custom handling for text query
        if (this.collection.QUERY_ID === this.getId()) {
            if (this.getViewHtml()) {
                params[this.collection.QUERY_PARAM] = this.getDisplayText(); // query string shouldn't be html-encoded
                return AJS.$.param(params);
            }
            return null;
        }

        // return jql for invalid searchers as the server doesn't return editHtml if a searcher is invalid, but it does return jql
        if ((!this.getValidSearcher() || /^\s*$/.test(this.getEditHtml())) && this.getJql()) {
            params = {};
            params[this.collection.JQL_INVALID_QUERY_PREFIX + this.getId()] = this.getJql();
            return AJS.$.param(params);
        }

        return this.getSerializedParams();
    },

    /**
     * Returns just the text of the viewHtml, cleaning up whitespace.
     */
    getDisplayText: function () {
        var html = this.getViewHtml();
        var text = '';
        if (html) {
            var $container = AJS.$('<div>').appendCatchExceptions(html);
            text = AJS.$.trim($container.text()).replace(/[\n\r\s]+/g, ' ');
        }
        return text;
    },

    hasClause: function () {
        if (this.collection.QUERY_ID === this.getId()) {
            return !!this.getViewHtml();
        }
        else if (!this.getValidSearcher()) {
            return !!this.getJql();
        }
        else {
            return !!this.getQueryString();
        }
    },

    /**
     * Reset the searcher's state.
     */
    clearSearchState: function () {
        this.set({
            viewHtml: null,
            editHtml: null,
            jql: null,
            validSearcher: null,
            isSelected: false
        });
    },

    /**
     * Returns the current time in milliseconds. Cannot always use Date.now() because of IE8.
     */
    _now: Date.now || function () {
        return new Date().getTime();
    },

    select: function () {
        this.set({
            isSelected: true,
            position: this.collection.getNextPosition(),
            validSearcher: true,
            lastViewed: this._now()
        });
    },

    /**
     * Ensures edit html exists. Triggers readyForDisplay when editHtml has been retrieved, which may be asynchronous if the value has not been retrieved,
     * or immediate if we already have editHtml
     */
    retrieveEditHtml: function() {
        var jql = this.collection.createJql();

        // TODO: Abort a pending request if there is one.
        // Avoid race condition -- ensure last request received is also last request issued.

        /* EditHtml is always populated when we request the list of searchers from the server,
         * for all searchers that have a value. We will always have editHtml when first showing a searcher.
         * this.getEditHtml() will be null in 2 cases:
         * - when we have just added a new searcher and it has no value
         * - when we have closed a dialog and are waiting for the list of searchers (and new editHtml) to update.
         * The second case is done because we *don't* cache the editHtml DOM elements. When showing an edit dialog
         * we have to regenerate the DOM - however because the elements might have changed (eg values entered into
         * inputs) we clear this.editHtml on hide (see this.clearEditHtml) so that it will be re-requested on show.
         */

        if (this.getEditHtml()) {
            return jQuery.Deferred().resolve(this.getEditHtml());
        }

        return SmartAjax.makeRequest({
            type: "POST",
            url: QuoteFlow.RootUrl +"/secure/QueryComponentRendererEdit!Default.jspa",
            success: _.bind(this.setEditHtml, this),
            dataType: "html",
            error: JIRA.Issues.displayFailSearchMessage,
            data: {
                fieldId: this.getId(),
                decorator: "none",
                jqlContext: jql
            }
        });
    },

    /**
     * Returns true if this element has edit html and it contains an error class
     */
    hasErrorInEditHtml: function() {
        var $el = jQuery('<div>').htmlCatchExceptions(this.getEditHtml());
        if (!$el) {
            return false;
        } else {
            return $el.find(".error, .has-errors").length > 0;
        }
    },

    /**
     * @return the text to be shown in the searcher's tooltip.
     */
    getTooltipText: function () {
        if (this.getValidSearcher !== false) {
            var value = this.getDisplayText() || AJS.I18n.getText("issues.components.query.search.all");
            return this.getName() + ": " + value;
        } else {
            return AJS.I18n.getText("issues.components.query.searcher.invalid.searcher");
        }
    },

    /**
     * Wait any in flight updates to search collection.
     */
    searchersReady: function () {
        return this.collection.searchersReady()
    }
});

module.exports = AssetSearcherModel;
