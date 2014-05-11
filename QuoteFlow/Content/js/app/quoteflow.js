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

    QuoteFlow.Catalog = {};
    QuoteFlow.Collection = {};
    (function (Debug) {
        Debug.Views = [];
        Debug.Models = [];
        Debug.Collections = [];
    })(QuoteFlow.Debug || (QuoteFlow.Debug = {}));
    var Debug = QuoteFlow.Debug;
    QuoteFlow.Models = {};
    QuoteFlow.Pages = {};
    QuoteFlow.Routers = {};
    (function (UI) {
        UI.Catalog = {};
        UI.Common = {};
    })(QuoteFlow.UI || (QuoteFlow.UI = {}));
    var UI = QuoteFlow.UI;
    QuoteFlow.Utilities = {};
    QuoteFlow.Vent = _.extend({}, Backbone.Events);
    QuoteFlow.Views = {};

    var Initialize = (function () {
        function Initialize(rootUrl, applicationPath, currentOrgId) {
            var parsedOrgId = parseInt(currentOrgId, 10);

            _rootUrl = this.buildRootUrl(rootUrl);
            _applicationPath = applicationPath;
            _currentOrganizationId = parsedOrgId;
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
