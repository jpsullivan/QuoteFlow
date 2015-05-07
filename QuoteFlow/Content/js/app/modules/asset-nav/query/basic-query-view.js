"use strict";

var Brace = require('backbone-brace');

var ClauseButtonView = require('./basic/clause-button-view');
var ExtendedCriteriaContainerView = require('./basic/extended-criteria-container-view');
var OrderByComponent = require('../orderby/component');
var PrimaryCriteriaContainerView = require('./basic/primary-criteria-container-view');
var TextFieldView = require('./basic/text-field-view');

/**
 * Renders the Basic Query component.
 */
var BasicQueryView = Brace.View.extend({

    namedEvents: ["verticalResize", "searchRequested"],

    template: JST["quote-builder/query/basic-query"],

    initialize: function(options) {
        this.searcherCollection = options.searcherCollection;
        this.queryStateModel = options.queryStateModel;

        this.textFieldView = new TextFieldView({
            collection: this.searcherCollection
        });

        // Subview for rendering primary criteria (project, assignee, etc)
        this.primaryCriteriaContainerView = new PrimaryCriteriaContainerView({
            collection: this.searcherCollection
        });

        // Subview for rendering extended criteria
        this.extendedCriteriaContainerView = new ExtendedCriteriaContainerView({
            collection: this.searcherCollection
        }).onVerticalResize(this.triggerVerticalResize, this);


        if (this.queryStateModel.getBasicOrderBy()) {
            this.basicOrderByView = OrderByComponent.create();

            this.queryStateModel.on("change:jql", function () {
                this.basicOrderByView.setJql(this.queryStateModel.getJql());
            }, this);

            this.basicOrderByView.onSort(this.triggerSearchRequested, this);
        }

        // The subview for the clear all and add additional filters.
        this.clauseButtonView = new ClauseButtonView({
            searcherCollection: this.searcherCollection,
            queryStateModel: this.queryStateModel
        });

        this.searcherCollection.onInteractiveChanged(this._handleInteractiveChanged, this);
        this.searcherCollection.onBeforeCriteriaRemoved(this._handleBeforeCriteriaRemoved, this);
    },

    _handleInteractiveChanged: function(interactive) {
        this.$el.toggleClass("loading", !interactive);
    },

    _handleBeforeCriteriaRemoved: function(id, direction) {
        // If a criteria was removed with a direction, focus on the next focusable element in that direction
        if (direction) {
            this._shiftFocus(id, direction);
        }
    },

    hasPdl: function () {
        var version = AJS.Meta.get("version-number");
        if (version) {
            return parseInt(version.charAt(0), 10) >= 6;
        }
    },

    /**
     * This render function is only called when the BasicQueryView is first initialized,
     * and also when switching from advanced to basic mode.
     *
     * All further renders are handled at the sub-view level.
     */
    render: function() {
        this.$el.html(this.template({
            hasPdl: this.hasPdl(),
            hasOrderBy: this.queryStateModel.getBasicOrderBy(),
            hasSearchButton: this.queryStateModel.hasSearchButton()
        }));

        this.primaryCriteriaContainerView.setElement(this.$el.find(".search-criteria .criteria-list"));
        this.textFieldView.setElement(this.$el.find("input.search-entry"));
        this.clauseButtonView.setElement(this.$el.find(".criteria-actions"));
        this.extendedCriteriaContainerView.setElement(this.$el.find(".search-criteria-extended .criteria-list"));
        if (this.basicOrderByView) {
            this.basicOrderByView.setElement(this.$el.find(".list-ordering")).render();
            this.basicOrderByView.setJql(this.queryStateModel.getJql());
        }

        this.primaryCriteriaContainerView.render();
        this.$el.find(".text-query").removeClass("hidden");

        this.textFieldView.render();
        this.clauseButtonView.render();

        this.extendedCriteriaContainerView.render();

        this._handleInteractiveChanged(this.searcherCollection.isInteractive());
        this.triggerVerticalResize();

        // new JIRA.Issues.Tipsy({
        //     el: this.$el.find(".search-button"),
        //     tipsy: {
        //         trigger: "hover",
        //         delayIn: 300
        //     }
        // })

        return this;
    },

    /**
     * Performs a search using the current state in the Basic Query View.
     */
    search: function() {
        this.searcherCollection.handleBasicViewSubmit();
    },

    /**
     * Put the focus on the next focusable element in the given direction (-1: back, 1: forward),
     * assuming the currently focused item is about to be removed
     */
    _shiftFocus: function(currentId, direction) {
        var $current = this.primaryCriteriaContainerView.getFocusableForCriteria(currentId);
        if (!$current.length) $current = this.extendedCriteriaContainerView.getFocusableForCriteria(currentId);
        var $allFocusables = this.primaryCriteriaContainerView.getFocusables()
            .add(this.extendedCriteriaContainerView.getFocusables());
        var currentIndex = $allFocusables.index($current);
        var nextIndex;
        // If the element being removed is at either end, there is only 1 element the focus can move to,
        // irrespective of direction
        if (currentIndex === 0) {
            nextIndex = 1;
        } else if (currentIndex === $allFocusables.length - 1) {
            nextIndex = $allFocusables.length - 2;
        } else {
            nextIndex = currentIndex + direction;
        }
        $allFocusables.eq(nextIndex).focus();
    },

    focus: function() {
        this.primaryCriteriaContainerView.getFocusables().first().focus();
    }
});

module.exports = BasicQueryView;
