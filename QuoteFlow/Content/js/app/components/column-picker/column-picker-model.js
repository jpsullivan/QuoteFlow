"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

/**
 * 
 */
var ColumnPickerModel = Brace.Model.extend({
    namedAttributes: [
        /** The source of the column configuration, e.g. "user", "filter", "explicit" */
        "columnConfig",
        "savedColumnConfig",
        "search",
        "availableColumns",
        "autoUpdate"
    ],

    namedEvents: [
        "columnsSync",
        "changeColumnConfigDisabled",
        "destroyColumnConfig"
    ],

    allColumnsUrl: '/rest/gadget/1.0/availableColumns',


    shouldRefreshSearchOnActivation: function () {
        return this.getCurrentColumnConfig().shouldRefreshSearchOnActivation();
    },

    shouldCloseOnActivation: function () {
        return this.getCurrentColumnConfig().shouldCloseOnActivation();
    },

    shouldLoadDefaultsOnActivation: function () {
        return this.getCurrentColumnConfig().shouldLoadDefaultsOnActivation();
    },

    shouldRevertOnHide: function () {
        return this.getCurrentColumnConfig().shouldRevertOnHide();
    },

    loadDefaultColumns: function () {
        this.getCurrentColumnConfig().loadDefaultColumns();
    },

    initialize: function () {
        this.columnsData = {};
    },

    refreshSearchWithColumns: function (columnConfigModel) {
        columnConfigModel = columnConfigModel || this.getCurrentColumnConfig();
        this.triggerColumnsSync(columnConfigModel.getName(), columnConfigModel.getColumns());
    },

    fetchAvailableColumnsIfNeeded: function () {
        if (!this.has("availableColumns")) {
            jQuery.ajax(this.allColumnsUrl).done(_.bind(function (response) {
                this.setAvailableColumns(response.availableColumns);
            }, this));
        }
    },

    getCurrentColumnConfig: function (name) {
        if (!name) {
            name = this.getColumnConfig();
        }
        return this.columnsData[name];
    },

    revertColumnConfig: function () {
        if (this.shouldRevertOnHide() && this.getSavedColumnConfig() != this.getColumnConfig()) {
            //HACK
            //BackboneJS does not update the 'previousAttributes' if the change is silent. That means
            //the next time we change columnConfig (non-silent change), we will get the wrong value.
            this._previousAttributes["columnConfig"] = this.getSavedColumnConfig();
            this.setColumnConfig(this.getSavedColumnConfig(), { silent: true });
        }
    },

    setCurrentColumnConfig: function (columnConfig) {
        this.setColumnConfig(columnConfig);
        this.setSavedColumnConfig(columnConfig);
    },

    syncColumns: function (newColumnConfigName, columns) {
        _.each(this.columnsData, function (columnConfigModel) {
            if (columnConfigModel.getName() == newColumnConfigName) {
                columnConfigModel.setColumns(columns);
                columnConfigModel.setSavedColumns(columns);
            }
        });
    },

    /**
     * Save a list of columns in the current ColumnConfig.
     * 
     * This method will save the list of columns in the provided order. In other words,
     * the previous order is deleted.
     * 
     * @param {Array.string} cols Columns to save
     */
    saveColumns: function (cols) {
        this.getCurrentColumnConfig().setColumns(cols);
        this.getCurrentColumnConfig().save();
        this.getCurrentColumnConfig().triggerColumnsSync();
    },


    addColumnProvider: function (id, provider) {
        this.columnsData[id] = provider;

        provider.on("columnsSync sync destroy", _.bind(function () {
            this.refreshSearchWithColumns(provider);
        }, this));

        provider.on("change:isDisabled", _.bind(function (model, isDisabled) {
            this.triggerChangeColumnConfigDisabled(model, isDisabled);
        }, this));

        provider.on("destroy", _.bind(function (model) {
            this.triggerDestroyColumnConfig(model);
        }, this));
    }
});

module.exports = ColumnPickerModel;