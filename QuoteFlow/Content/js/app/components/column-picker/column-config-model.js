"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

/**
 * 
 */
var ColumnConfigModel = Brace.Model.extend({
    namedAttributes: [
        "name",
        "columns",
        "autoUpdate",
        "savedColumns",
        "description",
        "previousColumns",
        "actionBarText",
        "isActive"
    ],

    namedEvents: [
        "columnsSync"
    ],

    defaults: {
        actionBarText: "Restore Defaults",
        isActive: false
    },

    idAttribute: "name",

    initialize: function (attributes, options) {
        _.extend(this, options);
    },

    /**
     * Get the defaults columns, parsing the response from defaultColumns().
     * The promise can be resolved with the list of columns (Array.string) or
     * rejected with a null value
     * 
     * @returns {jQuery.Promise}
     */
    getDefaultColumns: function () {
        var deferred = jQuery.Deferred();

        this.defaultColumns().done(_.bind(function (response) {
            var parsedResponse = this.parse(response);
            if (parsedResponse && parsedResponse.columns) {
                deferred.resolve(parsedResponse.columns);
            } else {
                deferred.reject(null);
            }
        }, this));

        return deferred.promise();
    },

    /**
     * Parses the REST response to extract our model values.
     *
     * @param {Object} response REST response
     */
    parse: function (response) {
        if (!response) {
            return {};
        }

        return {
            columns: _.compact(_.pluck(response, "value"))
        };
    },

    /**
     * Checks if the column config is disabled.
     *
     * For example, "filter" column config is disabled if there is no active filter.
     *
     * By default, this method returns false (i.e. all column config are enabled). It can be
     * overridden when creating this model
     *
     * @returns {boolean}
     */
    isDisabled: function () {
        return false;
    },

    /**
     * Checks if the column config edition is disabled.
     *
     * For example, "filter" column config edition is disabled if the user does not own the filter.
     *
     * By default, this method returns false (i.e. all column config edit are enabled). It can be
     * overridden when creating this model
     *
     * @returns {boolean}
     */
    isEditDisabled: function () {
        return false;
    },

    /**
     * Returns a promise that will resolve with a list of defaults columns.
     *
     * This method will be used if the columnConfig contains no columns. By default,
     * the returned promise resolves immediately to an empty array.
     *
     * This method can be overridden when constructing this model
     *
     * @returns {jQuery.Promise}
     */
    defaultColumns: function () {
        var deferred = jQuery.Deferred();
        deferred.resolve([]);
        return deferred.promise();
    },

    /**
     * Sorts a list of columns preserving the original order, using the following pattern:
     *    - Columns already present will keep their order
     *    - New columns will be append at the end
     *
     * Example:
     *    - Current columns ["a", "b", "c"]
     *    - New columns ["c", "d", "a"]
     *
     *    - Result ["a", "c", "d"]
     *
     * @param {Array.string} currentColumns Columns already present in the model
     * @param {Array.string} newColumns New columns to sort
     * @returns {Array.string} Sorted columns
     * @private
     */
    _sortColumnsUsingOriginalOrder: function (currentColumns, newColumns) {
        return _.intersection(currentColumns, newColumns).concat(_.difference(newColumns, currentColumns));
    },

    /**
     * Sets an unsorted list of columns in the model.
     *
     * This method will store the new list of columns, preserving the order of the previous column list.
     *
     * @param {Array.<string>} columns Columns to update the model with
     */
    setUnsortedColumns: function (columns) {
        columns = this._sortColumnsUsingOriginalOrder(this.getColumns(), columns);
        //TODO Check if this can be moved to backbone's previous() functionality
        this.setPreviousColumns(this.getColumns(), { silent: true });
        this.setColumns(columns);
    },

    /**
     * Creates a JSON representation for this model.
     *
     * This method will be used by Backbone when saving a model. We don't want to save the entire model, just
     * the columns, so the output only contains that attribute.
     *
     * @returns {{columns: Array.<string>}}
     */
    toJSON: function () {
        return {
            columns: this.getColumns()
        };
    },

    revertUnsavedColumns: function (opts) {
        if (this.getSavedColumns() !== this.getColumns()) {
            this.setColumns(this.getSavedColumns(), opts);
        }
    },

    loadDefaultColumns: function () {
        this.getDefaultColumns().done(_.bind(function (columns) {
            this.setColumns(columns);
        }, this));
    },

    shouldRefreshSearchOnActivation: function () {
        return true;
    },

    shouldCloseOnActivation: function () {
        return true;
    },

    shouldLoadDefaultsOnActivation: function () {
        return false;
    },

    shouldRevertOnHide: function () {
        return true;
    }
});

ColumnConfigModel.create = function (name, description, overriddenMethods, autoUpdate) {
    return new ColumnConfigModel({
        autoUpdate: autoUpdate,
        name: name,
        description: description
    }, overriddenMethods);
};

module.exports = ColumnConfigModel;