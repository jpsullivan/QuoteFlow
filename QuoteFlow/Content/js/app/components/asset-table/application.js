"use strict";

var $;
window.jQuery = $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var BackboneBrace = require('backbone-brace');
Backbone.$ = $;

var Marionette = require('backbone.marionette');

var _rootUrl, _applicationPath, _currentOrganizationId, _currentUserId;

var AssetTableApplication = Marionette.Application.extend({
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

        // register all the handlebars helpers
        ApplicationHelpers.initialize();
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

var qfApp = new AssetTableApplication({
    rootUrl: window.rootUrl,
    applicationPath: window.applicationPath,
    currentOrganization: window.currentOrganization,
    currentUser: window.currentUser
});

qfApp.on("start", function (options) {
    if (Backbone.history) {
        Backbone.history.start({ pushState: true, root: QuoteFlow.ApplicationPath });
    }
});

module.exports = AssetTableApplication;