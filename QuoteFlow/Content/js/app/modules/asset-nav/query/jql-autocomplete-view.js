"use strict";

var Brace = require('backbone-brace');

/**
 * 
 */
var JQLAutoCompleteView = Brace.View.extend({

    namedEvents: ["jqlValid", "jqlInvalid", "searchRequested"],

    events: {
        "focus": "_onFocus"
    },

    getJqlAutoCompleteData: function () {
        var deferred = jQuery.Deferred();
        if (this.jqlFieldNames == null || this.jqlFunctionNames == null || this.jqlReservedWords == null) {
            var $jqlFieldNames = jQuery("#jqlFieldz");
            var $jqlFunctionNames = jQuery("#jqlFunctionNamez");
            var $jqlReservedWords = jQuery("#jqlReservedWordz");
            if ($jqlFieldNames.length > 0 && $jqlFunctionNames.length > 0 && $jqlReservedWords.length > 0) {
                this.jqlFieldNames = JSON.parse($jqlFieldNames.text());
                this.jqlFunctionNames = JSON.parse($jqlFunctionNames.text());
                this.jqlReservedWords = JSON.parse($jqlReservedWords.text());
            } else {
                AJS.$.ajax({
                    url: QuoteFlow.RootUrl + "/rest/querycomponent/latest/jqlAutoComplete"
                }).done(_.bind(function (response) {
                    var jqlAutoCompleteData = JSON.parse(response);
                    this.jqlFieldNames = _.reject(JSON.parse(jqlAutoCompleteData.jqlFieldz), _.bind(function (item) {
                        return _.contains(this.model.getWithout(), item.value);
                    }, this));
                    this.jqlFunctionNames = JSON.parse(jqlAutoCompleteData.jqlFunctionNamez);
                    this.jqlReservedWords = JSON.parse(jqlAutoCompleteData.jqlReservedWordz);
                    deferred.resolve();
                }, this));
                return deferred.promise();
            }
        }
        return deferred.resolve().promise();
    },

    _initJQLAutoComplete: function () {
        var $advSearch = this.$el;
        var jqlAutoComplete = JIRA.JQLAutoComplete({
            fieldID: $advSearch.attr("id"),
            parser: JIRA.JQLAutoComplete.MyParser(this.jqlReservedWords),
            queryDelay: .65,
            jqlFieldNames: this.jqlFieldNames,
            jqlFunctionNames: this.jqlFunctionNames,
            minQueryLength: 0,
            allowArrowCarousel: true,
            autoSelectFirst: false,
            errorID: 'jqlerrormsg'
        });

        var instance = this;

        $advSearch.keypress(function (event) {
            if (jqlAutoComplete.dropdownController === null || !jqlAutoComplete.dropdownController.displayed || jqlAutoComplete.selectedIndex < 0) {
                if (event.keyCode == 13 && !event.ctrlKey && !event.shiftKey) {
                    event.preventDefault();
                    jqlAutoComplete.dropdownController.hideDropdown();
                    // Dodgy but the JQL auto complete has stopped propagation of the keypress event. And we need to let
                    // the query module know a search has been requested.
                    instance.triggerSearchRequested($advSearch.val());
                }
            }
        });

        var instance = this;
        var oldUpdateParseIndicator = jqlAutoComplete.updateParseIndicator;
        jqlAutoComplete.updateParseIndicator = function (token) {
            oldUpdateParseIndicator.apply(this, arguments);
            if (!token.getParseError()) {
                instance.triggerJqlValid($advSearch.val());
            } else {
                instance.triggerJqlInvalid($advSearch.val());
            }
        };


        jqlAutoComplete.buildResponseContainer();
        jqlAutoComplete.parse($advSearch.val());
        jqlAutoComplete.updateColumnLineCount();


        $advSearch.bind('expandedOnInput', function () {
            jqlAutoComplete.positionResponseContainer();
        }).bind("updateParseIndicator", function () {
            jqlAutoComplete.parse($advSearch.val());
        }).click(function () {
            jqlAutoComplete.dropdownController.hideDropdown();
        });
    },

    _onFocus: function () {
        // The renderer may destroy the old <textarea> and insert a new one, so we'll need to
        // init JQL autocompletion anytime this property is not set.
        if (!this.model.getAutocompleteEnabled() || this.$el.data("JQLAutoComplete_init")) {
            return;
        }
        this.$el.data("JQLAutoComplete_init", true);
        this.getJqlAutoCompleteData().done(_.bind(this._initJQLAutoComplete, this));
    }
});

module.exports = JQLAutoCompleteView;