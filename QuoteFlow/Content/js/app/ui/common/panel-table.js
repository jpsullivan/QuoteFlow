"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

/**
 *
 */
var PanelTable = Marionette.ItemView.extend({
    className: "panel",

    template: JST['common/panel-table'],

    templateHelpers: function () {
        return {
            leftHeader: this.options.leftHeader,
            rightHeader: this.options.rightHeader,
            bodyRows: this.bodyRows
        };
    },

    options: {
        leftHeader: "",
        rightHeader: "",
        rowKey: "",
        rowData: []
    },

    events: {},

    initialize: function(options) {
        this.bodyRows = this.buildTableRows();
    },

    /**
     * Takes the unformatted row data and converts it into
     * an iterable object that handlebars can easily consume.
     * @return {object} The formatted body content.
     */
    buildTableRows: function() {
        var rows = [], key = this.options.rowKey;

        _.each(this.options.rowData, function(row) {
            rows.push({ "rowKey": key, "rowData": row });
        });

        return rows;
    }
});

module.exports = PanelTable;