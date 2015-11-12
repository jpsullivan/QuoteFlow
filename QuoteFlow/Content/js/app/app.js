"use strict";

var $;
window.jQuery = $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var BackboneBrace = require('backbone-brace');
Backbone.$ = $;

var Events = require('./util/events');
var Marionette = require('backbone.marionette');

// QuoteFlow Namespace (hold-over from non CommonJS method)
var QuoteFlow = {
    application: {},
    Backbone: {},
    Catalog: {},
    Collection: {
        Asset: {}
    },
    Components: {},
    Debug: {
        Collections: {},
        Models: {},
        Views: {}
    },
    Events: {},
    Interactive: {},
    Model: {
        Asset: {}
    },
    Module: {
        Asset: {}
    },
    Pages: {},
    Routes: {},
    trace: $.noop,
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

// Bind these jquery-like event handlers on the global namespace for now
QuoteFlow.bind = Events.bind;
QuoteFlow.unbind = Events.unbind;
QuoteFlow.one = Events.one;
QuoteFlow.trigger = Events.trigger;

window.QuoteFlow = QuoteFlow;

var NoInlineAssetEditorModule = require('./modules/asset-nav/search/asset-editor/app-module-no-inline');
var AssetModule = require('./modules/asset/module');
var AssetTableModule = require('./modules/asset-nav/module');
var CatalogModule = require('./modules/catalog/module');
var QuoteStatusModule = require('./modules/quote-status/module');

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
    // register all the handlebars helpers
    ApplicationHelpers.initialize();

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

/**
 * Autoload any modules that may exist
 */
//var LoadedModules = require('./autoload/modules')(QuoteFlow.application);

QuoteFlow.application.on("start", function (options) {
    if (Backbone.history) {
        Backbone.history.start({ pushState: true, root: QuoteFlow.ApplicationPath });
    }
});

QuoteFlow.application.module("asset", AssetModule);
QuoteFlow.application.module("assetEditor", new NoInlineAssetEditorModule().definition);
QuoteFlow.application.module("asset-table", AssetTableModule);
QuoteFlow.application.module("catalog", CatalogModule);
QuoteFlow.application.module("quote-status", QuoteStatusModule);

module.exports = QuoteFlow.application;
