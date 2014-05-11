QuoteFlow.UI.Catalog.SelectAssetVarModal = QuoteFlow.Views.Base.extend({
    templateName: "catalog/select-asset-var-modal",

    options: {},

    events: {
        "click #dialog-close-button": "closeModal"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
            assetVars: this.assetVars.toJSON()
        });
    },

    initialize: function(options) {
        this.assetVars = this.fetchAssetVars();
    },

    postRenderTemplate: function () {
        var self = this;

        this.assetVars = 

        _.defer(function () {
            self.modalAJS = AJS.dialog2("#asset_var_selection_modal");
        });
    },

    fetchAssetVars: function() {
        var assetVars = new QuoteFlow.Collection.AssetVars();
        assetVars.fetch({
            data: $.param({ id: QuoteFlow.CurrentOrganizationId }),
            async: false
        });

        return assetVars;
    },

    showModal: function () {
        var self = this;
        _.defer(function() {
            self.modalAJS.show();
        });
    },

    closeModal: function () {
        this.modalAJS.remove();
        this.remove();
    }
})