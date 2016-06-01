"use strict";

var Marionette = require('backbone.marionette');
var MarionetteViewManager = require('../../../../mixins/marionette-viewmanager');

var AssetListView = require('../views/asset-list');
var EndStableSearch = require('../views/end-stable-search');
var Layout = require('../views/layout');
var Loading = require('../views/loading');
var Pagination = require('../views/pagination');
var Refresh = require('../views/refresh');

var SimpleAssetListController = MarionetteViewManager.extend({
    _buildPagination: function () {
        return this.buildView("pagination", function () {
            var view = new Pagination({
                baseURL: this.baseURL
            });
            this.listenTo(view, {
                "prev": function () {
                    this.trigger("goToPreviousPage");
                },
                "next": function () {
                    this.trigger("goToNextPage");
                },
                "goto": function (page) {
                    this.trigger("goToPage", page);
                }
            });
            return view;
        });
    },

    _buildList: function () {
        return this.buildView("list", function () {
            var view = new AssetListView();
            this.listenTo(view, {
                "childview:select": function (childview) {
                    var model = childview.model;
                    this.trigger("selectAsset", {
                        id: model.get('id'),
                        sku: model.get('sku')
                    });
                },
                "update": function () {
                    this.trigger("update");
                }
            });
            return view;
        });
    },

    _buildEndStableSearch: function () {
        return this.buildView("endOfStableSearch", function () {
            return new EndStableSearch();
        });
    },

    _buildRefresh: function () {
        return this.buildView("refresh", function () {
            var refresh = new Refresh();
            this.listenTo(refresh, {
                "refresh": function () {
                    this.trigger("refresh");
                }
            });
            return refresh;
        });
    },

    _buildLayout: function () {
        return this.buildView("layout", function () {
            var view = new Layout();
            this.listenTo(view, {
                "render": function () {
                    // Render the sub-views
                    this.getView("layout").getRegion("pagination").show(this.getView("pagination"));
                    this.getView("layout").getRegion("searchResults").show(this.getView("list"));
                    this.getView("layout").getRegion("refresh").show(this.getView("refresh"));

                    // OrderBy is a special case, as it is not a view but an external component
                    this.getView("layout").getRegion("orderBy")._ensureElement();
                    this.orderBy.setElement(this.getView("layout").getRegion("orderBy").$el);
                    this.orderBy.render();

                    if (this.inlineIssueCreate) {
                        this.getView("layout").inlineIssueCreateContainer._ensureElement();
                        this.inlineIssueCreate.show(this.getView("layout").inlineIssueCreateContainer.$el);
                    }
                }
            });
            return view;
        });
    },

    _buildOrderBy: function () {
        this.orderBy = JIRA.Components.OrderBy.create();

        this.orderBy.onSort(function (jql) {
            this.trigger("sort", jql);
        }, this);

        return this.orderBy;
    },

    _buildViews: function () {
        // Build the views
        this._buildLayout();
        this._buildPagination();
        this._buildList();
        this._buildRefresh();
        this._buildEndStableSearch();

        // Render the layout
        this.getView("layout").render();
    },

    initialize: function (options) {
        // As OrderBy is not an internal view, we can create and reuse it when needed.
        this._buildOrderBy();
        this.baseURL = options.baseURL;
        this.inlineIssueCreate = options.inlineIssueCreate;
        this._buildViews();
    },

    render: function (options) {
        options = options || {};
        var el = options.el;
        el.append(this.getView("layout").$el);
        this.getView("layout").setElement(el);
    },

    onDestroy: function () {
        this.hideView("layout");
    },

    update: function (searchResults) {
        // Display the EndOfStableSearch message if needed
        if (searchResults.isAtTheEndOfStableSearch()) {
            this.getView("layout").getRegion("endOfStableMessageContainer").show(this.getView("endOfStableSearch"));
        } else {
            this.getView("layout").getRegion("endOfStableMessageContainer").empty();
        }

        // Update the views with the search results
        this.getView("list").update(searchResults);
        this.getView("refresh").render();
        this.getView("pagination").update(searchResults);
        this.orderBy.setElement(this.getView("layout").getRegion("orderBy").$el);
        this.orderBy.render();
        this.orderBy.setJql(searchResults.jql);

        if (this.inlineIssueCreate) {
            this.inlineIssueCreate.render();
        }

        // Make sure everything fits on the screen
        this.adjustSize();
    },

    hideLoading: function () {
        this.hideView("loading");
    },

    showLoading: function () {
        this.showView("loading", function () {
            var view = new Loading();
            this.listenTo(view, {
                "render": function () {
                    this.getView("layout").$el.prepend(view.$el);
                }
            });
            return view;
        });
    },

    updateIssue: function (model) {
        this.getView("list").updateIssue(model);
    },

    unselectIssue: function (issueId) {
        this.getView("list").unselectIssue(issueId);
    },

    selectIssue: function (issueId) {
        this.getView("list").selectIssue(issueId);
    },

    adjustSize: function () {
        if (!this.getView("layout")) {
            return;
        }
        if (!this.getView("list")) {
            return;
        }

        var listPanel = this.getView("layout").ui.listPanel;
        var listContent = this.getView("list").$el;
        var paginationContainer = this.getView("layout").getRegion("pagination").$el;

        var offsetTop = listContent.offset().top + listPanel.scrollTop();
        var paginationHeight = (paginationContainer ? paginationContainer.outerHeight() : 0);
        var windowHeight = window.innerHeight;
        var endOfStableSearchMessageHeight = (!this.getView("endOfStableSearch") ? 0 : this.getView("endOfStableSearch").$el.outerHeight());
        var inlineIssueCreateHeight = this.getView("layout").inlineIssueCreateContainer.$el.outerHeight() || 0;

        listPanel.css("height", windowHeight - offsetTop - endOfStableSearchMessageHeight - paginationHeight - inlineIssueCreateHeight);

        this.getView("list").scrollSelectedIssueIntoView();
    }
});

module.exports = SimpleAssetListController;
