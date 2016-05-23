"use strict";

var $ = require('jquery');
var moment = require('moment');
var Marionette = require('backbone.marionette');

var AssetVarCollection = require('../../../collections/asset_vars');
var AssetVarModel = require('../../../models/asset_var');

/**
 * [extend description]
 * @extends Marionette.ItemView
 */
var SelectAssetVarDialog = Marionette.ItemView.extend({
    template: JST["catalog/select-asset-var-modal"],

    ui: {
        assetVarsDropdown: "#select_asset_var",
        newAssetVarTextbox: "#new_asset_var",
        button: ".dialog-save-button",
        cancel: ".dialog-cancel-button",
        footerSpinner: ".dialog-footer-spinner"
    },

    events: {
        "keyup @ui.newAssetVarTextbox": "newAssetVarKeypressHandler",
        "click @ui.button": "onButtonClick",
        "click @ui.cancel": "onCancelClick"
    },

    serializeData: function () {
        return {
            assetVars: this.assetVars.toJSON()
        };
    },

    initialize: function (options) {
        this.dialogSubmitFn = options.dialogSubmitFn;
        this.assetVars = this._fetchAssetVars();
    },

    onRender: function () {
        this.assetVarsSelect2Dropdown = this.ui.assetVarsDropdown.auiSelect2();
        this._formDisabled = false;
        this.ui.cancel.focus();
    },

    onButtonClick: function () {
        this.disableForm();

        var assetVar;
        if (this.assetVarsSelect2Dropdown.prop('disabled')) {
            // user opted to create a new assetvar
            this._createAssetVar(this.getNewAssetVarName());
            this.assetVars = this._fetchAssetVars(); // re-fetch collection to get the id
            assetVar = this.assetVars.at(this.assetVars.length - 1);
        } else {
            // the user has selected an existing assetvar
            var assetVarId = this.getSelectedExistingAssetVar();
            assetVar = this.assetVars.findWhere({ id: parseInt(assetVarId, 10) });
        }

        this.dialogSubmitFn(assetVar);
        this.trigger("cancel-requested");
    },

    onCancelClick: function (e) {
        e.preventDefault();
        this.trigger("cancel-requested");
    },

    disableForm: function () {
        this.enableButton(false);
        this.ui.footerSpinner.removeClass("hidden");
    },

    enableButton: function (enable) {
        var enabled = enable === undefined ? true : enable;

        this.ui.button.prop("disabled", !enabled);
        this.ui.button.attr("aria-disabled", "" + !enabled);
    },

    newAssetVarKeypressHandler: function (e) {
        var el = $(e.currentTarget);

        if (el.val() !== "") {
            this._disableAssetVarsDropdown();
        } else {
            this._enableAssetVarsDropdown();
        }
    },

    _disableAssetVarsDropdown: function () {
        this.assetVarsSelect2Dropdown.select2("enable", false);
    },

    _enableAssetVarsDropdown: function () {
        this.assetVarsSelect2Dropdown.select2("enable", true);
    },

    _fetchAssetVars: function () {
        var assetVars = new AssetVarCollection();
        assetVars.fetch({
            data: $.param({ id: QuoteFlow.CurrentOrganizationId }),
            async: false
        });

        return assetVars;
    },

    _createAssetVar: function (assetVarName) {
        if (assetVarName === "") {
            // todo: return failed validation for empty string
        }

        // does this assetvar exist yet?
        var existing = this.assetVars.findWhere({ name: assetVarName });
        if (existing !== undefined) {
            // todo: return failed validation because assetvar already exists
        }

        var assetVar = new AssetVarModel({
            name: assetVarName,
            description: null,
            valueType: "String",
            organizationId: QuoteFlow.CurrentOrganizationId,
            enabled: true,
            createdUtc: moment().format(),
            createdBy: QuoteFlow.CurrentUserId
        });

        return this.assetVars.create(assetVar, { wait: true });
    },

    getNewAssetVarName: function () {
        return this.ui.newAssetVarTextbox.val();
    },

    getSelectedExistingAssetVar: function () {
        return this.assetVarsSelect2Dropdown.val();
    }
});

module.exports = SelectAssetVarDialog;
