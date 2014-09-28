"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

var BaseView = require('../../../view');

/**
 *
 */
var AssetVarEditRow = BaseView.extend({
    className: 'field-group',

    templateName: 'asset/edit/asset-var-edit-row',

    options: {
        assetVarNames: {},
        assetVar: null
    },

    events: {
        "click #add_asset_var": "showAssetVarFieldSelectionModal"
    },

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
            assetVar: this.options.assetVar.toJSON(),
            assetVarNames: this.options.assetVarNames,
            buttonOwns: "assetvar_" + this.options.assetVar.get('Id')
        });
    },

    initialize: function(options) {
        this.options = options || {};
    },

    postRenderTemplate: function() {
        _.defer(function() {
            AJS.$('select').auiSelect2();
        });
    },

    /**
     * Removes the asset var row from the table. Disposes the view.
     */
    removeRow: function() {
        this.remove();
    }
});

module.exports = AssetVarEditRow;
