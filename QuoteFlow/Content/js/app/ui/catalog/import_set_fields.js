QuoteFlow.UI.Catalog.ImportSetFields = QuoteFlow.Views.Base.extend({
    el: '.aui-page-panel-content',

    options: {
        rawRows: null
    },

    events: {
        "change .field-group select": "changeHeader"
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

    validateHeaderSelection: function(index, valueType) {
        
    }
})