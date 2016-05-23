"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Marionette = require('backbone.marionette');

var AssetNavCreator = require('./search/asset-nav-creator');
var Message = require('../../ui/common/message');
var Resize = require('./util/resize');
var SmartAjax = require('../../components/ajax/smart-ajax');

// component initializers
var InitSparklers = require('./searchers/initSparklers');
var InitUserSparkler = require('./searchers/initUserSparkler');

/**
 * Contains callbacks for the asset module router.
 */
var AssetNavController = Marionette.Controller.extend({

    builder: function (query) {
        // a horrible, horrible hack to prevent this from loading
        // on other pages besides the quote builder
        var shouldStart = $('#startAssetNav').length > 0;
        if (!shouldStart) {
            return;
        }

        // initialize renderable components here (such as sparklers)
        InitSparklers.register();
        InitUserSparkler.register();

        var options = this.initOptions();
        var creator = AssetNavCreator.create(AJS.$(document), {
            initialAssetTableState: options.initialAssetTableState,
            initialSearcherCollectionState: options.initialSearcherCollectionState,
            initialSessionSearchState: options.initialSessionSearchState,
            initialSelectedAsset: options.selectedAsset,
            initialAssetIds: options.initialAssetIds,
            systemFilters: options.systemFilters,
            lineItems: options.lineItems
        });

        /**
         * Some shenanigans to get get table to resize with window gracefully.
         * Sets the width of the asset navigator results wrapper.
         * Keeps the right hand page elements within the browser view when the
         * results table is wider than the browser view.
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

       // Trigger the event on page load to make sure the controls are visible.
        AJS.$(document).trigger("resultsWidthChanged");

        QuoteFlow.Interactive.onVerticalResize(function () {
            $.event.trigger("updateOffsets.popout");
        });

    //    // When switching layouts we need to update the height of sidebar
    //    JIRA.bind(JIRA.Events.LAYOUT_RENDERED, function () {
    //        _.defer(function () {
    //            jQuery.event.trigger("updateOffsets.popout");
    //        });
    //    });

        /**
         * Determines the appropriate message to show upon search ajax failure
         *
         * @param xhr XHR object from jQuery.ajax
         */
        QuoteFlow.displayFailSearchMessage = function (xhr) {
            if (xhr && xhr.statusText !== "abort") {
                return Message.showErrorMsg(SmartAjax.buildSimpleErrorContent(xhr), {
                    closeable: true
                });
            }
        };
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
        var initialAssetTableState = $navigatorContent.data("issue-table-model-state");
        if (initialAssetTableState && !initialAssetTableState.table) {
            var wrapper = AJS.$("<div></div>").append($navigatorContent.children().clone());
            initialAssetTableState.table = wrapper.html();
        }

        var initialAssetIds = AJS.$('#stableSearchIds').data('ids');
        var selectedAsset = $navigatorContent.data("selected-issue");

        var criteriaJson = jQuery("#criteriaJson").text();
        var systemFiltersJson = jQuery("#systemFiltersJson").text();
        var lineItemsJson = jQuery("#lineItemsJson").text();

        var initialSearcherCollectionState = _.isEmpty(criteriaJson) ? null : JSON.parse(criteriaJson);
        var initialSessionSearchState = $navigatorContent.data("session-search-state");
        var systemFilters = _.isEmpty(systemFiltersJson) ? null : JSON.parse(systemFiltersJson);
        var lineItems = _.isEmpty(lineItemsJson) ? null : JSON.parse(lineItemsJson);

        var options = _.extend({}, {
            initialAssetTableState: initialAssetTableState,
            initialAssetIds: initialAssetIds,
            selectedAsset: selectedAsset,
            initialSearcherCollectionState: initialSearcherCollectionState,
            initialSessionSearchState: initialSessionSearchState,
            systemFilters: systemFilters,
            lineItems: lineItems
        });

        return options;
    }
});

module.exports = AssetNavController;
