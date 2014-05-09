QuoteFlow.UI.Catalog.SelectAssetVarModal = QuoteFlow.Views.Base.extend({
    templateName: "catalog/select-asset-var-modal",

    options: {},

    events: {
        "click #dialog-close-button": "closeModal"
    },

    initialize: function(options) {},

    postRenderTemplate: function () {
        var self = this;
        _.defer(function () {
            self.modalAJS = AJS.dialog2("#asset_var_selection_modal");
        });
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