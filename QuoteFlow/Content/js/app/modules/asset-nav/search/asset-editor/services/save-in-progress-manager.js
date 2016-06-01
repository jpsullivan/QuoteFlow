"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

var AssetFieldUtil = require('../../asset-viewer/legacy/asset-field-util');
var EventTypes = require('../../../util/types');
var SmartAjax = require('../../../../../components/ajax/smart-ajax');

var SaveInProgressManager = Brace.Model.extend({
    namedAttributes: [
        "savesInProgress"
    ],

    namedEvents: ["beforeSaving", "savingStarted", "saveSuccess", "saveError"],

    initialize: function () {
        this.setSavesInProgress([]);
    },

    saveAsset: function (assetId, assetSku, fieldsToSave, data, ajaxProperties) {
        this.triggerBeforeSaving();

        var instance = this;
        var saveInProgress;
        var allParams;

        allParams = _.extend(data, {
            assetId: assetId,
            /* eslint-disable camelcase*/
            atl_token: atl_token(),
            /* eslint-enable camelcase*/
            singleFieldEdit: true,
            fieldsToForcePresent: fieldsToSave
        });

        var ajaxOpts = _.extend({
            type: "POST",
            url: QuoteFlow.rootUrl + "/secure/AjaxIssueAction.jspa?decorator=none",
            headers: {'X-SITEMESH-OFF': true},
            error: function (xhr) {
                instance._handleSaveError(assetId, assetSku, fieldsToSave, xhr);
            },
            success: function (resp, statusText, xhr, smartAjaxResult) {
                var responseData = smartAjaxResult.data;
                // Was the response HTML?
                if (typeof responseData === "string") {
                    instance._handleHtmlResponse(assetId, assetSku, fieldsToSave, responseData);
                } else {
                    AssetFieldUtil.transformFieldHtml(responseData);
                    instance.triggerSaveSuccess(assetId, assetSku, fieldsToSave, responseData);
                }
            },
            complete: function () {
                instance.removeSaveInProgress(saveInProgress);
                QuoteFlow.trigger(EventTypes.INLINE_EDIT_SAVE_COMPLETE);
            },
            data: allParams
        }, ajaxProperties);

        saveInProgress = SmartAjax.makeRequest(ajaxOpts);
        this.addSaveInProgress(saveInProgress);
        this.triggerSavingStarted(assetId, fieldsToSave, data);
    },

    hasSavesInProgress: function () {
        return this.getSavesInProgress().length > 0;
    },

    removeSaveInProgress: function (saveInProgress) {
        this.setSavesInProgress(_.without(this.getSavesInProgress(), saveInProgress));
    },

    addSaveInProgress: function (saveInProgress) {
        var savesInProgress = this.getSavesInProgress();
        savesInProgress.push(saveInProgress);
        this.setSavesInProgress(savesInProgress);
    },

    _handleHtmlResponse: function (assetId, assetSku, fieldsToSave, responseData) {
        var instance = this;
        var responseBody = AJS.$(AJS.extractBodyFromResponse(responseData));
        var updatedXSRFToken = responseBody.find("#atl_token").val();

        // If we've received an XSRF token error, an updated token will be in the response.
        if (updatedXSRFToken) {
            AJS.$("#atlassian-token").attr("content", updatedXSRFToken);
        }

        var dialog = new JIRA.FormDialog({
            offsetTarget: "body",
            content: responseBody
        });

        this.triggerSaveError(assetId, assetSku, fieldsToSave);

        // If clicking the XSRF dialog's "Retry" button worked, continue.
        dialog._handleServerSuccess = function (xsrfResponseData) {
            dialog.hide();
            var data = instance._parseResponse(xsrfResponseData);
            if (data) {
                AssetFieldUtil.transformFieldHtml(responseData);
                instance.triggerSaveSuccess(assetId, assetSku, fieldsToSave, data);
            }
        };

        // If clicking the XSRF dialog's "Retry" button didn't work, trigger a save error
        dialog._handleServerError = function (xhr) {
            dialog.hide();
            var data = instance._parseResponse(xhr.responseText);
            if (data) {
                AssetFieldUtil.transformFieldHtml(data);
                instance.triggerSaveError(assetId, assetSku, fieldsToSave, data);
            }
        };

        dialog.show();
    },

    _handleSaveError: function (assetId, assetSku, fieldsToSave, xhr) {
        var data = this._parseResponse(xhr.responseText);
        if (data) {
            AssetFieldUtil.transformFieldHtml(data);
            this.triggerSaveError(assetId, assetSku, fieldsToSave, data);
        }
    },

    /**
     * Attempts to parse raw response to JSON. If parsing fails, shows a global error message and returns null
     * @param responseText raw http response data
     */
    _parseResponse: function (responseText) {
        try {
            return JSON.parse(responseText);
        } catch (e) {
            // parse JSON failed
            this._showFatalErrorMessage();
            return null;
        }
    },

    _showFatalErrorMessage: function () {
        // TODO: would be nice to extract this error from smartAjax and make it uniform in JIRA
        var msg = '<p>' + AJS.I18n.getText("common.forms.ajax.error.dialog.heading") + '</p>' +
            '<p>' + AJS.I18n.getText("common.forms.ajax.error.dialog") + '</p>';
        JIRA.Messages.showErrorMsg(msg, {
            closeable: true
        });
    }
});

module.exports = SaveInProgressManager;
