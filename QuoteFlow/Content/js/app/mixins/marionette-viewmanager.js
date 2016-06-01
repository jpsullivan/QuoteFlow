"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var ViewManager = Marionette.Object.extend({
    constructor: function () {
        // Make sure we initialize the 'views' map first, so descentants
        // of this class can build views in their constructor
        this.views = {};
        Marionette.Controller.apply(this, arguments);
    },

    hideView: function (viewName) {
        var view = this.views[viewName];
        if (view) {
            this.stopListening(view);
            if (!view.isDestroyed) {
                view.destroy();
            }
            delete this.views[viewName];
        }
    },

    showView: function (viewName, factory) {
        var view = this.buildView(viewName, factory);
        view.render();
    },

    buildView: function (viewName, factory) {
        var view = this.views[viewName];
        if (!view) {
            view = factory.call(this);
            this.listenTo(view, "destroy", function () {
                this.hideView(viewName);
            });
            this.views[viewName] = view;
        }
        return view;
    },

    getView: function (viewName) {
        return this.views[viewName];
    },

    onDestroy: function () {
        _.each(this.views, function (view) {
            this.hideView(view);
        }, this);
    }
});

module.exports = ViewManager;
