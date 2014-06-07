﻿/// <reference path="../lib/underscore.d.ts" />
/// <reference path="../lib/jquery.d.ts" />
/// <reference path="../lib/backbone.d.ts" />

module QuoteFlow {
    var _rootUrl: string;
    export declare var RootUrl: string;
    Object.defineProperty(QuoteFlow, 'RootUrl', {
        get: () => _rootUrl,
        set: value => { _rootUrl = value; }
    }); 

    var _applicationPath: string;
    export declare var ApplicationPath: string;
    Object.defineProperty(QuoteFlow, 'ApplicationPath', {
        get: () => _applicationPath,
        set: value => { _applicationPath = value; }
    }); 

    var _currentOrganizationId: number;
    export declare var CurrentOrganizationId: number;
    Object.defineProperty(QuoteFlow, 'CurrentOrganizationId', {
        get: () => _currentOrganizationId,
        set: value => { _currentOrganizationId = value; }
    });

    var _currentUserId: number;
    export declare var CurrentUserId: number;
    Object.defineProperty(QuoteFlow, 'CurrentUserId', {
        get: () => _currentUserId,
        set: value => { _currentUserId = value; }
    }); 

    export module Backbone { }
    export var Catalog = {}
    export var Collection = {}
    export module Debug {
        export var Views = [];
        export var Models = [];
        export var Collections = [];
    }
    export var Model = {}
    export var Pages = {}
    export var Routers = {}
    export module UI {
        export var Asset = {};
        export var Catalog = {};
        export var Common = {};
    }
    export var Utilities = {}
    export var Vent = _.extend({}, Backbone.Events);
    export var Views = {}

    export class Initialize {
        constructor(rootUrl: string, applicationPath: string, currentOrgId: string, currentUser: any) {
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
        }

        buildRootUrl(context: string) {
            if (context === "/") {
                return context;
            } else {
                if (context.charAt(context.length - 1) === "/") {
                    return context;
                } else {
                    return context + "/";
                }
            }
        }
    }

    
}