QuoteFlow.UI.Catalog.ImportSetOptionalFields = QuoteFlow.Views.Base.extend({
    el: '.aui-page-panel-content',

    options: {
        rawRows: null
    },

    events: {
        "click #new_asset_var_field": "showAssetVarFieldSelectionModal",
        "change .field-group select": "changeHeader",
        "click .tooltip": "showPreview"
    },

    subviews: {
        "#asset_var_selection_container": "assetVarSelectionModal"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {});
    },

    initialize: function (options) {
        this.rows = new Backbone.Collection().reset(options.rawRows);

        this.assetVarFieldsList = this.$('table#asset_var_fields tbody');
        this.assetVarSelectionModalContainer = this.$('#asset_var_selection_container');
        this.assetVarSelectionModal = this.getAssetVarSelectionModalView();

        this.renderSubviews(); // manually call since this view technically isn't ever rendered

        AJS.$(".tooltip").tooltip();
        AJS.$('select').auiSelect2();
    },

    getAssetVarSelectionModalView: function () {
        // todo: dispose the existing modal object if exists
        return new QuoteFlow.UI.Catalog.SelectAssetVarModal();
    },

    showAssetVarFieldSelectionModal: function () {
        // forcefull render the select asset var modal
        this.assetVarSelectionModal.showModal();
    },

    changeHeader: function (e) {
        var el = $(e.currentTarget);
        var index = el.prop('selectedIndex');

        if (index === 0) return;

        var valueType = el.parent('.field-group').data('value-type');

        this.validateHeaderSelection(index, valueType);
    },

    /**
     * Determines if a random sample of the CSV rows
     * passes the value type check. This is of course
     * a dirty check that doesn't guarantee 100% exact
     * results, but assuming that the input data isn't 
     * total garbage, should yield correct estimations.
     */
    validateHeaderSelection: function (index, valueType) {

    },

    /**
     * Gathers a random collection of data from the selected
     * header group and displays it in a table.
     */
    showPreview: function (e) {
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

        var panelView = new QuoteFlow.UI.Common.PanelTable({
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
    getSampleRowData: function (index) {
        return _.sample(this.rows.pluck(index), 3);
    }
})