QuoteFlow.UI.Asset.Edit.AddAssetVarField = QuoteFlow.Views.Base.extend({

    el: ".aui-page-panel-content",

    options: {
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

        var view = new QuoteFlow.UI.Catalog.ImportAssetVarRow({
            headers: this.options.headers,
            assetVar: assetVar
        });

        //this.assetVarFieldsList.append(view.render().el);
    }
})