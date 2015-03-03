"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Marionette = require('backbone.marionette');

var AssetNavCreator = require('./search/asset-nav-creator');

/**
 * Contains callbacks for the asset module router.
 */
var AssetNavController = Marionette.Controller.extend({

    builder: function () {
        var options = this.initOptions();
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
    },

    /**
     * Equivalent of AssetNavInit.js
     */
    initOptions: function () {
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
        if (initialIssueTableState && !initialIssueTableState.Table) {
            var wrapper = AJS.$("<div></div>").append($navigatorContent.children().clone());
            initialIssueTableState.Table = wrapper.html();
        }

        var initialIssueIds = AJS.$('#stableSearchIds').data('ids');
        var selectedIssue = $navigatorContent.data("selected-issue");

        var criteriaJson = jQuery("#criteriaJson").text();
        var systemFiltersJson = jQuery("#systemFiltersJson").text();
        var initialSearcherCollectionState = _.isEmpty(criteriaJson) ? null : JSON.parse(criteriaJson);
        var initialSessionSearchState = $navigatorContent.data("session-search-state");
        var systemFilters = _.isEmpty(systemFiltersJson) ? null : JSON.parse(systemFiltersJson);

        var options = _.extend({}, {
            initialIssueTableState: initialIssueTableState,
            initialIssueIds: initialIssueIds,
            selectedIssue: selectedIssue,
            initialSearcherCollectionState: initialSearcherCollectionState,
            initialSessionSearchState: initialSessionSearchState,
            systemFilters: systemFilters
        });

        return options;
    },
});

module.exports = AssetNavController;

