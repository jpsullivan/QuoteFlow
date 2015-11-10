"use strict";

var Brace = require('backbone-brace');
var AssetTableHeaderOperationsView = require('../../../components/table/header-operations-view');

/**
 * Interface to the search header.
 */
var SearchHeaderModule = Brace.Evented.extend({

    initialize: function (options) {
        this._searchPageModule = options.searchPageModule;
    },

    registerSearch: function (search) {
        this._search = search;
    },

    createToolsView: function ($toolsEl) {
        new AssetTableHeaderOperationsView({
            el: $toolsEl,
            search: this._search,
            searchPageModule: this._searchPageModule
        });
    }
});

module.exports = SearchHeaderModule;
