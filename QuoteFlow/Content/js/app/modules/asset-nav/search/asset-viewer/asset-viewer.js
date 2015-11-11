"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Marionette = require('backbone.marionette');
var MarionetteMixins = require('../../../../mixins/marionette');

var AssetViewerController = require('./controllers/asset-controller');
var AssetEventBus = require('./legacy/asset-event-bus');
var AssetLoaderService = require('./services/asset-loader');
var AssetModel = require('./asset-model');
var ErrorController = require('./controllers/error-controller');
var EventTypes = require('../../util/types');
var ViewAssetData = require('./legacy/view-asset-data');

/**
 * This module provides the AssetViewerController. It will load an asset,
 * update it and render the UI to view the asset.
 * @extends Marionette.Controller
 */
var AssetViewer = Marionette.Controller.extend({
    namedEvents: [
        /**
         * @event loadComplete
         * Triggered when an asset has loaded successfully.
         *
         * @param {Asset} model Model with the asset we have loaded
         * @param {Object} options
         * @param {boolean} options.isNewAsset Whether the loaded asset is a new one
         */
        "loadComplete",

        /**
         * @event loadError
         * Triggered when there is an error when loading an asset.
         *
         * @param {Object} options
         */
        "loadError",

        /**
         * @event replacedFocusedPanel
         * Triggered when the view has rendered a panel that has the focus
         * //TODO This seems to be too specific, why others needs to know about this?
         */
        "replacedFocusedPanel"
    ],

    /**
     * @constructor
     * Initialize this module and all the services/controllers
     *
     * //TODO When this module is transformed into a Marionette.Module, this should be onStart()
     *
     * @param {Object} options
     * @param {boolean|function} [options.showReturnToSearchOnError=false] Whether the error views should display a 'Return to Search' link
     */
    initialize: function (options) {
        options = options || {};

        this.model = new AssetModel();
        this.eventBus = new AssetEventBus();
        this.viewIssueData = new ViewAssetData();

        // Services
        this._buildIssueLoader();

        // Controllers
        this._buildErrorController({
            showReturnToSearchOnError: options.showReturnToSearchOnError
        });
        this._buildIssueController();

        QuoteFlow.bind(EventTypes.REFRESH_ASSET_PAGE, _.bind(function (e, issueId, options) {
            if (this.model.isCurrentIssue(issueId)) {
                this.refreshIssue(options);
            }
        }, this));
    },

    /**
     * Builds the assetLoader service and listens for its events
     *
     * @private
     */
    _buildIssueLoader: function () {
        this.assetLoader = new AssetLoaderService();

        this.listenTo(this.assetLoader, "error", function(reason, props) {
            this.trigger("loadError", props);
            this.errorController.render(reason, props.issueKey);
            this.removeAssetMetadata();

            // Traces
            QuoteFlow.trace("quoteflow.asset.refreshed", { id: props.assetId });

            // QuoteFlow Events
            QuoteFlow.trigger(EventTypes.ASSET_REFRESHED, [props.assetId]);
        });

        this.listenTo(this.assetLoader, "assetLoaded", this._onAssetLoaded);
    },

    /**
     * Builds the ErrorController
     *
     * @param {Object} options
     * @param {boolean|function} [options.showReturnToSearchOnError=false] Whether the error views should display a 'Return to Search' link
     * @private
     */
    _buildErrorController: function (options) {
        options = options || {};

        this.errorController = new ErrorController({
            contextPath: QuoteFlow.rootUrl,
            showReturnToSearchOnError: options.showReturnToSearchOnError
        });

        this.listenTo(this.errorController, "before:render", function() {
            this.assetController.close();
        });

        this.listenTo(this.errorController, "returnToSearch", function() {
            this.trigger("close");
        });

        this.listenAndRethrow(this.errorController, "render");
    },

    /**
     * Builds the Asset controller, the main controller for viewing assets
     * @private
     */
    _buildIssueController: function () {
        this.assetController = new AssetViewerController({
            model: this.model
        });
        this.listenTo(this.assetController, "render", function(regions, options) {
            JIRA.Components.IssueViewer.Utils.hideDropdown();
            this.errorController.close();
            this.trigger("render", regions, options);

            QuoteFlow.trace("jira.psycho.issue.refreshed", { id: this.model.getId() });
        });
        this.listenAndRethrow(this.assetController, "replacedFocusedPanel");
        this.listenTo(this.assetController, "panelRendered", function(panel, $ctx) {
            this.eventBus.triggerPanelRendered(panel, $ctx);
        });
        this.listenTo(this.assetController, "close", function() {
            JIRA.Components.IssueViewer.Utils.hideDropdown();
        });
    },

    /**
     * Update our model with new data
     *
     * @param {Object} data
     * @param {Object} options
     */
    _updateModel: function (data, options) {
        this.model.update(data, options);
    },

    /**
     * Handler for issueLoaded, when an issue has been loaded by IssueLoader service
     *
     * @param {Object} data
     * @param {Object} meta
     * @param {Object} options
     * @private
     */
    _onAssetLoaded: function(data, meta, options) {
        //TODO Why assetEntity is not loaded from data?
        var isPrefetchEnabled = false;
        var assetEntity = options.assetEntity;
        // TODO options.initialize, meta.mergeIntoCurrent and meta.isUpdate seems to represent the same thing
        //      Investigate if all of them are in use and are actually necessary
        var initialize = !meta.mergeIntoCurrent && options.initialize !== false;
        var isNewIssue = !this.model.isCurrentIssue(assetEntity.id);
        var detailView = !!assetEntity.detailView;

        // Clear previous model and errors if this is not an update or is the initial render
        if (!meta.isUpdate || initialize) {
            this.model.resetToDefault();
            this.errorController.close();
        }

        // Update the model with the new data
        this._updateModel(data, {
            initialize: initialize,
            changed: meta.changed,
            mergeIntoCurrent: meta.mergeIntoCurrent
        });

        // Clear previous render if this is not an update or is the initial render
        if (!meta.isUpdate || initialize) {
            this.assetController.close();
        }
        // Display the controller
        this.assetController.show();

        // Refresh the issue if it is loaded from the cache
        if (isPrefetchEnabled && meta.fromCache) {
            this.refreshIssue(assetEntity, {
                fromCache: true,
                mergeIntoCurrent: !meta.error, // If we previously showed error then load everything instead of merging.
                detailView: detailView  // JRA-36659: keep track of whether we are in detail view
            });
        }

        // Save issue metadata
        JIRA.Components.IssueViewer.Services.Metadata.addIssueMetadata(this.model);

        //TODO This should be moved to assetController. Also, assetEntity has no business with bringToFocus
        if (assetEntity.bringToFocus) {
            assetEntity.bringToFocus();
        }

        this.trigger("loadComplete", this.model, {
            isNewIssue: isNewIssue,
            issueId: assetEntity.id,
            duration: meta.loadDuration,
            loadReason: meta.fromCache?'issues-cache-refresh':undefined,
            fromCache: meta.fromCache
        });

        // Traces
        var traceData = { id: assetEntity.id };
        if (meta.fromCache) {
            QuoteFlow.trace('quoteflow.asset.loadFromCache', traceData);
        } else {
            QuoteFlow.trace('quoteflow.asset.loadFromServer', traceData);
        }
        QuoteFlow.trace("jira.issue.refreshed", traceData);

        // QuoteFlow Events
        QuoteFlow.trigger(EventTypes.ASSET_REFRESHED, [assetEntity.id]);
    },

    /**
     * Cancels any pending load so that their handlers aren't called
     */
    abortPending: function () {
        this.assetLoader.cancel();
    },

    /**
     * Shows a dirty form warning if the comment field has been modified.
     *
     * @returns {boolean}
     */
    canDismissComment: function () {
        return this.assetController.canDismissComment();
    },

    /**
     * Clean up before hiding an issue (hide UI widgets, remove metadata, etc.).
     */
    beforeHide: function () {
        JIRA.Components.IssueViewer.Utils.hideLightbox();
        JIRA.Components.IssueViewer.Utils.hideDropdown();
        this.abortPending();
        this.removeAssetMetadata();
    },

    /**
     * Prepare for an issue to be shown.
     */
    beforeShow: function () {
        JIRA.Components.IssueViewer.Services.Metadata.addIssueMetadata(this.model);
    },

    /**
     * @return {null|number} the current issue's ID or null if no valid issue is selected.
     */
    getAssetId: function () {
        return this.model.getEntity().id || null;
    },

    /**
     * Loads an issue already rendered by the server.
     *
     * @param {Object} assetEntity
     */
    _loadIssueFromDom: function(assetEntity) {
        // Many places in KickAss use the presence of an issue ID / key to determine if an issue is selected. We
        // can't extract either from an error message, so pass a dud ID to make it look like an issue is selected.
        if (!assetEntity.id || assetEntity.id == -1) {
            this.errorController.applyToDom("notfound", assetEntity.key);
            this.trigger("loadError");
        } else {
            this.assetController.applyToDom({
                id: assetEntity.id || -1,
                key: assetEntity.key,
                viewIssueQuery: assetEntity.viewIssueQuery
            });
        }

        // After initial load, the server rendered view issue page will be the same as
        // regular ajax view issue page. Thus removing the meta so it can resume
        // to work regularly.
        AJS.Meta.set("serverRenderedViewIssue", null);

        var traceData = { id: this.getIssueId() };
        //TODO These traces should be inside IssueController, as it has more knowledge about when the issue is loaded
        QuoteFlow.trace("jira.issue.refreshed", traceData);
    },

    /**
     * Load an issue and show it in the container.
     *
     * @param {Object} assetEntity
     * @param {number} assetEntity.id The issue's ID.
     * @param {string} assetEntity.key The issue's key.
     * @param {string} [assetEntity.viewIssueQuery] The query string that was provided
     *
     * @returns {jQuery.Promise}
     */
    loadAsset: function (assetEntity) {
        var isServerRendered = AJS.Meta.get("serverRenderedViewAsset");

        if (isServerRendered) {
            assetEntity.id = issueKey.attr("rel") || -1;
            this._loadIssueFromDom(assetEntity);
            return $.Deferred().resolve().promise();
        } else {
            if (!this.canDismissComment() || !assetEntity.key) {
                return $.Deferred().reject();
            }

            this.assetController.showLoading();
            return this.assetLoader.load({
                assetEntity: assetEntity,
                viewIssueData: this.viewIssueData
            });
        }
    },

    /**
     * Refresh the content of the issue, by merging changes from the server.
     *
     * The returned promise is:
     * - resolved when the selected issue is refreshed, or if there is no selected issue
     * - rejected *only* when refreshing the selected issue fails
     *
     * @param {boolean} [options.mergeIntoCurrent] Whether the refresh should merge the retrieved data into the current model
     * @param {function} [options.complete] a function to call after the update has finished
     * @returns {jQuery.Promise}
     */
    refreshAsset: function (options) {
        var promise;
        options = _.defaults({}, options, {
            mergeIntoCurrent: true
        });

        if (this.model.hasIssue()) {
            promise = this.assetLoader.update({
                viewIssueData: this.viewIssueData,
                assetEntity: this.model.getEntity(),
                mergeIntoCurrent: options.mergeIntoCurrent,
                detailView: options.detailView // JRA-36659: keep track of whether we are in detail view
            });

            if (options.complete) {
                promise = promise.done(options.complete).fail(options.complete);
            }

            this.assetController.showLoading();
        } else {
            promise = $.Deferred().resolve().promise();
        }

        return promise;
    },

    /**
     * Remove the issue metadata
     */
    removeAssetMetadata: function() {
        JIRA.Components.IssueViewer.Services.Metadata.removeAssetMetadata(this.model);
    },

    /**
     * Set the container that the issue should be rendered into.
     *
     * @param {jQuery} container The container the issue should be rendered into.
     */
    setContainer: function (container) {
        this.errorController.setElement(container);
        this.assetController.setElement(container);
    },

    /**
     * Returns a deferred that is resolved once issue has loaded.
     * Or straight away if you there are no issue loading in progress.
     *
     * @return {boolean}
     */
    isCurrentlyLoading: function () {
        return this.assetLoader.isLoading();
    },

    /**
     * Updates the current issue with a new ViewIssueQuery
     *
     * @param query {Object} New query to use for the request
     */
    updateAssetWithQuery: function(query) {
        this.model.updateIssueQuery(query);
        this.assetLoader.update({
            viewIssueData: this.viewIssueData,
            assetEntity: this.model.getEntity(),
            mergeIntoCurrent: true
        });
    },

    /**
     * Closes the IssueViewer, cleaning the model and closing all the views
     */
    dismiss: function() {
        this.model.resetToDefault();
        this.errorController.close();
        this.assetController.close();
    },

    close: function() {
        if (this.canDismissComment()) {
            this.dismiss();
            this.trigger("close");
        }
    }
});

module.exports = AssetViewer;
