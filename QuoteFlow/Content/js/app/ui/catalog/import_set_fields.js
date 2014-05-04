QuoteFlow.UI.Catalog.ImportSetFields = QuoteFlow.Views.Base.extend({
    el: '.aui-page-panel-content',

    options: {
        rawRows: null
    },

    events: {
        "change .field-group select": "changeHeader",
        "click .tooltip": "showPreview"
    },

    initialize: function (options) {
        this.rows = new Backbone.Collection().reset(options.rawRows);

        AJS.$(".tooltip").tooltip();
    },

    changeHeader: function(e) {
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
    validateHeaderSelection: function(index, valueType) {
        
    },

    /**
     * 
     */
    showPreview: function(e) {
        var el = $(e.currentTarget);
        var fieldGroup = el.parent();

        var valueType = fieldGroup.data('value-type');
        var index = $('select', fieldGroup).prop('selectedIndex') - 1; // -1 to compensate for the default select opt
        var panelKey = $('.aui-lozenge', fieldGroup).html();

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