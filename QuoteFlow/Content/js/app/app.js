"use strict";

var $;
window.jQuery = $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var BackboneBrace = require('backbone-brace');
Backbone.$ = $;

var Marionette = require('backbone.marionette');

var AssetModule = require('./modules/asset/module');
var AssetTableModule = require('./modules/asset-nav/module');
var CatalogModule = require('./modules/catalog/module');

// QuoteFlow Namespace (hold-over from non CommonJS method)
var QuoteFlow = {
    application: {},
    Backbone: {},
    Catalog: {},
    Collection: {
        Asset: {}
    },
    Debug: {
        Collections: {},
        Models: {},
        Views: {}
    },
    Components: {},
    Interactive: {},
    Model: {
        Asset: {}
    },
    Module: {
        Asset: {}
    },
    Pages: {},
    Routes: {},
    Utilities: {},
    UI: {
        Asset: {
            Edit: {},
            Navigator: {}
        },
        Catalog: {},
        Common: {}
    },
    Views: {}
};

window.QuoteFlow = QuoteFlow;

// App Dependencies
var jquery_browser = require('jquery.browser'); // so that aui works
var Router = require('./router');

// Helpers
var ApplicationHelpers = require('./helpers/application_helpers');

var _rootUrl, _applicationPath, _currentOrganizationId, _currentUserId;

var Application = Marionette.Application.extend({

    /**
     *
     */
    initialize: function (options) {
        var parsedOrgId = parseInt(options.currentOrgId, 10);
        var parsedUserId;

        if (_.isUndefined(options.currentUser) || _.isNull(options.currentUser)) {
            parsedUserId = 0;
        } else {
            parsedUserId = parseInt(options.currentUser.Id, 10);
        }

        _rootUrl = this.buildRootUrl(options.rootUrl);
        _applicationPath = options.applicationPath;
        _currentOrganizationId = parsedOrgId;
        _currentUserId = parsedUserId;

        this.mapProperties();
    },

    /**
     *
     */
    buildRootUrl: function (context) {
        if (context === "/") {
            return context;
        } else {
            if (context.charAt(context.length - 1) === "/") {
                return context;
            } else {
                return context + "/";
            }
        }
    },

    /**
     *
     */
    mapProperties: function () {
        Object.defineProperty(QuoteFlow, 'RootUrl', {
            get: function () {
                return _rootUrl;
            },
            set: function (value) {
                _rootUrl = value;
            }
        });

        Object.defineProperty(QuoteFlow, 'ApplicationPath', {
            get: function () {
                return _applicationPath;
            },
            set: function (value) {
                _applicationPath = value;
            }
        });

        Object.defineProperty(QuoteFlow, 'CurrentOrganizationId', {
            get: function () {
                return _currentOrganizationId;
            },
            set: function (value) {
                _currentOrganizationId = value;
            }
        });

        Object.defineProperty(QuoteFlow, 'CurrentUserId', {
            get: function () {
                return _currentUserId;
            },
            set: function (value) {
                _currentUserId = value;
            }
        });
    }
});

QuoteFlow.application = new Application({
    rootUrl: window.rootUrl,
    applicationPath: window.applicationPath,
    currentOrgId: window.currentOrganization,
    currentUser: window.currentUser
});

QuoteFlow.application.on("before:start", function (options) {
    // add some mixins to underscore
    _.mixin({
        lambda: function(x) {
            return function() { return x; };
        },
        isNotBlank: function(object) {
            return !!object;
        },
        bindObjectTo: function(obj, context) {
            _.map(obj, function(value, key) {
                if (_.isFunction(value)) {
                    obj[key] = _.bind(value, context);
                }
            });
        }
    });
});

QuoteFlow.application.on("start", function (options) {
    if (Backbone.history) {
        Backbone.history.start({ pushState: true, root: QuoteFlow.ApplicationPath });
    }

    // register all the handlebars helpers
    ApplicationHelpers.initialize();
});

QuoteFlow.application.module("asset", AssetModule);
QuoteFlow.application.module("asset-table", AssetTableModule);
QuoteFlow.application.module("catalog", CatalogModule);

module.exports = QuoteFlow.application;
