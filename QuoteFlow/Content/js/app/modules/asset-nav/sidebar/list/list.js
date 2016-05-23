"use strict";

var Marionette = require('backbone.marionette');
var SidebarListLayout = require('./layout');
var SidebarListMessage = require('./message');
var SidebarListView = require('../line-items-list.js');

/**
 * This controller displays a list of line items.
 * @extends Marionette.Controller
 */
var SidebarListController = Marionette.Controller.extend({
    /**
     * Main view rendered by this controller
     * @type {JIRA.Components.Filters.Views.List.Module}
     */
    view: null,

    /**
     * @param {Object} options
     * @param {LineItems} options.collection Collection of line items
     */
    initialize: function (options) {
        this.collection = options.collection;
        this.title = options.title;
        this.className = options.className;
        this.errorMessage = options.errorMessage;
        this.loadingMessage = options.loadingMessage;
        this.emptyMessage = options.emptyMessage;
    },

    /**
     * Displays the filter's list in the provided element
     *
     * @param {jQuery} el Container for the filter's list
     */
    show: function (el) {
        if (this.view) {
            this.destroy();
        }

        this.view = new SidebarListLayout({
            el: el,
            title: this.title
        });
        this.view.render();

        this._showInternalView(this.collection.fetchState);
        this.listenTo(this.collection, "change:fetchState", this._showInternalView);
    },

    destroy: function () {
        if (this.view) {
            this.stopListening(this.collection, "change:fetchState", this._showRegion);
            this.stopListening(this.view);
            this.view.destroy();
            this.view = null;
        }
    },

    /**
     * Highlight a filter, unhighlights all the other filters     *
     * @param {LineItem} filterModel Model to highlight
     */
    highlightLineItem: function (filterModel) {
        if (this._listView) {
            this._listView.unhighlightAllFilters();
            if (filterModel) {
                this._listView.highlightLineItem(filterModel);
            }
        }
    },

    /**
     * Displays the internal view based on the state of the filter's collection.
     *
     * @param {string} fetchState Fetch state of the collection, valid values are "error", "fetched" or ""
     * @private
     */
    _showInternalView: function (fetchState) {
        switch (fetchState) {
            case "error":
                this._showError();
                break;
            case "fetched":
                this._showList();
                break;
            default:
                this._showLoading();
                break;
        }
    },

    _showLoading: function () {
        this.view.content.show(new SidebarListMessage({
            className: this.className,
            text: this.loadingMessage
        }));
    },

    _showError: function () {
        this.view.content.show(new SidebarListMessage({
            className: this.className,
            text: this.errorMessage
        }));
    },

    _showList: function () {
        var collection = this.collection;
        if (collection.length) {
            this._showListWithItems();
        } else {
            this._showEmptyList();
        }
    },

    _getListViewConstructor: function () {
        return SidebarListView;
    },

    _showListWithItems: function () {
        var ViewConstructor = this._getListViewConstructor();
        this._listView = new ViewConstructor({
            collection: this.collection,
            className: this.className
        });
        this.view.content.show(this._listView);
        this.trigger("render");

        // When we remove the last item from the collection, render the empty list
        this.listenTo(this.collection, "remove", function f () {
            if (!this.collection.length) {
                this.stopListening(this.collection, "remove", f);
                this._showEmptyList();
            }
        });

        this.listenTo(this._listView, "itemview:selectFilter", function (itemView, args) {
            this.trigger("selectFilter", args.model);
        });
        this.listenTo(this._listView, "itemview:render", function () {
            this.trigger("render");
        });

        this.triggerMethod("list:render", this._listView);
    },

    _showEmptyList: function () {
        this.view.content.show(new SidebarListMessage({
            className: this.className,
            text: this.emptyMessage
        }));

        // When we add a new item to the collection, render the list with items
        this.listenToOnce(this.collection, "add", function () {
            this._showListWithItems();
        });
    }
});

module.exports = SidebarListController;
