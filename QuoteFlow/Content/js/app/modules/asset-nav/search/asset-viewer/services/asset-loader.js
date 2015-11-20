"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Marionette = require('backbone.marionette');

var AssetFieldUtil = require('../legacy/asset-field-util');
var RecurringPromise = require('../../../../../util/jquery/RecurringPromise');

/**
 * This service is responsible for loading data for an asset.
 * @extends Marionette.Controller
 */
var AssetLoaderService = Marionette.Controller.extend({
    /**
     * @event assetLoaded
     * Fired when an asset has been loaded or updated
     * @param {Object} data
     * @param {Object} meta
     * @param {Object} options
     */

    /**
     * @event error
     * Fired when there is an error loading or updating the asset
     * @param {string} reason Type of error: "auth", "forbidden", "notfound" or "generic"
     * @param {Object} options
     */

    /**
     * @constructor
     */
    initialize: function() {
        this._promiseManager = new $.RecurringPromise();

        this._updatingPromise = this._promiseManager.sub();
        this._updatingPromise
            .done(_.bind(this._onUpdatingDone, this))
            .fail(_.bind(this._onUpdatingError, this));

        this._loadingPromise = this._promiseManager.sub();
        this._loadingPromise
            .done(_.bind(this._onLoadingDone, this))
            .fail(_.bind(this._onLoadingError, this));
    },

    /**
     * Cancel all pending requests for our promises
     */
    cancel: function() {
        this._promiseManager.reset();
    },

    /**
     * Updates an asset
     *
     * @param {Object} options
     * @param {ViewAssetData} options.viewAssetData
     * @param {Object} options.assetEntity Asset entity to update
     * @param {boolean} options.mergeIntoCurrent whether the update should just merge into the current asset
     */
    update: function (options) {
        var assetEntity = options.assetEntity,
            mergeIntoCurrent = options.mergeIntoCurrent,
            viewAssetData = options.viewAssetData,
            detailView = !!options.detailView;

        // Add the updating task to the recurring promise
        var deferred = new $.Deferred(),
            recurrantControl;

        function genPromiseWrapper(taskKey) {
            return function() {
                return {
                    taskKey: taskKey,
                    data: arguments,
                    startIssueLoad: new Date()
                };
            };
        }

        recurrantControl = this._updatingPromise.add(viewAssetData.get(assetEntity.id, true, {
            assetEntity: assetEntity,
            loadingDeferred: deferred,
            mergeIntoCurrent: mergeIntoCurrent,
            startAssetLoad: new Date(),
            detailView: detailView // JRA-36659: keep track of whether we are in detail view
        }).pipe(genPromiseWrapper("asset"), genPromiseWrapper("asset")));

        recurrantControl.fail(function() {
            deferred.reject.apply(deferred, arguments);
        });

        return deferred.promise();
    },

    /**
     * Loads an asset
     *
     * @param {Object} options
     * @param {ViewAssetData} options.viewAssetData
     * @param {Object} options.assetEntity Asset entity to update

     * @returns {jQuery.Promise}
     */
    load: function(options) {
        var assetEntity = options.assetEntity;
        var viewAssetData = options.viewAssetData;
        var detailView = !!assetEntity.detailView;

        this._currentlyLoading = true;

        // Add the loading task to the recurring promise
        var deferred = new $.Deferred();

        this._loadingPromise.add(viewAssetData.get(assetEntity.id, false, {
            assetEntity: assetEntity,
            loadFields: false,
            loadingDeferred: deferred,
            startAssetLoad: new Date(),
            detailView: detailView // JRA-36659: keep track of whether we are in detail view
        }));

        return deferred.promise();
    },

    /**
     * Checks if there is a loading in progress
     * @returns {boolean} Whether there is a loading operation in progress
     */
    isLoading: function() {
        return this._currentlyLoading;
    },

    /**
     * Handler for loading:done, when an asset has been loaded
     *
     * @param {Object} data
     * @param {Object} meta
     * @param {Object} options
     * @private
     */
    _onLoadingDone: function(data, meta, options) {
        // Massage data
        var assetEntity = options.assetEntity;
        assetEntity.id = data.asset.id;
        if (!data.pager) {
            data.pager = assetEntity.pager;
        }
        AssetFieldUtil.transformFieldHtml(data);

        // Mark request as loaded
        this._currentlyLoading = false;

        // Compute loading time
        meta.loadDuration = new Date() - options.startIssueLoad;

        // Trigger our main event
        this.trigger("assetLoaded", data, meta, options);

        // Resolve the main promise
        options.loadingDeferred.resolve(data, meta, options);
    },

    /**
     * Handler for updating:done, when an asset has been updated.
     *
     * This method does virtually nothing, just call _onLoadingDone with meta.isUpdate=true
     *
     * @param {Object[]} args
     * @private
     */
    _onUpdatingDone: function(args) {
        var meta = args.data[1];
        if (meta) {
            meta.isUpdate = true;
        }
        this._onLoadingDone.apply(this, args.data);
    },

    /**
     * Handler for loading:reject, when there are errors loading an asset
     *
     * @param {Object} data
     * @param {Object} meta
     * @param {Object} options
     * @private
     */
    _onLoadingError: function(data, meta, options) {
        // Extract some data
        var assetEntity = options.assetEntity;
        var props = {
            assetId: assetEntity.id,
            response: data
        };

        // Mark request as loaded
        this._currentlyLoading = false;

        // Trigger our main event and resolve the promise
        var reason;
        switch (data.status) {
            case 401:
                reason = "auth";
                break;
            case 403:
                reason = "forbidden";
                break;
            case 404:
                reason = "notfound";
                break;
            default:
                reason = "generic";
                break;
        }

        this.trigger("error", reason, props);
        options.loadingDeferred.reject();
    },

    /**
     * Handler for updating:reject, when there are errors updating an asset
     *
     * This method does virtually nothing, just call _onLoadingError with the right params
     *
     * @param {Object[]} args
     * @private
     */
    _onUpdatingError: function(args) {
        this._onLoadingError.apply(this, args.data);
    }
});

module.exports = AssetLoaderService;
