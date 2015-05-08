"use strict";

var Brace = require('backbone-brace');

var QuerySwitcherViewModel = require('../switcher/query-switcher-view-model');
var SwitcherView = require('../switcher/view');
var SwitcherCollection = require('../switcher/collection');

/**
 * This renders the container for either the basic query view or the advanced query view.
 *
 * @see JqlQueryView
 * @see BasicQueryView
 */
var QueryView = Brace.View.extend({

    namedEvents: ["verticalResize"],

    template: JST["quote-builder/query/query"],

    events: {
        "click .search-button" : "search",
        "submit" : "preventDefault"
    },

    initialize: function (options) {
        this.queryStateModel = options.queryStateModel;

        var switcherCollection = new SwitcherCollection([{
            id: this.queryStateModel.BASIC_SEARCH,
            name: "Basic",
            view: options.basicQueryModule.createView()
        }, {
            id: this.queryStateModel.ADVANCED_SEARCH,
            name: "Advanced",
            view: options.jqlQueryModule.createView()
        }]);

        // TODO: hack for search() below, to fix
        this.jqlQueryModule = options.jqlQueryModule;

        this.switcherViewModel = new QuerySwitcherViewModel({
            collection: switcherCollection
        }, {
            queryStateModel: this.queryStateModel
        });

        this.switcherView = new SwitcherView({
            template: JST["quote-builder/query/search-switcher"],
            model: this.switcherViewModel,
            containerClass: ".search-container"
        }).onVerticalResize(this.triggerVerticalResize, this)

    },

    render: function() {
        this.$el.html(this.template(this.queryStateModel.toJSON()));
        this.switcherView.setElement(this.$el).render();
        return this;
    },

    preventDefault: function (e) {
        e.preventDefault();
    },

    /**
     * Clear the notifications area (errors and warnings).
     */
    clearNotifications: function() {
        this.$(".notifications").empty();
    },

    /**
     * Performs a search with a query defined by the value of the textarea.
     *
     * This calls search on the model, only if we don't have an active saved search,
     * or if the query was changed from the current active saved search.
     *
     * @param e {Event} The submit event.
     */
    search: function() {
        // TODO: temp hack for jql query view rework
        if (this.queryStateModel.ADVANCED_SEARCH === this.switcherViewModel.getSelected().get('id')) {
            this.jqlQueryModule.search();
        }
        else {
            this.getView().search();
        }
    },

    getView: function() {
        return this.switcherViewModel.getSelected().getView();
    },

    /**
     * Display one or more errors in the notification area.
     *
     * @param {Array} errors The error(s) to be displayed.
     */
    showErrors: function(errors) {
        this.$(".notifications").append(JIRA.Templates.Issues.ComponentUtil.errorMessage({messages: errors}));
    },

    /**
     * Display one or more warnings in the notification area.
     *
     * @param {Array} warnings The warning(s) to be displayed.
     */
    showWarnings: function(warnings) {
        this.$(".notifications").append(JIRA.Templates.Issues.ComponentUtil.warningMessage({messages: warnings}));
    },

    /**
     * Hide or show the view (excluding error messages).
     *
     * @param isVisible Whether the view should be visible.
     */
    setVisible: function(isVisible) {
        this.$(".search-container").toggle(isVisible);
        this.switcherView.setVisible(isVisible);
    }
});

module.exports = QueryView;
