"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var expandOnInput = require('jquery-expand-on-input');

var JQLAutoCompleteView = require('./jql-autocomplete-view');
var Meta = require('../../../util/meta');

/**
 * Renders the JQL textarea.
 */
var JqlQueryView = Brace.View.extend({
    template: JST["quote-builder/query/jql-query-view"],

    namedEvents: ["verticalResize", "searchRequested"],

    events: {
        "expandedOnInput": "_handleExpandOnInput"
    },

    initialize: function (options) {
        this.queryStateModel = options.queryStateModel;
        this.queryStateModel.on("change:jql", this.setQuery, this);
        AJS.$(document).bind('issueNavWidthChanged', _.bind(this._resizeHeight, this));

        if (JQLAutoCompleteView) {
            this.JQLAutoCompleteView = new JQLAutoCompleteView({ model: this.queryStateModel });
            this.JQLAutoCompleteView.onJqlValid(function (jql) {
                if (this.queryStateModel.getAdvancedAutoUpdate()) {
                    this.triggerSearchRequested(jql);
                }
            }, this);
            this.JQLAutoCompleteView.onSearchRequested(this.triggerSearchRequested, this)
        }
    },

    render: function () {

        this.$el.html(this.template({
            helpUrl: AJS.Meta.get('advanced-search-help-url'),
            helpTitle: AJS.Meta.get('advanced-search-help-title'),
            hasSearchButton: this.queryStateModel.hasSearchButton()
        }));

        this.$el.addClass("loading");

        if (this.JQLAutoCompleteView) {
            this.JQLAutoCompleteView.setElement(this.$el.find(".advanced-search"));
            this.JQLAutoCompleteView.getJqlAutoCompleteData();
        }

        new JIRA.Issues.Tipsy({
            el: this.$el.find(".search-button"),
            tipsy: {
                trigger: "hover",
                delayIn: 300
            }
        });

        this.setQuery();
        return this;
    },

    readJql: function () {
        var $inputField = this._getInputField();
        var jql = $inputField.val() || "";

        // Prettify input field with trimmed JQL
        var trimmedJql = AJS.$.trim(jql);
        if (jql !== trimmedJql) {
            $inputField.val(trimmedJql);
        }
        this._resizeHeight();
        return trimmedJql;
    },

    _handleExpandOnInput: function () {
        this.triggerVerticalResize();
    },

    _resizeHeight: function () {
        var $input = this._getInputField();
        // Need to set the height of the input to 0 for expandOnInput to reliably expand.
        // However, expandOnInput doesn't change the height of empty inputs, so need to handle those a little differently.
        if ($input.val()) {
            $input.height(0).expandOnInput();
        } else {
            $input.expandOnInput().height(0).trigger('refreshInputHeight');
        }

    },

    setQuery: function () {
        this.$el.removeClass("loading"); // Just in case we have come here from basic, loading class might be persisted. So remove.
        this._getInputField().val(this.queryStateModel.getJql());
        this._getInputField().trigger("updateParseIndicator");
        this._resizeHeight();
    },

    focus: function () {
        this._getInputField().focus();
    },

    _getInputField: function () {
        return this.$el ? this.$el.find("textarea") : AJS.$();
    }
});

module.exports = JqlQueryView;
