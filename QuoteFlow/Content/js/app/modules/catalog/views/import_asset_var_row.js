"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Marionette = require('backbone.marionette');
var AuiSelect2 = require('@atlassian/aui/lib/js/aui/select2');

var PanelTable = require('../../../ui/common/panel-table');

/**
 * [extend description]
 * @extends Marionette.ItemView
 */
var ImportAssetVarRow = Marionette.ItemView.extend({
    tagName: 'tr',

    template: JST['catalog/import-asset-var-row'],

    initialize: function (options) {
        this.headers = options.headers;
        this.assetVar = options.assetVar;
    },

    serializeData: function () {
        return {
            assetVar: this.assetVar.toJSON(),
            catalogHeaders: this.headers
        };
    },

    onRender: function() {
        _.defer(function() {
            AJS.$('select').auiSelect2();
        });
    },

    changeHeader: function(e) {
        var el = $(e.currentTarget);
        var index = el.prop('selectedIndex');

        if (index === 0) {
            return;
        }

        var valueType = el.parent('.field-group').data('value-type');

        this.validateHeaderSelection(index, valueType);
    },

    /**
     * Determines if a random sample of the CSV rows passes the value type check.
     * This is of course a dirty check that doesn't guarantee 100% exact
     * results, but assuming that the input data isn't total garbage, should
     * yield correct estimations.
     */
    validateHeaderSelection: function(index, valueType) {
        // ahem...
    },

    /**
     * Gathers a random collection of data from the selected
     * header group and displays it in a table.
     */
    showPreview: function(e) {
        var el = $(e.currentTarget);
        var fieldGroup = el.parent();

        var valueType = fieldGroup.data('value-type');
        var index = $('select', fieldGroup).prop('selectedIndex');
        var panelKey = $('.aui-lozenge', fieldGroup).html();

        if (index === 0) {
            //            // show an inline popup
            //            AJS.InlineDialog(AJS.$("select", fieldGroup), "myDialog",
            //                function(content, trigger, showPopup) {
            //                    content.css({ "padding": "20px" }).html('<p>Please select a field header.</p>');
            //                    showPopup();
            //                    return false;
            //                }
            //            );
            return false;
        }

        index = index - 1; // -1 to compensate for the default select opt

        var panelView = new PanelTable({
            leftHeader: "Asset Key",
            rightHeader: "Sample Values",
            rowKey: panelKey,
            rowData: this.getSampleRowData(index)
        });

        $('.sample-data-container', fieldGroup).html(panelView.render().el);
    },

    /**
     *
     */
    getSampleRowData: function(index) {
        return _.sample(this.rows.pluck(index), 3);
    },

    /**
     * Removes the asset var row from the table. Disposes the view.
     */
    removeRow: function() {
        this.destroy();
    }
});

module.exports = ImportAssetVarRow;
