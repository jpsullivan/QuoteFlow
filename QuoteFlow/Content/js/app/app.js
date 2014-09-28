"use strict";

var $;
window.jQuery = $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

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

// App Dependencies
var aui = require('aui');
var Router = require('./router');

// Helpers
var ApplicationHelpers = require('./helpers/application_helpers');

var _rootUrl, _applicationPath, _currentOrganizationId, _currentUserId;

var Application = {

    /**
     * 
     */
    initialize: function(rootUrl, applicationPath, currentOrgId, currentUser) {
        this.mapProperties();
        this.initRouter();

        var parsedOrgId = parseInt(currentOrgId, 10);
        var parsedUserId;

        if (currentUser === undefined || currentUser === null) {
            parsedUserId = 0;
        } else {
            parsedUserId = parseInt(currentUser.Id, 10);
        }

        _rootUrl = this.buildRootUrl(rootUrl);
        _applicationPath = applicationPath;
        _currentOrganizationId = parsedOrgId;
        _currentUserId = parsedUserId;

        // register all the handlebars helpers
        ApplicationHelpers.initialize();
    },

    /**
     * 
     */
    buildRootUrl: function() {
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
    mapProperties: function() {
        Object.defineProperty(QuoteFlow, 'RootUrl', {
            get: function() {
                return _rootUrl;
            },
            set: function(value) {
                _rootUrl = value;
            }
        });

        Object.defineProperty(QuoteFlow, 'ApplicationPath', {
            get: function() {
                return _applicationPath;
            },
            set: function(value) {
                _applicationPath = value;
            }
        });

        Object.defineProperty(QuoteFlow, 'CurrentOrganizationId', {
            get: function() {
                return _currentOrganizationId;
            },
            set: function(value) {
                _currentOrganizationId = value;
            }
        });

        Object.defineProperty(QuoteFlow, 'CurrentUserId', {
            get: function() {
                return _currentUserId;
            },
            set: function(value) {
                _currentUserId = value;
            }
        });
    },

    /**
     * News up a fresh router.
     */
    initRouter: function() {
        QuoteFlow.Router = new Router();
        Backbone.history.start({ pushState: true, root: QuoteFlow.ApplicationPath });
    }
};

Application.initialize(window.rootUrl, window.applicationPath, window.currentOrganization, window.currentUser);

module.exports = Application;
