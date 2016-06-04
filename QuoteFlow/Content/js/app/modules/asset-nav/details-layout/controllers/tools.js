"use strict";

var Marionette = require('backbone.marionette');
var MarionetteViewManager = require('../../../../mixins/marionette-viewmanager');
var Pager = require('../../../pager/pager');
var ToolsView = require('../views/tools');

var ToolsController = MarionetteViewManager.extend({
    initialize: function (options) {
        this.showExpand = options.showExpand;

        this._buildTools();
        this._buildPager();
    },

    show: function (el) {
        this.toolsView.setElement(el);
        this.toolsView.render();
    },

    load: function (searchResults) {
        this.pager.update(searchResults);
    },

    onDestroy: function () {
        this.pager.destroy();
    },

    _buildTools: function () {
        this.toolsView = this.buildView("toolsView", function () {
            var view = new ToolsView({
                showExpand: this.showExpand
            });
            this.listenTo(view, {
                render: function () {
                    view.pager._ensureElement();
                    this.pager.show(view.pager.$el);
                },
                expand: function () {
                    this.trigger('expand');
                }
            });
            return view;
        });
    },

    _buildPager: function () {
        this.pager = new Pager();
        this.listenTo(this.pager, {
            "next": function () {
                this.trigger("pager:next");
            },
            "previous": function () {
                this.trigger("pager:previous");
            }
        });
    }
});

module.exports = ToolsController;
