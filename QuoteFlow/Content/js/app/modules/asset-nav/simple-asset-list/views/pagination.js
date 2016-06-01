"use strict";

var Marionette = require('backbone.marionette');
var URI = require('urijs');

var PaginationView = Marionette.ItemView.extend({
    template: JST["quote-builder/simple-asset-list/pagination"],

    ui: {
        prev: ".icon-previous",
        next: ".icon-next",
        page: "a[data-page]"
    },

    triggers: {
        "click @ui.prev": "prev",
        "click @ui.next": "next"
    },

    events: {
        "click @ui.page": function (e) {
            var val = e.target.getAttribute("data-page");
            e.preventDefault();
            this.trigger("goto", parseInt(val, 10) - 1);
        }
    },

    onRender: function () {
        this.unwrapTemplate();
    },

    update: function (collection) {
        this.collection = collection;
        this.render();
    },

    serializeData: function () {
        if (!this.collection) {
            // Because on the very first render, we don't have the collection yet.
            return;
        }

        var url = new URI(this.options.baseURL);
        var hasQueryString = url.query().length > 0;

        return {
            startIndex: this._getStartIndex(),
            prevStartIndex: this._getPrevStartIndex(),
            prevPageIsNotFirstPage: this._getPrevStartIndex() >= 0,
            hasAnotherPage: this._hasAnotherPage(),
            pageSize: this.collection.state.pageSize,
            searchQuery: url.toString(),
            hasQueryString: hasQueryString,
            displayableTotal: this.collection.state.totalRecords,

            pageNumber: this._getPageNumber(),
            startPage: this._getStartPage(),
            endPage: this._getEndPage(),
            lastPage: this._getLastPage(),
            urlFragment: this._getUrlFragment(),
            currentPage: this._getPageNumber() + 1,
            maxPagesDisplayed: this.MAX_PAGES_DISPLAYED,

            prevPageTitle: this._getPrevPageTitle(),
            nextPageTitle: this._getNextPageTitle()
        };
    },

    MAX_PAGES_DISPLAYED: 5,

    _getNextStartIndex: function () {
        return Math.min(this._getStartIndex() + this.collection.state.pageSize, this.collection.state.totalRecords);
    },

    _hasAnotherPage: function () {
        return this._getNextStartIndex() < this.collection.state.totalRecords;
    },

    _getStartIndex: function () {
        return this.collection.state.pageSize * this.collection.state.currentPage;
    },

    _getPrevStartIndex: function () {
        return this._getStartIndex() - this.collection.state.pageSize;
    },

    _getPageNumber: function () {
        return Math.floor();
    },

    _getStartPage: function () {
        return this._getPageNumber() - Math.floor(this.MAX_PAGES_DISPLAYED / 2);
    },

    _getEndPage: function () {
        return this._getPageNumber() + Math.floor(this.MAX_PAGES_DISPLAYED / 2);
    },

    _getLastPage: function () {
        return Math.floor((this.collection.state.totalRecords - 1) / this.collection.state.pageSize);
    },

    _getUrlFragment: function () {
        var url = new URI(this.options.baseURL);
        var hasQueryString = url.query().length > 0;

        if (hasQueryString) {
            return url.toString() + "&startIndex=";
        }

        return url.toString() + "?startIndex=";
    },

    _getPrevPageTitle: function () {
        var pageNumber = this._getPageNumber();
        var lastPage = this._getLastPage() + 1;
        return "Go to page " + pageNumber + " of " + lastPage;
    },

    _getNextPageTitle: function () {
        var pageNumber = this._getPageNumber() + 2;
        var lastPage = this._getLastPage() + 1;
        return "Go to page " + pageNumber + " of " + lastPage;
    }
});

module.exports = PaginationView;
