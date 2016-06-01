"use strict";

var Marionette = require('backbone.marionette');

var AssetView = require('./asset');
var InaccessibleAssetView = require('./inaccessible-asset');

var AssetListView = Marionette.CompositeView.extend({
    template: JST["quote-builder/simple-asset-list/asset-list"],

    childViewContainer: "ol",

    getChildView: function (asset) {
        if (asset.get('inaccessible')) {
            return InaccessibleAssetView;
        }
        return AssetView;
    },

    /**
     * Needs to be overwritten with noop because the default implementation
     * is to add some events to the collection when this view is initially
     * rendered, for handling cases like an item added or removed to/from
     * the collection. That case is not going to happen (as we always reset
     * the full collection), so those events are not necessary.
     */
    _initialEvents: function () {},

    onUpdate: function () {
        // If this asset was already selected, highlight it again
        if (this.collection.selected) {
            var childView = this.children.findByModel(this.collection.selected);
            if (childView) {
                childView.highlight();
            }
        }
    },

    onRender: function () {
        this.unwrapTemplate();
    },

    /**
     * Updates the list with new data.
     * @param {JIRA.Components.Search.Results} collection Data to use
     */
    update: function (collection) {
        this.collection = collection;
        this.render();
        this.triggerMethod("update");
    },

    /**
     * Updates a single asset with new data.
     * @param {SearchResultModel} model The asset model
     */
    updateIssue: function (model) {
        var view = this.children.findByModel(model);

        // Destroy existing view
        if (!view) {
            return;
        }

        this.removeChildView(view);

        // Create the child view from scratch
        var ChildView = this.getChildView(model);
        var index = this.collection.indexOf(model);
        this.addChild(model, ChildView, index);

        this.triggerMethod("update");
    },

    /**
     * Unselects an issue, if exists
     *
     * @param {Number} issueId Issue to unselect
     */
    unselectIssue: function (issueId) {
        // Depending on the network speed, this method could be called before having an actual collection.
        if (!this.collection) {
            return;
        }

        var model = this.collection.get(issueId);
        if (!model) {
            return;
        }

        var view = this.children.findByModel(model);
        if (!view) {
            return;
        }

        view.unhighlight();
    },

    /**
     * Selects an issue, if exists
     *
     * @param {Number} issueId Issue to select
     */
    selectIssue: function (issueId) {
        // Depending on the network speed, this method could be called before having an actual collection.
        if (!this.collection) {
            return;
        }

        var model = this.collection.get(issueId);
        if (!model) {
            return;
        }

        var view = this.children.findByModel(model);
        if (!view) {
            return;
        }

        view.highlight();
    },

    scrollSelectedIssueIntoView: function () {
        // Depending on the network speed, this method could be called before having an actual collection.
        if (!this.collection) {
            return;
        }

        var selected = this.collection.selected;
        if (!selected) {
            return;
        }

        var view = this.children.findByModel(selected);
        if (!view) {
            return;
        }

        view.scrollIntoView();
    }
});

module.exports = AssetListView;
