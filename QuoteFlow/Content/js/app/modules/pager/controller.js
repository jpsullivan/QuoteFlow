"use strict";

var Marionette = require('backbone.marionette');

var PagerView = require('./view');

var PagerController = Marionette.Controller.extend({
    /**
     * Initialize this controller.
     *
     * @param {Object} options Configuration object
     * @param {Backbone.Model} options.model Model used for the view
     */
    initialize: function(options) {
        this.model = options.model;
    },

    /**
     * Returns the instance for the view (and creates it if needed)
     *
     * @returns {JIRA.Components.Pager.View}
     */
    getView: function(container) {
        if (!this.pagerView) {
            this.pagerView = new PagerView({
                model: this.model,
                el: container
            });
            this.listenTo(this.pagerView, "close", this.destroyView);
            this.listenAndRethrow(this.pagerView, ["goBack", "nextItem", "previousItem"]);
        } else if (container) {
            this.pagerView.setElement(container);
        }
        return this.pagerView;
    },

    /**
     * Display the view inside a region or an element.
     *
     * @param {Marionette.Region|jQuery} container Container where the view should be rendered
     */
    show: function(container) {
        this.getView(container).render();
    },

    /**
     * When the controller is closed, also close the view
     */
    onClose: function(){
        this.pagerView.close();
    },

    /**
     * Unreference the view
     */
    destroyView: function() {
        delete this.pagerView;
    }
});

module.exports = PagerController;
