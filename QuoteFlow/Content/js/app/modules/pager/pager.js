"use strict";

var Marionette = require('backbone.marionette');
var MarionetteViewManager = require('../../mixins/marionette-viewmanager');
var View = require('./views/pager');

var Pager = MarionetteViewManager.extend({
    _buildSearchResults: function (searchResults) {
        this._destroySearchResults();

        this.searchResults = searchResults;
        this.listenTo(this.searchResults, {
            "select reset": function () {
                this.getView("view").update(this.searchResults);
            }
        });
    },

    _destroySearchResults: function () {
        if (!this.searchResults) {
            return;
        }

        this.stopListening(this.searchResults);
        delete this.searchResults;
    },

    _buildView: function () {
        this.buildView("view", function () {
            var view = new View({
                el: this.el
            });
            this.listenTo(view, {
                "next": function () {
                    this.trigger("next");
                },
                "previous": function () {
                    this.trigger("previous");
                }
            });
            return view;
        });
    },

    initialize: function (options) {
        this.el = options.el;

        this._buildView();
    },

    onDestroy: function () {
        Marionette.ViewManager.prototype.onDestroy.call(this);
        this._destroySearchResults();
    },

    load: function (searchResults) {
        this._buildSearchResults(searchResults);
    },

    show: function (el) {
        this.hideView("view");
        this.showView("view", function () {
            var view = new View({
                el: el
            });
            this.listenTo(view, {
                "next": function () {
                    this.trigger("next");
                },
                "previous": function () {
                    this.trigger("previous");
                }
            });
            return view;
        });
    }
});

module.exports = Pager;
