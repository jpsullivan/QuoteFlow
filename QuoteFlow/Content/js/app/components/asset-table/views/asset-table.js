"use strict";

var Marionette = require('backbone.marionette');

/**
 * A table of assets.
 *
 * @extends Marionette.ItemView
 */
var AssetTableView = Marionette.ItemView.extend({
    /**
     * @event columnsChanged
     * When the order of the columns has changed
     */

    /**
     * @event highlightAsset
     * When the user wants to highlight an asset
     */

    /**
     * @event sort
     * When the user clicks on the column's header
     */

    events: {
        "blur .hidden-link": function (e) {
            jQuery(e.target).attr("tabIndex", -1);
        },
        "click tr.assetrow": function (e) {
            var row = jQuery(e.target).closest(".assetrow");
            var assetId = Number(row.attr("rel"));
            this.trigger("highlightAsset", assetId);
        },
        "click .sortable": function (e) {
            e.preventDefault();
            this.trigger("sort", jQuery(e.currentTarget).data("id"));
        }
    },

    /**
     *
     * @param {object} options Configuration object
     * @param {jQuery} options.resultsTable Server-side rendered table with the results
     * @param {Object} [options.sortOptions] State of the current sort options
     * @param {string} options.sortOptions.fieldId ID of the field used for sorting the results
     * @param {string} options.sortOptions.order Direction used for the sorting ("DESC", "ASC")
     */
    initialize: function (options) {
        this.resultsTable = options.resultsTable;
        this.sortOptions = options.sortOptions || {};
    },

    /**
     * Render the table of assets.
     *
     * @returns {JIRA.Components.IssueTable.Views.IssueTable} <tt>this</tt>
     */
    render: function () {
        this.triggerMethod("before:render", this);
        var table = jQuery(this.resultsTable)[0];
        this.$el.empty().append(table);
        this.triggerMethod("render", this);
        return this;
    },

    onRender: function () {
        this._decorateTableHeaderWithSortOptions();
        this._addDraggable();
        this._removeEmptyTextNodes();
    },

    onClose: function () {
        this._removeDraggable();
    },

    /**
     * Adds header decorations for sorted column.
     */
    _decorateTableHeaderWithSortOptions: function () {
        var $sortableColumns = this.$el.find('.rowHeader .sortable');
        $sortableColumns.each(function () {
            this.onclick = null;
        });

        var sortOptions = this.sortOptions;
        if (sortOptions.fieldId) {
            // Locate the element which should be decorated
            var $sortEl = $sortableColumns.filter(function () {
                return this.getAttribute("data-id") === sortOptions.fieldId;
            });

            // If the sorting element is not already decorated
            if ($sortEl.size() && !$sortEl.hasClass("descending") && !$sortEl.hasClass("ascending")) {
                // Clean any header with sort-related classes
                $sortableColumns
                    .removeClass("descending ascending active")
                    .addClass("colHeaderLink");

                // Decorate the element as sorted
                var direction = sortOptions.order === "DESC" ? "descending" : "ascending";
                $sortEl.removeClass('colHeaderLink').addClass('active ' + direction);
            }
        }
    },

    _addDraggable: function () {
        this.$("#assettable").dragtable({
            maxMovingRows: 1,
            containment: 'body',
            axis: false,
            revert: false,
            clickDelay: 250,
            tolerance: "intersect",
            dragaccept: ":not(.headerrow-actions)",
            persistState: _.bind(this._saveColumns, this)
        });
    },

    _removeDraggable: function () {
        this.$("#assettable").dragtable("destroy");
    },

    /**
     * Fix the asset table element so it displays correctly in IE9.
     *
     * Cells can become misaligned in IE9 if the table's markup contains whitespace.
     * Only affects IE9. IE10 and IE11 are okay.
     */
    _removeEmptyTextNodes: function () {
        this.$el.find("table, tbody, thead, tr").contents().filter(function () {
            return this.nodeType === 3; // Node.TEXT_NODE
        }).remove();
    },

    _saveColumns: function () {
        var cols = [];
        this.$("#assettable").find('th').each(function () {
            var id = jQuery(this).data('id');
            if (_.isNotBlank(id)) {
                cols.push(id);
            }
        });
        this.trigger("columnsChanged", cols);
    },

    /**
     * Highlight an asset and scroll it into view.
     *
     * Triggers a "highlightAsset" event, passing the asset's ID.
     *
     * @param {number} assetId The ID of the asset to highlight.
     * @param {boolean} [focus=true] Whether the highlighted asset should have the focus
     */
    highlightAsset: function (assetId, focus) {
        var newRow = this._getAssetRow(assetId);
        var oldRow = this.$(".focused");

        oldRow.removeClass("focused");
        newRow.addClass("focused");

        if (newRow.length && newRow.closest(document.body).length) {
            // Focus the row so you can tab through its links.
            if (focus !== false) {
                newRow.find(".hidden-link").removeAttr("tabIndex").focus();
            }
            newRow.scrollIntoView({ marginBottom: 50 });
        }
    },

    /**
     * Gets the row element that represents a particular asset.
     *
     * @param {number} assetId ID of the asset to look for
     * @param {jQuery} [container=this.$el] Table that contain the asset's row
     * @returns {jQuery} The row element
     */
    _getAssetRow: function (assetId, container) {
        container = container || this.$el;
        return container.find(". assetrow").filter(function () {
            return this.getAttribute("rel") == assetId;
        });
    },

    /**
     * Updates an asset in the table with a new row and highlights it.
     *
     * @param {number} assetId ID of the asset to update
     * @param {jQuery} newTable Server rendered table with the new data
     */
    updateAsset: function (assetId, newTable) {
        var $newRow = this._getAssetRow(assetId, jQuery(newTable));
        this._getAssetRow(assetId).replaceWith($newRow);

        this.highlightAsset(assetId);
        this.trigger("assetRowUpdated", $newRow);
    }
});

module.exports = AssetTableView;