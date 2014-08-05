QuoteFlow.UI.Asset.EditAsset = QuoteFlow.Views.Base.extend({

    el: ".aui-page-panel-content",

    options: {
        assetId: 0,
        assetVarNames: {}
    },

    events: {
        "click #add_asset_var": "showAssetVarFieldSelectionModal"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function (options) {
        this.options = options || {};

        _.bindAll(this, 'addAssetVarRow');

        this.assetVarFieldsList = this.$('#asset_var_fields');
    },

    postRenderTemplate: function () { },

    getAssetVarSelectionModalView: function () {
        // todo: dispose the existing modal object if exists
        return new QuoteFlow.UI.Catalog.SelectAssetVarModal({
            okFunc: this.addAssetVarRow
        });
    },

    /**
     * Displays the asset var modal window.
     */
    showAssetVarFieldSelectionModal: function (e) {
        e.preventDefault();

        // forcefull render the select asset var modal to reset form fields
        this.assetVarSelectionModal = this.getAssetVarSelectionModalView();
        this.$('#asset_var_selection_container').html(this.assetVarSelectionModal.render().el);
        this.assetVarSelectionModal.showModal();
    },

    /**
     * Adds an asset var row based on the select asset var modal result.
     */
    addAssetVarRow: function (assetVar) {
        if (assetVar === null) {
            // todo: throw some kind of validation failure
        }

        this.insertAssetVarValue(this.options.assetId, assetVar.get("Id"));

        var view = new QuoteFlow.UI.Asset.Edit.AssetVarEditRow({
            assetVarNames: this.options.assetVarNames,
            assetVar: assetVar
        });

        this.assetVarFieldsList.append(view.render().el);
    },

    /**
     * 
     */
    insertAssetVarValue: function (assetId, assetVarId) {
        var varValue = new QuoteFlow.Model.AssetVarValue({
            AssetId: parseInt(assetId, 10),
            AssetVarId: assetVarId,
            VarValue: "",
            OrganizationId: QuoteFlow.CurrentOrganizationId
        });

        var req = varValue.save({ wait: true });
        req.done(function (result) {
            console.log(result);
        });
    }
})