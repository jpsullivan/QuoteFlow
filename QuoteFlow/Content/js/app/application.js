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
    Views: {},
    Pages: {},
    Routers: {},
    UI: {
       Catalog: {}
    },
    Utilities: {},
    Vent: _.extend({}, Backbone.Events),

    initialize: function () {
        QuoteFlow.Vent.on('domchange:title', this.onDomChangeTitle, this);

        Backbone.history.start({ pushState: true, root: QuoteFlow.applicationPath });
    },

    onDomChangeTitle: function (title) {
        $(document).attr('title', "QuoteFlow - " + title);
    }
};