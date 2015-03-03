"use strict";

var Marionette = require('backbone.marionette');

/**
  * Creates a shifter group factory for search criteria.
  *
  * @param {object} options
  * @param {function} options.isBasicMode A function that returns true iff basic mode is selected.
  * @param {function} options.isFullScreenIssue A function that returns true iff a full screen issue is visible.
  * @param {SearcherCollection} options.searcherCollection The application's searcher collection.
  * @return {function} A shifter group factory suitable to be passed to <tt>JIRA.Shifter.register</tt>.
  */
var SearchShifter = function(options) {
    var getSuggestions,
        onSelection,
        shouldShow,
        toSuggestion;

    getSuggestions = function () {
        var suggestions = options.searcherCollection.chain()
            .filter(shouldShow)
            .map(toSuggestion)
            .value();

        return function () {
            return jQuery.Deferred().resolve(suggestions).promise();
        };
    };

    onSelection = function (id) {
        var currentSearcher = JIRA.Issues.SearcherDialog.instance.getCurrentSearcher(),
            searcher = options.searcherCollection.get(id);

        if (!searcher.getIsSelected()) {
            searcher.select();
        }

        // toggle closes the dialog if it's open, so ensure that's not the case.
        if (!currentSearcher || currentSearcher.getId() !== searcher.getId()) {
            JIRA.Issues.SearcherDialog.instance.toggle(searcher);
        }
    };

    // Determine whether the given searcher should be suggested.
    shouldShow = function (searcherModel) {
        return searcherModel.getIsShown();
    };

    // Create a shifter suggestion from a SearcherModel.
    toSuggestion = function (searcherModel) {
        return {
            label: searcherModel.getName(),
            value: searcherModel.getId()
        }
    };

    return function () {
        // Only show suggestions if we're in basic mode and the search criteria are visible.
        if (!options.isBasicMode() || options.isFullScreenIssue()) {
            return null;
        }

        return {
            getSuggestions: getSuggestions(),
            name: "shifter.group.searchcriteria",
            onSelection: onSelection,
            weight: 150
        };
    };
};

module.exports = SearchShifter;