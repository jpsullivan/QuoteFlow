﻿"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

/**
 * Module for JQL query mode
 */
var AssetJqlQueryModule = Brace.Evented.extend({
    namedEvents: ["searchRequested", "verticalResize"],

    initialize: function(options) {
        this._queryStateModel = options.queryStateModel;
//        this.view = new JIRA.Issues.JqlQueryView({
//            queryStateModel: options.queryStateModel
//        })
//        .onVerticalResize(this.triggerVerticalResize, this)
//        .onSearchRequested(this.triggerSearchRequested, this);
//
//        /* Absolute hack to prevent DESK-1623 - after return to search, the jql box is thin cause issue nav is hidden when
//           rendered so height calculation is wrong. We need to trigger it to recalculate height on return to search.
//           I have added a method, refreshLayout to the query component which we now call from issue-nav-plugin
//           SearchPageModule, however jira can be using a newer version of issue-nav-components that issue-nav-plugin
//           (installed via service desk). So we need this nasty hack until the minimum version of jira service desk
//           supports has the updateLayout call inside of SearchPageModule.
//         */
//        JIRA.bind(JIRA.Events.NEW_CONTENT_ADDED, _.bind(function (e, el, reason) {
//            if (reason === JIRA.CONTENT_ADDED_REASON.returnToSearch) {
//                this.setQuery();
//            }
//        }, this));
    },

    search: function () {
        var jql = this.view.readJql();
        this.triggerSearchRequested(jql);
    },

    setQuery: function () {
        this.view.setQuery();
    },

    createView: function () {
        return this.view;
    }
});

module.exports = AssetJqlQueryModule;