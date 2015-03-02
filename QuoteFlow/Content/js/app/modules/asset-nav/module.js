"use strict";

var _ = require('underscore');

var Marionette = require('backbone.marionette');

var AssetController = require('./controller');
var AssetNavCreator = require('./search/asset-nav-creator');
var AssetRouter = require('./router');

/**
 * 
 */
var AssetNavModule = Marionette.Module.extend({

    onStart: function (options) {
        options = this.init(options);
        return this.startMediator(options);
    },

    /**
     * Equivalent of AssetNavInit.js
     */
    init: function(options) {
        var $navigatorContent = AJS.$(".navigator-content");

        /**
         * Read all the initial data in the DOM
         *
         * If the ColumnConfigState has been sent from the server we want to take the HTML from the table
         * and pop it onto its table property.
         *
         * This prevents us from having to populate the HTML twice in the dom. Once in the HTML and another time in the
         * JSON. It also prevents us needing to ensure there are no XSS vulnerabilities in the JSON HTML string.
         */
        var initialIssueTableState = $navigatorContent.data("issue-table-model-state");
        if (initialIssueTableState && !initialIssueTableState.table) {
            var wrapper = AJS.$("<div></div>").append($navigatorContent.children().clone());
            initialIssueTableState.assetTable.table = wrapper.html();
        }

        var initialIssueIds = AJS.$('#stableSearchIds').data('ids');
        var selectedIssue = $navigatorContent.data("selected-issue");

        // jQuery.parseJSON gracefully returns null given an empty string.
        // Would be even nicer if the json was placed in a data- attribute, which jQuery will automatically parse with .data().
        var initialSearcherCollectionState = jQuery.parseJSON(jQuery("#criteriaJson").text());
        var initialSessionSearchState = $navigatorContent.data("session-search-state");
        var systemFilters = jQuery.parseJSON(jQuery("#systemFiltersJson").text());

        _.extend(options, {
            initialIssueTableState: initialIssueTableState,
            initialIssueIds: initialIssueIds,
            selectedIssue: selectedIssue,
            initialSearcherCollectionState: initialSearcherCollectionState,
            initialSessionSearchState: initialSessionSearchState,
            systemFilters: systemFilters
        });

        return options;
    },

    startMediator: function (options) {
        var creator = AssetNavCreator.create(AJS.$(document), {
            initialIssueTableState: options.initialIssueTableState,
            initialSearcherCollectionState: options.initialSearcherCollectionState,
            initialSessionSearchState: options.initialSessionSearchState,
            initialSelectedIssue: options.selectedIssue,
            initialIssueIds: options.initialIssueIds,
            systemFilters: options.systemFilters
        });

        /**
         * Some shenanigans to get get table to resize with window gracefully. Sets the width of the issue navigator results
         * wrapper. Keeps the right hand page elements within the browser view when the results table is wider than the browser view.
         */
        var bodyMinWidth = parseInt(jQuery('body').css('minWidth'), 10);
        jQuery(document).bind('resultsWidthChanged', function () {
            var $contained = jQuery('.contained-content');
            var $containedParent = $contained.parent();

            if ($contained.length > 0) {
                var containedLeft = $contained.offset().left;
                var target = Math.max(window.innerWidth, bodyMinWidth) - containedLeft;
                var targetPct = target / $containedParent.width() * 100;
                if (targetPct < 100) {
                    $contained.css('width', targetPct + '%');
                } else {
                    $contained.css('width', '');
                }
                jQuery(document).trigger('issueNavWidthChanged');
            }
        });

//        // Trigger the event on page load to make sure the controls are visible.
//        AJS.$(document).trigger("resultsWidthChanged");
//
//        JIRA.Issues.onVerticalResize(function () {
//            jQuery.event.trigger("updateOffsets.popout");
//        });
//
//        // When switching layouts we need to update the height of sidebar
//        JIRA.bind(JIRA.Events.LAYOUT_RENDERED, function () {
//            _.defer(function () {
//                jQuery.event.trigger("updateOffsets.popout");
//            });
//        });

        this.controller = new AssetController();
        return new AssetRouter({
            controller: this.controller,
            searchPageModule: this.searchPageModule
        });
    }
});

module.exports = AssetNavModule;