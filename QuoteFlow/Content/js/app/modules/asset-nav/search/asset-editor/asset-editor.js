"use strict";

var _ = require('underscore');
var MarionetteMixins = require('../../../../mixins/marionette');

var AssetViewer = require('../asset-viewer/asset-viewer');
var EditAssetController = require('./controllers/edit-asset');
var FieldsController = require('./controllers/fields');

var AssetSaverService = require('./services/asset-saver-service');
var EventTypes = require('../../util/types');
var SaveInProgressManager = require('./services/save-in-progress-manager');
var UnloadInterceptor = require('./services/unload-interceptor');

/**
 * A module that handles loading and showing assets in a given container.
 */
var AssetEditor = AssetViewer.extend({
    namedEvents: [
        /*
         Triggered when an asset has loaded successfully.
         */
        "loadComplete",

        /*
         Triggered when there is an error when loading an asset.
         */
        "loadError",

        /*
         We should return to asset search in response to some action.
         */
        "returnToSearch",

        /**
         * Triggered when inline edit is successful
         * .. {number} the asset id
         */
        "saveSuccess",
        "saveError",
        "editField",

        /**
         * Field has been submitted by user.
         */
        "fieldSubmitted",

        /**
         * The asset panel that previously had focus was replaced.
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
        AssetViewer.prototype.initialize.call(this, options);

        // Services
        // this._buildFieldsLoader();
        this._buildIssueSaver();

        // Controllers
        this._buildEditIssueController();
        this._buildFieldsController();

        // Other services
        this._handleUnload();

        this.listenAndRethrow(this.eventBus, "fieldSubmitted");
    },

    _buildFieldsLoader: function () {
        this.fieldsLoader = new JIRA.Components.IssueEditor.Services.FieldsLoader({
            contextPath: AJS.contextPath()
        });

        this.listenTo(this.fieldsLoader, "fieldsLoaded", function (result) {
            this.viewAssetData.updateAsset(result.assetSku, result);

            // Ensure assetID is a number, otherwise some checks might fail
            result.assetId = Number(result.assetId);

            var editable = result.fields && result.fields.length;
            if (editable) {
                this.editAssetController.update(result);
            } else {
                this.editAssetController.reset();
            }

            QuoteFlow.trace("quoteflow.asset.fields.loaded", {id: result.assetId});
        });

        this.listenTo(this.fieldsLoader, "fieldsError", function (result) {
            this.fieldsController.showError(result.errorCollection, result.isTimeout);
            QuoteFlow.trace("quoteflow.asset.fields.loaded", {id: result.assetId});
        });
    },

    _buildIssueSaver: function () {
        this.saveInProgressManager = new SaveInProgressManager();

        this.assetSaver = new AssetSaverService({
            saveInProgressManager: this.saveInProgressManager,
            model: this.model
        });

        this.listenTo(this.assetSaver, "error", function (options) {
            var assetId = options.assetId;
            var attemptedSavedIds = options.attemptedSavedIds;
            var response = options.response;
            var duration = options.duration;

            // Show a global error if the asset is no longer visible.
            if (!this.model.isCurrentAsset(assetId)) {
                this._showSaveError({
                    assetId: assetId,
                    assetSku: options.assetSku,
                    response: response
                });
                return;
            }

            this.eventBus.triggerSaveError(assetId, attemptedSavedIds, response);

            // if no longer editable, reload our model
            var isEditable = response && response.fields && response.fields.length;
            if (!isEditable && response) {
                this.editAssetController.reset();
                this.model.update(response, {
                    fieldsSaved: [],
                    fieldsInProgress: []
                });
            }

            this.trigger("saveError", {
                assetId: assetId,
                duration: duration,
                deferred: false
            });

            QuoteFlow.trace("quoteflow.asset.refreshed", {id: assetId});
        });

        this.listenTo(this.assetSaver, "saveStarted", function (assetId, savedFieldIds) {
            this.eventBus.triggerSavingStarted(savedFieldIds);
        });

        this.listenTo(this.assetSaver, "save", function (options) {
            var assetId = options.assetId;
            var assetSku = options.assetSku;
            var savedFieldIds = options.savedFieldIds;
            var response = options.response;
            var shouldSkipSaveAssetSuccessHandler = options.shouldSkipSaveAssetSuccessHandler;
            var duration = options.duration;

            if (this.model.isCurrentAsset(assetId) && !shouldSkipSaveAssetSuccessHandler) {
                this.eventBus.triggerSaveSuccess(assetId, assetSku, savedFieldIds, response);

                // Updating view asset cache with the new data we get back from a successful save.
                this.viewAssetData.set(assetSku, response);

                // Update the model with the new data, including the list of fields still in progress
                this.model.update(response, {
                    fieldsSaved: savedFieldIds,
                    fieldsInProgress: this.editAssetController.getEditsInProgress()
                });

                QuoteFlow.trace("quoteflow.psycho.asset.refreshed", {id: assetId});

                // Don't check for the meta value if the page is standalone view asset page
                // because it doesn't exist
                var editable = response.fields && response.fields.length;
                if (editable) {
                    this.editAssetController.update({
                        fields: response.fields,
                        assetId: assetId,
                        assetSku: assetSku
                    }, {
                        editable: true,
                        editAssetController: this.editAssetController,
                        assetModel: this.model,
                        assetId: assetId,
                        fieldsSaved: savedFieldIds,
                        initialize: false,
                        fieldsInProgress: this.editAssetController.getEditsInProgress()
                    });
                }

                QuoteFlow.trace("quoteflow.asset.refreshed", {id: assetId});
            }
            this.trigger("saveSuccess", {
                assetId: assetId,
                assetSku: assetSku,
                savedFieldIds: savedFieldIds,
                duration: duration
            });
        });
    },

    _buildEditIssueController: function () {
        this.editAssetController = new EditAssetController({
            assetEventBus: this.eventBus
        });
        this.listenTo(this.editAssetController, "save", function (assetId, assetSku, toSaveIds, params, ajaxProperties) {
            // Make sure we don't skip the SaveAssetHandler
            // While the save request is being processed, this handler will be set to being skipped if the ViewIssue
            // module has been closed
            this.assetSaver.setSkipSaveAssetSuccessHandler(false);
            this.assetSaver.save(assetId, assetSku, toSaveIds, params, ajaxProperties);
        });
        this.listenAndRethrow(this.editAssetController, "editField");
    },

    _buildFieldsController: function () {
        this.fieldsController = new FieldsController();
    },

    _onAssetLoaded: function (data, meta, options) {
        if (data.asset.isEditable) {
            // If this asset is NOT from the cache and this is NOT an update, request the fields from the server
            var isPrefetchEnabled = false;
            if (isPrefetchEnabled && !meta.error && !meta.fromCache) {
                this.fieldsLoader.load({
                    viewAssetData: this.viewAssetData,
                    assetEntity: options.assetEntity
                });
            }
        }

        var initialize = !meta.mergeIntoCurrent && options.initialize !== false;
        if (!meta.isUpdate || initialize) {
            this.editAssetController.reset();
        }
        AssetViewer.prototype._onAssetLoaded.call(this, data, meta, options);

        // If we have fields data, update it
        // This needs to be done after calling the original onAssetLoaded, as that method can reset
        // the editAsset controller
        if (data.fields) {
            this.editAssetController.update({
                fields: data.fields,
                assetId: data.asset.id,
                assetSku: data.asset.sku
            });
            QuoteFlow.trace("quoteflow.asset.fields.loaded", {id: data.asset.id});
        }
    },

    _handleUnload: function () {
        var unloadHandler = _.bind(function () {
            var result;
            if (this.editAssetController.getDirtyEditsInProgress().length > 0) {
                result = AJS.I18n.getText("viewasset.editing.leave");
            }
            return result;
        }, this);
        this.unloadInterceptor = new UnloadInterceptor();

        this.unloadInterceptor.addAfterEvent(unloadHandler);
        this.on("destroy", function () {
            this.unloadInterceptor.removeAfterEvent(unloadHandler);
        });
    },

    _updateModel: function (data, options) {
        // Update editAssetController when whe update the model
        var editable = data.fields && data.fields.length;
        if (editable) {
            this.editAssetController.update(data.fields, {
                fieldsInProgress: this.editAssetController.getEditsInProgress(),
                changed: options.changed
            });
        }

        options = _.extend({}, options, {
            fieldsInProgress: this.editAssetController.getEditsInProgress()
        });
        AssetViewer.prototype._updateModel.call(this, data, options);
    },

    _loadAssetFromDom: function (assetEntity) {
        if (AJS.Meta.get("server-view-asset-is-editable")) {
            // Edit Asset Controller
            this.editAssetController.setAssetId(assetEntity.id);
            this.editAssetController.setAssetSku(assetEntity.sku);

            this.fieldsLoader.load({
                viewAssetData: this.viewAssetData,
                assetEntity: assetEntity
            });
            AJS.Meta.set("server-view-asset-is-editable", null);
        }
        AssetViewer.prototype._loadAssetFromDom.call(this, assetEntity);
    },

    /**
     * Initiate the editing of a field.
     *
     * @param {JIRA.Components.IssueEditor.Models.Field} field The field to edit.
     */
    editField: function (field) {
        if (!field.isEditable()) {
            return;
        }

        var asset = this.model.getEntity();
        // Defer determining whether the field is present until
        // after the save completes; it may be removed by the save.
        var execute = _.bind(function () {
            if (field.matchesFieldSelector()) {
                field.edit();
            } else {
                new JIRA.Components.IssueEditor.Views.ModalFieldView({
                    assetEventBus: this.eventBus,
                    model: field,
                    asset: asset
                }).show();
            }
        }, this);

        if (!field.getSaving()) {
            execute();
        } else {
            QuoteFlow.one(EventTypes.ASSET_REFRESHED, function () {
                execute();
            });
        }
    },

    /**
     * Cancels any pending load so that their handlers aren't called
     */
    abortPending: function () {
        this.fieldsLoader.cancel();
        AssetViewer.prototype.abortPending.call(this);
    },

    /**
     * Clean up before hiding an asset (hide UI widgets, remove metadata, etc.).
     */
    beforeHide: function () {
        this.assetSaver.setSkipSaveAssetSuccessHandler(true);
        AssetViewer.prototype.beforeHide.call(this);
    },

    /**
     * Set the container that the asset should be rendered into.
     *
     * @param {element} container The container the asset should be rendered into.
     */
    setContainer: function (container) {
        this.editAssetController.setAssetViewContext(container);
        AssetViewer.prototype.setContainer.call(this, container);
    },

    dismiss: function () {
        this.editAssetController.reset();
        AssetViewer.prototype.dismiss.call(this);
    },

    getFields: function () {
        return this.editAssetController.getFields();
    },

    hasSavesInProgress: function () {
        return this.saveInProgressManager.hasSavesInProgress();
    },

    /**
     * Create and show a `SaveError`.
     *
     * @param {object} options
     * @param {string} options.assetId The ID of the asset that failed to save.
     * @param {string} options.assetSku The SKU of the asset that failed to save.
     * @param {object} options.response An `AssetSaverService` response.
     * @private
     */
    _showSaveError: function (options) {
        var saveError, stopListening;

        saveError = new JIRA.Components.IssueEditor.Views.SaveError(options).render();
        stopListening = _.partial(this.stopListening, saveError);

        this.listenTo(saveError, {
            close: stopListening,
            issueLinkClick: function (issueData) {
                // Poor man preventable event
                var ev = {
                    isPrevented: false,
                    preventDefault: function () { this.isPrevented = true; },
                    isDefaultPrevented: function () { return this.isPrevented === true; },
                    issueData: issueData
                };
                this.trigger("linkInErrorMessage", ev);

                if (!ev.isDefaultPrevented()) {
                    this.loadAsset(issueData);
                }
            }
        });
    }
});

module.exports = AssetEditor;
