"use strict";

var _ = require('underscore');

var ColumnConfigModel = require('../column-picker/column-config-model');
var ColumnPickerComponent = require('../column-picker/column-picker');

/**
 * A module that controls the column picker model and the column picker view in a single interface.
 */
var ColumnPicker = ColumnPickerComponent.extend({
    initialize: function (attr, options) {
        var providers = [this._getUserColumnProvider(), this._getFilterColumnProvider()];
//        if (JIRA.isAdmin()) {
//            providers.push(this._getSystemColumnProvider());
//        }
        ColumnPickerComponent.prototype.initialize.call(this, null, {
            providers: providers
        });
        this.search = options.search;
        this.search.on("change:filter", function () {
            var filterModel = this.filterColumn;
            var isEditDisabled = filterModel.isEditDisabled();
            var isColumnConfigDisabled = filterModel.isDisabled();
            filterModel.trigger("change:isDisabled", filterModel, isColumnConfigDisabled);
            filterModel.trigger("change:editDisabled", filterModel, isColumnConfigDisabled || isEditDisabled);
        }, this);
    },

    /**
     * Is the system filter column config in play.
     * @returns {boolean}
     */
    isSystemMode: function () {
        return this.columnPickerModel.getColumnConfig() === "system";
    },

    sanitizeColumnConfig: function (columnConfig) {
        columnConfig = columnConfig.toLowerCase();
        var shouldOverrideSystem = columnConfig == "system" && !this.isSystemMode();
        return shouldOverrideSystem ? "user" : columnConfig;
    },

    syncColumns: function (newColumnConfigName, columns) {
        newColumnConfigName = this.sanitizeColumnConfig(newColumnConfigName);
        return ColumnPickerComponent.prototype.syncColumns.call(this, newColumnConfigName, columns);
    },

    clearFilterConfiguration: function () {
        this.filterColumn.unset('columns');
        this.filterColumn.unset('savedColumns');
    },

    setCurrentColumnConfig: function (columnConfig) {
        columnConfig = this.sanitizeColumnConfig(columnConfig);
        return ColumnPickerComponent.prototype.setCurrentColumnConfig.call(this, columnConfig);
    },

    _getFilterColumnProvider: function () {
        var instance = this;
        return this.filterColumn = ColumnConfigModel.create(
            "filter",
            AJS.I18n.getText("issues.components.column.config.filter.columns"),
            {
                url: function () {
                    return '/rest/api/2/filter/' + instance.search.getState().filter + '/columns';
                },
                isDisabled: function () {
                    return !instance.search.getFilter() || instance.search.getFilter().getIsSystem();
                },
                isEditDisabled: function () {
                    return !instance.search.filterModule.canEditColumns();
                },
                shouldRefreshSearchOnActivation: function () {
                    //If the filter has configured columns, refresh the search on activation
                    return this.has("savedColumns");
                },
                shouldCloseOnActivation: function () {
                    //If the filter has configured columns, close the column picker on activation
                    return this.has("savedColumns");
                },
                shouldLoadDefaultsOnActivation: function () {
                    //If the filter has not  configured columns, load the default columns
                    return !this.has("savedColumns");
                },
                defaultColumns: _.bind(function () {
                    //If a filter doesn't have columns, then it will use the user columns by default
                    //Need to retrieve the user columns and set that as the selected columns
                    if (this.userColumns.has("columns")) {
                        var deferred = jQuery.Deferred();
                        deferred.resolve(_.map(this.userColumns.getColumns(), function (item) { return { value: item } }));
                        return deferred.promise();
                    } else {
                        return jQuery.ajax(this.userColumns.url()).promise();
                    }
                }, this)
            }
        )
    },

    _getSystemColumnProvider: function () {
        //Adding a toggle for 'system' columns if user is an admin

        return ColumnConfigModel.create(
            "system",
            AJS.I18n.getText('issues.components.column.config.system.columns'),
            {
                url: _.lambda('/rest/api/2/settings/columns'),
                defaultColumns: function () {
                    //Load columns from our REST endpoint
                    return jQuery.ajax(this.url()).promise();
                },
                shouldRefreshSearchOnActivation: function () {
                    //Always refresh the search on activation
                    return true;
                },
                shouldCloseOnActivation: function () {
                    //Never close the column picker on activation
                    return false;
                },
                shouldLoadDefaultsOnActivation: function () {
                    //Always load the default columns
                    return true;
                },
                shouldRevertOnHide: function () {
                    return false;
                }
            }
        );
    },

    _getUserColumnProvider: function () {
        this.userColumns = ColumnConfigModel.create("user",
            "issues.components.column.config.user.columns",
            {
                url: _.lambda('/rest/api/2/user/columns')
            }
        );
        return this.userColumns;
    }
});

ColumnPicker.create = function(options) {
    return new ColumnPicker(null, options);
};

module.exports = ColumnPicker;