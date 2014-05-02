// Provide top-level namespaces for our javascript.
var QuoteFlow = {
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
        admin: {
            overview: {},
            ProfileDetails: {
                PickupsEditor: {}
            },
            Router: {}
        },
        asset: {},
        search: {},
        settings: {
            Account: {},
            Preferences: {},
            ProfileDetails: {},
            Router: {}
        },
        sort: {},
        wizards: {
            reports: {}
        }
    },
    Utilities: {},
    Vent: _.extend({}, Backbone.Events),

    initialize: function () {
        QuoteFlow.Vent.on('domchange:title', this.onDomChangeTitle, this);

        this.setHandlebarsHelpers();

        Backbone.history.start({ pushState: true, root: QuoteFlow.applicationPath });
    },

    onDomChangeTitle: function (title) {
        $(document).attr('title', "QuoteFlow - " + title);
    },

    setHandlebarsHelpers: function () {
        Handlebars.registerHelper('eachkeys', function (context, options) {
            var fn = options.fn, inverse = options.inverse, ret = "", empty = true;

            for (key in context) { empty = false; break; }

            if (!empty) {
                for (key in context) {
                    ret = ret + fn({ 'key': key, 'value': context[key] });
                }
            } else {
                ret = inverse(this);
            }
            return ret;
        });

        Handlebars.registerHelper('ifCond', function (v1, v2, options) {
            if (v1 == v2) {
                return options.fn(this);
            }
            return options.inverse(this);
        });

        // debug helper
        // usage: {{debug}} or {{debug someValue}}
        // from: @commondream (http://thinkvitamin.com/code/handlebars-js-part-3-tips-and-tricks/)
        Handlebars.registerHelper('debug', function (optionalValue) {
            console.log("Current Context");
            console.log("====================");
            console.log(this);

            if (optionalValue) {
                console.log("Value");
                console.log("====================");
                console.log(optionalValue);
            }
        });

        //  return the first item of a list only
        // usage: {{#first items}}{{name}}{{/first}}
        Handlebars.registerHelper('first', function (context, block) {
            return block(context[0]);
        });
    }
};