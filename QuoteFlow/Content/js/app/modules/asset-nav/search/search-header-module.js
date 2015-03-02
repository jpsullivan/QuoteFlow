"use strict";

var Brace = require('backbone-brace');

/**
 * Interface to the search header.
 */
var SearchHeaderModule = Brace.Evented.extend({

    initialize: function (options) {
        this._searchPageModule = options.searchPageModule;
    },

    registerSearch: function (search) {
        this._search = search;
    }
});

module.exports = SearchHeaderModule;