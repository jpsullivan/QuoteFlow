"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

/**
 * Displays the main layout for the list of line items. This view only
 * renders the placeholder for the other views (list, Empty or Error),
 * and an optional title.
 * @extends Marionette.Layout
 */
var SidebarListLayout = Marionette.LayoutView.extend({
    template: JST["quote-builder/sidebar/list-module"],

    /**
     * Title to display in the message
     * @type {string}
     */
    title: "",

    regions: {
        content: ".quote-content"
    },

    /**
     * @param {Object} options Options
     * @param {string} [options.title] Title of the module. If not provided, the module will be rendered without title markup.
     */
    initialize: function(options) {
        options = _.defaults({}, options, {
            title: ""
        });
        this.title = options.title;
    },

    templateHelpers: function() {
        return {
            title: this.title
        };
    }
});

module.exports = SidebarListLayout;
