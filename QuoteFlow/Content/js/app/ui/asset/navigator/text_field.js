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
        _.bindAll(this,
            "_updateSearcherCollectionTextField",
            "_handleInteractiveChanged",
            "render");

        this.collection.on("remove change add", _.bind(function (model) {
            if (model.getId() === this.collection.QUERY_ID) {
                this.render();
            }
        }, this));
        this.collection.onTextFieldChanged(this.render);
        this.collection.onRequestUpdateFromView(this._updateSearcherCollectionTextField);
        this.collection.onInteractiveChanged(this._handleInteractiveChanged);
    },

    render: function() {
        // Attempt to extract the query from the model's edit HTML. If that
        // fails, just fall back to its display value (used in tests too).
        var model = this.collection.get(this.collection.QUERY_ID);

        if (model) {
            // the html is just a raw value html encoded
            var val = model.getEditHtml();
            var decodedVal = AJS.$('<div></div>').html(val || '').text();
            this.setQuery(decodedVal);
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

    setQuery: function (query) {
        this.$el.val(query);
    },

    _updateSearcherCollectionTextField: function () {
        if (this.$el.is("input")) {
            var textFieldValue = AJS.$.trim(this.$el.val());
            this.collection.updateTextQuery(textFieldValue);
        }
    },

    _handleInteractiveChanged: function (interactive) {
        // Disable the text input while noninteractive.
        this.$el.prop("disabled", !interactive);
    }
});

module.exports = NavigatorTextField;