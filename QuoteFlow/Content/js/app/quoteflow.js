/// <reference path="../lib/underscore.d.ts" />
/// <reference path="../lib/jquery.d.ts" />
/// <reference path="../lib/backbone.d.ts" />
var QuoteFlow;
(function (QuoteFlow) {
    var _rootUrl;

    Object.defineProperty(QuoteFlow, 'RootUrl', {
        get: function () {
            return _rootUrl;
        },
        set: function (value) {
            _rootUrl = value;
        }
    });

    var _applicationPath;

    Object.defineProperty(QuoteFlow, 'ApplicationPath', {
        get: function () {
            return _applicationPath;
        },
        set: function (value) {
            _applicationPath = value;
        }
    });

    var _currentOrganizationId;

    Object.defineProperty(QuoteFlow, 'CurrentOrganizationId', {
        get: function () {
            return _currentOrganizationId;
        },
        set: function (value) {
            _currentOrganizationId = value;
        }
    });

    var _currentUserId;

    Object.defineProperty(QuoteFlow, 'CurrentUserId', {
        get: function () {
            return _currentUserId;
        },
        set: function (value) {
            _currentUserId = value;
        }
    });

    QuoteFlow.Catalog = {};
    QuoteFlow.Collection = {};
    (function (Debug) {
        Debug.Views = [];
        Debug.Models = [];
        Debug.Collections = [];
    })(QuoteFlow.Debug || (QuoteFlow.Debug = {}));
    var Debug = QuoteFlow.Debug;
    QuoteFlow.Interactive = {};
    (function (Model) {
        Model.Asset = {};
    })(QuoteFlow.Model || (QuoteFlow.Model = {}));
    var Model = QuoteFlow.Model;
    QuoteFlow.Pages = {};
    QuoteFlow.Routers = {};
    (function (UI) {
        (function (Asset) {
            Asset.Edit = {};
            Asset.Navigator = {};
        })(UI.Asset || (UI.Asset = {}));
        var Asset = UI.Asset;
        ;
        UI.Catalog = {};
        UI.Common = {};
    })(QuoteFlow.UI || (QuoteFlow.UI = {}));
    var UI = QuoteFlow.UI;

    QuoteFlow.Vent = _.extend({}, Backbone.Events);
    QuoteFlow.Views = {};

    var Initialize = (function () {
        function Initialize(rootUrl, applicationPath, currentOrgId, currentUser) {
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
        Initialize.prototype.buildRootUrl = function (context) {
            if (context === "/") {
                return context;
            } else {
                if (context.charAt(context.length - 1) === "/") {
                    return context;
                } else {
                    return context + "/";
                }
            }
        };
        return Initialize;
    })();
    QuoteFlow.Initialize = Initialize;
})(QuoteFlow || (QuoteFlow = {}));
//# sourceMappingURL=quoteflow.js.map
