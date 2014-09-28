"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

var BaseView = require('../../view');

/**
 *
 */
var PanelTable = BaseView.extend({
    className: "panel",

    templateName: 'common/panel-table',

    options: {
        leftHeader: "",
        rightHeader: "",
        rowKey: "",
        rowData: []
    },

    events: {},

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
            leftHeader: this.options.leftHeader,
            rightHeader: this.options.rightHeader,
            bodyRows: this.bodyRows
        });
    },

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