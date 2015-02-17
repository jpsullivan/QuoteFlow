"use strict";

var $;
window.jQuery = $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var BackboneBrace = require('backbone-brace');
Backbone.$ = $;

var Marionette = require('backbone.marionette');

var CatalogModule = require('./modules/catalog/module');

// QuoteFlow Namespace (hold-over from non CommonJS method)
var QuoteFlow = {
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
    Vent: _.extend({}, Backbone.Events),
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

var qfApp = new Application({
    rootUrl: window.rootUrl,
    applicationPath: window.applicationPath,
    currentOrgId: window.currentOrganization,
    currentUser: window.currentUser
});

qfApp.on("start", function (options) {
    if (Backbone.history) {
        Backbone.history.start({ pushState: true, root: QuoteFlow.ApplicationPath });
    }

    // register all the handlebars helpers
    ApplicationHelpers.initialize();
});

qfApp.module("catalog", CatalogModule);

module.exports = qfApp;
