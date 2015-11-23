"use strict";

import $ from "jquery";
import _ from "underscore";
import Backbone from "backbone";
Backbone.$ = $;
import Events from "./util/events";
import Marionette from "backbone.marionette";

// so that aui works
var jquery_browser = require('jquery.browser');

import AppRouter from "./router";
import AppHeader from "./ui/app-header";

// app-wide private variables
let _rootUrl;
let _applicationPath;
let _currentOrganizationId;
let _currentUserId;

var AssetEditorModule = require('./modules/asset-nav/search/asset-editor/app-module-no-inline');
var AssetModule = require('./modules/asset/module');
var AssetTableModule = require('./modules/asset-nav/module');
var CatalogModule = require('./modules/catalog/module');
var AssetNavigationModule = require('./modules/asset-nav/navigation/navigation-app-module');
var PagerModule = require('./modules/pager/module');
// var QuoteStatusModule = require('./modules/quote-status/module');

const QuoteFlowApplication = Marionette.Application.extend({
    initialize (options) {
        this.router = new AppRouter({application: this});
        this._mapGlobalsToQuoteFlowNamespace();
        this._setUnderscoreMixins();

        var parsedOrgId = parseInt(options.currentOrgId, 10);
        var parsedUserId;

        if (_.isUndefined(options.currentUser) || _.isNull(options.currentUser)) {
            parsedUserId = 0;
        } else {
            parsedUserId = parseInt(options.currentUser.Id, 10);
        }

        _rootUrl = this._buildRootUrl(options.rootUrl);
        _applicationPath = options.applicationPath;
        _currentOrganizationId = parsedOrgId;
        _currentUserId = parsedUserId;

        this._mapProperties();
        this._initModules();

        new AppHeader();
    },

    onStart () {
        // so that webpack can hot reload changes and re-fire router events
        if (Backbone.history && Backbone.History.started) {
            Backbone.history.stop();
        }

        if (Backbone.history && !Backbone.History.started) {
            Backbone.history.start({ pushState: true, root: QuoteFlow.ApplicationPath });
        }
    },

    initModules () {
        QuoteFlow.application.module("asset", AssetModule);
        QuoteFlow.application.module("assetEditor", new AssetEditorModule().definition);
        QuoteFlow.application.module("asset-table", AssetTableModule);
        QuoteFlow.application.module("catalog", CatalogModule);
        QuoteFlow.application.module("navigation", new AssetNavigationModule().definition);
        QuoteFlow.application.module("pager", PagerModule);
        QuoteFlow.application.module("quote-status", QuoteStatusModule);
    },

    /**
     * We need to do this here since bootstrapping data into
     * browserify means bypassing the IIFE model. Map server vars
     * to the global `window` object and remap them here.
     * @private
     */
    _mapGlobalsToQuoteFlowNamespace () {
        // QuoteFlow Namespace (hold-over from non CommonJS method)
        let QuoteFlow = {
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
        QuoteFlow.rootUrl = window.rootUrl;
        QuoteFlow.applicationPath = window.applicationPath;
        QuoteFlow.currentUser = window.currentUser;
    },

    mapProperties () {
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
    },

    /**
     * Sets any Underscore mixins that are used throughout the app.
     * @private
     */
    _setUnderscoreMixins () {
        /**
         * _.move - takes array and moves item at index and moves to another index.
         */
        _.mixin({
            move: function (array, fromIndex, toIndex) {
                array.splice(toIndex, 0, array.splice(fromIndex, 1)[0]);
                return array;
            },
            lambda: function (x) {
                return function () {
                    return x;
                };
            },
            isNotBlank: function (object) {
                return Boolean(object);
            },
            bindObjectTo: function (obj, context) {
                _.map(obj, function (value, key) {
                    if (_.isFunction(value)) {
                        obj[key] = _.bind(value, context);
                    }
                });
            }
        });
    },

    /**
     * @private
     */
    _buildRootUrl: function (context) {
        if (context === "/") {
            return context;
        }

        if (context.charAt(context.length - 1) === "/") {
            return context;
        }

        return context + "/";
    }
});

export default QuoteFlowApplication;
