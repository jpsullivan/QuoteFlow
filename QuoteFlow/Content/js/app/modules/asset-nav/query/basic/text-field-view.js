"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

/**
 * Simple Query View's text search field.
 */
var TextFieldView = Brace.View.extend({

    namedEvents: ["searchRequested"],

    events: {
        keypress: "handleKeypress"
    },

    initialize: function() {
        _.bindAll(this,
            "_updateSearcherCollectionTextField",
            "_handleInteractiveChanged",
            "render");

        this.collection.on("remove change add", _.bind(function(model) {
            if (model.getId() === this.collection.QUERY_ID) {
                this.render();
            }
        }, this));
        this.collection.onTextFieldChanged(this.render);
        this.collection.onRequestUpdateFromView(this._updateSearcherCollectionTextField);
        this.collection.onInteractiveChanged(this._handleInteractiveChanged);
    },

    handleKeypress: function (e) {
        if (e.keyCode === 13) {
            this.collection.handleBasicViewSubmit();
            e.preventDefault();
        }
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

    setQuery: function(query) {
        this.$el.val(query);
    },

    _updateSearcherCollectionTextField: function() {
        if (this.$el.is("input")) {
            var textFieldValue = AJS.$.trim(this.$el.val());
            this.collection.updateTextQuery(textFieldValue);
        }
    },

    _handleInteractiveChanged: function(interactive) {
        // Disable the text input while noninteractive.
        this.$el.prop("disabled", !interactive);
    }
});

module.exports = TextFieldView;
