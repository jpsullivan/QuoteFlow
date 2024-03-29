"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var AssetBodyLayout = require('../views/asset/asset-body-layout');
var AssetHeaderLayout = require('../views/asset/asset-header-layout');
var AssetViewLayout = require('../views/asset/asset-layout');

/**
 * Controller for the asset viewer.
 *
 * This controller is a mediator for the AssetView. It is responsible for
 * creating/updating the view from a pre-rendered DOM and rendering the view.
 * It also creates all the subviews and composes the layout.
 *
 * @extends Marionette.Controller
 */
var AssetController = Marionette.Controller.extend({
    /**
     * @event replacedFocusedPanel
     * Triggered when the view has rendered a panel that has the focus
     */

    /**
     * @event panelRendered
     * Triggered when the view has rendered a panel
     * @param {string} panelId ID of the panel
     * @param {jQuery} $ctx The new panel element
     */

    /**
     * @constructor
     * @param {Object} options
     * @param {AssetModel} options.model Model used by this view
     */
    initialize: function (options) {
        this.model = options.model;
    },

    /**
     * Creates the main view and all the subviews
     * @private
     */
    _createViews: function () {
        this.view = this._createMainView();
        this.bodyView = this._createBodyView();
        this.headerView = this._createHeaderView();
        // this.leftPanelsView = this._createPanelsView(this.model.getPanels().getLeftPanels());
        // this.rightPanelsView = this._createPanelsView(this.model.getPanels().getRightPanels());
        // this.infoPanelsView = this._createPanelsView(this.model.getPanels().getInfoPanels());
    },

    /**
     * Creates the main view. It is just a container for HeaderView and BodyView
     * @return {AssetViewLayout}
     * @private
     */
    _createMainView: function () {
        return new AssetViewLayout({
            model: this.model,
            el: this.$el
        });
    },

    /**
     * Creates the view for rendering the body of the asset.
     * It is just a collection of panels.
     * @return {AssetBodyLayout}
     * @private
     */
    _createBodyView: function () {
        return new AssetBodyLayout({
            model: this.model
        });
    },

    /**
     * Creates the view for rendering the header. It includes regions for
     * the opsbar and the pager.
     * @return {AssetHeaderLayout}
     * @private
     */
    _createHeaderView: function () {
        var view = new AssetHeaderLayout({
            model: this.model
        });
        this.listenAndRethrow(view, "panelRendered");

        this.listenTo(view, "updated", function () {
            this.trigger("render", {pager: this.view.$(this.headerView.pager.el)}, {assetId: this.model.getId()});
        });

        this.listenTo(view, "addToQuote", function (event) {
            QuoteFlow.application.execute("assetEditor:addToQuote", event.model);
        });

        return view;
    },

    /**
     * Creates the view for the panels. This view renders a collection of panels. The BodyView contains three
     * views of this kind: left panels, right panels and info panels.
     * @param {JIRA.Components.IssueViewer.Collections.Panels} collection Collection of panels to render
     * @returns {JIRA.Components.IssueViewer.Views.IssuePanels}
     * @private
     */
    _createPanelsView: function (collection) {
        var view = new JIRA.Components.IssueViewer.Views.IssuePanels({
            collection: collection
        });
        this.listenTo(view, "itemview:replacedFocusedPanel", function () {
            this.trigger("replacedFocusedPanel");
        });
        this.listenTo(view, "itemview:panelRendered", function (view, panelId, $ctx) {
            this.trigger("panelRendered", panelId, $ctx);
        });
        return view;
    },

    /**
     * Creates the view and composes the layout
     */
    createView: function () {
        this._createViews();

        // When main view is rendered, inject all the subviews
        // We need to wait until the main view is rendered because otherwise the regions are not defined
        this.listenTo(this.view, "render", function () {
            this.headerView = this._createHeaderView();
            this.view.header.show(this.headerView);

            this.bodyView = this._createBodyView();
            this.view.body.show(this.bodyView);

            // this.bodyView.leftPanels.show(this.leftPanelsView);
            // this.bodyView.rightPanels.show(this.rightPanelsView);
            // this.bodyView.infoPanels.show(this.infoPanelsView);

            this.trigger("render", {
                pager: this.view.$(this.headerView.pager.el)
            }, {
                loadedFromDom: false,
                assetId: this.model.getId()
            });
        });
    },

    /**
     * Creates the view using a server-rendered markup
     */
    createViewFromDom: function () {
        this._createViews();

        // When the view is loaded from the dom, inject all the subviews and update them from the DOM
        this.listenTo(this.view, "applyToDom", function () {
            /**
             * Helper method to inject a region and update it from the DOM.
             * Maybe we can move it to JIRA.Marionette.Layout
             *
             * @param {Marionette.Region} region Region where the view should be inserted
             * @param {Backbone.View} view View to render in the region
             * @param {jQuery} el DOMElement that contains the pre-rendered markup for the view
             * @ignore
             */
            function updateRegionFromDom (region, view, el) {
                view.setElement(el);
                view.applyToDom();
                region.attachView(view);
            }

            // The pre-rendered markup has not the same selector than the region.
            updateRegionFromDom(this.view.body, this.bodyView, this.view.$el.find(".asset-body-content"));
            updateRegionFromDom(this.view.body, this.headerView, this.view.$el.find("#stalker"));

            updateRegionFromDom(this.bodyView.leftPanels, this.leftPanelsView, this.bodyView.$el.find(this.bodyView.leftPanels.el));
            updateRegionFromDom(this.bodyView.rightPanels, this.rightPanelsView, this.bodyView.$el.find(this.bodyView.rightPanels.el));
            updateRegionFromDom(this.bodyView.infoPanels, this.infoPanelsView, this.bodyView.$el.find(this.bodyView.infoPanels.el));

            this.trigger("render", {pager: this.view.$(this.headerView.pager.el)}, {
                loadedFromDom: true,
                assetId: this.model.getId()
            });
        });
    },

    /**
     * Changes the element where the view should be rendered
     *
     * @param {jQuery} element Container for the view
     */
    setElement: function (element) {
        this.$el = element;
        if (this.view) {
            this.view.setElement(element);
        }
    },

    /**
     * Loads a view from server-rendered markup.
     * @param {Object} assetEntity The asset entity object
     */
    applyToDom: function (assetEntity) {
        assetEntity.id = +assetEntity.id; // Ensure value grabbed from DOM is converted into a number
        this.model.updateFromEntity(assetEntity);
        this.createViewFromDom();
        this.view.applyToDom();
    },

    /**
     * Displays the view
     */
    show: function () {
        if (this.view) {
            this.trigger("render", {pager: this.view.$(this.headerView.pager.el)}, {
                loadedFromDom: false,
                assetId: this.model.getId()
            });
        } else {
            this.createView();
            this.view.render();
        }
        this.view.hideLoading();
    },

    /**
     * Closes the view
     */
    destroy: function () {
        if (this.view) {
            this.view.destroy();
        }
        this.hideLoading();
        delete this.view;
    },

    /**
     * Hides the loading spinner
     */
    hideLoading: function () {
        if (this.view) {
            this.view.hideLoading();
        }
    },

    /**
     * Shows the loading spinner
     */
    showLoading: function () {
        if (this.view) {
            this.view.showLoading();
        }
    },

    /**
     * Shows a dirty form warning if the comment field has been modified.
     * @return {boolean} If user has opted to dismiss comment or not.
     */
    canDismissComment: function () {
        var dirtyMessage;
        // var commentForm = JIRA.Issue.CommentForm;
        // if (!commentForm.getForm().data("dismissed")) {
        //     dirtyMessage = commentForm.handleBrowseAway();
        //     if (dirtyMessage) {
        //         if (!confirm(dirtyMessage)) {
        //             commentForm.focus();
        //             return false;
        //         } else {
        //             commentForm.getForm().data("dismissed", true);
        //         }
        //     }
        // }
        return true;
    },

    applyResponsiveDesign: function () {
        if (!this.$el) {
            return;
        }

        var width = this.$el.width();
        this.$el.toggleClass("skinny", width < 900);
        this.$el.toggleClass("very-skinny", width < 600);
    }
});

module.exports = AssetController;
