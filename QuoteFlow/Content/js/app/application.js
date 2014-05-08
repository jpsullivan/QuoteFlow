// Provide top-level namespaces for javascript assets.
var QuoteFlow = {
    Backbone: {},
    Catalog: {},
    Debug: {
        Views: [],
        Models: [],
        Collections: []
    },
    Models: {},
    Pages: {},
    Routers: {},
    UI: {
        Catalog: {},
        Common: {}
    },
    Utilities: {},
    Vent: _.extend({}, Backbone.Events),
    Views: {},

    initialize: function () {
        QuoteFlow.Vent.on('domchange:title', this.onDomChangeTitle, this);

        Backbone.history.start({ pushState: true, root: QuoteFlow.applicationPath });
    },

    onDomChangeTitle: function (title) {
        $(document).attr('title', "QuoteFlow - " + title);
    }
};