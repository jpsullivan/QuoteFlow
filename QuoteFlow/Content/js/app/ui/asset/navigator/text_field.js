"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

// Data Layer
var NavigationCriteria = require('./criteria');
var CriteriaModel = require('../../../models/asset/criteria');

// UI Components
var BaseView = require('../../../view');

/**
 *
 */
var NavigatorTextField = BaseView.extend({
    el: '.text-query-container input',

    events: {
        keypress: 'handleKeypress'
    },

    initialize: function() {
        _.bindAll(this, "updateSearcherCollectionTextField", "render");

        this.collection.on("remove change add", _.bind(function (a) {
            if (a.getId() === this.collection.QUERY_ID) {
                this.render();
            }
        }, this));

        this.collection.onTextFieldChanged(this.render);
        this.collection.onRequestUpdateFromView(this.updateSearcherCollectionTextField);
        this.collection.onInteractiveChanged(this.handleInteractiveChanged);
    },

    render: function() {
        var a = this.collection.get(this.collection.QUERY_ID);
        if (a) {
            var c = a.getEditHtml();
            var b = AJS.$("<div></div>").html(c || "").text();
            this.setQuery(b);
        } else {
            this.setQuery("");
        }
    },

    handleKeypress: function (e) {
        // enter key detection
        if (e.keyCode === 13) {
            this.collection.handleBasicViewSubmit();
            e.preventDefault();
        }
    },

    setQuery: function(query) {
        this.$el.valueOf(query);
    },

    updateSearcherCollectionTextField: function() {
        if (this.$el.is('input')) {
            var query = AJS.$.trim(this.$el.val());
            this.collection.updateTextQuery(query);
        }
    },

    handleInteractiveChanged: function (a) {
        this.$el.prop("disabled", !a);
    }
});

module.exports = NavigatorTextField;