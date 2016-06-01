"use strict";

var Marionette = require('backbone.marionette');
var LoadingError = require('../views/loading-error');

var FieldsController = Marionette.Controller.extend({
    showError: function (errorCollection, isTimeout) {
        if (errorCollection || isTimeout) {
            // Close previous view, if any
            this.close();

            // Let others know we are rendering, Marionette style
            this.triggerMethod("before:render");

            this.view = new LoadingError(errorCollection, isTimeout);
            this.view.render();

            // Listen for view events
            this.listenTo(this.view, "close", this.close);

            // Let others know we have rendered our view, Marionette style
            this.triggerMethod("render");
        }
    },

    /**
     * Closes and deletes the view
     */
    close: function () {
        if (this.view) {
            this.view.close();
            this.stopListening(this.view);
            delete this.view;
        }
    }
});

module.exports = FieldsController;
