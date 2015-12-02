"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Brace = require('backbone-brace');
var Marionette = require('backbone.marionette');

var AuiSelect2 = require('@atlassian/aui/lib/js/aui/select2');
var Dialog2 = require('@atlassian/aui/lib/js/aui/dialog2');
var Tooltip = require('@atlassian/aui/lib/js/aui/tooltip');

var AssetVarCollection = require('../../../collections/asset_vars');
var AssetVarModel = require('../../../models/asset_var');
var ImportAssetVarRow = require('./import_asset_var_row');
var PanelTable = require('../../../ui/common/panel-table');
var SelectAssetVarModal = require('./select_asset_var_modal');

/**
 * [extend description]
 * @extends Marionette.ItemView
 */
var ImportSetOptionalFields = Marionette.ItemView.extend({
    el: '.aui-page-panel-content',

    ui: {
        newAssetVarFieldButton: "#new_asset_var_field",
        fieldGroupDropdown: ".field-group select",
        tooltip: ".tooltip"
    },

    events: {
        "click @ui.newAssetVarFieldButton": "showAssetVarFieldSelectionModal",
        "change @ui.fieldGroupDropdown": "changeHeader",
        "click @ui.tooltip": "showPreview"
    },

    /**
     * [function description]
     * @param  {[type]} options.headers [description]
     * @param  {[type]} options.rawRows [description]
     * @return {[type]}         [description]
     */
    initialize: function(options) {
        this.headers = options.headers;

        _.bindAll(this, 'addAssetVarRow');

        this.rows = new Brace.Collection().reset(options.rawRows);

        this.assetVarFieldsList = this.$('table#asset_var_fields tbody');
        this._initDialogs();

        AJS.$(".tooltip").tooltip();
        AJS.$('select').auiSelect2();
    },

    _initDialogs: function () {
        var selectAssetDialogView = new SelectAssetVarModal({
            el: "#asset-var-selection-dialog",
            dialogSubmitFn: _.bind(this.addAssetVarRow, this)
        });
        this.selectAssetDialog = AJS.dialog2("#asset-var-selection-dialog");
        this.selectAssetDialog.on("show", _.bind(function () {
            selectAssetDialogView.render();
        }, this));
        this.listenTo(selectAssetDialogView, "cancel-requested", _.bind(function () {
            // required to wrap this in a dumb bind fn or else it never hides
            this.selectAssetDialog.hide();
        }, this));
    },

    showAssetVarFieldSelectionModal: function() {
        return this.selectAssetDialog.show();
    },

    changeHeader: function(e) {
        var el = $(e.currentTarget);
        var index = el.prop('selectedIndex');

        if (index === 0) {
            return;
        }

        var valueType = el.parent('.field-group').data('value-type');

        this.validateHeaderSelection(index, valueType);
    },

    /**
     * Determines if a random sample of the CSV rows passes the value type check.
     * This is of course a dirty check that doesn't guarantee 100% exact results,
     * but assuming that the input data isn't total garbage, should yield correct estimations.
     */
    validateHeaderSelection: function(index, valueType) {

    },

    /**
     * Gathers a random collection of data from the selected
     * header group and displays it in a table.
     */
    showPreview: function(e) {
        var el = $(e.currentTarget);
        var fieldGroup = el.parent();

        var valueType = fieldGroup.data('value-type');
        var index = $('select', fieldGroup).prop('selectedIndex');
        var panelKey = $('.aui-lozenge', fieldGroup).html();

        if (index === 0) {
            //            // show an inline popup
            //            AJS.InlineDialog(AJS.$("select", fieldGroup), "myDialog",
            //                function(content, trigger, showPopup) {
            //                    content.css({ "padding": "20px" }).html('<p>Please select a field header.</p>');
            //                    showPopup();
            //                    return false;
            //                }
            //            );
            return false;
        }

        index = index - 1; // -1 to compensate for the default select opt

        var panelView = new PanelTable({
            leftHeader: "Asset Key",
            rightHeader: "Sample Values",
            rowKey: panelKey,
            rowData: this.getSampleRowData(index)
        });

        $('.sample-data-container', fieldGroup).html(panelView.render().el);
    },

    /**
     *
     */
    getSampleRowData: function(index) {
        return _.sample(this.rows.pluck(index), 3);
    },

    /**
     * Adds an asset var row based on the select asset var modal result.
     */
    addAssetVarRow: function(assetVar) {
        if (assetVar === null) {
            // todo: throw some kind of validation failure
        }

        var view = new ImportAssetVarRow({
            headers: this.headers,
            assetVar: assetVar
        });

        this.assetVarFieldsList.append(view.render().el);
    }
});

module.exports = ImportSetOptionalFields;
