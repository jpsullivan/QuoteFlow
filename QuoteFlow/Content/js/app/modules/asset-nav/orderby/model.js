"use strict";

var $ = require('jquery');
var Brace = require('backbone-brace');

/**
 * Model for order by dropdown & toggle
 *
 * @type JIRA.Issues.OrderByModel
 */
var OrderByModel = Brace.Model.extend({

    namedEvents: ["sort"],
    namedAttributes: ["sortBy", "jql"],

    initialize: function () {
        this.on("change:jql", this.updateSelectedSort, this);
    },

    /**
     * Whenever we change jql get the data we need to display the description of the order (the ASC/DEC toggle)
     */
    updateSelectedSort: function () {
      console.warn("todo: perform orderby request on jql change");
        // jQuery.ajax({
        //     type: "POST",
        //     url: AJS.contextPath() + "/rest/orderbycomponent/latest/orderByOptions/primary",
        //     data: JSON.stringify({ jql: this.getJql() }),
        //     contentType: 'application/json',
        //     success: _.bind(function (res) {
        //         this.setSortBy(res);
        //     }, this)
        // });
    },

    /**
     * Toggles sort jql between DESC/ASC
     */
    toggleSort: function () {
        this.setJql(this.getSortBy().toggleJql);
        this.triggerSort(this.getJql());
    },

    /**
     * Sets updated jql and publishes event
     * @param jql
     */
    doSort: function (jql) {
        this.setJql(jql);
        this.triggerSort(this.getJql());
    }
});

module.exports = OrderByModel;
