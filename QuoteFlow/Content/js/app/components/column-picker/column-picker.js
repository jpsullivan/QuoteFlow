"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

var ColumnConfigModel = require('./column-config-model');
var ColumnPickerModel = require('./column-picker-model');
var ColumnPickerView = require('./column-picker-view');

/**
 * A module that controls the column picker model and the column picker view in a single interface.
 */
var ColumnPickerComponent = Brace.Model.extend({
    initialize: function (attr, options) {
        this.columnPickerModel = new ColumnPickerModel({ autoUpdate: options.autoUpdate });
        this.columnPickerView = new ColumnPickerView({ columnPickerModel: this.columnPickerModel });

        _.each(options.providers, _.bind(function (descriptor) {
            if (descriptor instanceof ColumnConfigModel) {
                this.columnPickerModel.addColumnProvider(descriptor.getName(), descriptor);
            } else {
                this.columnPickerModel.addColumnProvider(descriptor.id, this._createProviderModel(descriptor));
                if (descriptor.columns) {
                    this.columnPickerModel.syncColumns(descriptor.id, descriptor.columns);
                }
            }
        }, this));

        this.setCurrentColumnConfig(options.providers[0].id);
        if (options.el) {
            this.columnPickerView.setElement(options.el).render();
        }
    },

    _createProviderModel: function (descriptor) {
        return ColumnConfigModel.create(descriptor.id, descriptor.label,
            _.omit(descriptor, "id", "label", "columns"));
    },

    getCurrentColumnConfig: function () {
        return this.columnPickerModel.getCurrentColumnConfig();
    },

    setElement: function ($el) {
        this.columnPickerView.setElement($el);
        return this;
    },
    render: function () {
        this.columnPickerView.render();
        return this;
    },
    clearFilterConfiguration: function () {
        this.columnPickerModel.clearFilterConfiguration();
        return this;
    },
    adjustHeight: function () {
        this.columnPickerView.adjustHeight();
        return this;
    },
    setCurrentColumnConfig: function (name) {
        this.columnPickerModel.setCurrentColumnConfig(name);
        return this;
    },
    saveColumns: function (cols) {
        this.columnPickerModel.saveColumns(cols);
        return this;
    },
    syncColumns: function (name, columns) {
        this.columnPickerModel.syncColumns(name, columns);
        return this;
    },
    getColumnConfig: function () {
        this.columnPickerModel.getColumnConfig();
    },
    onColumnsSync: function (func, ctx) {
        this.columnPickerModel.onColumnsSync(func, ctx);
        return this;
    },
    on: function (evt, func, ctx) {
        this.columnPickerModel.on(evt, func, ctx);
        return this;
    },
    off: function (evt, func) {
        this.columnPickerModel.off(evt, func);
        return this;
    }
});

ColumnPickerComponent.create = function(options) {
    return new ColumnPickerComponent(null, options);
};

module.exports = ColumnPickerComponent;