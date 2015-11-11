"use strict";

var Marionette = require('backbone.marionette');

/**
 * Abstract class for all of the error views.
 * @extends Marionette.Layout
 * @abstract
 */
var ErrorLayout = Marionette.LayoutView.extend({

    className: "asset-container",

    regions: {
        pager: "#asset-header-pager"
    },

    /**
     * Remove the view
     *
     * Override Backbone's method, as we don't want to remove the container from the view.
     *
     * @return {ErrorLayout} this
     */
    remove: function () {
        this.stopListening();
        this.$el.empty();
        return this;
    },

    /**
     * Handler for render event
     */
    onRender: function () {
        // $el has been modified outside this view, we need to restore the className
        this.$el.addClass(this.className);
        this.$el.attr("tabindex", "-1");
    }
});

module.exports = ErrorLayout;
