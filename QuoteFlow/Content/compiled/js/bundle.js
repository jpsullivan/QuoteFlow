(function e(t,n,r){function s(o,u){if(!n[o]){if(!t[o]){var a=typeof require=="function"&&require;if(!u&&a)return a(o,!0);if(i)return i(o,!0);var f=new Error("Cannot find module '"+o+"'");throw f.code="MODULE_NOT_FOUND",f}var l=n[o]={exports:{}};t[o][0].call(l.exports,function(e){var n=t[o][1][e];return s(n?n:e)},l,l.exports,e,t,n,r)}return n[o].exports}var i=typeof require=="function"&&require;for(var o=0;o<r.length;o++)s(r[o]);return s})({"./QuoteFlow/Content/js/app/app.js":[function(require,module,exports){
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

},{"./helpers/application_helpers":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\helpers\\application_helpers.js","./router":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\router.js","aui":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\lib\\aui\\aui.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\collections\\asset_vars.js":[function(require,module,exports){
QuoteFlow.Collection.AssetVars = Backbone.Collection.extend({

    model: QuoteFlow.Model.AssetVar,

    url: function() {
        return QuoteFlow.RootUrl + "api/assetvar";
    }
});
},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\components\\routes.js":[function(require,module,exports){
"use strict";

var Utilities = require('./utilities');

var Routes = {

    ///////////////////////////
    /// ASSET ROUTES
    ///////////////////////////
    asset: function(routeValues) {
        var assetId = routeValues.hash.id;
        var assetName = routeValues.hash.name;

        return '{0}asset/{1}/{2}'.f(QuoteFlow.RootUrl, assetId, Utilities.urlFriendly(assetName));
    },

    editAsset: function(routeValues) {
        var assetId = routeValues.hash.id;
        var assetName = routeValues.hash.name;

        return '{0}asset/{1}/{2}/edit'.f(QuoteFlow.RootUrl, assetId, Utilities.urlFriendly(assetName));
    },

    ///////////////////////////
    /// MANUFACTURER ROUTES
    //////////////////////////
    manufacturer: function(routeValues) {
        var manufacturerId = routeValues.hash.id;
        var manufacturerName = routeValues.hash.name;

        return '{0}manufacturer/{1}/{2}'.f(QuoteFlow.RootUrl, manufacturerId, Utilities.urlFriendly(manufacturerName));
    },

    editManufacturer: function(routeValues) {
        var manufacturerId = routeValues.hash.id;
        var manufacturerName = routeValues.hash.name;

        return '{0}manufacturer/{1}/{2}/edit'.f(QuoteFlow.RootUrl, manufacturerId, Utilities.urlFriendly(manufacturerName));
    },

    manufacturerLogo: function(routeValues) {
        var manufacturerId = routeValues.hash.id;
        var manufacturerName = routeValues.hash.name;

        return '{0}manufacturer/{1}/{2}/logo'.f(QuoteFlow.RootUrl, manufacturerId, Utilities.urlFriendly(manufacturerName));
    }
};

module.exports = Routes;
},{"./utilities":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\components\\utilities.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\components\\utilities.js":[function(require,module,exports){
"use strict";

var HtmlSanitizer = require('html-sanitizer');
var parseUri = require('parseuri');

/**
 * General utility functions
 */
var Utilities = {

    /**
        Takes a url and returns only the part that does not contain
        either the root or the application path.

        For example, a url of http://localhost:18272/this/is/theApplicationPath/admin/1,
        would yield the result of "admin/1".
    **/
    pathFromUrl: function (url, enforceTrailingSlash) {
        if (!url) {
            return false;
        }

        var parsedUrl = parseUri(url);
        var path = parsedUrl.path.replace(QuoteFlow.ApplicationPath, "");

        if (enforceTrailingSlash) {
            if (path.charAt(path.length - 1) !== "/") {
                path += "/";
            }
        }

        return path;
    },

    /**
        Used in API request fail handlers to parse a standard api error
        response json for the message to display
    **/
    getRequestErrorMessage: function (request) {
        var message,
            msgDetail;

        // Can't really continue without a request
        if (!request) {
            return null;
        }

        // Seems like a sensible default
        message = request.statusText;

        // If a non 200 response
        if (request.status !== 200) {
            try {
                // Try to parse out the error, or default to "Unknown"
                message = request.responseJSON.error || "Unknown Error";
            } catch (e) {
                msgDetail = request.status ? request.status + " - " + request.statusText : "Server was not available";
                message = "The server returned an error (" + msgDetail + ").";
            }
        }

        return message;
    },

    /**
        Fetch any URL vars (query strings) that may exist
    **/
    getUrlVariables: function () {
        var vars = [],
            hash,
            hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&'),
            i;

        for (i = 0; i < hashes.length; i += 1) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },

    /**
     * A collection of triggers that fire whenever a resize is detected.
     * These triggers are namespaced within the QuoteFlow.Interactive convention
     * because they really shouldn't be used outside of a full-screen environment such
     * as the interactive asset viewer / quote builder.
     */
    initializeResizeHooks: function() {
        var horizontal = "horizontalResize",
                vertical = "verticalResize";

        QuoteFlow.Interactive.offHorizontalResize = function (c) {
            AJS.$(document).off(horizontal, c);
        };
        QuoteFlow.Interactive.onHorizontalResize = function (c) {
            AJS.$(document).on(horizontal, c);
        };
        QuoteFlow.Interactive.triggerHorizontalResize = _.throttle(function () {
            AJS.$(document).trigger(horizontal);
        }, 100);
        QuoteFlow.Interactive.offVerticalResize = function (c) {
            AJS.$(document).off(vertical, c);
        };
        QuoteFlow.Interactive.onVerticalResize = function (c) {
            AJS.$(document).on(vertical, c);
        };
        QuoteFlow.Interactive.triggerVerticalResize = _.throttle(function () {
            AJS.$(document).trigger(vertical);
        }, 100);

        jQuery(window).resize(QuoteFlow.Interactive.triggerVerticalResize);
    },

    /**
     * Produces optional, URL-friendly version of a title, "like-this-one".
     * Totally copied from the UrlHelpers class and seems to work fine.
     */
    urlFriendly: function (title) {
        if (title === "") {
            return "";
        }

        var maxlen = 80,
          len = title.length,
          prevdash = false,
          sb = [],
          s, c;

        for (var i = 0; i < len; i++) {
            c = title[i];
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) {
                sb.push(c);
                prevdash = false;
            } else if (c >= 'A' && c <= 'Z') {
                sb.push(c.toLowerCase());
                prevdash = false;
            } else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c ==
              '-' || c == '_') {
                if (!prevdash && sb.length > 0) {
                    sb.push('-');
                    prevdash = true;
                }
            } else if (c >= 128) {
                s = c.toLowerCase();
                if ("àåáâäãåą".indexOf("s") > -1) {
                    sb.push("a");
                } else if ("èéêëę".indexOf("s") > -1) {
                    sb.push("e");
                } else if ("ìíîïı".indexOf("s") > -1) {
                    sb.push("i");
                } else if ("òóôõöø".indexOf("s") > -1) {
                    sb.push("o");
                } else if ("ùúûü".indexOf("s") > -1) {
                    sb.push("u");
                } else if ("çćč".indexOf("s") > -1) {
                    sb.push("c");
                } else if ("żźž".indexOf("s") > -1) {
                    sb.push("z");
                } else if ("śşš".indexOf("s") > -1) {
                    sb.push("s");
                } else if ("ñń".indexOf("s") > -1) {
                    sb.push("n");
                } else if ("ýŸ".indexOf("s") > -1) {
                    sb.push("y");
                } else if (c == 'ł') {
                    sb.push("l");
                } else if (c == 'đ') {
                    sb.push("d");
                } else if (c == 'ß') {
                    sb.push("ss");
                } else if (c == 'ğ') {
                    sb.push("g");
                }
                prevdash = false;
            }
            if (i == maxlen) break;
        }
        return prevdash ? sb.substr(0, sb.length - 1) : sb.join("");
    }
};

module.exports = Utilities;
},{"html-sanitizer":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\lib\\html-sanitizer-bundle.js","parseuri":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\parseuri\\index.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\helpers\\application_helpers.js":[function(require,module,exports){
"use strict";

var moment = require('moment');
var Routes = require('../components/routes');

var ApplicationHelpers = {

    /**
     * Just a wrapper method to allow for Handlebars templates to register.
     */
    initialize: function() {
        /**
          Determines a 

          @method eachKeys
          @for Handlebars
        **/
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

        /**
          Allows an inline if statement to be performed within the markup
        
          @method ifCond
          @for Handlebars
        **/
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

        // Gravatar thumbnail
        // Usage: {{#gravatar email size="64"}}{{/gravatar}} [depends on md5.js]
        // Author: Makis Tracend (@tracend)
        Handlebars.registerHelper('gravatar', function (context, options) {

            var email = context;
            var size = (typeof (options.hash.size) === "undefined") ? 32 : options.hash.size;

            return "http://www.gravatar.com/avatar/" + MD5(email) + "?s=" + size;
        });

        // Converts a decimal to a percentage (including the % sign)
        // Usage: {{#percentage 0.15}}{{/percentage}}
        Handlebars.registerHelper('percentage', function (value) {
            return (value * 100).toFixed(2) + " %";
        });

        // Wraps a "select" element in a handlebars template with {{#select foo}}.
        // usage: {{#select foo}} <option value="bar">Baz</option> {{/select}}
        Handlebars.registerHelper('select', function (value, options) {
            // Create a select element 
            var select = document.createElement('select');

            // Populate it with the option HTML
            select.innerHTML = options.fn(this);

            // Set the value
            select.value = value;

            // Find the selected node, if it exists, add the selected attribute to it
            if (select.children[select.selectedIndex]) {
                select.children[select.selectedIndex].setAttribute('selected', 'selected');
            } else { //select first option if that exists
                if (select.children[0]) {
                    select.children[0].setAttribute('selected', 'selected');
                }
            }

            return select.innerHTML;
        });

        /**
         * Produces a url based on the route name and its values. The route name
         * is just the name of the function within the routes.js file. The routeValues
         * argument is taken in via an options hash (arg1=arg1) and sent through as a 
         * key-value object.
         * Usage: {{#routeUrl 'showAsset' arg1=arg1 arg2=arg2 }}
         */
        Handlebars.registerHelper('routeUrl', function (routeName, routeValues) {
            return Routes[routeName](routeValues);
        });


        var dateFormats = {
            "short": "DD MMMM - YYYY",
            "long": "dddd DD.MM.YYYY HH:mm",
            "comment": "DD/MMM/YY hh:mm A"
        };

        Handlebars.registerHelper("formatDate", function (datetime, format) {
            if (moment) {
                if (format === "relative") {
                    return moment(datetime).fromNow();
                }

                var f = dateFormats[format];
                return moment(datetime).format(f);
            }
            else {
                return datetime;
            }
        });
    }
};

module.exports = ApplicationHelpers;
},{"../components/routes":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\components\\routes.js","moment":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\moment\\moment.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\models\\asset_var.js":[function(require,module,exports){
QuoteFlow.Model.AssetVar = Backbone.Model.extend({
    
    url: function() {
        return QuoteFlow.RootUrl + "api/assetvar";
    },

    defaults: function() {
        return {
            Id: null,
            Name: "",
            Description: "",
            ValueType: "",
            OrganizationId: "",
            Enabled: true,
            CreatedUtc: null,
            CreatedBy: 0
        }
    },

    isEnabled: function () {
        var enabled = this.get("Enabled");
        if (enabled === false || enabled === undefined) {
            return false;
        }
        return true;
    },
})
},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\models\\asset_var_value.js":[function(require,module,exports){
QuoteFlow.Model.AssetVarValue = Backbone.Model.extend({

    url: function () {
        return QuoteFlow.RootUrl + "api/assetvarvalue";
    },

    defaults: function () {
        return {
            Id: null,
            AssetId: 0,
            VarValue: "",
            AssetVarId: 0,
            OrganizationId: 0
        }
    }
})
},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\pages\\asset.js":[function(require,module,exports){
"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var SubRoute = require('backbone.subroute');
Backbone.$ = $;

var BaseView = require('../view');

// UI Components
var EditAsset = require('../ui/asset/edit');
var ShowAsset = require('../ui/asset/show');

/**
 *
 */
var AssetPage = BaseView.extend({
    templateName: null,
    subviews: {},
    events: {},
    options: {},

    presenter: function () { },

    initialize: function () {
        var data = window.assetDetailsHeaderData;
        if (data) {
            var headerView = new AssetDetailsHeader(data);
            var assetDetailsGrid = new AssetDetailsGrid();
        }
    },

    postRenderTemplate: function () { },

    /**
     * 
     */
    show: function(assetId, assetName) {
        var view = new ShowAsset();
    },

    /**
     * 
     */
    edit: function(assetId, assetName) {
        // get the bootstrapped data
        var data = window.editAssetData;
        if (data) {
            var assetId = data.assetId;
            var assetVarNames = data.assetVarNames;

            var view = new EditAsset({
                assetId: assetId,
                assetVarNames: assetVarNames
            });
        }

        AJS.$(document).ready(function () {
            AJS.$('select').auiSelect2();
        }); 
    }
});

AssetPage.Router = Backbone.SubRoute.extend({
    routes: {
        "asset/:assetId/:assetName": "show",
        "asset/:assetId/:assetName/edit": "edit"
    },

    show: function (assetId, assetName) {
        return new AssetPage().show(assetId, assetName);
    },

    edit: function(assetId, assetName) {
        return new AssetPage().edit(assetId, assetName);
    }
});

module.exports = AssetPage;

},{"../ui/asset/edit":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\asset\\edit.js","../ui/asset/show":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\asset\\show.js","../view":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\view.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","backbone.subroute":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone.subroute\\backbone.subroute.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\router.js":[function(require,module,exports){
"use strict";

var $ = require('jquery');
var Backbone = require('backbone');
Backbone.$ = $;

// Pages
var AssetPage = require('../app/pages/asset');

var Router = Backbone.Router.extend({
    routes: {
        "asset/*subroute": "asset"
    },

    asset: function() {
        this.renderPage(function() {
            return new AssetPage();
        });
    }
});
},{"../app/pages/asset":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\pages\\asset.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\asset\\edit.js":[function(require,module,exports){
"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

// Data Layer
var AssetVarValueModel = require('../../models/asset_var_value');

// UI Components
var BaseView = require('../../view');
var AssetVarEditRow = require('./edit/asset_var_edit_row');
var SelectAssetVarModal = require('../catalog/select_asset_var_modal');


/**
 *
 */
var EditAsset = BaseView.extend({
    el: ".aui-page-panel-content",

    options: {
        assetId: 0,
        assetVarNames: {}
    },

    events: {
        "click #add_asset_var": "showAssetVarFieldSelectionModal"
    },

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
        
        });
    },

    initialize: function(options) {
        this.options = options || {};

        _.bindAll(this, 'addAssetVarRow');

        this.assetVarFieldsList = this.$('#asset_var_fields');
    },

    postRenderTemplate: function() {},

    getAssetVarSelectionModalView: function() {
        // todo: dispose the existing modal object if exists
        return new SelectAssetVarModal({
            okFunc: this.addAssetVarRow
        });
    },

    /**
     * Displays the asset var modal window.
     */
    showAssetVarFieldSelectionModal: function(e) {
        e.preventDefault();

        // forcefull render the select asset var modal to reset form fields
        this.assetVarSelectionModal = this.getAssetVarSelectionModalView();
        this.$('#asset_var_selection_container').html(this.assetVarSelectionModal.render().el);
        this.assetVarSelectionModal.showModal();
    },

    /**
     * Adds an asset var row based on the select asset var modal result.
     */
    addAssetVarRow: function(assetVar) {
        if (assetVar === null) {
            // todo: throw some kind of validation failure
        }

        this.insertAssetVarValue(this.options.assetId, assetVar.get("Id"));

        var view = new AssetVarEditRow({
            assetVarNames: this.options.assetVarNames,
            assetVar: assetVar
        });

        this.assetVarFieldsList.append(view.render().el);
    },

    /**
     * 
     */
    insertAssetVarValue: function(assetId, assetVarId) {
        var varValue = new AssetVarValueModel({
            AssetId: parseInt(assetId, 10),
            AssetVarId: assetVarId,
            VarValue: "",
            OrganizationId: QuoteFlow.CurrentOrganizationId
        });

        var req = varValue.save({ wait: true });
        req.done(function(result) {
            console.log(result);
        });
    }
});

module.exports = EditAsset;
},{"../../models/asset_var_value":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\models\\asset_var_value.js","../../view":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\view.js","../catalog/select_asset_var_modal":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\catalog\\select_asset_var_modal.js","./edit/asset_var_edit_row":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\asset\\edit\\asset_var_edit_row.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\asset\\edit\\asset_var_edit_row.js":[function(require,module,exports){
"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

var BaseView = require('../../../view');

/**
 *
 */
var AssetVarEditRow = BaseView.extend({
    className: 'field-group',

    templateName: 'asset/edit/asset-var-edit-row',

    options: {
        assetVarNames: {},
        assetVar: null
    },

    events: {
        "click #add_asset_var": "showAssetVarFieldSelectionModal"
    },

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
            assetVar: this.options.assetVar.toJSON(),
            assetVarNames: this.options.assetVarNames,
            buttonOwns: "assetvar_" + this.options.assetVar.get('Id')
        });
    },

    initialize: function(options) {
        this.options = options || {};
    },

    postRenderTemplate: function() {
        _.defer(function() {
            AJS.$('select').auiSelect2();
        });
    },

    /**
     * Removes the asset var row from the table. Disposes the view.
     */
    removeRow: function() {
        this.remove();
    }
});

module.exports = AssetVarEditRow;

},{"../../../view":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\view.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\asset\\show.js":[function(require,module,exports){
"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

var BaseView = require('../../view');

/**
 *
 */
var ShowAsset = BaseView.extend({
    el: ".asset-container",

    options: {},

    events: {
        "click #footer_comment_button": "showCommentModule",
        "click #asset_comment_add_cancel": "hideCommentModule"
    },

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
        
        });
    },

    initialize: function(options) {},

    postRenderTemplate: function() {},

    /**
     * Shows the comment form.
     */
    showCommentModule: function() {
        $('#addcomment').addClass('active');
    },

    /**
     * Hides the comment form.
     */
    hideCommentModule: function() {
        $('#addcomment').removeClass('active');
        $('#Comment').val("");
    }
});

module.exports = ShowAsset;
},{"../../view":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\view.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\ui\\catalog\\select_asset_var_modal.js":[function(require,module,exports){
"use strict";

var $ = require('jquery');
var _ = require('underscore');
var moment = require('moment');
var Backbone = require('backbone');
Backbone.$ = $;

var BaseView = require('../../view');

var AssetVarCollection = require('../../collections/asset_vars');
var AssetVarModel = require('../../models/asset_var');

/**
 *
 */
var SelectAssetVarModal = BaseView.extend({
    templateName: "catalog/select-asset-var-modal",

    options: {
        okFunc: null
    },

    events: {},

    presenter: function() {
        return _.extend(this.defaultPresenter(), {
            assetVars: this.assetVars.toJSON()
        });
    },

    initialize: function(options) {
        // so we can use options throughout each function
        this.options = options || {};

        // required sinced AJS overrides 'this'
        _.bindAll(this, 'submitModal', 'closeModal', 'newAssetVarKeypressHandler', 'disableAssetVarsDropdown', 'createAssetVar', 'getNewAssetVarName', 'getSelectedExistingAssetVar');

        this.assetVars = this.fetchAssetVars();
    },

    postRenderTemplate: function() {
        var self = this;

        _.defer(function() {
            self.modalAJS = AJS.dialog2("#asset_var_selection_modal");
            self.assetVarsDropdown = AJS.$('#select_asset_var').auiSelect2();

            // handle event bindings here since AJS apparently overrides them...
            AJS.$('#new_asset_var').on('keyup', self.newAssetVarKeypressHandler);
            AJS.$('#dialog-close-button').on('click', self.closeModal);
            AJS.$('#dialog-create').on('click', self.submitModal);
        });
    },

    fetchAssetVars: function() {
        var assetVars = new AssetVarCollection();
        assetVars.fetch({
            data: $.param({ id: QuoteFlow.CurrentOrganizationId }),
            async: false
        });

        return assetVars;
    },

    showModal: function() {
        var self = this;
        _.defer(function() {
            self.modalAJS.show();
        });
    },

    closeModal: function() {
        AJS.dialog2("#asset_var_selection_modal").hide();
        this.remove();
        AJS.dialog2("#asset_var_selection_modal").remove();
    },

    submitModal: function(e) {
        var el = $(e.currentTarget);
        el.attr('aria-disabled', "true");

        var assetVar;
        if (this.assetVarsDropdown.prop('disabled')) {
            // user opted to create a new assetvar
            this.createAssetVar(this.getNewAssetVarName());
            this.assetVars = this.fetchAssetVars(); // re-fetch collection to get the id
            assetVar = this.assetVars.at(this.assetVars.length - 1);
        } else {
            // the user has selected an existing assetvar
            var assetVarId = this.getSelectedExistingAssetVar();
            assetVar = this.assetVars.findWhere({ Id: parseInt(assetVarId, 10) });
        }

        this.options.okFunc(assetVar);

        this.closeModal();
    },

    newAssetVarKeypressHandler: function(e) {
        var el = $(e.currentTarget);

        if (el.val() !== "") {
            this.disableAssetVarsDropdown();
        } else {
            this.enableAssetVarsDropdown();
        }
    },

    disableAssetVarsDropdown: function() {
        this.assetVarsDropdown.select2("enable", false);
    },

    enableAssetVarsDropdown: function() {
        this.assetVarsDropdown.select2("enable", true);
    },

    createAssetVar: function(assetVarName) {
        if (assetVarName === "") {
            // todo: return failed validation for empty string
        }

        // does this assetvar exist yet?
        var existing = this.assetVars.findWhere({ Name: assetVarName });
        if (existing != undefined) {
            // todo: return failed validation because assetvar already exists
        }

        var assetVar = new AssetVarModel({
            Name: assetVarName,
            Description: null,
            ValueType: "String",
            OrganizationId: QuoteFlow.CurrentOrganizationId,
            Enabled: true,
            CreatedUtc: moment().format(),
            CreatedBy: QuoteFlow.CurrentUserId
        });

        return this.assetVars.create(assetVar, { wait: true });
    },

    getNewAssetVarName: function() {
        return $('#new_asset_var').val();
    },

    getSelectedExistingAssetVar: function() {
        return this.assetVarsDropdown.val();
    }
});

module.exports = SelectAssetVarModal;

},{"../../collections/asset_vars":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\collections\\asset_vars.js","../../models/asset_var":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\models\\asset_var.js","../../view":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\view.js","backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","moment":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\moment\\moment.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\app\\view.js":[function(require,module,exports){
"use strict";﻿

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

﻿var BaseView = Backbone.View.extend({

    initialize: function (options) {
        this.options = options || {};
        this.setupRenderEvents();
    },

    presenter: function () {
        return this.defaultPresenter();
    },

    setupRenderEvents: function () {
        if (this.model) {
            this.model.bind('remove', this.remove, this);
        }
    },

    // automatically plugs-in the model attributes into the JST, as well as
    // some site-wide attributes that we define below
    defaultPresenter: function () {
        var modelJson = this.model && this.model.attributes ? _.clone(this.model.attributes) : {};

        var imageUrl;
        if (QuoteFlow.RootUrl === "/") {
            imageUrl = "/Content/images/";
        } else {
            imageUrl = QuoteFlow.RootUrl + "Content/images/";
        }

        return _.extend(modelJson, {
            RootUrl: QuoteFlow.RootUrl,
            ImageUrl: imageUrl
        });
    },

    render: function () {
        this.renderTemplate();
        this.renderSubviews();

        return this;
    },

    renderTemplate: function () {
        var presenter = _.isFunction(this.presenter) ? this.presenter() : this.presenter;

        // skip over render phase if templateName is null
        if (_.isNull(this.templateName)) {
            return;
        }

        this.template = JST[this.templateName];
        if (!this.template) {
            console.log(!_.isUndefined(this.templateName) ? ("no template for " + this.templateName) : "no templateName specified");
        }

        this.$el
          .html(this.template(presenter))
          .attr("data-template", _.last(this.templateName.split("/")));
        this.postRenderTemplate();
    },

    postRenderTemplate: $.noop, // hella callbax yo

    renderSubviews: function () {
        var self = this;
        _.each(this.subviews, function (property, selector) {
            var view = _.isFunction(self[property]) ? self[property]() : self[property];
            if (view) {
                if (_.isArray(view)) {
                    // If we pass an array of views into the subviews, append each to the selector.
                    // This should generally only be used when dealing with parent-specific class issues.
                    // For example, if you need to supply a "span*" class on the parent to prevent the
                    // box model from breaking.
                    _.each(view, function (arrayView) {
                        var subView = _.isFunction(arrayView) ? arrayView() : arrayView;
                        if (arrayView) {
                            self.$(selector).append(subView.render().el);
                            subView.delegateEvents();
                        }
                    });
                } else {
                    // drop the view directly into the selector
                    self.$(selector).html(view.render().el);
                    view.delegateEvents();
                }
            }
        });
    },

    remove: function () {
        if (this.subviews) {
            this.removeSubviews();
        }

        // remove this from the debug array if it exists
        if (QuoteFlow.Debug.Views.length > 0) {
            var debugIndex = _.indexOf(QuoteFlow.Debug.Views, this);
            if (debugIndex > -1) {
                QuoteFlow.Debug.Views.splice(debugIndex, 1);
            }
        }

        // completely unbind the view
        this.undelegateEvents();
        this.off(); // Kills off remaining events

        // Remove the view from the DOM
        return Backbone.View.prototype.remove.apply(this, arguments);
    },

    /**
        Removes any subviews associated with this view, which will in-turn remove any
        children of those views, and so on.
    **/
    removeSubviews: function () {
        var self = this,
            children = this.subviews,
            childViews = [];

        if (!children) {
            return this;
        }

        _.each(children, function (property, selector) {
            var view = _.isFunction(self[property]) ? self[property]() : self[property];
            if (view) {
                if (_.isArray(view)) {
                    // ensure that subview arrays are also properly disposed of
                    _.each(view, function (arrayView) {
                        var subView = _.isFunction(arrayView) ? arrayView() : arrayView;
                        if (arrayView) {
                            childViews.push(subView);
                        }
                    });
                } else {
                    childViews.push(view);
                }
            }
        });


        _(childViews).invoke("remove");

        this.subviews = {};
        return this;
    }
});

module.exports = BaseView;

},{"backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","jquery":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\lib\\aui\\aui.js":[function(require,module,exports){
jQuery.noConflict(),function(a){function b(){if(a.fn.ajaxSubmit.debug){var b="[jquery.form] "+Array.prototype.join.call(arguments,"");window.console&&window.console.log?window.console.log(b):window.opera&&window.opera.postError&&window.opera.postError(b)}}a.fn.ajaxSubmit=function(c){function d(){function d(){var b=m.attr("target"),c=m.attr("action");f.setAttribute("target",h),"POST"!=f.getAttribute("method")&&f.setAttribute("method","POST"),f.getAttribute("action")!=g.url&&f.setAttribute("action",g.url),g.skipEncodingOverride||m.attr({encoding:"multipart/form-data",enctype:"multipart/form-data"}),g.timeout&&setTimeout(function(){n=!0,e()},g.timeout);var d=[];try{if(g.extraData)for(var k in g.extraData)d.push(a('<input type="hidden" name="'+k+'" value="'+g.extraData[k]+'" />').appendTo(f)[0]);i.appendTo("body"),j.attachEvent?j.attachEvent("onload",e):j.addEventListener("load",e,!1),f.submit()}finally{f.setAttribute("action",c),b?f.setAttribute("target",b):m.removeAttr("target"),a(d).remove()}}function e(){if(!k.aborted){var c=j.contentWindow?j.contentWindow.document:j.contentDocument?j.contentDocument:j.document;if(c&&c.location.href!=g.iframeSrc){j.detachEvent?j.detachEvent("onload",e):j.removeEventListener("load",e,!1);var d=!0;try{if(n)throw"timeout";var f="xml"==g.dataType||c.XMLDocument||a.isXMLDoc(c);if(b("isXml="+f),!f&&window.opera&&(null==c.body||""==c.body.innerHTML)&&--r)return b("requeing onLoad callback, DOM not available"),void setTimeout(e,250);k.responseText=c.body?c.body.innerHTML:c.documentElement?c.documentElement.innerHTML:null,k.responseXML=c.XMLDocument?c.XMLDocument:c,k.getResponseHeader=function(a){var b={"content-type":g.dataType};return b[a]};var h=/(json|script)/.test(g.dataType);if(h||g.textarea){var m=c.getElementsByTagName("textarea")[0];if(m)k.responseText=m.value;else if(h){var o=c.getElementsByTagName("pre")[0],p=c.getElementsByTagName("body")[0];o?k.responseText=o.textContent:p&&(k.responseText=p.innerHTML)}}else"xml"!=g.dataType||k.responseXML||null==k.responseText||(k.responseXML=s(k.responseText));q=u(k,g.dataType,g)}catch(t){b("error caught:",t),d=!1,k.error=t,g.error&&g.error.call(g.context,k,"error",t),l&&a.event.trigger("ajaxError",[k,g,t])}k.aborted&&(b("upload aborted"),d=!1),d&&(g.success&&g.success.call(g.context,q,"success",k),l&&a.event.trigger("ajaxSuccess",[k,g])),l&&a.event.trigger("ajaxComplete",[k,g]),l&&!--a.active&&a.event.trigger("ajaxStop"),g.complete&&g.complete.call(g.context,k,d?"success":"error"),setTimeout(function(){i.removeData("form-plugin-onload"),i.remove(),k.responseXML=null},100)}}}var f=m[0];if(a(":input[name=submit],:input[id=submit]",f).length)return void alert('Error: Form elements must not have name or id of "submit".');var g=a.extend(!0,{},a.ajaxSettings,c);g.context=g.context||g;var h="jqFormIO"+(new Date).getTime(),i=a('<iframe id="'+h+'" name="'+h+'" src="'+g.iframeSrc+'" />'),j=i[0];i.css({position:"absolute",top:"-1000px",left:"-1000px"});var k={aborted:0,responseText:null,responseXML:null,status:0,statusText:"n/a",getAllResponseHeaders:function(){},getResponseHeader:function(){},setRequestHeader:function(){},abort:function(){b("aborting upload...");var c="aborted";this.aborted=1,i.attr("src",g.iframeSrc),k.error=c,g.error&&g.error.call(g.context,k,"error",c),l&&a.event.trigger("ajaxError",[k,g,c]),g.complete&&g.complete.call(g.context,k,"error")}},l=g.global;if(l&&!a.active++&&a.event.trigger("ajaxStart"),l&&a.event.trigger("ajaxSend",[k,g]),g.beforeSend&&g.beforeSend.call(g.context,k,g)===!1)return void(g.global&&a.active--);if(!k.aborted){var n=0,o=f.clk;if(o){var p=o.name;p&&!o.disabled&&(g.extraData=g.extraData||{},g.extraData[p]=o.value,"image"==o.type&&(g.extraData[p+".x"]=f.clk_x,g.extraData[p+".y"]=f.clk_y))}g.forceSync?d():setTimeout(d,10);var q,r=50,s=a.parseXML||function(a,b){return window.ActiveXObject?(b=new ActiveXObject("Microsoft.XMLDOM"),b.async="false",b.loadXML(a)):b=(new DOMParser).parseFromString(a,"text/xml"),b&&b.documentElement&&"parsererror"!=b.documentElement.nodeName?b:null},t=a.parseJSON||function(a){return window.eval("("+a+")")},u=function(b,c,d){var e=b.getResponseHeader("content-type")||"",f="xml"===c||!c&&e.indexOf("xml")>=0,g=f?b.responseXML:b.responseText;return f&&"parsererror"===g.documentElement.nodeName&&a.error&&a.error("parsererror"),d&&d.dataFilter&&(g=d.dataFilter(g,c)),"string"==typeof g&&("json"===c||!c&&e.indexOf("json")>=0?g=t(g):("script"===c||!c&&e.indexOf("javascript")>=0)&&a.globalEval(g)),g}}}if(!this.length)return b("ajaxSubmit: skipping submit process - no element selected"),this;"function"==typeof c&&(c={success:c});var e=this.attr("action"),f="string"==typeof e?a.trim(e):"";f&&(f=(f.match(/^([^#]+)/)||[])[1]),f=f||window.location.href||"",c=a.extend(!0,{url:f,type:this[0].getAttribute("method")||"GET",iframeSrc:/^https/i.test(window.location.href||"")?"javascript:false":"about:blank"},c);var g={};if(this.trigger("form-pre-serialize",[this,c,g]),g.veto)return b("ajaxSubmit: submit vetoed via form-pre-serialize trigger"),this;if(c.beforeSerialize&&c.beforeSerialize(this,c)===!1)return b("ajaxSubmit: submit aborted via beforeSerialize callback"),this;var h,i,j=this.formToArray(c.semantic);if(c.data){c.extraData=c.data;for(h in c.data)if(c.data[h]instanceof Array)for(var k in c.data[h])j.push({name:h,value:c.data[h][k]});else i=c.data[h],i=a.isFunction(i)?i():i,j.push({name:h,value:i})}if(c.beforeSubmit&&c.beforeSubmit(j,this,c)===!1)return b("ajaxSubmit: submit aborted via beforeSubmit callback"),this;if(this.trigger("form-submit-validate",[j,this,c,g]),g.veto)return b("ajaxSubmit: submit vetoed via form-submit-validate trigger"),this;var l=a.param(j);"GET"==c.type.toUpperCase()?(c.url+=(c.url.indexOf("?")>=0?"&":"?")+l,c.data=null):c.data=l;var m=this,n=[];if(c.resetForm&&n.push(function(){m.resetForm()}),c.clearForm&&n.push(function(){m.clearForm()}),!c.dataType&&c.target){var o=c.success||function(){};n.push(function(b){var d=c.replaceTarget?"replaceWith":"html";a(c.target)[d](b).each(o,arguments)})}else c.success&&n.push(c.success);c.success=function(a,b,d){for(var e=c.context||c,f=0,g=n.length;g>f;f++)n[f].apply(e,[a,b,d||m,m])};var p=a("input:file",this).length>0,q="multipart/form-data",r=m.attr("enctype")==q||m.attr("encoding")==q;return c.iframe!==!1&&(p||c.iframe||r)?c.closeKeepAlive?a.get(c.closeKeepAlive,d):d():a.ajax(c),this.trigger("form-submit-notify",[this,c]),this},a.fn.ajaxForm=function(c){if(0===this.length){var d={s:this.selector,c:this.context};return!a.isReady&&d.s?(b("DOM not ready, queuing ajaxForm"),a(function(){a(d.s,d.c).ajaxForm(c)}),this):(b("terminating; zero elements found by selector"+(a.isReady?"":" (DOM not ready)")),this)}return this.ajaxFormUnbind().bind("submit.form-plugin",function(b){b.isDefaultPrevented()||(b.preventDefault(),a(this).ajaxSubmit(c))}).bind("click.form-plugin",function(b){var c=b.target,d=a(c);if(!d.is(":submit,input:image")){var e=d.closest(":submit");if(0==e.length)return;c=e[0]}var f=this;if(f.clk=c,"image"==c.type)if(void 0!=b.offsetX)f.clk_x=b.offsetX,f.clk_y=b.offsetY;else if("function"==typeof a.fn.offset){var g=d.offset();f.clk_x=b.pageX-g.left,f.clk_y=b.pageY-g.top}else f.clk_x=b.pageX-c.offsetLeft,f.clk_y=b.pageY-c.offsetTop;setTimeout(function(){f.clk=f.clk_x=f.clk_y=null},100)})},a.fn.ajaxFormUnbind=function(){return this.unbind("submit.form-plugin click.form-plugin")},a.fn.formToArray=function(b){var c=[];if(0===this.length)return c;var d=this[0],e=b?d.getElementsByTagName("*"):d.elements;if(!e)return c;var f,g,h,i,j,k,l;for(f=0,k=e.length;k>f;f++)if(j=e[f],h=j.name)if(b&&d.clk&&"image"==j.type)j.disabled||d.clk!=j||(c.push({name:h,value:a(j).val()}),c.push({name:h+".x",value:d.clk_x},{name:h+".y",value:d.clk_y}));else if(i=a.fieldValue(j,!0),i&&i.constructor==Array)for(g=0,l=i.length;l>g;g++)c.push({name:h,value:i[g]});else null!==i&&"undefined"!=typeof i&&c.push({name:h,value:i});if(!b&&d.clk){var m=a(d.clk),n=m[0];h=n.name,h&&!n.disabled&&"image"==n.type&&(c.push({name:h,value:m.val()}),c.push({name:h+".x",value:d.clk_x},{name:h+".y",value:d.clk_y}))}return c},a.fn.formSerialize=function(b){return a.param(this.formToArray(b))},a.fn.fieldSerialize=function(b){var c=[];return this.each(function(){var d=this.name;if(d){var e=a.fieldValue(this,b);if(e&&e.constructor==Array)for(var f=0,g=e.length;g>f;f++)c.push({name:d,value:e[f]});else null!==e&&"undefined"!=typeof e&&c.push({name:this.name,value:e})}}),a.param(c)},a.fn.fieldValue=function(b){for(var c=[],d=0,e=this.length;e>d;d++){var f=this[d],g=a.fieldValue(f,b);null===g||"undefined"==typeof g||g.constructor==Array&&!g.length||(g.constructor==Array?a.merge(c,g):c.push(g))}return c},a.fieldValue=function(b,c){var d=b.name,e=b.type,f=b.tagName.toLowerCase();if(void 0===c&&(c=!0),c&&(!d||b.disabled||"reset"==e||"button"==e||("checkbox"==e||"radio"==e)&&!b.checked||("submit"==e||"image"==e)&&b.form&&b.form.clk!=b||"select"==f&&-1==b.selectedIndex))return null;if("select"==f){var g=b.selectedIndex;if(0>g)return null;for(var h=[],i=b.options,j="select-one"==e,k=j?g+1:i.length,l=j?g:0;k>l;l++){var m=i[l];if(m.selected){var n=m.value;if(n||(n=m.attributes&&m.attributes.value&&!m.attributes.value.specified?m.text:m.value),j)return n;h.push(n)}}return h}return a(b).val()},a.fn.clearForm=function(){return this.each(function(){a("input,select,textarea",this).clearFields()})},a.fn.clearFields=a.fn.clearInputs=function(){return this.each(function(){var a=this.type,b=this.tagName.toLowerCase();"text"==a||"password"==a||"textarea"==b?this.value="":"checkbox"==a||"radio"==a?this.checked=!1:"select"==b&&(this.selectedIndex=-1)})},a.fn.resetForm=function(){return this.each(function(){("function"==typeof this.reset||"object"==typeof this.reset&&!this.reset.nodeType)&&this.reset()})},a.fn.enable=function(a){return void 0===a&&(a=!0),this.each(function(){this.disabled=!a})},a.fn.selected=function(b){return void 0===b&&(b=!0),this.each(function(){var c=this.type;if("checkbox"==c||"radio"==c)this.checked=b;else if("option"==this.tagName.toLowerCase()){var d=a(this).parent("select");b&&d[0]&&"select-one"==d[0].type&&d.find("option").selected(!1),this.selected=b}})}}(jQuery),function(){var _after=1,_afterThrow=2,_afterFinally=3,_before=4,_around=5,_intro=6,_regexEnabled=!0,_arguments="arguments",_undef="undefined",getType=function(){for(var a=Object.prototype.toString,b={},c={1:"element",3:"textnode",9:"document",11:"fragment"},d="Arguments Array Boolean Date Document Element Error Fragment Function NodeList Null Number Object RegExp String TextNode Undefined Window".split(" "),e=d.length;e--;){var f=d[e],g=window[f];if(g)try{b[a.call(new g)]=f.toLowerCase()}catch(h){}}return function(d){return null==d&&(void 0===d?_undef:"null")||d.nodeType&&c[d.nodeType]||"number"==typeof d.length&&(d.callee&&_arguments||d.alert&&"window"||d.item&&"nodelist")||b[a.call(d)]}}(),isFunc=function(a){return"function"==getType(a)},weaveOne=function(source,method,advice){var old=source[method];if(advice.type!=_intro&&!isFunc(old)){var oldObject=old;old=function(){for(var code=arguments.length>0?_arguments+"[0]":"",i=1;i<arguments.length;i++)code+=","+_arguments+"["+i+"]";return eval("oldObject("+code+");")}}var aspect;return advice.type==_after||advice.type==_afterThrow||advice.type==_afterFinally?aspect=function(){var a,b=null;try{a=old.apply(this,arguments)}catch(c){b=c}if(advice.type==_after){if(null!=b)throw b;a=advice.value.apply(this,[a,method])}else advice.type==_afterThrow&&null!=b?a=advice.value.apply(this,[b,method]):advice.type==_afterFinally&&(a=advice.value.apply(this,[a,b,method]));return a}:advice.type==_before?aspect=function(){return advice.value.apply(this,[arguments,method]),old.apply(this,arguments)}:advice.type==_intro?aspect=function(){return advice.value.apply(this,arguments)}:advice.type==_around&&(aspect=function(){var a={object:this,args:Array.prototype.slice.call(arguments)};return advice.value.apply(a.object,[{arguments:a.args,method:method,proceed:function(){return old.apply(a.object,a.args)}}])}),aspect.unweave=function(){source[method]=old,pointcut=source=aspect=old=null},source[method]=aspect,aspect},search=function(a,b,c){var d=[];for(var e in a){var f=null;try{f=a[e]}catch(g){}null!=f&&e.match(b.method)&&isFunc(f)&&(d[d.length]={source:a,method:e,advice:c})}return d},weave=function(a,b){var c=typeof a.target.prototype!=_undef?a.target.prototype:a.target,d=[];if(b.type!=_intro&&typeof c[a.method]==_undef){var e=search(a.target,a,b);0==e.length&&(e=search(c,a,b));for(var f in e)d[d.length]=weaveOne(e[f].source,e[f].method,e[f].advice)}else d[0]=weaveOne(c,a.method,b);return _regexEnabled?d:d[0]};jQuery.aop={after:function(a,b){return weave(a,{type:_after,value:b})},afterThrow:function(a,b){return weave(a,{type:_afterThrow,value:b})},afterFinally:function(a,b){return weave(a,{type:_afterFinally,value:b})},before:function(a,b){return weave(a,{type:_before,value:b})},around:function(a,b){return weave(a,{type:_around,value:b})},introduction:function(a,b){return weave(a,{type:_intro,value:b})},setup:function(a){_regexEnabled=a.regexMatch}}}(),window.Raphael&&(Raphael.shadow=function(a,b,c,d,e){e=e||{};var f,g,h,i=jQuery(e.target),j=jQuery("<div/>",{"class":"aui-shadow"}),k=e.shadow||e.color||"#000",l=10*e.size||0,m=e.offsetSize||3,n=e.zindex||0,o=e.radius||0,p="0.4",q=e.blur||3;return c+=l+2*q,d+=l+2*q,Raphael.shadow.BOX_SHADOW_SUPPORT?(i.addClass("aui-box-shadow"),j.addClass("hidden")):(0===a&&0===b&&i.length>0&&(h=i.offset(),a=m-q+h.left,b=m-q+h.top),jQuery.browser.msie&&jQuery.browser.version<"9"&&(k="#f0f0f0",p="0.2"),j.css({position:"absolute",left:a,top:b,width:c,height:d,zIndex:n}),i.length>0?(j.appendTo(document.body),f=Raphael(j[0],c,d,o)):f=Raphael(a,b,c,d,o),f.canvas.style.position="absolute",g=f.rect(q,q,c-2*q,d-2*q).attr({fill:k,stroke:k,blur:""+q,opacity:p}),j)},Raphael.shadow.BOX_SHADOW_SUPPORT=function(){for(var a=document.documentElement.style,b=["boxShadow","MozBoxShadow","WebkitBoxShadow","msBoxShadow"],c=0;c<b.length;c++)if(b[c]in a)return!0;return!1}()),jQuery.os={};var jQueryOSplatform=navigator.platform.toLowerCase();jQuery.os.windows=-1!=jQueryOSplatform.indexOf("win"),jQuery.os.mac=-1!=jQueryOSplatform.indexOf("mac"),jQuery.os.linux=-1!=jQueryOSplatform.indexOf("linux"),function(a){function b(a){this.num=0,this.timer=a>0?a:!1}function c(c){if(a.isPlainObject(c.data)||a.isArray(c.data)||"string"==typeof c.data){var e=c.handler,f={timer:700};!function(b){"string"==typeof b?f.combo=[b]:a.isArray(b)?f.combo=b:a.extend(f,b),f.combo=a.map(f.combo,function(a){return a.toLowerCase()})}(c.data),c.index=new b(f.timer),c.handler=function(b){if(this===b.target||!/textarea|select|input/i.test(b.target.nodeName)){var g="keypress"!==b.type?a.hotkeys.specialKeys[b.which]:null,h=String.fromCharCode(b.which).toLowerCase(),i="",j={};b.altKey&&"alt"!==g&&(i+="alt+"),b.ctrlKey&&"ctrl"!==g&&(i+="ctrl+"),b.metaKey&&!b.ctrlKey&&"meta"!==g&&(i+="meta+"),b.shiftKey&&"shift"!==g&&(i+="shift+"),b.metaKey&&"["===h&&(h=null),g&&(j[i+g]=!0),h&&(j[i+h]=!0),/shift+/.test(i)&&(j[i.replace("shift+","")+a.hotkeys.shiftNums[g||h]]=!0);var k=c.index,l=f.combo;if(d(l[k.val()],j)){if(k.val()===l.length-1)return k.reset(),e.apply(this,arguments);k.inc()}else k.reset(),d(l[0],j)&&k.inc()}}}}function d(a,b){for(var c=a.split(" "),d=0,e=c.length;e>d;d++)if(b[c[d]])return!0;return!1}a.hotkeys={version:"0.8",specialKeys:{8:"backspace",9:"tab",13:"return",16:"shift",17:"ctrl",18:"alt",19:"pause",20:"capslock",27:"esc",32:"space",33:"pageup",34:"pagedown",35:"end",36:"home",37:"left",38:"up",39:"right",40:"down",45:"insert",46:"del",91:"meta",96:"0",97:"1",98:"2",99:"3",100:"4",101:"5",102:"6",103:"7",104:"8",105:"9",106:"*",107:"+",109:"-",110:".",111:"/",112:"f1",113:"f2",114:"f3",115:"f4",116:"f5",117:"f6",118:"f7",119:"f8",120:"f9",121:"f10",122:"f11",123:"f12",144:"numlock",145:"scroll",188:",",190:".",191:"/",224:"meta",219:"[",221:"]"},keypressKeys:["<",">","?"],shiftNums:{"`":"~",1:"!",2:"@",3:"#",4:"$",5:"%",6:"^",7:"&",8:"*",9:"(",0:")","-":"_","=":"+",";":":","'":'"',",":"<",".":">","/":"?","\\":"|"}},a.each(a.hotkeys.keypressKeys,function(b,c){a.hotkeys.shiftNums[c]=c}),b.prototype.val=function(){return this.num},b.prototype.inc=function(){this.timer&&(clearTimeout(this.timeout),this.timeout=setTimeout(a.proxy(b.prototype.reset,this),this.timer)),this.num++},b.prototype.reset=function(){this.timer&&clearTimeout(this.timeout),this.num=0},a.each(["keydown","keyup","keypress"],function(){a.event.special[this]={add:c}})}(jQuery),jQuery.fn.moveTo=function(a){var b,c={transition:!1,scrollOffset:35},d=jQuery.extend(c,a),e=this,f=e.offset().top;if((jQuery(window).scrollTop()+jQuery(window).height()-this.outerHeight()<f||jQuery(window).scrollTop()+d.scrollOffset>f)&&jQuery(window).height()>d.scrollOffset){if(b=jQuery(window).scrollTop()+d.scrollOffset>f?f-(jQuery(window).height()-this.outerHeight())+d.scrollOffset:f-d.scrollOffset,!jQuery.fn.moveTo.animating&&d.transition)return jQuery(document).trigger("moveToStarted",this),jQuery.fn.moveTo.animating=!0,jQuery("html,body").animate({scrollTop:b},1e3,function(){jQuery(document).trigger("moveToFinished",e),delete jQuery.fn.moveTo.animating}),this;var g=jQuery("html, body");return g.is(":animated")&&(g.stop(),delete jQuery.fn.moveTo.animating),jQuery(document).trigger("moveToStarted"),jQuery(window).scrollTop(b),setTimeout(function(){jQuery(document).trigger("moveToFinished",e)},100),this}return jQuery(document).trigger("moveToFinished",this),this},function($,undefined){function Datepicker(){this.debug=!1,this._curInst=null,this._keyEvent=!1,this._disabledInputs=[],this._datepickerShowing=!1,this._inDialog=!1,this._mainDivId="ui-datepicker-div",this._inlineClass="ui-datepicker-inline",this._appendClass="ui-datepicker-append",this._triggerClass="ui-datepicker-trigger",this._dialogClass="ui-datepicker-dialog",this._disableClass="ui-datepicker-disabled",this._unselectableClass="ui-datepicker-unselectable",this._currentClass="ui-datepicker-current-day",this._dayOverClass="ui-datepicker-days-cell-over",this.regional=[],this.regional[""]={closeText:"Done",prevText:"Prev",nextText:"Next",currentText:"Today",monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],monthNamesShort:["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesShort:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],dayNamesMin:["Su","Mo","Tu","We","Th","Fr","Sa"],weekHeader:"Wk",dateFormat:"mm/dd/yy",firstDay:0,isRTL:!1,showMonthAfterYear:!1,yearSuffix:""},this._defaults={showOn:"focus",showAnim:"fadeIn",showOptions:{},defaultDate:null,appendText:"",buttonText:"...",buttonImage:"",buttonImageOnly:!1,hideIfNoPrevNext:!1,navigationAsDateFormat:!1,gotoCurrent:!1,changeMonth:!1,changeYear:!1,yearRange:"c-10:c+10",showOtherMonths:!1,selectOtherMonths:!1,showWeek:!1,calculateWeek:this.iso8601Week,shortYearCutoff:"+10",minDate:null,maxDate:null,duration:"fast",beforeShowDay:null,beforeShow:null,onSelect:null,onChangeMonthYear:null,onClose:null,numberOfMonths:1,showCurrentAtPos:0,stepMonths:1,stepBigMonths:12,altField:"",altFormat:"",constrainInput:!0,showButtonPanel:!1,autoSize:!1,disabled:!1},$.extend(this._defaults,this.regional[""]),this.dpDiv=bindHover($('<div id="'+this._mainDivId+'" class="ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"></div>'))}function bindHover(a){var b="button, .ui-datepicker-prev, .ui-datepicker-next, .ui-datepicker-calendar td a";return a.bind("mouseout",function(a){var c=$(a.target).closest(b);c.length&&c.removeClass("ui-state-hover ui-datepicker-prev-hover ui-datepicker-next-hover")}).bind("mouseover",function(c){var d=$(c.target).closest(b);!$.datepicker._isDisabledDatepicker(instActive.inline?a.parent()[0]:instActive.input[0])&&d.length&&(d.parents(".ui-datepicker-calendar").find("a").removeClass("ui-state-hover"),d.addClass("ui-state-hover"),d.hasClass("ui-datepicker-prev")&&d.addClass("ui-datepicker-prev-hover"),d.hasClass("ui-datepicker-next")&&d.addClass("ui-datepicker-next-hover"))})}function extendRemove(a,b){$.extend(a,b);for(var c in b)(null==b[c]||b[c]==undefined)&&(a[c]=b[c]);return a}function isArray(a){return a&&($.browser.safari&&"object"==typeof a&&a.length||a.constructor&&a.constructor.toString().match(/\Array\(\)/))}$.extend($.ui,{datepicker:{version:"1.8.24"}});var PROP_NAME="datepicker",dpuuid=(new Date).getTime(),instActive;$.extend(Datepicker.prototype,{markerClassName:"hasDatepicker",maxRows:4,log:function(){this.debug&&console.log.apply("",arguments)},_widgetDatepicker:function(){return this.dpDiv},setDefaults:function(a){return extendRemove(this._defaults,a||{}),this},_attachDatepicker:function(target,settings){var inlineSettings=null;for(var attrName in this._defaults){var attrValue=target.getAttribute("date:"+attrName);if(attrValue){inlineSettings=inlineSettings||{};try{inlineSettings[attrName]=eval(attrValue)}catch(err){inlineSettings[attrName]=attrValue}}}var nodeName=target.nodeName.toLowerCase(),inline="div"==nodeName||"span"==nodeName;target.id||(this.uuid+=1,target.id="dp"+this.uuid);var inst=this._newInst($(target),inline);inst.settings=$.extend({},settings||{},inlineSettings||{}),"input"==nodeName?this._connectDatepicker(target,inst):inline&&this._inlineDatepicker(target,inst)},_newInst:function(a,b){var c=a[0].id.replace(/([^A-Za-z0-9_-])/g,"\\\\$1");return{id:c,input:a,selectedDay:0,selectedMonth:0,selectedYear:0,drawMonth:0,drawYear:0,inline:b,dpDiv:b?bindHover($('<div class="'+this._inlineClass+' ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"></div>')):this.dpDiv}},_connectDatepicker:function(a,b){var c=$(a);b.append=$([]),b.trigger=$([]),c.hasClass(this.markerClassName)||(this._attachments(c,b),c.addClass(this.markerClassName).keydown(this._doKeyDown).keypress(this._doKeyPress).keyup(this._doKeyUp).bind("setData.datepicker",function(a,c,d){b.settings[c]=d}).bind("getData.datepicker",function(a,c){return this._get(b,c)}),this._autoSize(b),$.data(a,PROP_NAME,b),b.settings.disabled&&this._disableDatepicker(a))},_attachments:function(a,b){var c=this._get(b,"appendText"),d=this._get(b,"isRTL");b.append&&b.append.remove(),c&&(b.append=$('<span class="'+this._appendClass+'">'+c+"</span>"),a[d?"before":"after"](b.append)),a.unbind("focus",this._showDatepicker),b.trigger&&b.trigger.remove();var e=this._get(b,"showOn");if(("focus"==e||"both"==e)&&a.focus(this._showDatepicker),"button"==e||"both"==e){var f=this._get(b,"buttonText"),g=this._get(b,"buttonImage");b.trigger=$(this._get(b,"buttonImageOnly")?$("<img/>").addClass(this._triggerClass).attr({src:g,alt:f,title:f}):$('<button type="button"></button>').addClass(this._triggerClass).html(""==g?f:$("<img/>").attr({src:g,alt:f,title:f}))),a[d?"before":"after"](b.trigger),b.trigger.click(function(){return $.datepicker._datepickerShowing&&$.datepicker._lastInput==a[0]?$.datepicker._hideDatepicker():$.datepicker._datepickerShowing&&$.datepicker._lastInput!=a[0]?($.datepicker._hideDatepicker(),$.datepicker._showDatepicker(a[0])):$.datepicker._showDatepicker(a[0]),!1})}},_autoSize:function(a){if(this._get(a,"autoSize")&&!a.inline){var b=new Date(2009,11,20),c=this._get(a,"dateFormat");if(c.match(/[DM]/)){var d=function(a){for(var b=0,c=0,d=0;d<a.length;d++)a[d].length>b&&(b=a[d].length,c=d);return c};b.setMonth(d(this._get(a,c.match(/MM/)?"monthNames":"monthNamesShort"))),b.setDate(d(this._get(a,c.match(/DD/)?"dayNames":"dayNamesShort"))+20-b.getDay())}a.input.attr("size",this._formatDate(a,b).length)}},_inlineDatepicker:function(a,b){var c=$(a);c.hasClass(this.markerClassName)||(c.addClass(this.markerClassName).append(b.dpDiv).bind("setData.datepicker",function(a,c,d){b.settings[c]=d}).bind("getData.datepicker",function(a,c){return this._get(b,c)}),$.data(a,PROP_NAME,b),this._setDate(b,this._getDefaultDate(b),!0),this._updateDatepicker(b),this._updateAlternate(b),b.settings.disabled&&this._disableDatepicker(a),b.dpDiv.css("display","block"))},_dialogDatepicker:function(a,b,c,d,e){var f=this._dialogInst;if(!f){this.uuid+=1;var g="dp"+this.uuid;this._dialogInput=$('<input type="text" id="'+g+'" style="position: absolute; top: -100px; width: 0px;"/>'),this._dialogInput.keydown(this._doKeyDown),$("body").append(this._dialogInput),f=this._dialogInst=this._newInst(this._dialogInput,!1),f.settings={},$.data(this._dialogInput[0],PROP_NAME,f)}if(extendRemove(f.settings,d||{}),b=b&&b.constructor==Date?this._formatDate(f,b):b,this._dialogInput.val(b),this._pos=e?e.length?e:[e.pageX,e.pageY]:null,!this._pos){var h=document.documentElement.clientWidth,i=document.documentElement.clientHeight,j=document.documentElement.scrollLeft||document.body.scrollLeft,k=document.documentElement.scrollTop||document.body.scrollTop;this._pos=[h/2-100+j,i/2-150+k]}return this._dialogInput.css("left",this._pos[0]+20+"px").css("top",this._pos[1]+"px"),f.settings.onSelect=c,this._inDialog=!0,this.dpDiv.addClass(this._dialogClass),this._showDatepicker(this._dialogInput[0]),$.blockUI&&$.blockUI(this.dpDiv),$.data(this._dialogInput[0],PROP_NAME,f),this},_destroyDatepicker:function(a){var b=$(a),c=$.data(a,PROP_NAME);if(b.hasClass(this.markerClassName)){var d=a.nodeName.toLowerCase();$.removeData(a,PROP_NAME),"input"==d?(c.append.remove(),c.trigger.remove(),b.removeClass(this.markerClassName).unbind("focus",this._showDatepicker).unbind("keydown",this._doKeyDown).unbind("keypress",this._doKeyPress).unbind("keyup",this._doKeyUp)):("div"==d||"span"==d)&&b.removeClass(this.markerClassName).empty()}},_enableDatepicker:function(a){var b=$(a),c=$.data(a,PROP_NAME);if(b.hasClass(this.markerClassName)){var d=a.nodeName.toLowerCase();if("input"==d)a.disabled=!1,c.trigger.filter("button").each(function(){this.disabled=!1}).end().filter("img").css({opacity:"1.0",cursor:""});else if("div"==d||"span"==d){var e=b.children("."+this._inlineClass);e.children().removeClass("ui-state-disabled"),e.find("select.ui-datepicker-month, select.ui-datepicker-year").removeAttr("disabled")}this._disabledInputs=$.map(this._disabledInputs,function(b){return b==a?null:b})}},_disableDatepicker:function(a){var b=$(a),c=$.data(a,PROP_NAME);if(b.hasClass(this.markerClassName)){var d=a.nodeName.toLowerCase();if("input"==d)a.disabled=!0,c.trigger.filter("button").each(function(){this.disabled=!0}).end().filter("img").css({opacity:"0.5",cursor:"default"});else if("div"==d||"span"==d){var e=b.children("."+this._inlineClass);e.children().addClass("ui-state-disabled"),e.find("select.ui-datepicker-month, select.ui-datepicker-year").attr("disabled","disabled")}this._disabledInputs=$.map(this._disabledInputs,function(b){return b==a?null:b}),this._disabledInputs[this._disabledInputs.length]=a}},_isDisabledDatepicker:function(a){if(!a)return!1;for(var b=0;b<this._disabledInputs.length;b++)if(this._disabledInputs[b]==a)return!0;return!1},_getInst:function(a){try{return $.data(a,PROP_NAME)}catch(b){throw"Missing instance data for this datepicker"}},_optionDatepicker:function(a,b,c){var d=this._getInst(a);if(2==arguments.length&&"string"==typeof b)return"defaults"==b?$.extend({},$.datepicker._defaults):d?"all"==b?$.extend({},d.settings):this._get(d,b):null;var e=b||{};if("string"==typeof b&&(e={},e[b]=c),d){this._curInst==d&&this._hideDatepicker();var f=this._getDateDatepicker(a,!0),g=this._getMinMaxDate(d,"min"),h=this._getMinMaxDate(d,"max");extendRemove(d.settings,e),null!==g&&e.dateFormat!==undefined&&e.minDate===undefined&&(d.settings.minDate=this._formatDate(d,g)),null!==h&&e.dateFormat!==undefined&&e.maxDate===undefined&&(d.settings.maxDate=this._formatDate(d,h)),this._attachments($(a),d),this._autoSize(d),this._setDate(d,f),this._updateAlternate(d),this._updateDatepicker(d)}},_changeDatepicker:function(a,b,c){this._optionDatepicker(a,b,c)},_refreshDatepicker:function(a){var b=this._getInst(a);b&&this._updateDatepicker(b)},_setDateDatepicker:function(a,b){var c=this._getInst(a);c&&(this._setDate(c,b),this._updateDatepicker(c),this._updateAlternate(c))},_getDateDatepicker:function(a,b){var c=this._getInst(a);return c&&!c.inline&&this._setDateFromField(c,b),c?this._getDate(c):null},_doKeyDown:function(a){var b=$.datepicker._getInst(a.target),c=!0,d=b.dpDiv.is(".ui-datepicker-rtl");if(b._keyEvent=!0,$.datepicker._datepickerShowing)switch(a.keyCode){case 9:$.datepicker._hideDatepicker(),c=!1;break;case 13:var e=$("td."+$.datepicker._dayOverClass+":not(."+$.datepicker._currentClass+")",b.dpDiv);e[0]&&$.datepicker._selectDay(a.target,b.selectedMonth,b.selectedYear,e[0]);var f=$.datepicker._get(b,"onSelect");if(f){var g=$.datepicker._formatDate(b);f.apply(b.input?b.input[0]:null,[g,b])}else $.datepicker._hideDatepicker();return!1;case 27:$.datepicker._hideDatepicker();break;case 33:$.datepicker._adjustDate(a.target,a.ctrlKey?-$.datepicker._get(b,"stepBigMonths"):-$.datepicker._get(b,"stepMonths"),"M");break;case 34:$.datepicker._adjustDate(a.target,a.ctrlKey?+$.datepicker._get(b,"stepBigMonths"):+$.datepicker._get(b,"stepMonths"),"M");break;case 35:(a.ctrlKey||a.metaKey)&&$.datepicker._clearDate(a.target),c=a.ctrlKey||a.metaKey;break;case 36:(a.ctrlKey||a.metaKey)&&$.datepicker._gotoToday(a.target),c=a.ctrlKey||a.metaKey;break;case 37:(a.ctrlKey||a.metaKey)&&$.datepicker._adjustDate(a.target,d?1:-1,"D"),c=a.ctrlKey||a.metaKey,a.originalEvent.altKey&&$.datepicker._adjustDate(a.target,a.ctrlKey?-$.datepicker._get(b,"stepBigMonths"):-$.datepicker._get(b,"stepMonths"),"M");break;case 38:(a.ctrlKey||a.metaKey)&&$.datepicker._adjustDate(a.target,-7,"D"),c=a.ctrlKey||a.metaKey;break;case 39:(a.ctrlKey||a.metaKey)&&$.datepicker._adjustDate(a.target,d?-1:1,"D"),c=a.ctrlKey||a.metaKey,a.originalEvent.altKey&&$.datepicker._adjustDate(a.target,a.ctrlKey?+$.datepicker._get(b,"stepBigMonths"):+$.datepicker._get(b,"stepMonths"),"M");break;case 40:(a.ctrlKey||a.metaKey)&&$.datepicker._adjustDate(a.target,7,"D"),c=a.ctrlKey||a.metaKey;break;default:c=!1}else 36==a.keyCode&&a.ctrlKey?$.datepicker._showDatepicker(this):c=!1;c&&(a.preventDefault(),a.stopPropagation())},_doKeyPress:function(a){var b=$.datepicker._getInst(a.target);if($.datepicker._get(b,"constrainInput")){var c=$.datepicker._possibleChars($.datepicker._get(b,"dateFormat")),d=String.fromCharCode(a.charCode==undefined?a.keyCode:a.charCode);return a.ctrlKey||a.metaKey||" ">d||!c||c.indexOf(d)>-1}},_doKeyUp:function(a){var b=$.datepicker._getInst(a.target);if(b.input.val()!=b.lastVal)try{var c=$.datepicker.parseDate($.datepicker._get(b,"dateFormat"),b.input?b.input.val():null,$.datepicker._getFormatConfig(b));c&&($.datepicker._setDateFromField(b),$.datepicker._updateAlternate(b),$.datepicker._updateDatepicker(b))}catch(d){$.datepicker.log(d)}return!0},_showDatepicker:function(a){if(a=a.target||a,"input"!=a.nodeName.toLowerCase()&&(a=$("input",a.parentNode)[0]),!$.datepicker._isDisabledDatepicker(a)&&$.datepicker._lastInput!=a){var b=$.datepicker._getInst(a);$.datepicker._curInst&&$.datepicker._curInst!=b&&($.datepicker._curInst.dpDiv.stop(!0,!0),b&&$.datepicker._datepickerShowing&&$.datepicker._hideDatepicker($.datepicker._curInst.input[0]));var c=$.datepicker._get(b,"beforeShow"),d=c?c.apply(a,[a,b]):{};if(d!==!1){extendRemove(b.settings,d),b.lastVal=null,$.datepicker._lastInput=a,$.datepicker._setDateFromField(b),$.datepicker._inDialog&&(a.value=""),$.datepicker._pos||($.datepicker._pos=$.datepicker._findPos(a),$.datepicker._pos[1]+=a.offsetHeight);var e=!1;$(a).parents().each(function(){return e|="fixed"==$(this).css("position"),!e}),e&&$.browser.opera&&($.datepicker._pos[0]-=document.documentElement.scrollLeft,$.datepicker._pos[1]-=document.documentElement.scrollTop);var f={left:$.datepicker._pos[0],top:$.datepicker._pos[1]};if($.datepicker._pos=null,b.dpDiv.empty(),b.dpDiv.css({position:"absolute",display:"block",top:"-1000px"}),$.datepicker._updateDatepicker(b),f=$.datepicker._checkOffset(b,f,e),b.dpDiv.css({position:$.datepicker._inDialog&&$.blockUI?"static":e?"fixed":"absolute",display:"none",left:f.left+"px",top:f.top+"px"}),!b.inline){var g=$.datepicker._get(b,"showAnim"),h=$.datepicker._get(b,"duration"),i=function(){var a=b.dpDiv.find("iframe.ui-datepicker-cover");
if(a.length){var c=$.datepicker._getBorders(b.dpDiv);a.css({left:-c[0],top:-c[1],width:b.dpDiv.outerWidth(),height:b.dpDiv.outerHeight()})}};b.dpDiv.zIndex($(a).zIndex()+1),$.datepicker._datepickerShowing=!0,$.effects&&$.effects[g]?b.dpDiv.show(g,$.datepicker._get(b,"showOptions"),h,i):b.dpDiv[g||"show"](g?h:null,i),g&&h||i(),b.input.is(":visible")&&!b.input.is(":disabled")&&b.input.focus(),$.datepicker._curInst=b}}}},_updateDatepicker:function(a){var b=this;b.maxRows=4;var c=$.datepicker._getBorders(a.dpDiv);instActive=a,a.dpDiv.empty().append(this._generateHTML(a)),this._attachHandlers(a);var d=a.dpDiv.find("iframe.ui-datepicker-cover");d.length&&d.css({left:-c[0],top:-c[1],width:a.dpDiv.outerWidth(),height:a.dpDiv.outerHeight()}),a.dpDiv.find("."+this._dayOverClass+" a").mouseover();var e=this._getNumberOfMonths(a),f=e[1],g=17;if(a.dpDiv.removeClass("ui-datepicker-multi-2 ui-datepicker-multi-3 ui-datepicker-multi-4").width(""),f>1&&a.dpDiv.addClass("ui-datepicker-multi-"+f).css("width",g*f+"em"),a.dpDiv[(1!=e[0]||1!=e[1]?"add":"remove")+"Class"]("ui-datepicker-multi"),a.dpDiv[(this._get(a,"isRTL")?"add":"remove")+"Class"]("ui-datepicker-rtl"),a==$.datepicker._curInst&&$.datepicker._datepickerShowing&&a.input&&a.input.is(":visible")&&!a.input.is(":disabled")&&a.input[0]!=document.activeElement&&a.input.focus(),a.yearshtml){var h=a.yearshtml;setTimeout(function(){h===a.yearshtml&&a.yearshtml&&a.dpDiv.find("select.ui-datepicker-year:first").replaceWith(a.yearshtml),h=a.yearshtml=null},0)}},_getBorders:function(a){var b=function(a){return{thin:1,medium:2,thick:3}[a]||a};return[parseFloat(b(a.css("border-left-width"))),parseFloat(b(a.css("border-top-width")))]},_checkOffset:function(a,b,c){var d=a.dpDiv.outerWidth(),e=a.dpDiv.outerHeight(),f=a.input?a.input.outerWidth():0,g=a.input?a.input.outerHeight():0,h=document.documentElement.clientWidth+(c?0:$(document).scrollLeft()),i=document.documentElement.clientHeight+(c?0:$(document).scrollTop());return b.left-=this._get(a,"isRTL")?d-f:0,b.left-=c&&b.left==a.input.offset().left?$(document).scrollLeft():0,b.top-=c&&b.top==a.input.offset().top+g?$(document).scrollTop():0,b.left-=Math.min(b.left,b.left+d>h&&h>d?Math.abs(b.left+d-h):0),b.top-=Math.min(b.top,b.top+e>i&&i>e?Math.abs(e+g):0),b},_findPos:function(a){for(var b=this._getInst(a),c=this._get(b,"isRTL");a&&("hidden"==a.type||1!=a.nodeType||$.expr.filters.hidden(a));)a=a[c?"previousSibling":"nextSibling"];var d=$(a).offset();return[d.left,d.top]},_hideDatepicker:function(a){var b=this._curInst;if(b&&(!a||b==$.data(a,PROP_NAME))&&this._datepickerShowing){var c=this._get(b,"showAnim"),d=this._get(b,"duration"),e=function(){$.datepicker._tidyDialog(b)};$.effects&&$.effects[c]?b.dpDiv.hide(c,$.datepicker._get(b,"showOptions"),d,e):b.dpDiv["slideDown"==c?"slideUp":"fadeIn"==c?"fadeOut":"hide"](c?d:null,e),c||e(),this._datepickerShowing=!1;var f=this._get(b,"onClose");f&&f.apply(b.input?b.input[0]:null,[b.input?b.input.val():"",b]),this._lastInput=null,this._inDialog&&(this._dialogInput.css({position:"absolute",left:"0",top:"-100px"}),$.blockUI&&($.unblockUI(),$("body").append(this.dpDiv))),this._inDialog=!1}},_tidyDialog:function(a){a.dpDiv.removeClass(this._dialogClass).unbind(".ui-datepicker-calendar")},_checkExternalClick:function(a){if($.datepicker._curInst){var b=$(a.target),c=$.datepicker._getInst(b[0]);(b[0].id!=$.datepicker._mainDivId&&0==b.parents("#"+$.datepicker._mainDivId).length&&!b.hasClass($.datepicker.markerClassName)&&!b.closest("."+$.datepicker._triggerClass).length&&$.datepicker._datepickerShowing&&(!$.datepicker._inDialog||!$.blockUI)||b.hasClass($.datepicker.markerClassName)&&$.datepicker._curInst!=c)&&$.datepicker._hideDatepicker()}},_adjustDate:function(a,b,c){var d=$(a),e=this._getInst(d[0]);this._isDisabledDatepicker(d[0])||(this._adjustInstDate(e,b+("M"==c?this._get(e,"showCurrentAtPos"):0),c),this._updateDatepicker(e))},_gotoToday:function(a){var b=$(a),c=this._getInst(b[0]);if(this._get(c,"gotoCurrent")&&c.currentDay)c.selectedDay=c.currentDay,c.drawMonth=c.selectedMonth=c.currentMonth,c.drawYear=c.selectedYear=c.currentYear;else{var d=new Date;c.selectedDay=d.getDate(),c.drawMonth=c.selectedMonth=d.getMonth(),c.drawYear=c.selectedYear=d.getFullYear()}this._notifyChange(c),this._adjustDate(b)},_selectMonthYear:function(a,b,c){var d=$(a),e=this._getInst(d[0]);e["selected"+("M"==c?"Month":"Year")]=e["draw"+("M"==c?"Month":"Year")]=parseInt(b.options[b.selectedIndex].value,10),this._notifyChange(e),this._adjustDate(d)},_selectDay:function(a,b,c,d){var e=$(a);if(!$(d).hasClass(this._unselectableClass)&&!this._isDisabledDatepicker(e[0])){var f=this._getInst(e[0]);f.selectedDay=f.currentDay=$("a",d).html(),f.selectedMonth=f.currentMonth=b,f.selectedYear=f.currentYear=c,this._selectDate(a,this._formatDate(f,f.currentDay,f.currentMonth,f.currentYear))}},_clearDate:function(a){{var b=$(a);this._getInst(b[0])}this._selectDate(b,"")},_selectDate:function(a,b){var c=$(a),d=this._getInst(c[0]);b=null!=b?b:this._formatDate(d),d.input&&d.input.val(b),this._updateAlternate(d);var e=this._get(d,"onSelect");e?e.apply(d.input?d.input[0]:null,[b,d]):d.input&&d.input.trigger("change"),d.inline?this._updateDatepicker(d):(this._hideDatepicker(),this._lastInput=d.input[0],"object"!=typeof d.input[0]&&d.input.focus(),this._lastInput=null)},_updateAlternate:function(a){var b=this._get(a,"altField");if(b){var c=this._get(a,"altFormat")||this._get(a,"dateFormat"),d=this._getDate(a),e=this.formatDate(c,d,this._getFormatConfig(a));$(b).each(function(){$(this).val(e)})}},noWeekends:function(a){var b=a.getDay();return[b>0&&6>b,""]},iso8601Week:function(a){var b=new Date(a.getTime());b.setDate(b.getDate()+4-(b.getDay()||7));var c=b.getTime();return b.setMonth(0),b.setDate(1),Math.floor(Math.round((c-b)/864e5)/7)+1},parseDate:function(a,b,c){if(null==a||null==b)throw"Invalid arguments";if(b="object"==typeof b?b.toString():b+"",""==b)return null;var d=(c?c.shortYearCutoff:null)||this._defaults.shortYearCutoff;d="string"!=typeof d?d:(new Date).getFullYear()%100+parseInt(d,10);for(var e=(c?c.dayNamesShort:null)||this._defaults.dayNamesShort,f=(c?c.dayNames:null)||this._defaults.dayNames,g=(c?c.monthNamesShort:null)||this._defaults.monthNamesShort,h=(c?c.monthNames:null)||this._defaults.monthNames,i=-1,j=-1,k=-1,l=-1,m=!1,n=function(b){var c=s+1<a.length&&a.charAt(s+1)==b;return c&&s++,c},o=function(a){var c=n(a),d="@"==a?14:"!"==a?20:"y"==a&&c?4:"o"==a?3:2,e=new RegExp("^\\d{1,"+d+"}"),f=b.substring(r).match(e);if(!f)throw"Missing number at position "+r;return r+=f[0].length,parseInt(f[0],10)},p=function(a,c,d){var e=$.map(n(a)?d:c,function(a,b){return[[b,a]]}).sort(function(a,b){return-(a[1].length-b[1].length)}),f=-1;if($.each(e,function(a,c){var d=c[1];return b.substr(r,d.length).toLowerCase()==d.toLowerCase()?(f=c[0],r+=d.length,!1):void 0}),-1!=f)return f+1;throw"Unknown name at position "+r},q=function(){if(b.charAt(r)!=a.charAt(s))throw"Unexpected literal at position "+r;r++},r=0,s=0;s<a.length;s++)if(m)"'"!=a.charAt(s)||n("'")?q():m=!1;else switch(a.charAt(s)){case"d":k=o("d");break;case"D":p("D",e,f);break;case"o":l=o("o");break;case"m":j=o("m");break;case"M":j=p("M",g,h);break;case"y":i=o("y");break;case"@":var t=new Date(o("@"));i=t.getFullYear(),j=t.getMonth()+1,k=t.getDate();break;case"!":var t=new Date((o("!")-this._ticksTo1970)/1e4);i=t.getFullYear(),j=t.getMonth()+1,k=t.getDate();break;case"'":n("'")?q():m=!0;break;default:q()}if(r<b.length)throw"Extra/unparsed characters found in date: "+b.substring(r);if(-1==i?i=(new Date).getFullYear():100>i&&(i+=(new Date).getFullYear()-(new Date).getFullYear()%100+(d>=i?0:-100)),l>-1)for(j=1,k=l;;){var u=this._getDaysInMonth(i,j-1);if(u>=k)break;j++,k-=u}var t=this._daylightSavingAdjust(new Date(i,j-1,k));if(t.getFullYear()!=i||t.getMonth()+1!=j||t.getDate()!=k)throw"Invalid date";return t},ATOM:"yy-mm-dd",COOKIE:"D, dd M yy",ISO_8601:"yy-mm-dd",RFC_822:"D, d M y",RFC_850:"DD, dd-M-y",RFC_1036:"D, d M y",RFC_1123:"D, d M yy",RFC_2822:"D, d M yy",RSS:"D, d M y",TICKS:"!",TIMESTAMP:"@",W3C:"yy-mm-dd",_ticksTo1970:24*(718685+Math.floor(492.5)-Math.floor(19.7)+Math.floor(4.925))*60*60*1e7,formatDate:function(a,b,c){if(!b)return"";var d=(c?c.dayNamesShort:null)||this._defaults.dayNamesShort,e=(c?c.dayNames:null)||this._defaults.dayNames,f=(c?c.monthNamesShort:null)||this._defaults.monthNamesShort,g=(c?c.monthNames:null)||this._defaults.monthNames,h=function(b){var c=m+1<a.length&&a.charAt(m+1)==b;return c&&m++,c},i=function(a,b,c){var d=""+b;if(h(a))for(;d.length<c;)d="0"+d;return d},j=function(a,b,c,d){return h(a)?d[b]:c[b]},k="",l=!1;if(b)for(var m=0;m<a.length;m++)if(l)"'"!=a.charAt(m)||h("'")?k+=a.charAt(m):l=!1;else switch(a.charAt(m)){case"d":k+=i("d",b.getDate(),2);break;case"D":k+=j("D",b.getDay(),d,e);break;case"o":k+=i("o",Math.round((new Date(b.getFullYear(),b.getMonth(),b.getDate()).getTime()-new Date(b.getFullYear(),0,0).getTime())/864e5),3);break;case"m":k+=i("m",b.getMonth()+1,2);break;case"M":k+=j("M",b.getMonth(),f,g);break;case"y":k+=h("y")?b.getFullYear():(b.getYear()%100<10?"0":"")+b.getYear()%100;break;case"@":k+=b.getTime();break;case"!":k+=1e4*b.getTime()+this._ticksTo1970;break;case"'":h("'")?k+="'":l=!0;break;default:k+=a.charAt(m)}return k},_possibleChars:function(a){for(var b="",c=!1,d=function(b){var c=e+1<a.length&&a.charAt(e+1)==b;return c&&e++,c},e=0;e<a.length;e++)if(c)"'"!=a.charAt(e)||d("'")?b+=a.charAt(e):c=!1;else switch(a.charAt(e)){case"d":case"m":case"y":case"@":b+="0123456789";break;case"D":case"M":return null;case"'":d("'")?b+="'":c=!0;break;default:b+=a.charAt(e)}return b},_get:function(a,b){return a.settings[b]!==undefined?a.settings[b]:this._defaults[b]},_setDateFromField:function(a,b){if(a.input.val()!=a.lastVal){var c,d,e=this._get(a,"dateFormat"),f=a.lastVal=a.input?a.input.val():null;c=d=this._getDefaultDate(a);var g=this._getFormatConfig(a);try{c=this.parseDate(e,f,g)||d}catch(h){this.log(h),f=b?"":f}a.selectedDay=c.getDate(),a.drawMonth=a.selectedMonth=c.getMonth(),a.drawYear=a.selectedYear=c.getFullYear(),a.currentDay=f?c.getDate():0,a.currentMonth=f?c.getMonth():0,a.currentYear=f?c.getFullYear():0,this._adjustInstDate(a)}},_getDefaultDate:function(a){return this._restrictMinMax(a,this._determineDate(a,this._get(a,"defaultDate"),new Date))},_determineDate:function(a,b,c){var d=function(a){var b=new Date;return b.setDate(b.getDate()+a),b},e=function(b){try{return $.datepicker.parseDate($.datepicker._get(a,"dateFormat"),b,$.datepicker._getFormatConfig(a))}catch(c){}for(var d=(b.toLowerCase().match(/^c/)?$.datepicker._getDate(a):null)||new Date,e=d.getFullYear(),f=d.getMonth(),g=d.getDate(),h=/([+-]?[0-9]+)\s*(d|D|w|W|m|M|y|Y)?/g,i=h.exec(b);i;){switch(i[2]||"d"){case"d":case"D":g+=parseInt(i[1],10);break;case"w":case"W":g+=7*parseInt(i[1],10);break;case"m":case"M":f+=parseInt(i[1],10),g=Math.min(g,$.datepicker._getDaysInMonth(e,f));break;case"y":case"Y":e+=parseInt(i[1],10),g=Math.min(g,$.datepicker._getDaysInMonth(e,f))}i=h.exec(b)}return new Date(e,f,g)},f=null==b||""===b?c:"string"==typeof b?e(b):"number"==typeof b?isNaN(b)?c:d(b):new Date(b.getTime());return f=f&&"Invalid Date"==f.toString()?c:f,f&&(f.setHours(0),f.setMinutes(0),f.setSeconds(0),f.setMilliseconds(0)),this._daylightSavingAdjust(f)},_daylightSavingAdjust:function(a){return a?(a.setHours(a.getHours()>12?a.getHours()+2:0),a):null},_setDate:function(a,b,c){var d=!b,e=a.selectedMonth,f=a.selectedYear,g=this._restrictMinMax(a,this._determineDate(a,b,new Date));a.selectedDay=a.currentDay=g.getDate(),a.drawMonth=a.selectedMonth=a.currentMonth=g.getMonth(),a.drawYear=a.selectedYear=a.currentYear=g.getFullYear(),e==a.selectedMonth&&f==a.selectedYear||c||this._notifyChange(a),this._adjustInstDate(a),a.input&&a.input.val(d?"":this._formatDate(a))},_getDate:function(a){var b=!a.currentYear||a.input&&""==a.input.val()?null:this._daylightSavingAdjust(new Date(a.currentYear,a.currentMonth,a.currentDay));return b},_attachHandlers:function(a){var b=this._get(a,"stepMonths"),c="#"+a.id.replace(/\\\\/g,"\\");a.dpDiv.find("[data-handler]").map(function(){var a={prev:function(){window["DP_jQuery_"+dpuuid].datepicker._adjustDate(c,-b,"M")},next:function(){window["DP_jQuery_"+dpuuid].datepicker._adjustDate(c,+b,"M")},hide:function(){window["DP_jQuery_"+dpuuid].datepicker._hideDatepicker()},today:function(){window["DP_jQuery_"+dpuuid].datepicker._gotoToday(c)},selectDay:function(){return window["DP_jQuery_"+dpuuid].datepicker._selectDay(c,+this.getAttribute("data-month"),+this.getAttribute("data-year"),this),!1},selectMonth:function(){return window["DP_jQuery_"+dpuuid].datepicker._selectMonthYear(c,this,"M"),!1},selectYear:function(){return window["DP_jQuery_"+dpuuid].datepicker._selectMonthYear(c,this,"Y"),!1}};$(this).bind(this.getAttribute("data-event"),a[this.getAttribute("data-handler")])})},_generateHTML:function(a){var b=new Date;b=this._daylightSavingAdjust(new Date(b.getFullYear(),b.getMonth(),b.getDate()));var c=this._get(a,"isRTL"),d=this._get(a,"showButtonPanel"),e=this._get(a,"hideIfNoPrevNext"),f=this._get(a,"navigationAsDateFormat"),g=this._getNumberOfMonths(a),h=this._get(a,"showCurrentAtPos"),i=this._get(a,"stepMonths"),j=1!=g[0]||1!=g[1],k=this._daylightSavingAdjust(a.currentDay?new Date(a.currentYear,a.currentMonth,a.currentDay):new Date(9999,9,9)),l=this._getMinMaxDate(a,"min"),m=this._getMinMaxDate(a,"max"),n=a.drawMonth-h,o=a.drawYear;if(0>n&&(n+=12,o--),m){var p=this._daylightSavingAdjust(new Date(m.getFullYear(),m.getMonth()-g[0]*g[1]+1,m.getDate()));for(p=l&&l>p?l:p;this._daylightSavingAdjust(new Date(o,n,1))>p;)n--,0>n&&(n=11,o--)}a.drawMonth=n,a.drawYear=o;var q=this._get(a,"prevText");q=f?this.formatDate(q,this._daylightSavingAdjust(new Date(o,n-i,1)),this._getFormatConfig(a)):q;var r=this._canAdjustMonth(a,-1,o,n)?'<a class="ui-datepicker-prev ui-corner-all" data-handler="prev" data-event="click" title="'+q+'"><span class="ui-icon ui-icon-circle-triangle-'+(c?"e":"w")+'">'+q+"</span></a>":e?"":'<a class="ui-datepicker-prev ui-corner-all ui-state-disabled" title="'+q+'"><span class="ui-icon ui-icon-circle-triangle-'+(c?"e":"w")+'">'+q+"</span></a>",s=this._get(a,"nextText");s=f?this.formatDate(s,this._daylightSavingAdjust(new Date(o,n+i,1)),this._getFormatConfig(a)):s;var t=this._canAdjustMonth(a,1,o,n)?'<a class="ui-datepicker-next ui-corner-all" data-handler="next" data-event="click" title="'+s+'"><span class="ui-icon ui-icon-circle-triangle-'+(c?"w":"e")+'">'+s+"</span></a>":e?"":'<a class="ui-datepicker-next ui-corner-all ui-state-disabled" title="'+s+'"><span class="ui-icon ui-icon-circle-triangle-'+(c?"w":"e")+'">'+s+"</span></a>",u=this._get(a,"currentText"),v=this._get(a,"gotoCurrent")&&a.currentDay?k:b;u=f?this.formatDate(u,v,this._getFormatConfig(a)):u;var w=a.inline?"":'<button type="button" class="ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all" data-handler="hide" data-event="click">'+this._get(a,"closeText")+"</button>",x=d?'<div class="ui-datepicker-buttonpane ui-widget-content">'+(c?w:"")+(this._isInRange(a,v)?'<button type="button" class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all" data-handler="today" data-event="click">'+u+"</button>":"")+(c?"":w)+"</div>":"",y=parseInt(this._get(a,"firstDay"),10);y=isNaN(y)?0:y;for(var z=this._get(a,"showWeek"),A=this._get(a,"dayNames"),B=(this._get(a,"dayNamesShort"),this._get(a,"dayNamesMin")),C=this._get(a,"monthNames"),D=this._get(a,"monthNamesShort"),E=this._get(a,"beforeShowDay"),F=this._get(a,"showOtherMonths"),G=this._get(a,"selectOtherMonths"),H=(this._get(a,"calculateWeek")||this.iso8601Week,this._getDefaultDate(a)),I="",J=0;J<g[0];J++){var K="";this.maxRows=4;for(var L=0;L<g[1];L++){var M=this._daylightSavingAdjust(new Date(o,n,a.selectedDay)),N=" ui-corner-all",O="";if(j){if(O+='<div class="ui-datepicker-group',g[1]>1)switch(L){case 0:O+=" ui-datepicker-group-first",N=" ui-corner-"+(c?"right":"left");break;case g[1]-1:O+=" ui-datepicker-group-last",N=" ui-corner-"+(c?"left":"right");break;default:O+=" ui-datepicker-group-middle",N=""}O+='">'}O+='<div class="ui-datepicker-header ui-widget-header ui-helper-clearfix'+N+'">'+(/all|left/.test(N)&&0==J?c?t:r:"")+(/all|right/.test(N)&&0==J?c?r:t:"")+this._generateMonthYearHeader(a,n,o,l,m,J>0||L>0,C,D)+'</div><table class="ui-datepicker-calendar"><thead><tr>';for(var P=z?'<th class="ui-datepicker-week-col">'+this._get(a,"weekHeader")+"</th>":"",Q=0;7>Q;Q++){var R=(Q+y)%7;P+="<th"+((Q+y+6)%7>=5?' class="ui-datepicker-week-end"':"")+'><span title="'+A[R]+'">'+B[R]+"</span></th>"}O+=P+"</tr></thead><tbody>";var S=this._getDaysInMonth(o,n);o==a.selectedYear&&n==a.selectedMonth&&(a.selectedDay=Math.min(a.selectedDay,S));var T=(this._getFirstDayOfMonth(o,n)-y+7)%7,U=Math.ceil((T+S)/7),V=j&&this.maxRows>U?this.maxRows:U;this.maxRows=V;for(var W=this._daylightSavingAdjust(new Date(o,n,1-T)),X=0;V>X;X++){O+="<tr>";for(var Y=z?'<td class="ui-datepicker-week-col">'+this._get(a,"calculateWeek")(W)+"</td>":"",Q=0;7>Q;Q++){var Z=E?E.apply(a.input?a.input[0]:null,[W]):[!0,""],_=W.getMonth()!=n,ab=_&&!G||!Z[0]||l&&l>W||m&&W>m;Y+='<td class="'+((Q+y+6)%7>=5?" ui-datepicker-week-end":"")+(_?" ui-datepicker-other-month":"")+(W.getTime()==M.getTime()&&n==a.selectedMonth&&a._keyEvent||H.getTime()==W.getTime()&&H.getTime()==M.getTime()?" "+this._dayOverClass:"")+(ab?" "+this._unselectableClass+" ui-state-disabled":"")+(_&&!F?"":" "+Z[1]+(W.getTime()==k.getTime()?" "+this._currentClass:"")+(W.getTime()==b.getTime()?" ui-datepicker-today":""))+'"'+(_&&!F||!Z[2]?"":' title="'+Z[2]+'"')+(ab?"":' data-handler="selectDay" data-event="click" data-month="'+W.getMonth()+'" data-year="'+W.getFullYear()+'"')+">"+(_&&!F?"&#xa0;":ab?'<span class="ui-state-default">'+W.getDate()+"</span>":'<a class="ui-state-default'+(W.getTime()==b.getTime()?" ui-state-highlight":"")+(W.getTime()==k.getTime()?" ui-state-active":"")+(_?" ui-priority-secondary":"")+'" href="#">'+W.getDate()+"</a>")+"</td>",W.setDate(W.getDate()+1),W=this._daylightSavingAdjust(W)}O+=Y+"</tr>"}n++,n>11&&(n=0,o++),O+="</tbody></table>"+(j?"</div>"+(g[0]>0&&L==g[1]-1?'<div class="ui-datepicker-row-break"></div>':""):""),K+=O}I+=K}return I+=x+($.browser.msie&&parseInt($.browser.version,10)<7&&!a.inline?'<iframe src="javascript:false;" class="ui-datepicker-cover" frameborder="0"></iframe>':""),a._keyEvent=!1,I},_generateMonthYearHeader:function(a,b,c,d,e,f,g,h){var i=this._get(a,"changeMonth"),j=this._get(a,"changeYear"),k=this._get(a,"showMonthAfterYear"),l='<div class="ui-datepicker-title">',m="";if(f||!i)m+='<span class="ui-datepicker-month">'+g[b]+"</span>";else{var n=d&&d.getFullYear()==c,o=e&&e.getFullYear()==c;m+='<select class="ui-datepicker-month" data-handler="selectMonth" data-event="change">';for(var p=0;12>p;p++)(!n||p>=d.getMonth())&&(!o||p<=e.getMonth())&&(m+='<option value="'+p+'"'+(p==b?' selected="selected"':"")+">"+h[p]+"</option>");m+="</select>"}if(k||(l+=m+(!f&&i&&j?"":"&#xa0;")),!a.yearshtml)if(a.yearshtml="",f||!j)l+='<span class="ui-datepicker-year">'+c+"</span>";else{var q=this._get(a,"yearRange").split(":"),r=(new Date).getFullYear(),s=function(a){var b=a.match(/c[+-].*/)?c+parseInt(a.substring(1),10):a.match(/[+-].*/)?r+parseInt(a,10):parseInt(a,10);return isNaN(b)?r:b},t=s(q[0]),u=Math.max(t,s(q[1]||""));for(t=d?Math.max(t,d.getFullYear()):t,u=e?Math.min(u,e.getFullYear()):u,a.yearshtml+='<select class="ui-datepicker-year" data-handler="selectYear" data-event="change">';u>=t;t++)a.yearshtml+='<option value="'+t+'"'+(t==c?' selected="selected"':"")+">"+t+"</option>";a.yearshtml+="</select>",l+=a.yearshtml,a.yearshtml=null}return l+=this._get(a,"yearSuffix"),k&&(l+=(!f&&i&&j?"":"&#xa0;")+m),l+="</div>"},_adjustInstDate:function(a,b,c){var d=a.drawYear+("Y"==c?b:0),e=a.drawMonth+("M"==c?b:0),f=Math.min(a.selectedDay,this._getDaysInMonth(d,e))+("D"==c?b:0),g=this._restrictMinMax(a,this._daylightSavingAdjust(new Date(d,e,f)));a.selectedDay=g.getDate(),a.drawMonth=a.selectedMonth=g.getMonth(),a.drawYear=a.selectedYear=g.getFullYear(),("M"==c||"Y"==c)&&this._notifyChange(a)},_restrictMinMax:function(a,b){var c=this._getMinMaxDate(a,"min"),d=this._getMinMaxDate(a,"max"),e=c&&c>b?c:b;return e=d&&e>d?d:e},_notifyChange:function(a){var b=this._get(a,"onChangeMonthYear");b&&b.apply(a.input?a.input[0]:null,[a.selectedYear,a.selectedMonth+1,a])},_getNumberOfMonths:function(a){var b=this._get(a,"numberOfMonths");return null==b?[1,1]:"number"==typeof b?[1,b]:b},_getMinMaxDate:function(a,b){return this._determineDate(a,this._get(a,b+"Date"),null)},_getDaysInMonth:function(a,b){return 32-this._daylightSavingAdjust(new Date(a,b,32)).getDate()},_getFirstDayOfMonth:function(a,b){return new Date(a,b,1).getDay()},_canAdjustMonth:function(a,b,c,d){var e=this._getNumberOfMonths(a),f=this._daylightSavingAdjust(new Date(c,d+(0>b?b:e[0]*e[1]),1));return 0>b&&f.setDate(this._getDaysInMonth(f.getFullYear(),f.getMonth())),this._isInRange(a,f)},_isInRange:function(a,b){var c=this._getMinMaxDate(a,"min"),d=this._getMinMaxDate(a,"max");return(!c||b.getTime()>=c.getTime())&&(!d||b.getTime()<=d.getTime())},_getFormatConfig:function(a){var b=this._get(a,"shortYearCutoff");return b="string"!=typeof b?b:(new Date).getFullYear()%100+parseInt(b,10),{shortYearCutoff:b,dayNamesShort:this._get(a,"dayNamesShort"),dayNames:this._get(a,"dayNames"),monthNamesShort:this._get(a,"monthNamesShort"),monthNames:this._get(a,"monthNames")}},_formatDate:function(a,b,c,d){b||(a.currentDay=a.selectedDay,a.currentMonth=a.selectedMonth,a.currentYear=a.selectedYear);var e=b?"object"==typeof b?b:this._daylightSavingAdjust(new Date(d,c,b)):this._daylightSavingAdjust(new Date(a.currentYear,a.currentMonth,a.currentDay));return this.formatDate(this._get(a,"dateFormat"),e,this._getFormatConfig(a))}}),$.fn.datepicker=function(a){if(!this.length)return this;$.datepicker.initialized||($(document).mousedown($.datepicker._checkExternalClick).find("body").append($.datepicker.dpDiv),$.datepicker.initialized=!0);var b=Array.prototype.slice.call(arguments,1);return"string"!=typeof a||"isDisabled"!=a&&"getDate"!=a&&"widget"!=a?"option"==a&&2==arguments.length&&"string"==typeof arguments[1]?$.datepicker["_"+a+"Datepicker"].apply($.datepicker,[this[0]].concat(b)):this.each(function(){"string"==typeof a?$.datepicker["_"+a+"Datepicker"].apply($.datepicker,[this].concat(b)):$.datepicker._attachDatepicker(this,a)}):$.datepicker["_"+a+"Datepicker"].apply($.datepicker,[this[0]].concat(b))},$.datepicker=new Datepicker,$.datepicker.initialized=!1,$.datepicker.uuid=(new Date).getTime(),$.datepicker.version="1.8.24",window["DP_jQuery_"+dpuuid]=$}(jQuery),function(){"use strict";if(!window.jQuery&&!window.Zepto)throw new Error("either jQuery or Zepto is required for AJS to function.");"undefined"==typeof window.console?window.console={messages:[],log:function(a){this.messages.push(a)},show:function(){alert(this.messages.join("\n")),this.messages=[]}}:console.show=function(){},window.AJS=function(){function a(a){var b={"<":"&lt;",">":"&gt;","&":"&amp;","'":"&#39;","`":"&#96;"};return"string"==typeof b[a]?b[a]:"&quot;"}var b,c,d=[],e=0,f=/[&"'<>`]/g,g={version:"5.5.1",params:{},$:window.jQuery||window.Zepto,log:function(){return"undefined"!=typeof console&&console.log?Function.prototype.bind?Function.prototype.bind.call(console.log,console):function(){Function.prototype.apply.call(console.log,console,arguments)}:function(){}}(),warn:function(){"undefined"!=typeof console&&console.warn&&Function.prototype.apply.apply(console.warn,[console,arguments])},error:function(){"undefined"!=typeof console&&console.error&&Function.prototype.apply.apply(console.error,[console,arguments])},preventDefault:function(a){a.preventDefault()},stopEvent:function(a){return a.stopPropagation(),!1},include:function(a){if(!this.contains(d,a)){d.push(a);var b=document.createElement("script");b.src=a,this.$("body").append(b)}},toggleClassName:function(a,b){(a=this.$(a))&&a.toggleClass(b)},setVisible:function(a,b){if(a=this.$(a)){var c=this.$;c(a).each(function(){var a=c(this).hasClass("hidden");a&&b?c(this).removeClass("hidden"):a||b||c(this).addClass("hidden")})}},setCurrent:function(a,b){(a=this.$(a))&&(b?a.addClass("current"):a.removeClass("current"))},isVisible:function(a){return!this.$(a).hasClass("hidden")},isClipped:function(a){return a=AJS.$(a),a.prop("scrollWidth")>a.prop("clientWidth")},populateParameters:function(a){a||(a=this.params);var b=this;this.$(".parameters input").each(function(){var c=this.value,d=this.title||this.id;b.$(this).hasClass("list")?a[d]?a[d].push(c):a[d]=[c]:a[d]=c.match(/^(tru|fals)e$/i)?"true"===c.toLowerCase():c})},toInit:function(a){var b=this;return this.$(function(){try{a.apply(this,arguments)}catch(c){b.log("Failed to run init function: "+c+"\n"+a.toString())}}),this},indexOf:function(a,b,c){var d=a.length;c?0>c&&(c=Math.max(0,d+c)):c=0;for(var e=c;d>e;e++)if(a[e]===b)return e;return-1},contains:function(a,b){return this.indexOf(a,b)>-1},firebug:function(){AJS.log("DEPRECATED: AJS.firebug should no longer be used.");var a=this.$(document.createElement("script"));a.attr("src","https://getfirebug.com/releases/lite/1.2/firebug-lite-compressed.js"),this.$("head").append(a),function(){window.firebug?firebug.init():setTimeout(AJS.firebug,0)}()},clone:function(a){return AJS.$(a).clone().removeAttr("id")},alphanum:function(a,b){a=(a+"").toLowerCase(),b=(b+"").toLowerCase();for(var c=/(\d+|\D+)/g,d=a.match(c),e=b.match(c),f=Math.max(d.length,e.length),g=0;f>g;g++){if(g===d.length)return-1;if(g===e.length)return 1;var h=parseInt(d[g],10)+"",i=parseInt(e[g],10)+"";if(h===d[g]&&i===e[g]&&h!==i)return(h-i)/Math.abs(h-i);if((h!==d[g]||i!==e[g])&&d[g]!==e[g])return d[g]<e[g]?-1:1}return 0},onTextResize:function(a){if("function"==typeof a)if(AJS.onTextResize["on-text-resize"])AJS.onTextResize["on-text-resize"].push(function(b){a(b)});else{var b=AJS("div");b.css({width:"1em",height:"1em",position:"absolute",top:"-9999em",left:"-9999em"}),this.$("body").append(b),b.size=b.width(),setInterval(function(){if(b.size!==b.width()){b.size=b.width();for(var a=0,c=AJS.onTextResize["on-text-resize"].length;c>a;a++)AJS.onTextResize["on-text-resize"][a](b.size)}},0),AJS.onTextResize.em=b,AJS.onTextResize["on-text-resize"]=[function(b){a(b)}]}},unbindTextResize:function(a){for(var b=0,c=AJS.onTextResize["on-text-resize"].length;c>b;b++)if(AJS.onTextResize["on-text-resize"][b]===a)return AJS.onTextResize["on-text-resize"].splice(b,1)},escape:function(a){return escape(a).replace(/%u\w{4}/gi,function(a){return unescape(a)})},escapeHtml:function(b){return b.replace(f,a)},filterBySearch:function(a,b,c){if(!b)return[];var d=this.$,e=c&&c.keywordsField||"keywords",f=c&&c.ignoreForCamelCase?"i":"",g=c&&c.matchBoundary?"\\b":"",h=c&&c.splitRegex||/\s+/,i=b.split(h),j=[];d.each(i,function(){var a=[new RegExp(g+this,"i")];if(/^([A-Z][a-z]*) {2,}$/.test(this)){var b=this.replace(/([A-Z][a-z]*)/g,"\\b$1[^,]*");a.push(new RegExp(b,f))}j.push(a)});var k=[];return d.each(a,function(){for(var a=0;a<j.length;a++){for(var b=!1,c=0;c<j[a].length;c++)if(j[a][c].test(this[e])){b=!0;break}if(!b)return}k.push(this)}),k},drawLogo:function(a){AJS.log("DEPRECATED: AJS.drawLogo should no longer be used.");var b=a.scaleFactor||1,c=a.fill||"#fff",d=a.stroke||"#000",e=400*b,f=40*b,g=a.strokeWidth||1,h=a.containerID||".aui-logo";AJS.$(".aui-logo").length||AJS.$("body").append('<div id="aui-logo" class="aui-logo"><div>');var i=Raphael(h,e+50*b,f+100*b),j=i.path("M 0,0 c 3.5433333,-4.7243333 7.0866667,-9.4486667 10.63,-14.173 -14.173,0 -28.346,0 -42.519,0 C -35.432667,-9.4486667 -38.976333,-4.7243333 -42.52,0 -28.346667,0 -14.173333,0 0,0 z m 277.031,28.346 c -14.17367,0 -28.34733,0 -42.521,0 C 245.14,14.173 255.77,0 266.4,-14.173 c -14.17267,0 -28.34533,0 -42.518,0 C 213.25167,0 202.62133,14.173 191.991,28.346 c -14.17333,0 -28.34667,0 -42.52,0 14.17333,-18.8976667 28.34667,-37.7953333 42.52,-56.693 -7.08667,-9.448667 -14.17333,-18.897333 -21.26,-28.346 -14.173,0 -28.346,0 -42.519,0 7.08667,9.448667 14.17333,18.897333 21.26,28.346 -14.17333,18.8976667 -28.34667,37.7953333 -42.52,56.693 -14.173333,0 -28.346667,0 -42.52,0 10.63,-14.173 21.26,-28.346 31.89,-42.519 -14.390333,0 -28.780667,0 -43.171,0 C 42.520733,1.330715e-4 31.889933,14.174867 21.26,28.347 c -42.520624,6.24e-4 -85.039187,-8.13e-4 -127.559,-0.001 11.220667,-14.961 22.441333,-29.922 33.662,-44.883 -6.496,-8.661 -12.992,-17.322 -19.488,-25.983 5.905333,0 11.810667,0 17.716,0 -10.63,-14.173333 -21.26,-28.346667 -31.89,-42.52 14.173333,0 28.346667,0 42.52,0 10.63,14.173333 21.26,28.346667 31.89,42.52 14.173333,0 28.3466667,0 42.52,0 -10.63,-14.173333 -21.26,-28.346667 -31.89,-42.52 14.1733333,0 28.3466667,0 42.52,0 10.63,14.173333 21.26,28.346667 31.89,42.52 14.390333,0 28.780667,0 43.171,0 -10.63,-14.173333 -21.26,-28.346667 -31.89,-42.52 42.51967,0 85.03933,0 127.559,0 10.63033,14.173333 21.26067,28.346667 31.891,42.52 14.17267,0 28.34533,0 42.518,0 -10.63,-14.173333 -21.26,-28.346667 -31.89,-42.52 14.17367,0 28.34733,0 42.521,0 14.17333,18.897667 28.34667,37.795333 42.52,56.693 -14.17333,18.8976667 -28.34667,37.7953333 -42.52,56.693 z");j.scale(b,-b,0,0),j.translate(120*b,f),j.attr("fill",c),j.attr("stroke",d),j.attr("stroke-width",g)},debounce:function(a,b){var c,d;return function(){var e=arguments,f=this,g=function(){d=a.apply(f,e)};return clearTimeout(c),c=setTimeout(g,b),d}},id:function(a){if(b=e++ +"",c=a?a+b:"aui-uid-"+b,document.getElementById(c)){if(c=c+"-"+(new Date).getTime(),document.getElementById(c))throw new Error("ERROR: timestamped fallback ID "+c+" exists. AJS.id stopped.");return c}return c},_addID:function(a,b){var c=AJS.$(a),d=b||!1;c.each(function(){var a=AJS.$(this);a.attr("id")||a.attr("id",AJS.id(d))})},enable:function(a,b){var c=AJS.$(a);return"undefined"==typeof b&&(b=!0),c.each(function(){this.disabled=!b})}};if("undefined"!=typeof AJS)for(var h in AJS)g[h]=AJS[h];var i=function(){var a=null;return arguments.length&&"string"==typeof arguments[0]&&(a=AJS.$(document.createElement(arguments[0])),2===arguments.length&&a.html(arguments[1])),a};for(var j in g)i[j]=g[j];return i}(),AJS.$(function(){var a=AJS.$("body");a.data("auiVersion")||a.attr("data-aui-version",AJS.version),AJS.populateParameters()}),AJS.$.ajaxSettings.traditional=!0}(),AJS.format=function(){var a=/'(?!')/g,b=/^\d+$/,c=/^(\d+),number$/,d=/^(\d+)\,choice\,(.+)/,e=/^(\d+)([#<])(.+)/,f=function(a,f){var g,h="";if(g=a.match(b))h=f.length>++a?f[a]:"";else if(g=a.match(c))h=f.length>++g[1]?f[g[1]]:"";else if(g=a.match(d)){var i=f.length>++g[1]?f[g[1]]:null;if(null!==i){for(var j=g[2].split("|"),k=null,l=0;l<j.length;l++){var m=j[l].match(e),n=parseInt(m[1],10);if(n>i){if(k){h=k;break}h=m[3];break}if(i==n&&"#"==m[2]){h=m[3];break}l==j.length-1&&(h=m[3]),k=m[3]}var o=[h].concat(Array.prototype.slice.call(f,1));h=AJS.format.apply(AJS,o)}}return h},g=function(a){for(var b=!1,c=-1,d=0,e=0;e<a.length;e++){var f=a.charAt(e);if("'"==f&&(b=!b),!b)if("{"===f)0===d&&(c=e),d++;else if("}"===f&&d>0&&(d--,0===d)){var g=[];return g.push(a.substring(0,e+1)),g.push(a.substring(0,c)),g.push(a.substring(c+1,e)),g}}return null},h=function(b){for(var c=arguments,d="",e=g(b);e;)b=b.substring(e[0].length),d+=e[1].replace(a,""),d+=f(e[2],c),e=g(b);return d+=b.replace(a,"")};return h.apply(AJS,arguments)},AJS.I18n={getText:function(a){var b=Array.prototype.slice.call(arguments,1);return AJS.I18n.keys&&Object.prototype.hasOwnProperty.call(AJS.I18n.keys,a)?AJS.format.apply(null,[AJS.I18n.keys[a]].concat(b)):a}},AJS.I18n.keys={},AJS.I18n.keys["aui.words.add"]="Add",AJS.I18n.keys["aui.words.update"]="Update",AJS.I18n.keys["aui.words.delete"]="Delete",AJS.I18n.keys["aui.words.remove"]="Remove",AJS.I18n.keys["aui.words.cancel"]="Cancel",AJS.I18n.keys["aui.words.loading"]="Loading",AJS.I18n.keys["aui.words.close"]="Close",AJS.I18n.keys["aui.enter.value"]="Enter value",AJS.I18n.keys["aui.keyboard.shortcut.type.x"]="Type ''{0}''",AJS.I18n.keys["aui.keyboard.shortcut.then.x"]="then ''{0}''",AJS.I18n.keys["aui.keyboard.shortcut.or.x"]="OR ''{0}''",AJS.I18n.keys["aui.words.more"]="More",AJS.I18n.keys["aui.validation.message.maxlength"]="Must be fewer than {0} characters",AJS.I18n.keys["aui.validation.message.minlength"]="Must be greater than {0} characters",AJS.I18n.keys["aui.validation.message.matchingfield"]="{0} and {1} do not match.",AJS.I18n.keys["aui.validation.message.doesnotcontain"]="Do not include the phrase {0} in this field",AJS.I18n.keys["aui.validation.message.pattern"]="This field does not match the required format",AJS.I18n.keys["aui.validation.message.required"]="This is a required field",AJS.I18n.keys["aui.validation.message.validnumber"]="Please enter a valid number",AJS.I18n.keys["aui.validation.message.min"]="Enter a value greater than {0}",AJS.I18n.keys["aui.validation.message.max"]="Enter a value less than {0}",AJS.I18n.keys["aui.validation.message.dateformat"]="Enter a valid date",AJS.I18n.keys["aui.validation.message.minchecked"]="Tick at least {0} checkboxes.",AJS.I18n.keys["aui.validation.message.maxchecked"]="Tick at most {0} checkboxes.",AJS._internal||(AJS._internal={}),function(a){AJS._internal.browser={};
var b=null;AJS._internal.browser.supportsCalc=function(){if(null===b){var c=a('<div style="height: 10px; height: -webkit-calc(20px + 0); height: calc(20px);"></div>');b=20===c.appendTo(document.documentElement).height(),c.remove()}return b}}(AJS.$),function(a){AJS._internal=AJS._internal||{},AJS._internal.widget=function(b,c){var d="_aui-widget-"+b;return function(b,e){var f,g;a.isPlainObject(b)?g=b:(f=b,g=e);var h,i=f&&a(f);return i&&i.data(d)?h=i.data(d):(h=new c(i,g||{}),i=h.$el,i.data(d,h)),h}}}(AJS.$),function(){function a(a,b){for(var c=a.length;c--;)if(b(a[c]))return c;return-1}function b(b,c){return a(b,function(a){return a[0]===c[0]})}function c(b){return a(b,function(a){return AJS.layer(a).isBlanketed()})}function d(a){var b;if(a.length){var c=a[a.length-1],d=parseInt(c.css("z-index"),10);b=(isNaN(d)?0:d)+100}else b=0;return Math.max(3e3,b)}function e(){this._stack=[]}e.prototype.push=function(a){if(b(this._stack,a)>=0)throw new Error("The given element is already an active layer");var e=AJS.layer(a),f=d(this._stack);e._showLayer(f),e.isBlanketed()&&(c(this._stack)>=0&&AJS.undim(),AJS.dim(!1,f-20)),this._stack.push(a)},e.prototype.popUntil=function(a){var d=b(this._stack,a);if(0>d)return null;var e=this._stack.slice(d);this._stack=this._stack.slice(0,d);var f=c(e);if(f>=0){AJS.undim();var g=c(this._stack);g>=0&&AJS.dim(!1,this._stack[g].css("z-index")-20)}for(var h;e.length;)h=e.pop(),AJS.layer(h)._hideLayer();return h},e.prototype.getTopLayer=function(){if(!this._stack.length)return null;var a=this._stack[this._stack.length-1];return a},e.prototype.popTopIfNonModal=function(){var a=this.getTopLayer();return!a||AJS.layer(a).isModal()?null:this.popUntil(a)},e.prototype.popUntilTopBlanketed=function(){var a=c(this._stack);if(0>a)return null;var b=this._stack[a];return AJS.layer(b).isModal()?null:this.popUntil(b)},AJS.LayerManager=e}(AJS.$),function(a){AJS.LayerManager.global=new AJS.LayerManager,a(document).on("keydown",function(a){if(a.keyCode===AJS.keyCode.ESCAPE){var b=AJS.LayerManager.global.popTopIfNonModal();b&&a.preventDefault()}}).on("click",".aui-blanket",function(a){var b=AJS.LayerManager.global.popUntilTopBlanketed();b&&a.preventDefault()})}(AJS.$),AJS.FocusManager=function(a){function b(){this._focusTrapStack=[],a(document).on("focusout",{focusTrapStack:this._focusTrapStack},f)}function c(a,b){b.push(a)}function d(a){a.pop()}function e(a){return a.is(".aui-dialog2")}function f(a){var b=a.data.focusTrapStack;if(a.relatedTarget&&0!==b.length){var c=b[b.length-1],d=a.target,e=a.relatedTarget,f=c.find(":aui-tabbable"),g=AJS.$(f.first()),h=AJS.$(f.last()),i=0===c.has(e).length,j=i&&e;j&&(g.is(d)?h.focus():h.is(d)&&g.focus())}}!function(){function b(b){return!a(b).parents().andSelf().filter(function(){return"hidden"===a.curCSS(this,"visibility")||a.expr.filters.hidden(this)}).length}function c(c,d){var e=c.nodeName.toLowerCase();if("area"===e){var f=c.parentNode,g=f.name,h=a("img[usemap=#"+g+"]").get();return c.href&&g&&"map"===f.nodeName.toLowerCase()?h&&b(h):!1}var i=/input|select|textarea|button|object/.test(e),j="a"===e,k=c.href||d;return(i?!c.disabled:j?k:d)&&b(c)}function d(b){var d=a.attr(b,"tabindex"),e=isNaN(d),f=e||d>=0;return f&&c(b,!e)}a.extend(a.expr[":"],{"aui-focusable":function(b){return c(b,!isNaN(a.attr(b,"tabindex")))},"aui-tabbable":d})}();var g="_aui-focus-restore";return b.defaultFocusSelector=":aui-tabbable",b.prototype.enter=function(d){if(d.data(g,a(document.activeElement)),"false"!==d.attr("data-aui-focus")){var f=d.attr("data-aui-focus-selector")||b.defaultFocusSelector,h=d.is(f)?d:d.find(f);h.first().focus()}e(d)&&c(d,this._focusTrapStack)},b.prototype.exit=function(b){e(b)&&d(this._focusTrapStack);var c=document.activeElement;(b[0]===c||b.has(c).length)&&a(c).blur();var f=b.data(g);f&&f.length&&(b.removeData(g),f.focus())},b.global=new b,b}(AJS.$),AJS=AJS||{},function(){var a="%CONTEXT_PATH%";a=0===a.indexOf("%CONTEXT_PATH")?!1:a,AJS.contextPath=function(){for(var b=null,c=[a,window.contextPath,window.Confluence&&Confluence.getContextPath(),window.BAMBOO&&BAMBOO.contextPath,window.FECRU&&FECRU.pageContext],d=0;d<c.length;d++)if("string"==typeof c[d]){b=c[d];break}return b}}(),function(){function a(a,b){b=b||"";var c=new RegExp(f(a)+"=([^|]+)"),d=b.match(c);return d&&d[1]}function b(a,b,c){var d=new RegExp("(\\s|\\|)*\\b"+f(a)+"=[^|]*[|]*");if(c=c||"",c=c.replace(d,"|"),""!==b){var e=a+"="+b;c.length+e.length<4020&&(c+="|"+e)}return c.replace(i,"|")}function c(a){return a.replace(h,"")}function d(a){var b=new RegExp("\\b"+f(a)+"=((?:[^\\\\;]+|\\\\.)*)(?:;|$)"),d=document.cookie.match(b);return d&&c(d[1])}function e(a,b,c){var d,e="",f='"'+b.replace(j,'\\"')+'"';c&&(d=new Date,d.setTime(+d+24*c*60*60*1e3),e="; expires="+d.toGMTString()),document.cookie=a+"="+f+e+";path=/"}function f(a){return a.replace(k,"\\$&")}var g="AJS.conglomerate.cookie",h=/(\\|^"|"$)/g,i=/\|\|+/g,j=/"/g,k=/[.*+?|^$()[\]{\\]/g;AJS.Cookie={save:function(a,c,f){var h=d(g);h=b(a,c,h),e(g,h,f||365)},read:function(b,c){var e=d(g),f=a(b,e);return null!=f?f:c},erase:function(a){this.save(a,"")}}}(),function(a){var b;AJS.dim=function(c,d){return b||(b=a(document.body)),c===!0&&AJS.log("DEPRECATED: useShim is calculated by dim() now"),AJS.dim.$dim||(AJS.dim.$dim=AJS("div").addClass("aui-blanket"),AJS.dim.$dim.attr("tabindex","0"),d&&AJS.dim.$dim.css({zIndex:d}),a.browser.msie&&AJS.dim.$dim.css({width:"200%",height:Math.max(a(document).height(),a(window).height())+"px"}),a("body").append(AJS.dim.$dim),(c||AJS.$.browser.msie&&Math.floor(AJS.$.browser.version)<9)&&(AJS.dim.$shim=a('<iframe frameBorder="0" class="aui-blanket-shim" tabindex="-1" src="about:blank"/>'),AJS.dim.$shim.css({height:Math.max(a(document).height(),a(window).height())+"px"}),d&&AJS.dim.$shim.css({zIndex:d-1}),a("body").append(AJS.dim.$shim),AJS.dim.shim=AJS.dim.$shim),AJS.dim.cachedOverflow=b.css("overflow"),b.css("overflow","hidden")),AJS.dim.$dim},AJS.undim=function(){if(AJS.dim.$dim&&(AJS.dim.$dim.remove(),AJS.dim.$dim=null,AJS.dim.$shim&&(AJS.dim.$shim.remove(),AJS.dim.$shim=null),b&&b.css("overflow",AJS.dim.cachedOverflow),a.browser.safari)){var c=a(window).scrollTop();a(window).scrollTop(10+5*(10==c)).scrollTop(c)}}}(AJS.$),function(a){function b(b){this.$el=a(b||'<div class="aui-layer" aria-hidden="true"></div>')}var c="_aui-internal-layer-",d="_aui-internal-layer-global-";b.prototype.changeSize=function(a,b){return this.$el.css("width",a),this.$el.css("height","content"===b?"":b),this},b.prototype.on=function(a,b){return this.$el.on(c+a,b),this},b.prototype.off=function(a,b){return this.$el.off(c+a,b),this},b.prototype.show=function(){if(this.$el.is(":visible"))return this;var b=AJS.$.Event(c+"beforeShow");this.$el.trigger(b);var e=AJS.$.Event(d+"beforeShow");return a(document).trigger(e,[this.$el]),b.isDefaultPrevented()||e.isDefaultPrevented()||(AJS.LayerManager.global.push(this.$el),this.$el.attr("data-aui-alignment")&&this.$el[0]._tether.enable()),this},b.prototype.hide=function(){if(!this.$el.is(":visible"))return this;var b=AJS.$.Event(c+"beforeHide");this.$el.trigger(b);var e=AJS.$.Event(d+"beforeHide");return a(document).trigger(e,[this.$el]),b.isDefaultPrevented()||e.isDefaultPrevented()||AJS.LayerManager.global.popUntil(this.$el),this},b.prototype.isVisible=function(){return"false"===this.$el.attr("aria-hidden")},b.prototype.remove=function(){this.hide(),this.$el[0]._tether&&this.$el[0]._tether.destroy(),this.$el.remove(),this.$el=null},b.prototype._showLayer=function(b){this.$el.parent().is("body")||this.$el.appendTo(document.body),this.$el.data("_aui-layer-cached-z-index",this.$el.css("z-index")),this.$el.css("z-index",b),this.$el.attr("aria-hidden","false"),AJS.FocusManager.global.enter(this.$el),this.$el.trigger(c+"show"),a(document).trigger(d+"show",[this.$el])},b.prototype._hideLayer=function(){AJS.FocusManager.global.exit(this.$el),this.$el.attr("aria-hidden","true"),this.$el.css("z-index",this.$el.data("_aui-layer-cached-z-index")||""),this.$el.data("_aui-layer-cached-z-index",""),this.$el.trigger(c+"hide"),a(document).trigger(d+"hide",[this.$el])},b.prototype.isBlanketed=function(){return"true"===this.$el.attr("data-aui-blanketed")},b.prototype.isModal=function(){return"true"===this.$el.attr("data-aui-modal")},AJS.layer=AJS._internal.widget("layer",b),AJS.layer.on=function(b,c){return a(document).on(d+b,c),this},AJS.layer.off=function(b,c){return a(document).off(d+b,c),this}}(AJS.$),AJS.popup=function(a){var b={width:800,height:600,closeOnOutsideClick:!1,keypressListener:function(a){27===a.keyCode&&c.is(":visible")&&i.hide()}};"object"!=typeof a&&(a={width:arguments[0],height:arguments[1],id:arguments[2]},a=AJS.$.extend({},a,arguments[3])),a=AJS.$.extend({},b,a);var c=AJS("div").addClass("aui-popup");a.id&&c.attr("id",a.id);var d=3e3;AJS.$(".aui-dialog").each(function(){var a=AJS.$(this);d=a.css("z-index")>d?a.css("z-index"):d});var e=function(b,e){return a.width=b=b||a.width,a.height=e=e||a.height,c.css({marginTop:-Math.round(e/2)+"px",marginLeft:-Math.round(b/2)+"px",width:b,height:e,"z-index":parseInt(d,10)+2}),arguments.callee}(a.width,a.height);AJS.$("body").append(c),c.hide(),AJS.enable(c);var f=AJS.$(".aui-blanket"),g=function(a,b){var c=AJS.$(a,b);return c.length?(c.focus(),!0):!1},h=function(b){if(0===AJS.$(".dialog-page-body",b).find(":focus").length){if(a.focusSelector)return g(a.focusSelector,b);var c=":input:visible:enabled:first";g(c,AJS.$(".dialog-page-body",b))||g(c,AJS.$(".dialog-button-panel",b))||g(c,AJS.$(".dialog-page-menu",b))}},i={changeSize:function(b,c){(b&&b!=a.width||c&&c!=a.height)&&e(b,c),this.show()},show:function(){var b=function(){AJS.$(document).off("keydown",a.keypressListener).on("keydown",a.keypressListener),AJS.dim(),f=AJS.$(".aui-blanket"),0!=f.size()&&a.closeOnOutsideClick&&f.click(function(){c.is(":visible")&&i.hide()}),c.show(),AJS.popup.current=this,h(c),AJS.$(document).trigger("showLayer",["popup",this])};b.call(this),this.show=b},hide:function(){AJS.$(document).unbind("keydown",a.keypressListener),f.unbind(),this.element.hide(),0==AJS.$(".aui-dialog:visible").size()&&AJS.undim();var b=document.activeElement;this.element.has(b).length&&b.blur(),AJS.$(document).trigger("hideLayer",["popup",this]),AJS.popup.current=null,this.enable()},element:c,remove:function(){c.remove(),this.element=null},disable:function(){this.disabled||(this.popupBlanket=AJS.$("<div class='dialog-blanket'> </div>").css({height:c.height(),width:c.width()}),c.append(this.popupBlanket),this.disabled=!0)},enable:function(){this.disabled&&(this.disabled=!1,this.popupBlanket.remove(),this.popupBlanket=null)}};return i},function(){function a(a,b,c,d){a.buttonpanel||a.addButtonPanel(),this.page=a,this.onclick=c,this._onclick=function(b){return c.call(this,a.dialog,a,b)===!0},this.item=AJS("button",b).addClass("button-panel-button"),d&&this.item.addClass(d),"function"==typeof c&&this.item.click(this._onclick),a.buttonpanel.append(this.item),this.id=a.button.length,a.button[this.id]=this}function b(a,b,c,d,e){a.buttonpanel||a.addButtonPanel(),e||(e="#"),this.page=a,this.onclick=c,this._onclick=function(b){return c.call(this,a.dialog,a,b)===!0},this.item=AJS("a",b).attr("href",e).addClass("button-panel-link"),d&&this.item.addClass(d),"function"==typeof c&&this.item.click(this._onclick),a.buttonpanel.append(this.item),this.id=a.button.length,a.button[this.id]=this}function c(a,b){var c="left"==a?-1:1;return function(a){var d=this.page[b];if(this.id!=(1==c?d.length-1:0)){c*=a||1,d[this.id+c].item[0>c?"before":"after"](this.item),d.splice(this.id,1),d.splice(this.id+c,0,this);for(var e=0,f=d.length;f>e;e++)"panel"==b&&this.page.curtab==d[e].id&&(this.page.curtab=e),d[e].id=e}return this}}function d(a){return function(){this.page[a].splice(this.id,1);for(var b=0,c=this.page[a].length;c>b;b++)this.page[a][b].id=b;this.item.remove()}}a.prototype.moveUp=a.prototype.moveLeft=c("left","button"),a.prototype.moveDown=a.prototype.moveRight=c("right","button"),a.prototype.remove=d("button"),a.prototype.html=function(a){return this.item.html(a)},a.prototype.onclick=function(a){return"undefined"==typeof a?this.onclick:(this.item.unbind("click",this._onclick),this._onclick=function(b){return a.call(this,page.dialog,page,b)===!0},"function"==typeof a&&this.item.click(this._onclick),void 0)};var e=20,f=function(a,b,c,d,f){c instanceof AJS.$||(c=AJS.$(c)),this.dialog=a.dialog,this.page=a,this.id=a.panel.length,this.button=AJS("button").html(b).addClass("item-button"),f&&(this.button[0].id=f),this.item=AJS("li").append(this.button).addClass("page-menu-item"),this.body=AJS("div").append(c).addClass("dialog-panel-body").css("height",a.dialog.height+"px"),this.padding=e,d&&this.body.addClass(d);var g=a.panel.length,h=this;a.menu.append(this.item),a.body.append(this.body),a.panel[g]=this;var i=function(){var b;a.curtab+1&&(b=a.panel[a.curtab],b.body.hide(),b.item.removeClass("selected"),"function"==typeof b.onblur&&b.onblur()),a.curtab=h.id,h.body.show(),h.item.addClass("selected"),"function"==typeof h.onselect&&h.onselect(),"function"==typeof a.ontabchange&&a.ontabchange(h,b)};this.button.click?this.button.click(i):(AJS.log("atlassian-dialog:Panel:constructor - this.button.click false"),this.button.onclick=i),i(),0==g?a.menu.css("display","none"):a.menu.show()};f.prototype.select=function(){this.button.click()},f.prototype.moveUp=f.prototype.moveLeft=c("left","panel"),f.prototype.moveDown=f.prototype.moveRight=c("right","panel"),f.prototype.remove=d("panel"),f.prototype.html=function(a){return a?(this.body.html(a),this):this.body.html()},f.prototype.setPadding=function(a){return isNaN(+a)||(this.body.css("padding",+a),this.padding=+a,this.page.recalcSize()),this};var g=56,h=51,i=50,j=function(a,b){this.dialog=a,this.id=a.page.length,this.element=AJS("div").addClass("dialog-components"),this.body=AJS("div").addClass("dialog-page-body"),this.menu=AJS("ul").addClass("dialog-page-menu").css("height",a.height+"px"),this.body.append(this.menu),this.curtab,this.panel=[],this.button=[],b&&this.body.addClass(b),a.popup.element.append(this.element.append(this.menu).append(this.body)),a.page[a.page.length]=this};j.prototype.recalcSize=function(){for(var a=this.header?g:0,b=this.buttonpanel?h:0,c=this.panel.length;c--;){var d=this.dialog.height-a-b;this.panel[c].body.css("height",d),this.menu.css("height",d)}},j.prototype.addButtonPanel=function(){this.buttonpanel=AJS("div").addClass("dialog-button-panel"),this.element.append(this.buttonpanel)},j.prototype.addPanel=function(a,b,c,d){return new f(this,a,b,c,d),this.recalcSize(),this},j.prototype.addHeader=function(a,b){return this.header&&this.header.remove(),this.header=AJS("h2").text(a||"").addClass("dialog-title"),b&&this.header.addClass(b),this.element.prepend(this.header),this.recalcSize(),this},j.prototype.addButton=function(b,c,d){return new a(this,b,c,d),this.recalcSize(),this},j.prototype.addLink=function(a,c,d,e){return new b(this,a,c,d,e),this.recalcSize(),this},j.prototype.gotoPanel=function(a){this.panel[a.id||a].select()},j.prototype.getCurrentPanel=function(){return this.panel[this.curtab]},j.prototype.hide=function(){this.element.hide()},j.prototype.show=function(){this.element.show()},j.prototype.remove=function(){this.element.remove()},AJS.Dialog=function(a,b,c){var d={};+a||(d=Object(a),a=d.width,b=d.height,c=d.id),this.height=b||480,this.width=a||640,this.id=c,d=AJS.$.extend({},d,{width:this.width,height:this.height,id:this.id}),this.popup=AJS.popup(d),this.popup.element.addClass("aui-dialog"),this.page=[],this.curpage=0,new j(this)},AJS.Dialog.prototype.addHeader=function(a,b){return this.page[this.curpage].addHeader(a,b),this},AJS.Dialog.prototype.addButton=function(a,b,c){return this.page[this.curpage].addButton(a,b,c),this},AJS.Dialog.prototype.addLink=function(a,b,c,d){return this.page[this.curpage].addLink(a,b,c,d),this},AJS.Dialog.prototype.addSubmit=function(a,b){return this.page[this.curpage].addButton(a,b,"button-panel-submit-button"),this},AJS.Dialog.prototype.addCancel=function(a,b){return this.page[this.curpage].addLink(a,b,"button-panel-cancel-link"),this},AJS.Dialog.prototype.addButtonPanel=function(){return this.page[this.curpage].addButtonPanel(),this},AJS.Dialog.prototype.addPanel=function(a,b,c,d){return this.page[this.curpage].addPanel(a,b,c,d),this},AJS.Dialog.prototype.addPage=function(a){return new j(this,a),this.page[this.curpage].hide(),this.curpage=this.page.length-1,this},AJS.Dialog.prototype.nextPage=function(){return this.page[this.curpage++].hide(),this.curpage>=this.page.length&&(this.curpage=0),this.page[this.curpage].show(),this},AJS.Dialog.prototype.prevPage=function(){return this.page[this.curpage--].hide(),this.curpage<0&&(this.curpage=this.page.length-1),this.page[this.curpage].show(),this},AJS.Dialog.prototype.gotoPage=function(a){return this.page[this.curpage].hide(),this.curpage=a,this.curpage<0?this.curpage=this.page.length-1:this.curpage>=this.page.length&&(this.curpage=0),this.page[this.curpage].show(),this},AJS.Dialog.prototype.getPanel=function(a,b){var c=null==b?this.curpage:a;return null==b&&(b=a),this.page[c].panel[b]},AJS.Dialog.prototype.getPage=function(a){return this.page[a]},AJS.Dialog.prototype.getCurrentPanel=function(){return this.page[this.curpage].getCurrentPanel()},AJS.Dialog.prototype.gotoPanel=function(a,b){if(null!=b){var c=a.id||a;this.gotoPage(c)}this.page[this.curpage].gotoPanel("undefined"==typeof b?a:b)},AJS.Dialog.prototype.show=function(){return this.popup.show(),AJS.trigger("show.dialog",{dialog:this}),this},AJS.Dialog.prototype.hide=function(){return this.popup.hide(),AJS.trigger("hide.dialog",{dialog:this}),this},AJS.Dialog.prototype.remove=function(){this.popup.hide(),this.popup.remove(),AJS.trigger("remove.dialog",{dialog:this})},AJS.Dialog.prototype.disable=function(){return this.popup.disable(),this},AJS.Dialog.prototype.enable=function(){return this.popup.enable(),this},AJS.Dialog.prototype.get=function(a){var b=[],c=this,d='#([^"][^ ]*|"[^"]*")',e=":(\\d+)",f="page|panel|button|header",g="(?:("+f+")(?:"+d+"|"+e+")?|"+d+")",h=new RegExp("(?:^|,)\\s*"+g+"(?:\\s+"+g+")?\\s*(?=,|$)","ig");(a+"").replace(h,function(a,d,e,f,g,h,i,j,k){d=d&&d.toLowerCase();var l=[];if("page"==d&&c.page[f]?(l.push(c.page[f]),d=h,d=d&&d.toLowerCase(),e=i,f=j,g=k):l=c.page,e=e&&(e+"").replace(/"/g,""),i=i&&(i+"").replace(/"/g,""),g=g&&(g+"").replace(/"/g,""),k=k&&(k+"").replace(/"/g,""),d||g)for(var m=l.length;m--;){if(g||"panel"==d&&(e||!e&&null==f))for(var n=l[m].panel.length;n--;)(l[m].panel[n].button.html()==g||l[m].panel[n].button.html()==e||"panel"==d&&!e&&null==f)&&b.push(l[m].panel[n]);if(g||"button"==d&&(e||!e&&null==f))for(var n=l[m].button.length;n--;)(l[m].button[n].item.html()==g||l[m].button[n].item.html()==e||"button"==d&&!e&&null==f)&&b.push(l[m].button[n]);l[m][d]&&l[m][d][f]&&b.push(l[m][d][f]),"header"==d&&l[m].header&&b.push(l[m].header)}else b=b.concat(l)});for(var i={length:b.length},j=b.length;j--;){i[j]=b[j];for(var k in b[j])k in i||!function(a){i[a]=function(){for(var b=this.length;b--;)"function"==typeof this[b][a]&&this[b][a].apply(this[b],arguments)}}(k)}return i},AJS.Dialog.prototype.updateHeight=function(){for(var a=0,b=AJS.$(window).height()-g-h-2*i,c=0;this.getPanel(c);c++)this.getPanel(c).body.css({height:"auto",display:"block"}).outerHeight()>a&&(a=Math.min(b,this.getPanel(c).body.outerHeight())),c!==this.page[this.curpage].curtab&&this.getPanel(c).body.css({display:"none"});for(c=0;this.getPanel(c);c++)this.getPanel(c).body.css({height:a||this.height});this.page[0].menu.height(a),this.height=a+g+h+1,this.popup.changeSize(void 0,this.height)},AJS.Dialog.prototype.isMaximised=function(){return this.popup.element.outerHeight()>=AJS.$(window).height()-2*i},AJS.Dialog.prototype.getCurPanel=function(){return this.getPanel(this.page[this.curpage].curtab)},AJS.Dialog.prototype.getCurPanelButton=function(){return this.getCurPanel().button}}(),function(a){function b(a){jQuery.each(e,function(b,c){var d="data-"+b;a[0].hasAttribute(d)||a.attr(d,c)})}function c(c){this.$el=a(c?c:AJS.parseHtml(a(aui.dialog.dialog2({})))),b(this.$el)}function d(b){AJS.layer.on(b,function(c,d){if(d.is(".aui-dialog2")){var e=AJS.$.Event(f+b);return a(document).trigger(e,[d]),!e.isDefaultPrevented()}return!0})}var e={"aui-focus":"false","aui-blanketed":"true"};c.prototype.on=function(a,b){return AJS.layer(this.$el).on(a,b),this},c.prototype.off=function(a,b){return AJS.layer(this.$el).off(a,b),this},c.prototype.show=function(){var a=AJS.layer(this.$el);return a.show(),this},c.prototype.hide=function(){return AJS.layer(this.$el).hide(),this},c.prototype.remove=function(){return AJS.layer(this.$el).remove(),this},AJS.dialog2=AJS._internal.widget("dialog2",c);for(var f="_aui-internal-dialog2-global-",g=["show","hide","beforeShow","beforeHide"],h=0;h<g.length;++h)d(g[h]);AJS.dialog2.on=function(b,c){return a(document).on(f+b,c),this},AJS.dialog2.off=function(b,c){return a(document).off(f+b,c),this},a(document).on("click",".aui-dialog2-header-close",function(b){b.preventDefault(),AJS.dialog2(a(this).closest(".aui-dialog2")).hide()}),AJS.dialog2.on("show",function(a,b){var c,d=[".aui-dialog2-content",".aui-dialog2-footer",".aui-dialog2-header"];d.some(function(a){return c=b.find(a+" :aui-tabbable"),c.length}),c&&c.first().focus()}),AJS.dialog2.on("hide",function(a,b){var c=AJS.layer(b);b.data("aui-remove-on-hide")&&c.remove()})}(AJS.$),function(a){"use strict";var b=0;AJS.DatePicker=function(c,d){var e,f,g,h;return e={},h=b++,g=a(c),g.attr("data-aui-dp-uuid",h),d=a.extend(void 0,AJS.DatePicker.prototype.defaultOptions,d),e.getField=function(){return g},e.getOptions=function(){return d},f=function(){var b,c,f,i,j,k,l,m,n,o;e.hide=function(){n.hide()},e.show=function(){n.show()},e.setDate=function(a){"undefined"!=typeof b&&b.datepicker("setDate",a)},e.getDate=function(){return"undefined"!=typeof b?b.datepicker("getDate"):void 0},k=function(c){if(o.off(),d.hint){var i=a("<div/>").addClass("aui-datepicker-hint");i.append("<span/>").text(d.hint),o.append(i)}b=a("<div/>"),b.attr("data-aui-dp-popup-uuid",h),o.append(b);var k={dateFormat:d.dateFormat,defaultDate:g.val(),maxDate:g.attr("max"),minDate:g.attr("min"),nextText:">",onSelect:function(a){g.val(a),g.change(),e.hide(),l=!0,g.focus(),d.onSelect&&d.onSelect.call(this,a)},onChangeMonthYear:function(){setTimeout(n.refresh,0)},prevText:"<"};a.extend(k,c),d.firstDay>-1&&(k.firstDay=d.firstDay),"undefined"!=typeof g.attr("step")&&AJS.log("WARNING: The AJS date picker polyfill currently does not support the step attribute!"),b.datepicker(k),g.on("focusout",f),g.on("propertychange keyup input paste",j)},c=function(b){var c=a(b.target);b.preventDefault(),c.closest(o).length||c.is(g)||c.closest(".ui-datepicker-header").length||e.hide()},f=function(){m||(a("body").on("focus blur click mousedown","*",c),m=!0)},i=function(){l?l=!1:e.show()},j=function(){var c=a(this).val();c&&(b.datepicker("setDate",g.val()),b.datepicker("option",{maxDate:g.attr("max"),minDate:g.attr("min")}))},e.destroyPolyfill=function(){e.hide(),g.attr("placeholder",null),g.off("propertychange keyup input paste",j),g.off("focus click",i),g.off("focusout",f),AJS.DatePicker.prototype.browserSupportsDateField&&(g[0].type="date"),"undefined"!=typeof b&&b.datepicker("destroy"),delete e.destroyPolyfill,delete e.show,delete e.hide},l=!1,m=!1,d.languageCode in AJS.DatePicker.prototype.localisations||(d.languageCode="");var p=AJS.DatePicker.prototype.localisations[d.languageCode],q="",r=240;"large"===p.size&&(r=325,q="aui-datepicker-dialog-large");var s={hideCallback:function(){a("body").off("focus blur click mousedown","*",c),m=!1},hideDelay:null,noBind:!0,persistent:!0,width:r};d.position&&(s.calculatePositions=function(b,c){var e=a(b[0]);return d.position.call(this,e,c)}),n=AJS.InlineDialog(g,void 0,function(a,c,d){"undefined"==typeof b&&(o=a,k(p)),d()},s),n.addClass("aui-datepicker-dialog"),n.addClass(q),g.on("focus click",i),g.attr("placeholder",d.dateFormat),d.overrideBrowserDefault&&AJS.DatePicker.prototype.browserSupportsDateField&&(g[0].type="text")},e.reset=function(){"function"==typeof e.destroyPolyfill&&e.destroyPolyfill(),(!AJS.DatePicker.prototype.browserSupportsDateField||d.overrideBrowserDefault)&&f()},e.reset(),e},AJS.DatePicker.prototype.browserSupportsDateField="date"===a('<input type="date" />')[0].type,AJS.DatePicker.prototype.defaultOptions={overrideBrowserDefault:!1,firstDay:-1,languageCode:AJS.$("html").attr("lang")||"en-AU",dateFormat:a.datepicker.W3C},AJS.DatePicker.prototype.localisations={"":{dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesMin:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],firstDay:0,isRTL:!1,monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],showMonthAfterYear:!1,yearSuffix:""},af:{dayNames:["Sondag","Maandag","Dinsdag","Woensdag","Donderdag","Vrydag","Saterdag"],dayNamesMin:["Son","Maa","Din","Woe","Don","Vry","Sat"],firstDay:1,isRTL:!1,monthNames:["Januarie","Februarie","Maart","April","Mei","Junie","Julie","Augustus","September","Oktober","November","Desember"],showMonthAfterYear:!1,yearSuffix:""},"ar-DZ":{dayNames:["\u0627\u0644\u0623\u062d\u062f","\u0627\u0644\u0627\u062b\u0646\u064a\u0646","\u0627\u0644\u062b\u0644\u0627\u062b\u0627\u0621","\u0627\u0644\u0623\u0631\u0628\u0639\u0627\u0621","\u0627\u0644\u062e\u0645\u064a\u0633","\u0627\u0644\u062c\u0645\u0639\u0629","\u0627\u0644\u0633\u0628\u062a"],dayNamesMin:["\u0627\u0644\u0623\u062d\u062f","\u0627\u0644\u0627\u062b\u0646\u064a\u0646","\u0627\u0644\u062b\u0644\u0627\u062b\u0627\u0621","\u0627\u0644\u0623\u0631\u0628\u0639\u0627\u0621","\u0627\u0644\u062e\u0645\u064a\u0633","\u0627\u0644\u062c\u0645\u0639\u0629","\u0627\u0644\u0633\u0628\u062a"],firstDay:6,isRTL:!0,monthNames:["\u062c\u0627\u0646\u0641\u064a","\u0641\u064a\u0641\u0631\u064a","\u0645\u0627\u0631\u0633","\u0623\u0641\u0631\u064a\u0644","\u0645\u0627\u064a","\u062c\u0648\u0627\u0646","\u062c\u0648\u064a\u0644\u064a\u0629","\u0623\u0648\u062a","\u0633\u0628\u062a\u0645\u0628\u0631","\u0623\u0643\u062a\u0648\u0628\u0631","\u0646\u0648\u0641\u0645\u0628\u0631","\u062f\u064a\u0633\u0645\u0628\u0631"],showMonthAfterYear:!1,yearSuffix:""},ar:{dayNames:["\u0627\u0644\u0623\u062d\u062f","\u0627\u0644\u0627\u062b\u0646\u064a\u0646","\u0627\u0644\u062b\u0644\u0627\u062b\u0627\u0621","\u0627\u0644\u0623\u0631\u0628\u0639\u0627\u0621","\u0627\u0644\u062e\u0645\u064a\u0633","\u0627\u0644\u062c\u0645\u0639\u0629","\u0627\u0644\u0633\u0628\u062a"],dayNamesMin:["\u0627\u0644\u0623\u062d\u062f","\u0627\u0644\u0627\u062b\u0646\u064a\u0646","\u0627\u0644\u062b\u0644\u0627\u062b\u0627\u0621","\u0627\u0644\u0623\u0631\u0628\u0639\u0627\u0621","\u0627\u0644\u062e\u0645\u064a\u0633","\u0627\u0644\u062c\u0645\u0639\u0629","\u0627\u0644\u0633\u0628\u062a"],firstDay:6,isRTL:!0,monthNames:["\u0643\u0627\u0646\u0648\u0646 \u0627\u0644\u062b\u0627\u0646\u064a","\u0634\u0628\u0627\u0637","\u0622\u0630\u0627\u0631","\u0646\u064a\u0633\u0627\u0646","\u0645\u0627\u064a\u0648","\u062d\u0632\u064a\u0631\u0627\u0646","\u062a\u0645\u0648\u0632","\u0622\u0628","\u0623\u064a\u0644\u0648\u0644","\u062a\u0634\u0631\u064a\u0646 \u0627\u0644\u0623\u0648\u0644","\u062a\u0634\u0631\u064a\u0646 \u0627\u0644\u062b\u0627\u0646\u064a","\u0643\u0627\u0646\u0648\u0646 \u0627\u0644\u0623\u0648\u0644"],showMonthAfterYear:!1,yearSuffix:""},az:{dayNames:["Bazar","Bazar ert\u0259si","\xc7\u0259r\u015f\u0259nb\u0259 ax\u015fam\u0131","\xc7\u0259r\u015f\u0259nb\u0259","C\xfcm\u0259 ax\u015fam\u0131","C\xfcm\u0259","\u015e\u0259nb\u0259"],dayNamesMin:["B","Be","\xc7a","\xc7","Ca","C","\u015e"],firstDay:1,isRTL:!1,monthNames:["Yanvar","Fevral","Mart","Aprel","May","\u0130yun","\u0130yul","Avqust","Sentyabr","Oktyabr","Noyabr","Dekabr"],showMonthAfterYear:!1,yearSuffix:""},bg:{dayNames:["\u041d\u0435\u0434\u0435\u043b\u044f","\u041f\u043e\u043d\u0435\u0434\u0435\u043b\u043d\u0438\u043a","\u0412\u0442\u043e\u0440\u043d\u0438\u043a","\u0421\u0440\u044f\u0434\u0430","\u0427\u0435\u0442\u0432\u044a\u0440\u0442\u044a\u043a","\u041f\u0435\u0442\u044a\u043a","\u0421\u044a\u0431\u043e\u0442\u0430"],dayNamesMin:["\u041d\u0435\u0434","\u041f\u043e\u043d","\u0412\u0442\u043e","\u0421\u0440\u044f","\u0427\u0435\u0442","\u041f\u0435\u0442","\u0421\u044a\u0431"],firstDay:1,isRTL:!1,monthNames:["\u042f\u043d\u0443\u0430\u0440\u0438","\u0424\u0435\u0432\u0440\u0443\u0430\u0440\u0438","\u041c\u0430\u0440\u0442","\u0410\u043f\u0440\u0438\u043b","\u041c\u0430\u0439","\u042e\u043d\u0438","\u042e\u043b\u0438","\u0410\u0432\u0433\u0443\u0441\u0442","\u0421\u0435\u043f\u0442\u0435\u043c\u0432\u0440\u0438","\u041e\u043a\u0442\u043e\u043c\u0432\u0440\u0438","\u041d\u043e\u0435\u043c\u0432\u0440\u0438","\u0414\u0435\u043a\u0435\u043c\u0432\u0440\u0438"],showMonthAfterYear:!1,yearSuffix:""},bs:{dayNames:["Nedelja","Ponedeljak","Utorak","Srijeda","\u010cetvrtak","Petak","Subota"],dayNamesMin:["Ned","Pon","Uto","Sri","\u010cet","Pet","Sub"],firstDay:1,isRTL:!1,monthNames:["Januar","Februar","Mart","April","Maj","Juni","Juli","August","Septembar","Oktobar","Novembar","Decembar"],showMonthAfterYear:!1,yearSuffix:""},ca:{dayNames:["Diumenge","Dilluns","Dimarts","Dimecres","Dijous","Divendres","Dissabte"],dayNamesMin:["Dug","Dln","Dmt","Dmc","Djs","Dvn","Dsb"],firstDay:1,isRTL:!1,monthNames:["Gener","Febrer","Mar&ccedil;","Abril","Maig","Juny","Juliol","Agost","Setembre","Octubre","Novembre","Desembre"],showMonthAfterYear:!1,yearSuffix:""},cs:{dayNames:["ned\u011ble","pond\u011bl\xed","\xfater\xfd","st\u0159eda","\u010dtvrtek","p\xe1tek","sobota"],dayNamesMin:["ne","po","\xfat","st","\u010dt","p\xe1","so"],firstDay:1,isRTL:!1,monthNames:["leden","\xfanor","b\u0159ezen","duben","kv\u011bten","\u010derven","\u010dervenec","srpen","z\xe1\u0159\xed","\u0159\xedjen","listopad","prosinec"],showMonthAfterYear:!1,yearSuffix:""},da:{dayNames:["S\xf8ndag","Mandag","Tirsdag","Onsdag","Torsdag","Fredag","L\xf8rdag"],dayNamesMin:["S\xf8n","Man","Tir","Ons","Tor","Fre","L\xf8r"],firstDay:1,isRTL:!1,monthNames:["Januar","Februar","Marts","April","Maj","Juni","Juli","August","September","Oktober","November","December"],showMonthAfterYear:!1,yearSuffix:""},de:{dayNames:["Sonntag","Montag","Dienstag","Mittwoch","Donnerstag","Freitag","Samstag"],dayNamesMin:["So","Mo","Di","Mi","Do","Fr","Sa"],firstDay:1,isRTL:!1,monthNames:["Januar","Februar","M\xe4rz","April","Mai","Juni","Juli","August","September","Oktober","November","Dezember"],showMonthAfterYear:!1,yearSuffix:""},el:{dayNames:["\u039a\u03c5\u03c1\u03b9\u03b1\u03ba\u03ae","\u0394\u03b5\u03c5\u03c4\u03ad\u03c1\u03b1","\u03a4\u03c1\u03af\u03c4\u03b7","\u03a4\u03b5\u03c4\u03ac\u03c1\u03c4\u03b7","\u03a0\u03ad\u03bc\u03c0\u03c4\u03b7","\u03a0\u03b1\u03c1\u03b1\u03c3\u03ba\u03b5\u03c5\u03ae","\u03a3\u03ac\u03b2\u03b2\u03b1\u03c4\u03bf"],dayNamesMin:["\u039a\u03c5\u03c1","\u0394\u03b5\u03c5","\u03a4\u03c1\u03b9","\u03a4\u03b5\u03c4","\u03a0\u03b5\u03bc","\u03a0\u03b1\u03c1","\u03a3\u03b1\u03b2"],firstDay:1,isRTL:!1,monthNames:["\u0399\u03b1\u03bd\u03bf\u03c5\u03ac\u03c1\u03b9\u03bf\u03c2","\u03a6\u03b5\u03b2\u03c1\u03bf\u03c5\u03ac\u03c1\u03b9\u03bf\u03c2","\u039c\u03ac\u03c1\u03c4\u03b9\u03bf\u03c2","\u0391\u03c0\u03c1\u03af\u03bb\u03b9\u03bf\u03c2","\u039c\u03ac\u03b9\u03bf\u03c2","\u0399\u03bf\u03cd\u03bd\u03b9\u03bf\u03c2","\u0399\u03bf\u03cd\u03bb\u03b9\u03bf\u03c2","\u0391\u03cd\u03b3\u03bf\u03c5\u03c3\u03c4\u03bf\u03c2","\u03a3\u03b5\u03c0\u03c4\u03ad\u03bc\u03b2\u03c1\u03b9\u03bf\u03c2","\u039f\u03ba\u03c4\u03ce\u03b2\u03c1\u03b9\u03bf\u03c2","\u039d\u03bf\u03ad\u03bc\u03b2\u03c1\u03b9\u03bf\u03c2","\u0394\u03b5\u03ba\u03ad\u03bc\u03b2\u03c1\u03b9\u03bf\u03c2"],showMonthAfterYear:!1,yearSuffix:""},"en-AU":{dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesMin:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],firstDay:1,isRTL:!1,monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],showMonthAfterYear:!1,yearSuffix:""},"en-GB":{dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesMin:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],firstDay:1,isRTL:!1,monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],showMonthAfterYear:!1,yearSuffix:""},"en-NZ":{dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesMin:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],firstDay:1,isRTL:!1,monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],showMonthAfterYear:!1,yearSuffix:""},eo:{dayNames:["Diman\u0109o","Lundo","Mardo","Merkredo","\u0134a\u016ddo","Vendredo","Sabato"],dayNamesMin:["Dim","Lun","Mar","Mer","\u0134a\u016d","Ven","Sab"],firstDay:0,isRTL:!1,monthNames:["Januaro","Februaro","Marto","Aprilo","Majo","Junio","Julio","A\u016dgusto","Septembro","Oktobro","Novembro","Decembro"],showMonthAfterYear:!1,yearSuffix:""},es:{dayNames:["Domingo","Lunes","Martes","Mi&eacute;rcoles","Jueves","Viernes","S&aacute;bado"],dayNamesMin:["Dom","Lun","Mar","Mi&eacute;","Juv","Vie","S&aacute;b"],firstDay:1,isRTL:!1,monthNames:["Enero","Febrero","Marzo","Abril","Mayo","Junio","Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"],showMonthAfterYear:!1,yearSuffix:""},et:{dayNames:["P\xfchap\xe4ev","Esmasp\xe4ev","Teisip\xe4ev","Kolmap\xe4ev","Neljap\xe4ev","Reede","Laup\xe4ev"],dayNamesMin:["P\xfchap","Esmasp","Teisip","Kolmap","Neljap","Reede","Laup"],firstDay:1,isRTL:!1,monthNames:["Jaanuar","Veebruar","M\xe4rts","Aprill","Mai","Juuni","Juuli","August","September","Oktoober","November","Detsember"],showMonthAfterYear:!1,yearSuffix:"",size:"large"},eu:{dayNames:["Igandea","Astelehena","Asteartea","Asteazkena","Osteguna","Ostirala","Larunbata"],dayNamesMin:["Iga","Ast","Ast","Ast","Ost","Ost","Lar"],firstDay:1,isRTL:!1,monthNames:["Urtarrila","Otsaila","Martxoa","Apirila","Maiatza","Ekaina","Uztaila","Abuztua","Iraila","Urria","Azaroa","Abendua"],showMonthAfterYear:!1,yearSuffix:""},fa:{dayNames:["\u064a\u06a9\u0634\u0646\u0628\u0647","\u062f\u0648\u0634\u0646\u0628\u0647","\u0633\u0647\u0634\u0646\u0628\u0647","\u0686\u0647\u0627\u0631\u0634\u0646\u0628\u0647","\u067e\u0646\u062c\u0634\u0646\u0628\u0647","\u062c\u0645\u0639\u0647","\u0634\u0646\u0628\u0647"],dayNamesMin:["\u064a","\u062f","\u0633","\u0686","\u067e","\u062c","\u0634"],firstDay:6,isRTL:!0,monthNames:["\u0641\u0631\u0648\u0631\u062f\u064a\u0646","\u0627\u0631\u062f\u064a\u0628\u0647\u0634\u062a","\u062e\u0631\u062f\u0627\u062f","\u062a\u064a\u0631","\u0645\u0631\u062f\u0627\u062f","\u0634\u0647\u0631\u064a\u0648\u0631","\u0645\u0647\u0631","\u0622\u0628\u0627\u0646","\u0622\u0630\u0631","\u062f\u064a","\u0628\u0647\u0645\u0646","\u0627\u0633\u0641\u0646\u062f"],showMonthAfterYear:!1,yearSuffix:""},fi:{dayNames:["Sunnuntai","Maanantai","Tiistai","Keskiviikko","Torstai","Perjantai","Lauantai"],dayNamesMin:["Su","Ma","Ti","Ke","To","Pe","Su"],firstDay:1,isRTL:!1,monthNames:["Tammikuu","Helmikuu","Maaliskuu","Huhtikuu","Toukokuu","Kes&auml;kuu","Hein&auml;kuu","Elokuu","Syyskuu","Lokakuu","Marraskuu","Joulukuu"],showMonthAfterYear:!1,yearSuffix:""},fo:{dayNames:["Sunnudagur","M\xe1nadagur","T\xfdsdagur","Mikudagur","H\xf3sdagur","Fr\xedggjadagur","Leyardagur"],dayNamesMin:["Sun","M\xe1n","T\xfds","Mik","H\xf3s","Fr\xed","Ley"],firstDay:0,isRTL:!1,monthNames:["Januar","Februar","Mars","Apr\xedl","Mei","Juni","Juli","August","September","Oktober","November","Desember"],showMonthAfterYear:!1,yearSuffix:""},"fr-CH":{dayNames:["Dimanche","Lundi","Mardi","Mercredi","Jeudi","Vendredi","Samedi"],dayNamesMin:["Dim","Lun","Mar","Mer","Jeu","Ven","Sam"],firstDay:1,isRTL:!1,monthNames:["Janvier","F\xe9vrier","Mars","Avril","Mai","Juin","Juillet","Ao\xfbt","Septembre","Octobre","Novembre","D\xe9cembre"],showMonthAfterYear:!1,yearSuffix:""},fr:{dayNames:["Dimanche","Lundi","Mardi","Mercredi","Jeudi","Vendredi","Samedi"],dayNamesMin:["Dim.","Lun.","Mar.","Mer.","Jeu.","Ven.","Sam."],firstDay:1,isRTL:!1,monthNames:["Janvier","F\xe9vrier","Mars","Avril","Mai","Juin","Juillet","Ao\xfbt","Septembre","Octobre","Novembre","D\xe9cembre"],showMonthAfterYear:!1,yearSuffix:""},gl:{dayNames:["Domingo","Luns","Martes","M&eacute;rcores","Xoves","Venres","S&aacute;bado"],dayNamesMin:["Dom","Lun","Mar","M&eacute;r","Xov","Ven","S&aacute;b"],firstDay:1,isRTL:!1,monthNames:["Xaneiro","Febreiro","Marzo","Abril","Maio","Xu\xf1o","Xullo","Agosto","Setembro","Outubro","Novembro","Decembro"],showMonthAfterYear:!1,yearSuffix:""},he:{dayNames:["\u05e8\u05d0\u05e9\u05d5\u05df","\u05e9\u05e0\u05d9","\u05e9\u05dc\u05d9\u05e9\u05d9","\u05e8\u05d1\u05d9\u05e2\u05d9","\u05d7\u05de\u05d9\u05e9\u05d9","\u05e9\u05d9\u05e9\u05d9","\u05e9\u05d1\u05ea"],dayNamesMin:["\u05d0'","\u05d1'","\u05d2'","\u05d3'","\u05d4'","\u05d5'","\u05e9\u05d1\u05ea"],firstDay:0,isRTL:!0,monthNames:["\u05d9\u05e0\u05d5\u05d0\u05e8","\u05e4\u05d1\u05e8\u05d5\u05d0\u05e8","\u05de\u05e8\u05e5","\u05d0\u05e4\u05e8\u05d9\u05dc","\u05de\u05d0\u05d9","\u05d9\u05d5\u05e0\u05d9","\u05d9\u05d5\u05dc\u05d9","\u05d0\u05d5\u05d2\u05d5\u05e1\u05d8","\u05e1\u05e4\u05d8\u05de\u05d1\u05e8","\u05d0\u05d5\u05e7\u05d8\u05d5\u05d1\u05e8","\u05e0\u05d5\u05d1\u05de\u05d1\u05e8","\u05d3\u05e6\u05de\u05d1\u05e8"],showMonthAfterYear:!1,yearSuffix:""},hr:{dayNames:["Nedjelja","Ponedjeljak","Utorak","Srijeda","\u010cetvrtak","Petak","Subota"],dayNamesMin:["Ned","Pon","Uto","Sri","\u010cet","Pet","Sub"],firstDay:1,isRTL:!1,monthNames:["Sije\u010danj","Velja\u010da","O\u017eujak","Travanj","Svibanj","Lipanj","Srpanj","Kolovoz","Rujan","Listopad","Studeni","Prosinac"],showMonthAfterYear:!1,yearSuffix:""},hu:{dayNames:["Vas\xe1rnap","H\xe9tf\xf6","Kedd","Szerda","Cs\xfct\xf6rt\xf6k","P\xe9ntek","Szombat"],dayNamesMin:["Vas","H\xe9t","Ked","Sze","Cs\xfc","P\xe9n","Szo"],firstDay:1,isRTL:!1,monthNames:["Janu\xe1r","Febru\xe1r","M\xe1rcius","\xc1prilis","M\xe1jus","J\xfanius","J\xfalius","Augusztus","Szeptember","Okt\xf3ber","November","December"],showMonthAfterYear:!0,yearSuffix:""},hy:{dayNames:["\u056f\u056b\u0580\u0561\u056f\u056b","\u0565\u056f\u0578\u0582\u0577\u0561\u0562\u0569\u056b","\u0565\u0580\u0565\u0584\u0577\u0561\u0562\u0569\u056b","\u0579\u0578\u0580\u0565\u0584\u0577\u0561\u0562\u0569\u056b","\u0570\u056b\u0576\u0563\u0577\u0561\u0562\u0569\u056b","\u0578\u0582\u0580\u0562\u0561\u0569","\u0577\u0561\u0562\u0561\u0569"],dayNamesMin:["\u056f\u056b\u0580","\u0565\u0580\u056f","\u0565\u0580\u0584","\u0579\u0580\u0584","\u0570\u0576\u0563","\u0578\u0582\u0580\u0562","\u0577\u0562\u0569"],firstDay:1,isRTL:!1,monthNames:["\u0540\u0578\u0582\u0576\u057e\u0561\u0580","\u0553\u0565\u057f\u0580\u057e\u0561\u0580","\u0544\u0561\u0580\u057f","\u0531\u057a\u0580\u056b\u056c","\u0544\u0561\u0575\u056b\u057d","\u0540\u0578\u0582\u0576\u056b\u057d","\u0540\u0578\u0582\u056c\u056b\u057d","\u0555\u0563\u0578\u057d\u057f\u0578\u057d","\u054d\u0565\u057a\u057f\u0565\u0574\u0562\u0565\u0580","\u0540\u0578\u056f\u057f\u0565\u0574\u0562\u0565\u0580","\u0546\u0578\u0575\u0565\u0574\u0562\u0565\u0580","\u0534\u0565\u056f\u057f\u0565\u0574\u0562\u0565\u0580"],showMonthAfterYear:!1,yearSuffix:""},id:{dayNames:["Minggu","Senin","Selasa","Rabu","Kamis","Jumat","Sabtu"],dayNamesMin:["Min","Sen","Sel","Rab","kam","Jum","Sab"],firstDay:0,isRTL:!1,monthNames:["Januari","Februari","Maret","April","Mei","Juni","Juli","Agustus","September","Oktober","Nopember","Desember"],showMonthAfterYear:!1,yearSuffix:""},is:{dayNames:["Sunnudagur","M&aacute;nudagur","&THORN;ri&eth;judagur","Mi&eth;vikudagur","Fimmtudagur","F&ouml;studagur","Laugardagur"],dayNamesMin:["Sun","M&aacute;n","&THORN;ri","Mi&eth;","Fim","F&ouml;s","Lau"],firstDay:0,isRTL:!1,monthNames:["Jan&uacute;ar","Febr&uacute;ar","Mars","Apr&iacute;l","Ma&iacute","J&uacute;n&iacute;","J&uacute;l&iacute;","&Aacute;g&uacute;st","September","Okt&oacute;ber","N&oacute;vember","Desember"],showMonthAfterYear:!1,yearSuffix:""},it:{dayNames:["Domenica","Luned&#236","Marted&#236","Mercoled&#236","Gioved&#236","Venerd&#236","Sabato"],dayNamesMin:["Dom","Lun","Mar","Mer","Gio","Ven","Sab"],firstDay:1,isRTL:!1,monthNames:["Gennaio","Febbraio","Marzo","Aprile","Maggio","Giugno","Luglio","Agosto","Settembre","Ottobre","Novembre","Dicembre"],showMonthAfterYear:!1,yearSuffix:""},ja:{dayNames:["\u65e5\u66dc\u65e5","\u6708\u66dc\u65e5","\u706b\u66dc\u65e5","\u6c34\u66dc\u65e5","\u6728\u66dc\u65e5","\u91d1\u66dc\u65e5","\u571f\u66dc\u65e5"],dayNamesMin:["\u65e5","\u6708","\u706b","\u6c34","\u6728","\u91d1","\u571f"],firstDay:0,isRTL:!1,monthNames:["1\u6708","2\u6708","3\u6708","4\u6708","5\u6708","6\u6708","7\u6708","8\u6708","9\u6708","10\u6708","11\u6708","12\u6708"],showMonthAfterYear:!0,yearSuffix:"\u5e74"},ko:{dayNames:["\uc77c","\uc6d4","\ud654","\uc218","\ubaa9","\uae08","\ud1a0"],dayNamesMin:["\uc77c","\uc6d4","\ud654","\uc218","\ubaa9","\uae08","\ud1a0"],firstDay:0,isRTL:!1,monthNames:["1\uc6d4(JAN)","2\uc6d4(FEB)","3\uc6d4(MAR)","4\uc6d4(APR)","5\uc6d4(MAY)","6\uc6d4(JUN)","7\uc6d4(JUL)","8\uc6d4(AUG)","9\uc6d4(SEP)","10\uc6d4(OCT)","11\uc6d4(NOV)","12\uc6d4(DEC)"],showMonthAfterYear:!1,yearSuffix:"\ub144"},kz:{dayNames:["\u0416\u0435\u043a\u0441\u0435\u043d\u0431\u0456","\u0414\u04af\u0439\u0441\u0435\u043d\u0431\u0456","\u0421\u0435\u0439\u0441\u0435\u043d\u0431\u0456","\u0421\u04d9\u0440\u0441\u0435\u043d\u0431\u0456","\u0411\u0435\u0439\u0441\u0435\u043d\u0431\u0456","\u0416\u04b1\u043c\u0430","\u0421\u0435\u043d\u0431\u0456"],dayNamesMin:["\u0436\u043a\u0441","\u0434\u0441\u043d","\u0441\u0441\u043d","\u0441\u0440\u0441","\u0431\u0441\u043d","\u0436\u043c\u0430","\u0441\u043d\u0431"],firstDay:1,isRTL:!1,monthNames:["\u049a\u0430\u04a3\u0442\u0430\u0440","\u0410\u049b\u043f\u0430\u043d","\u041d\u0430\u0443\u0440\u044b\u0437","\u0421\u04d9\u0443\u0456\u0440","\u041c\u0430\u043c\u044b\u0440","\u041c\u0430\u0443\u0441\u044b\u043c","\u0428\u0456\u043b\u0434\u0435","\u0422\u0430\u043c\u044b\u0437","\u049a\u044b\u0440\u043a\u04af\u0439\u0435\u043a","\u049a\u0430\u0437\u0430\u043d","\u049a\u0430\u0440\u0430\u0448\u0430","\u0416\u0435\u043b\u0442\u043e\u049b\u0441\u0430\u043d"],showMonthAfterYear:!1,yearSuffix:""},lt:{dayNames:["sekmadienis","pirmadienis","antradienis","tre\u010diadienis","ketvirtadienis","penktadienis","\u0161e\u0161tadienis"],dayNamesMin:["sek","pir","ant","tre","ket","pen","\u0161e\u0161"],firstDay:1,isRTL:!1,monthNames:["Sausis","Vasaris","Kovas","Balandis","Gegu\u017e\u0117","Bir\u017eelis","Liepa","Rugpj\u016btis","Rugs\u0117jis","Spalis","Lapkritis","Gruodis"],showMonthAfterYear:!1,yearSuffix:""},lv:{dayNames:["sv\u0113tdiena","pirmdiena","otrdiena","tre\u0161diena","ceturtdiena","piektdiena","sestdiena"],dayNamesMin:["svt","prm","otr","tre","ctr","pkt","sst"],firstDay:1,isRTL:!1,monthNames:["Janv\u0101ris","Febru\u0101ris","Marts","Apr\u012blis","Maijs","J\u016bnijs","J\u016blijs","Augusts","Septembris","Oktobris","Novembris","Decembris"],showMonthAfterYear:!1,yearSuffix:""},ml:{dayNames:["\u0d1e\u0d3e\u0d2f\u0d30\u0d4d","\u0d24\u0d3f\u0d19\u0d4d\u0d15\u0d33\u0d4d","\u0d1a\u0d4a\u0d35\u0d4d\u0d35","\u0d2c\u0d41\u0d27\u0d28\u0d4d","\u0d35\u0d4d\u0d2f\u0d3e\u0d34\u0d02","\u0d35\u0d46\u0d33\u0d4d\u0d33\u0d3f","\u0d36\u0d28\u0d3f"],dayNamesMin:["\u0d1e\u0d3e\u0d2f","\u0d24\u0d3f\u0d19\u0d4d\u0d15","\u0d1a\u0d4a\u0d35\u0d4d\u0d35","\u0d2c\u0d41\u0d27","\u0d35\u0d4d\u0d2f\u0d3e\u0d34\u0d02","\u0d35\u0d46\u0d33\u0d4d\u0d33\u0d3f","\u0d36\u0d28\u0d3f"],firstDay:1,isRTL:!1,monthNames:["\u0d1c\u0d28\u0d41\u0d35\u0d30\u0d3f","\u0d2b\u0d46\u0d2c\u0d4d\u0d30\u0d41\u0d35\u0d30\u0d3f","\u0d2e\u0d3e\u0d30\u0d4d\u0d1a\u0d4d\u0d1a\u0d4d","\u0d0f\u0d2a\u0d4d\u0d30\u0d3f\u0d32\u0d4d","\u0d2e\u0d47\u0d2f\u0d4d","\u0d1c\u0d42\u0d23\u0d4d","\u0d1c\u0d42\u0d32\u0d48","\u0d06\u0d17\u0d38\u0d4d\u0d31\u0d4d\u0d31\u0d4d","\u0d38\u0d46\u0d2a\u0d4d\u0d31\u0d4d\u0d31\u0d02\u0d2c\u0d30\u0d4d","\u0d12\u0d15\u0d4d\u0d1f\u0d4b\u0d2c\u0d30\u0d4d","\u0d28\u0d35\u0d02\u0d2c\u0d30\u0d4d","\u0d21\u0d3f\u0d38\u0d02\u0d2c\u0d30\u0d4d"],showMonthAfterYear:!1,yearSuffix:""},ms:{dayNames:["Ahad","Isnin","Selasa","Rabu","Khamis","Jumaat","Sabtu"],dayNamesMin:["Aha","Isn","Sel","Rab","kha","Jum","Sab"],firstDay:0,isRTL:!1,monthNames:["Januari","Februari","Mac","April","Mei","Jun","Julai","Ogos","September","Oktober","November","Disember"],showMonthAfterYear:!1,yearSuffix:""},nl:{dayNames:["zondag","maandag","dinsdag","woensdag","donderdag","vrijdag","zaterdag"],dayNamesMin:["zon","maa","din","woe","don","vri","zat"],firstDay:1,isRTL:!1,monthNames:["januari","februari","maart","april","mei","juni","juli","augustus","september","oktober","november","december"],showMonthAfterYear:!1,yearSuffix:""},no:{dayNames:["s\xf8ndag","mandag","tirsdag","onsdag","torsdag","fredag","l\xf8rdag"],dayNamesMin:["s\xf8n","man","tir","ons","tor","fre","l\xf8r"],firstDay:1,isRTL:!1,monthNames:["januar","februar","mars","april","mai","juni","juli","august","september","oktober","november","desember"],showMonthAfterYear:!1,yearSuffix:""},pl:{dayNames:["Niedziela","Poniedzia\u0142ek","Wtorek","\u015aroda","Czwartek","Pi\u0105tek","Sobota"],dayNamesMin:["Nie","Pn","Wt","\u015ar","Czw","Pt","So"],firstDay:1,isRTL:!1,monthNames:["Stycze\u0144","Luty","Marzec","Kwiecie\u0144","Maj","Czerwiec","Lipiec","Sierpie\u0144","Wrzesie\u0144","Pa\u017adziernik","Listopad","Grudzie\u0144"],showMonthAfterYear:!1,yearSuffix:""},"pt-BR":{dayNames:["Domingo","Segunda-feira","Ter&ccedil;a-feira","Quarta-feira","Quinta-feira","Sexta-feira","S&aacute;bado"],dayNamesMin:["Dom","Seg","Ter","Qua","Qui","Sex","S&aacute;b"],firstDay:0,isRTL:!1,monthNames:["Janeiro","Fevereiro","Mar&ccedil;o","Abril","Maio","Junho","Julho","Agosto","Setembro","Outubro","Novembro","Dezembro"],showMonthAfterYear:!1,yearSuffix:""},pt:{dayNames:["Domingo","Segunda-feira","Ter&ccedil;a-feira","Quarta-feira","Quinta-feira","Sexta-feira","S&aacute;bado"],dayNamesMin:["Dom","Seg","Ter","Qua","Qui","Sex","S&aacute;b"],firstDay:0,isRTL:!1,monthNames:["Janeiro","Fevereiro","Mar&ccedil;o","Abril","Maio","Junho","Julho","Agosto","Setembro","Outubro","Novembro","Dezembro"],showMonthAfterYear:!1,yearSuffix:""},rm:{dayNames:["Dumengia","Glindesdi","Mardi","Mesemna","Gievgia","Venderdi","Sonda"],dayNamesMin:["Dum","Gli","Mar","Mes","Gie","Ven","Som"],firstDay:1,isRTL:!1,monthNames:["Schaner","Favrer","Mars","Avrigl","Matg","Zercladur","Fanadur","Avust","Settember","October","November","December"],showMonthAfterYear:!1,yearSuffix:""},ro:{dayNames:["Duminic\u0103","Luni","Mar\u0163i","Miercuri","Joi","Vineri","S\xe2mb\u0103t\u0103"],dayNamesMin:["Dum","Lun","Mar","Mie","Joi","Vin","S\xe2m"],firstDay:1,isRTL:!1,monthNames:["Ianuarie","Februarie","Martie","Aprilie","Mai","Iunie","Iulie","August","Septembrie","Octombrie","Noiembrie","Decembrie"],showMonthAfterYear:!1,yearSuffix:""},ru:{dayNames:["\u0432\u043e\u0441\u043a\u0440\u0435\u0441\u0435\u043d\u044c\u0435","\u043f\u043e\u043d\u0435\u0434\u0435\u043b\u044c\u043d\u0438\u043a","\u0432\u0442\u043e\u0440\u043d\u0438\u043a","\u0441\u0440\u0435\u0434\u0430","\u0447\u0435\u0442\u0432\u0435\u0440\u0433","\u043f\u044f\u0442\u043d\u0438\u0446\u0430","\u0441\u0443\u0431\u0431\u043e\u0442\u0430"],dayNamesMin:["\u0432\u0441\u043a","\u043f\u043d\u0434","\u0432\u0442\u0440","\u0441\u0440\u0434","\u0447\u0442\u0432","\u043f\u0442\u043d","\u0441\u0431\u0442"],firstDay:1,isRTL:!1,monthNames:["\u042f\u043d\u0432\u0430\u0440\u044c","\u0424\u0435\u0432\u0440\u0430\u043b\u044c","\u041c\u0430\u0440\u0442","\u0410\u043f\u0440\u0435\u043b\u044c","\u041c\u0430\u0439","\u0418\u044e\u043d\u044c","\u0418\u044e\u043b\u044c","\u0410\u0432\u0433\u0443\u0441\u0442","\u0421\u0435\u043d\u0442\u044f\u0431\u0440\u044c","\u041e\u043a\u0442\u044f\u0431\u0440\u044c","\u041d\u043e\u044f\u0431\u0440\u044c","\u0414\u0435\u043a\u0430\u0431\u0440\u044c"],showMonthAfterYear:!1,yearSuffix:""},sk:{dayNames:["Nede\u013ea","Pondelok","Utorok","Streda","\u0160tvrtok","Piatok","Sobota"],dayNamesMin:["Ned","Pon","Uto","Str","\u0160tv","Pia","Sob"],firstDay:1,isRTL:!1,monthNames:["Janu\xe1r","Febru\xe1r","Marec","Apr\xedl","M\xe1j","J\xfan","J\xfal","August","September","Okt\xf3ber","November","December"],showMonthAfterYear:!1,yearSuffix:""},sl:{dayNames:["Nedelja","Ponedeljek","Torek","Sreda","&#x10C;etrtek","Petek","Sobota"],dayNamesMin:["Ned","Pon","Tor","Sre","&#x10C;et","Pet","Sob"],firstDay:1,isRTL:!1,monthNames:["Januar","Februar","Marec","April","Maj","Junij","Julij","Avgust","September","Oktober","November","December"],showMonthAfterYear:!1,yearSuffix:""},sq:{dayNames:["E Diel","E H\xebn\xeb","E Mart\xeb","E M\xebrkur\xeb","E Enjte","E Premte","E Shtune"],dayNamesMin:["Di","H\xeb","Ma","M\xeb","En","Pr","Sh"],firstDay:1,isRTL:!1,monthNames:["Janar","Shkurt","Mars","Prill","Maj","Qershor","Korrik","Gusht","Shtator","Tetor","N\xebntor","Dhjetor"],showMonthAfterYear:!1,yearSuffix:""},"sr-SR":{dayNames:["Nedelja","Ponedeljak","Utorak","Sreda","\u010cetvrtak","Petak","Subota"],dayNamesMin:["Ned","Pon","Uto","Sre","\u010cet","Pet","Sub"],firstDay:1,isRTL:!1,monthNames:["Januar","Februar","Mart","April","Maj","Jun","Jul","Avgust","Septembar","Oktobar","Novembar","Decembar"],showMonthAfterYear:!1,yearSuffix:""},sr:{dayNames:["\u041d\u0435\u0434\u0435\u0459\u0430","\u041f\u043e\u043d\u0435\u0434\u0435\u0459\u0430\u043a","\u0423\u0442\u043e\u0440\u0430\u043a","\u0421\u0440\u0435\u0434\u0430","\u0427\u0435\u0442\u0432\u0440\u0442\u0430\u043a","\u041f\u0435\u0442\u0430\u043a","\u0421\u0443\u0431\u043e\u0442\u0430"],dayNamesMin:["\u041d\u0435\u0434","\u041f\u043e\u043d","\u0423\u0442\u043e","\u0421\u0440\u0435","\u0427\u0435\u0442","\u041f\u0435\u0442","\u0421\u0443\u0431"],firstDay:1,isRTL:!1,monthNames:["\u0408\u0430\u043d\u0443\u0430\u0440","\u0424\u0435\u0431\u0440\u0443\u0430\u0440","\u041c\u0430\u0440\u0442","\u0410\u043f\u0440\u0438\u043b","\u041c\u0430\u0458","\u0408\u0443\u043d","\u0408\u0443\u043b","\u0410\u0432\u0433\u0443\u0441\u0442","\u0421\u0435\u043f\u0442\u0435\u043c\u0431\u0430\u0440","\u041e\u043a\u0442\u043e\u0431\u0430\u0440","\u041d\u043e\u0432\u0435\u043c\u0431\u0430\u0440","\u0414\u0435\u0446\u0435\u043c\u0431\u0430\u0440"],showMonthAfterYear:!1,yearSuffix:""},sv:{dayNames:["S\xf6ndag","M\xe5ndag","Tisdag","Onsdag","Torsdag","Fredag","L\xf6rdag"],dayNamesMin:["S\xf6n","M\xe5n","Tis","Ons","Tor","Fre","L\xf6r"],firstDay:1,isRTL:!1,monthNames:["Januari","Februari","Mars","April","Maj","Juni","Juli","Augusti","September","Oktober","November","December"],showMonthAfterYear:!1,yearSuffix:""},ta:{dayNames:["\u0b9e\u0bbe\u0baf\u0bbf\u0bb1\u0bcd\u0bb1\u0bc1\u0b95\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8","\u0ba4\u0bbf\u0b99\u0bcd\u0b95\u0b9f\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8","\u0b9a\u0bc6\u0bb5\u0bcd\u0bb5\u0bbe\u0baf\u0bcd\u0b95\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8","\u0baa\u0bc1\u0ba4\u0ba9\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8","\u0bb5\u0bbf\u0baf\u0bbe\u0bb4\u0b95\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8","\u0bb5\u0bc6\u0bb3\u0bcd\u0bb3\u0bbf\u0b95\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8","\u0b9a\u0ba9\u0bbf\u0b95\u0bcd\u0b95\u0bbf\u0bb4\u0bae\u0bc8"],dayNamesMin:["\u0b9e\u0bbe\u0baf\u0bbf\u0bb1\u0bc1","\u0ba4\u0bbf\u0b99\u0bcd\u0b95\u0bb3\u0bcd","\u0b9a\u0bc6\u0bb5\u0bcd\u0bb5\u0bbe\u0baf\u0bcd","\u0baa\u0bc1\u0ba4\u0ba9\u0bcd","\u0bb5\u0bbf\u0baf\u0bbe\u0bb4\u0ba9\u0bcd","\u0bb5\u0bc6\u0bb3\u0bcd\u0bb3\u0bbf","\u0b9a\u0ba9\u0bbf"],firstDay:1,isRTL:!1,monthNames:["\u0ba4\u0bc8","\u0bae\u0bbe\u0b9a\u0bbf","\u0baa\u0b99\u0bcd\u0b95\u0bc1\u0ba9\u0bbf","\u0b9a\u0bbf\u0ba4\u0bcd\u0ba4\u0bbf\u0bb0\u0bc8","\u0bb5\u0bc8\u0b95\u0bbe\u0b9a\u0bbf","\u0b86\u0ba9\u0bbf","\u0b86\u0b9f\u0bbf","\u0b86\u0bb5\u0ba3\u0bbf","\u0baa\u0bc1\u0bb0\u0b9f\u0bcd\u0b9f\u0bbe\u0b9a\u0bbf","\u0b90\u0baa\u0bcd\u0baa\u0b9a\u0bbf","\u0b95\u0bbe\u0bb0\u0bcd\u0ba4\u0bcd\u0ba4\u0bbf\u0b95\u0bc8","\u0bae\u0bbe\u0bb0\u0bcd\u0b95\u0bb4\u0bbf"],showMonthAfterYear:!1,yearSuffix:""},th:{dayNames:["\u0e2d\u0e32\u0e17\u0e34\u0e15\u0e22\u0e4c","\u0e08\u0e31\u0e19\u0e17\u0e23\u0e4c","\u0e2d\u0e31\u0e07\u0e04\u0e32\u0e23","\u0e1e\u0e38\u0e18","\u0e1e\u0e24\u0e2b\u0e31\u0e2a\u0e1a\u0e14\u0e35","\u0e28\u0e38\u0e01\u0e23\u0e4c","\u0e40\u0e2a\u0e32\u0e23\u0e4c"],dayNamesMin:["\u0e2d\u0e32.","\u0e08.","\u0e2d.","\u0e1e.","\u0e1e\u0e24.","\u0e28.","\u0e2a."],firstDay:0,isRTL:!1,monthNames:["\u0e21\u0e01\u0e23\u0e32\u0e04\u0e21","\u0e01\u0e38\u0e21\u0e20\u0e32\u0e1e\u0e31\u0e19\u0e18\u0e4c","\u0e21\u0e35\u0e19\u0e32\u0e04\u0e21","\u0e40\u0e21\u0e29\u0e32\u0e22\u0e19","\u0e1e\u0e24\u0e29\u0e20\u0e32\u0e04\u0e21","\u0e21\u0e34\u0e16\u0e38\u0e19\u0e32\u0e22\u0e19","\u0e01\u0e23\u0e01\u0e0e\u0e32\u0e04\u0e21","\u0e2a\u0e34\u0e07\u0e2b\u0e32\u0e04\u0e21","\u0e01\u0e31\u0e19\u0e22\u0e32\u0e22\u0e19","\u0e15\u0e38\u0e25\u0e32\u0e04\u0e21","\u0e1e\u0e24\u0e28\u0e08\u0e34\u0e01\u0e32\u0e22\u0e19","\u0e18\u0e31\u0e19\u0e27\u0e32\u0e04\u0e21"],showMonthAfterYear:!1,yearSuffix:""},tj:{dayNames:["\u044f\u043a\u0448\u0430\u043d\u0431\u0435","\u0434\u0443\u0448\u0430\u043d\u0431\u0435","\u0441\u0435\u0448\u0430\u043d\u0431\u0435","\u0447\u043e\u0440\u0448\u0430\u043d\u0431\u0435","\u043f\u0430\u043d\u04b7\u0448\u0430\u043d\u0431\u0435","\u04b7\u0443\u043c\u044a\u0430","\u0448\u0430\u043d\u0431\u0435"],dayNamesMin:["\u044f\u043a\u0448","\u0434\u0443\u0448","\u0441\u0435\u0448","\u0447\u043e\u0440","\u043f\u0430\u043d","\u04b7\u0443\u043c","\u0448\u0430\u043d"],firstDay:1,isRTL:!1,monthNames:["\u042f\u043d\u0432\u0430\u0440","\u0424\u0435\u0432\u0440\u0430\u043b","\u041c\u0430\u0440\u0442","\u0410\u043f\u0440\u0435\u043b","\u041c\u0430\u0439","\u0418\u044e\u043d","\u0418\u044e\u043b","\u0410\u0432\u0433\u0443\u0441\u0442","\u0421\u0435\u043d\u0442\u044f\u0431\u0440","\u041e\u043a\u0442\u044f\u0431\u0440","\u041d\u043e\u044f\u0431\u0440","\u0414\u0435\u043a\u0430\u0431\u0440"],showMonthAfterYear:!1,yearSuffix:""},tr:{dayNames:["Pazar","Pazartesi","Sal\u0131","\xc7ar\u015famba","Per\u015fembe","Cuma","Cumartesi"],dayNamesMin:["Pz","Pt","Sa","\xc7a","Pe","Cu","Ct"],firstDay:1,isRTL:!1,monthNames:["Ocak","\u015eubat","Mart","Nisan","May\u0131s","Haziran","Temmuz","A\u011fustos","Eyl\xfcl","Ekim","Kas\u0131m","Aral\u0131k"],showMonthAfterYear:!1,yearSuffix:""},uk:{dayNames:["\u043d\u0435\u0434\u0456\u043b\u044f","\u043f\u043e\u043d\u0435\u0434\u0456\u043b\u043e\u043a","\u0432\u0456\u0432\u0442\u043e\u0440\u043e\u043a","\u0441\u0435\u0440\u0435\u0434\u0430","\u0447\u0435\u0442\u0432\u0435\u0440","\u043f\u2019\u044f\u0442\u043d\u0438\u0446\u044f","\u0441\u0443\u0431\u043e\u0442\u0430"],dayNamesMin:["\u043d\u0435\u0434","\u043f\u043d\u0434","\u0432\u0456\u0432","\u0441\u0440\u0434","\u0447\u0442\u0432","\u043f\u0442\u043d","\u0441\u0431\u0442"],firstDay:1,isRTL:!1,monthNames:["\u0421\u0456\u0447\u0435\u043d\u044c","\u041b\u044e\u0442\u0438\u0439","\u0411\u0435\u0440\u0435\u0437\u0435\u043d\u044c","\u041a\u0432\u0456\u0442\u0435\u043d\u044c","\u0422\u0440\u0430\u0432\u0435\u043d\u044c","\u0427\u0435\u0440\u0432\u0435\u043d\u044c","\u041b\u0438\u043f\u0435\u043d\u044c","\u0421\u0435\u0440\u043f\u0435\u043d\u044c","\u0412\u0435\u0440\u0435\u0441\u0435\u043d\u044c","\u0416\u043e\u0432\u0442\u0435\u043d\u044c","\u041b\u0438\u0441\u0442\u043e\u043f\u0430\u0434","\u0413\u0440\u0443\u0434\u0435\u043d\u044c"],showMonthAfterYear:!1,yearSuffix:""},vi:{dayNames:["Ch\u1ee7 Nh\u1eadt","Th\u1ee9 Hai","Th\u1ee9 Ba","Th\u1ee9 T\u01b0","Th\u1ee9 N\u0103m","Th\u1ee9 S\xe1u","Th\u1ee9 B\u1ea3y"],dayNamesMin:["CN","T2","T3","T4","T5","T6","T7"],firstDay:0,isRTL:!1,monthNames:["Th\xe1ng M\u1ed9t","Th\xe1ng Hai","Th\xe1ng Ba","Th\xe1ng T\u01b0","Th\xe1ng N\u0103m","Th\xe1ng S\xe1u","Th\xe1ng B\u1ea3y","Th\xe1ng T\xe1m","Th\xe1ng Ch\xedn","Th\xe1ng M\u01b0\u1eddi","Th\xe1ng M\u01b0\u1eddi M\u1ed9t","Th\xe1ng M\u01b0\u1eddi Hai"],showMonthAfterYear:!1,yearSuffix:""},"zh-CN":{dayNames:["\u661f\u671f\u65e5","\u661f\u671f\u4e00","\u661f\u671f\u4e8c","\u661f\u671f\u4e09","\u661f\u671f\u56db","\u661f\u671f\u4e94","\u661f\u671f\u516d"],dayNamesMin:["\u5468\u65e5","\u5468\u4e00","\u5468\u4e8c","\u5468\u4e09","\u5468\u56db","\u5468\u4e94","\u5468\u516d"],firstDay:1,isRTL:!1,monthNames:["\u4e00\u6708","\u4e8c\u6708","\u4e09\u6708","\u56db\u6708","\u4e94\u6708","\u516d\u6708","\u4e03\u6708","\u516b\u6708","\u4e5d\u6708","\u5341\u6708","\u5341\u4e00\u6708","\u5341\u4e8c\u6708"],showMonthAfterYear:!0,yearSuffix:"\u5e74"},"zh-HK":{dayNames:["\u661f\u671f\u65e5","\u661f\u671f\u4e00","\u661f\u671f\u4e8c","\u661f\u671f\u4e09","\u661f\u671f\u56db","\u661f\u671f\u4e94","\u661f\u671f\u516d"],dayNamesMin:["\u5468\u65e5","\u5468\u4e00","\u5468\u4e8c","\u5468\u4e09","\u5468\u56db","\u5468\u4e94","\u5468\u516d"],firstDay:0,isRTL:!1,monthNames:["\u4e00\u6708","\u4e8c\u6708","\u4e09\u6708","\u56db\u6708","\u4e94\u6708","\u516d\u6708","\u4e03\u6708","\u516b\u6708","\u4e5d\u6708","\u5341\u6708","\u5341\u4e00\u6708","\u5341\u4e8c\u6708"],showMonthAfterYear:!0,yearSuffix:"\u5e74"},"zh-TW":{dayNames:["\u661f\u671f\u65e5","\u661f\u671f\u4e00","\u661f\u671f\u4e8c","\u661f\u671f\u4e09","\u661f\u671f\u56db","\u661f\u671f\u4e94","\u661f\u671f\u516d"],dayNamesMin:["\u5468\u65e5","\u5468\u4e00","\u5468\u4e8c","\u5468\u4e09","\u5468\u56db","\u5468\u4e94","\u5468\u516d"],firstDay:1,isRTL:!1,monthNames:["\u4e00\u6708","\u4e8c\u6708","\u4e09\u6708","\u56db\u6708","\u4e94\u6708","\u516d\u6708","\u4e03\u6708","\u516b\u6708","\u4e5d\u6708","\u5341\u6708","\u5341\u4e00\u6708","\u5341\u4e8c\u6708"],showMonthAfterYear:!0,yearSuffix:"\u5e74"}},a.fn.datePicker=function(a){return new AJS.DatePicker(this,a)
}}(jQuery),AJS.dropDown=function(a,b){var c=null,d=[],e=!1,f=AJS.$(document),g={item:"li:has(a)",activeClass:"active",alignment:"right",displayHandler:function(a){return a.name},escapeHandler:function(){return this.hide("escape"),!1},hideHandler:function(){},moveHandler:function(){},useDisabled:!1};if(AJS.$.extend(g,b),g.alignment={left:"left",right:"right"}[g.alignment.toLowerCase()]||"left",a&&a.jquery)c=a;else if("string"==typeof a)c=AJS.$(a);else{if(!a||a.constructor!=Array)throw new Error("AJS.dropDown function was called with illegal parameter. Should be AJS.$ object, AJS.$ selector or array.");c=AJS("div").addClass("aui-dropdown").toggleClass("hidden",!!g.isHiddenByDefault);for(var h=0,i=a.length;i>h;h++){for(var j=AJS("ol"),k=0,l=a[h].length;l>k;k++){var m=AJS("li"),n=a[h][k];n.href?(m.append(AJS("a").html("<span>"+g.displayHandler(n)+"</span>").attr({href:n.href}).addClass(n.className)),AJS.$.data(AJS.$("a > span",m)[0],"properties",n)):m.html(n.html).addClass(n.className),n.icon&&m.prepend(AJS("img").attr("src",n.icon)),n.insideSpanIcon&&m.children("a").prepend(AJS("span").attr("class","icon")),AJS.$.data(m[0],"properties",n),j.append(m)}h==i-1&&j.addClass("last"),c.append(j)}AJS.$("body").append(c)}var o=function(){q(1)},p=function(){q(-1)},q=function(a){var b=!e,c=AJS.dropDown.current.$[0],d=AJS.dropDown.current.links,f=c.focused;if(e=!0,0!==d.length){if(c.focused="number"==typeof f?f:-1,!AJS.dropDown.current)return AJS.log("move - not current, aborting"),!0;c.focused+=a,c.focused<0?c.focused=d.length-1:c.focused>=d.length&&(c.focused=0),g.moveHandler(AJS.$(d[c.focused]),0>a?"up":"down"),b&&d.length?(AJS.$(d[c.focused]).addClass(g.activeClass),e=!1):d.length||(e=!1)}},r=function(a){if(!AJS.dropDown.current)return!0;var b=a.which,c=AJS.dropDown.current.$[0],d=AJS.dropDown.current.links;switch(AJS.dropDown.current.cleanActive(),b){case 40:o();break;case 38:p();break;case 27:return g.escapeHandler.call(AJS.dropDown.current,a);case 13:return c.focused>=0?g.selectionHandler?g.selectionHandler.call(AJS.dropDown.current,a,AJS.$(d[c.focused])):"a"!=AJS.$(d[c.focused]).attr("nodeName")?AJS.$("a",d[c.focused]).trigger("focus"):AJS.$(d[c.focused]).trigger("focus"):!0;default:return d.length&&AJS.$(d[c.focused]).addClass(g.activeClass),!0}return a.stopPropagation(),a.preventDefault(),!1},s=function(a){a&&a.which&&3==a.which||a&&a.button&&2==a.button||AJS.dropDown.current&&AJS.dropDown.current.hide("click")},t=function(a){return function(){AJS.dropDown.current&&(AJS.dropDown.current.cleanFocus(),this.originalClass=this.className,AJS.$(this).addClass(g.activeClass),AJS.dropDown.current.$[0].focused=a)}},u=function(a){return a.button||a.metaKey||a.ctrlKey||a.shiftKey?!0:void(AJS.dropDown.current&&g.selectionHandler&&g.selectionHandler.call(AJS.dropDown.current,a,AJS.$(this)))},v=function(a){var b=!1;return a.data("events")&&AJS.$.each(a.data("events"),function(a,c){AJS.$.each(c,function(a,c){return u===c?(b=!0,!1):void 0})}),b};return c.each(function(){var a=this,b=AJS.$(this),c={},e={reset:function(){c=AJS.$.extend(c,{$:b,links:AJS.$(g.item||"li:has(a)",a),cleanActive:function(){a.focused+1&&c.links.length&&AJS.$(c.links[a.focused]).removeClass(g.activeClass)},cleanFocus:function(){c.cleanActive(),a.focused=-1},moveDown:o,moveUp:p,moveFocus:r,getFocusIndex:function(){return"number"==typeof a.focused?a.focused:-1}}),c.links.each(function(a){var b=AJS.$(this);v(b)||(b.hover(t(a),c.cleanFocus),b.click(u))})},appear:function(a){a?(b.removeClass("hidden"),b.addClass("aui-dropdown-"+g.alignment)):b.addClass("hidden")},fade:function(a){a?b.fadeIn("fast"):b.fadeOut("fast")},scroll:function(a){a?b.slideDown("fast"):b.slideUp("fast")}};c.reset=e.reset,c.reset(),c.addControlProcess=function(a,b){AJS.$.aop.around({target:this,method:a},b)},c.addCallback=function(a,b){return AJS.$.aop.after({target:this,method:a},b)},c.show=function(b){g.useDisabled&&this.$.closest(".aui-dd-parent").hasClass("disabled")||(this.alignment=g.alignment,s(),AJS.dropDown.current=this,this.method=b||this.method||"appear",this.timer=setTimeout(function(){f.click(s)},0),f.keydown(r),g.firstSelected&&this.links[0]&&t(0).call(this.links[0]),AJS.$(a.offsetParent).css({zIndex:2e3}),e[this.method](!0),AJS.$(document).trigger("showLayer",["dropdown",AJS.dropDown.current]))},c.hide=function(a){return this.method=this.method||"appear",AJS.$(b.get(0).offsetParent).css({zIndex:""}),this.cleanFocus(),e[this.method](!1),f.unbind("click",s).unbind("keydown",r),AJS.$(document).trigger("hideLayer",["dropdown",AJS.dropDown.current]),AJS.dropDown.current=null,a},c.addCallback("reset",function(){g.firstSelected&&this.links[0]&&t(0).call(this.links[0])}),AJS.dropDown.iframes||(AJS.dropDown.iframes=[]),AJS.dropDown.createShims=function(){return AJS.$("iframe").each(function(){var a=this;a.shim||(a.shim=AJS.$("<div />").addClass("shim hidden").appendTo("body"),AJS.dropDown.iframes.push(a))}),arguments.callee}(),c.addCallback("show",function(){AJS.$(AJS.dropDown.iframes).each(function(){var a=AJS.$(this);if(a.is(":visible")){var b=a.offset();b.height=a.height(),b.width=a.width(),this.shim.css({left:b.left+"px",top:b.top+"px",height:b.height+"px",width:b.width+"px"}).removeClass("hidden")}})}),c.addCallback("hide",function(){AJS.$(AJS.dropDown.iframes).each(function(){this.shim.addClass("hidden")}),g.hideHandler()}),AJS.$.browser.msie&&~~AJS.$.browser.version<9&&!function(){var a=function(){this.$.is(":visible")&&(this.iframeShim||(this.iframeShim=AJS.$('<iframe class="dropdown-shim" src="javascript:false;" frameBorder="0" />').insertBefore(this.$)),this.iframeShim.css({display:"block",top:this.$.css("top"),width:this.$.outerWidth()+"px",height:this.$.outerHeight()+"px"}),this.iframeShim.css("left"==g.alignment?{left:"0px"}:{right:"0px"}))};c.addCallback("reset",a),c.addCallback("show",a),c.addCallback("hide",function(){this.iframeShim&&this.iframeShim.css({display:"none"})})}(),d.push(c)}),d},AJS.dropDown.getAdditionalPropertyValue=function(a,b){var c=a[0];c&&"string"==typeof c.tagName&&"li"==c.tagName.toLowerCase()||AJS.log("AJS.dropDown.getAdditionalPropertyValue : item passed in should be an LI element wrapped by jQuery");var d=AJS.$.data(c,"properties");return d?d[b]:null},AJS.dropDown.removeAllAdditionalProperties=function(){},AJS.dropDown.Standard=function(a){var b,c=[],d={selector:".aui-dd-parent",dropDown:".aui-dropdown",trigger:".aui-dd-trigger"};AJS.$.extend(d,a);var e=function(a,b,c,e){AJS.$.extend(e,{trigger:a}),b.addClass("dd-allocated"),c.addClass("hidden"),0==d.isHiddenByDefault&&e.show(),e.addCallback("show",function(){b.addClass("active")}),e.addCallback("hide",function(){b.removeClass("active")})},f=function(a,b,c,d){d!=AJS.dropDown.current&&(c.css({top:b.outerHeight()}),d.show(),a.stopImmediatePropagation()),a.preventDefault()};if(d.useLiveEvents){var g=[],h=[];AJS.$(d.trigger).live("click",function(a){var b,c,i,j,k=AJS.$(this);if((j=AJS.$.inArray(this,g))>=0){var l=h[j];b=l.parent,c=l.dropdown,i=l.ddcontrol}else{if(b=k.closest(d.selector),c=b.find(d.dropDown),0===c.length)return;if(i=AJS.dropDown(c,d)[0],!i)return;g.push(this),l={parent:b,dropdown:c,ddcontrol:i},e(k,b,c,i),h.push(l)}f(a,k,c,i)})}else b=this instanceof AJS.$?this:AJS.$(d.selector),b=b.not(".dd-allocated").filter(":has("+d.dropDown+")").filter(":has("+d.trigger+")"),b.each(function(){var a=AJS.$(this),b=AJS.$(d.dropDown,this),g=AJS.$(d.trigger,this),h=AJS.dropDown(b,d)[0];AJS.$.extend(h,{trigger:g}),e(g,a,b,h),g.click(function(a){f(a,g,b,h)}),c.push(h)});return c},AJS.dropDown.Ajax=function(a){var b,c={cache:!0};return AJS.$.extend(c,a||{}),b=AJS.dropDown.Standard.call(this,c),AJS.$(b).each(function(){var a=this;AJS.$.extend(a,{getAjaxOptions:function(b){var d=function(b){c.formatResults&&(b=c.formatResults(b)),c.cache&&a.cache.set(a.getAjaxOptions(),b),a.refreshSuccess(b)};return c.ajaxOptions?AJS.$.isFunction(c.ajaxOptions)?AJS.$.extend(c.ajaxOptions.call(a),{success:d}):AJS.$.extend(c.ajaxOptions,{success:d}):AJS.$.extend(b,{success:d})},refreshSuccess:function(a){this.$.html(a)},cache:function(){var a={};return{get:function(b){var c=b.data||"";return a[(b.url+c).replace(/[\?\&]/gi,"")]},set:function(b,c){var d=b.data||"";a[(b.url+d).replace(/[\?\&]/gi,"")]=c},reset:function(){a={}}}}(),show:function(b){return function(){c.cache&&a.cache.get(a.getAjaxOptions())?(a.refreshSuccess(a.cache.get(a.getAjaxOptions())),b.call(a)):(AJS.$(AJS.$.ajax(a.getAjaxOptions())).throbber({target:a.$,end:function(){a.reset()}}),b.call(a),a.iframeShim&&a.iframeShim.hide())}}(a.show),resetCache:function(){a.cache.reset()}}),a.addCallback("refreshSuccess",function(){a.reset()})}),b},AJS.$.fn.dropDown=function(a,b){return a=(a||"Standard").replace(/^([a-z])/,function(a){return a.toUpperCase()}),AJS.dropDown[a].call(this,b)},function(a){function b(a){a.preventDefault()}function c(a){if(a.click)a.click();else{var b=document.createEvent("MouseEvents");b.initMouseEvent("click",!0,!0,window,0,0,0,0,0,!1,!1,!1,!1,0,null),a.dispatchEvent(b)}}function d(b,c){return b===c||a.contains(c,b)}function e(b){b instanceof AJS.$||(b=a(b));var c=b.attr("aria-owns"),d=b.attr("aria-haspopup"),e=document.getElementById(c);if(e)return a(e);if(!c)throw new Error("Dropdown 2 trigger required attribute not set: aria-owns");if(!d)throw new Error("Dropdown 2 trigger required attribute not set: aria-haspopup");if(!e)throw new Error("Dropdown 2 trigger aria-owns attr set to nonexistent id: "+c);throw new Error("Dropdown 2 trigger unknown error. I don't know what you did, but there's smoke everywhere. Consult the documentation.")}var f=a(document),g=AJS.$.browser.msie&&8==parseInt(AJS.$.browser.version,10),h=null,i=function(){function c(b){g||1!==b.which||(g=!0,f.bind("mouseup mouseleave",d),a(this).trigger("aui-button-invoke"))}function d(){f.unbind("mouseup mouseleave",d),setTimeout(function(){g=!1},0)}function e(){g||a(this).trigger("aui-button-invoke")}var g=!1;return"undefined"==typeof document.addEventListener?{click:e,"click selectstart":b,mousedown:function(a){function b(a){switch(a.toElement){case null:case d:case document.body:case document.documentElement:a.returnValue=!1}}var d=this,e=document.activeElement;c.call(this,a),null!==e&&(e.attachEvent("onbeforedeactivate",b),setTimeout(function(){e.detachEvent("onbeforedeactivate",b)},0))}}:{click:e,"click mousedown":b,mousedown:c}}(),j={"aui-button-invoke":function(i,j){function k(b,c){b.each(function(){var b=a(this);b.attr("role",c),b.hasClass("checked")||b.hasClass("aui-dropdown2-checked")?(b.attr("aria-checked","true"),"radio"==c&&b.closest("ul").attr("role","radiogroup")):b.attr("aria-checked","false")})}function l(){var b=E.offset(),c=E.outerWidth();D.css({left:0,top:0});var d,e=D.outerWidth(),f=a("body").outerWidth(!0),h=Math.max(parseInt(D.css("min-width"),10),c),i=E.data("container")||!1,j="left";g&&(d=parseInt(D.css("border-left-width"),10)+parseInt(D.css("border-right-width"),10),e-=d,h-=d),F||D.css("min-width",h+"px");var k=b.left,l=b.top+E.outerHeight();if(F){var m=3;k=b.left+L.outerWidth()-m,l=b.top}if(k+e>f&&k+c>=e&&(k=b.left+c-e,F&&(k=b.left-e),j="right"),i){var n=(E.closest(i),E.offset().left+E.outerWidth()),o=n+e;h>=e&&(e=h),o>n&&(k=n-e,j="right"),g&&(k-=d)}D.attr({"data-dropdown2-alignment":j,"aria-hidden":"false"}).css({display:"block",left:k+"px",top:l+"px"}),D.appendTo(document.body)}function m(){A(),C("off"),setTimeout(function(){D.css("display","none").css("min-width","").insertAfter(E).attr("aria-hidden","true"),F||E.removeClass("active aui-dropdown2-active"),s().removeClass("active aui-dropdown2-active"),D.removeClass("aui-dropdown2-in-toolbar"),D.removeClass("aui-dropdown2-in-buttons"),H?D.insertBefore(H):D.appendTo(G),D.trigger("aui-dropdown2-hide")},0)}function n(){m(),F&&L.trigger("aui-dropdown2-hide-all")}function o(a){F&&a.target===L[0]&&m()}function p(a){return!a.is(".disabled, .aui-dropdown2-disabled, [aria-disabled=true]")}function q(a){return a.hasClass("aui-dropdown2-sub-trigger")}function r(b,c){if(q(b)){c=a.extend({},c,{$menu:K});var d=e(b);d.is(":visible")?d.trigger("aui-dropdown2-select-first"):b.trigger("aui-button-invoke",c)}}function s(){return D.find("a.active,a.aui-dropdown2-active")}function t(a){return O&&O[0]===a[0]?!1:(O=a,s().removeClass("active aui-dropdown2-active"),p(a)&&a.addClass("active aui-dropdown2-active"),D.trigger("aui-dropdown2-item-selected"),B(),!0)}function u(){t(D.find("a:not(.disabled):not(.aui-dropdown2-disabled)").first())}function v(a){var b=D.find("> ul > li > a, > .aui-dropdown2-section > ul > li > a").not(".disabled,.aui-dropdown2-disabled");t(y(b,a,!0))}function w(a){a.length>0&&(n(),a.trigger("aui-button-invoke"))}function x(a){w(y(K.find(".aui-dropdown2-trigger").not(".disabled, .aui-dropdown2-disabled, [aria-disabled=true], .aui-dropdown2-sub-trigger"),a,!1))}function y(a,b,c){var d=a.index(a.filter(".active,.aui-dropdown2-active"));return d+=0>d&&0>b?1:0,d+=b,c?d%=a.length:0>d&&(d=a.length),a.eq(d)}function z(){w(a(this))}function A(){h===N&&(f.unbind(N),h=null)}function B(){h!==N&&(f.unbind(h),f.bind(N),h=N)}function C(a){var b="bind",c="delegate";"on"!==a&&(b="unbind",c="undelegate"),F?L[b]("aui-dropdown2-hide aui-dropdown2-item-selected aui-dropdown2-step-out",o):(K[c](".aui-dropdown2-trigger:not(.active):not(.aui-dropdown2-active)","mousemove",z),E[b]("aui-button-invoke",m)),D[b]("aui-dropdown2-hide-all",n),D[c]("a",M),D[b]("aui-dropdown2-hide",B),D[b]("aui-dropdown2-select-first",u)}j=a.extend({selectFirst:!0},j);var D=e(this),E=a(this).addClass("active aui-dropdown2-active"),F=E.hasClass("aui-dropdown2-sub-trigger"),G=D.parent()[0],H=D.next()[0],I=a(this).attr("data-dropdown2-hide-location");if(I){var J=document.getElementById(I);if(!J)throw new Error("The specified data-dropdown2-hide-location id doesn't exist");G=a(J),H=void 0}var K=j.$menu||E.closest(".aui-dropdown2-trigger-group");if(F){var L=E.closest(".aui-dropdown2");D.addClass(L.attr("class")).addClass("aui-dropdown2-sub-menu")}var M={click:function(c){var d=a(this);p(d)&&(d.hasClass("interactive")||d.hasClass("aui-dropdown2-interactive")||n(),q(d)&&(r(d,{selectFirst:!1}),b(c)))},mousemove:function(){var b=a(this),c=t(b);c&&r(b,{selectFirst:!1})}},N={"click focusin mousedown":function(a){var b=a.target;(document!==b||"focusin"!==a.type)&&(d(b,D[0])||d(b,E[0])||n())},keydown:function(a){var d;if(a.shiftKey&&9==a.keyCode)v(-1);else switch(a.keyCode){case 13:d=s(),q(d)?r(d):c(d[0]);break;case 27:m();break;case 37:if(d=s(),q(d)){var f=e(d);if(f.is(":visible"))return void D.trigger("aui-dropdown2-step-out")}F?m():x(-1);break;case 38:v(-1);break;case 39:d=s(),q(d)?r(d):x(1);break;case 40:v(1);break;case 9:v(1);break;default:return}b(a)}};E.attr("aria-controls",E.attr("aria-owns")),g&&D.removeClass("aui-dropdown2-tailed"),D.find(".disabled,.aui-dropdown2-disabled").attr("aria-disabled","true"),D.find("li.hidden > a,li.aui-dropdown2-hidden > a").addClass("disabled aui-dropdown2-disabled").attr("aria-disabled","true"),k(D.find(".aui-dropdown2-checkbox"),"checkbox"),k(D.find(".aui-dropdown2-radio"),"radio"),l(),E.hasClass("toolbar-trigger")&&D.addClass("aui-dropdown2-in-toolbar"),E.parent().hasClass("aui-buttons")&&D.addClass("aui-dropdown2-in-buttons"),E.parents().hasClass("aui-header")&&D.addClass("aui-dropdown2-in-header"),D.trigger("aui-dropdown2-show",j),j.selectFirst&&u(),C("on");var O=null},mousedown:function(b){1===b.which&&a(this).bind(k)}},k={mouseleave:function(){f.bind(l)},"mouseup mouseleave":function(){a(this).unbind(k)}},l={mouseup:function(b){var d=a(b.target).closest(".aui-dropdown2 a, .aui-dropdown2-trigger")[0];d&&setTimeout(function(){c(d)},0)},"mouseup mouseleave":function(){a(this).unbind(l)}};f.delegate(".aui-dropdown2-trigger",i),f.delegate(".aui-dropdown2-trigger:not(.active):not(.aui-dropdown2-active):not([aria-disabled=true]),.aui-dropdown2-sub-trigger:not([aria-disabled=true])",j),f.delegate(".aui-dropdown2-checkbox:not(.disabled):not(.aui-dropdown2-disabled)","click",function(){var b=a(this);b.hasClass("checked")||b.hasClass("aui-dropdown2-checked")?(b.removeClass("checked aui-dropdown2-checked").attr("aria-checked","false"),b.trigger("aui-dropdown2-item-uncheck")):(b.addClass("checked aui-dropdown2-checked").attr("aria-checked","true"),b.trigger("aui-dropdown2-item-check"))}),f.delegate(".aui-dropdown2-radio:not(.checked):not(.aui-dropdown2-checked):not(.disabled):not(.aui-dropdown2-disabled)","click",function(){var b=a(this),c=b.closest("ul").find(".checked,.aui-dropdown2-checked");c.removeClass("checked aui-dropdown2-checked").attr("aria-checked","false").trigger("aui-dropdown2-item-uncheck"),b.addClass("checked aui-dropdown2-checked").attr("aria-checked","true").trigger("aui-dropdown2-item-check")}),f.delegate(".aui-dropdown2 a.disabled,.aui-dropdown2 a.aui-dropdown2-disabled","click",function(a){b(a)})}(AJS.$),AJS.bind=function(a,b,c){try{return"function"==typeof c?AJS.$(window).bind(a,b,c):AJS.$(window).bind(a,b)}catch(d){AJS.log("error while binding: "+d.message)}},AJS.unbind=function(a,b){try{return AJS.$(window).unbind(a,b)}catch(c){AJS.log("error while unbinding: "+c.message)}},AJS.trigger=function(a,b){try{return AJS.$(window).trigger(a,b)}catch(c){AJS.log("error while triggering: "+c.message)}},AJS.warnAboutFirebug=function(){AJS.log("DEPRECATED: please remove all uses of AJS.warnAboutFirebug")},AJS.inlineHelp=function(){AJS.$(".icon-inline-help").click(function(){var a=AJS.$(this).siblings(".field-help");a.hasClass("hidden")?a.removeClass("hidden"):a.addClass("hidden")})},function(a){function b(b){var c=a(b),d=a.extend({left:0,top:0},c.offset());return{left:d.left,top:d.top,width:c.outerWidth(),height:c.outerHeight()}}function c(a,b,c,d){var e=AJS.$.isFunction(d.offsetX)?d.offsetX(a,b,c,d):d.offsetX,f=AJS.$.isFunction(d.offsetY)?d.offsetY(a,b,c,d):d.offsetY,g=AJS.$.isFunction(d.arrowOffsetX)?d.arrowOffsetX(a,b,c,d):d.arrowOffsetX,h=AJS.$.isFunction(d.arrowOffsetY)?d.arrowOffsetY(a,b,c,d):d.arrowOffsetY,i="body"!==d.container.toLowerCase(),j=AJS.$(d.container),k=i?AJS.$(d.container).parent():AJS.$(window),l=i?j.offset():{left:0,top:0},m=i?k.offset():{left:0,top:0},n=b.target,o=n.offset(),p=n[0].getBBox&&n[0].getBBox();return{screenPadding:10,arrowMargin:5,window:{top:m.top,left:m.left,scrollTop:k.scrollTop(),scrollLeft:k.scrollLeft(),width:k.width(),height:k.height()},scrollContainer:{width:j.width(),height:j.height()},trigger:{top:o.top-l.top,left:o.left-l.left,width:p?p.width:n.outerWidth(),height:p?p.height:n.outerHeight()},dialog:{width:a.width(),height:a.height(),offset:{top:f,left:e}},arrow:{height:a.find(".arrow").outerHeight(),offset:{top:h,left:g}}}}function d(a,b,d,e){var f=c(a,b,d,e),g=f.screenPadding,h=f.window,i=f.trigger,j=f.dialog,k=f.arrow,l=f.scrollContainer,m={top:i.top-h.scrollTop,left:i.left-h.scrollLeft},n=Math.floor(i.height/2),o=Math.floor(j.height/2),p=Math.floor(k.height/2),q=m.left-j.offset.left-g,r=l.width-m.left-i.width-j.offset.left-g,s=q>=j.width,t=r>=j.width,u=!t&&s?"e":"w",v=m.top+n-p,w=h.height-v-k.height;g=Math.min(g,v-f.arrowMargin),g=Math.min(g,w-f.arrowMargin);var x=Math.max(m.top-g,0),y=Math.max(h.height-m.top-i.height-g,0),z=Math.max(o-n-j.offset.top-x,0),A=Math.max(o-n+j.offset.top-y,0),B=z||-A||0,C={top:i.top+n-o+j.offset.top+B,left:"w"===u?i.left+i.width+j.offset.left:i.left-j.width-j.offset.left},D={top:o-p+k.offset.top+B};return{gravity:u,popupCss:C,arrowCss:D}}function e(a,c,d,e){var f=AJS.$.isFunction(e.offsetX)?e.offsetX(a,c,d,e):e.offsetX,g=AJS.$.isFunction(e.offsetY)?e.offsetY(a,c,d,e):e.offsetY,h=AJS.$.isFunction(e.arrowOffsetX)?e.arrowOffsetX(a,c,d,e):e.arrowOffsetX,i=(AJS.$.isFunction(e.arrowOffsetY)?e.arrowOffsetY(a,c,d,e):e.arrowOffsetY,b(window)),j=b(c.target),k=b(a),l=b(a.find(".aui-inline-dialog-arrow")),m=j.left+j.width/2,n=(window.pageYOffset||document.documentElement.scrollTop)+i.height,o=10;k.top=j.top+j.height+~~g,k.left=j.left+~~f;var p=i.width-(k.left+k.width+o);l.left=m-k.left+~~h,l.top=-(l.height/2);var q=j.top>k.height,r=k.top+k.height<n,s=!r&&q||q&&"s"===e.gravity;if(s&&(k.top=j.top-k.height-l.height/2,l.top=k.height),e.isRelativeToMouse)0>p?(k.right=o,k.left="auto",l.left=d.x-(i.width-k.width)):(k.left=d.x-20,l.left=d.x-k.left);else if(0>p){k.right=o,k.left="auto";var t=i.width-k.right,u=t-k.width;l.right="auto",l.left=m-u-l.width/2}else k.width<=j.width/2&&(l.left=k.width/2,k.left=m-k.width/2);return{gravity:s?"s":"n",displayAbove:s,popupCss:{left:k.left,top:k.top,right:k.right},arrowCss:{left:l.left,top:l.top,right:l.right}}}AJS.InlineDialog=function(b,c,d,e){if(e&&e.getArrowAttributes&&AJS.log("DEPRECATED: getArrowAttributes - See https://ecosystem.atlassian.net/browse/AUI-1362"),e&&e.getArrowPath&&(AJS.log("DEPRECATED: getArrowPath - See https://ecosystem.atlassian.net/browse/AUI-1362"),void 0!==e.gravity&&AJS.log("DEPRECATED: getArrowPath does not support gravity - See https://ecosystem.atlassian.net/browse/AUI-2197")),e&&void 0!==e.onTop&&(AJS.log("DEPRECATED: onTop has been replaced with gravity - See https://ecosystem.atlassian.net/browse/AUI-2197"),e.onTop&&void 0===e.gravity&&(e.gravity="s")),"undefined"==typeof c&&(c=String(Math.random()).replace(".",""),a("#inline-dialog-"+c+", #arrow-"+c+", #inline-dialog-shim-"+c).length))throw"GENERATED_IDENTIFIER_NOT_UNIQUE";var f=a.extend(!1,AJS.InlineDialog.opts,e);"w"===f.gravity&&(f.offsetX=void 0===e.offsetX?10:e.offsetX,f.offsetY=void 0===e.offsetY?0:e.offsetY);var g,h,i,j,k,l=function(){return window.Raphael&&e&&(e.getArrowPath||e.getArrowAttributes)},m=!1,n=!1,o=!1,p=a('<div id="inline-dialog-'+c+'" class="aui-inline-dialog"><div class="aui-inline-dialog-contents contents"></div><div id="arrow-'+c+'" class="aui-inline-dialog-arrow arrow"></div></div>'),q=a("#arrow-"+c,p),r=p.find(".contents");l()||p.find(".aui-inline-dialog-arrow").addClass("aui-css-arrow"),f.displayShadow||r.addClass("aui-inline-dialog-no-shadow"),f.autoWidth?r.addClass("aui-inline-dialog-auto-width"):r.css("width",f.width+"px"),r.mouseover(function(){clearTimeout(h),p.unbind("mouseover")}).mouseout(function(){u()});var s=function(){return g||(g={popup:p,hide:function(){u(0)},id:c,show:function(){t()},persistent:f.persistent?!0:!1,reset:function(){function b(b,d){if(b.css(d.popupCss),l()){"s"===d.gravity&&(d.arrowCss.top-=a.browser.msie?10:9),b.arrowCanvas||(b.arrowCanvas=Raphael("arrow-"+c,16,16));var e=f.getArrowPath,g=a.isFunction(e)?e(d):e;b.arrowCanvas.path(g).attr(f.getArrowAttributes())}else q.removeClass("aui-bottom-arrow aui-left-arrow aui-right-arrow"),"s"!==d.gravity||q.hasClass("aui-bottom-arrow")?"n"===d.gravity||("w"===d.gravity?q.addClass("aui-left-arrow"):"e"===d.gravity&&q.addClass("aui-right-arrow")):q.addClass("aui-bottom-arrow");q.css(d.arrowCss)}var d=f.calculatePositions(p,k,j,f);if(void 0!==d.displayAbove&&(AJS.log("DEPRECATED: displayAbove has been replaced with gravity - See https://ecosystem.atlassian.net/browse/AUI-2197"),d.gravity=d.displayAbove?"s":"n"),b(p,d),p.fadeIn(f.fadeTime,function(){}),a.browser.msie&&~~a.browser.version<10){var e=a("#inline-dialog-shim-"+c);e.length||a(p).prepend(a('<iframe class = "inline-dialog-shim" id="inline-dialog-shim-'+c+'" frameBorder="0" src="javascript:false;"></iframe>')),e.css({width:r.outerWidth(),height:r.outerHeight()})}}}),g},t=function(){p.is(":visible")||(i=setTimeout(function(){o&&n&&(f.addActiveClass&&a(b).addClass("active"),m=!0,f.persistent||B(),AJS.InlineDialog.current=s(),a(document).trigger("showLayer",["inlineDialog",s()]),s().reset())},f.showDelay))},u=function(c){"undefined"==typeof c&&f.persistent||(n=!1,m&&f.preHideCallback.call(p[0].popup)&&(c=null==c?f.hideDelay:c,clearTimeout(h),clearTimeout(i),null!=c&&(h=setTimeout(function(){C(),f.addActiveClass&&a(b).removeClass("active"),p.fadeOut(f.fadeTime,function(){f.hideCallback.call(p[0].popup)}),p.arrowCanvas&&(p.arrowCanvas.remove(),p.arrowCanvas=null),m=!1,n=!1,a(document).trigger("hideLayer",["inlineDialog",s()]),AJS.InlineDialog.current=null,f.cacheContent||(o=!1,w=!1)},c))))},v=function(b,e){var g=a(e);f.upfrontCallback.call({popup:p,hide:function(){u(0)},id:c,show:function(){t()}}),p.each(function(){"undefined"!=typeof this.popup&&this.popup.hide()}),f.closeOthers&&a(".aui-inline-dialog").each(function(){!this.popup.persistent&&this.popup.hide()}),k={target:g},j=b?{x:b.pageX,y:b.pageY}:{x:g.offset().left,y:g.offset().top},m||clearTimeout(i),n=!0;var l=function(){w=!1,o=!0,f.initCallback.call({popup:p,hide:function(){u(0)},id:c,show:function(){t()}}),t()};return w||(w=!0,a.isFunction(d)?d(r,e,l):a.get(d,function(a,b,d){r.html(f.responseHandler(a,b,d)),o=!0,f.initCallback.call({popup:p,hide:function(){u(0)},id:c,show:function(){t()}}),t()})),clearTimeout(h),m||t(),!1};p[0].popup=s();var w=!1,x=!1,y=function(){x||(a(f.container).append(p),x=!0)},z=a(b);f.onHover?f.useLiveEvents?z.selector?a(document).on("mousemove",z.selector,function(a){y(),v(a,this)}).on("mouseout",z.selector,function(){u()}):AJS.log("Warning: inline dialog trigger elements must have a jQuery selector when the useLiveEvents option is enabled."):z.mousemove(function(a){y(),v(a,this)}).mouseout(function(){u()}):f.noBind||(f.useLiveEvents?z.selector?a(document).on("click",z.selector,function(a){return y(),A()?p.hide():v(a,this),!1}).on("mouseout",z.selector,function(){u()}):AJS.log("Warning: inline dialog trigger elements must have a jQuery selector when the useLiveEvents option is enabled."):z.click(function(a){return y(),A()?p.hide():v(a,this),!1}).mouseout(function(){u()}));var A=function(){return m&&f.closeOnTriggerClick},B=function(){F(),I()},C=function(){G(),J()},D=!1,E=c+".inline-dialog-check",F=function(){D||(a("body").bind("click."+E,function(b){var d=a(b.target);0===d.closest("#inline-dialog-"+c+" .contents").length&&u(0)}),D=!0)},G=function(){D&&a("body").unbind("click."+E),D=!1},H=function(a){27===a.keyCode&&u(0)},I=function(){a(document).on("keydown",H)},J=function(){a(document).off("keydown",H)};return p.show=function(a,c){a&&a.stopPropagation(),y(),!e.noBind||b&&b.length?v(a,b):v(a,void 0===c?a.target:c)},p.hide=function(){u(0)},p.refresh=function(){m&&s().reset()},p.getOptions=function(){return f},p},AJS.InlineDialog.opts={onTop:!1,responseHandler:function(a){return a},closeOthers:!0,isRelativeToMouse:!1,addActiveClass:!0,onHover:!1,useLiveEvents:!1,noBind:!1,fadeTime:100,persistent:!1,hideDelay:1e4,showDelay:0,width:300,offsetX:0,offsetY:10,arrowOffsetX:0,arrowOffsetY:0,container:"body",cacheContent:!0,displayShadow:!0,autoWidth:!1,gravity:"n",closeOnTriggerClick:!1,preHideCallback:function(){return!0},hideCallback:function(){},initCallback:function(){},upfrontCallback:function(){},calculatePositions:function(a,b,c,f){f=f||{};var g="w"===f.gravity?d:e;return g(a,b,c,f)},getArrowPath:function(a){return"s"===a.gravity?"M0,8L8,16,16,8":"M0,8L8,0,16,8"},getArrowAttributes:function(){return{fill:"#fff",stroke:"#ccc"}}}}(AJS.$),function(){AJS.keyCode={ALT:18,BACKSPACE:8,CAPS_LOCK:20,COMMA:188,COMMAND:91,COMMAND_LEFT:91,COMMAND_RIGHT:93,CONTROL:17,DELETE:46,DOWN:40,END:35,ENTER:13,ESCAPE:27,HOME:36,INSERT:45,LEFT:37,MENU:93,NUMPAD_ADD:107,NUMPAD_DECIMAL:110,NUMPAD_DIVIDE:111,NUMPAD_ENTER:108,NUMPAD_MULTIPLY:106,NUMPAD_SUBTRACT:109,PAGE_DOWN:34,PAGE_UP:33,PERIOD:190,RIGHT:39,SHIFT:16,SPACE:32,TAB:9,UP:38,WINDOWS:91}}(AJS.$),function(){var a=500,b=5e3,c=100;AJS.messages={setup:function(){AJS.messages.createMessage("generic"),AJS.messages.createMessage("error"),AJS.messages.createMessage("warning"),AJS.messages.createMessage("info"),AJS.messages.createMessage("success"),AJS.messages.createMessage("hint"),AJS.messages.makeCloseable(),AJS.messages.makeFadeout()},makeCloseable:function(a){AJS.$(a||"div.aui-message.closeable").each(function(){var a=AJS.$(this),b=AJS.$('<span class="aui-icon icon-close" role="button" tabindex="0"></span>').click(function(){a.closeMessage()}).keypress(function(b){(b.which===AJS.keyCode.ENTER||b.which===AJS.keyCode.SPACE)&&(a.closeMessage(),b.preventDefault())});a.append(b)})},makeFadeout:function(d,e,f){e="undefined"!=typeof e?e:b,f="undefined"!=typeof f?f:a,AJS.$(d||"div.aui-message.fadeout").each(function(){function a(){g.stop(!0,!1).delay(e).fadeOut(f,function(){g.closeMessage()})}function b(){g.stop(!0,!1).fadeTo(c,1)}function d(){return!h&&!i}var g=AJS.$(this),h=!1,i=!1;g.focusin(function(){h=!0,b()}).focusout(function(){h=!1,d()&&a()}).hover(function(){i=!0,b()},function(){i=!1,d()&&a()}),a()})},template:'<div class="aui-message {type} {closeable} {shadowed} {fadeout}"><p class="title"><strong>{title}</strong></p>{body}<!-- .aui-message --></div>',createMessage:function(a){AJS.messages[a]=function(b,c){var d,e,f=this.template;return c||(c=b,b="#aui-message-bar"),c.closeable=c.closeable!==!1,c.shadowed=c.shadowed!==!1,d=AJS.$(AJS.template(f).fill({type:"aui-message-"+a+" "+a,closeable:c.closeable?"closeable":"",shadowed:c.shadowed?"shadowed":"",fadeout:c.fadeout?"fadeout":"",title:c.title||"","body:html":c.body||""}).toString()),c.id&&(/[#\'\"\.\s]/g.test(c.id)?AJS.log("AJS.Messages error: ID rejected, must not include spaces, hashes, dots or quotes."):d.attr("id",c.id)),e=c.insert||"append","prepend"===e?d.prependTo(b):d.appendTo(b),c.closeable&&AJS.messages.makeCloseable(d),c.fadeout&&AJS.messages.makeFadeout(d,c.delay,c.duration),d}}},AJS.$.fn.closeMessage=function(){var a=AJS.$(this);a.hasClass("aui-message","closeable")&&(a.stop(!0),a.trigger("messageClose",[this]).remove(),AJS.$(document).trigger("aui-message-close",[this]))},AJS.$(function(){AJS.messages.setup()})}(),function(a){"use strict";function b(){var b=a(this);AJS._addID(b),b.attr("role","tab");var c=b.attr("href");a(c).attr("aria-labelledby",b.attr("id")),b.parent().hasClass(k)?b.attr(m,"true"):b.attr(m,"false")}function c(b,c){var d=a(b),e=d.parent(),f=d.find(".tabs-menu").first(),g=f.find("li:not(.aui-tabs-responsive-trigger-item)"),h=f.find(".aui-tabs-responsive-trigger").parent(),j=h.find("a"),k=j.attr("aria-owns"),l=a(document).find("#"+k).attr("aria-checked",!1),m=l.length>0,n=r.totalTabsWidth(g,l),o=n>e.outerWidth();if(!m&&o&&(h=r.createResponsiveDropdownTrigger(f,c),l=r.createResponsiveDropdown(d,c)),j.attr("aria-owns","aui-tabs-responsive-dropdown-"+c),j.attr("id","aui-tabs-responsive-trigger-"+c),j.attr("href","aui-tabs-responsive-trigger-"+c),l.attr("id","aui-tabs-responsive-dropdown-"+c),o){var p=r.processVisibleTabs(g.toArray(),e,h),q=r.totalVisibleTabWidth(p),s=e.outerWidth()-q-h.outerWidth(!0),t=s>0;if(t){var u=l.find("li");r.processInvisibleTabs(u.toArray(),s,h)}l.on("click","a",i)}m&&!o&&(l.find("li").each(function(){r.moveTabOutOfDropdown(a(this),h)}),r.removeResponsiveDropdown(l,h))}function d(b){if(!b.hasClass("aui-tabs-responsive-trigger")){var c=a(b.attr("href").match(j)[0]);c.addClass(l).attr(n,"false").siblings(".tabs-pane").removeClass(l).attr(n,"true");var d=b.parents(".aui-tabs").find(".aui-tabs-responsive-trigger-item a"),e=d.attr("aria-owns"),f=a(document).find("#"+e);f.find("li a").attr("aria-checked",!1).removeClass("checked aui-dropdown2-checked"),f.find("li").removeClass("active-tab")}if(b.parent("li.menu-item").addClass(k).siblings(".menu-item").removeClass(k),b.hasClass("aui-tabs-responsive-item")){var g=c.parent(".aui-tabs").find("li.menu-item:not(.aui-tabs-responsive-trigger-item)");g.removeClass(k),g.find("a").removeClass("checked").removeAttr("aria-checked")}b.closest(".tabs-menu").find("a").attr(m,"false"),b.attr(m,"true"),b.trigger("tabSelect",{tab:b,pane:c})}function e(a){return void 0!==a.attr(o)&&"false"!==a.attr(o)}function f(a){var b=a.attr("id"),c=a.attr(o);return p+(b?b:"")+(c&&"true"!==c?"-"+c:"")}function g(a){for(var b=0,c=a.length;c>b;b++){var g=a.eq(b);if(e(g)){var h=g.attr("id");if(h){var i=window.localStorage.getItem(f(g));if(i){var j=g.find("#"+i);j.length&&d(j)}}else AJS.warn("A tab group must specify an id attribute if it specifies data-aui-persist")}}}function h(a){var b=a.closest(".aui-tabs"),c=b.attr("id");if(c){var d=a.attr("id");d&&window.localStorage.setItem(f(b),d)}else AJS.warn("A tab group must specify an id attribute if it specifies data-aui-persist")
}function i(b){AJS.tabs.change(a(this),b),b&&b.preventDefault()}var j=/#.*/,k="active-tab",l="active-pane",m="aria-selected",n="aria-hidden",o="data-aui-persist",p="_internal-aui-tabs-",q=".aui-tabs.horizontal-tabs[data-aui-responsive]:not([data-aui-responsive='false'])",r={totalTabsWidth:function(a,b){var c=this.totalVisibleTabWidth(a),d=0;return b.find("li").each(function(a,b){d+=parseInt(b.getAttribute("data-aui-tab-width"))}),c+d},totalVisibleTabWidth:function(b){var c=0;return b.each(function(b,d){c+=a(d).outerWidth()}),c},removeResponsiveDropdown:function(a,b){a.remove(),b.remove()},createResponsiveDropdownTrigger:function(a,b){var c='<li class="menu-item aui-tabs-responsive-trigger-item"><a class="aui-dropdown2-trigger aui-tabs-responsive-trigger" id="aui-tabs-responsive-trigger-'+b+'" aria-haspopup="true" aria-owns="aui-tabs-responsive-dropdown-'+b+'" href="aui-tabs-responsive-dropdown-'+b+'">...</a></li>';a.append(c);var d=a.find(".aui-tabs-responsive-trigger-item");return d},createResponsiveDropdown:function(a,b){var c='<div class="aui-dropdown2 aui-style-default aui-tabs-responsive-dropdown" id="aui-tabs-responsive-dropdown-'+b+'"><ul></ul></div>';a.append(c);var d=a.find("#aui-tabs-responsive-dropdown-"+b);return d},findNewVisibleTabs:function(b,c,d){function e(a,b,c){return c>=a+b}for(var f=0,g=0;e(f,d,c)&&g<b.length;g++){var h=a(b[g]),i=h.outerWidth(!0);f+=i}return b.slice(0,g-1)},processVisibleTabs:function(b,c,d){for(var e=d.find("a").attr("aria-owns"),f=a("#"+e),g=this.findNewVisibleTabs(b,c.outerWidth(),d.parent().outerWidth(!0)),h=g.length-1,i=b.length-1;i>=h;i--){var j=a(b[i]);this.moveTabToResponsiveDropdown(j,f,d)}return a(g)},moveTabToResponsiveDropdown:function(a,b,c){var d=a.find("a");a.attr("data-aui-tab-width",a.outerWidth(!0)),d.addClass("aui-dropdown2-radio aui-tabs-responsive-item"),a.hasClass("active-tab")&&(d.addClass("aui-dropdown2-checked"),c.addClass("active-tab")),b.find("ul").prepend(a)},processInvisibleTabs:function(b,c,d){function e(a){return a>0}for(var f=0;e(c)&&f<b.length;f++){var g=a(b[f]),h=parseInt(g.attr("data-aui-tab-width"),10),i=c>h;i&&this.moveTabOutOfDropdown(g,d),c-=h}},moveTabOutOfDropdown:function(a,b){var c=a.find("a").hasClass("aui-dropdown2-checked");c&&(a.addClass("active-tab"),b.removeClass("active-tab")),a.children("a").removeClass("aui-dropdown2-radio aui-tabs-responsive-item aui-dropdown2-checked"),b.before(a)}};AJS.tabs={setup:function(){function d(a){for(var b in a)c(a[b],b)}var e=a(".aui-tabs:not(.aui-tabs-disabled)"),f=a(q).toArray();d(f);var h=AJS.debounce(d,200);a(window).resize(function(){h(f)}),e.attr("role","application"),e.find(".tabs-pane").each(function(){var b=a(this);b.attr("role","tabpanel"),b.hasClass(l)?b.attr(n,"false"):b.attr(n,"true")});for(var j=0,k=e.length;k>j;j++){var m=e.eq(j);if(!m.data("aui-tab-events-bound")){var o=m.children("ul.tabs-menu");o.attr("role","tablist"),o.children("li").attr("role","presentation"),o.find("> .menu-item a").each(b),o.delegate("a","click",i),m.data("aui-tab-events-bound",!0)}}window.localStorage&&g(e),a(".aui-tabs.vertical-tabs").find("a").each(function(){var b=a(this);if(!b.attr("title")){var c=b.children("strong:first");AJS.isClipped(c)&&b.attr("title",b.text())}})},change:function(a){d(a);var b=a.closest(".aui-tabs");window.localStorage&&e(b)&&h(a)}},a(AJS.tabs.setup)}(AJS.$),AJS.template=function(a){var b=/\{([^\}]+)\}/g,c=/(?:(?:^|\.)(.+?)(?=\[|\.|$|\()|\[('|")(.+?)\2\])(\(\))?/g,d=/([^\\])'/g,e=function(a,b,d,e){var f=d;return b.replace(c,function(a,b,c,d,g){b=b||d,f&&(b+":html"in f?(f=f[b+":html"],e=!0):b in f&&(f=f[b]),g&&"function"==typeof f&&(f=f()))}),(null==f||f==d)&&(f=a),f=String(f),e||(f=i.escape(f)),f},f=function(a){return this.template=this.template.replace(b,function(b,c){return e(b,c,a,!0)}),this},g=function(a){return this.template=this.template.replace(b,function(b,c){return e(b,c,a)}),this},h=function(){return this.template},i=function(a){function b(){return b.template}return b.template=String(a),b.toString=b.valueOf=h,b.fill=g,b.fillHtml=f,b},j={},k=[];return i.load=function(b){return b=String(b),j.hasOwnProperty(b)||(k.length>=1e3&&delete j[k.shift()],k.push(b),j[b]=a("script[title='"+b.replace(d,"$1\\'")+"']")[0].text),this(j[b])},i.escape=AJS.escapeHtml,i}(AJS.$),function(a,b){var c=-1!==navigator.platform.indexOf("Mac"),d=/^(backspace|tab|r(ight|eturn)|s(hift|pace|croll)|c(trl|apslock)|alt|pa(use|ge(up|down))|e(sc|nd)|home|left|up|d(el|own)|insert|f\d\d?|numlock|meta)/i;a.whenIType=function(e){function f(a){!AJS.popup.current&&p&&p.fire(a)}function g(a){a.preventDefault()}function h(a){var c=a&&a.split?b.trim(a).split(" "):[a];b.each(c,function(){j(this)})}function i(a){for(var b=a.length;b--;)if(a[b].length>1&&"space"!==a[b])return!0;return!1}function j(a){var c=a instanceof Array?a:k(a.toString()),d=i(c)?"keydown":"keypress";o.push(c),b(document).bind(d,c,f),b(document).bind(d+" keyup",c,g)}function k(a){for(var b,c,e=[],f="";a.length;)(b=a.match(/^(ctrl|meta|shift|alt)\+/i))?(f+=b[0],a=a.substring(b[0].length)):(c=a.match(d))?(e.push(f+c[0]),a=a.substring(c[0].length),f=""):(e.push(f+a[0]),a=a.substring(1),f="");return e}function l(a){for(var d=b(a),e=d.attr("title")||"",f=o.slice(),g=d.data("kbShortcutAppended")||"",h=!g,i=h?e:e.substring(0,e.length-g.length);f.length;)g=n(f.shift().slice(),g,h),h=!1;c&&(g=g.replace(/Meta/gi,"\u2318").replace(/Shift/gi,"\u21e7")),d.attr("title",i+g),d.data("kbShortcutAppended",g)}function m(a){var c=b(a),d=c.data("kbShortcutAppended");if(d){var e=c.attr("title");c.attr("title",e.replace(d,"")),c.removeData("kbShortcutAppended")}}function n(a,c,d){return d?c+=" ("+AJS.I18n.getText("aui.keyboard.shortcut.type.x",a.shift()):(c=c.replace(/\)$/,""),c+=AJS.I18n.getText("aui.keyboard.shortcut.or.x",a.shift())),b.each(a,function(){c+=" "+AJS.I18n.getText("aui.keyboard.shortcut.then.x",this)}),c+=")"}var o=[],p=b.Callbacks();return h(e),a.whenIType.makeShortcut({executor:p,bindKeys:h,addShortcutsToTitle:l,removeShortcutsFromTitle:m,keypressHandler:f,defaultPreventionHandler:g})},a.whenIType.makeShortcut=function(a){function c(a){return function(c,e){e=e||{};var f=e.focusedClass||"focused",g=e.hasOwnProperty("wrapAround")?e.wrapAround:!0,h=e.hasOwnProperty("escToCancel")?e.escToCancel:!0;return d.add(function(){var d=b(c),e=d.filter("."+f),i=0===e.length?void 0:{transition:!0};h&&b(document).one("keydown",function(a){a.keyCode===AJS.keyCode.ESCAPE&&e&&e.removeClass(f)}),e.length&&e.removeClass(f),e=a(e,d,g),e&&e.length>0&&(e.addClass(f),e.moveTo(i),e.is("a")?e.focus():e.find("a:first").focus())}),this}}var d=a.executor,e=a.bindKeys,f=a.addShortcutsToTitle,g=a.removeShortcutsFromTitle,h=a.keypressHandler,i=a.defaultPreventionHandler,j=[];return{moveToNextItem:c(function(a,c,d){var e;return d&&0===a.length?c.eq(0):(e=b.inArray(a.get(0),c),e<c.length-1?(e+=1,c.eq(e)):d?c.eq(0):a)}),moveToPrevItem:c(function(a,c,d){var e;return d&&0===a.length?c.filter(":last"):(e=b.inArray(a.get(0),c),e>0?(e-=1,c.eq(e)):d?c.filter(":last"):a)}),click:function(a){return j.push(a),f(a),d.add(function(){var c=b(a);c.length>0&&c.click()}),this},goTo:function(a){return d.add(function(){window.location.href=a}),this},followLink:function(a){return j.push(a),f(a),d.add(function(){var c=b(a)[0];c&&{a:!0,link:!0}[c.nodeName.toLowerCase()]&&(window.location.href=c.href)}),this},execute:function(a){var b=this;return d.add(function(){a.apply(b,arguments)}),this},evaluate:function(a){a.call(this)},moveToAndClick:function(a){return j.push(a),f(a),d.add(function(){var c=b(a);c.length>0&&(c.click(),c.moveTo())}),this},moveToAndFocus:function(a){return j.push(a),f(a),d.add(function(b){var c=AJS.$(a);c.length>0&&(c.focus(),c.moveTo&&c.moveTo(),c.is(":input")&&b.preventDefault())}),this},or:function(a){return e(a),this},unbind:function(){b(document).unbind("keydown keypress",h).unbind("keydown keypress keyup",i);for(var a=0,c=j.length;c>a;a++)g(j[a]);j=[]}}},a.whenIType.fromJSON=function(a,d){var e=[];return a&&b.each(a,function(a,f){var g,h=f.op,i=f.param;if("execute"===h||"evaluate"===h)g=[new Function(i)];else if(/^\[[^\]\[]*,[^\]\[]*\]$/.test(i)){try{g=JSON.parse(i)}catch(j){AJS.error("When using a parameter array, array must be in strict JSON format: "+i)}b.isArray(g)||AJS.error("Badly formatted shortcut parameter. String or JSON Array of parameters required: "+i)}else g=[i];b.each(f.keys,function(){var a=this;d&&c&&(a=b.map(this,function(a){return a.replace(/ctrl/i,"meta")}));var f=AJS.whenIType(a);f[h].apply(f,g),e.push(f)})}),e},b(document).bind("iframeAppended",function(a,c){b(c).load(function(){var a=b(c).contents();a.bind("keyup keydown keypress",function(a){b.browser.safari&&"keypress"===a.type||b(a.target).is(":input")||b.event.trigger(a,arguments,document,!0)})})})}(AJS,AJS.$),function(a){AJS.responsiveheader={},AJS.responsiveheader.setup=function(){function b(b,c){function d(a){var b;if(e(),!(n>o)){k.show(),b=n-q;for(var c=0;b>=0;c++)b-=m[c].itemWidth;return c-=1,h(c,a),g(c,l,a),c}i(a)}function e(){var b=0!==j.length?j.offset().left:a(window).width(),c=p.offset().left+p.outerWidth(!0)+s;n=b-c}function f(b){var c=a("<li>"+aui.dropdown2.trigger({menu:{id:"aui-responsive-header-dropdown-content-"+b},text:AJS.I18n.getText("aui.words.more"),extraAttributes:{href:"#"},id:"aui-responsive-header-dropdown-trigger-"+b})+"</li>");c.append(aui.dropdown2.contents({id:"aui-responsive-header-dropdown-content-"+b,extraClasses:"aui-style-default",content:aui.dropdown2.section({content:"<ul id='aui-responsive-header-dropdown-list-"+b+"'></ul>"})})),0===s?c.appendTo(r(".aui-nav")):c.insertBefore(r(".aui-nav > li > .aui-button").first().parent()),k=c,q=k.outerWidth(!0)}function g(b,c,d){if(!(0>b||0>c||b===c)){var e,f,g=a("#aui-responsive-header-dropdown-trigger-"+d),h=g.parent();g.hasClass("active")&&g.trigger("aui-button-invoke");for(var i=r(".aui-nav > li > a:not(.aui-button):not(#aui-responsive-header-dropdown-trigger-"+d+")").length;b>c;)e=m[c],e&&e.itemElement&&(f=e.itemElement,0===i?f.prependTo(r(".aui-nav")):f.insertBefore(h),f.children("a").removeClass("aui-dropdown2-sub-trigger active"),c+=1,i+=1)}}function h(b,c){if(!(0>b))for(var d=a("#aui-responsive-header-dropdown-list-"+c),e=b;e<m.length;e++){m[e].itemElement.appendTo(d);var f=m[e].itemElement.children("a");f.hasClass("aui-dropdown2-trigger")&&f.addClass("aui-dropdown2-sub-trigger")}}function i(a){k.hide(),g(m.length,l,a)}var j=b.find(".aui-header-secondary .aui-nav").first();a(".aui-header").attr("data-aui-responsive","true");var k,l,m=[],n=0,o=0,p=b.find("#logo"),q=0,r=function(){var a=b.find(".aui-header-primary").first();return function(b){return a.find(b)}}(),s=0;r(".aui-button").parent().each(function(b,c){s+=a(c).outerWidth(!0)}),r(".aui-nav > li > a:not(.aui-button)").each(function(b,c){var d=a(c).parent(),e=d.outerWidth(!0);m.push({itemElement:d,itemWidth:e}),o+=e}),l=m.length,a(window).resize(function(){l=d(c)}),f(c);var t=p.find("img");0!==t.length&&(t.attr("data-aui-responsive-header-index",c),t.load(function(){l=d(c)})),l=d(c),r(".aui-nav").css("width","auto")}var c=a(".aui-header");c.length&&c.each(function(c,d){b(a(d),c)})}}(AJS.$),AJS.$(AJS.responsiveheader.setup);
},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\QuoteFlow\\Content\\js\\lib\\html-sanitizer-bundle.js":[function(require,module,exports){
// Copyright (C) 2010 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

/**
 * @fileoverview
 * Implements RFC 3986 for parsing/formatting URIs.
 *
 * @author mikesamuel@gmail.com
 * \@provides URI
 * \@overrides window
 */

var URI = (function () {

    /**
     * creates a uri from the string form.  The parser is relaxed, so special
     * characters that aren't escaped but don't cause ambiguities will not cause
     * parse failures.
     *
     * @return {URI|null}
     */
    function parse(uriStr) {
        var m = ('' + uriStr).match(URI_RE_);
        if (!m) { return null; }
        return new URI(
            nullIfAbsent(m[1]),
            nullIfAbsent(m[2]),
            nullIfAbsent(m[3]),
            nullIfAbsent(m[4]),
            nullIfAbsent(m[5]),
            nullIfAbsent(m[6]),
            nullIfAbsent(m[7]));
    }


    /**
     * creates a uri from the given parts.
     *
     * @param scheme {string} an unencoded scheme such as "http" or null
     * @param credentials {string} unencoded user credentials or null
     * @param domain {string} an unencoded domain name or null
     * @param port {number} a port number in [1, 32768].
     *    -1 indicates no port, as does null.
     * @param path {string} an unencoded path
     * @param query {Array.<string>|string|null} a list of unencoded cgi
     *   parameters where even values are keys and odds the corresponding values
     *   or an unencoded query.
     * @param fragment {string} an unencoded fragment without the "#" or null.
     * @return {URI}
     */
    function create(scheme, credentials, domain, port, path, query, fragment) {
        var uri = new URI(
            encodeIfExists2(scheme, URI_DISALLOWED_IN_SCHEME_OR_CREDENTIALS_),
            encodeIfExists2(
                credentials, URI_DISALLOWED_IN_SCHEME_OR_CREDENTIALS_),
            encodeIfExists(domain),
            port > 0 ? port.toString() : null,
            encodeIfExists2(path, URI_DISALLOWED_IN_PATH_),
            null,
            encodeIfExists(fragment));
        if (query) {
            if ('string' === typeof query) {
                uri.setRawQuery(query.replace(/[^?&=0-9A-Za-z_\-~.%]/g, encodeOne));
            } else {
                uri.setAllParameters(query);
            }
        }
        return uri;
    }
    function encodeIfExists(unescapedPart) {
        if ('string' == typeof unescapedPart) {
            return encodeURIComponent(unescapedPart);
        }
        return null;
    };
    /**
     * if unescapedPart is non null, then escapes any characters in it that aren't
     * valid characters in a url and also escapes any special characters that
     * appear in extra.
     *
     * @param unescapedPart {string}
     * @param extra {RegExp} a character set of characters in [\01-\177].
     * @return {string|null} null iff unescapedPart == null.
     */
    function encodeIfExists2(unescapedPart, extra) {
        if ('string' == typeof unescapedPart) {
            return encodeURI(unescapedPart).replace(extra, encodeOne);
        }
        return null;
    };
    /** converts a character in [\01-\177] to its url encoded equivalent. */
    function encodeOne(ch) {
        var n = ch.charCodeAt(0);
        return '%' + '0123456789ABCDEF'.charAt((n >> 4) & 0xf) +
            '0123456789ABCDEF'.charAt(n & 0xf);
    }

    /**
     * {@updoc
     *  $ normPath('foo/./bar')
     *  # 'foo/bar'
     *  $ normPath('./foo')
     *  # 'foo'
     *  $ normPath('foo/.')
     *  # 'foo'
     *  $ normPath('foo//bar')
     *  # 'foo/bar'
     * }
     */
    function normPath(path) {
        return path.replace(/(^|\/)\.(?:\/|$)/g, '$1').replace(/\/{2,}/g, '/');
    }

    var PARENT_DIRECTORY_HANDLER = new RegExp(
        ''
        // A path break
        + '(/|^)'
        // followed by a non .. path element
        // (cannot be . because normPath is used prior to this RegExp)
        + '(?:[^./][^/]*|\\.{2,}(?:[^./][^/]*)|\\.{3,}[^/]*)'
        // followed by .. followed by a path break.
        + '/\\.\\.(?:/|$)');

    var PARENT_DIRECTORY_HANDLER_RE = new RegExp(PARENT_DIRECTORY_HANDLER);

    var EXTRA_PARENT_PATHS_RE = /^(?:\.\.\/)*(?:\.\.$)?/;

    /**
     * Normalizes its input path and collapses all . and .. sequences except for
     * .. sequences that would take it above the root of the current parent
     * directory.
     * {@updoc
     *  $ collapse_dots('foo/../bar')
     *  # 'bar'
     *  $ collapse_dots('foo/./bar')
     *  # 'foo/bar'
     *  $ collapse_dots('foo/../bar/./../../baz')
     *  # 'baz'
     *  $ collapse_dots('../foo')
     *  # '../foo'
     *  $ collapse_dots('../foo').replace(EXTRA_PARENT_PATHS_RE, '')
     *  # 'foo'
     * }
     */
    function collapse_dots(path) {
        if (path === null) { return null; }
        var p = normPath(path);
        // Only /../ left to flatten
        var r = PARENT_DIRECTORY_HANDLER_RE;
        // We replace with $1 which matches a / before the .. because this
        // guarantees that:
        // (1) we have at most 1 / between the adjacent place,
        // (2) always have a slash if there is a preceding path section, and
        // (3) we never turn a relative path into an absolute path.
        for (var q; (q = p.replace(r, '$1')) != p; p = q) { };
        return p;
    }

    /**
     * resolves a relative url string to a base uri.
     * @return {URI}
     */
    function resolve(baseUri, relativeUri) {
        // there are several kinds of relative urls:
        // 1. //foo - replaces everything from the domain on.  foo is a domain name
        // 2. foo - replaces the last part of the path, the whole query and fragment
        // 3. /foo - replaces the the path, the query and fragment
        // 4. ?foo - replace the query and fragment
        // 5. #foo - replace the fragment only

        var absoluteUri = baseUri.clone();
        // we satisfy these conditions by looking for the first part of relativeUri
        // that is not blank and applying defaults to the rest

        var overridden = relativeUri.hasScheme();

        if (overridden) {
            absoluteUri.setRawScheme(relativeUri.getRawScheme());
        } else {
            overridden = relativeUri.hasCredentials();
        }

        if (overridden) {
            absoluteUri.setRawCredentials(relativeUri.getRawCredentials());
        } else {
            overridden = relativeUri.hasDomain();
        }

        if (overridden) {
            absoluteUri.setRawDomain(relativeUri.getRawDomain());
        } else {
            overridden = relativeUri.hasPort();
        }

        var rawPath = relativeUri.getRawPath();
        var simplifiedPath = collapse_dots(rawPath);
        if (overridden) {
            absoluteUri.setPort(relativeUri.getPort());
            simplifiedPath = simplifiedPath
                && simplifiedPath.replace(EXTRA_PARENT_PATHS_RE, '');
        } else {
            overridden = !!rawPath;
            if (overridden) {
                // resolve path properly
                if (simplifiedPath.charCodeAt(0) !== 0x2f /* / */) {  // path is relative
                    var absRawPath = collapse_dots(absoluteUri.getRawPath() || '')
                        .replace(EXTRA_PARENT_PATHS_RE, '');
                    var slash = absRawPath.lastIndexOf('/') + 1;
                    simplifiedPath = collapse_dots(
                        (slash ? absRawPath.substring(0, slash) : '')
                        + collapse_dots(rawPath))
                        .replace(EXTRA_PARENT_PATHS_RE, '');
                }
            } else {
                simplifiedPath = simplifiedPath
                    && simplifiedPath.replace(EXTRA_PARENT_PATHS_RE, '');
                if (simplifiedPath !== rawPath) {
                    absoluteUri.setRawPath(simplifiedPath);
                }
            }
        }

        if (overridden) {
            absoluteUri.setRawPath(simplifiedPath);
        } else {
            overridden = relativeUri.hasQuery();
        }

        if (overridden) {
            absoluteUri.setRawQuery(relativeUri.getRawQuery());
        } else {
            overridden = relativeUri.hasFragment();
        }

        if (overridden) {
            absoluteUri.setRawFragment(relativeUri.getRawFragment());
        }

        return absoluteUri;
    }

    /**
     * a mutable URI.
     *
     * This class contains setters and getters for the parts of the URI.
     * The <tt>getXYZ</tt>/<tt>setXYZ</tt> methods return the decoded part -- so
     * <code>uri.parse('/foo%20bar').getPath()</code> will return the decoded path,
     * <tt>/foo bar</tt>.
     *
     * <p>The raw versions of fields are available too.
     * <code>uri.parse('/foo%20bar').getRawPath()</code> will return the raw path,
     * <tt>/foo%20bar</tt>.  Use the raw setters with care, since
     * <code>URI::toString</code> is not guaranteed to return a valid url if a
     * raw setter was used.
     *
     * <p>All setters return <tt>this</tt> and so may be chained, a la
     * <code>uri.parse('/foo').setFragment('part').toString()</code>.
     *
     * <p>You should not use this constructor directly -- please prefer the factory
     * functions {@link uri.parse}, {@link uri.create}, {@link uri.resolve}
     * instead.</p>
     *
     * <p>The parameters are all raw (assumed to be properly escaped) parts, and
     * any (but not all) may be null.  Undefined is not allowed.</p>
     *
     * @constructor
     */
    function URI(
        rawScheme,
        rawCredentials, rawDomain, port,
        rawPath, rawQuery, rawFragment) {
        this.scheme_ = rawScheme;
        this.credentials_ = rawCredentials;
        this.domain_ = rawDomain;
        this.port_ = port;
        this.path_ = rawPath;
        this.query_ = rawQuery;
        this.fragment_ = rawFragment;
        /**
         * @type {Array|null}
         */
        this.paramCache_ = null;
    }

    /** returns the string form of the url. */
    URI.prototype.toString = function () {
        var out = [];
        if (null !== this.scheme_) { out.push(this.scheme_, ':'); }
        if (null !== this.domain_) {
            out.push('//');
            if (null !== this.credentials_) { out.push(this.credentials_, '@'); }
            out.push(this.domain_);
            if (null !== this.port_) { out.push(':', this.port_.toString()); }
        }
        if (null !== this.path_) { out.push(this.path_); }
        if (null !== this.query_) { out.push('?', this.query_); }
        if (null !== this.fragment_) { out.push('#', this.fragment_); }
        return out.join('');
    };

    URI.prototype.clone = function () {
        return new URI(this.scheme_, this.credentials_, this.domain_, this.port_,
                       this.path_, this.query_, this.fragment_);
    };

    URI.prototype.getScheme = function () {
        // HTML5 spec does not require the scheme to be lowercased but
        // all common browsers except Safari lowercase the scheme.
        return this.scheme_ && decodeURIComponent(this.scheme_).toLowerCase();
    };
    URI.prototype.getRawScheme = function () {
        return this.scheme_;
    };
    URI.prototype.setScheme = function (newScheme) {
        this.scheme_ = encodeIfExists2(
            newScheme, URI_DISALLOWED_IN_SCHEME_OR_CREDENTIALS_);
        return this;
    };
    URI.prototype.setRawScheme = function (newScheme) {
        this.scheme_ = newScheme ? newScheme : null;
        return this;
    };
    URI.prototype.hasScheme = function () {
        return null !== this.scheme_;
    };


    URI.prototype.getCredentials = function () {
        return this.credentials_ && decodeURIComponent(this.credentials_);
    };
    URI.prototype.getRawCredentials = function () {
        return this.credentials_;
    };
    URI.prototype.setCredentials = function (newCredentials) {
        this.credentials_ = encodeIfExists2(
            newCredentials, URI_DISALLOWED_IN_SCHEME_OR_CREDENTIALS_);

        return this;
    };
    URI.prototype.setRawCredentials = function (newCredentials) {
        this.credentials_ = newCredentials ? newCredentials : null;
        return this;
    };
    URI.prototype.hasCredentials = function () {
        return null !== this.credentials_;
    };


    URI.prototype.getDomain = function () {
        return this.domain_ && decodeURIComponent(this.domain_);
    };
    URI.prototype.getRawDomain = function () {
        return this.domain_;
    };
    URI.prototype.setDomain = function (newDomain) {
        return this.setRawDomain(newDomain && encodeURIComponent(newDomain));
    };
    URI.prototype.setRawDomain = function (newDomain) {
        this.domain_ = newDomain ? newDomain : null;
        // Maintain the invariant that paths must start with a slash when the URI
        // is not path-relative.
        return this.setRawPath(this.path_);
    };
    URI.prototype.hasDomain = function () {
        return null !== this.domain_;
    };


    URI.prototype.getPort = function () {
        return this.port_ && decodeURIComponent(this.port_);
    };
    URI.prototype.setPort = function (newPort) {
        if (newPort) {
            newPort = Number(newPort);
            if (newPort !== (newPort & 0xffff)) {
                throw new Error('Bad port number ' + newPort);
            }
            this.port_ = '' + newPort;
        } else {
            this.port_ = null;
        }
        return this;
    };
    URI.prototype.hasPort = function () {
        return null !== this.port_;
    };


    URI.prototype.getPath = function () {
        return this.path_ && decodeURIComponent(this.path_);
    };
    URI.prototype.getRawPath = function () {
        return this.path_;
    };
    URI.prototype.setPath = function (newPath) {
        return this.setRawPath(encodeIfExists2(newPath, URI_DISALLOWED_IN_PATH_));
    };
    URI.prototype.setRawPath = function (newPath) {
        if (newPath) {
            newPath = String(newPath);
            this.path_ =
              // Paths must start with '/' unless this is a path-relative URL.
              (!this.domain_ || /^\//.test(newPath)) ? newPath : '/' + newPath;
        } else {
            this.path_ = null;
        }
        return this;
    };
    URI.prototype.hasPath = function () {
        return null !== this.path_;
    };


    URI.prototype.getQuery = function () {
        // From http://www.w3.org/Addressing/URL/4_URI_Recommentations.html
        // Within the query string, the plus sign is reserved as shorthand notation
        // for a space.
        return this.query_ && decodeURIComponent(this.query_).replace(/\+/g, ' ');
    };
    URI.prototype.getRawQuery = function () {
        return this.query_;
    };
    URI.prototype.setQuery = function (newQuery) {
        this.paramCache_ = null;
        this.query_ = encodeIfExists(newQuery);
        return this;
    };
    URI.prototype.setRawQuery = function (newQuery) {
        this.paramCache_ = null;
        this.query_ = newQuery ? newQuery : null;
        return this;
    };
    URI.prototype.hasQuery = function () {
        return null !== this.query_;
    };

    /**
     * sets the query given a list of strings of the form
     * [ key0, value0, key1, value1, ... ].
     *
     * <p><code>uri.setAllParameters(['a', 'b', 'c', 'd']).getQuery()</code>
     * will yield <code>'a=b&c=d'</code>.
     */
    URI.prototype.setAllParameters = function (params) {
        if (typeof params === 'object') {
            if (!(params instanceof Array)
                && (params instanceof Object
                    || Object.prototype.toString.call(params) !== '[object Array]')) {
                var newParams = [];
                var i = -1;
                for (var k in params) {
                    var v = params[k];
                    if ('string' === typeof v) {
                        newParams[++i] = k;
                        newParams[++i] = v;
                    }
                }
                params = newParams;
            }
        }
        this.paramCache_ = null;
        var queryBuf = [];
        var separator = '';
        for (var j = 0; j < params.length;) {
            var k = params[j++];
            var v = params[j++];
            queryBuf.push(separator, encodeURIComponent(k.toString()));
            separator = '&';
            if (v) {
                queryBuf.push('=', encodeURIComponent(v.toString()));
            }
        }
        this.query_ = queryBuf.join('');
        return this;
    };
    URI.prototype.checkParameterCache_ = function () {
        if (!this.paramCache_) {
            var q = this.query_;
            if (!q) {
                this.paramCache_ = [];
            } else {
                var cgiParams = q.split(/[&\?]/);
                var out = [];
                var k = -1;
                for (var i = 0; i < cgiParams.length; ++i) {
                    var m = cgiParams[i].match(/^([^=]*)(?:=(.*))?$/);
                    // From http://www.w3.org/Addressing/URL/4_URI_Recommentations.html
                    // Within the query string, the plus sign is reserved as shorthand
                    // notation for a space.
                    out[++k] = decodeURIComponent(m[1]).replace(/\+/g, ' ');
                    out[++k] = decodeURIComponent(m[2] || '').replace(/\+/g, ' ');
                }
                this.paramCache_ = out;
            }
        }
    };
    /**
     * sets the values of the named cgi parameters.
     *
     * <p>So, <code>uri.parse('foo?a=b&c=d&e=f').setParameterValues('c', ['new'])
     * </code> yields <tt>foo?a=b&c=new&e=f</tt>.</p>
     *
     * @param key {string}
     * @param values {Array.<string>} the new values.  If values is a single string
     *   then it will be treated as the sole value.
     */
    URI.prototype.setParameterValues = function (key, values) {
        // be nice and avoid subtle bugs where [] operator on string performs charAt
        // on some browsers and crashes on IE
        if (typeof values === 'string') {
            values = [values];
        }

        this.checkParameterCache_();
        var newValueIndex = 0;
        var pc = this.paramCache_;
        var params = [];
        for (var i = 0, k = 0; i < pc.length; i += 2) {
            if (key === pc[i]) {
                if (newValueIndex < values.length) {
                    params.push(key, values[newValueIndex++]);
                }
            } else {
                params.push(pc[i], pc[i + 1]);
            }
        }
        while (newValueIndex < values.length) {
            params.push(key, values[newValueIndex++]);
        }
        this.setAllParameters(params);
        return this;
    };
    URI.prototype.removeParameter = function (key) {
        return this.setParameterValues(key, []);
    };
    /**
     * returns the parameters specified in the query part of the uri as a list of
     * keys and values like [ key0, value0, key1, value1, ... ].
     *
     * @return {Array.<string>}
     */
    URI.prototype.getAllParameters = function () {
        this.checkParameterCache_();
        return this.paramCache_.slice(0, this.paramCache_.length);
    };
    /**
     * returns the value<b>s</b> for a given cgi parameter as a list of decoded
     * query parameter values.
     * @return {Array.<string>}
     */
    URI.prototype.getParameterValues = function (paramNameUnescaped) {
        this.checkParameterCache_();
        var values = [];
        for (var i = 0; i < this.paramCache_.length; i += 2) {
            if (paramNameUnescaped === this.paramCache_[i]) {
                values.push(this.paramCache_[i + 1]);
            }
        }
        return values;
    };
    /**
     * returns a map of cgi parameter names to (non-empty) lists of values.
     * @return {Object.<string,Array.<string>>}
     */
    URI.prototype.getParameterMap = function (paramNameUnescaped) {
        this.checkParameterCache_();
        var paramMap = {};
        for (var i = 0; i < this.paramCache_.length; i += 2) {
            var key = this.paramCache_[i++],
              value = this.paramCache_[i++];
            if (!(key in paramMap)) {
                paramMap[key] = [value];
            } else {
                paramMap[key].push(value);
            }
        }
        return paramMap;
    };
    /**
     * returns the first value for a given cgi parameter or null if the given
     * parameter name does not appear in the query string.
     * If the given parameter name does appear, but has no '<tt>=</tt>' following
     * it, then the empty string will be returned.
     * @return {string|null}
     */
    URI.prototype.getParameterValue = function (paramNameUnescaped) {
        this.checkParameterCache_();
        for (var i = 0; i < this.paramCache_.length; i += 2) {
            if (paramNameUnescaped === this.paramCache_[i]) {
                return this.paramCache_[i + 1];
            }
        }
        return null;
    };

    URI.prototype.getFragment = function () {
        return this.fragment_ && decodeURIComponent(this.fragment_);
    };
    URI.prototype.getRawFragment = function () {
        return this.fragment_;
    };
    URI.prototype.setFragment = function (newFragment) {
        this.fragment_ = newFragment ? encodeURIComponent(newFragment) : null;
        return this;
    };
    URI.prototype.setRawFragment = function (newFragment) {
        this.fragment_ = newFragment ? newFragment : null;
        return this;
    };
    URI.prototype.hasFragment = function () {
        return null !== this.fragment_;
    };

    function nullIfAbsent(matchPart) {
        return ('string' == typeof matchPart) && (matchPart.length > 0)
               ? matchPart
               : null;
    }




    /**
     * a regular expression for breaking a URI into its component parts.
     *
     * <p>http://www.gbiv.com/protocols/uri/rfc/rfc3986.html#RFC2234 says
     * As the "first-match-wins" algorithm is identical to the "greedy"
     * disambiguation method used by POSIX regular expressions, it is natural and
     * commonplace to use a regular expression for parsing the potential five
     * components of a URI reference.
     *
     * <p>The following line is the regular expression for breaking-down a
     * well-formed URI reference into its components.
     *
     * <pre>
     * ^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?
     *  12            3  4          5       6  7        8 9
     * </pre>
     *
     * <p>The numbers in the second line above are only to assist readability; they
     * indicate the reference points for each subexpression (i.e., each paired
     * parenthesis). We refer to the value matched for subexpression <n> as $<n>.
     * For example, matching the above expression to
     * <pre>
     *     http://www.ics.uci.edu/pub/ietf/uri/#Related
     * </pre>
     * results in the following subexpression matches:
     * <pre>
     *    $1 = http:
     *    $2 = http
     *    $3 = //www.ics.uci.edu
     *    $4 = www.ics.uci.edu
     *    $5 = /pub/ietf/uri/
     *    $6 = <undefined>
     *    $7 = <undefined>
     *    $8 = #Related
     *    $9 = Related
     * </pre>
     * where <undefined> indicates that the component is not present, as is the
     * case for the query component in the above example. Therefore, we can
     * determine the value of the five components as
     * <pre>
     *    scheme    = $2
     *    authority = $4
     *    path      = $5
     *    query     = $7
     *    fragment  = $9
     * </pre>
     *
     * <p>msamuel: I have modified the regular expression slightly to expose the
     * credentials, domain, and port separately from the authority.
     * The modified version yields
     * <pre>
     *    $1 = http              scheme
     *    $2 = <undefined>       credentials -\
     *    $3 = www.ics.uci.edu   domain       | authority
     *    $4 = <undefined>       port        -/
     *    $5 = /pub/ietf/uri/    path
     *    $6 = <undefined>       query without ?
     *    $7 = Related           fragment without #
     * </pre>
     */
    var URI_RE_ = new RegExp(
          "^" +
          "(?:" +
            "([^:/?#]+)" +         // scheme
          ":)?" +
          "(?://" +
            "(?:([^/?#]*)@)?" +    // credentials
            "([^/?#:@]*)" +        // domain
            "(?::([0-9]+))?" +     // port
          ")?" +
          "([^?#]+)?" +            // path
          "(?:\\?([^#]*))?" +      // query
          "(?:#(.*))?" +           // fragment
          "$"
          );

    var URI_DISALLOWED_IN_SCHEME_OR_CREDENTIALS_ = /[#\/\?@]/g;
    var URI_DISALLOWED_IN_PATH_ = /[\#\?]/g;

    URI.parse = parse;
    URI.create = create;
    URI.resolve = resolve;
    URI.collapse_dots = collapse_dots;  // Visible for testing.

    // lightweight string-based api for loadModuleMaker
    URI.utils = {
        mimeTypeOf: function (uri) {
            var uriObj = parse(uri);
            if (/\.html$/.test(uriObj.getPath())) {
                return 'text/html';
            } else {
                return 'application/javascript';
            }
        },
        resolve: function (base, uri) {
            if (base) {
                return resolve(parse(base), parse(uri)).toString();
            } else {
                return '' + uri;
            }
        }
    };


    return URI;
})();

// Exports for closure compiler.
if (typeof window !== 'undefined') {
    window['URI'] = URI;
}
;
// Copyright Google Inc.
// Licensed under the Apache Licence Version 2.0
// Autogenerated at Mon Oct 21 13:30:08 EDT 2013
// @overrides window
// @provides html4
var html4 = {};
html4.atype = {
    'NONE': 0,
    'URI': 1,
    'URI_FRAGMENT': 11,
    'SCRIPT': 2,
    'STYLE': 3,
    'HTML': 12,
    'ID': 4,
    'IDREF': 5,
    'IDREFS': 6,
    'GLOBAL_NAME': 7,
    'LOCAL_NAME': 8,
    'CLASSES': 9,
    'FRAME_TARGET': 10,
    'MEDIA_QUERY': 13
};
html4['atype'] = html4.atype;
html4.ATTRIBS = {
    '*::class': 9,
    '*::dir': 0,
    '*::draggable': 0,
    '*::hidden': 0,
    '*::id': 4,
    '*::inert': 0,
    '*::itemprop': 0,
    '*::itemref': 6,
    '*::itemscope': 0,
    '*::lang': 0,
    '*::onblur': 2,
    '*::onchange': 2,
    '*::onclick': 2,
    '*::ondblclick': 2,
    '*::onerror': 2,
    '*::onfocus': 2,
    '*::onkeydown': 2,
    '*::onkeypress': 2,
    '*::onkeyup': 2,
    '*::onload': 2,
    '*::onmousedown': 2,
    '*::onmousemove': 2,
    '*::onmouseout': 2,
    '*::onmouseover': 2,
    '*::onmouseup': 2,
    '*::onreset': 2,
    '*::onscroll': 2,
    '*::onselect': 2,
    '*::onsubmit': 2,
    '*::onunload': 2,
    '*::spellcheck': 0,
    '*::style': 3,
    '*::title': 0,
    '*::translate': 0,
    'a::accesskey': 0,
    'a::coords': 0,
    'a::href': 1,
    'a::hreflang': 0,
    'a::name': 7,
    'a::onblur': 2,
    'a::onfocus': 2,
    'a::shape': 0,
    'a::tabindex': 0,
    'a::target': 10,
    'a::type': 0,
    'bdo::dir': 0,
    'blockquote::cite': 1,
    'br::clear': 0,
    'caption::align': 0,
    'col::align': 0,
    'col::char': 0,
    'col::charoff': 0,
    'col::span': 0,
    'col::valign': 0,
    'col::width': 0,
    'colgroup::align': 0,
    'colgroup::char': 0,
    'colgroup::charoff': 0,
    'colgroup::span': 0,
    'colgroup::valign': 0,
    'colgroup::width': 0,
    'data::value': 0,
    'del::cite': 1,
    'del::datetime': 0,
    'details::open': 0,
    'dir::compact': 0,
    'div::align': 0,
    'dl::compact': 0,
    'h1::align': 0,
    'h2::align': 0,
    'h3::align': 0,
    'h4::align': 0,
    'h5::align': 0,
    'h6::align': 0,
    'hr::align': 0,
    'hr::noshade': 0,
    'hr::size': 0,
    'hr::width': 0,
    'iframe::align': 0,
    'iframe::frameborder': 0,
    'iframe::height': 0,
    'iframe::marginheight': 0,
    'iframe::marginwidth': 0,
    'iframe::width': 0,
    'iframe::src': 1,
    'img::alt': 0,
    'img::height': 0,
    'img::name': 7,
    'img::src': 1,
    'img::width': 0,
    'ins::cite': 1,
    'ins::datetime': 0,
    'label::accesskey': 0,
    'label::for': 5,
    'label::onblur': 2,
    'label::onfocus': 2,
    'legend::accesskey': 0,
    'legend::align': 0,
    'li::type': 0,
    'li::value': 0,
    'meter::high': 0,
    'meter::low': 0,
    'meter::max': 0,
    'meter::min': 0,
    'meter::value': 0,
    'ol::compact': 0,
    'ol::reversed': 0,
    'ol::start': 0,
    'ol::type': 0,
    'p::align': 0,
    'pre::width': 0,
    'q::cite': 1,
    'source::type': 0,
    'track::default': 0,
    'track::kind': 0,
    'track::label': 0,
    'track::srclang': 0,
    'ul::compact': 0,
    'ul::type': 0,
};
html4['ATTRIBS'] = html4.ATTRIBS;
html4.eflags = {
    'OPTIONAL_ENDTAG': 1,
    'EMPTY': 2,
    'CDATA': 4,
    'RCDATA': 8,
    'UNSAFE': 16,
    'FOLDABLE': 32,
    'SCRIPT': 64,
    'STYLE': 128,
    'VIRTUALIZED': 256
};
html4['eflags'] = html4.eflags;
html4.ELEMENTS = {
    'a': 0,
    'abbr': 0,
    'acronym': 0,
    'address': 0,
    'article': 0,
    'aside': 0,
    'b': 0,
    'base': 274,
    'bdi': 0,
    'bdo': 0,
    'big': 0,
    'blockquote': 0,
    'body': 305,
    'br': 2,
    'caption': 0,
    'cite': 0,
    'code': 0,
    'col': 2,
    'colgroup': 1,
    'data': 0,
    'dd': 1,
    'del': 0,
    'details': 0,
    'dfn': 0,
    'dialog': 272,
    'dir': 0,
    'div': 0,
    'dl': 0,
    'dt': 1,
    'em': 0,
    'figcaption': 0,
    'figure': 0,
    'frame': 274,
    'frameset': 272,
    'h1': 0,
    'h2': 0,
    'h3': 0,
    'h4': 0,
    'h5': 0,
    'h6': 0,
    'head': 305,
    'header': 0,
    'hgroup': 0,
    'hr': 2,
    'html': 305,
    'i': 0,
    'iframe': 4,
    'img': 2,
    'ins': 0,
    'isindex': 274,
    'kbd': 0,
    'keygen': 274,
    'label': 0,
    'legend': 0,
    'li': 1,
    'link': 274,
    'meter': 0,
    'nav': 0,
    'nobr': 0,
    'noembed': 276,
    'noframes': 276,
    'noscript': 276,
    'object': 272,
    'ol': 0,
    'p': 1,
    'param': 274,
    'pre': 0,
    'q': 0,
    's': 0,
    'samp': 0,
    'script': 84,
    'section': 0,
    'small': 0,
    'span': 0,
    'strike': 0,
    'strong': 0,
    'style': 148,
    'sub': 0,
    'summary': 0,
    'sup': 0,
    'table': 272,
    'tbody': 273,
    'td': 273,
    'tfoot': 1,
    'th': 273,
    'thead': 273,
    'time': 0,
    'title': 280,
    'tr': 273,
    'track': 2,
    'tt': 0,
    'u': 0,
    'ul': 0,
    'var': 0,
    'wbr': 2
};
html4['ELEMENTS'] = html4.ELEMENTS;
html4.ELEMENT_DOM_INTERFACES = {
    'a': 'HTMLAnchorElement',
    'abbr': 'HTMLElement',
    'acronym': 'HTMLElement',
    'address': 'HTMLElement',
    'applet': 'HTMLAppletElement',
    'area': 'HTMLAreaElement',
    'article': 'HTMLElement',
    'aside': 'HTMLElement',
    'audio': 'HTMLAudioElement',
    'b': 'HTMLElement',
    'base': 'HTMLBaseElement',
    'basefont': 'HTMLBaseFontElement',
    'bdi': 'HTMLElement',
    'bdo': 'HTMLElement',
    'big': 'HTMLElement',
    'blockquote': 'HTMLQuoteElement',
    'body': 'HTMLBodyElement',
    'br': 'HTMLBRElement',
    'caption': 'HTMLTableCaptionElement',
    'cite': 'HTMLElement',
    'code': 'HTMLElement',
    'col': 'HTMLTableColElement',
    'colgroup': 'HTMLTableColElement',
    'command': 'HTMLCommandElement',
    'data': 'HTMLElement',
    'datalist': 'HTMLDataListElement',
    'dd': 'HTMLElement',
    'del': 'HTMLModElement',
    'details': 'HTMLDetailsElement',
    'dfn': 'HTMLElement',
    'dialog': 'HTMLDialogElement',
    'dir': 'HTMLDirectoryElement',
    'div': 'HTMLDivElement',
    'dl': 'HTMLDListElement',
    'dt': 'HTMLElement',
    'em': 'HTMLElement',
    'fieldset': 'HTMLFieldSetElement',
    'figcaption': 'HTMLElement',
    'figure': 'HTMLElement',
    'footer': 'HTMLElement',
    'form': 'HTMLFormElement',
    'frame': 'HTMLFrameElement',
    'frameset': 'HTMLFrameSetElement',
    'h1': 'HTMLHeadingElement',
    'h2': 'HTMLHeadingElement',
    'h3': 'HTMLHeadingElement',
    'h4': 'HTMLHeadingElement',
    'h5': 'HTMLHeadingElement',
    'h6': 'HTMLHeadingElement',
    'head': 'HTMLHeadElement',
    'header': 'HTMLElement',
    'hgroup': 'HTMLElement',
    'hr': 'HTMLHRElement',
    'html': 'HTMLHtmlElement',
    'i': 'HTMLElement',
    'iframe': 'HTMLIFrameElement',
    'img': 'HTMLImageElement',
    'input': 'HTMLInputElement',
    'ins': 'HTMLModElement',
    'isindex': 'HTMLUnknownElement',
    'kbd': 'HTMLElement',
    'keygen': 'HTMLKeygenElement',
    'label': 'HTMLLabelElement',
    'legend': 'HTMLLegendElement',
    'li': 'HTMLLIElement',
    'link': 'HTMLLinkElement',
    'map': 'HTMLMapElement',
    'menu': 'HTMLMenuElement',
    'meta': 'HTMLMetaElement',
    'meter': 'HTMLMeterElement',
    'nav': 'HTMLElement',
    'nobr': 'HTMLElement',
    'noembed': 'HTMLElement',
    'noframes': 'HTMLElement',
    'noscript': 'HTMLElement',
    'object': 'HTMLObjectElement',
    'ol': 'HTMLOListElement',
    'optgroup': 'HTMLOptGroupElement',
    'option': 'HTMLOptionElement',
    'output': 'HTMLOutputElement',
    'p': 'HTMLParagraphElement',
    'param': 'HTMLParamElement',
    'pre': 'HTMLPreElement',
    'q': 'HTMLQuoteElement',
    's': 'HTMLElement',
    'samp': 'HTMLElement',
    'script': 'HTMLScriptElement',
    'section': 'HTMLElement',
    'select': 'HTMLSelectElement',
    'small': 'HTMLElement',
    'source': 'HTMLSourceElement',
    'span': 'HTMLSpanElement',
    'strike': 'HTMLElement',
    'strong': 'HTMLElement',
    'style': 'HTMLStyleElement',
    'sub': 'HTMLElement',
    'summary': 'HTMLElement',
    'sup': 'HTMLElement',
    'table': 'HTMLTableElement',
    'tbody': 'HTMLTableSectionElement',
    'td': 'HTMLTableDataCellElement',
    'tfoot': 'HTMLTableSectionElement',
    'th': 'HTMLTableHeaderCellElement',
    'thead': 'HTMLTableSectionElement',
    'time': 'HTMLTimeElement',
    'title': 'HTMLTitleElement',
    'tr': 'HTMLTableRowElement',
    'track': 'HTMLTrackElement',
    'tt': 'HTMLElement',
    'u': 'HTMLElement',
    'ul': 'HTMLUListElement',
    'var': 'HTMLElement',
    'video': 'HTMLVideoElement',
    'wbr': 'HTMLElement'
};
html4['ELEMENT_DOM_INTERFACES'] = html4.ELEMENT_DOM_INTERFACES;
html4.ueffects = {
    'NOT_LOADED': 0,
    'SAME_DOCUMENT': 1,
    'NEW_DOCUMENT': 2
};
html4['ueffects'] = html4.ueffects;
html4.URIEFFECTS = {
    'a::href': 2,
    'area::href': 2,
    'audio::src': 1,
    'blockquote::cite': 0,
    'command::icon': 1,
    'del::cite': 0,
    'form::action': 2,
    'iframe::src': 1,
    'img::src': 1,
    'input::src': 1,
    'ins::cite': 0,
    'q::cite': 0,
    'video::poster': 1,
    'video::src': 1
};
html4['URIEFFECTS'] = html4.URIEFFECTS;
html4.ltypes = {
    'UNSANDBOXED': 2,
    'SANDBOXED': 1,
    'DATA': 0
};
html4['ltypes'] = html4.ltypes;
html4.LOADERTYPES = {
    'a::href': 2,
    'area::href': 2,
    'audio::src': 2,
    'blockquote::cite': 2,
    'command::icon': 1,
    'del::cite': 2,
    'form::action': 2,
    'iframe::src': 2,
    'img::src': 1,
    'input::src': 1,
    'ins::cite': 2,
    'q::cite': 2,
    'video::poster': 1,
    'video::src': 2
};
html4['LOADERTYPES'] = html4.LOADERTYPES;
// NOTE: currently focused only on URI-type attributes
html4.REQUIREDATTRIBUTES = {
    "audio": ["src"],
    "form": ["action"],
    "iframe": ["src"],
    "image": ["src"],
    "video": ["src"]
};
html4['REQUIREDATTRIBUTES'] = html4.REQUIREDATTRIBUTES;
// export for Closure Compiler
if (typeof window !== 'undefined') {
    window['html4'] = html4;
}
;
// Copyright (C) 2006 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

/**
 * @fileoverview
 * An HTML sanitizer that can satisfy a variety of security policies.
 *
 * <p>
 * The HTML sanitizer is built around a SAX parser and HTML element and
 * attributes schemas.
 *
 * If the cssparser is loaded, inline styles are sanitized using the
 * css property and value schemas.  Else they are remove during
 * sanitization.
 *
 * If it exists, uses parseCssDeclarations, sanitizeCssProperty,  cssSchema
 *
 * @author mikesamuel@gmail.com
 * @author jasvir@gmail.com
 * \@requires html4, URI
 * \@overrides window
 * \@provides html, html_sanitize
 */

// The Turkish i seems to be a non-issue, but abort in case it is.
if ('I'.toLowerCase() !== 'i') { throw 'I/i problem'; }

/**
 * \@namespace
 */
var html = (function (html4) {

    // For closure compiler
    var parseCssDeclarations, sanitizeCssProperty, cssSchema;
    if ('undefined' !== typeof window) {
        parseCssDeclarations = window['parseCssDeclarations'];
        sanitizeCssProperty = window['sanitizeCssProperty'];
        cssSchema = window['cssSchema'];
    }

    // The keys of this object must be 'quoted' or JSCompiler will mangle them!
    // This is a partial list -- lookupEntity() uses the host browser's parser
    // (when available) to implement full entity lookup.
    // Note that entities are in general case-sensitive; the uppercase ones are
    // explicitly defined by HTML5 (presumably as compatibility).
    var ENTITIES = {
        'lt': '<',
        'LT': '<',
        'gt': '>',
        'GT': '>',
        'amp': '&',
        'AMP': '&',
        'quot': '"',
        'apos': '\'',
        'nbsp': '\240'
    };

    // Patterns for types of entity/character reference names.
    var decimalEscapeRe = /^#(\d+)$/;
    var hexEscapeRe = /^#x([0-9A-Fa-f]+)$/;
    // contains every entity per http://www.w3.org/TR/2011/WD-html5-20110113/named-character-references.html
    var safeEntityNameRe = /^[A-Za-z][A-za-z0-9]+$/;
    // Used as a hook to invoke the browser's entity parsing. <textarea> is used
    // because its content is parsed for entities but not tags.
    // TODO(kpreid): This retrieval is a kludge and leads to silent loss of
    // functionality if the document isn't available.
    var entityLookupElement =
        ('undefined' !== typeof window && window['document'])
            ? window['document'].createElement('textarea') : null;
    /**
     * Decodes an HTML entity.
     *
     * {\@updoc
     * $ lookupEntity('lt')
     * # '<'
     * $ lookupEntity('GT')
     * # '>'
     * $ lookupEntity('amp')
     * # '&'
     * $ lookupEntity('nbsp')
     * # '\xA0'
     * $ lookupEntity('apos')
     * # "'"
     * $ lookupEntity('quot')
     * # '"'
     * $ lookupEntity('#xa')
     * # '\n'
     * $ lookupEntity('#10')
     * # '\n'
     * $ lookupEntity('#x0a')
     * # '\n'
     * $ lookupEntity('#010')
     * # '\n'
     * $ lookupEntity('#x00A')
     * # '\n'
     * $ lookupEntity('Pi')      // Known failure
     * # '\u03A0'
     * $ lookupEntity('pi')      // Known failure
     * # '\u03C0'
     * }
     *
     * @param {string} name the content between the '&' and the ';'.
     * @return {string} a single unicode code-point as a string.
     */
    function lookupEntity(name) {
        // TODO: entity lookup as specified by HTML5 actually depends on the
        // presence of the ";".
        if (ENTITIES.hasOwnProperty(name)) { return ENTITIES[name]; }
        var m = name.match(decimalEscapeRe);
        if (m) {
            return String.fromCharCode(parseInt(m[1], 10));
        } else if (!!(m = name.match(hexEscapeRe))) {
            return String.fromCharCode(parseInt(m[1], 16));
        } else if (entityLookupElement && safeEntityNameRe.test(name)) {
            entityLookupElement.innerHTML = '&' + name + ';';
            var text = entityLookupElement.textContent;
            ENTITIES[name] = text;
            return text;
        } else {
            return '&' + name + ';';
        }
    }

    function decodeOneEntity(_, name) {
        return lookupEntity(name);
    }

    var nulRe = /\0/g;
    function stripNULs(s) {
        return s.replace(nulRe, '');
    }

    var ENTITY_RE_1 = /&(#[0-9]+|#[xX][0-9A-Fa-f]+|\w+);/g;
    var ENTITY_RE_2 = /^(#[0-9]+|#[xX][0-9A-Fa-f]+|\w+);/;
    /**
     * The plain text of a chunk of HTML CDATA which possibly containing.
     *
     * {\@updoc
     * $ unescapeEntities('')
     * # ''
     * $ unescapeEntities('hello World!')
     * # 'hello World!'
     * $ unescapeEntities('1 &lt; 2 &amp;&AMP; 4 &gt; 3&#10;')
     * # '1 < 2 && 4 > 3\n'
     * $ unescapeEntities('&lt;&lt <- unfinished entity&gt;')
     * # '<&lt <- unfinished entity>'
     * $ unescapeEntities('/foo?bar=baz&copy=true')  // & often unescaped in URLS
     * # '/foo?bar=baz&copy=true'
     * $ unescapeEntities('pi=&pi;&#x3c0;, Pi=&Pi;\u03A0') // FIXME: known failure
     * # 'pi=\u03C0\u03c0, Pi=\u03A0\u03A0'
     * }
     *
     * @param {string} s a chunk of HTML CDATA.  It must not start or end inside
     *     an HTML entity.
     */
    function unescapeEntities(s) {
        return s.replace(ENTITY_RE_1, decodeOneEntity);
    }

    var ampRe = /&/g;
    var looseAmpRe = /&([^a-z#]|#(?:[^0-9x]|x(?:[^0-9a-f]|$)|$)|$)/gi;
    var ltRe = /[<]/g;
    var gtRe = />/g;
    var quotRe = /\"/g;

    /**
     * Escapes HTML special characters in attribute values.
     *
     * {\@updoc
     * $ escapeAttrib('')
     * # ''
     * $ escapeAttrib('"<<&==&>>"')  // Do not just escape the first occurrence.
     * # '&#34;&lt;&lt;&amp;&#61;&#61;&amp;&gt;&gt;&#34;'
     * $ escapeAttrib('Hello <World>!')
     * # 'Hello &lt;World&gt;!'
     * }
     */
    function escapeAttrib(s) {
        return ('' + s).replace(ampRe, '&amp;').replace(ltRe, '&lt;')
            .replace(gtRe, '&gt;').replace(quotRe, '&#34;');
    }

    /**
     * Escape entities in RCDATA that can be escaped without changing the meaning.
     * {\@updoc
     * $ normalizeRCData('1 < 2 &&amp; 3 > 4 &amp;& 5 &lt; 7&8')
     * # '1 &lt; 2 &amp;&amp; 3 &gt; 4 &amp;&amp; 5 &lt; 7&amp;8'
     * }
     */
    function normalizeRCData(rcdata) {
        return rcdata
            .replace(looseAmpRe, '&amp;$1')
            .replace(ltRe, '&lt;')
            .replace(gtRe, '&gt;');
    }

    // TODO(felix8a): validate sanitizer regexs against the HTML5 grammar at
    // http://www.whatwg.org/specs/web-apps/current-work/multipage/syntax.html
    // http://www.whatwg.org/specs/web-apps/current-work/multipage/parsing.html
    // http://www.whatwg.org/specs/web-apps/current-work/multipage/tokenization.html
    // http://www.whatwg.org/specs/web-apps/current-work/multipage/tree-construction.html

    // We initially split input so that potentially meaningful characters
    // like '<' and '>' are separate tokens, using a fast dumb process that
    // ignores quoting.  Then we walk that token stream, and when we see a
    // '<' that's the start of a tag, we use ATTR_RE to extract tag
    // attributes from the next token.  That token will never have a '>'
    // character.  However, it might have an unbalanced quote character, and
    // when we see that, we combine additional tokens to balance the quote.

    var ATTR_RE = new RegExp(
      '^\\s*' +
      '([-.:\\w]+)' +             // 1 = Attribute name
      '(?:' + (
        '\\s*(=)\\s*' +           // 2 = Is there a value?
        '(' + (                   // 3 = Attribute value
          // TODO(felix8a): maybe use backref to match quotes
          '(\")[^\"]*(\"|$)' +    // 4, 5 = Double-quoted string
          '|' +
          '(\')[^\']*(\'|$)' +    // 6, 7 = Single-quoted string
          '|' +
          // Positive lookahead to prevent interpretation of
          // <foo a= b=c> as <foo a='b=c'>
          // TODO(felix8a): might be able to drop this case
          '(?=[a-z][-\\w]*\\s*=)' +
          '|' +
          // Unquoted value that isn't an attribute name
          // (since we didn't match the positive lookahead above)
          '[^\"\'\\s]*') +
        ')') +
      ')?',
      'i');

    // false on IE<=8, true on most other browsers
    var splitWillCapture = ('a,b'.split(/(,)/).length === 3);

    // bitmask for tags with special parsing, like <script> and <textarea>
    var EFLAGS_TEXT = html4.eflags['CDATA'] | html4.eflags['RCDATA'];

    /**
     * Given a SAX-like event handler, produce a function that feeds those
     * events and a parameter to the event handler.
     *
     * The event handler has the form:{@code
     * {
     *   // Name is an upper-case HTML tag name.  Attribs is an array of
     *   // alternating upper-case attribute names, and attribute values.  The
     *   // attribs array is reused by the parser.  Param is the value passed to
     *   // the saxParser.
     *   startTag: function (name, attribs, param) { ... },
     *   endTag:   function (name, param) { ... },
     *   pcdata:   function (text, param) { ... },
     *   rcdata:   function (text, param) { ... },
     *   cdata:    function (text, param) { ... },
     *   startDoc: function (param) { ... },
     *   endDoc:   function (param) { ... }
     * }}
     *
     * @param {Object} handler a record containing event handlers.
     * @return {function(string, Object)} A function that takes a chunk of HTML
     *     and a parameter.  The parameter is passed on to the handler methods.
     */
    function makeSaxParser(handler) {
        // Accept quoted or unquoted keys (Closure compat)
        var hcopy = {
            cdata: handler.cdata || handler['cdata'],
            comment: handler.comment || handler['comment'],
            endDoc: handler.endDoc || handler['endDoc'],
            endTag: handler.endTag || handler['endTag'],
            pcdata: handler.pcdata || handler['pcdata'],
            rcdata: handler.rcdata || handler['rcdata'],
            startDoc: handler.startDoc || handler['startDoc'],
            startTag: handler.startTag || handler['startTag']
        };
        return function (htmlText, param) {
            return parse(htmlText, hcopy, param);
        };
    }

    // Parsing strategy is to split input into parts that might be lexically
    // meaningful (every ">" becomes a separate part), and then recombine
    // parts if we discover they're in a different context.

    // TODO(felix8a): Significant performance regressions from -legacy,
    // tested on
    //    Chrome 18.0
    //    Firefox 11.0
    //    IE 6, 7, 8, 9
    //    Opera 11.61
    //    Safari 5.1.3
    // Many of these are unusual patterns that are linearly slower and still
    // pretty fast (eg 1ms to 5ms), so not necessarily worth fixing.

    // TODO(felix8a): "<script> && && && ... <\/script>" is slower on all
    // browsers.  The hotspot is htmlSplit.

    // TODO(felix8a): "<p title='>>>>...'><\/p>" is slower on all browsers.
    // This is partly htmlSplit, but the hotspot is parseTagAndAttrs.

    // TODO(felix8a): "<a><\/a><a><\/a>..." is slower on IE9.
    // "<a>1<\/a><a>1<\/a>..." is faster, "<a><\/a>2<a><\/a>2..." is faster.

    // TODO(felix8a): "<p<p<p..." is slower on IE[6-8]

    var continuationMarker = {};
    function parse(htmlText, handler, param) {
        var m, p, tagName;
        var parts = htmlSplit(htmlText);
        var state = {
            noMoreGT: false,
            noMoreEndComments: false
        };
        parseCPS(handler, parts, 0, state, param);
    }

    function continuationMaker(h, parts, initial, state, param) {
        return function () {
            parseCPS(h, parts, initial, state, param);
        };
    }

    function parseCPS(h, parts, initial, state, param) {
        try {
            if (h.startDoc && initial == 0) { h.startDoc(param); }
            var m, p, tagName;
            for (var pos = initial, end = parts.length; pos < end;) {
                var current = parts[pos++];
                var next = parts[pos];
                switch (current) {
                    case '&':
                        if (ENTITY_RE_2.test(next)) {
                            if (h.pcdata) {
                                h.pcdata('&' + next, param, continuationMarker,
                                  continuationMaker(h, parts, pos, state, param));
                            }
                            pos++;
                        } else {
                            if (h.pcdata) {
                                h.pcdata("&amp;", param, continuationMarker,
                                    continuationMaker(h, parts, pos, state, param));
                            }
                        }
                        break;
                    case '<\/':
                        if ((m = /^([-\w:]+)[^\'\"]*/.exec(next))) {
                            if (m[0].length === next.length && parts[pos + 1] === '>') {
                                // fast case, no attribute parsing needed
                                pos += 2;
                                tagName = m[1].toLowerCase();
                                if (h.endTag) {
                                    h.endTag(tagName, param, continuationMarker,
                                      continuationMaker(h, parts, pos, state, param));
                                }
                            } else {
                                // slow case, need to parse attributes
                                // TODO(felix8a): do we really care about misparsing this?
                                pos = parseEndTag(
                                  parts, pos, h, param, continuationMarker, state);
                            }
                        } else {
                            if (h.pcdata) {
                                h.pcdata('&lt;/', param, continuationMarker,
                                  continuationMaker(h, parts, pos, state, param));
                            }
                        }
                        break;
                    case '<':
                        if (m = /^([-\w:]+)\s*\/?/.exec(next)) {
                            if (m[0].length === next.length && parts[pos + 1] === '>') {
                                // fast case, no attribute parsing needed
                                pos += 2;
                                tagName = m[1].toLowerCase();
                                if (h.startTag) {
                                    h.startTag(tagName, [], param, continuationMarker,
                                      continuationMaker(h, parts, pos, state, param));
                                }
                                // tags like <script> and <textarea> have special parsing
                                var eflags = html4.ELEMENTS[tagName];
                                if (eflags & EFLAGS_TEXT) {
                                    var tag = { name: tagName, next: pos, eflags: eflags };
                                    pos = parseText(
                                      parts, tag, h, param, continuationMarker, state);
                                }
                            } else {
                                // slow case, need to parse attributes
                                pos = parseStartTag(
                                  parts, pos, h, param, continuationMarker, state);
                            }
                        } else {
                            if (h.pcdata) {
                                h.pcdata('&lt;', param, continuationMarker,
                                  continuationMaker(h, parts, pos, state, param));
                            }
                        }
                        break;
                    case '<\!--':
                        // The pathological case is n copies of '<\!--' without '-->', and
                        // repeated failure to find '-->' is quadratic.  We avoid that by
                        // remembering when search for '-->' fails.
                        if (!state.noMoreEndComments) {
                            // A comment <\!--x--> is split into three tokens:
                            //   '<\!--', 'x--', '>'
                            // We want to find the next '>' token that has a preceding '--'.
                            // pos is at the 'x--'.
                            for (p = pos + 1; p < end; p++) {
                                if (parts[p] === '>' && /--$/.test(parts[p - 1])) { break; }
                            }
                            if (p < end) {
                                if (h.comment) {
                                    var comment = parts.slice(pos, p).join('');
                                    h.comment(
                                      comment.substr(0, comment.length - 2), param,
                                      continuationMarker,
                                      continuationMaker(h, parts, p + 1, state, param));
                                }
                                pos = p + 1;
                            } else {
                                state.noMoreEndComments = true;
                            }
                        }
                        if (state.noMoreEndComments) {
                            if (h.pcdata) {
                                h.pcdata('&lt;!--', param, continuationMarker,
                                  continuationMaker(h, parts, pos, state, param));
                            }
                        }
                        break;
                    case '<\!':
                        if (!/^\w/.test(next)) {
                            if (h.pcdata) {
                                h.pcdata('&lt;!', param, continuationMarker,
                                  continuationMaker(h, parts, pos, state, param));
                            }
                        } else {
                            // similar to noMoreEndComment logic
                            if (!state.noMoreGT) {
                                for (p = pos + 1; p < end; p++) {
                                    if (parts[p] === '>') { break; }
                                }
                                if (p < end) {
                                    pos = p + 1;
                                } else {
                                    state.noMoreGT = true;
                                }
                            }
                            if (state.noMoreGT) {
                                if (h.pcdata) {
                                    h.pcdata('&lt;!', param, continuationMarker,
                                      continuationMaker(h, parts, pos, state, param));
                                }
                            }
                        }
                        break;
                    case '<?':
                        // similar to noMoreEndComment logic
                        if (!state.noMoreGT) {
                            for (p = pos + 1; p < end; p++) {
                                if (parts[p] === '>') { break; }
                            }
                            if (p < end) {
                                pos = p + 1;
                            } else {
                                state.noMoreGT = true;
                            }
                        }
                        if (state.noMoreGT) {
                            if (h.pcdata) {
                                h.pcdata('&lt;?', param, continuationMarker,
                                  continuationMaker(h, parts, pos, state, param));
                            }
                        }
                        break;
                    case '>':
                        if (h.pcdata) {
                            h.pcdata("&gt;", param, continuationMarker,
                              continuationMaker(h, parts, pos, state, param));
                        }
                        break;
                    case '':
                        break;
                    default:
                        if (h.pcdata) {
                            h.pcdata(current, param, continuationMarker,
                              continuationMaker(h, parts, pos, state, param));
                        }
                        break;
                }
            }
            if (h.endDoc) { h.endDoc(param); }
        } catch (e) {
            if (e !== continuationMarker) { throw e; }
        }
    }

    // Split str into parts for the html parser.
    function htmlSplit(str) {
        // can't hoist this out of the function because of the re.exec loop.
        var re = /(<\/|<\!--|<[!?]|[&<>])/g;
        str += '';
        if (splitWillCapture) {
            return str.split(re);
        } else {
            var parts = [];
            var lastPos = 0;
            var m;
            while ((m = re.exec(str)) !== null) {
                parts.push(str.substring(lastPos, m.index));
                parts.push(m[0]);
                lastPos = m.index + m[0].length;
            }
            parts.push(str.substring(lastPos));
            return parts;
        }
    }

    function parseEndTag(parts, pos, h, param, continuationMarker, state) {
        var tag = parseTagAndAttrs(parts, pos);
        // drop unclosed tags
        if (!tag) { return parts.length; }
        if (h.endTag) {
            h.endTag(tag.name, param, continuationMarker,
              continuationMaker(h, parts, pos, state, param));
        }
        return tag.next;
    }

    function parseStartTag(parts, pos, h, param, continuationMarker, state) {
        var tag = parseTagAndAttrs(parts, pos);
        // drop unclosed tags
        if (!tag) { return parts.length; }
        if (h.startTag) {
            h.startTag(tag.name, tag.attrs, param, continuationMarker,
              continuationMaker(h, parts, tag.next, state, param));
        }
        // tags like <script> and <textarea> have special parsing
        if (tag.eflags & EFLAGS_TEXT) {
            return parseText(parts, tag, h, param, continuationMarker, state);
        } else {
            return tag.next;
        }
    }

    var endTagRe = {};

    // Tags like <script> and <textarea> are flagged as CDATA or RCDATA,
    // which means everything is text until we see the correct closing tag.
    function parseText(parts, tag, h, param, continuationMarker, state) {
        var end = parts.length;
        if (!endTagRe.hasOwnProperty(tag.name)) {
            endTagRe[tag.name] = new RegExp('^' + tag.name + '(?:[\\s\\/]|$)', 'i');
        }
        var re = endTagRe[tag.name];
        var first = tag.next;
        var p = tag.next + 1;
        for (; p < end; p++) {
            if (parts[p - 1] === '<\/' && re.test(parts[p])) { break; }
        }
        if (p < end) { p -= 1; }
        var buf = parts.slice(first, p).join('');
        if (tag.eflags & html4.eflags['CDATA']) {
            if (h.cdata) {
                h.cdata(buf, param, continuationMarker,
                  continuationMaker(h, parts, p, state, param));
            }
        } else if (tag.eflags & html4.eflags['RCDATA']) {
            if (h.rcdata) {
                h.rcdata(normalizeRCData(buf), param, continuationMarker,
                  continuationMaker(h, parts, p, state, param));
            }
        } else {
            throw new Error('bug');
        }
        return p;
    }

    // at this point, parts[pos-1] is either "<" or "<\/".
    function parseTagAndAttrs(parts, pos) {
        var m = /^([-\w:]+)/.exec(parts[pos]);
        var tag = {};
        tag.name = m[1].toLowerCase();
        tag.eflags = html4.ELEMENTS[tag.name];
        var buf = parts[pos].substr(m[0].length);
        // Find the next '>'.  We optimistically assume this '>' is not in a
        // quoted context, and further down we fix things up if it turns out to
        // be quoted.
        var p = pos + 1;
        var end = parts.length;
        for (; p < end; p++) {
            if (parts[p] === '>') { break; }
            buf += parts[p];
        }
        if (end <= p) { return void 0; }
        var attrs = [];
        while (buf !== '') {
            m = ATTR_RE.exec(buf);
            if (!m) {
                // No attribute found: skip garbage
                buf = buf.replace(/^[\s\S][^a-z\s]*/, '');

            } else if ((m[4] && !m[5]) || (m[6] && !m[7])) {
                // Unterminated quote: slurp to the next unquoted '>'
                var quote = m[4] || m[6];
                var sawQuote = false;
                var abuf = [buf, parts[p++]];
                for (; p < end; p++) {
                    if (sawQuote) {
                        if (parts[p] === '>') { break; }
                    } else if (0 <= parts[p].indexOf(quote)) {
                        sawQuote = true;
                    }
                    abuf.push(parts[p]);
                }
                // Slurp failed: lose the garbage
                if (end <= p) { break; }
                // Otherwise retry attribute parsing
                buf = abuf.join('');
                continue;

            } else {
                // We have an attribute
                var aName = m[1].toLowerCase();
                var aValue = m[2] ? decodeValue(m[3]) : '';
                attrs.push(aName, aValue);
                buf = buf.substr(m[0].length);
            }
        }
        tag.attrs = attrs;
        tag.next = p + 1;
        return tag;
    }

    function decodeValue(v) {
        var q = v.charCodeAt(0);
        if (q === 0x22 || q === 0x27) { // " or '
            v = v.substr(1, v.length - 2);
        }
        return unescapeEntities(stripNULs(v));
    }

    /**
     * Returns a function that strips unsafe tags and attributes from html.
     * @param {function(string, Array.<string>): ?Array.<string>} tagPolicy
     *     A function that takes (tagName, attribs[]), where tagName is a key in
     *     html4.ELEMENTS and attribs is an array of alternating attribute names
     *     and values.  It should return a record (as follows), or null to delete
     *     the element.  It's okay for tagPolicy to modify the attribs array,
     *     but the same array is reused, so it should not be held between calls.
     *     Record keys:
     *        attribs: (required) Sanitized attributes array.
     *        tagName: Replacement tag name.
     * @return {function(string, Array)} A function that sanitizes a string of
     *     HTML and appends result strings to the second argument, an array.
     */
    function makeHtmlSanitizer(tagPolicy) {
        var stack;
        var ignoring;
        var emit = function (text, out) {
            if (!ignoring) { out.push(text); }
        };
        return makeSaxParser({
            'startDoc': function (_) {
                stack = [];
                ignoring = false;
            },
            'startTag': function (tagNameOrig, attribs, out) {
                if (ignoring) { return; }
                if (!html4.ELEMENTS.hasOwnProperty(tagNameOrig)) { return; }
                var eflagsOrig = html4.ELEMENTS[tagNameOrig];
                if (eflagsOrig & html4.eflags['FOLDABLE']) {
                    return;
                }

                var decision = tagPolicy(tagNameOrig, attribs);
                if (!decision) {
                    ignoring = !(eflagsOrig & html4.eflags['EMPTY']);
                    return;
                } else if (typeof decision !== 'object') {
                    throw new Error('tagPolicy did not return object (old API?)');
                }
                if ('attribs' in decision) {
                    attribs = decision['attribs'];
                } else {
                    throw new Error('tagPolicy gave no attribs');
                }
                var eflagsRep;
                var tagNameRep;
                if ('tagName' in decision) {
                    tagNameRep = decision['tagName'];
                    eflagsRep = html4.ELEMENTS[tagNameRep];
                } else {
                    tagNameRep = tagNameOrig;
                    eflagsRep = eflagsOrig;
                }
                // TODO(mikesamuel): relying on tagPolicy not to insert unsafe
                // attribute names.

                // If this is an optional-end-tag element and either this element or its
                // previous like sibling was rewritten, then insert a close tag to
                // preserve structure.
                if (eflagsOrig & html4.eflags['OPTIONAL_ENDTAG']) {
                    var onStack = stack[stack.length - 1];
                    if (onStack && onStack.orig === tagNameOrig &&
                        (onStack.rep !== tagNameRep || tagNameOrig !== tagNameRep)) {
                        out.push('<\/', onStack.rep, '>');
                    }
                }

                if (!(eflagsOrig & html4.eflags['EMPTY'])) {
                    stack.push({ orig: tagNameOrig, rep: tagNameRep });
                }

                out.push('<', tagNameRep);
                for (var i = 0, n = attribs.length; i < n; i += 2) {
                    var attribName = attribs[i],
                        value = attribs[i + 1];
                    if (value !== null && value !== void 0) {
                        out.push(' ', attribName, '="', escapeAttrib(value), '"');
                    }
                }
                out.push('>');

                if ((eflagsOrig & html4.eflags['EMPTY'])
                    && !(eflagsRep & html4.eflags['EMPTY'])) {
                    // replacement is non-empty, synthesize end tag
                    out.push('<\/', tagNameRep, '>');
                }
            },
            'endTag': function (tagName, out) {
                if (ignoring) {
                    ignoring = false;
                    return;
                }
                if (!html4.ELEMENTS.hasOwnProperty(tagName)) { return; }
                var eflags = html4.ELEMENTS[tagName];
                if (!(eflags & (html4.eflags['EMPTY'] | html4.eflags['FOLDABLE']))) {
                    var index;
                    if (eflags & html4.eflags['OPTIONAL_ENDTAG']) {
                        for (index = stack.length; --index >= 0;) {
                            var stackElOrigTag = stack[index].orig;
                            if (stackElOrigTag === tagName) { break; }
                            if (!(html4.ELEMENTS[stackElOrigTag] &
                                  html4.eflags['OPTIONAL_ENDTAG'])) {
                                // Don't pop non optional end tags looking for a match.
                                return;
                            }
                        }
                    } else {
                        for (index = stack.length; --index >= 0;) {
                            if (stack[index].orig === tagName) { break; }
                        }
                    }
                    if (index < 0) { return; }  // Not opened.
                    for (var i = stack.length; --i > index;) {
                        var stackElRepTag = stack[i].rep;
                        if (!(html4.ELEMENTS[stackElRepTag] &
                              html4.eflags['OPTIONAL_ENDTAG'])) {
                            out.push('<\/', stackElRepTag, '>');
                        }
                    }
                    if (index < stack.length) {
                        tagName = stack[index].rep;
                    }
                    stack.length = index;
                    out.push('<\/', tagName, '>');
                }
            },
            'pcdata': emit,
            'rcdata': emit,
            'cdata': emit,
            'endDoc': function (out) {
                for (; stack.length; stack.length--) {
                    out.push('<\/', stack[stack.length - 1].rep, '>');
                }
            }
        });
    }

    var ALLOWED_URI_SCHEMES = /^(?:https?|mailto)$/i;

    function safeUri(uri, effect, ltype, hints, naiveUriRewriter) {
        if (!naiveUriRewriter) { return null; }
        try {
            var parsed = URI.parse('' + uri);
            if (parsed) {
                if (!parsed.hasScheme() ||
                    ALLOWED_URI_SCHEMES.test(parsed.getScheme())) {
                    var safe = naiveUriRewriter(parsed, effect, ltype, hints);
                    return safe ? safe.toString() : null;
                }
            }
        } catch (e) {
            return null;
        }
        return null;
    }

    function log(logger, tagName, attribName, oldValue, newValue) {
        if (!attribName) {
            logger(tagName + " removed", {
                change: "removed",
                tagName: tagName
            });
        }
        if (oldValue !== newValue) {
            var changed = "changed";
            if (oldValue && !newValue) {
                changed = "removed";
            } else if (!oldValue && newValue) {
                changed = "added";
            }
            logger(tagName + "." + attribName + " " + changed, {
                change: changed,
                tagName: tagName,
                attribName: attribName,
                oldValue: oldValue,
                newValue: newValue
            });
        }
    }

    function lookupAttribute(map, tagName, attribName) {
        var attribKey;
        attribKey = tagName + '::' + attribName;
        if (map.hasOwnProperty(attribKey)) {
            return map[attribKey];
        }
        attribKey = '*::' + attribName;
        if (map.hasOwnProperty(attribKey)) {
            return map[attribKey];
        }
        return void 0;
    }
    function getAttributeType(tagName, attribName) {
        return lookupAttribute(html4.ATTRIBS, tagName, attribName);
    }
    function getLoaderType(tagName, attribName) {
        return lookupAttribute(html4.LOADERTYPES, tagName, attribName);
    }
    function getUriEffect(tagName, attribName) {
        return lookupAttribute(html4.URIEFFECTS, tagName, attribName);
    }

    /**
     * Sanitizes attributes on an HTML tag.
     * @param {string} tagName An HTML tag name in lowercase.
     * @param {Array.<?string>} attribs An array of alternating names and values.
     * @param {?function(?string): ?string} opt_naiveUriRewriter A transform to
     *     apply to URI attributes; it can return a new string value, or null to
     *     delete the attribute.  If unspecified, URI attributes are deleted.
     * @param {function(?string): ?string} opt_nmTokenPolicy A transform to apply
     *     to attributes containing HTML names, element IDs, and space-separated
     *     lists of classes; it can return a new string value, or null to delete
     *     the attribute.  If unspecified, these attributes are kept unchanged.
     * @return {Array.<?string>} The sanitized attributes as a list of alternating
     *     names and values, where a null value means to omit the attribute.
     */
    function sanitizeAttribs(tagName, attribs, opt_naiveUriRewriter, opt_nmTokenPolicy, opt_logger) {
        // TODO(felix8a): it's obnoxious that domado duplicates much of this
        // TODO(felix8a): maybe consistently enforce constraints like target=
        for (var i = 0; i < attribs.length; i += 2) {
            var attribName = attribs[i];
            var value = attribs[i + 1];
            var oldValue = value;
            var atype = null, attribKey;
            if ((attribKey = tagName + '::' + attribName,
                 html4.ATTRIBS.hasOwnProperty(attribKey)) ||
                (attribKey = '*::' + attribName,
                 html4.ATTRIBS.hasOwnProperty(attribKey))) {
                atype = html4.ATTRIBS[attribKey];
            }

            // Discourse modification: give us more flexibility with whitelists
            if (opt_nmTokenPolicy) {
                var newValue = opt_nmTokenPolicy(tagName, attribName, value);
                if (newValue) {
                    attribs[i + 1] = newValue;
                    continue;
                }
            }

            if (atype !== null) {
                switch (atype) {
                    case html4.atype['NONE']: break;
                    case html4.atype['SCRIPT']:
                        value = null;
                        if (opt_logger) {
                            log(opt_logger, tagName, attribName, oldValue, value);
                        }
                        break;
                    case html4.atype['STYLE']:
                        if ('undefined' === typeof parseCssDeclarations) {
                            value = null;
                            if (opt_logger) {
                                log(opt_logger, tagName, attribName, oldValue, value);
                            }
                            break;
                        }
                        var sanitizedDeclarations = [];
                        parseCssDeclarations(
                            value,
                            {
                                'declaration': function (property, tokens) {
                                    var normProp = property.toLowerCase();
                                    sanitizeCssProperty(
                                        normProp, tokens,
                                        opt_naiveUriRewriter
                                        ? function (url) {
                                            return safeUri(
                                                url, html4.ueffects.SAME_DOCUMENT,
                                                html4.ltypes.SANDBOXED,
                                                {
                                                    "TYPE": "CSS",
                                                    "CSS_PROP": normProp
                                                }, opt_naiveUriRewriter);
                                        }
                                        : null);
                                    if (tokens.length) {
                                        sanitizedDeclarations.push(
                                            normProp + ': ' + tokens.join(' '));
                                    }
                                }
                            });
                        value = sanitizedDeclarations.length > 0 ?
                          sanitizedDeclarations.join(' ; ') : null;
                        if (opt_logger) {
                            log(opt_logger, tagName, attribName, oldValue, value);
                        }
                        break;
                    case html4.atype['URI']:
                        value = safeUri(value,
                          getUriEffect(tagName, attribName),
                          getLoaderType(tagName, attribName),
                          {
                              "TYPE": "MARKUP",
                              "XML_ATTR": attribName,
                              "XML_TAG": tagName
                          }, opt_naiveUriRewriter);
                        if (opt_logger) {
                            log(opt_logger, tagName, attribName, oldValue, value);
                        }
                        break;
                    case html4.atype['URI_FRAGMENT']:
                        if (value && '#' === value.charAt(0)) {
                            value = value.substring(1);  // remove the leading '#'
                            if (value !== null && value !== void 0) {
                                value = '#' + value;  // restore the leading '#'
                            }
                        } else {
                            value = null;
                        }
                        if (opt_logger) {
                            log(opt_logger, tagName, attribName, oldValue, value);
                        }
                        break;
                    default:
                        value = null;
                        if (opt_logger) {
                            log(opt_logger, tagName, attribName, oldValue, value);
                        }
                        break;
                }
            } else {
                value = null;
                if (opt_logger) {
                    log(opt_logger, tagName, attribName, oldValue, value);
                }
            }
            attribs[i + 1] = value;
        }
        return attribs;
    }

    /**
     * Creates a tag policy that omits all tags marked UNSAFE in html4-defs.js
     * and applies the default attribute sanitizer with the supplied policy for
     * URI attributes and NMTOKEN attributes.
     * @param {?function(?string): ?string} opt_naiveUriRewriter A transform to
     *     apply to URI attributes.  If not given, URI attributes are deleted.
     * @param {function(?string): ?string} opt_nmTokenPolicy A transform to apply
     *     to attributes containing HTML names, element IDs, and space-separated
     *     lists of classes.  If not given, such attributes are left unchanged.
     * @return {function(string, Array.<?string>)} A tagPolicy suitable for
     *     passing to html.sanitize.
     */
    function makeTagPolicy(opt_naiveUriRewriter, opt_nmTokenPolicy, opt_logger) {
        return function (tagName, attribs) {
            if (!(html4.ELEMENTS[tagName] & html4.eflags['UNSAFE'])) {
                var sanitizedAttribs = sanitizeAttribs(tagName, attribs, opt_naiveUriRewriter, opt_nmTokenPolicy, opt_logger);
                var requiredAttributes = html4.REQUIREDATTRIBUTES[tagName];
                if (requiredAttributes && missRequiredAttributes(sanitizedAttribs, requiredAttributes)) { return }
                return { 'attribs': sanitizedAttribs };
            } else {
                if (opt_logger) {
                    log(opt_logger, tagName, undefined, undefined, undefined);
                }
            }
        };
    }

    function missRequiredAttributes(sanitizedAttributes, requiredAttributes) {
        var requiredAttributesWithValueCount = 0;
        for (var i = 0, length = sanitizedAttributes.length; i < length; i += 2) {
            var name = sanitizedAttributes[i];
            var value = sanitizedAttributes[i + 1];
            if (requiredAttributes.indexOf(name) > -1 && value && value.length > 0) { requiredAttributesWithValueCount++; }
        }
        return requiredAttributesWithValueCount != requiredAttributes.length;
    }

    /**
     * Sanitizes HTML tags and attributes according to a given policy.
     * @param {string} inputHtml The HTML to sanitize.
     * @param {function(string, Array.<?string>)} tagPolicy A function that
     *     decides which tags to accept and sanitizes their attributes (see
     *     makeHtmlSanitizer above for details).
     * @return {string} The sanitized HTML.
     */
    function sanitizeWithPolicy(inputHtml, tagPolicy) {
        var outputArray = [];
        makeHtmlSanitizer(tagPolicy)(inputHtml, outputArray);
        return outputArray.join('');
    }

    /**
     * Strips unsafe tags and attributes from HTML.
     * @param {string} inputHtml The HTML to sanitize.
     * @param {?function(?string): ?string} opt_naiveUriRewriter A transform to
     *     apply to URI attributes.  If not given, URI attributes are deleted.
     * @param {function(?string): ?string} opt_nmTokenPolicy A transform to apply
     *     to attributes containing HTML names, element IDs, and space-separated
     *     lists of classes.  If not given, such attributes are left unchanged.
     */
    function sanitize(inputHtml, opt_naiveUriRewriter, opt_nmTokenPolicy, opt_logger) {
        var tagPolicy = makeTagPolicy(opt_naiveUriRewriter, opt_nmTokenPolicy, opt_logger);
        return sanitizeWithPolicy(inputHtml, tagPolicy);
    }

    // Export both quoted and unquoted names for Closure linkage.
    var html = {};
    html.escapeAttrib = html['escapeAttrib'] = escapeAttrib;
    html.makeHtmlSanitizer = html['makeHtmlSanitizer'] = makeHtmlSanitizer;
    html.makeSaxParser = html['makeSaxParser'] = makeSaxParser;
    html.makeTagPolicy = html['makeTagPolicy'] = makeTagPolicy;
    html.normalizeRCData = html['normalizeRCData'] = normalizeRCData;
    html.sanitize = html['sanitize'] = sanitize;
    html.sanitizeAttribs = html['sanitizeAttribs'] = sanitizeAttribs;
    html.sanitizeWithPolicy = html['sanitizeWithPolicy'] = sanitizeWithPolicy;
    html.unescapeEntities = html['unescapeEntities'] = unescapeEntities;
    return html;
})(html4);

var html_sanitize = html['sanitize'];

// Exports for Closure compiler.  Note this file is also cajoled
// for domado and run in an environment without 'window'
if (typeof window !== 'undefined') {
    window['html'] = html;
    window['html_sanitize'] = html_sanitize;
}
},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone.subroute\\backbone.subroute.js":[function(require,module,exports){
// backbone-subroute.js v0.4.2
//
// Copyright (C) 2012 Dave Cadwallader, Model N, Inc.  
// Distributed under the MIT License
//
// Documentation and full license available at:
// https://github.com/ModelN/backbone.subroute

(function(factory) {
    if (typeof define === 'function' && define.amd) {
        // Register as an AMD module if available...
        define(['underscore', 'backbone'], factory);
    } else if (typeof exports === 'object') {
        // Next for Node.js, CommonJS, browserify...
        factory(require('underscore'), require('backbone'));
    } else {
        // Browser globals for the unenlightened...
        factory(_, Backbone);
    }
}(function(_, Backbone) {

    Backbone.SubRoute = Backbone.Router.extend({
        constructor: function(prefix, options) {

            // each subroute instance should have its own routes hash
            this.routes = _.clone(this.routes) || {};

            // Prefix is optional, set to empty string if not passed
            this.prefix = prefix = prefix || "";

            // SubRoute instances may be instantiated using a prefix with or without a trailing slash.
            // If the prefix does *not* have a trailing slash, we need to insert a slash as a separator
            // between the prefix and the sub-route path for each route that we register with Backbone.        
            this.separator = (prefix.slice(-1) === "/") ? "" : "/";

            // if you want to match "books" and "books/" without creating separate routes, set this
            // option to "true" and the sub-router will automatically create those routes for you.
            this.createTrailingSlashRoutes = options && options.createTrailingSlashRoutes;

            // Required to have Backbone set up routes
            Backbone.Router.prototype.constructor.call(this, options);

            // grab the full URL
            var hash;
            if (Backbone.history.fragment) {
                hash = Backbone.history.getFragment();
            } else {
                hash = Backbone.history.getHash();
            }

            // Trigger the subroute immediately.  this supports the case where 
            // a user directly navigates to a URL with a subroute on the first page load.
            // Check every element, if one matches, break. Prevent multiple matches
            _.every(this.routes, function(key, route) {
                // Use the Backbone parser to turn route into regex for matching
                if (hash.match(Backbone.Router.prototype._routeToRegExp(route))) {
                    Backbone.history.loadUrl(hash);
                    return false;
                }
                return true;
            }, this);

            if (this.postInitialize) {
                this.postInitialize(options);
            }
        },
        navigate: function(route, options) {
            if (route.substr(0, 1) != '/' &&
                route.indexOf(this.prefix.substr(0, this.prefix.length - 1)) !== 0) {

                route = this.prefix +
                    (route ? this.separator : "") +
                    route;
            }
            Backbone.Router.prototype.navigate.call(this, route, options);
        },
        route: function(route, name, callback) {
            // strip off any leading slashes in the sub-route path, 
            // since we already handle inserting them when needed.
            if (route.substr(0) === "/") {
                route = route.substr(1, route.length);
            }

            var _route = this.prefix;
            if (route && route.length > 0) {
                if (this.prefix.length > 0)
                    _route += this.separator;

                _route += route;
            }

            if (this.createTrailingSlashRoutes) {
                this.routes[_route + '/'] = name;
                Backbone.Router.prototype.route.call(this, _route + '/', name, callback);
            }

            // remove the un-prefixed route from our routes hash
            delete this.routes[route];

            // add the prefixed-route.  note that this routes hash is just provided 
            // for informational and debugging purposes and is not used by the actual routing code.
            this.routes[_route] = name;

            // delegate the creation of the properly-prefixed route to Backbone
            return Backbone.Router.prototype.route.call(this, _route, name, callback);
        }
    });
    return Backbone.SubRoute;
}));

},{"backbone":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js","underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\backbone.js":[function(require,module,exports){
//     Backbone.js 1.1.2

//     (c) 2010-2014 Jeremy Ashkenas, DocumentCloud and Investigative Reporters & Editors
//     Backbone may be freely distributed under the MIT license.
//     For all details and documentation:
//     http://backbonejs.org

(function(root, factory) {

  // Set up Backbone appropriately for the environment. Start with AMD.
  if (typeof define === 'function' && define.amd) {
    define(['underscore', 'jquery', 'exports'], function(_, $, exports) {
      // Export global even in AMD case in case this script is loaded with
      // others that may still expect a global Backbone.
      root.Backbone = factory(root, exports, _, $);
    });

  // Next for Node.js or CommonJS. jQuery may not be needed as a module.
  } else if (typeof exports !== 'undefined') {
    var _ = require('underscore');
    factory(root, exports, _);

  // Finally, as a browser global.
  } else {
    root.Backbone = factory(root, {}, root._, (root.jQuery || root.Zepto || root.ender || root.$));
  }

}(this, function(root, Backbone, _, $) {

  // Initial Setup
  // -------------

  // Save the previous value of the `Backbone` variable, so that it can be
  // restored later on, if `noConflict` is used.
  var previousBackbone = root.Backbone;

  // Create local references to array methods we'll want to use later.
  var array = [];
  var push = array.push;
  var slice = array.slice;
  var splice = array.splice;

  // Current version of the library. Keep in sync with `package.json`.
  Backbone.VERSION = '1.1.2';

  // For Backbone's purposes, jQuery, Zepto, Ender, or My Library (kidding) owns
  // the `$` variable.
  Backbone.$ = $;

  // Runs Backbone.js in *noConflict* mode, returning the `Backbone` variable
  // to its previous owner. Returns a reference to this Backbone object.
  Backbone.noConflict = function() {
    root.Backbone = previousBackbone;
    return this;
  };

  // Turn on `emulateHTTP` to support legacy HTTP servers. Setting this option
  // will fake `"PATCH"`, `"PUT"` and `"DELETE"` requests via the `_method` parameter and
  // set a `X-Http-Method-Override` header.
  Backbone.emulateHTTP = false;

  // Turn on `emulateJSON` to support legacy servers that can't deal with direct
  // `application/json` requests ... will encode the body as
  // `application/x-www-form-urlencoded` instead and will send the model in a
  // form param named `model`.
  Backbone.emulateJSON = false;

  // Backbone.Events
  // ---------------

  // A module that can be mixed in to *any object* in order to provide it with
  // custom events. You may bind with `on` or remove with `off` callback
  // functions to an event; `trigger`-ing an event fires all callbacks in
  // succession.
  //
  //     var object = {};
  //     _.extend(object, Backbone.Events);
  //     object.on('expand', function(){ alert('expanded'); });
  //     object.trigger('expand');
  //
  var Events = Backbone.Events = {

    // Bind an event to a `callback` function. Passing `"all"` will bind
    // the callback to all events fired.
    on: function(name, callback, context) {
      if (!eventsApi(this, 'on', name, [callback, context]) || !callback) return this;
      this._events || (this._events = {});
      var events = this._events[name] || (this._events[name] = []);
      events.push({callback: callback, context: context, ctx: context || this});
      return this;
    },

    // Bind an event to only be triggered a single time. After the first time
    // the callback is invoked, it will be removed.
    once: function(name, callback, context) {
      if (!eventsApi(this, 'once', name, [callback, context]) || !callback) return this;
      var self = this;
      var once = _.once(function() {
        self.off(name, once);
        callback.apply(this, arguments);
      });
      once._callback = callback;
      return this.on(name, once, context);
    },

    // Remove one or many callbacks. If `context` is null, removes all
    // callbacks with that function. If `callback` is null, removes all
    // callbacks for the event. If `name` is null, removes all bound
    // callbacks for all events.
    off: function(name, callback, context) {
      var retain, ev, events, names, i, l, j, k;
      if (!this._events || !eventsApi(this, 'off', name, [callback, context])) return this;
      if (!name && !callback && !context) {
        this._events = void 0;
        return this;
      }
      names = name ? [name] : _.keys(this._events);
      for (i = 0, l = names.length; i < l; i++) {
        name = names[i];
        if (events = this._events[name]) {
          this._events[name] = retain = [];
          if (callback || context) {
            for (j = 0, k = events.length; j < k; j++) {
              ev = events[j];
              if ((callback && callback !== ev.callback && callback !== ev.callback._callback) ||
                  (context && context !== ev.context)) {
                retain.push(ev);
              }
            }
          }
          if (!retain.length) delete this._events[name];
        }
      }

      return this;
    },

    // Trigger one or many events, firing all bound callbacks. Callbacks are
    // passed the same arguments as `trigger` is, apart from the event name
    // (unless you're listening on `"all"`, which will cause your callback to
    // receive the true name of the event as the first argument).
    trigger: function(name) {
      if (!this._events) return this;
      var args = slice.call(arguments, 1);
      if (!eventsApi(this, 'trigger', name, args)) return this;
      var events = this._events[name];
      var allEvents = this._events.all;
      if (events) triggerEvents(events, args);
      if (allEvents) triggerEvents(allEvents, arguments);
      return this;
    },

    // Tell this object to stop listening to either specific events ... or
    // to every object it's currently listening to.
    stopListening: function(obj, name, callback) {
      var listeningTo = this._listeningTo;
      if (!listeningTo) return this;
      var remove = !name && !callback;
      if (!callback && typeof name === 'object') callback = this;
      if (obj) (listeningTo = {})[obj._listenId] = obj;
      for (var id in listeningTo) {
        obj = listeningTo[id];
        obj.off(name, callback, this);
        if (remove || _.isEmpty(obj._events)) delete this._listeningTo[id];
      }
      return this;
    }

  };

  // Regular expression used to split event strings.
  var eventSplitter = /\s+/;

  // Implement fancy features of the Events API such as multiple event
  // names `"change blur"` and jQuery-style event maps `{change: action}`
  // in terms of the existing API.
  var eventsApi = function(obj, action, name, rest) {
    if (!name) return true;

    // Handle event maps.
    if (typeof name === 'object') {
      for (var key in name) {
        obj[action].apply(obj, [key, name[key]].concat(rest));
      }
      return false;
    }

    // Handle space separated event names.
    if (eventSplitter.test(name)) {
      var names = name.split(eventSplitter);
      for (var i = 0, l = names.length; i < l; i++) {
        obj[action].apply(obj, [names[i]].concat(rest));
      }
      return false;
    }

    return true;
  };

  // A difficult-to-believe, but optimized internal dispatch function for
  // triggering events. Tries to keep the usual cases speedy (most internal
  // Backbone events have 3 arguments).
  var triggerEvents = function(events, args) {
    var ev, i = -1, l = events.length, a1 = args[0], a2 = args[1], a3 = args[2];
    switch (args.length) {
      case 0: while (++i < l) (ev = events[i]).callback.call(ev.ctx); return;
      case 1: while (++i < l) (ev = events[i]).callback.call(ev.ctx, a1); return;
      case 2: while (++i < l) (ev = events[i]).callback.call(ev.ctx, a1, a2); return;
      case 3: while (++i < l) (ev = events[i]).callback.call(ev.ctx, a1, a2, a3); return;
      default: while (++i < l) (ev = events[i]).callback.apply(ev.ctx, args); return;
    }
  };

  var listenMethods = {listenTo: 'on', listenToOnce: 'once'};

  // Inversion-of-control versions of `on` and `once`. Tell *this* object to
  // listen to an event in another object ... keeping track of what it's
  // listening to.
  _.each(listenMethods, function(implementation, method) {
    Events[method] = function(obj, name, callback) {
      var listeningTo = this._listeningTo || (this._listeningTo = {});
      var id = obj._listenId || (obj._listenId = _.uniqueId('l'));
      listeningTo[id] = obj;
      if (!callback && typeof name === 'object') callback = this;
      obj[implementation](name, callback, this);
      return this;
    };
  });

  // Aliases for backwards compatibility.
  Events.bind   = Events.on;
  Events.unbind = Events.off;

  // Allow the `Backbone` object to serve as a global event bus, for folks who
  // want global "pubsub" in a convenient place.
  _.extend(Backbone, Events);

  // Backbone.Model
  // --------------

  // Backbone **Models** are the basic data object in the framework --
  // frequently representing a row in a table in a database on your server.
  // A discrete chunk of data and a bunch of useful, related methods for
  // performing computations and transformations on that data.

  // Create a new model with the specified attributes. A client id (`cid`)
  // is automatically generated and assigned for you.
  var Model = Backbone.Model = function(attributes, options) {
    var attrs = attributes || {};
    options || (options = {});
    this.cid = _.uniqueId('c');
    this.attributes = {};
    if (options.collection) this.collection = options.collection;
    if (options.parse) attrs = this.parse(attrs, options) || {};
    attrs = _.defaults({}, attrs, _.result(this, 'defaults'));
    this.set(attrs, options);
    this.changed = {};
    this.initialize.apply(this, arguments);
  };

  // Attach all inheritable methods to the Model prototype.
  _.extend(Model.prototype, Events, {

    // A hash of attributes whose current and previous value differ.
    changed: null,

    // The value returned during the last failed validation.
    validationError: null,

    // The default name for the JSON `id` attribute is `"id"`. MongoDB and
    // CouchDB users may want to set this to `"_id"`.
    idAttribute: 'id',

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // Return a copy of the model's `attributes` object.
    toJSON: function(options) {
      return _.clone(this.attributes);
    },

    // Proxy `Backbone.sync` by default -- but override this if you need
    // custom syncing semantics for *this* particular model.
    sync: function() {
      return Backbone.sync.apply(this, arguments);
    },

    // Get the value of an attribute.
    get: function(attr) {
      return this.attributes[attr];
    },

    // Get the HTML-escaped value of an attribute.
    escape: function(attr) {
      return _.escape(this.get(attr));
    },

    // Returns `true` if the attribute contains a value that is not null
    // or undefined.
    has: function(attr) {
      return this.get(attr) != null;
    },

    // Set a hash of model attributes on the object, firing `"change"`. This is
    // the core primitive operation of a model, updating the data and notifying
    // anyone who needs to know about the change in state. The heart of the beast.
    set: function(key, val, options) {
      var attr, attrs, unset, changes, silent, changing, prev, current;
      if (key == null) return this;

      // Handle both `"key", value` and `{key: value}` -style arguments.
      if (typeof key === 'object') {
        attrs = key;
        options = val;
      } else {
        (attrs = {})[key] = val;
      }

      options || (options = {});

      // Run validation.
      if (!this._validate(attrs, options)) return false;

      // Extract attributes and options.
      unset           = options.unset;
      silent          = options.silent;
      changes         = [];
      changing        = this._changing;
      this._changing  = true;

      if (!changing) {
        this._previousAttributes = _.clone(this.attributes);
        this.changed = {};
      }
      current = this.attributes, prev = this._previousAttributes;

      // Check for changes of `id`.
      if (this.idAttribute in attrs) this.id = attrs[this.idAttribute];

      // For each `set` attribute, update or delete the current value.
      for (attr in attrs) {
        val = attrs[attr];
        if (!_.isEqual(current[attr], val)) changes.push(attr);
        if (!_.isEqual(prev[attr], val)) {
          this.changed[attr] = val;
        } else {
          delete this.changed[attr];
        }
        unset ? delete current[attr] : current[attr] = val;
      }

      // Trigger all relevant attribute changes.
      if (!silent) {
        if (changes.length) this._pending = options;
        for (var i = 0, l = changes.length; i < l; i++) {
          this.trigger('change:' + changes[i], this, current[changes[i]], options);
        }
      }

      // You might be wondering why there's a `while` loop here. Changes can
      // be recursively nested within `"change"` events.
      if (changing) return this;
      if (!silent) {
        while (this._pending) {
          options = this._pending;
          this._pending = false;
          this.trigger('change', this, options);
        }
      }
      this._pending = false;
      this._changing = false;
      return this;
    },

    // Remove an attribute from the model, firing `"change"`. `unset` is a noop
    // if the attribute doesn't exist.
    unset: function(attr, options) {
      return this.set(attr, void 0, _.extend({}, options, {unset: true}));
    },

    // Clear all attributes on the model, firing `"change"`.
    clear: function(options) {
      var attrs = {};
      for (var key in this.attributes) attrs[key] = void 0;
      return this.set(attrs, _.extend({}, options, {unset: true}));
    },

    // Determine if the model has changed since the last `"change"` event.
    // If you specify an attribute name, determine if that attribute has changed.
    hasChanged: function(attr) {
      if (attr == null) return !_.isEmpty(this.changed);
      return _.has(this.changed, attr);
    },

    // Return an object containing all the attributes that have changed, or
    // false if there are no changed attributes. Useful for determining what
    // parts of a view need to be updated and/or what attributes need to be
    // persisted to the server. Unset attributes will be set to undefined.
    // You can also pass an attributes object to diff against the model,
    // determining if there *would be* a change.
    changedAttributes: function(diff) {
      if (!diff) return this.hasChanged() ? _.clone(this.changed) : false;
      var val, changed = false;
      var old = this._changing ? this._previousAttributes : this.attributes;
      for (var attr in diff) {
        if (_.isEqual(old[attr], (val = diff[attr]))) continue;
        (changed || (changed = {}))[attr] = val;
      }
      return changed;
    },

    // Get the previous value of an attribute, recorded at the time the last
    // `"change"` event was fired.
    previous: function(attr) {
      if (attr == null || !this._previousAttributes) return null;
      return this._previousAttributes[attr];
    },

    // Get all of the attributes of the model at the time of the previous
    // `"change"` event.
    previousAttributes: function() {
      return _.clone(this._previousAttributes);
    },

    // Fetch the model from the server. If the server's representation of the
    // model differs from its current attributes, they will be overridden,
    // triggering a `"change"` event.
    fetch: function(options) {
      options = options ? _.clone(options) : {};
      if (options.parse === void 0) options.parse = true;
      var model = this;
      var success = options.success;
      options.success = function(resp) {
        if (!model.set(model.parse(resp, options), options)) return false;
        if (success) success(model, resp, options);
        model.trigger('sync', model, resp, options);
      };
      wrapError(this, options);
      return this.sync('read', this, options);
    },

    // Set a hash of model attributes, and sync the model to the server.
    // If the server returns an attributes hash that differs, the model's
    // state will be `set` again.
    save: function(key, val, options) {
      var attrs, method, xhr, attributes = this.attributes;

      // Handle both `"key", value` and `{key: value}` -style arguments.
      if (key == null || typeof key === 'object') {
        attrs = key;
        options = val;
      } else {
        (attrs = {})[key] = val;
      }

      options = _.extend({validate: true}, options);

      // If we're not waiting and attributes exist, save acts as
      // `set(attr).save(null, opts)` with validation. Otherwise, check if
      // the model will be valid when the attributes, if any, are set.
      if (attrs && !options.wait) {
        if (!this.set(attrs, options)) return false;
      } else {
        if (!this._validate(attrs, options)) return false;
      }

      // Set temporary attributes if `{wait: true}`.
      if (attrs && options.wait) {
        this.attributes = _.extend({}, attributes, attrs);
      }

      // After a successful server-side save, the client is (optionally)
      // updated with the server-side state.
      if (options.parse === void 0) options.parse = true;
      var model = this;
      var success = options.success;
      options.success = function(resp) {
        // Ensure attributes are restored during synchronous saves.
        model.attributes = attributes;
        var serverAttrs = model.parse(resp, options);
        if (options.wait) serverAttrs = _.extend(attrs || {}, serverAttrs);
        if (_.isObject(serverAttrs) && !model.set(serverAttrs, options)) {
          return false;
        }
        if (success) success(model, resp, options);
        model.trigger('sync', model, resp, options);
      };
      wrapError(this, options);

      method = this.isNew() ? 'create' : (options.patch ? 'patch' : 'update');
      if (method === 'patch') options.attrs = attrs;
      xhr = this.sync(method, this, options);

      // Restore attributes.
      if (attrs && options.wait) this.attributes = attributes;

      return xhr;
    },

    // Destroy this model on the server if it was already persisted.
    // Optimistically removes the model from its collection, if it has one.
    // If `wait: true` is passed, waits for the server to respond before removal.
    destroy: function(options) {
      options = options ? _.clone(options) : {};
      var model = this;
      var success = options.success;

      var destroy = function() {
        model.trigger('destroy', model, model.collection, options);
      };

      options.success = function(resp) {
        if (options.wait || model.isNew()) destroy();
        if (success) success(model, resp, options);
        if (!model.isNew()) model.trigger('sync', model, resp, options);
      };

      if (this.isNew()) {
        options.success();
        return false;
      }
      wrapError(this, options);

      var xhr = this.sync('delete', this, options);
      if (!options.wait) destroy();
      return xhr;
    },

    // Default URL for the model's representation on the server -- if you're
    // using Backbone's restful methods, override this to change the endpoint
    // that will be called.
    url: function() {
      var base =
        _.result(this, 'urlRoot') ||
        _.result(this.collection, 'url') ||
        urlError();
      if (this.isNew()) return base;
      return base.replace(/([^\/])$/, '$1/') + encodeURIComponent(this.id);
    },

    // **parse** converts a response into the hash of attributes to be `set` on
    // the model. The default implementation is just to pass the response along.
    parse: function(resp, options) {
      return resp;
    },

    // Create a new model with identical attributes to this one.
    clone: function() {
      return new this.constructor(this.attributes);
    },

    // A model is new if it has never been saved to the server, and lacks an id.
    isNew: function() {
      return !this.has(this.idAttribute);
    },

    // Check if the model is currently in a valid state.
    isValid: function(options) {
      return this._validate({}, _.extend(options || {}, { validate: true }));
    },

    // Run validation against the next complete set of model attributes,
    // returning `true` if all is well. Otherwise, fire an `"invalid"` event.
    _validate: function(attrs, options) {
      if (!options.validate || !this.validate) return true;
      attrs = _.extend({}, this.attributes, attrs);
      var error = this.validationError = this.validate(attrs, options) || null;
      if (!error) return true;
      this.trigger('invalid', this, error, _.extend(options, {validationError: error}));
      return false;
    }

  });

  // Underscore methods that we want to implement on the Model.
  var modelMethods = ['keys', 'values', 'pairs', 'invert', 'pick', 'omit'];

  // Mix in each Underscore method as a proxy to `Model#attributes`.
  _.each(modelMethods, function(method) {
    Model.prototype[method] = function() {
      var args = slice.call(arguments);
      args.unshift(this.attributes);
      return _[method].apply(_, args);
    };
  });

  // Backbone.Collection
  // -------------------

  // If models tend to represent a single row of data, a Backbone Collection is
  // more analagous to a table full of data ... or a small slice or page of that
  // table, or a collection of rows that belong together for a particular reason
  // -- all of the messages in this particular folder, all of the documents
  // belonging to this particular author, and so on. Collections maintain
  // indexes of their models, both in order, and for lookup by `id`.

  // Create a new **Collection**, perhaps to contain a specific type of `model`.
  // If a `comparator` is specified, the Collection will maintain
  // its models in sort order, as they're added and removed.
  var Collection = Backbone.Collection = function(models, options) {
    options || (options = {});
    if (options.model) this.model = options.model;
    if (options.comparator !== void 0) this.comparator = options.comparator;
    this._reset();
    this.initialize.apply(this, arguments);
    if (models) this.reset(models, _.extend({silent: true}, options));
  };

  // Default options for `Collection#set`.
  var setOptions = {add: true, remove: true, merge: true};
  var addOptions = {add: true, remove: false};

  // Define the Collection's inheritable methods.
  _.extend(Collection.prototype, Events, {

    // The default model for a collection is just a **Backbone.Model**.
    // This should be overridden in most cases.
    model: Model,

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // The JSON representation of a Collection is an array of the
    // models' attributes.
    toJSON: function(options) {
      return this.map(function(model){ return model.toJSON(options); });
    },

    // Proxy `Backbone.sync` by default.
    sync: function() {
      return Backbone.sync.apply(this, arguments);
    },

    // Add a model, or list of models to the set.
    add: function(models, options) {
      return this.set(models, _.extend({merge: false}, options, addOptions));
    },

    // Remove a model, or a list of models from the set.
    remove: function(models, options) {
      var singular = !_.isArray(models);
      models = singular ? [models] : _.clone(models);
      options || (options = {});
      var i, l, index, model;
      for (i = 0, l = models.length; i < l; i++) {
        model = models[i] = this.get(models[i]);
        if (!model) continue;
        delete this._byId[model.id];
        delete this._byId[model.cid];
        index = this.indexOf(model);
        this.models.splice(index, 1);
        this.length--;
        if (!options.silent) {
          options.index = index;
          model.trigger('remove', model, this, options);
        }
        this._removeReference(model, options);
      }
      return singular ? models[0] : models;
    },

    // Update a collection by `set`-ing a new list of models, adding new ones,
    // removing models that are no longer present, and merging models that
    // already exist in the collection, as necessary. Similar to **Model#set**,
    // the core operation for updating the data contained by the collection.
    set: function(models, options) {
      options = _.defaults({}, options, setOptions);
      if (options.parse) models = this.parse(models, options);
      var singular = !_.isArray(models);
      models = singular ? (models ? [models] : []) : _.clone(models);
      var i, l, id, model, attrs, existing, sort;
      var at = options.at;
      var targetModel = this.model;
      var sortable = this.comparator && (at == null) && options.sort !== false;
      var sortAttr = _.isString(this.comparator) ? this.comparator : null;
      var toAdd = [], toRemove = [], modelMap = {};
      var add = options.add, merge = options.merge, remove = options.remove;
      var order = !sortable && add && remove ? [] : false;

      // Turn bare objects into model references, and prevent invalid models
      // from being added.
      for (i = 0, l = models.length; i < l; i++) {
        attrs = models[i] || {};
        if (attrs instanceof Model) {
          id = model = attrs;
        } else {
          id = attrs[targetModel.prototype.idAttribute || 'id'];
        }

        // If a duplicate is found, prevent it from being added and
        // optionally merge it into the existing model.
        if (existing = this.get(id)) {
          if (remove) modelMap[existing.cid] = true;
          if (merge) {
            attrs = attrs === model ? model.attributes : attrs;
            if (options.parse) attrs = existing.parse(attrs, options);
            existing.set(attrs, options);
            if (sortable && !sort && existing.hasChanged(sortAttr)) sort = true;
          }
          models[i] = existing;

        // If this is a new, valid model, push it to the `toAdd` list.
        } else if (add) {
          model = models[i] = this._prepareModel(attrs, options);
          if (!model) continue;
          toAdd.push(model);
          this._addReference(model, options);
        }

        // Do not add multiple models with the same `id`.
        model = existing || model;
        if (order && (model.isNew() || !modelMap[model.id])) order.push(model);
        modelMap[model.id] = true;
      }

      // Remove nonexistent models if appropriate.
      if (remove) {
        for (i = 0, l = this.length; i < l; ++i) {
          if (!modelMap[(model = this.models[i]).cid]) toRemove.push(model);
        }
        if (toRemove.length) this.remove(toRemove, options);
      }

      // See if sorting is needed, update `length` and splice in new models.
      if (toAdd.length || (order && order.length)) {
        if (sortable) sort = true;
        this.length += toAdd.length;
        if (at != null) {
          for (i = 0, l = toAdd.length; i < l; i++) {
            this.models.splice(at + i, 0, toAdd[i]);
          }
        } else {
          if (order) this.models.length = 0;
          var orderedModels = order || toAdd;
          for (i = 0, l = orderedModels.length; i < l; i++) {
            this.models.push(orderedModels[i]);
          }
        }
      }

      // Silently sort the collection if appropriate.
      if (sort) this.sort({silent: true});

      // Unless silenced, it's time to fire all appropriate add/sort events.
      if (!options.silent) {
        for (i = 0, l = toAdd.length; i < l; i++) {
          (model = toAdd[i]).trigger('add', model, this, options);
        }
        if (sort || (order && order.length)) this.trigger('sort', this, options);
      }

      // Return the added (or merged) model (or models).
      return singular ? models[0] : models;
    },

    // When you have more items than you want to add or remove individually,
    // you can reset the entire set with a new list of models, without firing
    // any granular `add` or `remove` events. Fires `reset` when finished.
    // Useful for bulk operations and optimizations.
    reset: function(models, options) {
      options || (options = {});
      for (var i = 0, l = this.models.length; i < l; i++) {
        this._removeReference(this.models[i], options);
      }
      options.previousModels = this.models;
      this._reset();
      models = this.add(models, _.extend({silent: true}, options));
      if (!options.silent) this.trigger('reset', this, options);
      return models;
    },

    // Add a model to the end of the collection.
    push: function(model, options) {
      return this.add(model, _.extend({at: this.length}, options));
    },

    // Remove a model from the end of the collection.
    pop: function(options) {
      var model = this.at(this.length - 1);
      this.remove(model, options);
      return model;
    },

    // Add a model to the beginning of the collection.
    unshift: function(model, options) {
      return this.add(model, _.extend({at: 0}, options));
    },

    // Remove a model from the beginning of the collection.
    shift: function(options) {
      var model = this.at(0);
      this.remove(model, options);
      return model;
    },

    // Slice out a sub-array of models from the collection.
    slice: function() {
      return slice.apply(this.models, arguments);
    },

    // Get a model from the set by id.
    get: function(obj) {
      if (obj == null) return void 0;
      return this._byId[obj] || this._byId[obj.id] || this._byId[obj.cid];
    },

    // Get the model at the given index.
    at: function(index) {
      return this.models[index];
    },

    // Return models with matching attributes. Useful for simple cases of
    // `filter`.
    where: function(attrs, first) {
      if (_.isEmpty(attrs)) return first ? void 0 : [];
      return this[first ? 'find' : 'filter'](function(model) {
        for (var key in attrs) {
          if (attrs[key] !== model.get(key)) return false;
        }
        return true;
      });
    },

    // Return the first model with matching attributes. Useful for simple cases
    // of `find`.
    findWhere: function(attrs) {
      return this.where(attrs, true);
    },

    // Force the collection to re-sort itself. You don't need to call this under
    // normal circumstances, as the set will maintain sort order as each item
    // is added.
    sort: function(options) {
      if (!this.comparator) throw new Error('Cannot sort a set without a comparator');
      options || (options = {});

      // Run sort based on type of `comparator`.
      if (_.isString(this.comparator) || this.comparator.length === 1) {
        this.models = this.sortBy(this.comparator, this);
      } else {
        this.models.sort(_.bind(this.comparator, this));
      }

      if (!options.silent) this.trigger('sort', this, options);
      return this;
    },

    // Pluck an attribute from each model in the collection.
    pluck: function(attr) {
      return _.invoke(this.models, 'get', attr);
    },

    // Fetch the default set of models for this collection, resetting the
    // collection when they arrive. If `reset: true` is passed, the response
    // data will be passed through the `reset` method instead of `set`.
    fetch: function(options) {
      options = options ? _.clone(options) : {};
      if (options.parse === void 0) options.parse = true;
      var success = options.success;
      var collection = this;
      options.success = function(resp) {
        var method = options.reset ? 'reset' : 'set';
        collection[method](resp, options);
        if (success) success(collection, resp, options);
        collection.trigger('sync', collection, resp, options);
      };
      wrapError(this, options);
      return this.sync('read', this, options);
    },

    // Create a new instance of a model in this collection. Add the model to the
    // collection immediately, unless `wait: true` is passed, in which case we
    // wait for the server to agree.
    create: function(model, options) {
      options = options ? _.clone(options) : {};
      if (!(model = this._prepareModel(model, options))) return false;
      if (!options.wait) this.add(model, options);
      var collection = this;
      var success = options.success;
      options.success = function(model, resp) {
        if (options.wait) collection.add(model, options);
        if (success) success(model, resp, options);
      };
      model.save(null, options);
      return model;
    },

    // **parse** converts a response into a list of models to be added to the
    // collection. The default implementation is just to pass it through.
    parse: function(resp, options) {
      return resp;
    },

    // Create a new collection with an identical list of models as this one.
    clone: function() {
      return new this.constructor(this.models);
    },

    // Private method to reset all internal state. Called when the collection
    // is first initialized or reset.
    _reset: function() {
      this.length = 0;
      this.models = [];
      this._byId  = {};
    },

    // Prepare a hash of attributes (or other model) to be added to this
    // collection.
    _prepareModel: function(attrs, options) {
      if (attrs instanceof Model) return attrs;
      options = options ? _.clone(options) : {};
      options.collection = this;
      var model = new this.model(attrs, options);
      if (!model.validationError) return model;
      this.trigger('invalid', this, model.validationError, options);
      return false;
    },

    // Internal method to create a model's ties to a collection.
    _addReference: function(model, options) {
      this._byId[model.cid] = model;
      if (model.id != null) this._byId[model.id] = model;
      if (!model.collection) model.collection = this;
      model.on('all', this._onModelEvent, this);
    },

    // Internal method to sever a model's ties to a collection.
    _removeReference: function(model, options) {
      if (this === model.collection) delete model.collection;
      model.off('all', this._onModelEvent, this);
    },

    // Internal method called every time a model in the set fires an event.
    // Sets need to update their indexes when models change ids. All other
    // events simply proxy through. "add" and "remove" events that originate
    // in other collections are ignored.
    _onModelEvent: function(event, model, collection, options) {
      if ((event === 'add' || event === 'remove') && collection !== this) return;
      if (event === 'destroy') this.remove(model, options);
      if (model && event === 'change:' + model.idAttribute) {
        delete this._byId[model.previous(model.idAttribute)];
        if (model.id != null) this._byId[model.id] = model;
      }
      this.trigger.apply(this, arguments);
    }

  });

  // Underscore methods that we want to implement on the Collection.
  // 90% of the core usefulness of Backbone Collections is actually implemented
  // right here:
  var methods = ['forEach', 'each', 'map', 'collect', 'reduce', 'foldl',
    'inject', 'reduceRight', 'foldr', 'find', 'detect', 'filter', 'select',
    'reject', 'every', 'all', 'some', 'any', 'include', 'contains', 'invoke',
    'max', 'min', 'toArray', 'size', 'first', 'head', 'take', 'initial', 'rest',
    'tail', 'drop', 'last', 'without', 'difference', 'indexOf', 'shuffle',
    'lastIndexOf', 'isEmpty', 'chain', 'sample'];

  // Mix in each Underscore method as a proxy to `Collection#models`.
  _.each(methods, function(method) {
    Collection.prototype[method] = function() {
      var args = slice.call(arguments);
      args.unshift(this.models);
      return _[method].apply(_, args);
    };
  });

  // Underscore methods that take a property name as an argument.
  var attributeMethods = ['groupBy', 'countBy', 'sortBy', 'indexBy'];

  // Use attributes instead of properties.
  _.each(attributeMethods, function(method) {
    Collection.prototype[method] = function(value, context) {
      var iterator = _.isFunction(value) ? value : function(model) {
        return model.get(value);
      };
      return _[method](this.models, iterator, context);
    };
  });

  // Backbone.View
  // -------------

  // Backbone Views are almost more convention than they are actual code. A View
  // is simply a JavaScript object that represents a logical chunk of UI in the
  // DOM. This might be a single item, an entire list, a sidebar or panel, or
  // even the surrounding frame which wraps your whole app. Defining a chunk of
  // UI as a **View** allows you to define your DOM events declaratively, without
  // having to worry about render order ... and makes it easy for the view to
  // react to specific changes in the state of your models.

  // Creating a Backbone.View creates its initial element outside of the DOM,
  // if an existing element is not provided...
  var View = Backbone.View = function(options) {
    this.cid = _.uniqueId('view');
    options || (options = {});
    _.extend(this, _.pick(options, viewOptions));
    this._ensureElement();
    this.initialize.apply(this, arguments);
    this.delegateEvents();
  };

  // Cached regex to split keys for `delegate`.
  var delegateEventSplitter = /^(\S+)\s*(.*)$/;

  // List of view options to be merged as properties.
  var viewOptions = ['model', 'collection', 'el', 'id', 'attributes', 'className', 'tagName', 'events'];

  // Set up all inheritable **Backbone.View** properties and methods.
  _.extend(View.prototype, Events, {

    // The default `tagName` of a View's element is `"div"`.
    tagName: 'div',

    // jQuery delegate for element lookup, scoped to DOM elements within the
    // current view. This should be preferred to global lookups where possible.
    $: function(selector) {
      return this.$el.find(selector);
    },

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // **render** is the core function that your view should override, in order
    // to populate its element (`this.el`), with the appropriate HTML. The
    // convention is for **render** to always return `this`.
    render: function() {
      return this;
    },

    // Remove this view by taking the element out of the DOM, and removing any
    // applicable Backbone.Events listeners.
    remove: function() {
      this.$el.remove();
      this.stopListening();
      return this;
    },

    // Change the view's element (`this.el` property), including event
    // re-delegation.
    setElement: function(element, delegate) {
      if (this.$el) this.undelegateEvents();
      this.$el = element instanceof Backbone.$ ? element : Backbone.$(element);
      this.el = this.$el[0];
      if (delegate !== false) this.delegateEvents();
      return this;
    },

    // Set callbacks, where `this.events` is a hash of
    //
    // *{"event selector": "callback"}*
    //
    //     {
    //       'mousedown .title':  'edit',
    //       'click .button':     'save',
    //       'click .open':       function(e) { ... }
    //     }
    //
    // pairs. Callbacks will be bound to the view, with `this` set properly.
    // Uses event delegation for efficiency.
    // Omitting the selector binds the event to `this.el`.
    // This only works for delegate-able events: not `focus`, `blur`, and
    // not `change`, `submit`, and `reset` in Internet Explorer.
    delegateEvents: function(events) {
      if (!(events || (events = _.result(this, 'events')))) return this;
      this.undelegateEvents();
      for (var key in events) {
        var method = events[key];
        if (!_.isFunction(method)) method = this[events[key]];
        if (!method) continue;

        var match = key.match(delegateEventSplitter);
        var eventName = match[1], selector = match[2];
        method = _.bind(method, this);
        eventName += '.delegateEvents' + this.cid;
        if (selector === '') {
          this.$el.on(eventName, method);
        } else {
          this.$el.on(eventName, selector, method);
        }
      }
      return this;
    },

    // Clears all callbacks previously bound to the view with `delegateEvents`.
    // You usually don't need to use this, but may wish to if you have multiple
    // Backbone views attached to the same DOM element.
    undelegateEvents: function() {
      this.$el.off('.delegateEvents' + this.cid);
      return this;
    },

    // Ensure that the View has a DOM element to render into.
    // If `this.el` is a string, pass it through `$()`, take the first
    // matching element, and re-assign it to `el`. Otherwise, create
    // an element from the `id`, `className` and `tagName` properties.
    _ensureElement: function() {
      if (!this.el) {
        var attrs = _.extend({}, _.result(this, 'attributes'));
        if (this.id) attrs.id = _.result(this, 'id');
        if (this.className) attrs['class'] = _.result(this, 'className');
        var $el = Backbone.$('<' + _.result(this, 'tagName') + '>').attr(attrs);
        this.setElement($el, false);
      } else {
        this.setElement(_.result(this, 'el'), false);
      }
    }

  });

  // Backbone.sync
  // -------------

  // Override this function to change the manner in which Backbone persists
  // models to the server. You will be passed the type of request, and the
  // model in question. By default, makes a RESTful Ajax request
  // to the model's `url()`. Some possible customizations could be:
  //
  // * Use `setTimeout` to batch rapid-fire updates into a single request.
  // * Send up the models as XML instead of JSON.
  // * Persist models via WebSockets instead of Ajax.
  //
  // Turn on `Backbone.emulateHTTP` in order to send `PUT` and `DELETE` requests
  // as `POST`, with a `_method` parameter containing the true HTTP method,
  // as well as all requests with the body as `application/x-www-form-urlencoded`
  // instead of `application/json` with the model in a param named `model`.
  // Useful when interfacing with server-side languages like **PHP** that make
  // it difficult to read the body of `PUT` requests.
  Backbone.sync = function(method, model, options) {
    var type = methodMap[method];

    // Default options, unless specified.
    _.defaults(options || (options = {}), {
      emulateHTTP: Backbone.emulateHTTP,
      emulateJSON: Backbone.emulateJSON
    });

    // Default JSON-request options.
    var params = {type: type, dataType: 'json'};

    // Ensure that we have a URL.
    if (!options.url) {
      params.url = _.result(model, 'url') || urlError();
    }

    // Ensure that we have the appropriate request data.
    if (options.data == null && model && (method === 'create' || method === 'update' || method === 'patch')) {
      params.contentType = 'application/json';
      params.data = JSON.stringify(options.attrs || model.toJSON(options));
    }

    // For older servers, emulate JSON by encoding the request into an HTML-form.
    if (options.emulateJSON) {
      params.contentType = 'application/x-www-form-urlencoded';
      params.data = params.data ? {model: params.data} : {};
    }

    // For older servers, emulate HTTP by mimicking the HTTP method with `_method`
    // And an `X-HTTP-Method-Override` header.
    if (options.emulateHTTP && (type === 'PUT' || type === 'DELETE' || type === 'PATCH')) {
      params.type = 'POST';
      if (options.emulateJSON) params.data._method = type;
      var beforeSend = options.beforeSend;
      options.beforeSend = function(xhr) {
        xhr.setRequestHeader('X-HTTP-Method-Override', type);
        if (beforeSend) return beforeSend.apply(this, arguments);
      };
    }

    // Don't process data on a non-GET request.
    if (params.type !== 'GET' && !options.emulateJSON) {
      params.processData = false;
    }

    // If we're sending a `PATCH` request, and we're in an old Internet Explorer
    // that still has ActiveX enabled by default, override jQuery to use that
    // for XHR instead. Remove this line when jQuery supports `PATCH` on IE8.
    if (params.type === 'PATCH' && noXhrPatch) {
      params.xhr = function() {
        return new ActiveXObject("Microsoft.XMLHTTP");
      };
    }

    // Make the request, allowing the user to override any Ajax options.
    var xhr = options.xhr = Backbone.ajax(_.extend(params, options));
    model.trigger('request', model, xhr, options);
    return xhr;
  };

  var noXhrPatch =
    typeof window !== 'undefined' && !!window.ActiveXObject &&
      !(window.XMLHttpRequest && (new XMLHttpRequest).dispatchEvent);

  // Map from CRUD to HTTP for our default `Backbone.sync` implementation.
  var methodMap = {
    'create': 'POST',
    'update': 'PUT',
    'patch':  'PATCH',
    'delete': 'DELETE',
    'read':   'GET'
  };

  // Set the default implementation of `Backbone.ajax` to proxy through to `$`.
  // Override this if you'd like to use a different library.
  Backbone.ajax = function() {
    return Backbone.$.ajax.apply(Backbone.$, arguments);
  };

  // Backbone.Router
  // ---------------

  // Routers map faux-URLs to actions, and fire events when routes are
  // matched. Creating a new one sets its `routes` hash, if not set statically.
  var Router = Backbone.Router = function(options) {
    options || (options = {});
    if (options.routes) this.routes = options.routes;
    this._bindRoutes();
    this.initialize.apply(this, arguments);
  };

  // Cached regular expressions for matching named param parts and splatted
  // parts of route strings.
  var optionalParam = /\((.*?)\)/g;
  var namedParam    = /(\(\?)?:\w+/g;
  var splatParam    = /\*\w+/g;
  var escapeRegExp  = /[\-{}\[\]+?.,\\\^$|#\s]/g;

  // Set up all inheritable **Backbone.Router** properties and methods.
  _.extend(Router.prototype, Events, {

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // Manually bind a single named route to a callback. For example:
    //
    //     this.route('search/:query/p:num', 'search', function(query, num) {
    //       ...
    //     });
    //
    route: function(route, name, callback) {
      if (!_.isRegExp(route)) route = this._routeToRegExp(route);
      if (_.isFunction(name)) {
        callback = name;
        name = '';
      }
      if (!callback) callback = this[name];
      var router = this;
      Backbone.history.route(route, function(fragment) {
        var args = router._extractParameters(route, fragment);
        router.execute(callback, args);
        router.trigger.apply(router, ['route:' + name].concat(args));
        router.trigger('route', name, args);
        Backbone.history.trigger('route', router, name, args);
      });
      return this;
    },

    // Execute a route handler with the provided parameters.  This is an
    // excellent place to do pre-route setup or post-route cleanup.
    execute: function(callback, args) {
      if (callback) callback.apply(this, args);
    },

    // Simple proxy to `Backbone.history` to save a fragment into the history.
    navigate: function(fragment, options) {
      Backbone.history.navigate(fragment, options);
      return this;
    },

    // Bind all defined routes to `Backbone.history`. We have to reverse the
    // order of the routes here to support behavior where the most general
    // routes can be defined at the bottom of the route map.
    _bindRoutes: function() {
      if (!this.routes) return;
      this.routes = _.result(this, 'routes');
      var route, routes = _.keys(this.routes);
      while ((route = routes.pop()) != null) {
        this.route(route, this.routes[route]);
      }
    },

    // Convert a route string into a regular expression, suitable for matching
    // against the current location hash.
    _routeToRegExp: function(route) {
      route = route.replace(escapeRegExp, '\\$&')
                   .replace(optionalParam, '(?:$1)?')
                   .replace(namedParam, function(match, optional) {
                     return optional ? match : '([^/?]+)';
                   })
                   .replace(splatParam, '([^?]*?)');
      return new RegExp('^' + route + '(?:\\?([\\s\\S]*))?$');
    },

    // Given a route, and a URL fragment that it matches, return the array of
    // extracted decoded parameters. Empty or unmatched parameters will be
    // treated as `null` to normalize cross-browser behavior.
    _extractParameters: function(route, fragment) {
      var params = route.exec(fragment).slice(1);
      return _.map(params, function(param, i) {
        // Don't decode the search params.
        if (i === params.length - 1) return param || null;
        return param ? decodeURIComponent(param) : null;
      });
    }

  });

  // Backbone.History
  // ----------------

  // Handles cross-browser history management, based on either
  // [pushState](http://diveintohtml5.info/history.html) and real URLs, or
  // [onhashchange](https://developer.mozilla.org/en-US/docs/DOM/window.onhashchange)
  // and URL fragments. If the browser supports neither (old IE, natch),
  // falls back to polling.
  var History = Backbone.History = function() {
    this.handlers = [];
    _.bindAll(this, 'checkUrl');

    // Ensure that `History` can be used outside of the browser.
    if (typeof window !== 'undefined') {
      this.location = window.location;
      this.history = window.history;
    }
  };

  // Cached regex for stripping a leading hash/slash and trailing space.
  var routeStripper = /^[#\/]|\s+$/g;

  // Cached regex for stripping leading and trailing slashes.
  var rootStripper = /^\/+|\/+$/g;

  // Cached regex for detecting MSIE.
  var isExplorer = /msie [\w.]+/;

  // Cached regex for removing a trailing slash.
  var trailingSlash = /\/$/;

  // Cached regex for stripping urls of hash.
  var pathStripper = /#.*$/;

  // Has the history handling already been started?
  History.started = false;

  // Set up all inheritable **Backbone.History** properties and methods.
  _.extend(History.prototype, Events, {

    // The default interval to poll for hash changes, if necessary, is
    // twenty times a second.
    interval: 50,

    // Are we at the app root?
    atRoot: function() {
      return this.location.pathname.replace(/[^\/]$/, '$&/') === this.root;
    },

    // Gets the true hash value. Cannot use location.hash directly due to bug
    // in Firefox where location.hash will always be decoded.
    getHash: function(window) {
      var match = (window || this).location.href.match(/#(.*)$/);
      return match ? match[1] : '';
    },

    // Get the cross-browser normalized URL fragment, either from the URL,
    // the hash, or the override.
    getFragment: function(fragment, forcePushState) {
      if (fragment == null) {
        if (this._hasPushState || !this._wantsHashChange || forcePushState) {
          fragment = decodeURI(this.location.pathname + this.location.search);
          var root = this.root.replace(trailingSlash, '');
          if (!fragment.indexOf(root)) fragment = fragment.slice(root.length);
        } else {
          fragment = this.getHash();
        }
      }
      return fragment.replace(routeStripper, '');
    },

    // Start the hash change handling, returning `true` if the current URL matches
    // an existing route, and `false` otherwise.
    start: function(options) {
      if (History.started) throw new Error("Backbone.history has already been started");
      History.started = true;

      // Figure out the initial configuration. Do we need an iframe?
      // Is pushState desired ... is it available?
      this.options          = _.extend({root: '/'}, this.options, options);
      this.root             = this.options.root;
      this._wantsHashChange = this.options.hashChange !== false;
      this._wantsPushState  = !!this.options.pushState;
      this._hasPushState    = !!(this.options.pushState && this.history && this.history.pushState);
      var fragment          = this.getFragment();
      var docMode           = document.documentMode;
      var oldIE             = (isExplorer.exec(navigator.userAgent.toLowerCase()) && (!docMode || docMode <= 7));

      // Normalize root to always include a leading and trailing slash.
      this.root = ('/' + this.root + '/').replace(rootStripper, '/');

      if (oldIE && this._wantsHashChange) {
        var frame = Backbone.$('<iframe src="javascript:0" tabindex="-1">');
        this.iframe = frame.hide().appendTo('body')[0].contentWindow;
        this.navigate(fragment);
      }

      // Depending on whether we're using pushState or hashes, and whether
      // 'onhashchange' is supported, determine how we check the URL state.
      if (this._hasPushState) {
        Backbone.$(window).on('popstate', this.checkUrl);
      } else if (this._wantsHashChange && ('onhashchange' in window) && !oldIE) {
        Backbone.$(window).on('hashchange', this.checkUrl);
      } else if (this._wantsHashChange) {
        this._checkUrlInterval = setInterval(this.checkUrl, this.interval);
      }

      // Determine if we need to change the base url, for a pushState link
      // opened by a non-pushState browser.
      this.fragment = fragment;
      var loc = this.location;

      // Transition from hashChange to pushState or vice versa if both are
      // requested.
      if (this._wantsHashChange && this._wantsPushState) {

        // If we've started off with a route from a `pushState`-enabled
        // browser, but we're currently in a browser that doesn't support it...
        if (!this._hasPushState && !this.atRoot()) {
          this.fragment = this.getFragment(null, true);
          this.location.replace(this.root + '#' + this.fragment);
          // Return immediately as browser will do redirect to new url
          return true;

        // Or if we've started out with a hash-based route, but we're currently
        // in a browser where it could be `pushState`-based instead...
        } else if (this._hasPushState && this.atRoot() && loc.hash) {
          this.fragment = this.getHash().replace(routeStripper, '');
          this.history.replaceState({}, document.title, this.root + this.fragment);
        }

      }

      if (!this.options.silent) return this.loadUrl();
    },

    // Disable Backbone.history, perhaps temporarily. Not useful in a real app,
    // but possibly useful for unit testing Routers.
    stop: function() {
      Backbone.$(window).off('popstate', this.checkUrl).off('hashchange', this.checkUrl);
      if (this._checkUrlInterval) clearInterval(this._checkUrlInterval);
      History.started = false;
    },

    // Add a route to be tested when the fragment changes. Routes added later
    // may override previous routes.
    route: function(route, callback) {
      this.handlers.unshift({route: route, callback: callback});
    },

    // Checks the current URL to see if it has changed, and if it has,
    // calls `loadUrl`, normalizing across the hidden iframe.
    checkUrl: function(e) {
      var current = this.getFragment();
      if (current === this.fragment && this.iframe) {
        current = this.getFragment(this.getHash(this.iframe));
      }
      if (current === this.fragment) return false;
      if (this.iframe) this.navigate(current);
      this.loadUrl();
    },

    // Attempt to load the current URL fragment. If a route succeeds with a
    // match, returns `true`. If no defined routes matches the fragment,
    // returns `false`.
    loadUrl: function(fragment) {
      fragment = this.fragment = this.getFragment(fragment);
      return _.any(this.handlers, function(handler) {
        if (handler.route.test(fragment)) {
          handler.callback(fragment);
          return true;
        }
      });
    },

    // Save a fragment into the hash history, or replace the URL state if the
    // 'replace' option is passed. You are responsible for properly URL-encoding
    // the fragment in advance.
    //
    // The options object can contain `trigger: true` if you wish to have the
    // route callback be fired (not usually desirable), or `replace: true`, if
    // you wish to modify the current URL without adding an entry to the history.
    navigate: function(fragment, options) {
      if (!History.started) return false;
      if (!options || options === true) options = {trigger: !!options};

      var url = this.root + (fragment = this.getFragment(fragment || ''));

      // Strip the hash for matching.
      fragment = fragment.replace(pathStripper, '');

      if (this.fragment === fragment) return;
      this.fragment = fragment;

      // Don't include a trailing slash on the root.
      if (fragment === '' && url !== '/') url = url.slice(0, -1);

      // If pushState is available, we use it to set the fragment as a real URL.
      if (this._hasPushState) {
        this.history[options.replace ? 'replaceState' : 'pushState']({}, document.title, url);

      // If hash changes haven't been explicitly disabled, update the hash
      // fragment to store history.
      } else if (this._wantsHashChange) {
        this._updateHash(this.location, fragment, options.replace);
        if (this.iframe && (fragment !== this.getFragment(this.getHash(this.iframe)))) {
          // Opening and closing the iframe tricks IE7 and earlier to push a
          // history entry on hash-tag change.  When replace is true, we don't
          // want this.
          if(!options.replace) this.iframe.document.open().close();
          this._updateHash(this.iframe.location, fragment, options.replace);
        }

      // If you've told us that you explicitly don't want fallback hashchange-
      // based history, then `navigate` becomes a page refresh.
      } else {
        return this.location.assign(url);
      }
      if (options.trigger) return this.loadUrl(fragment);
    },

    // Update the hash location, either replacing the current entry, or adding
    // a new one to the browser history.
    _updateHash: function(location, fragment, replace) {
      if (replace) {
        var href = location.href.replace(/(javascript:|#).*$/, '');
        location.replace(href + '#' + fragment);
      } else {
        // Some browsers require that `hash` contains a leading #.
        location.hash = '#' + fragment;
      }
    }

  });

  // Create the default Backbone.history.
  Backbone.history = new History;

  // Helpers
  // -------

  // Helper function to correctly set up the prototype chain, for subclasses.
  // Similar to `goog.inherits`, but uses a hash of prototype properties and
  // class properties to be extended.
  var extend = function(protoProps, staticProps) {
    var parent = this;
    var child;

    // The constructor function for the new subclass is either defined by you
    // (the "constructor" property in your `extend` definition), or defaulted
    // by us to simply call the parent's constructor.
    if (protoProps && _.has(protoProps, 'constructor')) {
      child = protoProps.constructor;
    } else {
      child = function(){ return parent.apply(this, arguments); };
    }

    // Add static properties to the constructor function, if supplied.
    _.extend(child, parent, staticProps);

    // Set the prototype chain to inherit from `parent`, without calling
    // `parent`'s constructor function.
    var Surrogate = function(){ this.constructor = child; };
    Surrogate.prototype = parent.prototype;
    child.prototype = new Surrogate;

    // Add prototype properties (instance properties) to the subclass,
    // if supplied.
    if (protoProps) _.extend(child.prototype, protoProps);

    // Set a convenience property in case the parent's prototype is needed
    // later.
    child.__super__ = parent.prototype;

    return child;
  };

  // Set up inheritance for the model, collection, router, view and history.
  Model.extend = Collection.extend = Router.extend = View.extend = History.extend = extend;

  // Throw an error when a URL is needed, and none is supplied.
  var urlError = function() {
    throw new Error('A "url" property or function must be specified');
  };

  // Wrap an optional error callback with a fallback error event.
  var wrapError = function(model, options) {
    var error = options.error;
    options.error = function(resp) {
      if (error) error(model, resp, options);
      model.trigger('error', model, resp, options);
    };
  };

  return Backbone;

}));

},{"underscore":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\node_modules\\underscore\\underscore.js"}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\node_modules\\underscore\\underscore.js":[function(require,module,exports){
//     Underscore.js 1.7.0
//     http://underscorejs.org
//     (c) 2009-2014 Jeremy Ashkenas, DocumentCloud and Investigative Reporters & Editors
//     Underscore may be freely distributed under the MIT license.

(function() {

  // Baseline setup
  // --------------

  // Establish the root object, `window` in the browser, or `exports` on the server.
  var root = this;

  // Save the previous value of the `_` variable.
  var previousUnderscore = root._;

  // Save bytes in the minified (but not gzipped) version:
  var ArrayProto = Array.prototype, ObjProto = Object.prototype, FuncProto = Function.prototype;

  // Create quick reference variables for speed access to core prototypes.
  var
    push             = ArrayProto.push,
    slice            = ArrayProto.slice,
    concat           = ArrayProto.concat,
    toString         = ObjProto.toString,
    hasOwnProperty   = ObjProto.hasOwnProperty;

  // All **ECMAScript 5** native function implementations that we hope to use
  // are declared here.
  var
    nativeIsArray      = Array.isArray,
    nativeKeys         = Object.keys,
    nativeBind         = FuncProto.bind;

  // Create a safe reference to the Underscore object for use below.
  var _ = function(obj) {
    if (obj instanceof _) return obj;
    if (!(this instanceof _)) return new _(obj);
    this._wrapped = obj;
  };

  // Export the Underscore object for **Node.js**, with
  // backwards-compatibility for the old `require()` API. If we're in
  // the browser, add `_` as a global object.
  if (typeof exports !== 'undefined') {
    if (typeof module !== 'undefined' && module.exports) {
      exports = module.exports = _;
    }
    exports._ = _;
  } else {
    root._ = _;
  }

  // Current version.
  _.VERSION = '1.7.0';

  // Internal function that returns an efficient (for current engines) version
  // of the passed-in callback, to be repeatedly applied in other Underscore
  // functions.
  var createCallback = function(func, context, argCount) {
    if (context === void 0) return func;
    switch (argCount == null ? 3 : argCount) {
      case 1: return function(value) {
        return func.call(context, value);
      };
      case 2: return function(value, other) {
        return func.call(context, value, other);
      };
      case 3: return function(value, index, collection) {
        return func.call(context, value, index, collection);
      };
      case 4: return function(accumulator, value, index, collection) {
        return func.call(context, accumulator, value, index, collection);
      };
    }
    return function() {
      return func.apply(context, arguments);
    };
  };

  // A mostly-internal function to generate callbacks that can be applied
  // to each element in a collection, returning the desired result — either
  // identity, an arbitrary callback, a property matcher, or a property accessor.
  _.iteratee = function(value, context, argCount) {
    if (value == null) return _.identity;
    if (_.isFunction(value)) return createCallback(value, context, argCount);
    if (_.isObject(value)) return _.matches(value);
    return _.property(value);
  };

  // Collection Functions
  // --------------------

  // The cornerstone, an `each` implementation, aka `forEach`.
  // Handles raw objects in addition to array-likes. Treats all
  // sparse array-likes as if they were dense.
  _.each = _.forEach = function(obj, iteratee, context) {
    if (obj == null) return obj;
    iteratee = createCallback(iteratee, context);
    var i, length = obj.length;
    if (length === +length) {
      for (i = 0; i < length; i++) {
        iteratee(obj[i], i, obj);
      }
    } else {
      var keys = _.keys(obj);
      for (i = 0, length = keys.length; i < length; i++) {
        iteratee(obj[keys[i]], keys[i], obj);
      }
    }
    return obj;
  };

  // Return the results of applying the iteratee to each element.
  _.map = _.collect = function(obj, iteratee, context) {
    if (obj == null) return [];
    iteratee = _.iteratee(iteratee, context);
    var keys = obj.length !== +obj.length && _.keys(obj),
        length = (keys || obj).length,
        results = Array(length),
        currentKey;
    for (var index = 0; index < length; index++) {
      currentKey = keys ? keys[index] : index;
      results[index] = iteratee(obj[currentKey], currentKey, obj);
    }
    return results;
  };

  var reduceError = 'Reduce of empty array with no initial value';

  // **Reduce** builds up a single result from a list of values, aka `inject`,
  // or `foldl`.
  _.reduce = _.foldl = _.inject = function(obj, iteratee, memo, context) {
    if (obj == null) obj = [];
    iteratee = createCallback(iteratee, context, 4);
    var keys = obj.length !== +obj.length && _.keys(obj),
        length = (keys || obj).length,
        index = 0, currentKey;
    if (arguments.length < 3) {
      if (!length) throw new TypeError(reduceError);
      memo = obj[keys ? keys[index++] : index++];
    }
    for (; index < length; index++) {
      currentKey = keys ? keys[index] : index;
      memo = iteratee(memo, obj[currentKey], currentKey, obj);
    }
    return memo;
  };

  // The right-associative version of reduce, also known as `foldr`.
  _.reduceRight = _.foldr = function(obj, iteratee, memo, context) {
    if (obj == null) obj = [];
    iteratee = createCallback(iteratee, context, 4);
    var keys = obj.length !== + obj.length && _.keys(obj),
        index = (keys || obj).length,
        currentKey;
    if (arguments.length < 3) {
      if (!index) throw new TypeError(reduceError);
      memo = obj[keys ? keys[--index] : --index];
    }
    while (index--) {
      currentKey = keys ? keys[index] : index;
      memo = iteratee(memo, obj[currentKey], currentKey, obj);
    }
    return memo;
  };

  // Return the first value which passes a truth test. Aliased as `detect`.
  _.find = _.detect = function(obj, predicate, context) {
    var result;
    predicate = _.iteratee(predicate, context);
    _.some(obj, function(value, index, list) {
      if (predicate(value, index, list)) {
        result = value;
        return true;
      }
    });
    return result;
  };

  // Return all the elements that pass a truth test.
  // Aliased as `select`.
  _.filter = _.select = function(obj, predicate, context) {
    var results = [];
    if (obj == null) return results;
    predicate = _.iteratee(predicate, context);
    _.each(obj, function(value, index, list) {
      if (predicate(value, index, list)) results.push(value);
    });
    return results;
  };

  // Return all the elements for which a truth test fails.
  _.reject = function(obj, predicate, context) {
    return _.filter(obj, _.negate(_.iteratee(predicate)), context);
  };

  // Determine whether all of the elements match a truth test.
  // Aliased as `all`.
  _.every = _.all = function(obj, predicate, context) {
    if (obj == null) return true;
    predicate = _.iteratee(predicate, context);
    var keys = obj.length !== +obj.length && _.keys(obj),
        length = (keys || obj).length,
        index, currentKey;
    for (index = 0; index < length; index++) {
      currentKey = keys ? keys[index] : index;
      if (!predicate(obj[currentKey], currentKey, obj)) return false;
    }
    return true;
  };

  // Determine if at least one element in the object matches a truth test.
  // Aliased as `any`.
  _.some = _.any = function(obj, predicate, context) {
    if (obj == null) return false;
    predicate = _.iteratee(predicate, context);
    var keys = obj.length !== +obj.length && _.keys(obj),
        length = (keys || obj).length,
        index, currentKey;
    for (index = 0; index < length; index++) {
      currentKey = keys ? keys[index] : index;
      if (predicate(obj[currentKey], currentKey, obj)) return true;
    }
    return false;
  };

  // Determine if the array or object contains a given value (using `===`).
  // Aliased as `include`.
  _.contains = _.include = function(obj, target) {
    if (obj == null) return false;
    if (obj.length !== +obj.length) obj = _.values(obj);
    return _.indexOf(obj, target) >= 0;
  };

  // Invoke a method (with arguments) on every item in a collection.
  _.invoke = function(obj, method) {
    var args = slice.call(arguments, 2);
    var isFunc = _.isFunction(method);
    return _.map(obj, function(value) {
      return (isFunc ? method : value[method]).apply(value, args);
    });
  };

  // Convenience version of a common use case of `map`: fetching a property.
  _.pluck = function(obj, key) {
    return _.map(obj, _.property(key));
  };

  // Convenience version of a common use case of `filter`: selecting only objects
  // containing specific `key:value` pairs.
  _.where = function(obj, attrs) {
    return _.filter(obj, _.matches(attrs));
  };

  // Convenience version of a common use case of `find`: getting the first object
  // containing specific `key:value` pairs.
  _.findWhere = function(obj, attrs) {
    return _.find(obj, _.matches(attrs));
  };

  // Return the maximum element (or element-based computation).
  _.max = function(obj, iteratee, context) {
    var result = -Infinity, lastComputed = -Infinity,
        value, computed;
    if (iteratee == null && obj != null) {
      obj = obj.length === +obj.length ? obj : _.values(obj);
      for (var i = 0, length = obj.length; i < length; i++) {
        value = obj[i];
        if (value > result) {
          result = value;
        }
      }
    } else {
      iteratee = _.iteratee(iteratee, context);
      _.each(obj, function(value, index, list) {
        computed = iteratee(value, index, list);
        if (computed > lastComputed || computed === -Infinity && result === -Infinity) {
          result = value;
          lastComputed = computed;
        }
      });
    }
    return result;
  };

  // Return the minimum element (or element-based computation).
  _.min = function(obj, iteratee, context) {
    var result = Infinity, lastComputed = Infinity,
        value, computed;
    if (iteratee == null && obj != null) {
      obj = obj.length === +obj.length ? obj : _.values(obj);
      for (var i = 0, length = obj.length; i < length; i++) {
        value = obj[i];
        if (value < result) {
          result = value;
        }
      }
    } else {
      iteratee = _.iteratee(iteratee, context);
      _.each(obj, function(value, index, list) {
        computed = iteratee(value, index, list);
        if (computed < lastComputed || computed === Infinity && result === Infinity) {
          result = value;
          lastComputed = computed;
        }
      });
    }
    return result;
  };

  // Shuffle a collection, using the modern version of the
  // [Fisher-Yates shuffle](http://en.wikipedia.org/wiki/Fisher–Yates_shuffle).
  _.shuffle = function(obj) {
    var set = obj && obj.length === +obj.length ? obj : _.values(obj);
    var length = set.length;
    var shuffled = Array(length);
    for (var index = 0, rand; index < length; index++) {
      rand = _.random(0, index);
      if (rand !== index) shuffled[index] = shuffled[rand];
      shuffled[rand] = set[index];
    }
    return shuffled;
  };

  // Sample **n** random values from a collection.
  // If **n** is not specified, returns a single random element.
  // The internal `guard` argument allows it to work with `map`.
  _.sample = function(obj, n, guard) {
    if (n == null || guard) {
      if (obj.length !== +obj.length) obj = _.values(obj);
      return obj[_.random(obj.length - 1)];
    }
    return _.shuffle(obj).slice(0, Math.max(0, n));
  };

  // Sort the object's values by a criterion produced by an iteratee.
  _.sortBy = function(obj, iteratee, context) {
    iteratee = _.iteratee(iteratee, context);
    return _.pluck(_.map(obj, function(value, index, list) {
      return {
        value: value,
        index: index,
        criteria: iteratee(value, index, list)
      };
    }).sort(function(left, right) {
      var a = left.criteria;
      var b = right.criteria;
      if (a !== b) {
        if (a > b || a === void 0) return 1;
        if (a < b || b === void 0) return -1;
      }
      return left.index - right.index;
    }), 'value');
  };

  // An internal function used for aggregate "group by" operations.
  var group = function(behavior) {
    return function(obj, iteratee, context) {
      var result = {};
      iteratee = _.iteratee(iteratee, context);
      _.each(obj, function(value, index) {
        var key = iteratee(value, index, obj);
        behavior(result, value, key);
      });
      return result;
    };
  };

  // Groups the object's values by a criterion. Pass either a string attribute
  // to group by, or a function that returns the criterion.
  _.groupBy = group(function(result, value, key) {
    if (_.has(result, key)) result[key].push(value); else result[key] = [value];
  });

  // Indexes the object's values by a criterion, similar to `groupBy`, but for
  // when you know that your index values will be unique.
  _.indexBy = group(function(result, value, key) {
    result[key] = value;
  });

  // Counts instances of an object that group by a certain criterion. Pass
  // either a string attribute to count by, or a function that returns the
  // criterion.
  _.countBy = group(function(result, value, key) {
    if (_.has(result, key)) result[key]++; else result[key] = 1;
  });

  // Use a comparator function to figure out the smallest index at which
  // an object should be inserted so as to maintain order. Uses binary search.
  _.sortedIndex = function(array, obj, iteratee, context) {
    iteratee = _.iteratee(iteratee, context, 1);
    var value = iteratee(obj);
    var low = 0, high = array.length;
    while (low < high) {
      var mid = low + high >>> 1;
      if (iteratee(array[mid]) < value) low = mid + 1; else high = mid;
    }
    return low;
  };

  // Safely create a real, live array from anything iterable.
  _.toArray = function(obj) {
    if (!obj) return [];
    if (_.isArray(obj)) return slice.call(obj);
    if (obj.length === +obj.length) return _.map(obj, _.identity);
    return _.values(obj);
  };

  // Return the number of elements in an object.
  _.size = function(obj) {
    if (obj == null) return 0;
    return obj.length === +obj.length ? obj.length : _.keys(obj).length;
  };

  // Split a collection into two arrays: one whose elements all satisfy the given
  // predicate, and one whose elements all do not satisfy the predicate.
  _.partition = function(obj, predicate, context) {
    predicate = _.iteratee(predicate, context);
    var pass = [], fail = [];
    _.each(obj, function(value, key, obj) {
      (predicate(value, key, obj) ? pass : fail).push(value);
    });
    return [pass, fail];
  };

  // Array Functions
  // ---------------

  // Get the first element of an array. Passing **n** will return the first N
  // values in the array. Aliased as `head` and `take`. The **guard** check
  // allows it to work with `_.map`.
  _.first = _.head = _.take = function(array, n, guard) {
    if (array == null) return void 0;
    if (n == null || guard) return array[0];
    if (n < 0) return [];
    return slice.call(array, 0, n);
  };

  // Returns everything but the last entry of the array. Especially useful on
  // the arguments object. Passing **n** will return all the values in
  // the array, excluding the last N. The **guard** check allows it to work with
  // `_.map`.
  _.initial = function(array, n, guard) {
    return slice.call(array, 0, Math.max(0, array.length - (n == null || guard ? 1 : n)));
  };

  // Get the last element of an array. Passing **n** will return the last N
  // values in the array. The **guard** check allows it to work with `_.map`.
  _.last = function(array, n, guard) {
    if (array == null) return void 0;
    if (n == null || guard) return array[array.length - 1];
    return slice.call(array, Math.max(array.length - n, 0));
  };

  // Returns everything but the first entry of the array. Aliased as `tail` and `drop`.
  // Especially useful on the arguments object. Passing an **n** will return
  // the rest N values in the array. The **guard**
  // check allows it to work with `_.map`.
  _.rest = _.tail = _.drop = function(array, n, guard) {
    return slice.call(array, n == null || guard ? 1 : n);
  };

  // Trim out all falsy values from an array.
  _.compact = function(array) {
    return _.filter(array, _.identity);
  };

  // Internal implementation of a recursive `flatten` function.
  var flatten = function(input, shallow, strict, output) {
    if (shallow && _.every(input, _.isArray)) {
      return concat.apply(output, input);
    }
    for (var i = 0, length = input.length; i < length; i++) {
      var value = input[i];
      if (!_.isArray(value) && !_.isArguments(value)) {
        if (!strict) output.push(value);
      } else if (shallow) {
        push.apply(output, value);
      } else {
        flatten(value, shallow, strict, output);
      }
    }
    return output;
  };

  // Flatten out an array, either recursively (by default), or just one level.
  _.flatten = function(array, shallow) {
    return flatten(array, shallow, false, []);
  };

  // Return a version of the array that does not contain the specified value(s).
  _.without = function(array) {
    return _.difference(array, slice.call(arguments, 1));
  };

  // Produce a duplicate-free version of the array. If the array has already
  // been sorted, you have the option of using a faster algorithm.
  // Aliased as `unique`.
  _.uniq = _.unique = function(array, isSorted, iteratee, context) {
    if (array == null) return [];
    if (!_.isBoolean(isSorted)) {
      context = iteratee;
      iteratee = isSorted;
      isSorted = false;
    }
    if (iteratee != null) iteratee = _.iteratee(iteratee, context);
    var result = [];
    var seen = [];
    for (var i = 0, length = array.length; i < length; i++) {
      var value = array[i];
      if (isSorted) {
        if (!i || seen !== value) result.push(value);
        seen = value;
      } else if (iteratee) {
        var computed = iteratee(value, i, array);
        if (_.indexOf(seen, computed) < 0) {
          seen.push(computed);
          result.push(value);
        }
      } else if (_.indexOf(result, value) < 0) {
        result.push(value);
      }
    }
    return result;
  };

  // Produce an array that contains the union: each distinct element from all of
  // the passed-in arrays.
  _.union = function() {
    return _.uniq(flatten(arguments, true, true, []));
  };

  // Produce an array that contains every item shared between all the
  // passed-in arrays.
  _.intersection = function(array) {
    if (array == null) return [];
    var result = [];
    var argsLength = arguments.length;
    for (var i = 0, length = array.length; i < length; i++) {
      var item = array[i];
      if (_.contains(result, item)) continue;
      for (var j = 1; j < argsLength; j++) {
        if (!_.contains(arguments[j], item)) break;
      }
      if (j === argsLength) result.push(item);
    }
    return result;
  };

  // Take the difference between one array and a number of other arrays.
  // Only the elements present in just the first array will remain.
  _.difference = function(array) {
    var rest = flatten(slice.call(arguments, 1), true, true, []);
    return _.filter(array, function(value){
      return !_.contains(rest, value);
    });
  };

  // Zip together multiple lists into a single array -- elements that share
  // an index go together.
  _.zip = function(array) {
    if (array == null) return [];
    var length = _.max(arguments, 'length').length;
    var results = Array(length);
    for (var i = 0; i < length; i++) {
      results[i] = _.pluck(arguments, i);
    }
    return results;
  };

  // Converts lists into objects. Pass either a single array of `[key, value]`
  // pairs, or two parallel arrays of the same length -- one of keys, and one of
  // the corresponding values.
  _.object = function(list, values) {
    if (list == null) return {};
    var result = {};
    for (var i = 0, length = list.length; i < length; i++) {
      if (values) {
        result[list[i]] = values[i];
      } else {
        result[list[i][0]] = list[i][1];
      }
    }
    return result;
  };

  // Return the position of the first occurrence of an item in an array,
  // or -1 if the item is not included in the array.
  // If the array is large and already in sort order, pass `true`
  // for **isSorted** to use binary search.
  _.indexOf = function(array, item, isSorted) {
    if (array == null) return -1;
    var i = 0, length = array.length;
    if (isSorted) {
      if (typeof isSorted == 'number') {
        i = isSorted < 0 ? Math.max(0, length + isSorted) : isSorted;
      } else {
        i = _.sortedIndex(array, item);
        return array[i] === item ? i : -1;
      }
    }
    for (; i < length; i++) if (array[i] === item) return i;
    return -1;
  };

  _.lastIndexOf = function(array, item, from) {
    if (array == null) return -1;
    var idx = array.length;
    if (typeof from == 'number') {
      idx = from < 0 ? idx + from + 1 : Math.min(idx, from + 1);
    }
    while (--idx >= 0) if (array[idx] === item) return idx;
    return -1;
  };

  // Generate an integer Array containing an arithmetic progression. A port of
  // the native Python `range()` function. See
  // [the Python documentation](http://docs.python.org/library/functions.html#range).
  _.range = function(start, stop, step) {
    if (arguments.length <= 1) {
      stop = start || 0;
      start = 0;
    }
    step = step || 1;

    var length = Math.max(Math.ceil((stop - start) / step), 0);
    var range = Array(length);

    for (var idx = 0; idx < length; idx++, start += step) {
      range[idx] = start;
    }

    return range;
  };

  // Function (ahem) Functions
  // ------------------

  // Reusable constructor function for prototype setting.
  var Ctor = function(){};

  // Create a function bound to a given object (assigning `this`, and arguments,
  // optionally). Delegates to **ECMAScript 5**'s native `Function.bind` if
  // available.
  _.bind = function(func, context) {
    var args, bound;
    if (nativeBind && func.bind === nativeBind) return nativeBind.apply(func, slice.call(arguments, 1));
    if (!_.isFunction(func)) throw new TypeError('Bind must be called on a function');
    args = slice.call(arguments, 2);
    bound = function() {
      if (!(this instanceof bound)) return func.apply(context, args.concat(slice.call(arguments)));
      Ctor.prototype = func.prototype;
      var self = new Ctor;
      Ctor.prototype = null;
      var result = func.apply(self, args.concat(slice.call(arguments)));
      if (_.isObject(result)) return result;
      return self;
    };
    return bound;
  };

  // Partially apply a function by creating a version that has had some of its
  // arguments pre-filled, without changing its dynamic `this` context. _ acts
  // as a placeholder, allowing any combination of arguments to be pre-filled.
  _.partial = function(func) {
    var boundArgs = slice.call(arguments, 1);
    return function() {
      var position = 0;
      var args = boundArgs.slice();
      for (var i = 0, length = args.length; i < length; i++) {
        if (args[i] === _) args[i] = arguments[position++];
      }
      while (position < arguments.length) args.push(arguments[position++]);
      return func.apply(this, args);
    };
  };

  // Bind a number of an object's methods to that object. Remaining arguments
  // are the method names to be bound. Useful for ensuring that all callbacks
  // defined on an object belong to it.
  _.bindAll = function(obj) {
    var i, length = arguments.length, key;
    if (length <= 1) throw new Error('bindAll must be passed function names');
    for (i = 1; i < length; i++) {
      key = arguments[i];
      obj[key] = _.bind(obj[key], obj);
    }
    return obj;
  };

  // Memoize an expensive function by storing its results.
  _.memoize = function(func, hasher) {
    var memoize = function(key) {
      var cache = memoize.cache;
      var address = hasher ? hasher.apply(this, arguments) : key;
      if (!_.has(cache, address)) cache[address] = func.apply(this, arguments);
      return cache[address];
    };
    memoize.cache = {};
    return memoize;
  };

  // Delays a function for the given number of milliseconds, and then calls
  // it with the arguments supplied.
  _.delay = function(func, wait) {
    var args = slice.call(arguments, 2);
    return setTimeout(function(){
      return func.apply(null, args);
    }, wait);
  };

  // Defers a function, scheduling it to run after the current call stack has
  // cleared.
  _.defer = function(func) {
    return _.delay.apply(_, [func, 1].concat(slice.call(arguments, 1)));
  };

  // Returns a function, that, when invoked, will only be triggered at most once
  // during a given window of time. Normally, the throttled function will run
  // as much as it can, without ever going more than once per `wait` duration;
  // but if you'd like to disable the execution on the leading edge, pass
  // `{leading: false}`. To disable execution on the trailing edge, ditto.
  _.throttle = function(func, wait, options) {
    var context, args, result;
    var timeout = null;
    var previous = 0;
    if (!options) options = {};
    var later = function() {
      previous = options.leading === false ? 0 : _.now();
      timeout = null;
      result = func.apply(context, args);
      if (!timeout) context = args = null;
    };
    return function() {
      var now = _.now();
      if (!previous && options.leading === false) previous = now;
      var remaining = wait - (now - previous);
      context = this;
      args = arguments;
      if (remaining <= 0 || remaining > wait) {
        clearTimeout(timeout);
        timeout = null;
        previous = now;
        result = func.apply(context, args);
        if (!timeout) context = args = null;
      } else if (!timeout && options.trailing !== false) {
        timeout = setTimeout(later, remaining);
      }
      return result;
    };
  };

  // Returns a function, that, as long as it continues to be invoked, will not
  // be triggered. The function will be called after it stops being called for
  // N milliseconds. If `immediate` is passed, trigger the function on the
  // leading edge, instead of the trailing.
  _.debounce = function(func, wait, immediate) {
    var timeout, args, context, timestamp, result;

    var later = function() {
      var last = _.now() - timestamp;

      if (last < wait && last > 0) {
        timeout = setTimeout(later, wait - last);
      } else {
        timeout = null;
        if (!immediate) {
          result = func.apply(context, args);
          if (!timeout) context = args = null;
        }
      }
    };

    return function() {
      context = this;
      args = arguments;
      timestamp = _.now();
      var callNow = immediate && !timeout;
      if (!timeout) timeout = setTimeout(later, wait);
      if (callNow) {
        result = func.apply(context, args);
        context = args = null;
      }

      return result;
    };
  };

  // Returns the first function passed as an argument to the second,
  // allowing you to adjust arguments, run code before and after, and
  // conditionally execute the original function.
  _.wrap = function(func, wrapper) {
    return _.partial(wrapper, func);
  };

  // Returns a negated version of the passed-in predicate.
  _.negate = function(predicate) {
    return function() {
      return !predicate.apply(this, arguments);
    };
  };

  // Returns a function that is the composition of a list of functions, each
  // consuming the return value of the function that follows.
  _.compose = function() {
    var args = arguments;
    var start = args.length - 1;
    return function() {
      var i = start;
      var result = args[start].apply(this, arguments);
      while (i--) result = args[i].call(this, result);
      return result;
    };
  };

  // Returns a function that will only be executed after being called N times.
  _.after = function(times, func) {
    return function() {
      if (--times < 1) {
        return func.apply(this, arguments);
      }
    };
  };

  // Returns a function that will only be executed before being called N times.
  _.before = function(times, func) {
    var memo;
    return function() {
      if (--times > 0) {
        memo = func.apply(this, arguments);
      } else {
        func = null;
      }
      return memo;
    };
  };

  // Returns a function that will be executed at most one time, no matter how
  // often you call it. Useful for lazy initialization.
  _.once = _.partial(_.before, 2);

  // Object Functions
  // ----------------

  // Retrieve the names of an object's properties.
  // Delegates to **ECMAScript 5**'s native `Object.keys`
  _.keys = function(obj) {
    if (!_.isObject(obj)) return [];
    if (nativeKeys) return nativeKeys(obj);
    var keys = [];
    for (var key in obj) if (_.has(obj, key)) keys.push(key);
    return keys;
  };

  // Retrieve the values of an object's properties.
  _.values = function(obj) {
    var keys = _.keys(obj);
    var length = keys.length;
    var values = Array(length);
    for (var i = 0; i < length; i++) {
      values[i] = obj[keys[i]];
    }
    return values;
  };

  // Convert an object into a list of `[key, value]` pairs.
  _.pairs = function(obj) {
    var keys = _.keys(obj);
    var length = keys.length;
    var pairs = Array(length);
    for (var i = 0; i < length; i++) {
      pairs[i] = [keys[i], obj[keys[i]]];
    }
    return pairs;
  };

  // Invert the keys and values of an object. The values must be serializable.
  _.invert = function(obj) {
    var result = {};
    var keys = _.keys(obj);
    for (var i = 0, length = keys.length; i < length; i++) {
      result[obj[keys[i]]] = keys[i];
    }
    return result;
  };

  // Return a sorted list of the function names available on the object.
  // Aliased as `methods`
  _.functions = _.methods = function(obj) {
    var names = [];
    for (var key in obj) {
      if (_.isFunction(obj[key])) names.push(key);
    }
    return names.sort();
  };

  // Extend a given object with all the properties in passed-in object(s).
  _.extend = function(obj) {
    if (!_.isObject(obj)) return obj;
    var source, prop;
    for (var i = 1, length = arguments.length; i < length; i++) {
      source = arguments[i];
      for (prop in source) {
        if (hasOwnProperty.call(source, prop)) {
            obj[prop] = source[prop];
        }
      }
    }
    return obj;
  };

  // Return a copy of the object only containing the whitelisted properties.
  _.pick = function(obj, iteratee, context) {
    var result = {}, key;
    if (obj == null) return result;
    if (_.isFunction(iteratee)) {
      iteratee = createCallback(iteratee, context);
      for (key in obj) {
        var value = obj[key];
        if (iteratee(value, key, obj)) result[key] = value;
      }
    } else {
      var keys = concat.apply([], slice.call(arguments, 1));
      obj = new Object(obj);
      for (var i = 0, length = keys.length; i < length; i++) {
        key = keys[i];
        if (key in obj) result[key] = obj[key];
      }
    }
    return result;
  };

   // Return a copy of the object without the blacklisted properties.
  _.omit = function(obj, iteratee, context) {
    if (_.isFunction(iteratee)) {
      iteratee = _.negate(iteratee);
    } else {
      var keys = _.map(concat.apply([], slice.call(arguments, 1)), String);
      iteratee = function(value, key) {
        return !_.contains(keys, key);
      };
    }
    return _.pick(obj, iteratee, context);
  };

  // Fill in a given object with default properties.
  _.defaults = function(obj) {
    if (!_.isObject(obj)) return obj;
    for (var i = 1, length = arguments.length; i < length; i++) {
      var source = arguments[i];
      for (var prop in source) {
        if (obj[prop] === void 0) obj[prop] = source[prop];
      }
    }
    return obj;
  };

  // Create a (shallow-cloned) duplicate of an object.
  _.clone = function(obj) {
    if (!_.isObject(obj)) return obj;
    return _.isArray(obj) ? obj.slice() : _.extend({}, obj);
  };

  // Invokes interceptor with the obj, and then returns obj.
  // The primary purpose of this method is to "tap into" a method chain, in
  // order to perform operations on intermediate results within the chain.
  _.tap = function(obj, interceptor) {
    interceptor(obj);
    return obj;
  };

  // Internal recursive comparison function for `isEqual`.
  var eq = function(a, b, aStack, bStack) {
    // Identical objects are equal. `0 === -0`, but they aren't identical.
    // See the [Harmony `egal` proposal](http://wiki.ecmascript.org/doku.php?id=harmony:egal).
    if (a === b) return a !== 0 || 1 / a === 1 / b;
    // A strict comparison is necessary because `null == undefined`.
    if (a == null || b == null) return a === b;
    // Unwrap any wrapped objects.
    if (a instanceof _) a = a._wrapped;
    if (b instanceof _) b = b._wrapped;
    // Compare `[[Class]]` names.
    var className = toString.call(a);
    if (className !== toString.call(b)) return false;
    switch (className) {
      // Strings, numbers, regular expressions, dates, and booleans are compared by value.
      case '[object RegExp]':
      // RegExps are coerced to strings for comparison (Note: '' + /a/i === '/a/i')
      case '[object String]':
        // Primitives and their corresponding object wrappers are equivalent; thus, `"5"` is
        // equivalent to `new String("5")`.
        return '' + a === '' + b;
      case '[object Number]':
        // `NaN`s are equivalent, but non-reflexive.
        // Object(NaN) is equivalent to NaN
        if (+a !== +a) return +b !== +b;
        // An `egal` comparison is performed for other numeric values.
        return +a === 0 ? 1 / +a === 1 / b : +a === +b;
      case '[object Date]':
      case '[object Boolean]':
        // Coerce dates and booleans to numeric primitive values. Dates are compared by their
        // millisecond representations. Note that invalid dates with millisecond representations
        // of `NaN` are not equivalent.
        return +a === +b;
    }
    if (typeof a != 'object' || typeof b != 'object') return false;
    // Assume equality for cyclic structures. The algorithm for detecting cyclic
    // structures is adapted from ES 5.1 section 15.12.3, abstract operation `JO`.
    var length = aStack.length;
    while (length--) {
      // Linear search. Performance is inversely proportional to the number of
      // unique nested structures.
      if (aStack[length] === a) return bStack[length] === b;
    }
    // Objects with different constructors are not equivalent, but `Object`s
    // from different frames are.
    var aCtor = a.constructor, bCtor = b.constructor;
    if (
      aCtor !== bCtor &&
      // Handle Object.create(x) cases
      'constructor' in a && 'constructor' in b &&
      !(_.isFunction(aCtor) && aCtor instanceof aCtor &&
        _.isFunction(bCtor) && bCtor instanceof bCtor)
    ) {
      return false;
    }
    // Add the first object to the stack of traversed objects.
    aStack.push(a);
    bStack.push(b);
    var size, result;
    // Recursively compare objects and arrays.
    if (className === '[object Array]') {
      // Compare array lengths to determine if a deep comparison is necessary.
      size = a.length;
      result = size === b.length;
      if (result) {
        // Deep compare the contents, ignoring non-numeric properties.
        while (size--) {
          if (!(result = eq(a[size], b[size], aStack, bStack))) break;
        }
      }
    } else {
      // Deep compare objects.
      var keys = _.keys(a), key;
      size = keys.length;
      // Ensure that both objects contain the same number of properties before comparing deep equality.
      result = _.keys(b).length === size;
      if (result) {
        while (size--) {
          // Deep compare each member
          key = keys[size];
          if (!(result = _.has(b, key) && eq(a[key], b[key], aStack, bStack))) break;
        }
      }
    }
    // Remove the first object from the stack of traversed objects.
    aStack.pop();
    bStack.pop();
    return result;
  };

  // Perform a deep comparison to check if two objects are equal.
  _.isEqual = function(a, b) {
    return eq(a, b, [], []);
  };

  // Is a given array, string, or object empty?
  // An "empty" object has no enumerable own-properties.
  _.isEmpty = function(obj) {
    if (obj == null) return true;
    if (_.isArray(obj) || _.isString(obj) || _.isArguments(obj)) return obj.length === 0;
    for (var key in obj) if (_.has(obj, key)) return false;
    return true;
  };

  // Is a given value a DOM element?
  _.isElement = function(obj) {
    return !!(obj && obj.nodeType === 1);
  };

  // Is a given value an array?
  // Delegates to ECMA5's native Array.isArray
  _.isArray = nativeIsArray || function(obj) {
    return toString.call(obj) === '[object Array]';
  };

  // Is a given variable an object?
  _.isObject = function(obj) {
    var type = typeof obj;
    return type === 'function' || type === 'object' && !!obj;
  };

  // Add some isType methods: isArguments, isFunction, isString, isNumber, isDate, isRegExp.
  _.each(['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'], function(name) {
    _['is' + name] = function(obj) {
      return toString.call(obj) === '[object ' + name + ']';
    };
  });

  // Define a fallback version of the method in browsers (ahem, IE), where
  // there isn't any inspectable "Arguments" type.
  if (!_.isArguments(arguments)) {
    _.isArguments = function(obj) {
      return _.has(obj, 'callee');
    };
  }

  // Optimize `isFunction` if appropriate. Work around an IE 11 bug.
  if (typeof /./ !== 'function') {
    _.isFunction = function(obj) {
      return typeof obj == 'function' || false;
    };
  }

  // Is a given object a finite number?
  _.isFinite = function(obj) {
    return isFinite(obj) && !isNaN(parseFloat(obj));
  };

  // Is the given value `NaN`? (NaN is the only number which does not equal itself).
  _.isNaN = function(obj) {
    return _.isNumber(obj) && obj !== +obj;
  };

  // Is a given value a boolean?
  _.isBoolean = function(obj) {
    return obj === true || obj === false || toString.call(obj) === '[object Boolean]';
  };

  // Is a given value equal to null?
  _.isNull = function(obj) {
    return obj === null;
  };

  // Is a given variable undefined?
  _.isUndefined = function(obj) {
    return obj === void 0;
  };

  // Shortcut function for checking if an object has a given property directly
  // on itself (in other words, not on a prototype).
  _.has = function(obj, key) {
    return obj != null && hasOwnProperty.call(obj, key);
  };

  // Utility Functions
  // -----------------

  // Run Underscore.js in *noConflict* mode, returning the `_` variable to its
  // previous owner. Returns a reference to the Underscore object.
  _.noConflict = function() {
    root._ = previousUnderscore;
    return this;
  };

  // Keep the identity function around for default iteratees.
  _.identity = function(value) {
    return value;
  };

  _.constant = function(value) {
    return function() {
      return value;
    };
  };

  _.noop = function(){};

  _.property = function(key) {
    return function(obj) {
      return obj[key];
    };
  };

  // Returns a predicate for checking whether an object has a given set of `key:value` pairs.
  _.matches = function(attrs) {
    var pairs = _.pairs(attrs), length = pairs.length;
    return function(obj) {
      if (obj == null) return !length;
      obj = new Object(obj);
      for (var i = 0; i < length; i++) {
        var pair = pairs[i], key = pair[0];
        if (pair[1] !== obj[key] || !(key in obj)) return false;
      }
      return true;
    };
  };

  // Run a function **n** times.
  _.times = function(n, iteratee, context) {
    var accum = Array(Math.max(0, n));
    iteratee = createCallback(iteratee, context, 1);
    for (var i = 0; i < n; i++) accum[i] = iteratee(i);
    return accum;
  };

  // Return a random integer between min and max (inclusive).
  _.random = function(min, max) {
    if (max == null) {
      max = min;
      min = 0;
    }
    return min + Math.floor(Math.random() * (max - min + 1));
  };

  // A (possibly faster) way to get the current timestamp as an integer.
  _.now = Date.now || function() {
    return new Date().getTime();
  };

   // List of HTML entities for escaping.
  var escapeMap = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;',
    '`': '&#x60;'
  };
  var unescapeMap = _.invert(escapeMap);

  // Functions for escaping and unescaping strings to/from HTML interpolation.
  var createEscaper = function(map) {
    var escaper = function(match) {
      return map[match];
    };
    // Regexes for identifying a key that needs to be escaped
    var source = '(?:' + _.keys(map).join('|') + ')';
    var testRegexp = RegExp(source);
    var replaceRegexp = RegExp(source, 'g');
    return function(string) {
      string = string == null ? '' : '' + string;
      return testRegexp.test(string) ? string.replace(replaceRegexp, escaper) : string;
    };
  };
  _.escape = createEscaper(escapeMap);
  _.unescape = createEscaper(unescapeMap);

  // If the value of the named `property` is a function then invoke it with the
  // `object` as context; otherwise, return it.
  _.result = function(object, property) {
    if (object == null) return void 0;
    var value = object[property];
    return _.isFunction(value) ? object[property]() : value;
  };

  // Generate a unique integer id (unique within the entire client session).
  // Useful for temporary DOM ids.
  var idCounter = 0;
  _.uniqueId = function(prefix) {
    var id = ++idCounter + '';
    return prefix ? prefix + id : id;
  };

  // By default, Underscore uses ERB-style template delimiters, change the
  // following template settings to use alternative delimiters.
  _.templateSettings = {
    evaluate    : /<%([\s\S]+?)%>/g,
    interpolate : /<%=([\s\S]+?)%>/g,
    escape      : /<%-([\s\S]+?)%>/g
  };

  // When customizing `templateSettings`, if you don't want to define an
  // interpolation, evaluation or escaping regex, we need one that is
  // guaranteed not to match.
  var noMatch = /(.)^/;

  // Certain characters need to be escaped so that they can be put into a
  // string literal.
  var escapes = {
    "'":      "'",
    '\\':     '\\',
    '\r':     'r',
    '\n':     'n',
    '\u2028': 'u2028',
    '\u2029': 'u2029'
  };

  var escaper = /\\|'|\r|\n|\u2028|\u2029/g;

  var escapeChar = function(match) {
    return '\\' + escapes[match];
  };

  // JavaScript micro-templating, similar to John Resig's implementation.
  // Underscore templating handles arbitrary delimiters, preserves whitespace,
  // and correctly escapes quotes within interpolated code.
  // NB: `oldSettings` only exists for backwards compatibility.
  _.template = function(text, settings, oldSettings) {
    if (!settings && oldSettings) settings = oldSettings;
    settings = _.defaults({}, settings, _.templateSettings);

    // Combine delimiters into one regular expression via alternation.
    var matcher = RegExp([
      (settings.escape || noMatch).source,
      (settings.interpolate || noMatch).source,
      (settings.evaluate || noMatch).source
    ].join('|') + '|$', 'g');

    // Compile the template source, escaping string literals appropriately.
    var index = 0;
    var source = "__p+='";
    text.replace(matcher, function(match, escape, interpolate, evaluate, offset) {
      source += text.slice(index, offset).replace(escaper, escapeChar);
      index = offset + match.length;

      if (escape) {
        source += "'+\n((__t=(" + escape + "))==null?'':_.escape(__t))+\n'";
      } else if (interpolate) {
        source += "'+\n((__t=(" + interpolate + "))==null?'':__t)+\n'";
      } else if (evaluate) {
        source += "';\n" + evaluate + "\n__p+='";
      }

      // Adobe VMs need the match returned to produce the correct offest.
      return match;
    });
    source += "';\n";

    // If a variable is not specified, place data values in local scope.
    if (!settings.variable) source = 'with(obj||{}){\n' + source + '}\n';

    source = "var __t,__p='',__j=Array.prototype.join," +
      "print=function(){__p+=__j.call(arguments,'');};\n" +
      source + 'return __p;\n';

    try {
      var render = new Function(settings.variable || 'obj', '_', source);
    } catch (e) {
      e.source = source;
      throw e;
    }

    var template = function(data) {
      return render.call(this, data, _);
    };

    // Provide the compiled source as a convenience for precompilation.
    var argument = settings.variable || 'obj';
    template.source = 'function(' + argument + '){\n' + source + '}';

    return template;
  };

  // Add a "chain" function. Start chaining a wrapped Underscore object.
  _.chain = function(obj) {
    var instance = _(obj);
    instance._chain = true;
    return instance;
  };

  // OOP
  // ---------------
  // If Underscore is called as a function, it returns a wrapped object that
  // can be used OO-style. This wrapper holds altered versions of all the
  // underscore functions. Wrapped objects may be chained.

  // Helper function to continue chaining intermediate results.
  var result = function(obj) {
    return this._chain ? _(obj).chain() : obj;
  };

  // Add your own custom functions to the Underscore object.
  _.mixin = function(obj) {
    _.each(_.functions(obj), function(name) {
      var func = _[name] = obj[name];
      _.prototype[name] = function() {
        var args = [this._wrapped];
        push.apply(args, arguments);
        return result.call(this, func.apply(_, args));
      };
    });
  };

  // Add all of the Underscore functions to the wrapper object.
  _.mixin(_);

  // Add all mutator Array functions to the wrapper.
  _.each(['pop', 'push', 'reverse', 'shift', 'sort', 'splice', 'unshift'], function(name) {
    var method = ArrayProto[name];
    _.prototype[name] = function() {
      var obj = this._wrapped;
      method.apply(obj, arguments);
      if ((name === 'shift' || name === 'splice') && obj.length === 0) delete obj[0];
      return result.call(this, obj);
    };
  });

  // Add all accessor Array functions to the wrapper.
  _.each(['concat', 'join', 'slice'], function(name) {
    var method = ArrayProto[name];
    _.prototype[name] = function() {
      return result.call(this, method.apply(this._wrapped, arguments));
    };
  });

  // Extracts the result from a wrapped and chained object.
  _.prototype.value = function() {
    return this._wrapped;
  };

  // AMD registration happens at the end for compatibility with AMD loaders
  // that may not enforce next-turn semantics on modules. Even though general
  // practice for AMD registration is to be anonymous, underscore registers
  // as a named module because, like jQuery, it is a base library that is
  // popular enough to be bundled in a third party lib, but not be part of
  // an AMD load request. Those cases could generate an error when an
  // anonymous define() is called outside of a loader request.
  if (typeof define === 'function' && define.amd) {
    define('underscore', [], function() {
      return _;
    });
  }
}.call(this));

},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\jquery\\dist\\jquery.js":[function(require,module,exports){
/*!
 * jQuery JavaScript Library v2.1.1
 * http://jquery.com/
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 *
 * Copyright 2005, 2014 jQuery Foundation, Inc. and other contributors
 * Released under the MIT license
 * http://jquery.org/license
 *
 * Date: 2014-05-01T17:11Z
 */

(function( global, factory ) {

	if ( typeof module === "object" && typeof module.exports === "object" ) {
		// For CommonJS and CommonJS-like environments where a proper window is present,
		// execute the factory and get jQuery
		// For environments that do not inherently posses a window with a document
		// (such as Node.js), expose a jQuery-making factory as module.exports
		// This accentuates the need for the creation of a real window
		// e.g. var jQuery = require("jquery")(window);
		// See ticket #14549 for more info
		module.exports = global.document ?
			factory( global, true ) :
			function( w ) {
				if ( !w.document ) {
					throw new Error( "jQuery requires a window with a document" );
				}
				return factory( w );
			};
	} else {
		factory( global );
	}

// Pass this if window is not defined yet
}(typeof window !== "undefined" ? window : this, function( window, noGlobal ) {

// Can't do this because several apps including ASP.NET trace
// the stack via arguments.caller.callee and Firefox dies if
// you try to trace through "use strict" call chains. (#13335)
// Support: Firefox 18+
//

var arr = [];

var slice = arr.slice;

var concat = arr.concat;

var push = arr.push;

var indexOf = arr.indexOf;

var class2type = {};

var toString = class2type.toString;

var hasOwn = class2type.hasOwnProperty;

var support = {};



var
	// Use the correct document accordingly with window argument (sandbox)
	document = window.document,

	version = "2.1.1",

	// Define a local copy of jQuery
	jQuery = function( selector, context ) {
		// The jQuery object is actually just the init constructor 'enhanced'
		// Need init if jQuery is called (just allow error to be thrown if not included)
		return new jQuery.fn.init( selector, context );
	},

	// Support: Android<4.1
	// Make sure we trim BOM and NBSP
	rtrim = /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g,

	// Matches dashed string for camelizing
	rmsPrefix = /^-ms-/,
	rdashAlpha = /-([\da-z])/gi,

	// Used by jQuery.camelCase as callback to replace()
	fcamelCase = function( all, letter ) {
		return letter.toUpperCase();
	};

jQuery.fn = jQuery.prototype = {
	// The current version of jQuery being used
	jquery: version,

	constructor: jQuery,

	// Start with an empty selector
	selector: "",

	// The default length of a jQuery object is 0
	length: 0,

	toArray: function() {
		return slice.call( this );
	},

	// Get the Nth element in the matched element set OR
	// Get the whole matched element set as a clean array
	get: function( num ) {
		return num != null ?

			// Return just the one element from the set
			( num < 0 ? this[ num + this.length ] : this[ num ] ) :

			// Return all the elements in a clean array
			slice.call( this );
	},

	// Take an array of elements and push it onto the stack
	// (returning the new matched element set)
	pushStack: function( elems ) {

		// Build a new jQuery matched element set
		var ret = jQuery.merge( this.constructor(), elems );

		// Add the old object onto the stack (as a reference)
		ret.prevObject = this;
		ret.context = this.context;

		// Return the newly-formed element set
		return ret;
	},

	// Execute a callback for every element in the matched set.
	// (You can seed the arguments with an array of args, but this is
	// only used internally.)
	each: function( callback, args ) {
		return jQuery.each( this, callback, args );
	},

	map: function( callback ) {
		return this.pushStack( jQuery.map(this, function( elem, i ) {
			return callback.call( elem, i, elem );
		}));
	},

	slice: function() {
		return this.pushStack( slice.apply( this, arguments ) );
	},

	first: function() {
		return this.eq( 0 );
	},

	last: function() {
		return this.eq( -1 );
	},

	eq: function( i ) {
		var len = this.length,
			j = +i + ( i < 0 ? len : 0 );
		return this.pushStack( j >= 0 && j < len ? [ this[j] ] : [] );
	},

	end: function() {
		return this.prevObject || this.constructor(null);
	},

	// For internal use only.
	// Behaves like an Array's method, not like a jQuery method.
	push: push,
	sort: arr.sort,
	splice: arr.splice
};

jQuery.extend = jQuery.fn.extend = function() {
	var options, name, src, copy, copyIsArray, clone,
		target = arguments[0] || {},
		i = 1,
		length = arguments.length,
		deep = false;

	// Handle a deep copy situation
	if ( typeof target === "boolean" ) {
		deep = target;

		// skip the boolean and the target
		target = arguments[ i ] || {};
		i++;
	}

	// Handle case when target is a string or something (possible in deep copy)
	if ( typeof target !== "object" && !jQuery.isFunction(target) ) {
		target = {};
	}

	// extend jQuery itself if only one argument is passed
	if ( i === length ) {
		target = this;
		i--;
	}

	for ( ; i < length; i++ ) {
		// Only deal with non-null/undefined values
		if ( (options = arguments[ i ]) != null ) {
			// Extend the base object
			for ( name in options ) {
				src = target[ name ];
				copy = options[ name ];

				// Prevent never-ending loop
				if ( target === copy ) {
					continue;
				}

				// Recurse if we're merging plain objects or arrays
				if ( deep && copy && ( jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)) ) ) {
					if ( copyIsArray ) {
						copyIsArray = false;
						clone = src && jQuery.isArray(src) ? src : [];

					} else {
						clone = src && jQuery.isPlainObject(src) ? src : {};
					}

					// Never move original objects, clone them
					target[ name ] = jQuery.extend( deep, clone, copy );

				// Don't bring in undefined values
				} else if ( copy !== undefined ) {
					target[ name ] = copy;
				}
			}
		}
	}

	// Return the modified object
	return target;
};

jQuery.extend({
	// Unique for each copy of jQuery on the page
	expando: "jQuery" + ( version + Math.random() ).replace( /\D/g, "" ),

	// Assume jQuery is ready without the ready module
	isReady: true,

	error: function( msg ) {
		throw new Error( msg );
	},

	noop: function() {},

	// See test/unit/core.js for details concerning isFunction.
	// Since version 1.3, DOM methods and functions like alert
	// aren't supported. They return false on IE (#2968).
	isFunction: function( obj ) {
		return jQuery.type(obj) === "function";
	},

	isArray: Array.isArray,

	isWindow: function( obj ) {
		return obj != null && obj === obj.window;
	},

	isNumeric: function( obj ) {
		// parseFloat NaNs numeric-cast false positives (null|true|false|"")
		// ...but misinterprets leading-number strings, particularly hex literals ("0x...")
		// subtraction forces infinities to NaN
		return !jQuery.isArray( obj ) && obj - parseFloat( obj ) >= 0;
	},

	isPlainObject: function( obj ) {
		// Not plain objects:
		// - Any object or value whose internal [[Class]] property is not "[object Object]"
		// - DOM nodes
		// - window
		if ( jQuery.type( obj ) !== "object" || obj.nodeType || jQuery.isWindow( obj ) ) {
			return false;
		}

		if ( obj.constructor &&
				!hasOwn.call( obj.constructor.prototype, "isPrototypeOf" ) ) {
			return false;
		}

		// If the function hasn't returned already, we're confident that
		// |obj| is a plain object, created by {} or constructed with new Object
		return true;
	},

	isEmptyObject: function( obj ) {
		var name;
		for ( name in obj ) {
			return false;
		}
		return true;
	},

	type: function( obj ) {
		if ( obj == null ) {
			return obj + "";
		}
		// Support: Android < 4.0, iOS < 6 (functionish RegExp)
		return typeof obj === "object" || typeof obj === "function" ?
			class2type[ toString.call(obj) ] || "object" :
			typeof obj;
	},

	// Evaluates a script in a global context
	globalEval: function( code ) {
		var script,
			indirect = eval;

		code = jQuery.trim( code );

		if ( code ) {
			// If the code includes a valid, prologue position
			// strict mode pragma, execute code by injecting a
			// script tag into the document.
			if ( code.indexOf("use strict") === 1 ) {
				script = document.createElement("script");
				script.text = code;
				document.head.appendChild( script ).parentNode.removeChild( script );
			} else {
			// Otherwise, avoid the DOM node creation, insertion
			// and removal by using an indirect global eval
				indirect( code );
			}
		}
	},

	// Convert dashed to camelCase; used by the css and data modules
	// Microsoft forgot to hump their vendor prefix (#9572)
	camelCase: function( string ) {
		return string.replace( rmsPrefix, "ms-" ).replace( rdashAlpha, fcamelCase );
	},

	nodeName: function( elem, name ) {
		return elem.nodeName && elem.nodeName.toLowerCase() === name.toLowerCase();
	},

	// args is for internal usage only
	each: function( obj, callback, args ) {
		var value,
			i = 0,
			length = obj.length,
			isArray = isArraylike( obj );

		if ( args ) {
			if ( isArray ) {
				for ( ; i < length; i++ ) {
					value = callback.apply( obj[ i ], args );

					if ( value === false ) {
						break;
					}
				}
			} else {
				for ( i in obj ) {
					value = callback.apply( obj[ i ], args );

					if ( value === false ) {
						break;
					}
				}
			}

		// A special, fast, case for the most common use of each
		} else {
			if ( isArray ) {
				for ( ; i < length; i++ ) {
					value = callback.call( obj[ i ], i, obj[ i ] );

					if ( value === false ) {
						break;
					}
				}
			} else {
				for ( i in obj ) {
					value = callback.call( obj[ i ], i, obj[ i ] );

					if ( value === false ) {
						break;
					}
				}
			}
		}

		return obj;
	},

	// Support: Android<4.1
	trim: function( text ) {
		return text == null ?
			"" :
			( text + "" ).replace( rtrim, "" );
	},

	// results is for internal usage only
	makeArray: function( arr, results ) {
		var ret = results || [];

		if ( arr != null ) {
			if ( isArraylike( Object(arr) ) ) {
				jQuery.merge( ret,
					typeof arr === "string" ?
					[ arr ] : arr
				);
			} else {
				push.call( ret, arr );
			}
		}

		return ret;
	},

	inArray: function( elem, arr, i ) {
		return arr == null ? -1 : indexOf.call( arr, elem, i );
	},

	merge: function( first, second ) {
		var len = +second.length,
			j = 0,
			i = first.length;

		for ( ; j < len; j++ ) {
			first[ i++ ] = second[ j ];
		}

		first.length = i;

		return first;
	},

	grep: function( elems, callback, invert ) {
		var callbackInverse,
			matches = [],
			i = 0,
			length = elems.length,
			callbackExpect = !invert;

		// Go through the array, only saving the items
		// that pass the validator function
		for ( ; i < length; i++ ) {
			callbackInverse = !callback( elems[ i ], i );
			if ( callbackInverse !== callbackExpect ) {
				matches.push( elems[ i ] );
			}
		}

		return matches;
	},

	// arg is for internal usage only
	map: function( elems, callback, arg ) {
		var value,
			i = 0,
			length = elems.length,
			isArray = isArraylike( elems ),
			ret = [];

		// Go through the array, translating each of the items to their new values
		if ( isArray ) {
			for ( ; i < length; i++ ) {
				value = callback( elems[ i ], i, arg );

				if ( value != null ) {
					ret.push( value );
				}
			}

		// Go through every key on the object,
		} else {
			for ( i in elems ) {
				value = callback( elems[ i ], i, arg );

				if ( value != null ) {
					ret.push( value );
				}
			}
		}

		// Flatten any nested arrays
		return concat.apply( [], ret );
	},

	// A global GUID counter for objects
	guid: 1,

	// Bind a function to a context, optionally partially applying any
	// arguments.
	proxy: function( fn, context ) {
		var tmp, args, proxy;

		if ( typeof context === "string" ) {
			tmp = fn[ context ];
			context = fn;
			fn = tmp;
		}

		// Quick check to determine if target is callable, in the spec
		// this throws a TypeError, but we will just return undefined.
		if ( !jQuery.isFunction( fn ) ) {
			return undefined;
		}

		// Simulated bind
		args = slice.call( arguments, 2 );
		proxy = function() {
			return fn.apply( context || this, args.concat( slice.call( arguments ) ) );
		};

		// Set the guid of unique handler to the same of original handler, so it can be removed
		proxy.guid = fn.guid = fn.guid || jQuery.guid++;

		return proxy;
	},

	now: Date.now,

	// jQuery.support is not used in Core but other projects attach their
	// properties to it so it needs to exist.
	support: support
});

// Populate the class2type map
jQuery.each("Boolean Number String Function Array Date RegExp Object Error".split(" "), function(i, name) {
	class2type[ "[object " + name + "]" ] = name.toLowerCase();
});

function isArraylike( obj ) {
	var length = obj.length,
		type = jQuery.type( obj );

	if ( type === "function" || jQuery.isWindow( obj ) ) {
		return false;
	}

	if ( obj.nodeType === 1 && length ) {
		return true;
	}

	return type === "array" || length === 0 ||
		typeof length === "number" && length > 0 && ( length - 1 ) in obj;
}
var Sizzle =
/*!
 * Sizzle CSS Selector Engine v1.10.19
 * http://sizzlejs.com/
 *
 * Copyright 2013 jQuery Foundation, Inc. and other contributors
 * Released under the MIT license
 * http://jquery.org/license
 *
 * Date: 2014-04-18
 */
(function( window ) {

var i,
	support,
	Expr,
	getText,
	isXML,
	tokenize,
	compile,
	select,
	outermostContext,
	sortInput,
	hasDuplicate,

	// Local document vars
	setDocument,
	document,
	docElem,
	documentIsHTML,
	rbuggyQSA,
	rbuggyMatches,
	matches,
	contains,

	// Instance-specific data
	expando = "sizzle" + -(new Date()),
	preferredDoc = window.document,
	dirruns = 0,
	done = 0,
	classCache = createCache(),
	tokenCache = createCache(),
	compilerCache = createCache(),
	sortOrder = function( a, b ) {
		if ( a === b ) {
			hasDuplicate = true;
		}
		return 0;
	},

	// General-purpose constants
	strundefined = typeof undefined,
	MAX_NEGATIVE = 1 << 31,

	// Instance methods
	hasOwn = ({}).hasOwnProperty,
	arr = [],
	pop = arr.pop,
	push_native = arr.push,
	push = arr.push,
	slice = arr.slice,
	// Use a stripped-down indexOf if we can't use a native one
	indexOf = arr.indexOf || function( elem ) {
		var i = 0,
			len = this.length;
		for ( ; i < len; i++ ) {
			if ( this[i] === elem ) {
				return i;
			}
		}
		return -1;
	},

	booleans = "checked|selected|async|autofocus|autoplay|controls|defer|disabled|hidden|ismap|loop|multiple|open|readonly|required|scoped",

	// Regular expressions

	// Whitespace characters http://www.w3.org/TR/css3-selectors/#whitespace
	whitespace = "[\\x20\\t\\r\\n\\f]",
	// http://www.w3.org/TR/css3-syntax/#characters
	characterEncoding = "(?:\\\\.|[\\w-]|[^\\x00-\\xa0])+",

	// Loosely modeled on CSS identifier characters
	// An unquoted value should be a CSS identifier http://www.w3.org/TR/css3-selectors/#attribute-selectors
	// Proper syntax: http://www.w3.org/TR/CSS21/syndata.html#value-def-identifier
	identifier = characterEncoding.replace( "w", "w#" ),

	// Attribute selectors: http://www.w3.org/TR/selectors/#attribute-selectors
	attributes = "\\[" + whitespace + "*(" + characterEncoding + ")(?:" + whitespace +
		// Operator (capture 2)
		"*([*^$|!~]?=)" + whitespace +
		// "Attribute values must be CSS identifiers [capture 5] or strings [capture 3 or capture 4]"
		"*(?:'((?:\\\\.|[^\\\\'])*)'|\"((?:\\\\.|[^\\\\\"])*)\"|(" + identifier + "))|)" + whitespace +
		"*\\]",

	pseudos = ":(" + characterEncoding + ")(?:\\((" +
		// To reduce the number of selectors needing tokenize in the preFilter, prefer arguments:
		// 1. quoted (capture 3; capture 4 or capture 5)
		"('((?:\\\\.|[^\\\\'])*)'|\"((?:\\\\.|[^\\\\\"])*)\")|" +
		// 2. simple (capture 6)
		"((?:\\\\.|[^\\\\()[\\]]|" + attributes + ")*)|" +
		// 3. anything else (capture 2)
		".*" +
		")\\)|)",

	// Leading and non-escaped trailing whitespace, capturing some non-whitespace characters preceding the latter
	rtrim = new RegExp( "^" + whitespace + "+|((?:^|[^\\\\])(?:\\\\.)*)" + whitespace + "+$", "g" ),

	rcomma = new RegExp( "^" + whitespace + "*," + whitespace + "*" ),
	rcombinators = new RegExp( "^" + whitespace + "*([>+~]|" + whitespace + ")" + whitespace + "*" ),

	rattributeQuotes = new RegExp( "=" + whitespace + "*([^\\]'\"]*?)" + whitespace + "*\\]", "g" ),

	rpseudo = new RegExp( pseudos ),
	ridentifier = new RegExp( "^" + identifier + "$" ),

	matchExpr = {
		"ID": new RegExp( "^#(" + characterEncoding + ")" ),
		"CLASS": new RegExp( "^\\.(" + characterEncoding + ")" ),
		"TAG": new RegExp( "^(" + characterEncoding.replace( "w", "w*" ) + ")" ),
		"ATTR": new RegExp( "^" + attributes ),
		"PSEUDO": new RegExp( "^" + pseudos ),
		"CHILD": new RegExp( "^:(only|first|last|nth|nth-last)-(child|of-type)(?:\\(" + whitespace +
			"*(even|odd|(([+-]|)(\\d*)n|)" + whitespace + "*(?:([+-]|)" + whitespace +
			"*(\\d+)|))" + whitespace + "*\\)|)", "i" ),
		"bool": new RegExp( "^(?:" + booleans + ")$", "i" ),
		// For use in libraries implementing .is()
		// We use this for POS matching in `select`
		"needsContext": new RegExp( "^" + whitespace + "*[>+~]|:(even|odd|eq|gt|lt|nth|first|last)(?:\\(" +
			whitespace + "*((?:-\\d)?\\d*)" + whitespace + "*\\)|)(?=[^-]|$)", "i" )
	},

	rinputs = /^(?:input|select|textarea|button)$/i,
	rheader = /^h\d$/i,

	rnative = /^[^{]+\{\s*\[native \w/,

	// Easily-parseable/retrievable ID or TAG or CLASS selectors
	rquickExpr = /^(?:#([\w-]+)|(\w+)|\.([\w-]+))$/,

	rsibling = /[+~]/,
	rescape = /'|\\/g,

	// CSS escapes http://www.w3.org/TR/CSS21/syndata.html#escaped-characters
	runescape = new RegExp( "\\\\([\\da-f]{1,6}" + whitespace + "?|(" + whitespace + ")|.)", "ig" ),
	funescape = function( _, escaped, escapedWhitespace ) {
		var high = "0x" + escaped - 0x10000;
		// NaN means non-codepoint
		// Support: Firefox<24
		// Workaround erroneous numeric interpretation of +"0x"
		return high !== high || escapedWhitespace ?
			escaped :
			high < 0 ?
				// BMP codepoint
				String.fromCharCode( high + 0x10000 ) :
				// Supplemental Plane codepoint (surrogate pair)
				String.fromCharCode( high >> 10 | 0xD800, high & 0x3FF | 0xDC00 );
	};

// Optimize for push.apply( _, NodeList )
try {
	push.apply(
		(arr = slice.call( preferredDoc.childNodes )),
		preferredDoc.childNodes
	);
	// Support: Android<4.0
	// Detect silently failing push.apply
	arr[ preferredDoc.childNodes.length ].nodeType;
} catch ( e ) {
	push = { apply: arr.length ?

		// Leverage slice if possible
		function( target, els ) {
			push_native.apply( target, slice.call(els) );
		} :

		// Support: IE<9
		// Otherwise append directly
		function( target, els ) {
			var j = target.length,
				i = 0;
			// Can't trust NodeList.length
			while ( (target[j++] = els[i++]) ) {}
			target.length = j - 1;
		}
	};
}

function Sizzle( selector, context, results, seed ) {
	var match, elem, m, nodeType,
		// QSA vars
		i, groups, old, nid, newContext, newSelector;

	if ( ( context ? context.ownerDocument || context : preferredDoc ) !== document ) {
		setDocument( context );
	}

	context = context || document;
	results = results || [];

	if ( !selector || typeof selector !== "string" ) {
		return results;
	}

	if ( (nodeType = context.nodeType) !== 1 && nodeType !== 9 ) {
		return [];
	}

	if ( documentIsHTML && !seed ) {

		// Shortcuts
		if ( (match = rquickExpr.exec( selector )) ) {
			// Speed-up: Sizzle("#ID")
			if ( (m = match[1]) ) {
				if ( nodeType === 9 ) {
					elem = context.getElementById( m );
					// Check parentNode to catch when Blackberry 4.6 returns
					// nodes that are no longer in the document (jQuery #6963)
					if ( elem && elem.parentNode ) {
						// Handle the case where IE, Opera, and Webkit return items
						// by name instead of ID
						if ( elem.id === m ) {
							results.push( elem );
							return results;
						}
					} else {
						return results;
					}
				} else {
					// Context is not a document
					if ( context.ownerDocument && (elem = context.ownerDocument.getElementById( m )) &&
						contains( context, elem ) && elem.id === m ) {
						results.push( elem );
						return results;
					}
				}

			// Speed-up: Sizzle("TAG")
			} else if ( match[2] ) {
				push.apply( results, context.getElementsByTagName( selector ) );
				return results;

			// Speed-up: Sizzle(".CLASS")
			} else if ( (m = match[3]) && support.getElementsByClassName && context.getElementsByClassName ) {
				push.apply( results, context.getElementsByClassName( m ) );
				return results;
			}
		}

		// QSA path
		if ( support.qsa && (!rbuggyQSA || !rbuggyQSA.test( selector )) ) {
			nid = old = expando;
			newContext = context;
			newSelector = nodeType === 9 && selector;

			// qSA works strangely on Element-rooted queries
			// We can work around this by specifying an extra ID on the root
			// and working up from there (Thanks to Andrew Dupont for the technique)
			// IE 8 doesn't work on object elements
			if ( nodeType === 1 && context.nodeName.toLowerCase() !== "object" ) {
				groups = tokenize( selector );

				if ( (old = context.getAttribute("id")) ) {
					nid = old.replace( rescape, "\\$&" );
				} else {
					context.setAttribute( "id", nid );
				}
				nid = "[id='" + nid + "'] ";

				i = groups.length;
				while ( i-- ) {
					groups[i] = nid + toSelector( groups[i] );
				}
				newContext = rsibling.test( selector ) && testContext( context.parentNode ) || context;
				newSelector = groups.join(",");
			}

			if ( newSelector ) {
				try {
					push.apply( results,
						newContext.querySelectorAll( newSelector )
					);
					return results;
				} catch(qsaError) {
				} finally {
					if ( !old ) {
						context.removeAttribute("id");
					}
				}
			}
		}
	}

	// All others
	return select( selector.replace( rtrim, "$1" ), context, results, seed );
}

/**
 * Create key-value caches of limited size
 * @returns {Function(string, Object)} Returns the Object data after storing it on itself with
 *	property name the (space-suffixed) string and (if the cache is larger than Expr.cacheLength)
 *	deleting the oldest entry
 */
function createCache() {
	var keys = [];

	function cache( key, value ) {
		// Use (key + " ") to avoid collision with native prototype properties (see Issue #157)
		if ( keys.push( key + " " ) > Expr.cacheLength ) {
			// Only keep the most recent entries
			delete cache[ keys.shift() ];
		}
		return (cache[ key + " " ] = value);
	}
	return cache;
}

/**
 * Mark a function for special use by Sizzle
 * @param {Function} fn The function to mark
 */
function markFunction( fn ) {
	fn[ expando ] = true;
	return fn;
}

/**
 * Support testing using an element
 * @param {Function} fn Passed the created div and expects a boolean result
 */
function assert( fn ) {
	var div = document.createElement("div");

	try {
		return !!fn( div );
	} catch (e) {
		return false;
	} finally {
		// Remove from its parent by default
		if ( div.parentNode ) {
			div.parentNode.removeChild( div );
		}
		// release memory in IE
		div = null;
	}
}

/**
 * Adds the same handler for all of the specified attrs
 * @param {String} attrs Pipe-separated list of attributes
 * @param {Function} handler The method that will be applied
 */
function addHandle( attrs, handler ) {
	var arr = attrs.split("|"),
		i = attrs.length;

	while ( i-- ) {
		Expr.attrHandle[ arr[i] ] = handler;
	}
}

/**
 * Checks document order of two siblings
 * @param {Element} a
 * @param {Element} b
 * @returns {Number} Returns less than 0 if a precedes b, greater than 0 if a follows b
 */
function siblingCheck( a, b ) {
	var cur = b && a,
		diff = cur && a.nodeType === 1 && b.nodeType === 1 &&
			( ~b.sourceIndex || MAX_NEGATIVE ) -
			( ~a.sourceIndex || MAX_NEGATIVE );

	// Use IE sourceIndex if available on both nodes
	if ( diff ) {
		return diff;
	}

	// Check if b follows a
	if ( cur ) {
		while ( (cur = cur.nextSibling) ) {
			if ( cur === b ) {
				return -1;
			}
		}
	}

	return a ? 1 : -1;
}

/**
 * Returns a function to use in pseudos for input types
 * @param {String} type
 */
function createInputPseudo( type ) {
	return function( elem ) {
		var name = elem.nodeName.toLowerCase();
		return name === "input" && elem.type === type;
	};
}

/**
 * Returns a function to use in pseudos for buttons
 * @param {String} type
 */
function createButtonPseudo( type ) {
	return function( elem ) {
		var name = elem.nodeName.toLowerCase();
		return (name === "input" || name === "button") && elem.type === type;
	};
}

/**
 * Returns a function to use in pseudos for positionals
 * @param {Function} fn
 */
function createPositionalPseudo( fn ) {
	return markFunction(function( argument ) {
		argument = +argument;
		return markFunction(function( seed, matches ) {
			var j,
				matchIndexes = fn( [], seed.length, argument ),
				i = matchIndexes.length;

			// Match elements found at the specified indexes
			while ( i-- ) {
				if ( seed[ (j = matchIndexes[i]) ] ) {
					seed[j] = !(matches[j] = seed[j]);
				}
			}
		});
	});
}

/**
 * Checks a node for validity as a Sizzle context
 * @param {Element|Object=} context
 * @returns {Element|Object|Boolean} The input node if acceptable, otherwise a falsy value
 */
function testContext( context ) {
	return context && typeof context.getElementsByTagName !== strundefined && context;
}

// Expose support vars for convenience
support = Sizzle.support = {};

/**
 * Detects XML nodes
 * @param {Element|Object} elem An element or a document
 * @returns {Boolean} True iff elem is a non-HTML XML node
 */
isXML = Sizzle.isXML = function( elem ) {
	// documentElement is verified for cases where it doesn't yet exist
	// (such as loading iframes in IE - #4833)
	var documentElement = elem && (elem.ownerDocument || elem).documentElement;
	return documentElement ? documentElement.nodeName !== "HTML" : false;
};

/**
 * Sets document-related variables once based on the current document
 * @param {Element|Object} [doc] An element or document object to use to set the document
 * @returns {Object} Returns the current document
 */
setDocument = Sizzle.setDocument = function( node ) {
	var hasCompare,
		doc = node ? node.ownerDocument || node : preferredDoc,
		parent = doc.defaultView;

	// If no document and documentElement is available, return
	if ( doc === document || doc.nodeType !== 9 || !doc.documentElement ) {
		return document;
	}

	// Set our document
	document = doc;
	docElem = doc.documentElement;

	// Support tests
	documentIsHTML = !isXML( doc );

	// Support: IE>8
	// If iframe document is assigned to "document" variable and if iframe has been reloaded,
	// IE will throw "permission denied" error when accessing "document" variable, see jQuery #13936
	// IE6-8 do not support the defaultView property so parent will be undefined
	if ( parent && parent !== parent.top ) {
		// IE11 does not have attachEvent, so all must suffer
		if ( parent.addEventListener ) {
			parent.addEventListener( "unload", function() {
				setDocument();
			}, false );
		} else if ( parent.attachEvent ) {
			parent.attachEvent( "onunload", function() {
				setDocument();
			});
		}
	}

	/* Attributes
	---------------------------------------------------------------------- */

	// Support: IE<8
	// Verify that getAttribute really returns attributes and not properties (excepting IE8 booleans)
	support.attributes = assert(function( div ) {
		div.className = "i";
		return !div.getAttribute("className");
	});

	/* getElement(s)By*
	---------------------------------------------------------------------- */

	// Check if getElementsByTagName("*") returns only elements
	support.getElementsByTagName = assert(function( div ) {
		div.appendChild( doc.createComment("") );
		return !div.getElementsByTagName("*").length;
	});

	// Check if getElementsByClassName can be trusted
	support.getElementsByClassName = rnative.test( doc.getElementsByClassName ) && assert(function( div ) {
		div.innerHTML = "<div class='a'></div><div class='a i'></div>";

		// Support: Safari<4
		// Catch class over-caching
		div.firstChild.className = "i";
		// Support: Opera<10
		// Catch gEBCN failure to find non-leading classes
		return div.getElementsByClassName("i").length === 2;
	});

	// Support: IE<10
	// Check if getElementById returns elements by name
	// The broken getElementById methods don't pick up programatically-set names,
	// so use a roundabout getElementsByName test
	support.getById = assert(function( div ) {
		docElem.appendChild( div ).id = expando;
		return !doc.getElementsByName || !doc.getElementsByName( expando ).length;
	});

	// ID find and filter
	if ( support.getById ) {
		Expr.find["ID"] = function( id, context ) {
			if ( typeof context.getElementById !== strundefined && documentIsHTML ) {
				var m = context.getElementById( id );
				// Check parentNode to catch when Blackberry 4.6 returns
				// nodes that are no longer in the document #6963
				return m && m.parentNode ? [ m ] : [];
			}
		};
		Expr.filter["ID"] = function( id ) {
			var attrId = id.replace( runescape, funescape );
			return function( elem ) {
				return elem.getAttribute("id") === attrId;
			};
		};
	} else {
		// Support: IE6/7
		// getElementById is not reliable as a find shortcut
		delete Expr.find["ID"];

		Expr.filter["ID"] =  function( id ) {
			var attrId = id.replace( runescape, funescape );
			return function( elem ) {
				var node = typeof elem.getAttributeNode !== strundefined && elem.getAttributeNode("id");
				return node && node.value === attrId;
			};
		};
	}

	// Tag
	Expr.find["TAG"] = support.getElementsByTagName ?
		function( tag, context ) {
			if ( typeof context.getElementsByTagName !== strundefined ) {
				return context.getElementsByTagName( tag );
			}
		} :
		function( tag, context ) {
			var elem,
				tmp = [],
				i = 0,
				results = context.getElementsByTagName( tag );

			// Filter out possible comments
			if ( tag === "*" ) {
				while ( (elem = results[i++]) ) {
					if ( elem.nodeType === 1 ) {
						tmp.push( elem );
					}
				}

				return tmp;
			}
			return results;
		};

	// Class
	Expr.find["CLASS"] = support.getElementsByClassName && function( className, context ) {
		if ( typeof context.getElementsByClassName !== strundefined && documentIsHTML ) {
			return context.getElementsByClassName( className );
		}
	};

	/* QSA/matchesSelector
	---------------------------------------------------------------------- */

	// QSA and matchesSelector support

	// matchesSelector(:active) reports false when true (IE9/Opera 11.5)
	rbuggyMatches = [];

	// qSa(:focus) reports false when true (Chrome 21)
	// We allow this because of a bug in IE8/9 that throws an error
	// whenever `document.activeElement` is accessed on an iframe
	// So, we allow :focus to pass through QSA all the time to avoid the IE error
	// See http://bugs.jquery.com/ticket/13378
	rbuggyQSA = [];

	if ( (support.qsa = rnative.test( doc.querySelectorAll )) ) {
		// Build QSA regex
		// Regex strategy adopted from Diego Perini
		assert(function( div ) {
			// Select is set to empty string on purpose
			// This is to test IE's treatment of not explicitly
			// setting a boolean content attribute,
			// since its presence should be enough
			// http://bugs.jquery.com/ticket/12359
			div.innerHTML = "<select msallowclip=''><option selected=''></option></select>";

			// Support: IE8, Opera 11-12.16
			// Nothing should be selected when empty strings follow ^= or $= or *=
			// The test attribute must be unknown in Opera but "safe" for WinRT
			// http://msdn.microsoft.com/en-us/library/ie/hh465388.aspx#attribute_section
			if ( div.querySelectorAll("[msallowclip^='']").length ) {
				rbuggyQSA.push( "[*^$]=" + whitespace + "*(?:''|\"\")" );
			}

			// Support: IE8
			// Boolean attributes and "value" are not treated correctly
			if ( !div.querySelectorAll("[selected]").length ) {
				rbuggyQSA.push( "\\[" + whitespace + "*(?:value|" + booleans + ")" );
			}

			// Webkit/Opera - :checked should return selected option elements
			// http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#checked
			// IE8 throws error here and will not see later tests
			if ( !div.querySelectorAll(":checked").length ) {
				rbuggyQSA.push(":checked");
			}
		});

		assert(function( div ) {
			// Support: Windows 8 Native Apps
			// The type and name attributes are restricted during .innerHTML assignment
			var input = doc.createElement("input");
			input.setAttribute( "type", "hidden" );
			div.appendChild( input ).setAttribute( "name", "D" );

			// Support: IE8
			// Enforce case-sensitivity of name attribute
			if ( div.querySelectorAll("[name=d]").length ) {
				rbuggyQSA.push( "name" + whitespace + "*[*^$|!~]?=" );
			}

			// FF 3.5 - :enabled/:disabled and hidden elements (hidden elements are still enabled)
			// IE8 throws error here and will not see later tests
			if ( !div.querySelectorAll(":enabled").length ) {
				rbuggyQSA.push( ":enabled", ":disabled" );
			}

			// Opera 10-11 does not throw on post-comma invalid pseudos
			div.querySelectorAll("*,:x");
			rbuggyQSA.push(",.*:");
		});
	}

	if ( (support.matchesSelector = rnative.test( (matches = docElem.matches ||
		docElem.webkitMatchesSelector ||
		docElem.mozMatchesSelector ||
		docElem.oMatchesSelector ||
		docElem.msMatchesSelector) )) ) {

		assert(function( div ) {
			// Check to see if it's possible to do matchesSelector
			// on a disconnected node (IE 9)
			support.disconnectedMatch = matches.call( div, "div" );

			// This should fail with an exception
			// Gecko does not error, returns false instead
			matches.call( div, "[s!='']:x" );
			rbuggyMatches.push( "!=", pseudos );
		});
	}

	rbuggyQSA = rbuggyQSA.length && new RegExp( rbuggyQSA.join("|") );
	rbuggyMatches = rbuggyMatches.length && new RegExp( rbuggyMatches.join("|") );

	/* Contains
	---------------------------------------------------------------------- */
	hasCompare = rnative.test( docElem.compareDocumentPosition );

	// Element contains another
	// Purposefully does not implement inclusive descendent
	// As in, an element does not contain itself
	contains = hasCompare || rnative.test( docElem.contains ) ?
		function( a, b ) {
			var adown = a.nodeType === 9 ? a.documentElement : a,
				bup = b && b.parentNode;
			return a === bup || !!( bup && bup.nodeType === 1 && (
				adown.contains ?
					adown.contains( bup ) :
					a.compareDocumentPosition && a.compareDocumentPosition( bup ) & 16
			));
		} :
		function( a, b ) {
			if ( b ) {
				while ( (b = b.parentNode) ) {
					if ( b === a ) {
						return true;
					}
				}
			}
			return false;
		};

	/* Sorting
	---------------------------------------------------------------------- */

	// Document order sorting
	sortOrder = hasCompare ?
	function( a, b ) {

		// Flag for duplicate removal
		if ( a === b ) {
			hasDuplicate = true;
			return 0;
		}

		// Sort on method existence if only one input has compareDocumentPosition
		var compare = !a.compareDocumentPosition - !b.compareDocumentPosition;
		if ( compare ) {
			return compare;
		}

		// Calculate position if both inputs belong to the same document
		compare = ( a.ownerDocument || a ) === ( b.ownerDocument || b ) ?
			a.compareDocumentPosition( b ) :

			// Otherwise we know they are disconnected
			1;

		// Disconnected nodes
		if ( compare & 1 ||
			(!support.sortDetached && b.compareDocumentPosition( a ) === compare) ) {

			// Choose the first element that is related to our preferred document
			if ( a === doc || a.ownerDocument === preferredDoc && contains(preferredDoc, a) ) {
				return -1;
			}
			if ( b === doc || b.ownerDocument === preferredDoc && contains(preferredDoc, b) ) {
				return 1;
			}

			// Maintain original order
			return sortInput ?
				( indexOf.call( sortInput, a ) - indexOf.call( sortInput, b ) ) :
				0;
		}

		return compare & 4 ? -1 : 1;
	} :
	function( a, b ) {
		// Exit early if the nodes are identical
		if ( a === b ) {
			hasDuplicate = true;
			return 0;
		}

		var cur,
			i = 0,
			aup = a.parentNode,
			bup = b.parentNode,
			ap = [ a ],
			bp = [ b ];

		// Parentless nodes are either documents or disconnected
		if ( !aup || !bup ) {
			return a === doc ? -1 :
				b === doc ? 1 :
				aup ? -1 :
				bup ? 1 :
				sortInput ?
				( indexOf.call( sortInput, a ) - indexOf.call( sortInput, b ) ) :
				0;

		// If the nodes are siblings, we can do a quick check
		} else if ( aup === bup ) {
			return siblingCheck( a, b );
		}

		// Otherwise we need full lists of their ancestors for comparison
		cur = a;
		while ( (cur = cur.parentNode) ) {
			ap.unshift( cur );
		}
		cur = b;
		while ( (cur = cur.parentNode) ) {
			bp.unshift( cur );
		}

		// Walk down the tree looking for a discrepancy
		while ( ap[i] === bp[i] ) {
			i++;
		}

		return i ?
			// Do a sibling check if the nodes have a common ancestor
			siblingCheck( ap[i], bp[i] ) :

			// Otherwise nodes in our document sort first
			ap[i] === preferredDoc ? -1 :
			bp[i] === preferredDoc ? 1 :
			0;
	};

	return doc;
};

Sizzle.matches = function( expr, elements ) {
	return Sizzle( expr, null, null, elements );
};

Sizzle.matchesSelector = function( elem, expr ) {
	// Set document vars if needed
	if ( ( elem.ownerDocument || elem ) !== document ) {
		setDocument( elem );
	}

	// Make sure that attribute selectors are quoted
	expr = expr.replace( rattributeQuotes, "='$1']" );

	if ( support.matchesSelector && documentIsHTML &&
		( !rbuggyMatches || !rbuggyMatches.test( expr ) ) &&
		( !rbuggyQSA     || !rbuggyQSA.test( expr ) ) ) {

		try {
			var ret = matches.call( elem, expr );

			// IE 9's matchesSelector returns false on disconnected nodes
			if ( ret || support.disconnectedMatch ||
					// As well, disconnected nodes are said to be in a document
					// fragment in IE 9
					elem.document && elem.document.nodeType !== 11 ) {
				return ret;
			}
		} catch(e) {}
	}

	return Sizzle( expr, document, null, [ elem ] ).length > 0;
};

Sizzle.contains = function( context, elem ) {
	// Set document vars if needed
	if ( ( context.ownerDocument || context ) !== document ) {
		setDocument( context );
	}
	return contains( context, elem );
};

Sizzle.attr = function( elem, name ) {
	// Set document vars if needed
	if ( ( elem.ownerDocument || elem ) !== document ) {
		setDocument( elem );
	}

	var fn = Expr.attrHandle[ name.toLowerCase() ],
		// Don't get fooled by Object.prototype properties (jQuery #13807)
		val = fn && hasOwn.call( Expr.attrHandle, name.toLowerCase() ) ?
			fn( elem, name, !documentIsHTML ) :
			undefined;

	return val !== undefined ?
		val :
		support.attributes || !documentIsHTML ?
			elem.getAttribute( name ) :
			(val = elem.getAttributeNode(name)) && val.specified ?
				val.value :
				null;
};

Sizzle.error = function( msg ) {
	throw new Error( "Syntax error, unrecognized expression: " + msg );
};

/**
 * Document sorting and removing duplicates
 * @param {ArrayLike} results
 */
Sizzle.uniqueSort = function( results ) {
	var elem,
		duplicates = [],
		j = 0,
		i = 0;

	// Unless we *know* we can detect duplicates, assume their presence
	hasDuplicate = !support.detectDuplicates;
	sortInput = !support.sortStable && results.slice( 0 );
	results.sort( sortOrder );

	if ( hasDuplicate ) {
		while ( (elem = results[i++]) ) {
			if ( elem === results[ i ] ) {
				j = duplicates.push( i );
			}
		}
		while ( j-- ) {
			results.splice( duplicates[ j ], 1 );
		}
	}

	// Clear input after sorting to release objects
	// See https://github.com/jquery/sizzle/pull/225
	sortInput = null;

	return results;
};

/**
 * Utility function for retrieving the text value of an array of DOM nodes
 * @param {Array|Element} elem
 */
getText = Sizzle.getText = function( elem ) {
	var node,
		ret = "",
		i = 0,
		nodeType = elem.nodeType;

	if ( !nodeType ) {
		// If no nodeType, this is expected to be an array
		while ( (node = elem[i++]) ) {
			// Do not traverse comment nodes
			ret += getText( node );
		}
	} else if ( nodeType === 1 || nodeType === 9 || nodeType === 11 ) {
		// Use textContent for elements
		// innerText usage removed for consistency of new lines (jQuery #11153)
		if ( typeof elem.textContent === "string" ) {
			return elem.textContent;
		} else {
			// Traverse its children
			for ( elem = elem.firstChild; elem; elem = elem.nextSibling ) {
				ret += getText( elem );
			}
		}
	} else if ( nodeType === 3 || nodeType === 4 ) {
		return elem.nodeValue;
	}
	// Do not include comment or processing instruction nodes

	return ret;
};

Expr = Sizzle.selectors = {

	// Can be adjusted by the user
	cacheLength: 50,

	createPseudo: markFunction,

	match: matchExpr,

	attrHandle: {},

	find: {},

	relative: {
		">": { dir: "parentNode", first: true },
		" ": { dir: "parentNode" },
		"+": { dir: "previousSibling", first: true },
		"~": { dir: "previousSibling" }
	},

	preFilter: {
		"ATTR": function( match ) {
			match[1] = match[1].replace( runescape, funescape );

			// Move the given value to match[3] whether quoted or unquoted
			match[3] = ( match[3] || match[4] || match[5] || "" ).replace( runescape, funescape );

			if ( match[2] === "~=" ) {
				match[3] = " " + match[3] + " ";
			}

			return match.slice( 0, 4 );
		},

		"CHILD": function( match ) {
			/* matches from matchExpr["CHILD"]
				1 type (only|nth|...)
				2 what (child|of-type)
				3 argument (even|odd|\d*|\d*n([+-]\d+)?|...)
				4 xn-component of xn+y argument ([+-]?\d*n|)
				5 sign of xn-component
				6 x of xn-component
				7 sign of y-component
				8 y of y-component
			*/
			match[1] = match[1].toLowerCase();

			if ( match[1].slice( 0, 3 ) === "nth" ) {
				// nth-* requires argument
				if ( !match[3] ) {
					Sizzle.error( match[0] );
				}

				// numeric x and y parameters for Expr.filter.CHILD
				// remember that false/true cast respectively to 0/1
				match[4] = +( match[4] ? match[5] + (match[6] || 1) : 2 * ( match[3] === "even" || match[3] === "odd" ) );
				match[5] = +( ( match[7] + match[8] ) || match[3] === "odd" );

			// other types prohibit arguments
			} else if ( match[3] ) {
				Sizzle.error( match[0] );
			}

			return match;
		},

		"PSEUDO": function( match ) {
			var excess,
				unquoted = !match[6] && match[2];

			if ( matchExpr["CHILD"].test( match[0] ) ) {
				return null;
			}

			// Accept quoted arguments as-is
			if ( match[3] ) {
				match[2] = match[4] || match[5] || "";

			// Strip excess characters from unquoted arguments
			} else if ( unquoted && rpseudo.test( unquoted ) &&
				// Get excess from tokenize (recursively)
				(excess = tokenize( unquoted, true )) &&
				// advance to the next closing parenthesis
				(excess = unquoted.indexOf( ")", unquoted.length - excess ) - unquoted.length) ) {

				// excess is a negative index
				match[0] = match[0].slice( 0, excess );
				match[2] = unquoted.slice( 0, excess );
			}

			// Return only captures needed by the pseudo filter method (type and argument)
			return match.slice( 0, 3 );
		}
	},

	filter: {

		"TAG": function( nodeNameSelector ) {
			var nodeName = nodeNameSelector.replace( runescape, funescape ).toLowerCase();
			return nodeNameSelector === "*" ?
				function() { return true; } :
				function( elem ) {
					return elem.nodeName && elem.nodeName.toLowerCase() === nodeName;
				};
		},

		"CLASS": function( className ) {
			var pattern = classCache[ className + " " ];

			return pattern ||
				(pattern = new RegExp( "(^|" + whitespace + ")" + className + "(" + whitespace + "|$)" )) &&
				classCache( className, function( elem ) {
					return pattern.test( typeof elem.className === "string" && elem.className || typeof elem.getAttribute !== strundefined && elem.getAttribute("class") || "" );
				});
		},

		"ATTR": function( name, operator, check ) {
			return function( elem ) {
				var result = Sizzle.attr( elem, name );

				if ( result == null ) {
					return operator === "!=";
				}
				if ( !operator ) {
					return true;
				}

				result += "";

				return operator === "=" ? result === check :
					operator === "!=" ? result !== check :
					operator === "^=" ? check && result.indexOf( check ) === 0 :
					operator === "*=" ? check && result.indexOf( check ) > -1 :
					operator === "$=" ? check && result.slice( -check.length ) === check :
					operator === "~=" ? ( " " + result + " " ).indexOf( check ) > -1 :
					operator === "|=" ? result === check || result.slice( 0, check.length + 1 ) === check + "-" :
					false;
			};
		},

		"CHILD": function( type, what, argument, first, last ) {
			var simple = type.slice( 0, 3 ) !== "nth",
				forward = type.slice( -4 ) !== "last",
				ofType = what === "of-type";

			return first === 1 && last === 0 ?

				// Shortcut for :nth-*(n)
				function( elem ) {
					return !!elem.parentNode;
				} :

				function( elem, context, xml ) {
					var cache, outerCache, node, diff, nodeIndex, start,
						dir = simple !== forward ? "nextSibling" : "previousSibling",
						parent = elem.parentNode,
						name = ofType && elem.nodeName.toLowerCase(),
						useCache = !xml && !ofType;

					if ( parent ) {

						// :(first|last|only)-(child|of-type)
						if ( simple ) {
							while ( dir ) {
								node = elem;
								while ( (node = node[ dir ]) ) {
									if ( ofType ? node.nodeName.toLowerCase() === name : node.nodeType === 1 ) {
										return false;
									}
								}
								// Reverse direction for :only-* (if we haven't yet done so)
								start = dir = type === "only" && !start && "nextSibling";
							}
							return true;
						}

						start = [ forward ? parent.firstChild : parent.lastChild ];

						// non-xml :nth-child(...) stores cache data on `parent`
						if ( forward && useCache ) {
							// Seek `elem` from a previously-cached index
							outerCache = parent[ expando ] || (parent[ expando ] = {});
							cache = outerCache[ type ] || [];
							nodeIndex = cache[0] === dirruns && cache[1];
							diff = cache[0] === dirruns && cache[2];
							node = nodeIndex && parent.childNodes[ nodeIndex ];

							while ( (node = ++nodeIndex && node && node[ dir ] ||

								// Fallback to seeking `elem` from the start
								(diff = nodeIndex = 0) || start.pop()) ) {

								// When found, cache indexes on `parent` and break
								if ( node.nodeType === 1 && ++diff && node === elem ) {
									outerCache[ type ] = [ dirruns, nodeIndex, diff ];
									break;
								}
							}

						// Use previously-cached element index if available
						} else if ( useCache && (cache = (elem[ expando ] || (elem[ expando ] = {}))[ type ]) && cache[0] === dirruns ) {
							diff = cache[1];

						// xml :nth-child(...) or :nth-last-child(...) or :nth(-last)?-of-type(...)
						} else {
							// Use the same loop as above to seek `elem` from the start
							while ( (node = ++nodeIndex && node && node[ dir ] ||
								(diff = nodeIndex = 0) || start.pop()) ) {

								if ( ( ofType ? node.nodeName.toLowerCase() === name : node.nodeType === 1 ) && ++diff ) {
									// Cache the index of each encountered element
									if ( useCache ) {
										(node[ expando ] || (node[ expando ] = {}))[ type ] = [ dirruns, diff ];
									}

									if ( node === elem ) {
										break;
									}
								}
							}
						}

						// Incorporate the offset, then check against cycle size
						diff -= last;
						return diff === first || ( diff % first === 0 && diff / first >= 0 );
					}
				};
		},

		"PSEUDO": function( pseudo, argument ) {
			// pseudo-class names are case-insensitive
			// http://www.w3.org/TR/selectors/#pseudo-classes
			// Prioritize by case sensitivity in case custom pseudos are added with uppercase letters
			// Remember that setFilters inherits from pseudos
			var args,
				fn = Expr.pseudos[ pseudo ] || Expr.setFilters[ pseudo.toLowerCase() ] ||
					Sizzle.error( "unsupported pseudo: " + pseudo );

			// The user may use createPseudo to indicate that
			// arguments are needed to create the filter function
			// just as Sizzle does
			if ( fn[ expando ] ) {
				return fn( argument );
			}

			// But maintain support for old signatures
			if ( fn.length > 1 ) {
				args = [ pseudo, pseudo, "", argument ];
				return Expr.setFilters.hasOwnProperty( pseudo.toLowerCase() ) ?
					markFunction(function( seed, matches ) {
						var idx,
							matched = fn( seed, argument ),
							i = matched.length;
						while ( i-- ) {
							idx = indexOf.call( seed, matched[i] );
							seed[ idx ] = !( matches[ idx ] = matched[i] );
						}
					}) :
					function( elem ) {
						return fn( elem, 0, args );
					};
			}

			return fn;
		}
	},

	pseudos: {
		// Potentially complex pseudos
		"not": markFunction(function( selector ) {
			// Trim the selector passed to compile
			// to avoid treating leading and trailing
			// spaces as combinators
			var input = [],
				results = [],
				matcher = compile( selector.replace( rtrim, "$1" ) );

			return matcher[ expando ] ?
				markFunction(function( seed, matches, context, xml ) {
					var elem,
						unmatched = matcher( seed, null, xml, [] ),
						i = seed.length;

					// Match elements unmatched by `matcher`
					while ( i-- ) {
						if ( (elem = unmatched[i]) ) {
							seed[i] = !(matches[i] = elem);
						}
					}
				}) :
				function( elem, context, xml ) {
					input[0] = elem;
					matcher( input, null, xml, results );
					return !results.pop();
				};
		}),

		"has": markFunction(function( selector ) {
			return function( elem ) {
				return Sizzle( selector, elem ).length > 0;
			};
		}),

		"contains": markFunction(function( text ) {
			return function( elem ) {
				return ( elem.textContent || elem.innerText || getText( elem ) ).indexOf( text ) > -1;
			};
		}),

		// "Whether an element is represented by a :lang() selector
		// is based solely on the element's language value
		// being equal to the identifier C,
		// or beginning with the identifier C immediately followed by "-".
		// The matching of C against the element's language value is performed case-insensitively.
		// The identifier C does not have to be a valid language name."
		// http://www.w3.org/TR/selectors/#lang-pseudo
		"lang": markFunction( function( lang ) {
			// lang value must be a valid identifier
			if ( !ridentifier.test(lang || "") ) {
				Sizzle.error( "unsupported lang: " + lang );
			}
			lang = lang.replace( runescape, funescape ).toLowerCase();
			return function( elem ) {
				var elemLang;
				do {
					if ( (elemLang = documentIsHTML ?
						elem.lang :
						elem.getAttribute("xml:lang") || elem.getAttribute("lang")) ) {

						elemLang = elemLang.toLowerCase();
						return elemLang === lang || elemLang.indexOf( lang + "-" ) === 0;
					}
				} while ( (elem = elem.parentNode) && elem.nodeType === 1 );
				return false;
			};
		}),

		// Miscellaneous
		"target": function( elem ) {
			var hash = window.location && window.location.hash;
			return hash && hash.slice( 1 ) === elem.id;
		},

		"root": function( elem ) {
			return elem === docElem;
		},

		"focus": function( elem ) {
			return elem === document.activeElement && (!document.hasFocus || document.hasFocus()) && !!(elem.type || elem.href || ~elem.tabIndex);
		},

		// Boolean properties
		"enabled": function( elem ) {
			return elem.disabled === false;
		},

		"disabled": function( elem ) {
			return elem.disabled === true;
		},

		"checked": function( elem ) {
			// In CSS3, :checked should return both checked and selected elements
			// http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#checked
			var nodeName = elem.nodeName.toLowerCase();
			return (nodeName === "input" && !!elem.checked) || (nodeName === "option" && !!elem.selected);
		},

		"selected": function( elem ) {
			// Accessing this property makes selected-by-default
			// options in Safari work properly
			if ( elem.parentNode ) {
				elem.parentNode.selectedIndex;
			}

			return elem.selected === true;
		},

		// Contents
		"empty": function( elem ) {
			// http://www.w3.org/TR/selectors/#empty-pseudo
			// :empty is negated by element (1) or content nodes (text: 3; cdata: 4; entity ref: 5),
			//   but not by others (comment: 8; processing instruction: 7; etc.)
			// nodeType < 6 works because attributes (2) do not appear as children
			for ( elem = elem.firstChild; elem; elem = elem.nextSibling ) {
				if ( elem.nodeType < 6 ) {
					return false;
				}
			}
			return true;
		},

		"parent": function( elem ) {
			return !Expr.pseudos["empty"]( elem );
		},

		// Element/input types
		"header": function( elem ) {
			return rheader.test( elem.nodeName );
		},

		"input": function( elem ) {
			return rinputs.test( elem.nodeName );
		},

		"button": function( elem ) {
			var name = elem.nodeName.toLowerCase();
			return name === "input" && elem.type === "button" || name === "button";
		},

		"text": function( elem ) {
			var attr;
			return elem.nodeName.toLowerCase() === "input" &&
				elem.type === "text" &&

				// Support: IE<8
				// New HTML5 attribute values (e.g., "search") appear with elem.type === "text"
				( (attr = elem.getAttribute("type")) == null || attr.toLowerCase() === "text" );
		},

		// Position-in-collection
		"first": createPositionalPseudo(function() {
			return [ 0 ];
		}),

		"last": createPositionalPseudo(function( matchIndexes, length ) {
			return [ length - 1 ];
		}),

		"eq": createPositionalPseudo(function( matchIndexes, length, argument ) {
			return [ argument < 0 ? argument + length : argument ];
		}),

		"even": createPositionalPseudo(function( matchIndexes, length ) {
			var i = 0;
			for ( ; i < length; i += 2 ) {
				matchIndexes.push( i );
			}
			return matchIndexes;
		}),

		"odd": createPositionalPseudo(function( matchIndexes, length ) {
			var i = 1;
			for ( ; i < length; i += 2 ) {
				matchIndexes.push( i );
			}
			return matchIndexes;
		}),

		"lt": createPositionalPseudo(function( matchIndexes, length, argument ) {
			var i = argument < 0 ? argument + length : argument;
			for ( ; --i >= 0; ) {
				matchIndexes.push( i );
			}
			return matchIndexes;
		}),

		"gt": createPositionalPseudo(function( matchIndexes, length, argument ) {
			var i = argument < 0 ? argument + length : argument;
			for ( ; ++i < length; ) {
				matchIndexes.push( i );
			}
			return matchIndexes;
		})
	}
};

Expr.pseudos["nth"] = Expr.pseudos["eq"];

// Add button/input type pseudos
for ( i in { radio: true, checkbox: true, file: true, password: true, image: true } ) {
	Expr.pseudos[ i ] = createInputPseudo( i );
}
for ( i in { submit: true, reset: true } ) {
	Expr.pseudos[ i ] = createButtonPseudo( i );
}

// Easy API for creating new setFilters
function setFilters() {}
setFilters.prototype = Expr.filters = Expr.pseudos;
Expr.setFilters = new setFilters();

tokenize = Sizzle.tokenize = function( selector, parseOnly ) {
	var matched, match, tokens, type,
		soFar, groups, preFilters,
		cached = tokenCache[ selector + " " ];

	if ( cached ) {
		return parseOnly ? 0 : cached.slice( 0 );
	}

	soFar = selector;
	groups = [];
	preFilters = Expr.preFilter;

	while ( soFar ) {

		// Comma and first run
		if ( !matched || (match = rcomma.exec( soFar )) ) {
			if ( match ) {
				// Don't consume trailing commas as valid
				soFar = soFar.slice( match[0].length ) || soFar;
			}
			groups.push( (tokens = []) );
		}

		matched = false;

		// Combinators
		if ( (match = rcombinators.exec( soFar )) ) {
			matched = match.shift();
			tokens.push({
				value: matched,
				// Cast descendant combinators to space
				type: match[0].replace( rtrim, " " )
			});
			soFar = soFar.slice( matched.length );
		}

		// Filters
		for ( type in Expr.filter ) {
			if ( (match = matchExpr[ type ].exec( soFar )) && (!preFilters[ type ] ||
				(match = preFilters[ type ]( match ))) ) {
				matched = match.shift();
				tokens.push({
					value: matched,
					type: type,
					matches: match
				});
				soFar = soFar.slice( matched.length );
			}
		}

		if ( !matched ) {
			break;
		}
	}

	// Return the length of the invalid excess
	// if we're just parsing
	// Otherwise, throw an error or return tokens
	return parseOnly ?
		soFar.length :
		soFar ?
			Sizzle.error( selector ) :
			// Cache the tokens
			tokenCache( selector, groups ).slice( 0 );
};

function toSelector( tokens ) {
	var i = 0,
		len = tokens.length,
		selector = "";
	for ( ; i < len; i++ ) {
		selector += tokens[i].value;
	}
	return selector;
}

function addCombinator( matcher, combinator, base ) {
	var dir = combinator.dir,
		checkNonElements = base && dir === "parentNode",
		doneName = done++;

	return combinator.first ?
		// Check against closest ancestor/preceding element
		function( elem, context, xml ) {
			while ( (elem = elem[ dir ]) ) {
				if ( elem.nodeType === 1 || checkNonElements ) {
					return matcher( elem, context, xml );
				}
			}
		} :

		// Check against all ancestor/preceding elements
		function( elem, context, xml ) {
			var oldCache, outerCache,
				newCache = [ dirruns, doneName ];

			// We can't set arbitrary data on XML nodes, so they don't benefit from dir caching
			if ( xml ) {
				while ( (elem = elem[ dir ]) ) {
					if ( elem.nodeType === 1 || checkNonElements ) {
						if ( matcher( elem, context, xml ) ) {
							return true;
						}
					}
				}
			} else {
				while ( (elem = elem[ dir ]) ) {
					if ( elem.nodeType === 1 || checkNonElements ) {
						outerCache = elem[ expando ] || (elem[ expando ] = {});
						if ( (oldCache = outerCache[ dir ]) &&
							oldCache[ 0 ] === dirruns && oldCache[ 1 ] === doneName ) {

							// Assign to newCache so results back-propagate to previous elements
							return (newCache[ 2 ] = oldCache[ 2 ]);
						} else {
							// Reuse newcache so results back-propagate to previous elements
							outerCache[ dir ] = newCache;

							// A match means we're done; a fail means we have to keep checking
							if ( (newCache[ 2 ] = matcher( elem, context, xml )) ) {
								return true;
							}
						}
					}
				}
			}
		};
}

function elementMatcher( matchers ) {
	return matchers.length > 1 ?
		function( elem, context, xml ) {
			var i = matchers.length;
			while ( i-- ) {
				if ( !matchers[i]( elem, context, xml ) ) {
					return false;
				}
			}
			return true;
		} :
		matchers[0];
}

function multipleContexts( selector, contexts, results ) {
	var i = 0,
		len = contexts.length;
	for ( ; i < len; i++ ) {
		Sizzle( selector, contexts[i], results );
	}
	return results;
}

function condense( unmatched, map, filter, context, xml ) {
	var elem,
		newUnmatched = [],
		i = 0,
		len = unmatched.length,
		mapped = map != null;

	for ( ; i < len; i++ ) {
		if ( (elem = unmatched[i]) ) {
			if ( !filter || filter( elem, context, xml ) ) {
				newUnmatched.push( elem );
				if ( mapped ) {
					map.push( i );
				}
			}
		}
	}

	return newUnmatched;
}

function setMatcher( preFilter, selector, matcher, postFilter, postFinder, postSelector ) {
	if ( postFilter && !postFilter[ expando ] ) {
		postFilter = setMatcher( postFilter );
	}
	if ( postFinder && !postFinder[ expando ] ) {
		postFinder = setMatcher( postFinder, postSelector );
	}
	return markFunction(function( seed, results, context, xml ) {
		var temp, i, elem,
			preMap = [],
			postMap = [],
			preexisting = results.length,

			// Get initial elements from seed or context
			elems = seed || multipleContexts( selector || "*", context.nodeType ? [ context ] : context, [] ),

			// Prefilter to get matcher input, preserving a map for seed-results synchronization
			matcherIn = preFilter && ( seed || !selector ) ?
				condense( elems, preMap, preFilter, context, xml ) :
				elems,

			matcherOut = matcher ?
				// If we have a postFinder, or filtered seed, or non-seed postFilter or preexisting results,
				postFinder || ( seed ? preFilter : preexisting || postFilter ) ?

					// ...intermediate processing is necessary
					[] :

					// ...otherwise use results directly
					results :
				matcherIn;

		// Find primary matches
		if ( matcher ) {
			matcher( matcherIn, matcherOut, context, xml );
		}

		// Apply postFilter
		if ( postFilter ) {
			temp = condense( matcherOut, postMap );
			postFilter( temp, [], context, xml );

			// Un-match failing elements by moving them back to matcherIn
			i = temp.length;
			while ( i-- ) {
				if ( (elem = temp[i]) ) {
					matcherOut[ postMap[i] ] = !(matcherIn[ postMap[i] ] = elem);
				}
			}
		}

		if ( seed ) {
			if ( postFinder || preFilter ) {
				if ( postFinder ) {
					// Get the final matcherOut by condensing this intermediate into postFinder contexts
					temp = [];
					i = matcherOut.length;
					while ( i-- ) {
						if ( (elem = matcherOut[i]) ) {
							// Restore matcherIn since elem is not yet a final match
							temp.push( (matcherIn[i] = elem) );
						}
					}
					postFinder( null, (matcherOut = []), temp, xml );
				}

				// Move matched elements from seed to results to keep them synchronized
				i = matcherOut.length;
				while ( i-- ) {
					if ( (elem = matcherOut[i]) &&
						(temp = postFinder ? indexOf.call( seed, elem ) : preMap[i]) > -1 ) {

						seed[temp] = !(results[temp] = elem);
					}
				}
			}

		// Add elements to results, through postFinder if defined
		} else {
			matcherOut = condense(
				matcherOut === results ?
					matcherOut.splice( preexisting, matcherOut.length ) :
					matcherOut
			);
			if ( postFinder ) {
				postFinder( null, results, matcherOut, xml );
			} else {
				push.apply( results, matcherOut );
			}
		}
	});
}

function matcherFromTokens( tokens ) {
	var checkContext, matcher, j,
		len = tokens.length,
		leadingRelative = Expr.relative[ tokens[0].type ],
		implicitRelative = leadingRelative || Expr.relative[" "],
		i = leadingRelative ? 1 : 0,

		// The foundational matcher ensures that elements are reachable from top-level context(s)
		matchContext = addCombinator( function( elem ) {
			return elem === checkContext;
		}, implicitRelative, true ),
		matchAnyContext = addCombinator( function( elem ) {
			return indexOf.call( checkContext, elem ) > -1;
		}, implicitRelative, true ),
		matchers = [ function( elem, context, xml ) {
			return ( !leadingRelative && ( xml || context !== outermostContext ) ) || (
				(checkContext = context).nodeType ?
					matchContext( elem, context, xml ) :
					matchAnyContext( elem, context, xml ) );
		} ];

	for ( ; i < len; i++ ) {
		if ( (matcher = Expr.relative[ tokens[i].type ]) ) {
			matchers = [ addCombinator(elementMatcher( matchers ), matcher) ];
		} else {
			matcher = Expr.filter[ tokens[i].type ].apply( null, tokens[i].matches );

			// Return special upon seeing a positional matcher
			if ( matcher[ expando ] ) {
				// Find the next relative operator (if any) for proper handling
				j = ++i;
				for ( ; j < len; j++ ) {
					if ( Expr.relative[ tokens[j].type ] ) {
						break;
					}
				}
				return setMatcher(
					i > 1 && elementMatcher( matchers ),
					i > 1 && toSelector(
						// If the preceding token was a descendant combinator, insert an implicit any-element `*`
						tokens.slice( 0, i - 1 ).concat({ value: tokens[ i - 2 ].type === " " ? "*" : "" })
					).replace( rtrim, "$1" ),
					matcher,
					i < j && matcherFromTokens( tokens.slice( i, j ) ),
					j < len && matcherFromTokens( (tokens = tokens.slice( j )) ),
					j < len && toSelector( tokens )
				);
			}
			matchers.push( matcher );
		}
	}

	return elementMatcher( matchers );
}

function matcherFromGroupMatchers( elementMatchers, setMatchers ) {
	var bySet = setMatchers.length > 0,
		byElement = elementMatchers.length > 0,
		superMatcher = function( seed, context, xml, results, outermost ) {
			var elem, j, matcher,
				matchedCount = 0,
				i = "0",
				unmatched = seed && [],
				setMatched = [],
				contextBackup = outermostContext,
				// We must always have either seed elements or outermost context
				elems = seed || byElement && Expr.find["TAG"]( "*", outermost ),
				// Use integer dirruns iff this is the outermost matcher
				dirrunsUnique = (dirruns += contextBackup == null ? 1 : Math.random() || 0.1),
				len = elems.length;

			if ( outermost ) {
				outermostContext = context !== document && context;
			}

			// Add elements passing elementMatchers directly to results
			// Keep `i` a string if there are no elements so `matchedCount` will be "00" below
			// Support: IE<9, Safari
			// Tolerate NodeList properties (IE: "length"; Safari: <number>) matching elements by id
			for ( ; i !== len && (elem = elems[i]) != null; i++ ) {
				if ( byElement && elem ) {
					j = 0;
					while ( (matcher = elementMatchers[j++]) ) {
						if ( matcher( elem, context, xml ) ) {
							results.push( elem );
							break;
						}
					}
					if ( outermost ) {
						dirruns = dirrunsUnique;
					}
				}

				// Track unmatched elements for set filters
				if ( bySet ) {
					// They will have gone through all possible matchers
					if ( (elem = !matcher && elem) ) {
						matchedCount--;
					}

					// Lengthen the array for every element, matched or not
					if ( seed ) {
						unmatched.push( elem );
					}
				}
			}

			// Apply set filters to unmatched elements
			matchedCount += i;
			if ( bySet && i !== matchedCount ) {
				j = 0;
				while ( (matcher = setMatchers[j++]) ) {
					matcher( unmatched, setMatched, context, xml );
				}

				if ( seed ) {
					// Reintegrate element matches to eliminate the need for sorting
					if ( matchedCount > 0 ) {
						while ( i-- ) {
							if ( !(unmatched[i] || setMatched[i]) ) {
								setMatched[i] = pop.call( results );
							}
						}
					}

					// Discard index placeholder values to get only actual matches
					setMatched = condense( setMatched );
				}

				// Add matches to results
				push.apply( results, setMatched );

				// Seedless set matches succeeding multiple successful matchers stipulate sorting
				if ( outermost && !seed && setMatched.length > 0 &&
					( matchedCount + setMatchers.length ) > 1 ) {

					Sizzle.uniqueSort( results );
				}
			}

			// Override manipulation of globals by nested matchers
			if ( outermost ) {
				dirruns = dirrunsUnique;
				outermostContext = contextBackup;
			}

			return unmatched;
		};

	return bySet ?
		markFunction( superMatcher ) :
		superMatcher;
}

compile = Sizzle.compile = function( selector, match /* Internal Use Only */ ) {
	var i,
		setMatchers = [],
		elementMatchers = [],
		cached = compilerCache[ selector + " " ];

	if ( !cached ) {
		// Generate a function of recursive functions that can be used to check each element
		if ( !match ) {
			match = tokenize( selector );
		}
		i = match.length;
		while ( i-- ) {
			cached = matcherFromTokens( match[i] );
			if ( cached[ expando ] ) {
				setMatchers.push( cached );
			} else {
				elementMatchers.push( cached );
			}
		}

		// Cache the compiled function
		cached = compilerCache( selector, matcherFromGroupMatchers( elementMatchers, setMatchers ) );

		// Save selector and tokenization
		cached.selector = selector;
	}
	return cached;
};

/**
 * A low-level selection function that works with Sizzle's compiled
 *  selector functions
 * @param {String|Function} selector A selector or a pre-compiled
 *  selector function built with Sizzle.compile
 * @param {Element} context
 * @param {Array} [results]
 * @param {Array} [seed] A set of elements to match against
 */
select = Sizzle.select = function( selector, context, results, seed ) {
	var i, tokens, token, type, find,
		compiled = typeof selector === "function" && selector,
		match = !seed && tokenize( (selector = compiled.selector || selector) );

	results = results || [];

	// Try to minimize operations if there is no seed and only one group
	if ( match.length === 1 ) {

		// Take a shortcut and set the context if the root selector is an ID
		tokens = match[0] = match[0].slice( 0 );
		if ( tokens.length > 2 && (token = tokens[0]).type === "ID" &&
				support.getById && context.nodeType === 9 && documentIsHTML &&
				Expr.relative[ tokens[1].type ] ) {

			context = ( Expr.find["ID"]( token.matches[0].replace(runescape, funescape), context ) || [] )[0];
			if ( !context ) {
				return results;

			// Precompiled matchers will still verify ancestry, so step up a level
			} else if ( compiled ) {
				context = context.parentNode;
			}

			selector = selector.slice( tokens.shift().value.length );
		}

		// Fetch a seed set for right-to-left matching
		i = matchExpr["needsContext"].test( selector ) ? 0 : tokens.length;
		while ( i-- ) {
			token = tokens[i];

			// Abort if we hit a combinator
			if ( Expr.relative[ (type = token.type) ] ) {
				break;
			}
			if ( (find = Expr.find[ type ]) ) {
				// Search, expanding context for leading sibling combinators
				if ( (seed = find(
					token.matches[0].replace( runescape, funescape ),
					rsibling.test( tokens[0].type ) && testContext( context.parentNode ) || context
				)) ) {

					// If seed is empty or no tokens remain, we can return early
					tokens.splice( i, 1 );
					selector = seed.length && toSelector( tokens );
					if ( !selector ) {
						push.apply( results, seed );
						return results;
					}

					break;
				}
			}
		}
	}

	// Compile and execute a filtering function if one is not provided
	// Provide `match` to avoid retokenization if we modified the selector above
	( compiled || compile( selector, match ) )(
		seed,
		context,
		!documentIsHTML,
		results,
		rsibling.test( selector ) && testContext( context.parentNode ) || context
	);
	return results;
};

// One-time assignments

// Sort stability
support.sortStable = expando.split("").sort( sortOrder ).join("") === expando;

// Support: Chrome<14
// Always assume duplicates if they aren't passed to the comparison function
support.detectDuplicates = !!hasDuplicate;

// Initialize against the default document
setDocument();

// Support: Webkit<537.32 - Safari 6.0.3/Chrome 25 (fixed in Chrome 27)
// Detached nodes confoundingly follow *each other*
support.sortDetached = assert(function( div1 ) {
	// Should return 1, but returns 4 (following)
	return div1.compareDocumentPosition( document.createElement("div") ) & 1;
});

// Support: IE<8
// Prevent attribute/property "interpolation"
// http://msdn.microsoft.com/en-us/library/ms536429%28VS.85%29.aspx
if ( !assert(function( div ) {
	div.innerHTML = "<a href='#'></a>";
	return div.firstChild.getAttribute("href") === "#" ;
}) ) {
	addHandle( "type|href|height|width", function( elem, name, isXML ) {
		if ( !isXML ) {
			return elem.getAttribute( name, name.toLowerCase() === "type" ? 1 : 2 );
		}
	});
}

// Support: IE<9
// Use defaultValue in place of getAttribute("value")
if ( !support.attributes || !assert(function( div ) {
	div.innerHTML = "<input/>";
	div.firstChild.setAttribute( "value", "" );
	return div.firstChild.getAttribute( "value" ) === "";
}) ) {
	addHandle( "value", function( elem, name, isXML ) {
		if ( !isXML && elem.nodeName.toLowerCase() === "input" ) {
			return elem.defaultValue;
		}
	});
}

// Support: IE<9
// Use getAttributeNode to fetch booleans when getAttribute lies
if ( !assert(function( div ) {
	return div.getAttribute("disabled") == null;
}) ) {
	addHandle( booleans, function( elem, name, isXML ) {
		var val;
		if ( !isXML ) {
			return elem[ name ] === true ? name.toLowerCase() :
					(val = elem.getAttributeNode( name )) && val.specified ?
					val.value :
				null;
		}
	});
}

return Sizzle;

})( window );



jQuery.find = Sizzle;
jQuery.expr = Sizzle.selectors;
jQuery.expr[":"] = jQuery.expr.pseudos;
jQuery.unique = Sizzle.uniqueSort;
jQuery.text = Sizzle.getText;
jQuery.isXMLDoc = Sizzle.isXML;
jQuery.contains = Sizzle.contains;



var rneedsContext = jQuery.expr.match.needsContext;

var rsingleTag = (/^<(\w+)\s*\/?>(?:<\/\1>|)$/);



var risSimple = /^.[^:#\[\.,]*$/;

// Implement the identical functionality for filter and not
function winnow( elements, qualifier, not ) {
	if ( jQuery.isFunction( qualifier ) ) {
		return jQuery.grep( elements, function( elem, i ) {
			/* jshint -W018 */
			return !!qualifier.call( elem, i, elem ) !== not;
		});

	}

	if ( qualifier.nodeType ) {
		return jQuery.grep( elements, function( elem ) {
			return ( elem === qualifier ) !== not;
		});

	}

	if ( typeof qualifier === "string" ) {
		if ( risSimple.test( qualifier ) ) {
			return jQuery.filter( qualifier, elements, not );
		}

		qualifier = jQuery.filter( qualifier, elements );
	}

	return jQuery.grep( elements, function( elem ) {
		return ( indexOf.call( qualifier, elem ) >= 0 ) !== not;
	});
}

jQuery.filter = function( expr, elems, not ) {
	var elem = elems[ 0 ];

	if ( not ) {
		expr = ":not(" + expr + ")";
	}

	return elems.length === 1 && elem.nodeType === 1 ?
		jQuery.find.matchesSelector( elem, expr ) ? [ elem ] : [] :
		jQuery.find.matches( expr, jQuery.grep( elems, function( elem ) {
			return elem.nodeType === 1;
		}));
};

jQuery.fn.extend({
	find: function( selector ) {
		var i,
			len = this.length,
			ret = [],
			self = this;

		if ( typeof selector !== "string" ) {
			return this.pushStack( jQuery( selector ).filter(function() {
				for ( i = 0; i < len; i++ ) {
					if ( jQuery.contains( self[ i ], this ) ) {
						return true;
					}
				}
			}) );
		}

		for ( i = 0; i < len; i++ ) {
			jQuery.find( selector, self[ i ], ret );
		}

		// Needed because $( selector, context ) becomes $( context ).find( selector )
		ret = this.pushStack( len > 1 ? jQuery.unique( ret ) : ret );
		ret.selector = this.selector ? this.selector + " " + selector : selector;
		return ret;
	},
	filter: function( selector ) {
		return this.pushStack( winnow(this, selector || [], false) );
	},
	not: function( selector ) {
		return this.pushStack( winnow(this, selector || [], true) );
	},
	is: function( selector ) {
		return !!winnow(
			this,

			// If this is a positional/relative selector, check membership in the returned set
			// so $("p:first").is("p:last") won't return true for a doc with two "p".
			typeof selector === "string" && rneedsContext.test( selector ) ?
				jQuery( selector ) :
				selector || [],
			false
		).length;
	}
});


// Initialize a jQuery object


// A central reference to the root jQuery(document)
var rootjQuery,

	// A simple way to check for HTML strings
	// Prioritize #id over <tag> to avoid XSS via location.hash (#9521)
	// Strict HTML recognition (#11290: must start with <)
	rquickExpr = /^(?:\s*(<[\w\W]+>)[^>]*|#([\w-]*))$/,

	init = jQuery.fn.init = function( selector, context ) {
		var match, elem;

		// HANDLE: $(""), $(null), $(undefined), $(false)
		if ( !selector ) {
			return this;
		}

		// Handle HTML strings
		if ( typeof selector === "string" ) {
			if ( selector[0] === "<" && selector[ selector.length - 1 ] === ">" && selector.length >= 3 ) {
				// Assume that strings that start and end with <> are HTML and skip the regex check
				match = [ null, selector, null ];

			} else {
				match = rquickExpr.exec( selector );
			}

			// Match html or make sure no context is specified for #id
			if ( match && (match[1] || !context) ) {

				// HANDLE: $(html) -> $(array)
				if ( match[1] ) {
					context = context instanceof jQuery ? context[0] : context;

					// scripts is true for back-compat
					// Intentionally let the error be thrown if parseHTML is not present
					jQuery.merge( this, jQuery.parseHTML(
						match[1],
						context && context.nodeType ? context.ownerDocument || context : document,
						true
					) );

					// HANDLE: $(html, props)
					if ( rsingleTag.test( match[1] ) && jQuery.isPlainObject( context ) ) {
						for ( match in context ) {
							// Properties of context are called as methods if possible
							if ( jQuery.isFunction( this[ match ] ) ) {
								this[ match ]( context[ match ] );

							// ...and otherwise set as attributes
							} else {
								this.attr( match, context[ match ] );
							}
						}
					}

					return this;

				// HANDLE: $(#id)
				} else {
					elem = document.getElementById( match[2] );

					// Check parentNode to catch when Blackberry 4.6 returns
					// nodes that are no longer in the document #6963
					if ( elem && elem.parentNode ) {
						// Inject the element directly into the jQuery object
						this.length = 1;
						this[0] = elem;
					}

					this.context = document;
					this.selector = selector;
					return this;
				}

			// HANDLE: $(expr, $(...))
			} else if ( !context || context.jquery ) {
				return ( context || rootjQuery ).find( selector );

			// HANDLE: $(expr, context)
			// (which is just equivalent to: $(context).find(expr)
			} else {
				return this.constructor( context ).find( selector );
			}

		// HANDLE: $(DOMElement)
		} else if ( selector.nodeType ) {
			this.context = this[0] = selector;
			this.length = 1;
			return this;

		// HANDLE: $(function)
		// Shortcut for document ready
		} else if ( jQuery.isFunction( selector ) ) {
			return typeof rootjQuery.ready !== "undefined" ?
				rootjQuery.ready( selector ) :
				// Execute immediately if ready is not present
				selector( jQuery );
		}

		if ( selector.selector !== undefined ) {
			this.selector = selector.selector;
			this.context = selector.context;
		}

		return jQuery.makeArray( selector, this );
	};

// Give the init function the jQuery prototype for later instantiation
init.prototype = jQuery.fn;

// Initialize central reference
rootjQuery = jQuery( document );


var rparentsprev = /^(?:parents|prev(?:Until|All))/,
	// methods guaranteed to produce a unique set when starting from a unique set
	guaranteedUnique = {
		children: true,
		contents: true,
		next: true,
		prev: true
	};

jQuery.extend({
	dir: function( elem, dir, until ) {
		var matched = [],
			truncate = until !== undefined;

		while ( (elem = elem[ dir ]) && elem.nodeType !== 9 ) {
			if ( elem.nodeType === 1 ) {
				if ( truncate && jQuery( elem ).is( until ) ) {
					break;
				}
				matched.push( elem );
			}
		}
		return matched;
	},

	sibling: function( n, elem ) {
		var matched = [];

		for ( ; n; n = n.nextSibling ) {
			if ( n.nodeType === 1 && n !== elem ) {
				matched.push( n );
			}
		}

		return matched;
	}
});

jQuery.fn.extend({
	has: function( target ) {
		var targets = jQuery( target, this ),
			l = targets.length;

		return this.filter(function() {
			var i = 0;
			for ( ; i < l; i++ ) {
				if ( jQuery.contains( this, targets[i] ) ) {
					return true;
				}
			}
		});
	},

	closest: function( selectors, context ) {
		var cur,
			i = 0,
			l = this.length,
			matched = [],
			pos = rneedsContext.test( selectors ) || typeof selectors !== "string" ?
				jQuery( selectors, context || this.context ) :
				0;

		for ( ; i < l; i++ ) {
			for ( cur = this[i]; cur && cur !== context; cur = cur.parentNode ) {
				// Always skip document fragments
				if ( cur.nodeType < 11 && (pos ?
					pos.index(cur) > -1 :

					// Don't pass non-elements to Sizzle
					cur.nodeType === 1 &&
						jQuery.find.matchesSelector(cur, selectors)) ) {

					matched.push( cur );
					break;
				}
			}
		}

		return this.pushStack( matched.length > 1 ? jQuery.unique( matched ) : matched );
	},

	// Determine the position of an element within
	// the matched set of elements
	index: function( elem ) {

		// No argument, return index in parent
		if ( !elem ) {
			return ( this[ 0 ] && this[ 0 ].parentNode ) ? this.first().prevAll().length : -1;
		}

		// index in selector
		if ( typeof elem === "string" ) {
			return indexOf.call( jQuery( elem ), this[ 0 ] );
		}

		// Locate the position of the desired element
		return indexOf.call( this,

			// If it receives a jQuery object, the first element is used
			elem.jquery ? elem[ 0 ] : elem
		);
	},

	add: function( selector, context ) {
		return this.pushStack(
			jQuery.unique(
				jQuery.merge( this.get(), jQuery( selector, context ) )
			)
		);
	},

	addBack: function( selector ) {
		return this.add( selector == null ?
			this.prevObject : this.prevObject.filter(selector)
		);
	}
});

function sibling( cur, dir ) {
	while ( (cur = cur[dir]) && cur.nodeType !== 1 ) {}
	return cur;
}

jQuery.each({
	parent: function( elem ) {
		var parent = elem.parentNode;
		return parent && parent.nodeType !== 11 ? parent : null;
	},
	parents: function( elem ) {
		return jQuery.dir( elem, "parentNode" );
	},
	parentsUntil: function( elem, i, until ) {
		return jQuery.dir( elem, "parentNode", until );
	},
	next: function( elem ) {
		return sibling( elem, "nextSibling" );
	},
	prev: function( elem ) {
		return sibling( elem, "previousSibling" );
	},
	nextAll: function( elem ) {
		return jQuery.dir( elem, "nextSibling" );
	},
	prevAll: function( elem ) {
		return jQuery.dir( elem, "previousSibling" );
	},
	nextUntil: function( elem, i, until ) {
		return jQuery.dir( elem, "nextSibling", until );
	},
	prevUntil: function( elem, i, until ) {
		return jQuery.dir( elem, "previousSibling", until );
	},
	siblings: function( elem ) {
		return jQuery.sibling( ( elem.parentNode || {} ).firstChild, elem );
	},
	children: function( elem ) {
		return jQuery.sibling( elem.firstChild );
	},
	contents: function( elem ) {
		return elem.contentDocument || jQuery.merge( [], elem.childNodes );
	}
}, function( name, fn ) {
	jQuery.fn[ name ] = function( until, selector ) {
		var matched = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			matched = jQuery.filter( selector, matched );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				jQuery.unique( matched );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				matched.reverse();
			}
		}

		return this.pushStack( matched );
	};
});
var rnotwhite = (/\S+/g);



// String to Object options format cache
var optionsCache = {};

// Convert String-formatted options into Object-formatted ones and store in cache
function createOptions( options ) {
	var object = optionsCache[ options ] = {};
	jQuery.each( options.match( rnotwhite ) || [], function( _, flag ) {
		object[ flag ] = true;
	});
	return object;
}

/*
 * Create a callback list using the following parameters:
 *
 *	options: an optional list of space-separated options that will change how
 *			the callback list behaves or a more traditional option object
 *
 * By default a callback list will act like an event callback list and can be
 * "fired" multiple times.
 *
 * Possible options:
 *
 *	once:			will ensure the callback list can only be fired once (like a Deferred)
 *
 *	memory:			will keep track of previous values and will call any callback added
 *					after the list has been fired right away with the latest "memorized"
 *					values (like a Deferred)
 *
 *	unique:			will ensure a callback can only be added once (no duplicate in the list)
 *
 *	stopOnFalse:	interrupt callings when a callback returns false
 *
 */
jQuery.Callbacks = function( options ) {

	// Convert options from String-formatted to Object-formatted if needed
	// (we check in cache first)
	options = typeof options === "string" ?
		( optionsCache[ options ] || createOptions( options ) ) :
		jQuery.extend( {}, options );

	var // Last fire value (for non-forgettable lists)
		memory,
		// Flag to know if list was already fired
		fired,
		// Flag to know if list is currently firing
		firing,
		// First callback to fire (used internally by add and fireWith)
		firingStart,
		// End of the loop when firing
		firingLength,
		// Index of currently firing callback (modified by remove if needed)
		firingIndex,
		// Actual callback list
		list = [],
		// Stack of fire calls for repeatable lists
		stack = !options.once && [],
		// Fire callbacks
		fire = function( data ) {
			memory = options.memory && data;
			fired = true;
			firingIndex = firingStart || 0;
			firingStart = 0;
			firingLength = list.length;
			firing = true;
			for ( ; list && firingIndex < firingLength; firingIndex++ ) {
				if ( list[ firingIndex ].apply( data[ 0 ], data[ 1 ] ) === false && options.stopOnFalse ) {
					memory = false; // To prevent further calls using add
					break;
				}
			}
			firing = false;
			if ( list ) {
				if ( stack ) {
					if ( stack.length ) {
						fire( stack.shift() );
					}
				} else if ( memory ) {
					list = [];
				} else {
					self.disable();
				}
			}
		},
		// Actual Callbacks object
		self = {
			// Add a callback or a collection of callbacks to the list
			add: function() {
				if ( list ) {
					// First, we save the current length
					var start = list.length;
					(function add( args ) {
						jQuery.each( args, function( _, arg ) {
							var type = jQuery.type( arg );
							if ( type === "function" ) {
								if ( !options.unique || !self.has( arg ) ) {
									list.push( arg );
								}
							} else if ( arg && arg.length && type !== "string" ) {
								// Inspect recursively
								add( arg );
							}
						});
					})( arguments );
					// Do we need to add the callbacks to the
					// current firing batch?
					if ( firing ) {
						firingLength = list.length;
					// With memory, if we're not firing then
					// we should call right away
					} else if ( memory ) {
						firingStart = start;
						fire( memory );
					}
				}
				return this;
			},
			// Remove a callback from the list
			remove: function() {
				if ( list ) {
					jQuery.each( arguments, function( _, arg ) {
						var index;
						while ( ( index = jQuery.inArray( arg, list, index ) ) > -1 ) {
							list.splice( index, 1 );
							// Handle firing indexes
							if ( firing ) {
								if ( index <= firingLength ) {
									firingLength--;
								}
								if ( index <= firingIndex ) {
									firingIndex--;
								}
							}
						}
					});
				}
				return this;
			},
			// Check if a given callback is in the list.
			// If no argument is given, return whether or not list has callbacks attached.
			has: function( fn ) {
				return fn ? jQuery.inArray( fn, list ) > -1 : !!( list && list.length );
			},
			// Remove all callbacks from the list
			empty: function() {
				list = [];
				firingLength = 0;
				return this;
			},
			// Have the list do nothing anymore
			disable: function() {
				list = stack = memory = undefined;
				return this;
			},
			// Is it disabled?
			disabled: function() {
				return !list;
			},
			// Lock the list in its current state
			lock: function() {
				stack = undefined;
				if ( !memory ) {
					self.disable();
				}
				return this;
			},
			// Is it locked?
			locked: function() {
				return !stack;
			},
			// Call all callbacks with the given context and arguments
			fireWith: function( context, args ) {
				if ( list && ( !fired || stack ) ) {
					args = args || [];
					args = [ context, args.slice ? args.slice() : args ];
					if ( firing ) {
						stack.push( args );
					} else {
						fire( args );
					}
				}
				return this;
			},
			// Call all the callbacks with the given arguments
			fire: function() {
				self.fireWith( this, arguments );
				return this;
			},
			// To know if the callbacks have already been called at least once
			fired: function() {
				return !!fired;
			}
		};

	return self;
};


jQuery.extend({

	Deferred: function( func ) {
		var tuples = [
				// action, add listener, listener list, final state
				[ "resolve", "done", jQuery.Callbacks("once memory"), "resolved" ],
				[ "reject", "fail", jQuery.Callbacks("once memory"), "rejected" ],
				[ "notify", "progress", jQuery.Callbacks("memory") ]
			],
			state = "pending",
			promise = {
				state: function() {
					return state;
				},
				always: function() {
					deferred.done( arguments ).fail( arguments );
					return this;
				},
				then: function( /* fnDone, fnFail, fnProgress */ ) {
					var fns = arguments;
					return jQuery.Deferred(function( newDefer ) {
						jQuery.each( tuples, function( i, tuple ) {
							var fn = jQuery.isFunction( fns[ i ] ) && fns[ i ];
							// deferred[ done | fail | progress ] for forwarding actions to newDefer
							deferred[ tuple[1] ](function() {
								var returned = fn && fn.apply( this, arguments );
								if ( returned && jQuery.isFunction( returned.promise ) ) {
									returned.promise()
										.done( newDefer.resolve )
										.fail( newDefer.reject )
										.progress( newDefer.notify );
								} else {
									newDefer[ tuple[ 0 ] + "With" ]( this === promise ? newDefer.promise() : this, fn ? [ returned ] : arguments );
								}
							});
						});
						fns = null;
					}).promise();
				},
				// Get a promise for this deferred
				// If obj is provided, the promise aspect is added to the object
				promise: function( obj ) {
					return obj != null ? jQuery.extend( obj, promise ) : promise;
				}
			},
			deferred = {};

		// Keep pipe for back-compat
		promise.pipe = promise.then;

		// Add list-specific methods
		jQuery.each( tuples, function( i, tuple ) {
			var list = tuple[ 2 ],
				stateString = tuple[ 3 ];

			// promise[ done | fail | progress ] = list.add
			promise[ tuple[1] ] = list.add;

			// Handle state
			if ( stateString ) {
				list.add(function() {
					// state = [ resolved | rejected ]
					state = stateString;

				// [ reject_list | resolve_list ].disable; progress_list.lock
				}, tuples[ i ^ 1 ][ 2 ].disable, tuples[ 2 ][ 2 ].lock );
			}

			// deferred[ resolve | reject | notify ]
			deferred[ tuple[0] ] = function() {
				deferred[ tuple[0] + "With" ]( this === deferred ? promise : this, arguments );
				return this;
			};
			deferred[ tuple[0] + "With" ] = list.fireWith;
		});

		// Make the deferred a promise
		promise.promise( deferred );

		// Call given func if any
		if ( func ) {
			func.call( deferred, deferred );
		}

		// All done!
		return deferred;
	},

	// Deferred helper
	when: function( subordinate /* , ..., subordinateN */ ) {
		var i = 0,
			resolveValues = slice.call( arguments ),
			length = resolveValues.length,

			// the count of uncompleted subordinates
			remaining = length !== 1 || ( subordinate && jQuery.isFunction( subordinate.promise ) ) ? length : 0,

			// the master Deferred. If resolveValues consist of only a single Deferred, just use that.
			deferred = remaining === 1 ? subordinate : jQuery.Deferred(),

			// Update function for both resolve and progress values
			updateFunc = function( i, contexts, values ) {
				return function( value ) {
					contexts[ i ] = this;
					values[ i ] = arguments.length > 1 ? slice.call( arguments ) : value;
					if ( values === progressValues ) {
						deferred.notifyWith( contexts, values );
					} else if ( !( --remaining ) ) {
						deferred.resolveWith( contexts, values );
					}
				};
			},

			progressValues, progressContexts, resolveContexts;

		// add listeners to Deferred subordinates; treat others as resolved
		if ( length > 1 ) {
			progressValues = new Array( length );
			progressContexts = new Array( length );
			resolveContexts = new Array( length );
			for ( ; i < length; i++ ) {
				if ( resolveValues[ i ] && jQuery.isFunction( resolveValues[ i ].promise ) ) {
					resolveValues[ i ].promise()
						.done( updateFunc( i, resolveContexts, resolveValues ) )
						.fail( deferred.reject )
						.progress( updateFunc( i, progressContexts, progressValues ) );
				} else {
					--remaining;
				}
			}
		}

		// if we're not waiting on anything, resolve the master
		if ( !remaining ) {
			deferred.resolveWith( resolveContexts, resolveValues );
		}

		return deferred.promise();
	}
});


// The deferred used on DOM ready
var readyList;

jQuery.fn.ready = function( fn ) {
	// Add the callback
	jQuery.ready.promise().done( fn );

	return this;
};

jQuery.extend({
	// Is the DOM ready to be used? Set to true once it occurs.
	isReady: false,

	// A counter to track how many items to wait for before
	// the ready event fires. See #6781
	readyWait: 1,

	// Hold (or release) the ready event
	holdReady: function( hold ) {
		if ( hold ) {
			jQuery.readyWait++;
		} else {
			jQuery.ready( true );
		}
	},

	// Handle when the DOM is ready
	ready: function( wait ) {

		// Abort if there are pending holds or we're already ready
		if ( wait === true ? --jQuery.readyWait : jQuery.isReady ) {
			return;
		}

		// Remember that the DOM is ready
		jQuery.isReady = true;

		// If a normal DOM Ready event fired, decrement, and wait if need be
		if ( wait !== true && --jQuery.readyWait > 0 ) {
			return;
		}

		// If there are functions bound, to execute
		readyList.resolveWith( document, [ jQuery ] );

		// Trigger any bound ready events
		if ( jQuery.fn.triggerHandler ) {
			jQuery( document ).triggerHandler( "ready" );
			jQuery( document ).off( "ready" );
		}
	}
});

/**
 * The ready event handler and self cleanup method
 */
function completed() {
	document.removeEventListener( "DOMContentLoaded", completed, false );
	window.removeEventListener( "load", completed, false );
	jQuery.ready();
}

jQuery.ready.promise = function( obj ) {
	if ( !readyList ) {

		readyList = jQuery.Deferred();

		// Catch cases where $(document).ready() is called after the browser event has already occurred.
		// we once tried to use readyState "interactive" here, but it caused issues like the one
		// discovered by ChrisS here: http://bugs.jquery.com/ticket/12282#comment:15
		if ( document.readyState === "complete" ) {
			// Handle it asynchronously to allow scripts the opportunity to delay ready
			setTimeout( jQuery.ready );

		} else {

			// Use the handy event callback
			document.addEventListener( "DOMContentLoaded", completed, false );

			// A fallback to window.onload, that will always work
			window.addEventListener( "load", completed, false );
		}
	}
	return readyList.promise( obj );
};

// Kick off the DOM ready check even if the user does not
jQuery.ready.promise();




// Multifunctional method to get and set values of a collection
// The value/s can optionally be executed if it's a function
var access = jQuery.access = function( elems, fn, key, value, chainable, emptyGet, raw ) {
	var i = 0,
		len = elems.length,
		bulk = key == null;

	// Sets many values
	if ( jQuery.type( key ) === "object" ) {
		chainable = true;
		for ( i in key ) {
			jQuery.access( elems, fn, i, key[i], true, emptyGet, raw );
		}

	// Sets one value
	} else if ( value !== undefined ) {
		chainable = true;

		if ( !jQuery.isFunction( value ) ) {
			raw = true;
		}

		if ( bulk ) {
			// Bulk operations run against the entire set
			if ( raw ) {
				fn.call( elems, value );
				fn = null;

			// ...except when executing function values
			} else {
				bulk = fn;
				fn = function( elem, key, value ) {
					return bulk.call( jQuery( elem ), value );
				};
			}
		}

		if ( fn ) {
			for ( ; i < len; i++ ) {
				fn( elems[i], key, raw ? value : value.call( elems[i], i, fn( elems[i], key ) ) );
			}
		}
	}

	return chainable ?
		elems :

		// Gets
		bulk ?
			fn.call( elems ) :
			len ? fn( elems[0], key ) : emptyGet;
};


/**
 * Determines whether an object can have data
 */
jQuery.acceptData = function( owner ) {
	// Accepts only:
	//  - Node
	//    - Node.ELEMENT_NODE
	//    - Node.DOCUMENT_NODE
	//  - Object
	//    - Any
	/* jshint -W018 */
	return owner.nodeType === 1 || owner.nodeType === 9 || !( +owner.nodeType );
};


function Data() {
	// Support: Android < 4,
	// Old WebKit does not have Object.preventExtensions/freeze method,
	// return new empty object instead with no [[set]] accessor
	Object.defineProperty( this.cache = {}, 0, {
		get: function() {
			return {};
		}
	});

	this.expando = jQuery.expando + Math.random();
}

Data.uid = 1;
Data.accepts = jQuery.acceptData;

Data.prototype = {
	key: function( owner ) {
		// We can accept data for non-element nodes in modern browsers,
		// but we should not, see #8335.
		// Always return the key for a frozen object.
		if ( !Data.accepts( owner ) ) {
			return 0;
		}

		var descriptor = {},
			// Check if the owner object already has a cache key
			unlock = owner[ this.expando ];

		// If not, create one
		if ( !unlock ) {
			unlock = Data.uid++;

			// Secure it in a non-enumerable, non-writable property
			try {
				descriptor[ this.expando ] = { value: unlock };
				Object.defineProperties( owner, descriptor );

			// Support: Android < 4
			// Fallback to a less secure definition
			} catch ( e ) {
				descriptor[ this.expando ] = unlock;
				jQuery.extend( owner, descriptor );
			}
		}

		// Ensure the cache object
		if ( !this.cache[ unlock ] ) {
			this.cache[ unlock ] = {};
		}

		return unlock;
	},
	set: function( owner, data, value ) {
		var prop,
			// There may be an unlock assigned to this node,
			// if there is no entry for this "owner", create one inline
			// and set the unlock as though an owner entry had always existed
			unlock = this.key( owner ),
			cache = this.cache[ unlock ];

		// Handle: [ owner, key, value ] args
		if ( typeof data === "string" ) {
			cache[ data ] = value;

		// Handle: [ owner, { properties } ] args
		} else {
			// Fresh assignments by object are shallow copied
			if ( jQuery.isEmptyObject( cache ) ) {
				jQuery.extend( this.cache[ unlock ], data );
			// Otherwise, copy the properties one-by-one to the cache object
			} else {
				for ( prop in data ) {
					cache[ prop ] = data[ prop ];
				}
			}
		}
		return cache;
	},
	get: function( owner, key ) {
		// Either a valid cache is found, or will be created.
		// New caches will be created and the unlock returned,
		// allowing direct access to the newly created
		// empty data object. A valid owner object must be provided.
		var cache = this.cache[ this.key( owner ) ];

		return key === undefined ?
			cache : cache[ key ];
	},
	access: function( owner, key, value ) {
		var stored;
		// In cases where either:
		//
		//   1. No key was specified
		//   2. A string key was specified, but no value provided
		//
		// Take the "read" path and allow the get method to determine
		// which value to return, respectively either:
		//
		//   1. The entire cache object
		//   2. The data stored at the key
		//
		if ( key === undefined ||
				((key && typeof key === "string") && value === undefined) ) {

			stored = this.get( owner, key );

			return stored !== undefined ?
				stored : this.get( owner, jQuery.camelCase(key) );
		}

		// [*]When the key is not a string, or both a key and value
		// are specified, set or extend (existing objects) with either:
		//
		//   1. An object of properties
		//   2. A key and value
		//
		this.set( owner, key, value );

		// Since the "set" path can have two possible entry points
		// return the expected data based on which path was taken[*]
		return value !== undefined ? value : key;
	},
	remove: function( owner, key ) {
		var i, name, camel,
			unlock = this.key( owner ),
			cache = this.cache[ unlock ];

		if ( key === undefined ) {
			this.cache[ unlock ] = {};

		} else {
			// Support array or space separated string of keys
			if ( jQuery.isArray( key ) ) {
				// If "name" is an array of keys...
				// When data is initially created, via ("key", "val") signature,
				// keys will be converted to camelCase.
				// Since there is no way to tell _how_ a key was added, remove
				// both plain key and camelCase key. #12786
				// This will only penalize the array argument path.
				name = key.concat( key.map( jQuery.camelCase ) );
			} else {
				camel = jQuery.camelCase( key );
				// Try the string as a key before any manipulation
				if ( key in cache ) {
					name = [ key, camel ];
				} else {
					// If a key with the spaces exists, use it.
					// Otherwise, create an array by matching non-whitespace
					name = camel;
					name = name in cache ?
						[ name ] : ( name.match( rnotwhite ) || [] );
				}
			}

			i = name.length;
			while ( i-- ) {
				delete cache[ name[ i ] ];
			}
		}
	},
	hasData: function( owner ) {
		return !jQuery.isEmptyObject(
			this.cache[ owner[ this.expando ] ] || {}
		);
	},
	discard: function( owner ) {
		if ( owner[ this.expando ] ) {
			delete this.cache[ owner[ this.expando ] ];
		}
	}
};
var data_priv = new Data();

var data_user = new Data();



/*
	Implementation Summary

	1. Enforce API surface and semantic compatibility with 1.9.x branch
	2. Improve the module's maintainability by reducing the storage
		paths to a single mechanism.
	3. Use the same single mechanism to support "private" and "user" data.
	4. _Never_ expose "private" data to user code (TODO: Drop _data, _removeData)
	5. Avoid exposing implementation details on user objects (eg. expando properties)
	6. Provide a clear path for implementation upgrade to WeakMap in 2014
*/
var rbrace = /^(?:\{[\w\W]*\}|\[[\w\W]*\])$/,
	rmultiDash = /([A-Z])/g;

function dataAttr( elem, key, data ) {
	var name;

	// If nothing was found internally, try to fetch any
	// data from the HTML5 data-* attribute
	if ( data === undefined && elem.nodeType === 1 ) {
		name = "data-" + key.replace( rmultiDash, "-$1" ).toLowerCase();
		data = elem.getAttribute( name );

		if ( typeof data === "string" ) {
			try {
				data = data === "true" ? true :
					data === "false" ? false :
					data === "null" ? null :
					// Only convert to a number if it doesn't change the string
					+data + "" === data ? +data :
					rbrace.test( data ) ? jQuery.parseJSON( data ) :
					data;
			} catch( e ) {}

			// Make sure we set the data so it isn't changed later
			data_user.set( elem, key, data );
		} else {
			data = undefined;
		}
	}
	return data;
}

jQuery.extend({
	hasData: function( elem ) {
		return data_user.hasData( elem ) || data_priv.hasData( elem );
	},

	data: function( elem, name, data ) {
		return data_user.access( elem, name, data );
	},

	removeData: function( elem, name ) {
		data_user.remove( elem, name );
	},

	// TODO: Now that all calls to _data and _removeData have been replaced
	// with direct calls to data_priv methods, these can be deprecated.
	_data: function( elem, name, data ) {
		return data_priv.access( elem, name, data );
	},

	_removeData: function( elem, name ) {
		data_priv.remove( elem, name );
	}
});

jQuery.fn.extend({
	data: function( key, value ) {
		var i, name, data,
			elem = this[ 0 ],
			attrs = elem && elem.attributes;

		// Gets all values
		if ( key === undefined ) {
			if ( this.length ) {
				data = data_user.get( elem );

				if ( elem.nodeType === 1 && !data_priv.get( elem, "hasDataAttrs" ) ) {
					i = attrs.length;
					while ( i-- ) {

						// Support: IE11+
						// The attrs elements can be null (#14894)
						if ( attrs[ i ] ) {
							name = attrs[ i ].name;
							if ( name.indexOf( "data-" ) === 0 ) {
								name = jQuery.camelCase( name.slice(5) );
								dataAttr( elem, name, data[ name ] );
							}
						}
					}
					data_priv.set( elem, "hasDataAttrs", true );
				}
			}

			return data;
		}

		// Sets multiple values
		if ( typeof key === "object" ) {
			return this.each(function() {
				data_user.set( this, key );
			});
		}

		return access( this, function( value ) {
			var data,
				camelKey = jQuery.camelCase( key );

			// The calling jQuery object (element matches) is not empty
			// (and therefore has an element appears at this[ 0 ]) and the
			// `value` parameter was not undefined. An empty jQuery object
			// will result in `undefined` for elem = this[ 0 ] which will
			// throw an exception if an attempt to read a data cache is made.
			if ( elem && value === undefined ) {
				// Attempt to get data from the cache
				// with the key as-is
				data = data_user.get( elem, key );
				if ( data !== undefined ) {
					return data;
				}

				// Attempt to get data from the cache
				// with the key camelized
				data = data_user.get( elem, camelKey );
				if ( data !== undefined ) {
					return data;
				}

				// Attempt to "discover" the data in
				// HTML5 custom data-* attrs
				data = dataAttr( elem, camelKey, undefined );
				if ( data !== undefined ) {
					return data;
				}

				// We tried really hard, but the data doesn't exist.
				return;
			}

			// Set the data...
			this.each(function() {
				// First, attempt to store a copy or reference of any
				// data that might've been store with a camelCased key.
				var data = data_user.get( this, camelKey );

				// For HTML5 data-* attribute interop, we have to
				// store property names with dashes in a camelCase form.
				// This might not apply to all properties...*
				data_user.set( this, camelKey, value );

				// *... In the case of properties that might _actually_
				// have dashes, we need to also store a copy of that
				// unchanged property.
				if ( key.indexOf("-") !== -1 && data !== undefined ) {
					data_user.set( this, key, value );
				}
			});
		}, null, value, arguments.length > 1, null, true );
	},

	removeData: function( key ) {
		return this.each(function() {
			data_user.remove( this, key );
		});
	}
});


jQuery.extend({
	queue: function( elem, type, data ) {
		var queue;

		if ( elem ) {
			type = ( type || "fx" ) + "queue";
			queue = data_priv.get( elem, type );

			// Speed up dequeue by getting out quickly if this is just a lookup
			if ( data ) {
				if ( !queue || jQuery.isArray( data ) ) {
					queue = data_priv.access( elem, type, jQuery.makeArray(data) );
				} else {
					queue.push( data );
				}
			}
			return queue || [];
		}
	},

	dequeue: function( elem, type ) {
		type = type || "fx";

		var queue = jQuery.queue( elem, type ),
			startLength = queue.length,
			fn = queue.shift(),
			hooks = jQuery._queueHooks( elem, type ),
			next = function() {
				jQuery.dequeue( elem, type );
			};

		// If the fx queue is dequeued, always remove the progress sentinel
		if ( fn === "inprogress" ) {
			fn = queue.shift();
			startLength--;
		}

		if ( fn ) {

			// Add a progress sentinel to prevent the fx queue from being
			// automatically dequeued
			if ( type === "fx" ) {
				queue.unshift( "inprogress" );
			}

			// clear up the last queue stop function
			delete hooks.stop;
			fn.call( elem, next, hooks );
		}

		if ( !startLength && hooks ) {
			hooks.empty.fire();
		}
	},

	// not intended for public consumption - generates a queueHooks object, or returns the current one
	_queueHooks: function( elem, type ) {
		var key = type + "queueHooks";
		return data_priv.get( elem, key ) || data_priv.access( elem, key, {
			empty: jQuery.Callbacks("once memory").add(function() {
				data_priv.remove( elem, [ type + "queue", key ] );
			})
		});
	}
});

jQuery.fn.extend({
	queue: function( type, data ) {
		var setter = 2;

		if ( typeof type !== "string" ) {
			data = type;
			type = "fx";
			setter--;
		}

		if ( arguments.length < setter ) {
			return jQuery.queue( this[0], type );
		}

		return data === undefined ?
			this :
			this.each(function() {
				var queue = jQuery.queue( this, type, data );

				// ensure a hooks for this queue
				jQuery._queueHooks( this, type );

				if ( type === "fx" && queue[0] !== "inprogress" ) {
					jQuery.dequeue( this, type );
				}
			});
	},
	dequeue: function( type ) {
		return this.each(function() {
			jQuery.dequeue( this, type );
		});
	},
	clearQueue: function( type ) {
		return this.queue( type || "fx", [] );
	},
	// Get a promise resolved when queues of a certain type
	// are emptied (fx is the type by default)
	promise: function( type, obj ) {
		var tmp,
			count = 1,
			defer = jQuery.Deferred(),
			elements = this,
			i = this.length,
			resolve = function() {
				if ( !( --count ) ) {
					defer.resolveWith( elements, [ elements ] );
				}
			};

		if ( typeof type !== "string" ) {
			obj = type;
			type = undefined;
		}
		type = type || "fx";

		while ( i-- ) {
			tmp = data_priv.get( elements[ i ], type + "queueHooks" );
			if ( tmp && tmp.empty ) {
				count++;
				tmp.empty.add( resolve );
			}
		}
		resolve();
		return defer.promise( obj );
	}
});
var pnum = (/[+-]?(?:\d*\.|)\d+(?:[eE][+-]?\d+|)/).source;

var cssExpand = [ "Top", "Right", "Bottom", "Left" ];

var isHidden = function( elem, el ) {
		// isHidden might be called from jQuery#filter function;
		// in that case, element will be second argument
		elem = el || elem;
		return jQuery.css( elem, "display" ) === "none" || !jQuery.contains( elem.ownerDocument, elem );
	};

var rcheckableType = (/^(?:checkbox|radio)$/i);



(function() {
	var fragment = document.createDocumentFragment(),
		div = fragment.appendChild( document.createElement( "div" ) ),
		input = document.createElement( "input" );

	// #11217 - WebKit loses check when the name is after the checked attribute
	// Support: Windows Web Apps (WWA)
	// `name` and `type` need .setAttribute for WWA
	input.setAttribute( "type", "radio" );
	input.setAttribute( "checked", "checked" );
	input.setAttribute( "name", "t" );

	div.appendChild( input );

	// Support: Safari 5.1, iOS 5.1, Android 4.x, Android 2.3
	// old WebKit doesn't clone checked state correctly in fragments
	support.checkClone = div.cloneNode( true ).cloneNode( true ).lastChild.checked;

	// Make sure textarea (and checkbox) defaultValue is properly cloned
	// Support: IE9-IE11+
	div.innerHTML = "<textarea>x</textarea>";
	support.noCloneChecked = !!div.cloneNode( true ).lastChild.defaultValue;
})();
var strundefined = typeof undefined;



support.focusinBubbles = "onfocusin" in window;


var
	rkeyEvent = /^key/,
	rmouseEvent = /^(?:mouse|pointer|contextmenu)|click/,
	rfocusMorph = /^(?:focusinfocus|focusoutblur)$/,
	rtypenamespace = /^([^.]*)(?:\.(.+)|)$/;

function returnTrue() {
	return true;
}

function returnFalse() {
	return false;
}

function safeActiveElement() {
	try {
		return document.activeElement;
	} catch ( err ) { }
}

/*
 * Helper functions for managing events -- not part of the public interface.
 * Props to Dean Edwards' addEvent library for many of the ideas.
 */
jQuery.event = {

	global: {},

	add: function( elem, types, handler, data, selector ) {

		var handleObjIn, eventHandle, tmp,
			events, t, handleObj,
			special, handlers, type, namespaces, origType,
			elemData = data_priv.get( elem );

		// Don't attach events to noData or text/comment nodes (but allow plain objects)
		if ( !elemData ) {
			return;
		}

		// Caller can pass in an object of custom data in lieu of the handler
		if ( handler.handler ) {
			handleObjIn = handler;
			handler = handleObjIn.handler;
			selector = handleObjIn.selector;
		}

		// Make sure that the handler has a unique ID, used to find/remove it later
		if ( !handler.guid ) {
			handler.guid = jQuery.guid++;
		}

		// Init the element's event structure and main handler, if this is the first
		if ( !(events = elemData.events) ) {
			events = elemData.events = {};
		}
		if ( !(eventHandle = elemData.handle) ) {
			eventHandle = elemData.handle = function( e ) {
				// Discard the second event of a jQuery.event.trigger() and
				// when an event is called after a page has unloaded
				return typeof jQuery !== strundefined && jQuery.event.triggered !== e.type ?
					jQuery.event.dispatch.apply( elem, arguments ) : undefined;
			};
		}

		// Handle multiple events separated by a space
		types = ( types || "" ).match( rnotwhite ) || [ "" ];
		t = types.length;
		while ( t-- ) {
			tmp = rtypenamespace.exec( types[t] ) || [];
			type = origType = tmp[1];
			namespaces = ( tmp[2] || "" ).split( "." ).sort();

			// There *must* be a type, no attaching namespace-only handlers
			if ( !type ) {
				continue;
			}

			// If event changes its type, use the special event handlers for the changed type
			special = jQuery.event.special[ type ] || {};

			// If selector defined, determine special event api type, otherwise given type
			type = ( selector ? special.delegateType : special.bindType ) || type;

			// Update special based on newly reset type
			special = jQuery.event.special[ type ] || {};

			// handleObj is passed to all event handlers
			handleObj = jQuery.extend({
				type: type,
				origType: origType,
				data: data,
				handler: handler,
				guid: handler.guid,
				selector: selector,
				needsContext: selector && jQuery.expr.match.needsContext.test( selector ),
				namespace: namespaces.join(".")
			}, handleObjIn );

			// Init the event handler queue if we're the first
			if ( !(handlers = events[ type ]) ) {
				handlers = events[ type ] = [];
				handlers.delegateCount = 0;

				// Only use addEventListener if the special events handler returns false
				if ( !special.setup || special.setup.call( elem, data, namespaces, eventHandle ) === false ) {
					if ( elem.addEventListener ) {
						elem.addEventListener( type, eventHandle, false );
					}
				}
			}

			if ( special.add ) {
				special.add.call( elem, handleObj );

				if ( !handleObj.handler.guid ) {
					handleObj.handler.guid = handler.guid;
				}
			}

			// Add to the element's handler list, delegates in front
			if ( selector ) {
				handlers.splice( handlers.delegateCount++, 0, handleObj );
			} else {
				handlers.push( handleObj );
			}

			// Keep track of which events have ever been used, for event optimization
			jQuery.event.global[ type ] = true;
		}

	},

	// Detach an event or set of events from an element
	remove: function( elem, types, handler, selector, mappedTypes ) {

		var j, origCount, tmp,
			events, t, handleObj,
			special, handlers, type, namespaces, origType,
			elemData = data_priv.hasData( elem ) && data_priv.get( elem );

		if ( !elemData || !(events = elemData.events) ) {
			return;
		}

		// Once for each type.namespace in types; type may be omitted
		types = ( types || "" ).match( rnotwhite ) || [ "" ];
		t = types.length;
		while ( t-- ) {
			tmp = rtypenamespace.exec( types[t] ) || [];
			type = origType = tmp[1];
			namespaces = ( tmp[2] || "" ).split( "." ).sort();

			// Unbind all events (on this namespace, if provided) for the element
			if ( !type ) {
				for ( type in events ) {
					jQuery.event.remove( elem, type + types[ t ], handler, selector, true );
				}
				continue;
			}

			special = jQuery.event.special[ type ] || {};
			type = ( selector ? special.delegateType : special.bindType ) || type;
			handlers = events[ type ] || [];
			tmp = tmp[2] && new RegExp( "(^|\\.)" + namespaces.join("\\.(?:.*\\.|)") + "(\\.|$)" );

			// Remove matching events
			origCount = j = handlers.length;
			while ( j-- ) {
				handleObj = handlers[ j ];

				if ( ( mappedTypes || origType === handleObj.origType ) &&
					( !handler || handler.guid === handleObj.guid ) &&
					( !tmp || tmp.test( handleObj.namespace ) ) &&
					( !selector || selector === handleObj.selector || selector === "**" && handleObj.selector ) ) {
					handlers.splice( j, 1 );

					if ( handleObj.selector ) {
						handlers.delegateCount--;
					}
					if ( special.remove ) {
						special.remove.call( elem, handleObj );
					}
				}
			}

			// Remove generic event handler if we removed something and no more handlers exist
			// (avoids potential for endless recursion during removal of special event handlers)
			if ( origCount && !handlers.length ) {
				if ( !special.teardown || special.teardown.call( elem, namespaces, elemData.handle ) === false ) {
					jQuery.removeEvent( elem, type, elemData.handle );
				}

				delete events[ type ];
			}
		}

		// Remove the expando if it's no longer used
		if ( jQuery.isEmptyObject( events ) ) {
			delete elemData.handle;
			data_priv.remove( elem, "events" );
		}
	},

	trigger: function( event, data, elem, onlyHandlers ) {

		var i, cur, tmp, bubbleType, ontype, handle, special,
			eventPath = [ elem || document ],
			type = hasOwn.call( event, "type" ) ? event.type : event,
			namespaces = hasOwn.call( event, "namespace" ) ? event.namespace.split(".") : [];

		cur = tmp = elem = elem || document;

		// Don't do events on text and comment nodes
		if ( elem.nodeType === 3 || elem.nodeType === 8 ) {
			return;
		}

		// focus/blur morphs to focusin/out; ensure we're not firing them right now
		if ( rfocusMorph.test( type + jQuery.event.triggered ) ) {
			return;
		}

		if ( type.indexOf(".") >= 0 ) {
			// Namespaced trigger; create a regexp to match event type in handle()
			namespaces = type.split(".");
			type = namespaces.shift();
			namespaces.sort();
		}
		ontype = type.indexOf(":") < 0 && "on" + type;

		// Caller can pass in a jQuery.Event object, Object, or just an event type string
		event = event[ jQuery.expando ] ?
			event :
			new jQuery.Event( type, typeof event === "object" && event );

		// Trigger bitmask: & 1 for native handlers; & 2 for jQuery (always true)
		event.isTrigger = onlyHandlers ? 2 : 3;
		event.namespace = namespaces.join(".");
		event.namespace_re = event.namespace ?
			new RegExp( "(^|\\.)" + namespaces.join("\\.(?:.*\\.|)") + "(\\.|$)" ) :
			null;

		// Clean up the event in case it is being reused
		event.result = undefined;
		if ( !event.target ) {
			event.target = elem;
		}

		// Clone any incoming data and prepend the event, creating the handler arg list
		data = data == null ?
			[ event ] :
			jQuery.makeArray( data, [ event ] );

		// Allow special events to draw outside the lines
		special = jQuery.event.special[ type ] || {};
		if ( !onlyHandlers && special.trigger && special.trigger.apply( elem, data ) === false ) {
			return;
		}

		// Determine event propagation path in advance, per W3C events spec (#9951)
		// Bubble up to document, then to window; watch for a global ownerDocument var (#9724)
		if ( !onlyHandlers && !special.noBubble && !jQuery.isWindow( elem ) ) {

			bubbleType = special.delegateType || type;
			if ( !rfocusMorph.test( bubbleType + type ) ) {
				cur = cur.parentNode;
			}
			for ( ; cur; cur = cur.parentNode ) {
				eventPath.push( cur );
				tmp = cur;
			}

			// Only add window if we got to document (e.g., not plain obj or detached DOM)
			if ( tmp === (elem.ownerDocument || document) ) {
				eventPath.push( tmp.defaultView || tmp.parentWindow || window );
			}
		}

		// Fire handlers on the event path
		i = 0;
		while ( (cur = eventPath[i++]) && !event.isPropagationStopped() ) {

			event.type = i > 1 ?
				bubbleType :
				special.bindType || type;

			// jQuery handler
			handle = ( data_priv.get( cur, "events" ) || {} )[ event.type ] && data_priv.get( cur, "handle" );
			if ( handle ) {
				handle.apply( cur, data );
			}

			// Native handler
			handle = ontype && cur[ ontype ];
			if ( handle && handle.apply && jQuery.acceptData( cur ) ) {
				event.result = handle.apply( cur, data );
				if ( event.result === false ) {
					event.preventDefault();
				}
			}
		}
		event.type = type;

		// If nobody prevented the default action, do it now
		if ( !onlyHandlers && !event.isDefaultPrevented() ) {

			if ( (!special._default || special._default.apply( eventPath.pop(), data ) === false) &&
				jQuery.acceptData( elem ) ) {

				// Call a native DOM method on the target with the same name name as the event.
				// Don't do default actions on window, that's where global variables be (#6170)
				if ( ontype && jQuery.isFunction( elem[ type ] ) && !jQuery.isWindow( elem ) ) {

					// Don't re-trigger an onFOO event when we call its FOO() method
					tmp = elem[ ontype ];

					if ( tmp ) {
						elem[ ontype ] = null;
					}

					// Prevent re-triggering of the same event, since we already bubbled it above
					jQuery.event.triggered = type;
					elem[ type ]();
					jQuery.event.triggered = undefined;

					if ( tmp ) {
						elem[ ontype ] = tmp;
					}
				}
			}
		}

		return event.result;
	},

	dispatch: function( event ) {

		// Make a writable jQuery.Event from the native event object
		event = jQuery.event.fix( event );

		var i, j, ret, matched, handleObj,
			handlerQueue = [],
			args = slice.call( arguments ),
			handlers = ( data_priv.get( this, "events" ) || {} )[ event.type ] || [],
			special = jQuery.event.special[ event.type ] || {};

		// Use the fix-ed jQuery.Event rather than the (read-only) native event
		args[0] = event;
		event.delegateTarget = this;

		// Call the preDispatch hook for the mapped type, and let it bail if desired
		if ( special.preDispatch && special.preDispatch.call( this, event ) === false ) {
			return;
		}

		// Determine handlers
		handlerQueue = jQuery.event.handlers.call( this, event, handlers );

		// Run delegates first; they may want to stop propagation beneath us
		i = 0;
		while ( (matched = handlerQueue[ i++ ]) && !event.isPropagationStopped() ) {
			event.currentTarget = matched.elem;

			j = 0;
			while ( (handleObj = matched.handlers[ j++ ]) && !event.isImmediatePropagationStopped() ) {

				// Triggered event must either 1) have no namespace, or
				// 2) have namespace(s) a subset or equal to those in the bound event (both can have no namespace).
				if ( !event.namespace_re || event.namespace_re.test( handleObj.namespace ) ) {

					event.handleObj = handleObj;
					event.data = handleObj.data;

					ret = ( (jQuery.event.special[ handleObj.origType ] || {}).handle || handleObj.handler )
							.apply( matched.elem, args );

					if ( ret !== undefined ) {
						if ( (event.result = ret) === false ) {
							event.preventDefault();
							event.stopPropagation();
						}
					}
				}
			}
		}

		// Call the postDispatch hook for the mapped type
		if ( special.postDispatch ) {
			special.postDispatch.call( this, event );
		}

		return event.result;
	},

	handlers: function( event, handlers ) {
		var i, matches, sel, handleObj,
			handlerQueue = [],
			delegateCount = handlers.delegateCount,
			cur = event.target;

		// Find delegate handlers
		// Black-hole SVG <use> instance trees (#13180)
		// Avoid non-left-click bubbling in Firefox (#3861)
		if ( delegateCount && cur.nodeType && (!event.button || event.type !== "click") ) {

			for ( ; cur !== this; cur = cur.parentNode || this ) {

				// Don't process clicks on disabled elements (#6911, #8165, #11382, #11764)
				if ( cur.disabled !== true || event.type !== "click" ) {
					matches = [];
					for ( i = 0; i < delegateCount; i++ ) {
						handleObj = handlers[ i ];

						// Don't conflict with Object.prototype properties (#13203)
						sel = handleObj.selector + " ";

						if ( matches[ sel ] === undefined ) {
							matches[ sel ] = handleObj.needsContext ?
								jQuery( sel, this ).index( cur ) >= 0 :
								jQuery.find( sel, this, null, [ cur ] ).length;
						}
						if ( matches[ sel ] ) {
							matches.push( handleObj );
						}
					}
					if ( matches.length ) {
						handlerQueue.push({ elem: cur, handlers: matches });
					}
				}
			}
		}

		// Add the remaining (directly-bound) handlers
		if ( delegateCount < handlers.length ) {
			handlerQueue.push({ elem: this, handlers: handlers.slice( delegateCount ) });
		}

		return handlerQueue;
	},

	// Includes some event props shared by KeyEvent and MouseEvent
	props: "altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "),

	fixHooks: {},

	keyHooks: {
		props: "char charCode key keyCode".split(" "),
		filter: function( event, original ) {

			// Add which for key events
			if ( event.which == null ) {
				event.which = original.charCode != null ? original.charCode : original.keyCode;
			}

			return event;
		}
	},

	mouseHooks: {
		props: "button buttons clientX clientY offsetX offsetY pageX pageY screenX screenY toElement".split(" "),
		filter: function( event, original ) {
			var eventDoc, doc, body,
				button = original.button;

			// Calculate pageX/Y if missing and clientX/Y available
			if ( event.pageX == null && original.clientX != null ) {
				eventDoc = event.target.ownerDocument || document;
				doc = eventDoc.documentElement;
				body = eventDoc.body;

				event.pageX = original.clientX + ( doc && doc.scrollLeft || body && body.scrollLeft || 0 ) - ( doc && doc.clientLeft || body && body.clientLeft || 0 );
				event.pageY = original.clientY + ( doc && doc.scrollTop  || body && body.scrollTop  || 0 ) - ( doc && doc.clientTop  || body && body.clientTop  || 0 );
			}

			// Add which for click: 1 === left; 2 === middle; 3 === right
			// Note: button is not normalized, so don't use it
			if ( !event.which && button !== undefined ) {
				event.which = ( button & 1 ? 1 : ( button & 2 ? 3 : ( button & 4 ? 2 : 0 ) ) );
			}

			return event;
		}
	},

	fix: function( event ) {
		if ( event[ jQuery.expando ] ) {
			return event;
		}

		// Create a writable copy of the event object and normalize some properties
		var i, prop, copy,
			type = event.type,
			originalEvent = event,
			fixHook = this.fixHooks[ type ];

		if ( !fixHook ) {
			this.fixHooks[ type ] = fixHook =
				rmouseEvent.test( type ) ? this.mouseHooks :
				rkeyEvent.test( type ) ? this.keyHooks :
				{};
		}
		copy = fixHook.props ? this.props.concat( fixHook.props ) : this.props;

		event = new jQuery.Event( originalEvent );

		i = copy.length;
		while ( i-- ) {
			prop = copy[ i ];
			event[ prop ] = originalEvent[ prop ];
		}

		// Support: Cordova 2.5 (WebKit) (#13255)
		// All events should have a target; Cordova deviceready doesn't
		if ( !event.target ) {
			event.target = document;
		}

		// Support: Safari 6.0+, Chrome < 28
		// Target should not be a text node (#504, #13143)
		if ( event.target.nodeType === 3 ) {
			event.target = event.target.parentNode;
		}

		return fixHook.filter ? fixHook.filter( event, originalEvent ) : event;
	},

	special: {
		load: {
			// Prevent triggered image.load events from bubbling to window.load
			noBubble: true
		},
		focus: {
			// Fire native event if possible so blur/focus sequence is correct
			trigger: function() {
				if ( this !== safeActiveElement() && this.focus ) {
					this.focus();
					return false;
				}
			},
			delegateType: "focusin"
		},
		blur: {
			trigger: function() {
				if ( this === safeActiveElement() && this.blur ) {
					this.blur();
					return false;
				}
			},
			delegateType: "focusout"
		},
		click: {
			// For checkbox, fire native event so checked state will be right
			trigger: function() {
				if ( this.type === "checkbox" && this.click && jQuery.nodeName( this, "input" ) ) {
					this.click();
					return false;
				}
			},

			// For cross-browser consistency, don't fire native .click() on links
			_default: function( event ) {
				return jQuery.nodeName( event.target, "a" );
			}
		},

		beforeunload: {
			postDispatch: function( event ) {

				// Support: Firefox 20+
				// Firefox doesn't alert if the returnValue field is not set.
				if ( event.result !== undefined && event.originalEvent ) {
					event.originalEvent.returnValue = event.result;
				}
			}
		}
	},

	simulate: function( type, elem, event, bubble ) {
		// Piggyback on a donor event to simulate a different one.
		// Fake originalEvent to avoid donor's stopPropagation, but if the
		// simulated event prevents default then we do the same on the donor.
		var e = jQuery.extend(
			new jQuery.Event(),
			event,
			{
				type: type,
				isSimulated: true,
				originalEvent: {}
			}
		);
		if ( bubble ) {
			jQuery.event.trigger( e, null, elem );
		} else {
			jQuery.event.dispatch.call( elem, e );
		}
		if ( e.isDefaultPrevented() ) {
			event.preventDefault();
		}
	}
};

jQuery.removeEvent = function( elem, type, handle ) {
	if ( elem.removeEventListener ) {
		elem.removeEventListener( type, handle, false );
	}
};

jQuery.Event = function( src, props ) {
	// Allow instantiation without the 'new' keyword
	if ( !(this instanceof jQuery.Event) ) {
		return new jQuery.Event( src, props );
	}

	// Event object
	if ( src && src.type ) {
		this.originalEvent = src;
		this.type = src.type;

		// Events bubbling up the document may have been marked as prevented
		// by a handler lower down the tree; reflect the correct value.
		this.isDefaultPrevented = src.defaultPrevented ||
				src.defaultPrevented === undefined &&
				// Support: Android < 4.0
				src.returnValue === false ?
			returnTrue :
			returnFalse;

	// Event type
	} else {
		this.type = src;
	}

	// Put explicitly provided properties onto the event object
	if ( props ) {
		jQuery.extend( this, props );
	}

	// Create a timestamp if incoming event doesn't have one
	this.timeStamp = src && src.timeStamp || jQuery.now();

	// Mark it as fixed
	this[ jQuery.expando ] = true;
};

// jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
// http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
jQuery.Event.prototype = {
	isDefaultPrevented: returnFalse,
	isPropagationStopped: returnFalse,
	isImmediatePropagationStopped: returnFalse,

	preventDefault: function() {
		var e = this.originalEvent;

		this.isDefaultPrevented = returnTrue;

		if ( e && e.preventDefault ) {
			e.preventDefault();
		}
	},
	stopPropagation: function() {
		var e = this.originalEvent;

		this.isPropagationStopped = returnTrue;

		if ( e && e.stopPropagation ) {
			e.stopPropagation();
		}
	},
	stopImmediatePropagation: function() {
		var e = this.originalEvent;

		this.isImmediatePropagationStopped = returnTrue;

		if ( e && e.stopImmediatePropagation ) {
			e.stopImmediatePropagation();
		}

		this.stopPropagation();
	}
};

// Create mouseenter/leave events using mouseover/out and event-time checks
// Support: Chrome 15+
jQuery.each({
	mouseenter: "mouseover",
	mouseleave: "mouseout",
	pointerenter: "pointerover",
	pointerleave: "pointerout"
}, function( orig, fix ) {
	jQuery.event.special[ orig ] = {
		delegateType: fix,
		bindType: fix,

		handle: function( event ) {
			var ret,
				target = this,
				related = event.relatedTarget,
				handleObj = event.handleObj;

			// For mousenter/leave call the handler if related is outside the target.
			// NB: No relatedTarget if the mouse left/entered the browser window
			if ( !related || (related !== target && !jQuery.contains( target, related )) ) {
				event.type = handleObj.origType;
				ret = handleObj.handler.apply( this, arguments );
				event.type = fix;
			}
			return ret;
		}
	};
});

// Create "bubbling" focus and blur events
// Support: Firefox, Chrome, Safari
if ( !support.focusinBubbles ) {
	jQuery.each({ focus: "focusin", blur: "focusout" }, function( orig, fix ) {

		// Attach a single capturing handler on the document while someone wants focusin/focusout
		var handler = function( event ) {
				jQuery.event.simulate( fix, event.target, jQuery.event.fix( event ), true );
			};

		jQuery.event.special[ fix ] = {
			setup: function() {
				var doc = this.ownerDocument || this,
					attaches = data_priv.access( doc, fix );

				if ( !attaches ) {
					doc.addEventListener( orig, handler, true );
				}
				data_priv.access( doc, fix, ( attaches || 0 ) + 1 );
			},
			teardown: function() {
				var doc = this.ownerDocument || this,
					attaches = data_priv.access( doc, fix ) - 1;

				if ( !attaches ) {
					doc.removeEventListener( orig, handler, true );
					data_priv.remove( doc, fix );

				} else {
					data_priv.access( doc, fix, attaches );
				}
			}
		};
	});
}

jQuery.fn.extend({

	on: function( types, selector, data, fn, /*INTERNAL*/ one ) {
		var origFn, type;

		// Types can be a map of types/handlers
		if ( typeof types === "object" ) {
			// ( types-Object, selector, data )
			if ( typeof selector !== "string" ) {
				// ( types-Object, data )
				data = data || selector;
				selector = undefined;
			}
			for ( type in types ) {
				this.on( type, selector, data, types[ type ], one );
			}
			return this;
		}

		if ( data == null && fn == null ) {
			// ( types, fn )
			fn = selector;
			data = selector = undefined;
		} else if ( fn == null ) {
			if ( typeof selector === "string" ) {
				// ( types, selector, fn )
				fn = data;
				data = undefined;
			} else {
				// ( types, data, fn )
				fn = data;
				data = selector;
				selector = undefined;
			}
		}
		if ( fn === false ) {
			fn = returnFalse;
		} else if ( !fn ) {
			return this;
		}

		if ( one === 1 ) {
			origFn = fn;
			fn = function( event ) {
				// Can use an empty set, since event contains the info
				jQuery().off( event );
				return origFn.apply( this, arguments );
			};
			// Use same guid so caller can remove using origFn
			fn.guid = origFn.guid || ( origFn.guid = jQuery.guid++ );
		}
		return this.each( function() {
			jQuery.event.add( this, types, fn, data, selector );
		});
	},
	one: function( types, selector, data, fn ) {
		return this.on( types, selector, data, fn, 1 );
	},
	off: function( types, selector, fn ) {
		var handleObj, type;
		if ( types && types.preventDefault && types.handleObj ) {
			// ( event )  dispatched jQuery.Event
			handleObj = types.handleObj;
			jQuery( types.delegateTarget ).off(
				handleObj.namespace ? handleObj.origType + "." + handleObj.namespace : handleObj.origType,
				handleObj.selector,
				handleObj.handler
			);
			return this;
		}
		if ( typeof types === "object" ) {
			// ( types-object [, selector] )
			for ( type in types ) {
				this.off( type, selector, types[ type ] );
			}
			return this;
		}
		if ( selector === false || typeof selector === "function" ) {
			// ( types [, fn] )
			fn = selector;
			selector = undefined;
		}
		if ( fn === false ) {
			fn = returnFalse;
		}
		return this.each(function() {
			jQuery.event.remove( this, types, fn, selector );
		});
	},

	trigger: function( type, data ) {
		return this.each(function() {
			jQuery.event.trigger( type, data, this );
		});
	},
	triggerHandler: function( type, data ) {
		var elem = this[0];
		if ( elem ) {
			return jQuery.event.trigger( type, data, elem, true );
		}
	}
});


var
	rxhtmlTag = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/gi,
	rtagName = /<([\w:]+)/,
	rhtml = /<|&#?\w+;/,
	rnoInnerhtml = /<(?:script|style|link)/i,
	// checked="checked" or checked
	rchecked = /checked\s*(?:[^=]|=\s*.checked.)/i,
	rscriptType = /^$|\/(?:java|ecma)script/i,
	rscriptTypeMasked = /^true\/(.*)/,
	rcleanScript = /^\s*<!(?:\[CDATA\[|--)|(?:\]\]|--)>\s*$/g,

	// We have to close these tags to support XHTML (#13200)
	wrapMap = {

		// Support: IE 9
		option: [ 1, "<select multiple='multiple'>", "</select>" ],

		thead: [ 1, "<table>", "</table>" ],
		col: [ 2, "<table><colgroup>", "</colgroup></table>" ],
		tr: [ 2, "<table><tbody>", "</tbody></table>" ],
		td: [ 3, "<table><tbody><tr>", "</tr></tbody></table>" ],

		_default: [ 0, "", "" ]
	};

// Support: IE 9
wrapMap.optgroup = wrapMap.option;

wrapMap.tbody = wrapMap.tfoot = wrapMap.colgroup = wrapMap.caption = wrapMap.thead;
wrapMap.th = wrapMap.td;

// Support: 1.x compatibility
// Manipulating tables requires a tbody
function manipulationTarget( elem, content ) {
	return jQuery.nodeName( elem, "table" ) &&
		jQuery.nodeName( content.nodeType !== 11 ? content : content.firstChild, "tr" ) ?

		elem.getElementsByTagName("tbody")[0] ||
			elem.appendChild( elem.ownerDocument.createElement("tbody") ) :
		elem;
}

// Replace/restore the type attribute of script elements for safe DOM manipulation
function disableScript( elem ) {
	elem.type = (elem.getAttribute("type") !== null) + "/" + elem.type;
	return elem;
}
function restoreScript( elem ) {
	var match = rscriptTypeMasked.exec( elem.type );

	if ( match ) {
		elem.type = match[ 1 ];
	} else {
		elem.removeAttribute("type");
	}

	return elem;
}

// Mark scripts as having already been evaluated
function setGlobalEval( elems, refElements ) {
	var i = 0,
		l = elems.length;

	for ( ; i < l; i++ ) {
		data_priv.set(
			elems[ i ], "globalEval", !refElements || data_priv.get( refElements[ i ], "globalEval" )
		);
	}
}

function cloneCopyEvent( src, dest ) {
	var i, l, type, pdataOld, pdataCur, udataOld, udataCur, events;

	if ( dest.nodeType !== 1 ) {
		return;
	}

	// 1. Copy private data: events, handlers, etc.
	if ( data_priv.hasData( src ) ) {
		pdataOld = data_priv.access( src );
		pdataCur = data_priv.set( dest, pdataOld );
		events = pdataOld.events;

		if ( events ) {
			delete pdataCur.handle;
			pdataCur.events = {};

			for ( type in events ) {
				for ( i = 0, l = events[ type ].length; i < l; i++ ) {
					jQuery.event.add( dest, type, events[ type ][ i ] );
				}
			}
		}
	}

	// 2. Copy user data
	if ( data_user.hasData( src ) ) {
		udataOld = data_user.access( src );
		udataCur = jQuery.extend( {}, udataOld );

		data_user.set( dest, udataCur );
	}
}

function getAll( context, tag ) {
	var ret = context.getElementsByTagName ? context.getElementsByTagName( tag || "*" ) :
			context.querySelectorAll ? context.querySelectorAll( tag || "*" ) :
			[];

	return tag === undefined || tag && jQuery.nodeName( context, tag ) ?
		jQuery.merge( [ context ], ret ) :
		ret;
}

// Support: IE >= 9
function fixInput( src, dest ) {
	var nodeName = dest.nodeName.toLowerCase();

	// Fails to persist the checked state of a cloned checkbox or radio button.
	if ( nodeName === "input" && rcheckableType.test( src.type ) ) {
		dest.checked = src.checked;

	// Fails to return the selected option to the default selected state when cloning options
	} else if ( nodeName === "input" || nodeName === "textarea" ) {
		dest.defaultValue = src.defaultValue;
	}
}

jQuery.extend({
	clone: function( elem, dataAndEvents, deepDataAndEvents ) {
		var i, l, srcElements, destElements,
			clone = elem.cloneNode( true ),
			inPage = jQuery.contains( elem.ownerDocument, elem );

		// Support: IE >= 9
		// Fix Cloning issues
		if ( !support.noCloneChecked && ( elem.nodeType === 1 || elem.nodeType === 11 ) &&
				!jQuery.isXMLDoc( elem ) ) {

			// We eschew Sizzle here for performance reasons: http://jsperf.com/getall-vs-sizzle/2
			destElements = getAll( clone );
			srcElements = getAll( elem );

			for ( i = 0, l = srcElements.length; i < l; i++ ) {
				fixInput( srcElements[ i ], destElements[ i ] );
			}
		}

		// Copy the events from the original to the clone
		if ( dataAndEvents ) {
			if ( deepDataAndEvents ) {
				srcElements = srcElements || getAll( elem );
				destElements = destElements || getAll( clone );

				for ( i = 0, l = srcElements.length; i < l; i++ ) {
					cloneCopyEvent( srcElements[ i ], destElements[ i ] );
				}
			} else {
				cloneCopyEvent( elem, clone );
			}
		}

		// Preserve script evaluation history
		destElements = getAll( clone, "script" );
		if ( destElements.length > 0 ) {
			setGlobalEval( destElements, !inPage && getAll( elem, "script" ) );
		}

		// Return the cloned set
		return clone;
	},

	buildFragment: function( elems, context, scripts, selection ) {
		var elem, tmp, tag, wrap, contains, j,
			fragment = context.createDocumentFragment(),
			nodes = [],
			i = 0,
			l = elems.length;

		for ( ; i < l; i++ ) {
			elem = elems[ i ];

			if ( elem || elem === 0 ) {

				// Add nodes directly
				if ( jQuery.type( elem ) === "object" ) {
					// Support: QtWebKit
					// jQuery.merge because push.apply(_, arraylike) throws
					jQuery.merge( nodes, elem.nodeType ? [ elem ] : elem );

				// Convert non-html into a text node
				} else if ( !rhtml.test( elem ) ) {
					nodes.push( context.createTextNode( elem ) );

				// Convert html into DOM nodes
				} else {
					tmp = tmp || fragment.appendChild( context.createElement("div") );

					// Deserialize a standard representation
					tag = ( rtagName.exec( elem ) || [ "", "" ] )[ 1 ].toLowerCase();
					wrap = wrapMap[ tag ] || wrapMap._default;
					tmp.innerHTML = wrap[ 1 ] + elem.replace( rxhtmlTag, "<$1></$2>" ) + wrap[ 2 ];

					// Descend through wrappers to the right content
					j = wrap[ 0 ];
					while ( j-- ) {
						tmp = tmp.lastChild;
					}

					// Support: QtWebKit
					// jQuery.merge because push.apply(_, arraylike) throws
					jQuery.merge( nodes, tmp.childNodes );

					// Remember the top-level container
					tmp = fragment.firstChild;

					// Fixes #12346
					// Support: Webkit, IE
					tmp.textContent = "";
				}
			}
		}

		// Remove wrapper from fragment
		fragment.textContent = "";

		i = 0;
		while ( (elem = nodes[ i++ ]) ) {

			// #4087 - If origin and destination elements are the same, and this is
			// that element, do not do anything
			if ( selection && jQuery.inArray( elem, selection ) !== -1 ) {
				continue;
			}

			contains = jQuery.contains( elem.ownerDocument, elem );

			// Append to fragment
			tmp = getAll( fragment.appendChild( elem ), "script" );

			// Preserve script evaluation history
			if ( contains ) {
				setGlobalEval( tmp );
			}

			// Capture executables
			if ( scripts ) {
				j = 0;
				while ( (elem = tmp[ j++ ]) ) {
					if ( rscriptType.test( elem.type || "" ) ) {
						scripts.push( elem );
					}
				}
			}
		}

		return fragment;
	},

	cleanData: function( elems ) {
		var data, elem, type, key,
			special = jQuery.event.special,
			i = 0;

		for ( ; (elem = elems[ i ]) !== undefined; i++ ) {
			if ( jQuery.acceptData( elem ) ) {
				key = elem[ data_priv.expando ];

				if ( key && (data = data_priv.cache[ key ]) ) {
					if ( data.events ) {
						for ( type in data.events ) {
							if ( special[ type ] ) {
								jQuery.event.remove( elem, type );

							// This is a shortcut to avoid jQuery.event.remove's overhead
							} else {
								jQuery.removeEvent( elem, type, data.handle );
							}
						}
					}
					if ( data_priv.cache[ key ] ) {
						// Discard any remaining `private` data
						delete data_priv.cache[ key ];
					}
				}
			}
			// Discard any remaining `user` data
			delete data_user.cache[ elem[ data_user.expando ] ];
		}
	}
});

jQuery.fn.extend({
	text: function( value ) {
		return access( this, function( value ) {
			return value === undefined ?
				jQuery.text( this ) :
				this.empty().each(function() {
					if ( this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9 ) {
						this.textContent = value;
					}
				});
		}, null, value, arguments.length );
	},

	append: function() {
		return this.domManip( arguments, function( elem ) {
			if ( this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9 ) {
				var target = manipulationTarget( this, elem );
				target.appendChild( elem );
			}
		});
	},

	prepend: function() {
		return this.domManip( arguments, function( elem ) {
			if ( this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9 ) {
				var target = manipulationTarget( this, elem );
				target.insertBefore( elem, target.firstChild );
			}
		});
	},

	before: function() {
		return this.domManip( arguments, function( elem ) {
			if ( this.parentNode ) {
				this.parentNode.insertBefore( elem, this );
			}
		});
	},

	after: function() {
		return this.domManip( arguments, function( elem ) {
			if ( this.parentNode ) {
				this.parentNode.insertBefore( elem, this.nextSibling );
			}
		});
	},

	remove: function( selector, keepData /* Internal Use Only */ ) {
		var elem,
			elems = selector ? jQuery.filter( selector, this ) : this,
			i = 0;

		for ( ; (elem = elems[i]) != null; i++ ) {
			if ( !keepData && elem.nodeType === 1 ) {
				jQuery.cleanData( getAll( elem ) );
			}

			if ( elem.parentNode ) {
				if ( keepData && jQuery.contains( elem.ownerDocument, elem ) ) {
					setGlobalEval( getAll( elem, "script" ) );
				}
				elem.parentNode.removeChild( elem );
			}
		}

		return this;
	},

	empty: function() {
		var elem,
			i = 0;

		for ( ; (elem = this[i]) != null; i++ ) {
			if ( elem.nodeType === 1 ) {

				// Prevent memory leaks
				jQuery.cleanData( getAll( elem, false ) );

				// Remove any remaining nodes
				elem.textContent = "";
			}
		}

		return this;
	},

	clone: function( dataAndEvents, deepDataAndEvents ) {
		dataAndEvents = dataAndEvents == null ? false : dataAndEvents;
		deepDataAndEvents = deepDataAndEvents == null ? dataAndEvents : deepDataAndEvents;

		return this.map(function() {
			return jQuery.clone( this, dataAndEvents, deepDataAndEvents );
		});
	},

	html: function( value ) {
		return access( this, function( value ) {
			var elem = this[ 0 ] || {},
				i = 0,
				l = this.length;

			if ( value === undefined && elem.nodeType === 1 ) {
				return elem.innerHTML;
			}

			// See if we can take a shortcut and just use innerHTML
			if ( typeof value === "string" && !rnoInnerhtml.test( value ) &&
				!wrapMap[ ( rtagName.exec( value ) || [ "", "" ] )[ 1 ].toLowerCase() ] ) {

				value = value.replace( rxhtmlTag, "<$1></$2>" );

				try {
					for ( ; i < l; i++ ) {
						elem = this[ i ] || {};

						// Remove element nodes and prevent memory leaks
						if ( elem.nodeType === 1 ) {
							jQuery.cleanData( getAll( elem, false ) );
							elem.innerHTML = value;
						}
					}

					elem = 0;

				// If using innerHTML throws an exception, use the fallback method
				} catch( e ) {}
			}

			if ( elem ) {
				this.empty().append( value );
			}
		}, null, value, arguments.length );
	},

	replaceWith: function() {
		var arg = arguments[ 0 ];

		// Make the changes, replacing each context element with the new content
		this.domManip( arguments, function( elem ) {
			arg = this.parentNode;

			jQuery.cleanData( getAll( this ) );

			if ( arg ) {
				arg.replaceChild( elem, this );
			}
		});

		// Force removal if there was no new content (e.g., from empty arguments)
		return arg && (arg.length || arg.nodeType) ? this : this.remove();
	},

	detach: function( selector ) {
		return this.remove( selector, true );
	},

	domManip: function( args, callback ) {

		// Flatten any nested arrays
		args = concat.apply( [], args );

		var fragment, first, scripts, hasScripts, node, doc,
			i = 0,
			l = this.length,
			set = this,
			iNoClone = l - 1,
			value = args[ 0 ],
			isFunction = jQuery.isFunction( value );

		// We can't cloneNode fragments that contain checked, in WebKit
		if ( isFunction ||
				( l > 1 && typeof value === "string" &&
					!support.checkClone && rchecked.test( value ) ) ) {
			return this.each(function( index ) {
				var self = set.eq( index );
				if ( isFunction ) {
					args[ 0 ] = value.call( this, index, self.html() );
				}
				self.domManip( args, callback );
			});
		}

		if ( l ) {
			fragment = jQuery.buildFragment( args, this[ 0 ].ownerDocument, false, this );
			first = fragment.firstChild;

			if ( fragment.childNodes.length === 1 ) {
				fragment = first;
			}

			if ( first ) {
				scripts = jQuery.map( getAll( fragment, "script" ), disableScript );
				hasScripts = scripts.length;

				// Use the original fragment for the last item instead of the first because it can end up
				// being emptied incorrectly in certain situations (#8070).
				for ( ; i < l; i++ ) {
					node = fragment;

					if ( i !== iNoClone ) {
						node = jQuery.clone( node, true, true );

						// Keep references to cloned scripts for later restoration
						if ( hasScripts ) {
							// Support: QtWebKit
							// jQuery.merge because push.apply(_, arraylike) throws
							jQuery.merge( scripts, getAll( node, "script" ) );
						}
					}

					callback.call( this[ i ], node, i );
				}

				if ( hasScripts ) {
					doc = scripts[ scripts.length - 1 ].ownerDocument;

					// Reenable scripts
					jQuery.map( scripts, restoreScript );

					// Evaluate executable scripts on first document insertion
					for ( i = 0; i < hasScripts; i++ ) {
						node = scripts[ i ];
						if ( rscriptType.test( node.type || "" ) &&
							!data_priv.access( node, "globalEval" ) && jQuery.contains( doc, node ) ) {

							if ( node.src ) {
								// Optional AJAX dependency, but won't run scripts if not present
								if ( jQuery._evalUrl ) {
									jQuery._evalUrl( node.src );
								}
							} else {
								jQuery.globalEval( node.textContent.replace( rcleanScript, "" ) );
							}
						}
					}
				}
			}
		}

		return this;
	}
});

jQuery.each({
	appendTo: "append",
	prependTo: "prepend",
	insertBefore: "before",
	insertAfter: "after",
	replaceAll: "replaceWith"
}, function( name, original ) {
	jQuery.fn[ name ] = function( selector ) {
		var elems,
			ret = [],
			insert = jQuery( selector ),
			last = insert.length - 1,
			i = 0;

		for ( ; i <= last; i++ ) {
			elems = i === last ? this : this.clone( true );
			jQuery( insert[ i ] )[ original ]( elems );

			// Support: QtWebKit
			// .get() because push.apply(_, arraylike) throws
			push.apply( ret, elems.get() );
		}

		return this.pushStack( ret );
	};
});


var iframe,
	elemdisplay = {};

/**
 * Retrieve the actual display of a element
 * @param {String} name nodeName of the element
 * @param {Object} doc Document object
 */
// Called only from within defaultDisplay
function actualDisplay( name, doc ) {
	var style,
		elem = jQuery( doc.createElement( name ) ).appendTo( doc.body ),

		// getDefaultComputedStyle might be reliably used only on attached element
		display = window.getDefaultComputedStyle && ( style = window.getDefaultComputedStyle( elem[ 0 ] ) ) ?

			// Use of this method is a temporary fix (more like optmization) until something better comes along,
			// since it was removed from specification and supported only in FF
			style.display : jQuery.css( elem[ 0 ], "display" );

	// We don't have any data stored on the element,
	// so use "detach" method as fast way to get rid of the element
	elem.detach();

	return display;
}

/**
 * Try to determine the default display value of an element
 * @param {String} nodeName
 */
function defaultDisplay( nodeName ) {
	var doc = document,
		display = elemdisplay[ nodeName ];

	if ( !display ) {
		display = actualDisplay( nodeName, doc );

		// If the simple way fails, read from inside an iframe
		if ( display === "none" || !display ) {

			// Use the already-created iframe if possible
			iframe = (iframe || jQuery( "<iframe frameborder='0' width='0' height='0'/>" )).appendTo( doc.documentElement );

			// Always write a new HTML skeleton so Webkit and Firefox don't choke on reuse
			doc = iframe[ 0 ].contentDocument;

			// Support: IE
			doc.write();
			doc.close();

			display = actualDisplay( nodeName, doc );
			iframe.detach();
		}

		// Store the correct default display
		elemdisplay[ nodeName ] = display;
	}

	return display;
}
var rmargin = (/^margin/);

var rnumnonpx = new RegExp( "^(" + pnum + ")(?!px)[a-z%]+$", "i" );

var getStyles = function( elem ) {
		return elem.ownerDocument.defaultView.getComputedStyle( elem, null );
	};



function curCSS( elem, name, computed ) {
	var width, minWidth, maxWidth, ret,
		style = elem.style;

	computed = computed || getStyles( elem );

	// Support: IE9
	// getPropertyValue is only needed for .css('filter') in IE9, see #12537
	if ( computed ) {
		ret = computed.getPropertyValue( name ) || computed[ name ];
	}

	if ( computed ) {

		if ( ret === "" && !jQuery.contains( elem.ownerDocument, elem ) ) {
			ret = jQuery.style( elem, name );
		}

		// Support: iOS < 6
		// A tribute to the "awesome hack by Dean Edwards"
		// iOS < 6 (at least) returns percentage for a larger set of values, but width seems to be reliably pixels
		// this is against the CSSOM draft spec: http://dev.w3.org/csswg/cssom/#resolved-values
		if ( rnumnonpx.test( ret ) && rmargin.test( name ) ) {

			// Remember the original values
			width = style.width;
			minWidth = style.minWidth;
			maxWidth = style.maxWidth;

			// Put in the new values to get a computed value out
			style.minWidth = style.maxWidth = style.width = ret;
			ret = computed.width;

			// Revert the changed values
			style.width = width;
			style.minWidth = minWidth;
			style.maxWidth = maxWidth;
		}
	}

	return ret !== undefined ?
		// Support: IE
		// IE returns zIndex value as an integer.
		ret + "" :
		ret;
}


function addGetHookIf( conditionFn, hookFn ) {
	// Define the hook, we'll check on the first run if it's really needed.
	return {
		get: function() {
			if ( conditionFn() ) {
				// Hook not needed (or it's not possible to use it due to missing dependency),
				// remove it.
				// Since there are no other hooks for marginRight, remove the whole object.
				delete this.get;
				return;
			}

			// Hook needed; redefine it so that the support test is not executed again.

			return (this.get = hookFn).apply( this, arguments );
		}
	};
}


(function() {
	var pixelPositionVal, boxSizingReliableVal,
		docElem = document.documentElement,
		container = document.createElement( "div" ),
		div = document.createElement( "div" );

	if ( !div.style ) {
		return;
	}

	div.style.backgroundClip = "content-box";
	div.cloneNode( true ).style.backgroundClip = "";
	support.clearCloneStyle = div.style.backgroundClip === "content-box";

	container.style.cssText = "border:0;width:0;height:0;top:0;left:-9999px;margin-top:1px;" +
		"position:absolute";
	container.appendChild( div );

	// Executing both pixelPosition & boxSizingReliable tests require only one layout
	// so they're executed at the same time to save the second computation.
	function computePixelPositionAndBoxSizingReliable() {
		div.style.cssText =
			// Support: Firefox<29, Android 2.3
			// Vendor-prefix box-sizing
			"-webkit-box-sizing:border-box;-moz-box-sizing:border-box;" +
			"box-sizing:border-box;display:block;margin-top:1%;top:1%;" +
			"border:1px;padding:1px;width:4px;position:absolute";
		div.innerHTML = "";
		docElem.appendChild( container );

		var divStyle = window.getComputedStyle( div, null );
		pixelPositionVal = divStyle.top !== "1%";
		boxSizingReliableVal = divStyle.width === "4px";

		docElem.removeChild( container );
	}

	// Support: node.js jsdom
	// Don't assume that getComputedStyle is a property of the global object
	if ( window.getComputedStyle ) {
		jQuery.extend( support, {
			pixelPosition: function() {
				// This test is executed only once but we still do memoizing
				// since we can use the boxSizingReliable pre-computing.
				// No need to check if the test was already performed, though.
				computePixelPositionAndBoxSizingReliable();
				return pixelPositionVal;
			},
			boxSizingReliable: function() {
				if ( boxSizingReliableVal == null ) {
					computePixelPositionAndBoxSizingReliable();
				}
				return boxSizingReliableVal;
			},
			reliableMarginRight: function() {
				// Support: Android 2.3
				// Check if div with explicit width and no margin-right incorrectly
				// gets computed margin-right based on width of container. (#3333)
				// WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
				// This support function is only executed once so no memoizing is needed.
				var ret,
					marginDiv = div.appendChild( document.createElement( "div" ) );

				// Reset CSS: box-sizing; display; margin; border; padding
				marginDiv.style.cssText = div.style.cssText =
					// Support: Firefox<29, Android 2.3
					// Vendor-prefix box-sizing
					"-webkit-box-sizing:content-box;-moz-box-sizing:content-box;" +
					"box-sizing:content-box;display:block;margin:0;border:0;padding:0";
				marginDiv.style.marginRight = marginDiv.style.width = "0";
				div.style.width = "1px";
				docElem.appendChild( container );

				ret = !parseFloat( window.getComputedStyle( marginDiv, null ).marginRight );

				docElem.removeChild( container );

				return ret;
			}
		});
	}
})();


// A method for quickly swapping in/out CSS properties to get correct calculations.
jQuery.swap = function( elem, options, callback, args ) {
	var ret, name,
		old = {};

	// Remember the old values, and insert the new ones
	for ( name in options ) {
		old[ name ] = elem.style[ name ];
		elem.style[ name ] = options[ name ];
	}

	ret = callback.apply( elem, args || [] );

	// Revert the old values
	for ( name in options ) {
		elem.style[ name ] = old[ name ];
	}

	return ret;
};


var
	// swappable if display is none or starts with table except "table", "table-cell", or "table-caption"
	// see here for display values: https://developer.mozilla.org/en-US/docs/CSS/display
	rdisplayswap = /^(none|table(?!-c[ea]).+)/,
	rnumsplit = new RegExp( "^(" + pnum + ")(.*)$", "i" ),
	rrelNum = new RegExp( "^([+-])=(" + pnum + ")", "i" ),

	cssShow = { position: "absolute", visibility: "hidden", display: "block" },
	cssNormalTransform = {
		letterSpacing: "0",
		fontWeight: "400"
	},

	cssPrefixes = [ "Webkit", "O", "Moz", "ms" ];

// return a css property mapped to a potentially vendor prefixed property
function vendorPropName( style, name ) {

	// shortcut for names that are not vendor prefixed
	if ( name in style ) {
		return name;
	}

	// check for vendor prefixed names
	var capName = name[0].toUpperCase() + name.slice(1),
		origName = name,
		i = cssPrefixes.length;

	while ( i-- ) {
		name = cssPrefixes[ i ] + capName;
		if ( name in style ) {
			return name;
		}
	}

	return origName;
}

function setPositiveNumber( elem, value, subtract ) {
	var matches = rnumsplit.exec( value );
	return matches ?
		// Guard against undefined "subtract", e.g., when used as in cssHooks
		Math.max( 0, matches[ 1 ] - ( subtract || 0 ) ) + ( matches[ 2 ] || "px" ) :
		value;
}

function augmentWidthOrHeight( elem, name, extra, isBorderBox, styles ) {
	var i = extra === ( isBorderBox ? "border" : "content" ) ?
		// If we already have the right measurement, avoid augmentation
		4 :
		// Otherwise initialize for horizontal or vertical properties
		name === "width" ? 1 : 0,

		val = 0;

	for ( ; i < 4; i += 2 ) {
		// both box models exclude margin, so add it if we want it
		if ( extra === "margin" ) {
			val += jQuery.css( elem, extra + cssExpand[ i ], true, styles );
		}

		if ( isBorderBox ) {
			// border-box includes padding, so remove it if we want content
			if ( extra === "content" ) {
				val -= jQuery.css( elem, "padding" + cssExpand[ i ], true, styles );
			}

			// at this point, extra isn't border nor margin, so remove border
			if ( extra !== "margin" ) {
				val -= jQuery.css( elem, "border" + cssExpand[ i ] + "Width", true, styles );
			}
		} else {
			// at this point, extra isn't content, so add padding
			val += jQuery.css( elem, "padding" + cssExpand[ i ], true, styles );

			// at this point, extra isn't content nor padding, so add border
			if ( extra !== "padding" ) {
				val += jQuery.css( elem, "border" + cssExpand[ i ] + "Width", true, styles );
			}
		}
	}

	return val;
}

function getWidthOrHeight( elem, name, extra ) {

	// Start with offset property, which is equivalent to the border-box value
	var valueIsBorderBox = true,
		val = name === "width" ? elem.offsetWidth : elem.offsetHeight,
		styles = getStyles( elem ),
		isBorderBox = jQuery.css( elem, "boxSizing", false, styles ) === "border-box";

	// some non-html elements return undefined for offsetWidth, so check for null/undefined
	// svg - https://bugzilla.mozilla.org/show_bug.cgi?id=649285
	// MathML - https://bugzilla.mozilla.org/show_bug.cgi?id=491668
	if ( val <= 0 || val == null ) {
		// Fall back to computed then uncomputed css if necessary
		val = curCSS( elem, name, styles );
		if ( val < 0 || val == null ) {
			val = elem.style[ name ];
		}

		// Computed unit is not pixels. Stop here and return.
		if ( rnumnonpx.test(val) ) {
			return val;
		}

		// we need the check for style in case a browser which returns unreliable values
		// for getComputedStyle silently falls back to the reliable elem.style
		valueIsBorderBox = isBorderBox &&
			( support.boxSizingReliable() || val === elem.style[ name ] );

		// Normalize "", auto, and prepare for extra
		val = parseFloat( val ) || 0;
	}

	// use the active box-sizing model to add/subtract irrelevant styles
	return ( val +
		augmentWidthOrHeight(
			elem,
			name,
			extra || ( isBorderBox ? "border" : "content" ),
			valueIsBorderBox,
			styles
		)
	) + "px";
}

function showHide( elements, show ) {
	var display, elem, hidden,
		values = [],
		index = 0,
		length = elements.length;

	for ( ; index < length; index++ ) {
		elem = elements[ index ];
		if ( !elem.style ) {
			continue;
		}

		values[ index ] = data_priv.get( elem, "olddisplay" );
		display = elem.style.display;
		if ( show ) {
			// Reset the inline display of this element to learn if it is
			// being hidden by cascaded rules or not
			if ( !values[ index ] && display === "none" ) {
				elem.style.display = "";
			}

			// Set elements which have been overridden with display: none
			// in a stylesheet to whatever the default browser style is
			// for such an element
			if ( elem.style.display === "" && isHidden( elem ) ) {
				values[ index ] = data_priv.access( elem, "olddisplay", defaultDisplay(elem.nodeName) );
			}
		} else {
			hidden = isHidden( elem );

			if ( display !== "none" || !hidden ) {
				data_priv.set( elem, "olddisplay", hidden ? display : jQuery.css( elem, "display" ) );
			}
		}
	}

	// Set the display of most of the elements in a second loop
	// to avoid the constant reflow
	for ( index = 0; index < length; index++ ) {
		elem = elements[ index ];
		if ( !elem.style ) {
			continue;
		}
		if ( !show || elem.style.display === "none" || elem.style.display === "" ) {
			elem.style.display = show ? values[ index ] || "" : "none";
		}
	}

	return elements;
}

jQuery.extend({
	// Add in style property hooks for overriding the default
	// behavior of getting and setting a style property
	cssHooks: {
		opacity: {
			get: function( elem, computed ) {
				if ( computed ) {
					// We should always get a number back from opacity
					var ret = curCSS( elem, "opacity" );
					return ret === "" ? "1" : ret;
				}
			}
		}
	},

	// Don't automatically add "px" to these possibly-unitless properties
	cssNumber: {
		"columnCount": true,
		"fillOpacity": true,
		"flexGrow": true,
		"flexShrink": true,
		"fontWeight": true,
		"lineHeight": true,
		"opacity": true,
		"order": true,
		"orphans": true,
		"widows": true,
		"zIndex": true,
		"zoom": true
	},

	// Add in properties whose names you wish to fix before
	// setting or getting the value
	cssProps: {
		// normalize float css property
		"float": "cssFloat"
	},

	// Get and set the style property on a DOM Node
	style: function( elem, name, value, extra ) {
		// Don't set styles on text and comment nodes
		if ( !elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style ) {
			return;
		}

		// Make sure that we're working with the right name
		var ret, type, hooks,
			origName = jQuery.camelCase( name ),
			style = elem.style;

		name = jQuery.cssProps[ origName ] || ( jQuery.cssProps[ origName ] = vendorPropName( style, origName ) );

		// gets hook for the prefixed version
		// followed by the unprefixed version
		hooks = jQuery.cssHooks[ name ] || jQuery.cssHooks[ origName ];

		// Check if we're setting a value
		if ( value !== undefined ) {
			type = typeof value;

			// convert relative number strings (+= or -=) to relative numbers. #7345
			if ( type === "string" && (ret = rrelNum.exec( value )) ) {
				value = ( ret[1] + 1 ) * ret[2] + parseFloat( jQuery.css( elem, name ) );
				// Fixes bug #9237
				type = "number";
			}

			// Make sure that null and NaN values aren't set. See: #7116
			if ( value == null || value !== value ) {
				return;
			}

			// If a number was passed in, add 'px' to the (except for certain CSS properties)
			if ( type === "number" && !jQuery.cssNumber[ origName ] ) {
				value += "px";
			}

			// Fixes #8908, it can be done more correctly by specifying setters in cssHooks,
			// but it would mean to define eight (for every problematic property) identical functions
			if ( !support.clearCloneStyle && value === "" && name.indexOf( "background" ) === 0 ) {
				style[ name ] = "inherit";
			}

			// If a hook was provided, use that value, otherwise just set the specified value
			if ( !hooks || !("set" in hooks) || (value = hooks.set( elem, value, extra )) !== undefined ) {
				style[ name ] = value;
			}

		} else {
			// If a hook was provided get the non-computed value from there
			if ( hooks && "get" in hooks && (ret = hooks.get( elem, false, extra )) !== undefined ) {
				return ret;
			}

			// Otherwise just get the value from the style object
			return style[ name ];
		}
	},

	css: function( elem, name, extra, styles ) {
		var val, num, hooks,
			origName = jQuery.camelCase( name );

		// Make sure that we're working with the right name
		name = jQuery.cssProps[ origName ] || ( jQuery.cssProps[ origName ] = vendorPropName( elem.style, origName ) );

		// gets hook for the prefixed version
		// followed by the unprefixed version
		hooks = jQuery.cssHooks[ name ] || jQuery.cssHooks[ origName ];

		// If a hook was provided get the computed value from there
		if ( hooks && "get" in hooks ) {
			val = hooks.get( elem, true, extra );
		}

		// Otherwise, if a way to get the computed value exists, use that
		if ( val === undefined ) {
			val = curCSS( elem, name, styles );
		}

		//convert "normal" to computed value
		if ( val === "normal" && name in cssNormalTransform ) {
			val = cssNormalTransform[ name ];
		}

		// Return, converting to number if forced or a qualifier was provided and val looks numeric
		if ( extra === "" || extra ) {
			num = parseFloat( val );
			return extra === true || jQuery.isNumeric( num ) ? num || 0 : val;
		}
		return val;
	}
});

jQuery.each([ "height", "width" ], function( i, name ) {
	jQuery.cssHooks[ name ] = {
		get: function( elem, computed, extra ) {
			if ( computed ) {
				// certain elements can have dimension info if we invisibly show them
				// however, it must have a current display style that would benefit from this
				return rdisplayswap.test( jQuery.css( elem, "display" ) ) && elem.offsetWidth === 0 ?
					jQuery.swap( elem, cssShow, function() {
						return getWidthOrHeight( elem, name, extra );
					}) :
					getWidthOrHeight( elem, name, extra );
			}
		},

		set: function( elem, value, extra ) {
			var styles = extra && getStyles( elem );
			return setPositiveNumber( elem, value, extra ?
				augmentWidthOrHeight(
					elem,
					name,
					extra,
					jQuery.css( elem, "boxSizing", false, styles ) === "border-box",
					styles
				) : 0
			);
		}
	};
});

// Support: Android 2.3
jQuery.cssHooks.marginRight = addGetHookIf( support.reliableMarginRight,
	function( elem, computed ) {
		if ( computed ) {
			// WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
			// Work around by temporarily setting element display to inline-block
			return jQuery.swap( elem, { "display": "inline-block" },
				curCSS, [ elem, "marginRight" ] );
		}
	}
);

// These hooks are used by animate to expand properties
jQuery.each({
	margin: "",
	padding: "",
	border: "Width"
}, function( prefix, suffix ) {
	jQuery.cssHooks[ prefix + suffix ] = {
		expand: function( value ) {
			var i = 0,
				expanded = {},

				// assumes a single number if not a string
				parts = typeof value === "string" ? value.split(" ") : [ value ];

			for ( ; i < 4; i++ ) {
				expanded[ prefix + cssExpand[ i ] + suffix ] =
					parts[ i ] || parts[ i - 2 ] || parts[ 0 ];
			}

			return expanded;
		}
	};

	if ( !rmargin.test( prefix ) ) {
		jQuery.cssHooks[ prefix + suffix ].set = setPositiveNumber;
	}
});

jQuery.fn.extend({
	css: function( name, value ) {
		return access( this, function( elem, name, value ) {
			var styles, len,
				map = {},
				i = 0;

			if ( jQuery.isArray( name ) ) {
				styles = getStyles( elem );
				len = name.length;

				for ( ; i < len; i++ ) {
					map[ name[ i ] ] = jQuery.css( elem, name[ i ], false, styles );
				}

				return map;
			}

			return value !== undefined ?
				jQuery.style( elem, name, value ) :
				jQuery.css( elem, name );
		}, name, value, arguments.length > 1 );
	},
	show: function() {
		return showHide( this, true );
	},
	hide: function() {
		return showHide( this );
	},
	toggle: function( state ) {
		if ( typeof state === "boolean" ) {
			return state ? this.show() : this.hide();
		}

		return this.each(function() {
			if ( isHidden( this ) ) {
				jQuery( this ).show();
			} else {
				jQuery( this ).hide();
			}
		});
	}
});


function Tween( elem, options, prop, end, easing ) {
	return new Tween.prototype.init( elem, options, prop, end, easing );
}
jQuery.Tween = Tween;

Tween.prototype = {
	constructor: Tween,
	init: function( elem, options, prop, end, easing, unit ) {
		this.elem = elem;
		this.prop = prop;
		this.easing = easing || "swing";
		this.options = options;
		this.start = this.now = this.cur();
		this.end = end;
		this.unit = unit || ( jQuery.cssNumber[ prop ] ? "" : "px" );
	},
	cur: function() {
		var hooks = Tween.propHooks[ this.prop ];

		return hooks && hooks.get ?
			hooks.get( this ) :
			Tween.propHooks._default.get( this );
	},
	run: function( percent ) {
		var eased,
			hooks = Tween.propHooks[ this.prop ];

		if ( this.options.duration ) {
			this.pos = eased = jQuery.easing[ this.easing ](
				percent, this.options.duration * percent, 0, 1, this.options.duration
			);
		} else {
			this.pos = eased = percent;
		}
		this.now = ( this.end - this.start ) * eased + this.start;

		if ( this.options.step ) {
			this.options.step.call( this.elem, this.now, this );
		}

		if ( hooks && hooks.set ) {
			hooks.set( this );
		} else {
			Tween.propHooks._default.set( this );
		}
		return this;
	}
};

Tween.prototype.init.prototype = Tween.prototype;

Tween.propHooks = {
	_default: {
		get: function( tween ) {
			var result;

			if ( tween.elem[ tween.prop ] != null &&
				(!tween.elem.style || tween.elem.style[ tween.prop ] == null) ) {
				return tween.elem[ tween.prop ];
			}

			// passing an empty string as a 3rd parameter to .css will automatically
			// attempt a parseFloat and fallback to a string if the parse fails
			// so, simple values such as "10px" are parsed to Float.
			// complex values such as "rotate(1rad)" are returned as is.
			result = jQuery.css( tween.elem, tween.prop, "" );
			// Empty strings, null, undefined and "auto" are converted to 0.
			return !result || result === "auto" ? 0 : result;
		},
		set: function( tween ) {
			// use step hook for back compat - use cssHook if its there - use .style if its
			// available and use plain properties where available
			if ( jQuery.fx.step[ tween.prop ] ) {
				jQuery.fx.step[ tween.prop ]( tween );
			} else if ( tween.elem.style && ( tween.elem.style[ jQuery.cssProps[ tween.prop ] ] != null || jQuery.cssHooks[ tween.prop ] ) ) {
				jQuery.style( tween.elem, tween.prop, tween.now + tween.unit );
			} else {
				tween.elem[ tween.prop ] = tween.now;
			}
		}
	}
};

// Support: IE9
// Panic based approach to setting things on disconnected nodes

Tween.propHooks.scrollTop = Tween.propHooks.scrollLeft = {
	set: function( tween ) {
		if ( tween.elem.nodeType && tween.elem.parentNode ) {
			tween.elem[ tween.prop ] = tween.now;
		}
	}
};

jQuery.easing = {
	linear: function( p ) {
		return p;
	},
	swing: function( p ) {
		return 0.5 - Math.cos( p * Math.PI ) / 2;
	}
};

jQuery.fx = Tween.prototype.init;

// Back Compat <1.8 extension point
jQuery.fx.step = {};




var
	fxNow, timerId,
	rfxtypes = /^(?:toggle|show|hide)$/,
	rfxnum = new RegExp( "^(?:([+-])=|)(" + pnum + ")([a-z%]*)$", "i" ),
	rrun = /queueHooks$/,
	animationPrefilters = [ defaultPrefilter ],
	tweeners = {
		"*": [ function( prop, value ) {
			var tween = this.createTween( prop, value ),
				target = tween.cur(),
				parts = rfxnum.exec( value ),
				unit = parts && parts[ 3 ] || ( jQuery.cssNumber[ prop ] ? "" : "px" ),

				// Starting value computation is required for potential unit mismatches
				start = ( jQuery.cssNumber[ prop ] || unit !== "px" && +target ) &&
					rfxnum.exec( jQuery.css( tween.elem, prop ) ),
				scale = 1,
				maxIterations = 20;

			if ( start && start[ 3 ] !== unit ) {
				// Trust units reported by jQuery.css
				unit = unit || start[ 3 ];

				// Make sure we update the tween properties later on
				parts = parts || [];

				// Iteratively approximate from a nonzero starting point
				start = +target || 1;

				do {
					// If previous iteration zeroed out, double until we get *something*
					// Use a string for doubling factor so we don't accidentally see scale as unchanged below
					scale = scale || ".5";

					// Adjust and apply
					start = start / scale;
					jQuery.style( tween.elem, prop, start + unit );

				// Update scale, tolerating zero or NaN from tween.cur()
				// And breaking the loop if scale is unchanged or perfect, or if we've just had enough
				} while ( scale !== (scale = tween.cur() / target) && scale !== 1 && --maxIterations );
			}

			// Update tween properties
			if ( parts ) {
				start = tween.start = +start || +target || 0;
				tween.unit = unit;
				// If a +=/-= token was provided, we're doing a relative animation
				tween.end = parts[ 1 ] ?
					start + ( parts[ 1 ] + 1 ) * parts[ 2 ] :
					+parts[ 2 ];
			}

			return tween;
		} ]
	};

// Animations created synchronously will run synchronously
function createFxNow() {
	setTimeout(function() {
		fxNow = undefined;
	});
	return ( fxNow = jQuery.now() );
}

// Generate parameters to create a standard animation
function genFx( type, includeWidth ) {
	var which,
		i = 0,
		attrs = { height: type };

	// if we include width, step value is 1 to do all cssExpand values,
	// if we don't include width, step value is 2 to skip over Left and Right
	includeWidth = includeWidth ? 1 : 0;
	for ( ; i < 4 ; i += 2 - includeWidth ) {
		which = cssExpand[ i ];
		attrs[ "margin" + which ] = attrs[ "padding" + which ] = type;
	}

	if ( includeWidth ) {
		attrs.opacity = attrs.width = type;
	}

	return attrs;
}

function createTween( value, prop, animation ) {
	var tween,
		collection = ( tweeners[ prop ] || [] ).concat( tweeners[ "*" ] ),
		index = 0,
		length = collection.length;
	for ( ; index < length; index++ ) {
		if ( (tween = collection[ index ].call( animation, prop, value )) ) {

			// we're done with this property
			return tween;
		}
	}
}

function defaultPrefilter( elem, props, opts ) {
	/* jshint validthis: true */
	var prop, value, toggle, tween, hooks, oldfire, display, checkDisplay,
		anim = this,
		orig = {},
		style = elem.style,
		hidden = elem.nodeType && isHidden( elem ),
		dataShow = data_priv.get( elem, "fxshow" );

	// handle queue: false promises
	if ( !opts.queue ) {
		hooks = jQuery._queueHooks( elem, "fx" );
		if ( hooks.unqueued == null ) {
			hooks.unqueued = 0;
			oldfire = hooks.empty.fire;
			hooks.empty.fire = function() {
				if ( !hooks.unqueued ) {
					oldfire();
				}
			};
		}
		hooks.unqueued++;

		anim.always(function() {
			// doing this makes sure that the complete handler will be called
			// before this completes
			anim.always(function() {
				hooks.unqueued--;
				if ( !jQuery.queue( elem, "fx" ).length ) {
					hooks.empty.fire();
				}
			});
		});
	}

	// height/width overflow pass
	if ( elem.nodeType === 1 && ( "height" in props || "width" in props ) ) {
		// Make sure that nothing sneaks out
		// Record all 3 overflow attributes because IE9-10 do not
		// change the overflow attribute when overflowX and
		// overflowY are set to the same value
		opts.overflow = [ style.overflow, style.overflowX, style.overflowY ];

		// Set display property to inline-block for height/width
		// animations on inline elements that are having width/height animated
		display = jQuery.css( elem, "display" );

		// Test default display if display is currently "none"
		checkDisplay = display === "none" ?
			data_priv.get( elem, "olddisplay" ) || defaultDisplay( elem.nodeName ) : display;

		if ( checkDisplay === "inline" && jQuery.css( elem, "float" ) === "none" ) {
			style.display = "inline-block";
		}
	}

	if ( opts.overflow ) {
		style.overflow = "hidden";
		anim.always(function() {
			style.overflow = opts.overflow[ 0 ];
			style.overflowX = opts.overflow[ 1 ];
			style.overflowY = opts.overflow[ 2 ];
		});
	}

	// show/hide pass
	for ( prop in props ) {
		value = props[ prop ];
		if ( rfxtypes.exec( value ) ) {
			delete props[ prop ];
			toggle = toggle || value === "toggle";
			if ( value === ( hidden ? "hide" : "show" ) ) {

				// If there is dataShow left over from a stopped hide or show and we are going to proceed with show, we should pretend to be hidden
				if ( value === "show" && dataShow && dataShow[ prop ] !== undefined ) {
					hidden = true;
				} else {
					continue;
				}
			}
			orig[ prop ] = dataShow && dataShow[ prop ] || jQuery.style( elem, prop );

		// Any non-fx value stops us from restoring the original display value
		} else {
			display = undefined;
		}
	}

	if ( !jQuery.isEmptyObject( orig ) ) {
		if ( dataShow ) {
			if ( "hidden" in dataShow ) {
				hidden = dataShow.hidden;
			}
		} else {
			dataShow = data_priv.access( elem, "fxshow", {} );
		}

		// store state if its toggle - enables .stop().toggle() to "reverse"
		if ( toggle ) {
			dataShow.hidden = !hidden;
		}
		if ( hidden ) {
			jQuery( elem ).show();
		} else {
			anim.done(function() {
				jQuery( elem ).hide();
			});
		}
		anim.done(function() {
			var prop;

			data_priv.remove( elem, "fxshow" );
			for ( prop in orig ) {
				jQuery.style( elem, prop, orig[ prop ] );
			}
		});
		for ( prop in orig ) {
			tween = createTween( hidden ? dataShow[ prop ] : 0, prop, anim );

			if ( !( prop in dataShow ) ) {
				dataShow[ prop ] = tween.start;
				if ( hidden ) {
					tween.end = tween.start;
					tween.start = prop === "width" || prop === "height" ? 1 : 0;
				}
			}
		}

	// If this is a noop like .hide().hide(), restore an overwritten display value
	} else if ( (display === "none" ? defaultDisplay( elem.nodeName ) : display) === "inline" ) {
		style.display = display;
	}
}

function propFilter( props, specialEasing ) {
	var index, name, easing, value, hooks;

	// camelCase, specialEasing and expand cssHook pass
	for ( index in props ) {
		name = jQuery.camelCase( index );
		easing = specialEasing[ name ];
		value = props[ index ];
		if ( jQuery.isArray( value ) ) {
			easing = value[ 1 ];
			value = props[ index ] = value[ 0 ];
		}

		if ( index !== name ) {
			props[ name ] = value;
			delete props[ index ];
		}

		hooks = jQuery.cssHooks[ name ];
		if ( hooks && "expand" in hooks ) {
			value = hooks.expand( value );
			delete props[ name ];

			// not quite $.extend, this wont overwrite keys already present.
			// also - reusing 'index' from above because we have the correct "name"
			for ( index in value ) {
				if ( !( index in props ) ) {
					props[ index ] = value[ index ];
					specialEasing[ index ] = easing;
				}
			}
		} else {
			specialEasing[ name ] = easing;
		}
	}
}

function Animation( elem, properties, options ) {
	var result,
		stopped,
		index = 0,
		length = animationPrefilters.length,
		deferred = jQuery.Deferred().always( function() {
			// don't match elem in the :animated selector
			delete tick.elem;
		}),
		tick = function() {
			if ( stopped ) {
				return false;
			}
			var currentTime = fxNow || createFxNow(),
				remaining = Math.max( 0, animation.startTime + animation.duration - currentTime ),
				// archaic crash bug won't allow us to use 1 - ( 0.5 || 0 ) (#12497)
				temp = remaining / animation.duration || 0,
				percent = 1 - temp,
				index = 0,
				length = animation.tweens.length;

			for ( ; index < length ; index++ ) {
				animation.tweens[ index ].run( percent );
			}

			deferred.notifyWith( elem, [ animation, percent, remaining ]);

			if ( percent < 1 && length ) {
				return remaining;
			} else {
				deferred.resolveWith( elem, [ animation ] );
				return false;
			}
		},
		animation = deferred.promise({
			elem: elem,
			props: jQuery.extend( {}, properties ),
			opts: jQuery.extend( true, { specialEasing: {} }, options ),
			originalProperties: properties,
			originalOptions: options,
			startTime: fxNow || createFxNow(),
			duration: options.duration,
			tweens: [],
			createTween: function( prop, end ) {
				var tween = jQuery.Tween( elem, animation.opts, prop, end,
						animation.opts.specialEasing[ prop ] || animation.opts.easing );
				animation.tweens.push( tween );
				return tween;
			},
			stop: function( gotoEnd ) {
				var index = 0,
					// if we are going to the end, we want to run all the tweens
					// otherwise we skip this part
					length = gotoEnd ? animation.tweens.length : 0;
				if ( stopped ) {
					return this;
				}
				stopped = true;
				for ( ; index < length ; index++ ) {
					animation.tweens[ index ].run( 1 );
				}

				// resolve when we played the last frame
				// otherwise, reject
				if ( gotoEnd ) {
					deferred.resolveWith( elem, [ animation, gotoEnd ] );
				} else {
					deferred.rejectWith( elem, [ animation, gotoEnd ] );
				}
				return this;
			}
		}),
		props = animation.props;

	propFilter( props, animation.opts.specialEasing );

	for ( ; index < length ; index++ ) {
		result = animationPrefilters[ index ].call( animation, elem, props, animation.opts );
		if ( result ) {
			return result;
		}
	}

	jQuery.map( props, createTween, animation );

	if ( jQuery.isFunction( animation.opts.start ) ) {
		animation.opts.start.call( elem, animation );
	}

	jQuery.fx.timer(
		jQuery.extend( tick, {
			elem: elem,
			anim: animation,
			queue: animation.opts.queue
		})
	);

	// attach callbacks from options
	return animation.progress( animation.opts.progress )
		.done( animation.opts.done, animation.opts.complete )
		.fail( animation.opts.fail )
		.always( animation.opts.always );
}

jQuery.Animation = jQuery.extend( Animation, {

	tweener: function( props, callback ) {
		if ( jQuery.isFunction( props ) ) {
			callback = props;
			props = [ "*" ];
		} else {
			props = props.split(" ");
		}

		var prop,
			index = 0,
			length = props.length;

		for ( ; index < length ; index++ ) {
			prop = props[ index ];
			tweeners[ prop ] = tweeners[ prop ] || [];
			tweeners[ prop ].unshift( callback );
		}
	},

	prefilter: function( callback, prepend ) {
		if ( prepend ) {
			animationPrefilters.unshift( callback );
		} else {
			animationPrefilters.push( callback );
		}
	}
});

jQuery.speed = function( speed, easing, fn ) {
	var opt = speed && typeof speed === "object" ? jQuery.extend( {}, speed ) : {
		complete: fn || !fn && easing ||
			jQuery.isFunction( speed ) && speed,
		duration: speed,
		easing: fn && easing || easing && !jQuery.isFunction( easing ) && easing
	};

	opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
		opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[ opt.duration ] : jQuery.fx.speeds._default;

	// normalize opt.queue - true/undefined/null -> "fx"
	if ( opt.queue == null || opt.queue === true ) {
		opt.queue = "fx";
	}

	// Queueing
	opt.old = opt.complete;

	opt.complete = function() {
		if ( jQuery.isFunction( opt.old ) ) {
			opt.old.call( this );
		}

		if ( opt.queue ) {
			jQuery.dequeue( this, opt.queue );
		}
	};

	return opt;
};

jQuery.fn.extend({
	fadeTo: function( speed, to, easing, callback ) {

		// show any hidden elements after setting opacity to 0
		return this.filter( isHidden ).css( "opacity", 0 ).show()

			// animate to the value specified
			.end().animate({ opacity: to }, speed, easing, callback );
	},
	animate: function( prop, speed, easing, callback ) {
		var empty = jQuery.isEmptyObject( prop ),
			optall = jQuery.speed( speed, easing, callback ),
			doAnimation = function() {
				// Operate on a copy of prop so per-property easing won't be lost
				var anim = Animation( this, jQuery.extend( {}, prop ), optall );

				// Empty animations, or finishing resolves immediately
				if ( empty || data_priv.get( this, "finish" ) ) {
					anim.stop( true );
				}
			};
			doAnimation.finish = doAnimation;

		return empty || optall.queue === false ?
			this.each( doAnimation ) :
			this.queue( optall.queue, doAnimation );
	},
	stop: function( type, clearQueue, gotoEnd ) {
		var stopQueue = function( hooks ) {
			var stop = hooks.stop;
			delete hooks.stop;
			stop( gotoEnd );
		};

		if ( typeof type !== "string" ) {
			gotoEnd = clearQueue;
			clearQueue = type;
			type = undefined;
		}
		if ( clearQueue && type !== false ) {
			this.queue( type || "fx", [] );
		}

		return this.each(function() {
			var dequeue = true,
				index = type != null && type + "queueHooks",
				timers = jQuery.timers,
				data = data_priv.get( this );

			if ( index ) {
				if ( data[ index ] && data[ index ].stop ) {
					stopQueue( data[ index ] );
				}
			} else {
				for ( index in data ) {
					if ( data[ index ] && data[ index ].stop && rrun.test( index ) ) {
						stopQueue( data[ index ] );
					}
				}
			}

			for ( index = timers.length; index--; ) {
				if ( timers[ index ].elem === this && (type == null || timers[ index ].queue === type) ) {
					timers[ index ].anim.stop( gotoEnd );
					dequeue = false;
					timers.splice( index, 1 );
				}
			}

			// start the next in the queue if the last step wasn't forced
			// timers currently will call their complete callbacks, which will dequeue
			// but only if they were gotoEnd
			if ( dequeue || !gotoEnd ) {
				jQuery.dequeue( this, type );
			}
		});
	},
	finish: function( type ) {
		if ( type !== false ) {
			type = type || "fx";
		}
		return this.each(function() {
			var index,
				data = data_priv.get( this ),
				queue = data[ type + "queue" ],
				hooks = data[ type + "queueHooks" ],
				timers = jQuery.timers,
				length = queue ? queue.length : 0;

			// enable finishing flag on private data
			data.finish = true;

			// empty the queue first
			jQuery.queue( this, type, [] );

			if ( hooks && hooks.stop ) {
				hooks.stop.call( this, true );
			}

			// look for any active animations, and finish them
			for ( index = timers.length; index--; ) {
				if ( timers[ index ].elem === this && timers[ index ].queue === type ) {
					timers[ index ].anim.stop( true );
					timers.splice( index, 1 );
				}
			}

			// look for any animations in the old queue and finish them
			for ( index = 0; index < length; index++ ) {
				if ( queue[ index ] && queue[ index ].finish ) {
					queue[ index ].finish.call( this );
				}
			}

			// turn off finishing flag
			delete data.finish;
		});
	}
});

jQuery.each([ "toggle", "show", "hide" ], function( i, name ) {
	var cssFn = jQuery.fn[ name ];
	jQuery.fn[ name ] = function( speed, easing, callback ) {
		return speed == null || typeof speed === "boolean" ?
			cssFn.apply( this, arguments ) :
			this.animate( genFx( name, true ), speed, easing, callback );
	};
});

// Generate shortcuts for custom animations
jQuery.each({
	slideDown: genFx("show"),
	slideUp: genFx("hide"),
	slideToggle: genFx("toggle"),
	fadeIn: { opacity: "show" },
	fadeOut: { opacity: "hide" },
	fadeToggle: { opacity: "toggle" }
}, function( name, props ) {
	jQuery.fn[ name ] = function( speed, easing, callback ) {
		return this.animate( props, speed, easing, callback );
	};
});

jQuery.timers = [];
jQuery.fx.tick = function() {
	var timer,
		i = 0,
		timers = jQuery.timers;

	fxNow = jQuery.now();

	for ( ; i < timers.length; i++ ) {
		timer = timers[ i ];
		// Checks the timer has not already been removed
		if ( !timer() && timers[ i ] === timer ) {
			timers.splice( i--, 1 );
		}
	}

	if ( !timers.length ) {
		jQuery.fx.stop();
	}
	fxNow = undefined;
};

jQuery.fx.timer = function( timer ) {
	jQuery.timers.push( timer );
	if ( timer() ) {
		jQuery.fx.start();
	} else {
		jQuery.timers.pop();
	}
};

jQuery.fx.interval = 13;

jQuery.fx.start = function() {
	if ( !timerId ) {
		timerId = setInterval( jQuery.fx.tick, jQuery.fx.interval );
	}
};

jQuery.fx.stop = function() {
	clearInterval( timerId );
	timerId = null;
};

jQuery.fx.speeds = {
	slow: 600,
	fast: 200,
	// Default speed
	_default: 400
};


// Based off of the plugin by Clint Helfers, with permission.
// http://blindsignals.com/index.php/2009/07/jquery-delay/
jQuery.fn.delay = function( time, type ) {
	time = jQuery.fx ? jQuery.fx.speeds[ time ] || time : time;
	type = type || "fx";

	return this.queue( type, function( next, hooks ) {
		var timeout = setTimeout( next, time );
		hooks.stop = function() {
			clearTimeout( timeout );
		};
	});
};


(function() {
	var input = document.createElement( "input" ),
		select = document.createElement( "select" ),
		opt = select.appendChild( document.createElement( "option" ) );

	input.type = "checkbox";

	// Support: iOS 5.1, Android 4.x, Android 2.3
	// Check the default checkbox/radio value ("" on old WebKit; "on" elsewhere)
	support.checkOn = input.value !== "";

	// Must access the parent to make an option select properly
	// Support: IE9, IE10
	support.optSelected = opt.selected;

	// Make sure that the options inside disabled selects aren't marked as disabled
	// (WebKit marks them as disabled)
	select.disabled = true;
	support.optDisabled = !opt.disabled;

	// Check if an input maintains its value after becoming a radio
	// Support: IE9, IE10
	input = document.createElement( "input" );
	input.value = "t";
	input.type = "radio";
	support.radioValue = input.value === "t";
})();


var nodeHook, boolHook,
	attrHandle = jQuery.expr.attrHandle;

jQuery.fn.extend({
	attr: function( name, value ) {
		return access( this, jQuery.attr, name, value, arguments.length > 1 );
	},

	removeAttr: function( name ) {
		return this.each(function() {
			jQuery.removeAttr( this, name );
		});
	}
});

jQuery.extend({
	attr: function( elem, name, value ) {
		var hooks, ret,
			nType = elem.nodeType;

		// don't get/set attributes on text, comment and attribute nodes
		if ( !elem || nType === 3 || nType === 8 || nType === 2 ) {
			return;
		}

		// Fallback to prop when attributes are not supported
		if ( typeof elem.getAttribute === strundefined ) {
			return jQuery.prop( elem, name, value );
		}

		// All attributes are lowercase
		// Grab necessary hook if one is defined
		if ( nType !== 1 || !jQuery.isXMLDoc( elem ) ) {
			name = name.toLowerCase();
			hooks = jQuery.attrHooks[ name ] ||
				( jQuery.expr.match.bool.test( name ) ? boolHook : nodeHook );
		}

		if ( value !== undefined ) {

			if ( value === null ) {
				jQuery.removeAttr( elem, name );

			} else if ( hooks && "set" in hooks && (ret = hooks.set( elem, value, name )) !== undefined ) {
				return ret;

			} else {
				elem.setAttribute( name, value + "" );
				return value;
			}

		} else if ( hooks && "get" in hooks && (ret = hooks.get( elem, name )) !== null ) {
			return ret;

		} else {
			ret = jQuery.find.attr( elem, name );

			// Non-existent attributes return null, we normalize to undefined
			return ret == null ?
				undefined :
				ret;
		}
	},

	removeAttr: function( elem, value ) {
		var name, propName,
			i = 0,
			attrNames = value && value.match( rnotwhite );

		if ( attrNames && elem.nodeType === 1 ) {
			while ( (name = attrNames[i++]) ) {
				propName = jQuery.propFix[ name ] || name;

				// Boolean attributes get special treatment (#10870)
				if ( jQuery.expr.match.bool.test( name ) ) {
					// Set corresponding property to false
					elem[ propName ] = false;
				}

				elem.removeAttribute( name );
			}
		}
	},

	attrHooks: {
		type: {
			set: function( elem, value ) {
				if ( !support.radioValue && value === "radio" &&
					jQuery.nodeName( elem, "input" ) ) {
					// Setting the type on a radio button after the value resets the value in IE6-9
					// Reset value to default in case type is set after value during creation
					var val = elem.value;
					elem.setAttribute( "type", value );
					if ( val ) {
						elem.value = val;
					}
					return value;
				}
			}
		}
	}
});

// Hooks for boolean attributes
boolHook = {
	set: function( elem, value, name ) {
		if ( value === false ) {
			// Remove boolean attributes when set to false
			jQuery.removeAttr( elem, name );
		} else {
			elem.setAttribute( name, name );
		}
		return name;
	}
};
jQuery.each( jQuery.expr.match.bool.source.match( /\w+/g ), function( i, name ) {
	var getter = attrHandle[ name ] || jQuery.find.attr;

	attrHandle[ name ] = function( elem, name, isXML ) {
		var ret, handle;
		if ( !isXML ) {
			// Avoid an infinite loop by temporarily removing this function from the getter
			handle = attrHandle[ name ];
			attrHandle[ name ] = ret;
			ret = getter( elem, name, isXML ) != null ?
				name.toLowerCase() :
				null;
			attrHandle[ name ] = handle;
		}
		return ret;
	};
});




var rfocusable = /^(?:input|select|textarea|button)$/i;

jQuery.fn.extend({
	prop: function( name, value ) {
		return access( this, jQuery.prop, name, value, arguments.length > 1 );
	},

	removeProp: function( name ) {
		return this.each(function() {
			delete this[ jQuery.propFix[ name ] || name ];
		});
	}
});

jQuery.extend({
	propFix: {
		"for": "htmlFor",
		"class": "className"
	},

	prop: function( elem, name, value ) {
		var ret, hooks, notxml,
			nType = elem.nodeType;

		// don't get/set properties on text, comment and attribute nodes
		if ( !elem || nType === 3 || nType === 8 || nType === 2 ) {
			return;
		}

		notxml = nType !== 1 || !jQuery.isXMLDoc( elem );

		if ( notxml ) {
			// Fix name and attach hooks
			name = jQuery.propFix[ name ] || name;
			hooks = jQuery.propHooks[ name ];
		}

		if ( value !== undefined ) {
			return hooks && "set" in hooks && (ret = hooks.set( elem, value, name )) !== undefined ?
				ret :
				( elem[ name ] = value );

		} else {
			return hooks && "get" in hooks && (ret = hooks.get( elem, name )) !== null ?
				ret :
				elem[ name ];
		}
	},

	propHooks: {
		tabIndex: {
			get: function( elem ) {
				return elem.hasAttribute( "tabindex" ) || rfocusable.test( elem.nodeName ) || elem.href ?
					elem.tabIndex :
					-1;
			}
		}
	}
});

// Support: IE9+
// Selectedness for an option in an optgroup can be inaccurate
if ( !support.optSelected ) {
	jQuery.propHooks.selected = {
		get: function( elem ) {
			var parent = elem.parentNode;
			if ( parent && parent.parentNode ) {
				parent.parentNode.selectedIndex;
			}
			return null;
		}
	};
}

jQuery.each([
	"tabIndex",
	"readOnly",
	"maxLength",
	"cellSpacing",
	"cellPadding",
	"rowSpan",
	"colSpan",
	"useMap",
	"frameBorder",
	"contentEditable"
], function() {
	jQuery.propFix[ this.toLowerCase() ] = this;
});




var rclass = /[\t\r\n\f]/g;

jQuery.fn.extend({
	addClass: function( value ) {
		var classes, elem, cur, clazz, j, finalValue,
			proceed = typeof value === "string" && value,
			i = 0,
			len = this.length;

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( j ) {
				jQuery( this ).addClass( value.call( this, j, this.className ) );
			});
		}

		if ( proceed ) {
			// The disjunction here is for better compressibility (see removeClass)
			classes = ( value || "" ).match( rnotwhite ) || [];

			for ( ; i < len; i++ ) {
				elem = this[ i ];
				cur = elem.nodeType === 1 && ( elem.className ?
					( " " + elem.className + " " ).replace( rclass, " " ) :
					" "
				);

				if ( cur ) {
					j = 0;
					while ( (clazz = classes[j++]) ) {
						if ( cur.indexOf( " " + clazz + " " ) < 0 ) {
							cur += clazz + " ";
						}
					}

					// only assign if different to avoid unneeded rendering.
					finalValue = jQuery.trim( cur );
					if ( elem.className !== finalValue ) {
						elem.className = finalValue;
					}
				}
			}
		}

		return this;
	},

	removeClass: function( value ) {
		var classes, elem, cur, clazz, j, finalValue,
			proceed = arguments.length === 0 || typeof value === "string" && value,
			i = 0,
			len = this.length;

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( j ) {
				jQuery( this ).removeClass( value.call( this, j, this.className ) );
			});
		}
		if ( proceed ) {
			classes = ( value || "" ).match( rnotwhite ) || [];

			for ( ; i < len; i++ ) {
				elem = this[ i ];
				// This expression is here for better compressibility (see addClass)
				cur = elem.nodeType === 1 && ( elem.className ?
					( " " + elem.className + " " ).replace( rclass, " " ) :
					""
				);

				if ( cur ) {
					j = 0;
					while ( (clazz = classes[j++]) ) {
						// Remove *all* instances
						while ( cur.indexOf( " " + clazz + " " ) >= 0 ) {
							cur = cur.replace( " " + clazz + " ", " " );
						}
					}

					// only assign if different to avoid unneeded rendering.
					finalValue = value ? jQuery.trim( cur ) : "";
					if ( elem.className !== finalValue ) {
						elem.className = finalValue;
					}
				}
			}
		}

		return this;
	},

	toggleClass: function( value, stateVal ) {
		var type = typeof value;

		if ( typeof stateVal === "boolean" && type === "string" ) {
			return stateVal ? this.addClass( value ) : this.removeClass( value );
		}

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( i ) {
				jQuery( this ).toggleClass( value.call(this, i, this.className, stateVal), stateVal );
			});
		}

		return this.each(function() {
			if ( type === "string" ) {
				// toggle individual class names
				var className,
					i = 0,
					self = jQuery( this ),
					classNames = value.match( rnotwhite ) || [];

				while ( (className = classNames[ i++ ]) ) {
					// check each className given, space separated list
					if ( self.hasClass( className ) ) {
						self.removeClass( className );
					} else {
						self.addClass( className );
					}
				}

			// Toggle whole class name
			} else if ( type === strundefined || type === "boolean" ) {
				if ( this.className ) {
					// store className if set
					data_priv.set( this, "__className__", this.className );
				}

				// If the element has a class name or if we're passed "false",
				// then remove the whole classname (if there was one, the above saved it).
				// Otherwise bring back whatever was previously saved (if anything),
				// falling back to the empty string if nothing was stored.
				this.className = this.className || value === false ? "" : data_priv.get( this, "__className__" ) || "";
			}
		});
	},

	hasClass: function( selector ) {
		var className = " " + selector + " ",
			i = 0,
			l = this.length;
		for ( ; i < l; i++ ) {
			if ( this[i].nodeType === 1 && (" " + this[i].className + " ").replace(rclass, " ").indexOf( className ) >= 0 ) {
				return true;
			}
		}

		return false;
	}
});




var rreturn = /\r/g;

jQuery.fn.extend({
	val: function( value ) {
		var hooks, ret, isFunction,
			elem = this[0];

		if ( !arguments.length ) {
			if ( elem ) {
				hooks = jQuery.valHooks[ elem.type ] || jQuery.valHooks[ elem.nodeName.toLowerCase() ];

				if ( hooks && "get" in hooks && (ret = hooks.get( elem, "value" )) !== undefined ) {
					return ret;
				}

				ret = elem.value;

				return typeof ret === "string" ?
					// handle most common string cases
					ret.replace(rreturn, "") :
					// handle cases where value is null/undef or number
					ret == null ? "" : ret;
			}

			return;
		}

		isFunction = jQuery.isFunction( value );

		return this.each(function( i ) {
			var val;

			if ( this.nodeType !== 1 ) {
				return;
			}

			if ( isFunction ) {
				val = value.call( this, i, jQuery( this ).val() );
			} else {
				val = value;
			}

			// Treat null/undefined as ""; convert numbers to string
			if ( val == null ) {
				val = "";

			} else if ( typeof val === "number" ) {
				val += "";

			} else if ( jQuery.isArray( val ) ) {
				val = jQuery.map( val, function( value ) {
					return value == null ? "" : value + "";
				});
			}

			hooks = jQuery.valHooks[ this.type ] || jQuery.valHooks[ this.nodeName.toLowerCase() ];

			// If set returns undefined, fall back to normal setting
			if ( !hooks || !("set" in hooks) || hooks.set( this, val, "value" ) === undefined ) {
				this.value = val;
			}
		});
	}
});

jQuery.extend({
	valHooks: {
		option: {
			get: function( elem ) {
				var val = jQuery.find.attr( elem, "value" );
				return val != null ?
					val :
					// Support: IE10-11+
					// option.text throws exceptions (#14686, #14858)
					jQuery.trim( jQuery.text( elem ) );
			}
		},
		select: {
			get: function( elem ) {
				var value, option,
					options = elem.options,
					index = elem.selectedIndex,
					one = elem.type === "select-one" || index < 0,
					values = one ? null : [],
					max = one ? index + 1 : options.length,
					i = index < 0 ?
						max :
						one ? index : 0;

				// Loop through all the selected options
				for ( ; i < max; i++ ) {
					option = options[ i ];

					// IE6-9 doesn't update selected after form reset (#2551)
					if ( ( option.selected || i === index ) &&
							// Don't return options that are disabled or in a disabled optgroup
							( support.optDisabled ? !option.disabled : option.getAttribute( "disabled" ) === null ) &&
							( !option.parentNode.disabled || !jQuery.nodeName( option.parentNode, "optgroup" ) ) ) {

						// Get the specific value for the option
						value = jQuery( option ).val();

						// We don't need an array for one selects
						if ( one ) {
							return value;
						}

						// Multi-Selects return an array
						values.push( value );
					}
				}

				return values;
			},

			set: function( elem, value ) {
				var optionSet, option,
					options = elem.options,
					values = jQuery.makeArray( value ),
					i = options.length;

				while ( i-- ) {
					option = options[ i ];
					if ( (option.selected = jQuery.inArray( option.value, values ) >= 0) ) {
						optionSet = true;
					}
				}

				// force browsers to behave consistently when non-matching value is set
				if ( !optionSet ) {
					elem.selectedIndex = -1;
				}
				return values;
			}
		}
	}
});

// Radios and checkboxes getter/setter
jQuery.each([ "radio", "checkbox" ], function() {
	jQuery.valHooks[ this ] = {
		set: function( elem, value ) {
			if ( jQuery.isArray( value ) ) {
				return ( elem.checked = jQuery.inArray( jQuery(elem).val(), value ) >= 0 );
			}
		}
	};
	if ( !support.checkOn ) {
		jQuery.valHooks[ this ].get = function( elem ) {
			// Support: Webkit
			// "" is returned instead of "on" if a value isn't specified
			return elem.getAttribute("value") === null ? "on" : elem.value;
		};
	}
});




// Return jQuery for attributes-only inclusion


jQuery.each( ("blur focus focusin focusout load resize scroll unload click dblclick " +
	"mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " +
	"change select submit keydown keypress keyup error contextmenu").split(" "), function( i, name ) {

	// Handle event binding
	jQuery.fn[ name ] = function( data, fn ) {
		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
});

jQuery.fn.extend({
	hover: function( fnOver, fnOut ) {
		return this.mouseenter( fnOver ).mouseleave( fnOut || fnOver );
	},

	bind: function( types, data, fn ) {
		return this.on( types, null, data, fn );
	},
	unbind: function( types, fn ) {
		return this.off( types, null, fn );
	},

	delegate: function( selector, types, data, fn ) {
		return this.on( types, selector, data, fn );
	},
	undelegate: function( selector, types, fn ) {
		// ( namespace ) or ( selector, types [, fn] )
		return arguments.length === 1 ? this.off( selector, "**" ) : this.off( types, selector || "**", fn );
	}
});


var nonce = jQuery.now();

var rquery = (/\?/);



// Support: Android 2.3
// Workaround failure to string-cast null input
jQuery.parseJSON = function( data ) {
	return JSON.parse( data + "" );
};


// Cross-browser xml parsing
jQuery.parseXML = function( data ) {
	var xml, tmp;
	if ( !data || typeof data !== "string" ) {
		return null;
	}

	// Support: IE9
	try {
		tmp = new DOMParser();
		xml = tmp.parseFromString( data, "text/xml" );
	} catch ( e ) {
		xml = undefined;
	}

	if ( !xml || xml.getElementsByTagName( "parsererror" ).length ) {
		jQuery.error( "Invalid XML: " + data );
	}
	return xml;
};


var
	// Document location
	ajaxLocParts,
	ajaxLocation,

	rhash = /#.*$/,
	rts = /([?&])_=[^&]*/,
	rheaders = /^(.*?):[ \t]*([^\r\n]*)$/mg,
	// #7653, #8125, #8152: local protocol detection
	rlocalProtocol = /^(?:about|app|app-storage|.+-extension|file|res|widget):$/,
	rnoContent = /^(?:GET|HEAD)$/,
	rprotocol = /^\/\//,
	rurl = /^([\w.+-]+:)(?:\/\/(?:[^\/?#]*@|)([^\/?#:]*)(?::(\d+)|)|)/,

	/* Prefilters
	 * 1) They are useful to introduce custom dataTypes (see ajax/jsonp.js for an example)
	 * 2) These are called:
	 *    - BEFORE asking for a transport
	 *    - AFTER param serialization (s.data is a string if s.processData is true)
	 * 3) key is the dataType
	 * 4) the catchall symbol "*" can be used
	 * 5) execution will start with transport dataType and THEN continue down to "*" if needed
	 */
	prefilters = {},

	/* Transports bindings
	 * 1) key is the dataType
	 * 2) the catchall symbol "*" can be used
	 * 3) selection will start with transport dataType and THEN go to "*" if needed
	 */
	transports = {},

	// Avoid comment-prolog char sequence (#10098); must appease lint and evade compression
	allTypes = "*/".concat("*");

// #8138, IE may throw an exception when accessing
// a field from window.location if document.domain has been set
try {
	ajaxLocation = location.href;
} catch( e ) {
	// Use the href attribute of an A element
	// since IE will modify it given document.location
	ajaxLocation = document.createElement( "a" );
	ajaxLocation.href = "";
	ajaxLocation = ajaxLocation.href;
}

// Segment location into parts
ajaxLocParts = rurl.exec( ajaxLocation.toLowerCase() ) || [];

// Base "constructor" for jQuery.ajaxPrefilter and jQuery.ajaxTransport
function addToPrefiltersOrTransports( structure ) {

	// dataTypeExpression is optional and defaults to "*"
	return function( dataTypeExpression, func ) {

		if ( typeof dataTypeExpression !== "string" ) {
			func = dataTypeExpression;
			dataTypeExpression = "*";
		}

		var dataType,
			i = 0,
			dataTypes = dataTypeExpression.toLowerCase().match( rnotwhite ) || [];

		if ( jQuery.isFunction( func ) ) {
			// For each dataType in the dataTypeExpression
			while ( (dataType = dataTypes[i++]) ) {
				// Prepend if requested
				if ( dataType[0] === "+" ) {
					dataType = dataType.slice( 1 ) || "*";
					(structure[ dataType ] = structure[ dataType ] || []).unshift( func );

				// Otherwise append
				} else {
					(structure[ dataType ] = structure[ dataType ] || []).push( func );
				}
			}
		}
	};
}

// Base inspection function for prefilters and transports
function inspectPrefiltersOrTransports( structure, options, originalOptions, jqXHR ) {

	var inspected = {},
		seekingTransport = ( structure === transports );

	function inspect( dataType ) {
		var selected;
		inspected[ dataType ] = true;
		jQuery.each( structure[ dataType ] || [], function( _, prefilterOrFactory ) {
			var dataTypeOrTransport = prefilterOrFactory( options, originalOptions, jqXHR );
			if ( typeof dataTypeOrTransport === "string" && !seekingTransport && !inspected[ dataTypeOrTransport ] ) {
				options.dataTypes.unshift( dataTypeOrTransport );
				inspect( dataTypeOrTransport );
				return false;
			} else if ( seekingTransport ) {
				return !( selected = dataTypeOrTransport );
			}
		});
		return selected;
	}

	return inspect( options.dataTypes[ 0 ] ) || !inspected[ "*" ] && inspect( "*" );
}

// A special extend for ajax options
// that takes "flat" options (not to be deep extended)
// Fixes #9887
function ajaxExtend( target, src ) {
	var key, deep,
		flatOptions = jQuery.ajaxSettings.flatOptions || {};

	for ( key in src ) {
		if ( src[ key ] !== undefined ) {
			( flatOptions[ key ] ? target : ( deep || (deep = {}) ) )[ key ] = src[ key ];
		}
	}
	if ( deep ) {
		jQuery.extend( true, target, deep );
	}

	return target;
}

/* Handles responses to an ajax request:
 * - finds the right dataType (mediates between content-type and expected dataType)
 * - returns the corresponding response
 */
function ajaxHandleResponses( s, jqXHR, responses ) {

	var ct, type, finalDataType, firstDataType,
		contents = s.contents,
		dataTypes = s.dataTypes;

	// Remove auto dataType and get content-type in the process
	while ( dataTypes[ 0 ] === "*" ) {
		dataTypes.shift();
		if ( ct === undefined ) {
			ct = s.mimeType || jqXHR.getResponseHeader("Content-Type");
		}
	}

	// Check if we're dealing with a known content-type
	if ( ct ) {
		for ( type in contents ) {
			if ( contents[ type ] && contents[ type ].test( ct ) ) {
				dataTypes.unshift( type );
				break;
			}
		}
	}

	// Check to see if we have a response for the expected dataType
	if ( dataTypes[ 0 ] in responses ) {
		finalDataType = dataTypes[ 0 ];
	} else {
		// Try convertible dataTypes
		for ( type in responses ) {
			if ( !dataTypes[ 0 ] || s.converters[ type + " " + dataTypes[0] ] ) {
				finalDataType = type;
				break;
			}
			if ( !firstDataType ) {
				firstDataType = type;
			}
		}
		// Or just use first one
		finalDataType = finalDataType || firstDataType;
	}

	// If we found a dataType
	// We add the dataType to the list if needed
	// and return the corresponding response
	if ( finalDataType ) {
		if ( finalDataType !== dataTypes[ 0 ] ) {
			dataTypes.unshift( finalDataType );
		}
		return responses[ finalDataType ];
	}
}

/* Chain conversions given the request and the original response
 * Also sets the responseXXX fields on the jqXHR instance
 */
function ajaxConvert( s, response, jqXHR, isSuccess ) {
	var conv2, current, conv, tmp, prev,
		converters = {},
		// Work with a copy of dataTypes in case we need to modify it for conversion
		dataTypes = s.dataTypes.slice();

	// Create converters map with lowercased keys
	if ( dataTypes[ 1 ] ) {
		for ( conv in s.converters ) {
			converters[ conv.toLowerCase() ] = s.converters[ conv ];
		}
	}

	current = dataTypes.shift();

	// Convert to each sequential dataType
	while ( current ) {

		if ( s.responseFields[ current ] ) {
			jqXHR[ s.responseFields[ current ] ] = response;
		}

		// Apply the dataFilter if provided
		if ( !prev && isSuccess && s.dataFilter ) {
			response = s.dataFilter( response, s.dataType );
		}

		prev = current;
		current = dataTypes.shift();

		if ( current ) {

		// There's only work to do if current dataType is non-auto
			if ( current === "*" ) {

				current = prev;

			// Convert response if prev dataType is non-auto and differs from current
			} else if ( prev !== "*" && prev !== current ) {

				// Seek a direct converter
				conv = converters[ prev + " " + current ] || converters[ "* " + current ];

				// If none found, seek a pair
				if ( !conv ) {
					for ( conv2 in converters ) {

						// If conv2 outputs current
						tmp = conv2.split( " " );
						if ( tmp[ 1 ] === current ) {

							// If prev can be converted to accepted input
							conv = converters[ prev + " " + tmp[ 0 ] ] ||
								converters[ "* " + tmp[ 0 ] ];
							if ( conv ) {
								// Condense equivalence converters
								if ( conv === true ) {
									conv = converters[ conv2 ];

								// Otherwise, insert the intermediate dataType
								} else if ( converters[ conv2 ] !== true ) {
									current = tmp[ 0 ];
									dataTypes.unshift( tmp[ 1 ] );
								}
								break;
							}
						}
					}
				}

				// Apply converter (if not an equivalence)
				if ( conv !== true ) {

					// Unless errors are allowed to bubble, catch and return them
					if ( conv && s[ "throws" ] ) {
						response = conv( response );
					} else {
						try {
							response = conv( response );
						} catch ( e ) {
							return { state: "parsererror", error: conv ? e : "No conversion from " + prev + " to " + current };
						}
					}
				}
			}
		}
	}

	return { state: "success", data: response };
}

jQuery.extend({

	// Counter for holding the number of active queries
	active: 0,

	// Last-Modified header cache for next request
	lastModified: {},
	etag: {},

	ajaxSettings: {
		url: ajaxLocation,
		type: "GET",
		isLocal: rlocalProtocol.test( ajaxLocParts[ 1 ] ),
		global: true,
		processData: true,
		async: true,
		contentType: "application/x-www-form-urlencoded; charset=UTF-8",
		/*
		timeout: 0,
		data: null,
		dataType: null,
		username: null,
		password: null,
		cache: null,
		throws: false,
		traditional: false,
		headers: {},
		*/

		accepts: {
			"*": allTypes,
			text: "text/plain",
			html: "text/html",
			xml: "application/xml, text/xml",
			json: "application/json, text/javascript"
		},

		contents: {
			xml: /xml/,
			html: /html/,
			json: /json/
		},

		responseFields: {
			xml: "responseXML",
			text: "responseText",
			json: "responseJSON"
		},

		// Data converters
		// Keys separate source (or catchall "*") and destination types with a single space
		converters: {

			// Convert anything to text
			"* text": String,

			// Text to html (true = no transformation)
			"text html": true,

			// Evaluate text as a json expression
			"text json": jQuery.parseJSON,

			// Parse text as xml
			"text xml": jQuery.parseXML
		},

		// For options that shouldn't be deep extended:
		// you can add your own custom options here if
		// and when you create one that shouldn't be
		// deep extended (see ajaxExtend)
		flatOptions: {
			url: true,
			context: true
		}
	},

	// Creates a full fledged settings object into target
	// with both ajaxSettings and settings fields.
	// If target is omitted, writes into ajaxSettings.
	ajaxSetup: function( target, settings ) {
		return settings ?

			// Building a settings object
			ajaxExtend( ajaxExtend( target, jQuery.ajaxSettings ), settings ) :

			// Extending ajaxSettings
			ajaxExtend( jQuery.ajaxSettings, target );
	},

	ajaxPrefilter: addToPrefiltersOrTransports( prefilters ),
	ajaxTransport: addToPrefiltersOrTransports( transports ),

	// Main method
	ajax: function( url, options ) {

		// If url is an object, simulate pre-1.5 signature
		if ( typeof url === "object" ) {
			options = url;
			url = undefined;
		}

		// Force options to be an object
		options = options || {};

		var transport,
			// URL without anti-cache param
			cacheURL,
			// Response headers
			responseHeadersString,
			responseHeaders,
			// timeout handle
			timeoutTimer,
			// Cross-domain detection vars
			parts,
			// To know if global events are to be dispatched
			fireGlobals,
			// Loop variable
			i,
			// Create the final options object
			s = jQuery.ajaxSetup( {}, options ),
			// Callbacks context
			callbackContext = s.context || s,
			// Context for global events is callbackContext if it is a DOM node or jQuery collection
			globalEventContext = s.context && ( callbackContext.nodeType || callbackContext.jquery ) ?
				jQuery( callbackContext ) :
				jQuery.event,
			// Deferreds
			deferred = jQuery.Deferred(),
			completeDeferred = jQuery.Callbacks("once memory"),
			// Status-dependent callbacks
			statusCode = s.statusCode || {},
			// Headers (they are sent all at once)
			requestHeaders = {},
			requestHeadersNames = {},
			// The jqXHR state
			state = 0,
			// Default abort message
			strAbort = "canceled",
			// Fake xhr
			jqXHR = {
				readyState: 0,

				// Builds headers hashtable if needed
				getResponseHeader: function( key ) {
					var match;
					if ( state === 2 ) {
						if ( !responseHeaders ) {
							responseHeaders = {};
							while ( (match = rheaders.exec( responseHeadersString )) ) {
								responseHeaders[ match[1].toLowerCase() ] = match[ 2 ];
							}
						}
						match = responseHeaders[ key.toLowerCase() ];
					}
					return match == null ? null : match;
				},

				// Raw string
				getAllResponseHeaders: function() {
					return state === 2 ? responseHeadersString : null;
				},

				// Caches the header
				setRequestHeader: function( name, value ) {
					var lname = name.toLowerCase();
					if ( !state ) {
						name = requestHeadersNames[ lname ] = requestHeadersNames[ lname ] || name;
						requestHeaders[ name ] = value;
					}
					return this;
				},

				// Overrides response content-type header
				overrideMimeType: function( type ) {
					if ( !state ) {
						s.mimeType = type;
					}
					return this;
				},

				// Status-dependent callbacks
				statusCode: function( map ) {
					var code;
					if ( map ) {
						if ( state < 2 ) {
							for ( code in map ) {
								// Lazy-add the new callback in a way that preserves old ones
								statusCode[ code ] = [ statusCode[ code ], map[ code ] ];
							}
						} else {
							// Execute the appropriate callbacks
							jqXHR.always( map[ jqXHR.status ] );
						}
					}
					return this;
				},

				// Cancel the request
				abort: function( statusText ) {
					var finalText = statusText || strAbort;
					if ( transport ) {
						transport.abort( finalText );
					}
					done( 0, finalText );
					return this;
				}
			};

		// Attach deferreds
		deferred.promise( jqXHR ).complete = completeDeferred.add;
		jqXHR.success = jqXHR.done;
		jqXHR.error = jqXHR.fail;

		// Remove hash character (#7531: and string promotion)
		// Add protocol if not provided (prefilters might expect it)
		// Handle falsy url in the settings object (#10093: consistency with old signature)
		// We also use the url parameter if available
		s.url = ( ( url || s.url || ajaxLocation ) + "" ).replace( rhash, "" )
			.replace( rprotocol, ajaxLocParts[ 1 ] + "//" );

		// Alias method option to type as per ticket #12004
		s.type = options.method || options.type || s.method || s.type;

		// Extract dataTypes list
		s.dataTypes = jQuery.trim( s.dataType || "*" ).toLowerCase().match( rnotwhite ) || [ "" ];

		// A cross-domain request is in order when we have a protocol:host:port mismatch
		if ( s.crossDomain == null ) {
			parts = rurl.exec( s.url.toLowerCase() );
			s.crossDomain = !!( parts &&
				( parts[ 1 ] !== ajaxLocParts[ 1 ] || parts[ 2 ] !== ajaxLocParts[ 2 ] ||
					( parts[ 3 ] || ( parts[ 1 ] === "http:" ? "80" : "443" ) ) !==
						( ajaxLocParts[ 3 ] || ( ajaxLocParts[ 1 ] === "http:" ? "80" : "443" ) ) )
			);
		}

		// Convert data if not already a string
		if ( s.data && s.processData && typeof s.data !== "string" ) {
			s.data = jQuery.param( s.data, s.traditional );
		}

		// Apply prefilters
		inspectPrefiltersOrTransports( prefilters, s, options, jqXHR );

		// If request was aborted inside a prefilter, stop there
		if ( state === 2 ) {
			return jqXHR;
		}

		// We can fire global events as of now if asked to
		fireGlobals = s.global;

		// Watch for a new set of requests
		if ( fireGlobals && jQuery.active++ === 0 ) {
			jQuery.event.trigger("ajaxStart");
		}

		// Uppercase the type
		s.type = s.type.toUpperCase();

		// Determine if request has content
		s.hasContent = !rnoContent.test( s.type );

		// Save the URL in case we're toying with the If-Modified-Since
		// and/or If-None-Match header later on
		cacheURL = s.url;

		// More options handling for requests with no content
		if ( !s.hasContent ) {

			// If data is available, append data to url
			if ( s.data ) {
				cacheURL = ( s.url += ( rquery.test( cacheURL ) ? "&" : "?" ) + s.data );
				// #9682: remove data so that it's not used in an eventual retry
				delete s.data;
			}

			// Add anti-cache in url if needed
			if ( s.cache === false ) {
				s.url = rts.test( cacheURL ) ?

					// If there is already a '_' parameter, set its value
					cacheURL.replace( rts, "$1_=" + nonce++ ) :

					// Otherwise add one to the end
					cacheURL + ( rquery.test( cacheURL ) ? "&" : "?" ) + "_=" + nonce++;
			}
		}

		// Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
		if ( s.ifModified ) {
			if ( jQuery.lastModified[ cacheURL ] ) {
				jqXHR.setRequestHeader( "If-Modified-Since", jQuery.lastModified[ cacheURL ] );
			}
			if ( jQuery.etag[ cacheURL ] ) {
				jqXHR.setRequestHeader( "If-None-Match", jQuery.etag[ cacheURL ] );
			}
		}

		// Set the correct header, if data is being sent
		if ( s.data && s.hasContent && s.contentType !== false || options.contentType ) {
			jqXHR.setRequestHeader( "Content-Type", s.contentType );
		}

		// Set the Accepts header for the server, depending on the dataType
		jqXHR.setRequestHeader(
			"Accept",
			s.dataTypes[ 0 ] && s.accepts[ s.dataTypes[0] ] ?
				s.accepts[ s.dataTypes[0] ] + ( s.dataTypes[ 0 ] !== "*" ? ", " + allTypes + "; q=0.01" : "" ) :
				s.accepts[ "*" ]
		);

		// Check for headers option
		for ( i in s.headers ) {
			jqXHR.setRequestHeader( i, s.headers[ i ] );
		}

		// Allow custom headers/mimetypes and early abort
		if ( s.beforeSend && ( s.beforeSend.call( callbackContext, jqXHR, s ) === false || state === 2 ) ) {
			// Abort if not done already and return
			return jqXHR.abort();
		}

		// aborting is no longer a cancellation
		strAbort = "abort";

		// Install callbacks on deferreds
		for ( i in { success: 1, error: 1, complete: 1 } ) {
			jqXHR[ i ]( s[ i ] );
		}

		// Get transport
		transport = inspectPrefiltersOrTransports( transports, s, options, jqXHR );

		// If no transport, we auto-abort
		if ( !transport ) {
			done( -1, "No Transport" );
		} else {
			jqXHR.readyState = 1;

			// Send global event
			if ( fireGlobals ) {
				globalEventContext.trigger( "ajaxSend", [ jqXHR, s ] );
			}
			// Timeout
			if ( s.async && s.timeout > 0 ) {
				timeoutTimer = setTimeout(function() {
					jqXHR.abort("timeout");
				}, s.timeout );
			}

			try {
				state = 1;
				transport.send( requestHeaders, done );
			} catch ( e ) {
				// Propagate exception as error if not done
				if ( state < 2 ) {
					done( -1, e );
				// Simply rethrow otherwise
				} else {
					throw e;
				}
			}
		}

		// Callback for when everything is done
		function done( status, nativeStatusText, responses, headers ) {
			var isSuccess, success, error, response, modified,
				statusText = nativeStatusText;

			// Called once
			if ( state === 2 ) {
				return;
			}

			// State is "done" now
			state = 2;

			// Clear timeout if it exists
			if ( timeoutTimer ) {
				clearTimeout( timeoutTimer );
			}

			// Dereference transport for early garbage collection
			// (no matter how long the jqXHR object will be used)
			transport = undefined;

			// Cache response headers
			responseHeadersString = headers || "";

			// Set readyState
			jqXHR.readyState = status > 0 ? 4 : 0;

			// Determine if successful
			isSuccess = status >= 200 && status < 300 || status === 304;

			// Get response data
			if ( responses ) {
				response = ajaxHandleResponses( s, jqXHR, responses );
			}

			// Convert no matter what (that way responseXXX fields are always set)
			response = ajaxConvert( s, response, jqXHR, isSuccess );

			// If successful, handle type chaining
			if ( isSuccess ) {

				// Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
				if ( s.ifModified ) {
					modified = jqXHR.getResponseHeader("Last-Modified");
					if ( modified ) {
						jQuery.lastModified[ cacheURL ] = modified;
					}
					modified = jqXHR.getResponseHeader("etag");
					if ( modified ) {
						jQuery.etag[ cacheURL ] = modified;
					}
				}

				// if no content
				if ( status === 204 || s.type === "HEAD" ) {
					statusText = "nocontent";

				// if not modified
				} else if ( status === 304 ) {
					statusText = "notmodified";

				// If we have data, let's convert it
				} else {
					statusText = response.state;
					success = response.data;
					error = response.error;
					isSuccess = !error;
				}
			} else {
				// We extract error from statusText
				// then normalize statusText and status for non-aborts
				error = statusText;
				if ( status || !statusText ) {
					statusText = "error";
					if ( status < 0 ) {
						status = 0;
					}
				}
			}

			// Set data for the fake xhr object
			jqXHR.status = status;
			jqXHR.statusText = ( nativeStatusText || statusText ) + "";

			// Success/Error
			if ( isSuccess ) {
				deferred.resolveWith( callbackContext, [ success, statusText, jqXHR ] );
			} else {
				deferred.rejectWith( callbackContext, [ jqXHR, statusText, error ] );
			}

			// Status-dependent callbacks
			jqXHR.statusCode( statusCode );
			statusCode = undefined;

			if ( fireGlobals ) {
				globalEventContext.trigger( isSuccess ? "ajaxSuccess" : "ajaxError",
					[ jqXHR, s, isSuccess ? success : error ] );
			}

			// Complete
			completeDeferred.fireWith( callbackContext, [ jqXHR, statusText ] );

			if ( fireGlobals ) {
				globalEventContext.trigger( "ajaxComplete", [ jqXHR, s ] );
				// Handle the global AJAX counter
				if ( !( --jQuery.active ) ) {
					jQuery.event.trigger("ajaxStop");
				}
			}
		}

		return jqXHR;
	},

	getJSON: function( url, data, callback ) {
		return jQuery.get( url, data, callback, "json" );
	},

	getScript: function( url, callback ) {
		return jQuery.get( url, undefined, callback, "script" );
	}
});

jQuery.each( [ "get", "post" ], function( i, method ) {
	jQuery[ method ] = function( url, data, callback, type ) {
		// shift arguments if data argument was omitted
		if ( jQuery.isFunction( data ) ) {
			type = type || callback;
			callback = data;
			data = undefined;
		}

		return jQuery.ajax({
			url: url,
			type: method,
			dataType: type,
			data: data,
			success: callback
		});
	};
});

// Attach a bunch of functions for handling common AJAX events
jQuery.each( [ "ajaxStart", "ajaxStop", "ajaxComplete", "ajaxError", "ajaxSuccess", "ajaxSend" ], function( i, type ) {
	jQuery.fn[ type ] = function( fn ) {
		return this.on( type, fn );
	};
});


jQuery._evalUrl = function( url ) {
	return jQuery.ajax({
		url: url,
		type: "GET",
		dataType: "script",
		async: false,
		global: false,
		"throws": true
	});
};


jQuery.fn.extend({
	wrapAll: function( html ) {
		var wrap;

		if ( jQuery.isFunction( html ) ) {
			return this.each(function( i ) {
				jQuery( this ).wrapAll( html.call(this, i) );
			});
		}

		if ( this[ 0 ] ) {

			// The elements to wrap the target around
			wrap = jQuery( html, this[ 0 ].ownerDocument ).eq( 0 ).clone( true );

			if ( this[ 0 ].parentNode ) {
				wrap.insertBefore( this[ 0 ] );
			}

			wrap.map(function() {
				var elem = this;

				while ( elem.firstElementChild ) {
					elem = elem.firstElementChild;
				}

				return elem;
			}).append( this );
		}

		return this;
	},

	wrapInner: function( html ) {
		if ( jQuery.isFunction( html ) ) {
			return this.each(function( i ) {
				jQuery( this ).wrapInner( html.call(this, i) );
			});
		}

		return this.each(function() {
			var self = jQuery( this ),
				contents = self.contents();

			if ( contents.length ) {
				contents.wrapAll( html );

			} else {
				self.append( html );
			}
		});
	},

	wrap: function( html ) {
		var isFunction = jQuery.isFunction( html );

		return this.each(function( i ) {
			jQuery( this ).wrapAll( isFunction ? html.call(this, i) : html );
		});
	},

	unwrap: function() {
		return this.parent().each(function() {
			if ( !jQuery.nodeName( this, "body" ) ) {
				jQuery( this ).replaceWith( this.childNodes );
			}
		}).end();
	}
});


jQuery.expr.filters.hidden = function( elem ) {
	// Support: Opera <= 12.12
	// Opera reports offsetWidths and offsetHeights less than zero on some elements
	return elem.offsetWidth <= 0 && elem.offsetHeight <= 0;
};
jQuery.expr.filters.visible = function( elem ) {
	return !jQuery.expr.filters.hidden( elem );
};




var r20 = /%20/g,
	rbracket = /\[\]$/,
	rCRLF = /\r?\n/g,
	rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
	rsubmittable = /^(?:input|select|textarea|keygen)/i;

function buildParams( prefix, obj, traditional, add ) {
	var name;

	if ( jQuery.isArray( obj ) ) {
		// Serialize array item.
		jQuery.each( obj, function( i, v ) {
			if ( traditional || rbracket.test( prefix ) ) {
				// Treat each array item as a scalar.
				add( prefix, v );

			} else {
				// Item is non-scalar (array or object), encode its numeric index.
				buildParams( prefix + "[" + ( typeof v === "object" ? i : "" ) + "]", v, traditional, add );
			}
		});

	} else if ( !traditional && jQuery.type( obj ) === "object" ) {
		// Serialize object item.
		for ( name in obj ) {
			buildParams( prefix + "[" + name + "]", obj[ name ], traditional, add );
		}

	} else {
		// Serialize scalar item.
		add( prefix, obj );
	}
}

// Serialize an array of form elements or a set of
// key/values into a query string
jQuery.param = function( a, traditional ) {
	var prefix,
		s = [],
		add = function( key, value ) {
			// If value is a function, invoke it and return its value
			value = jQuery.isFunction( value ) ? value() : ( value == null ? "" : value );
			s[ s.length ] = encodeURIComponent( key ) + "=" + encodeURIComponent( value );
		};

	// Set traditional to true for jQuery <= 1.3.2 behavior.
	if ( traditional === undefined ) {
		traditional = jQuery.ajaxSettings && jQuery.ajaxSettings.traditional;
	}

	// If an array was passed in, assume that it is an array of form elements.
	if ( jQuery.isArray( a ) || ( a.jquery && !jQuery.isPlainObject( a ) ) ) {
		// Serialize the form elements
		jQuery.each( a, function() {
			add( this.name, this.value );
		});

	} else {
		// If traditional, encode the "old" way (the way 1.3.2 or older
		// did it), otherwise encode params recursively.
		for ( prefix in a ) {
			buildParams( prefix, a[ prefix ], traditional, add );
		}
	}

	// Return the resulting serialization
	return s.join( "&" ).replace( r20, "+" );
};

jQuery.fn.extend({
	serialize: function() {
		return jQuery.param( this.serializeArray() );
	},
	serializeArray: function() {
		return this.map(function() {
			// Can add propHook for "elements" to filter or add form elements
			var elements = jQuery.prop( this, "elements" );
			return elements ? jQuery.makeArray( elements ) : this;
		})
		.filter(function() {
			var type = this.type;

			// Use .is( ":disabled" ) so that fieldset[disabled] works
			return this.name && !jQuery( this ).is( ":disabled" ) &&
				rsubmittable.test( this.nodeName ) && !rsubmitterTypes.test( type ) &&
				( this.checked || !rcheckableType.test( type ) );
		})
		.map(function( i, elem ) {
			var val = jQuery( this ).val();

			return val == null ?
				null :
				jQuery.isArray( val ) ?
					jQuery.map( val, function( val ) {
						return { name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
					}) :
					{ name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
		}).get();
	}
});


jQuery.ajaxSettings.xhr = function() {
	try {
		return new XMLHttpRequest();
	} catch( e ) {}
};

var xhrId = 0,
	xhrCallbacks = {},
	xhrSuccessStatus = {
		// file protocol always yields status code 0, assume 200
		0: 200,
		// Support: IE9
		// #1450: sometimes IE returns 1223 when it should be 204
		1223: 204
	},
	xhrSupported = jQuery.ajaxSettings.xhr();

// Support: IE9
// Open requests must be manually aborted on unload (#5280)
if ( window.ActiveXObject ) {
	jQuery( window ).on( "unload", function() {
		for ( var key in xhrCallbacks ) {
			xhrCallbacks[ key ]();
		}
	});
}

support.cors = !!xhrSupported && ( "withCredentials" in xhrSupported );
support.ajax = xhrSupported = !!xhrSupported;

jQuery.ajaxTransport(function( options ) {
	var callback;

	// Cross domain only allowed if supported through XMLHttpRequest
	if ( support.cors || xhrSupported && !options.crossDomain ) {
		return {
			send: function( headers, complete ) {
				var i,
					xhr = options.xhr(),
					id = ++xhrId;

				xhr.open( options.type, options.url, options.async, options.username, options.password );

				// Apply custom fields if provided
				if ( options.xhrFields ) {
					for ( i in options.xhrFields ) {
						xhr[ i ] = options.xhrFields[ i ];
					}
				}

				// Override mime type if needed
				if ( options.mimeType && xhr.overrideMimeType ) {
					xhr.overrideMimeType( options.mimeType );
				}

				// X-Requested-With header
				// For cross-domain requests, seeing as conditions for a preflight are
				// akin to a jigsaw puzzle, we simply never set it to be sure.
				// (it can always be set on a per-request basis or even using ajaxSetup)
				// For same-domain requests, won't change header if already provided.
				if ( !options.crossDomain && !headers["X-Requested-With"] ) {
					headers["X-Requested-With"] = "XMLHttpRequest";
				}

				// Set headers
				for ( i in headers ) {
					xhr.setRequestHeader( i, headers[ i ] );
				}

				// Callback
				callback = function( type ) {
					return function() {
						if ( callback ) {
							delete xhrCallbacks[ id ];
							callback = xhr.onload = xhr.onerror = null;

							if ( type === "abort" ) {
								xhr.abort();
							} else if ( type === "error" ) {
								complete(
									// file: protocol always yields status 0; see #8605, #14207
									xhr.status,
									xhr.statusText
								);
							} else {
								complete(
									xhrSuccessStatus[ xhr.status ] || xhr.status,
									xhr.statusText,
									// Support: IE9
									// Accessing binary-data responseText throws an exception
									// (#11426)
									typeof xhr.responseText === "string" ? {
										text: xhr.responseText
									} : undefined,
									xhr.getAllResponseHeaders()
								);
							}
						}
					};
				};

				// Listen to events
				xhr.onload = callback();
				xhr.onerror = callback("error");

				// Create the abort callback
				callback = xhrCallbacks[ id ] = callback("abort");

				try {
					// Do send the request (this may raise an exception)
					xhr.send( options.hasContent && options.data || null );
				} catch ( e ) {
					// #14683: Only rethrow if this hasn't been notified as an error yet
					if ( callback ) {
						throw e;
					}
				}
			},

			abort: function() {
				if ( callback ) {
					callback();
				}
			}
		};
	}
});




// Install script dataType
jQuery.ajaxSetup({
	accepts: {
		script: "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"
	},
	contents: {
		script: /(?:java|ecma)script/
	},
	converters: {
		"text script": function( text ) {
			jQuery.globalEval( text );
			return text;
		}
	}
});

// Handle cache's special case and crossDomain
jQuery.ajaxPrefilter( "script", function( s ) {
	if ( s.cache === undefined ) {
		s.cache = false;
	}
	if ( s.crossDomain ) {
		s.type = "GET";
	}
});

// Bind script tag hack transport
jQuery.ajaxTransport( "script", function( s ) {
	// This transport only deals with cross domain requests
	if ( s.crossDomain ) {
		var script, callback;
		return {
			send: function( _, complete ) {
				script = jQuery("<script>").prop({
					async: true,
					charset: s.scriptCharset,
					src: s.url
				}).on(
					"load error",
					callback = function( evt ) {
						script.remove();
						callback = null;
						if ( evt ) {
							complete( evt.type === "error" ? 404 : 200, evt.type );
						}
					}
				);
				document.head.appendChild( script[ 0 ] );
			},
			abort: function() {
				if ( callback ) {
					callback();
				}
			}
		};
	}
});




var oldCallbacks = [],
	rjsonp = /(=)\?(?=&|$)|\?\?/;

// Default jsonp settings
jQuery.ajaxSetup({
	jsonp: "callback",
	jsonpCallback: function() {
		var callback = oldCallbacks.pop() || ( jQuery.expando + "_" + ( nonce++ ) );
		this[ callback ] = true;
		return callback;
	}
});

// Detect, normalize options and install callbacks for jsonp requests
jQuery.ajaxPrefilter( "json jsonp", function( s, originalSettings, jqXHR ) {

	var callbackName, overwritten, responseContainer,
		jsonProp = s.jsonp !== false && ( rjsonp.test( s.url ) ?
			"url" :
			typeof s.data === "string" && !( s.contentType || "" ).indexOf("application/x-www-form-urlencoded") && rjsonp.test( s.data ) && "data"
		);

	// Handle iff the expected data type is "jsonp" or we have a parameter to set
	if ( jsonProp || s.dataTypes[ 0 ] === "jsonp" ) {

		// Get callback name, remembering preexisting value associated with it
		callbackName = s.jsonpCallback = jQuery.isFunction( s.jsonpCallback ) ?
			s.jsonpCallback() :
			s.jsonpCallback;

		// Insert callback into url or form data
		if ( jsonProp ) {
			s[ jsonProp ] = s[ jsonProp ].replace( rjsonp, "$1" + callbackName );
		} else if ( s.jsonp !== false ) {
			s.url += ( rquery.test( s.url ) ? "&" : "?" ) + s.jsonp + "=" + callbackName;
		}

		// Use data converter to retrieve json after script execution
		s.converters["script json"] = function() {
			if ( !responseContainer ) {
				jQuery.error( callbackName + " was not called" );
			}
			return responseContainer[ 0 ];
		};

		// force json dataType
		s.dataTypes[ 0 ] = "json";

		// Install callback
		overwritten = window[ callbackName ];
		window[ callbackName ] = function() {
			responseContainer = arguments;
		};

		// Clean-up function (fires after converters)
		jqXHR.always(function() {
			// Restore preexisting value
			window[ callbackName ] = overwritten;

			// Save back as free
			if ( s[ callbackName ] ) {
				// make sure that re-using the options doesn't screw things around
				s.jsonpCallback = originalSettings.jsonpCallback;

				// save the callback name for future use
				oldCallbacks.push( callbackName );
			}

			// Call if it was a function and we have a response
			if ( responseContainer && jQuery.isFunction( overwritten ) ) {
				overwritten( responseContainer[ 0 ] );
			}

			responseContainer = overwritten = undefined;
		});

		// Delegate to script
		return "script";
	}
});




// data: string of html
// context (optional): If specified, the fragment will be created in this context, defaults to document
// keepScripts (optional): If true, will include scripts passed in the html string
jQuery.parseHTML = function( data, context, keepScripts ) {
	if ( !data || typeof data !== "string" ) {
		return null;
	}
	if ( typeof context === "boolean" ) {
		keepScripts = context;
		context = false;
	}
	context = context || document;

	var parsed = rsingleTag.exec( data ),
		scripts = !keepScripts && [];

	// Single tag
	if ( parsed ) {
		return [ context.createElement( parsed[1] ) ];
	}

	parsed = jQuery.buildFragment( [ data ], context, scripts );

	if ( scripts && scripts.length ) {
		jQuery( scripts ).remove();
	}

	return jQuery.merge( [], parsed.childNodes );
};


// Keep a copy of the old load method
var _load = jQuery.fn.load;

/**
 * Load a url into a page
 */
jQuery.fn.load = function( url, params, callback ) {
	if ( typeof url !== "string" && _load ) {
		return _load.apply( this, arguments );
	}

	var selector, type, response,
		self = this,
		off = url.indexOf(" ");

	if ( off >= 0 ) {
		selector = jQuery.trim( url.slice( off ) );
		url = url.slice( 0, off );
	}

	// If it's a function
	if ( jQuery.isFunction( params ) ) {

		// We assume that it's the callback
		callback = params;
		params = undefined;

	// Otherwise, build a param string
	} else if ( params && typeof params === "object" ) {
		type = "POST";
	}

	// If we have elements to modify, make the request
	if ( self.length > 0 ) {
		jQuery.ajax({
			url: url,

			// if "type" variable is undefined, then "GET" method will be used
			type: type,
			dataType: "html",
			data: params
		}).done(function( responseText ) {

			// Save response for use in complete callback
			response = arguments;

			self.html( selector ?

				// If a selector was specified, locate the right elements in a dummy div
				// Exclude scripts to avoid IE 'Permission Denied' errors
				jQuery("<div>").append( jQuery.parseHTML( responseText ) ).find( selector ) :

				// Otherwise use the full result
				responseText );

		}).complete( callback && function( jqXHR, status ) {
			self.each( callback, response || [ jqXHR.responseText, status, jqXHR ] );
		});
	}

	return this;
};




jQuery.expr.filters.animated = function( elem ) {
	return jQuery.grep(jQuery.timers, function( fn ) {
		return elem === fn.elem;
	}).length;
};




var docElem = window.document.documentElement;

/**
 * Gets a window from an element
 */
function getWindow( elem ) {
	return jQuery.isWindow( elem ) ? elem : elem.nodeType === 9 && elem.defaultView;
}

jQuery.offset = {
	setOffset: function( elem, options, i ) {
		var curPosition, curLeft, curCSSTop, curTop, curOffset, curCSSLeft, calculatePosition,
			position = jQuery.css( elem, "position" ),
			curElem = jQuery( elem ),
			props = {};

		// Set position first, in-case top/left are set even on static elem
		if ( position === "static" ) {
			elem.style.position = "relative";
		}

		curOffset = curElem.offset();
		curCSSTop = jQuery.css( elem, "top" );
		curCSSLeft = jQuery.css( elem, "left" );
		calculatePosition = ( position === "absolute" || position === "fixed" ) &&
			( curCSSTop + curCSSLeft ).indexOf("auto") > -1;

		// Need to be able to calculate position if either top or left is auto and position is either absolute or fixed
		if ( calculatePosition ) {
			curPosition = curElem.position();
			curTop = curPosition.top;
			curLeft = curPosition.left;

		} else {
			curTop = parseFloat( curCSSTop ) || 0;
			curLeft = parseFloat( curCSSLeft ) || 0;
		}

		if ( jQuery.isFunction( options ) ) {
			options = options.call( elem, i, curOffset );
		}

		if ( options.top != null ) {
			props.top = ( options.top - curOffset.top ) + curTop;
		}
		if ( options.left != null ) {
			props.left = ( options.left - curOffset.left ) + curLeft;
		}

		if ( "using" in options ) {
			options.using.call( elem, props );

		} else {
			curElem.css( props );
		}
	}
};

jQuery.fn.extend({
	offset: function( options ) {
		if ( arguments.length ) {
			return options === undefined ?
				this :
				this.each(function( i ) {
					jQuery.offset.setOffset( this, options, i );
				});
		}

		var docElem, win,
			elem = this[ 0 ],
			box = { top: 0, left: 0 },
			doc = elem && elem.ownerDocument;

		if ( !doc ) {
			return;
		}

		docElem = doc.documentElement;

		// Make sure it's not a disconnected DOM node
		if ( !jQuery.contains( docElem, elem ) ) {
			return box;
		}

		// If we don't have gBCR, just use 0,0 rather than error
		// BlackBerry 5, iOS 3 (original iPhone)
		if ( typeof elem.getBoundingClientRect !== strundefined ) {
			box = elem.getBoundingClientRect();
		}
		win = getWindow( doc );
		return {
			top: box.top + win.pageYOffset - docElem.clientTop,
			left: box.left + win.pageXOffset - docElem.clientLeft
		};
	},

	position: function() {
		if ( !this[ 0 ] ) {
			return;
		}

		var offsetParent, offset,
			elem = this[ 0 ],
			parentOffset = { top: 0, left: 0 };

		// Fixed elements are offset from window (parentOffset = {top:0, left: 0}, because it is its only offset parent
		if ( jQuery.css( elem, "position" ) === "fixed" ) {
			// We assume that getBoundingClientRect is available when computed position is fixed
			offset = elem.getBoundingClientRect();

		} else {
			// Get *real* offsetParent
			offsetParent = this.offsetParent();

			// Get correct offsets
			offset = this.offset();
			if ( !jQuery.nodeName( offsetParent[ 0 ], "html" ) ) {
				parentOffset = offsetParent.offset();
			}

			// Add offsetParent borders
			parentOffset.top += jQuery.css( offsetParent[ 0 ], "borderTopWidth", true );
			parentOffset.left += jQuery.css( offsetParent[ 0 ], "borderLeftWidth", true );
		}

		// Subtract parent offsets and element margins
		return {
			top: offset.top - parentOffset.top - jQuery.css( elem, "marginTop", true ),
			left: offset.left - parentOffset.left - jQuery.css( elem, "marginLeft", true )
		};
	},

	offsetParent: function() {
		return this.map(function() {
			var offsetParent = this.offsetParent || docElem;

			while ( offsetParent && ( !jQuery.nodeName( offsetParent, "html" ) && jQuery.css( offsetParent, "position" ) === "static" ) ) {
				offsetParent = offsetParent.offsetParent;
			}

			return offsetParent || docElem;
		});
	}
});

// Create scrollLeft and scrollTop methods
jQuery.each( { scrollLeft: "pageXOffset", scrollTop: "pageYOffset" }, function( method, prop ) {
	var top = "pageYOffset" === prop;

	jQuery.fn[ method ] = function( val ) {
		return access( this, function( elem, method, val ) {
			var win = getWindow( elem );

			if ( val === undefined ) {
				return win ? win[ prop ] : elem[ method ];
			}

			if ( win ) {
				win.scrollTo(
					!top ? val : window.pageXOffset,
					top ? val : window.pageYOffset
				);

			} else {
				elem[ method ] = val;
			}
		}, method, val, arguments.length, null );
	};
});

// Add the top/left cssHooks using jQuery.fn.position
// Webkit bug: https://bugs.webkit.org/show_bug.cgi?id=29084
// getComputedStyle returns percent when specified for top/left/bottom/right
// rather than make the css module depend on the offset module, we just check for it here
jQuery.each( [ "top", "left" ], function( i, prop ) {
	jQuery.cssHooks[ prop ] = addGetHookIf( support.pixelPosition,
		function( elem, computed ) {
			if ( computed ) {
				computed = curCSS( elem, prop );
				// if curCSS returns percentage, fallback to offset
				return rnumnonpx.test( computed ) ?
					jQuery( elem ).position()[ prop ] + "px" :
					computed;
			}
		}
	);
});


// Create innerHeight, innerWidth, height, width, outerHeight and outerWidth methods
jQuery.each( { Height: "height", Width: "width" }, function( name, type ) {
	jQuery.each( { padding: "inner" + name, content: type, "": "outer" + name }, function( defaultExtra, funcName ) {
		// margin is only for outerHeight, outerWidth
		jQuery.fn[ funcName ] = function( margin, value ) {
			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height],
					// whichever is greatest
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
	});
});


// The number of elements contained in the matched element set
jQuery.fn.size = function() {
	return this.length;
};

jQuery.fn.andSelf = jQuery.fn.addBack;




// Register as a named AMD module, since jQuery can be concatenated with other
// files that may use define, but not via a proper concatenation script that
// understands anonymous AMD modules. A named AMD is safest and most robust
// way to register. Lowercase jquery is used because AMD module names are
// derived from file names, and jQuery is normally delivered in a lowercase
// file name. Do this after creating the global so that if an AMD module wants
// to call noConflict to hide this version of jQuery, it will work.

// Note that for maximum portability, libraries that are not jQuery should
// declare themselves as anonymous modules, and avoid setting a global if an
// AMD loader is present. jQuery is a special case. For more information, see
// https://github.com/jrburke/requirejs/wiki/Updating-existing-libraries#wiki-anon

if ( typeof define === "function" && define.amd ) {
	define( "jquery", [], function() {
		return jQuery;
	});
}




var
	// Map over jQuery in case of overwrite
	_jQuery = window.jQuery,

	// Map over the $ in case of overwrite
	_$ = window.$;

jQuery.noConflict = function( deep ) {
	if ( window.$ === jQuery ) {
		window.$ = _$;
	}

	if ( deep && window.jQuery === jQuery ) {
		window.jQuery = _jQuery;
	}

	return jQuery;
};

// Expose jQuery and $ identifiers, even in
// AMD (#7102#comment:10, https://github.com/jquery/jquery/pull/557)
// and CommonJS for browser emulators (#13566)
if ( typeof noGlobal === strundefined ) {
	window.jQuery = window.$ = jQuery;
}




return jQuery;

}));

},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\moment\\moment.js":[function(require,module,exports){
(function (global){
//! moment.js
//! version : 2.8.3
//! authors : Tim Wood, Iskren Chernev, Moment.js contributors
//! license : MIT
//! momentjs.com

(function (undefined) {
    /************************************
        Constants
    ************************************/

    var moment,
        VERSION = '2.8.3',
        // the global-scope this is NOT the global object in Node.js
        globalScope = typeof global !== 'undefined' ? global : this,
        oldGlobalMoment,
        round = Math.round,
        hasOwnProperty = Object.prototype.hasOwnProperty,
        i,

        YEAR = 0,
        MONTH = 1,
        DATE = 2,
        HOUR = 3,
        MINUTE = 4,
        SECOND = 5,
        MILLISECOND = 6,

        // internal storage for locale config files
        locales = {},

        // extra moment internal properties (plugins register props here)
        momentProperties = [],

        // check for nodeJS
        hasModule = (typeof module !== 'undefined' && module.exports),

        // ASP.NET json date format regex
        aspNetJsonRegex = /^\/?Date\((\-?\d+)/i,
        aspNetTimeSpanJsonRegex = /(\-)?(?:(\d*)\.)?(\d+)\:(\d+)(?:\:(\d+)\.?(\d{3})?)?/,

        // from http://docs.closure-library.googlecode.com/git/closure_goog_date_date.js.source.html
        // somewhat more in line with 4.4.3.2 2004 spec, but allows decimal anywhere
        isoDurationRegex = /^(-)?P(?:(?:([0-9,.]*)Y)?(?:([0-9,.]*)M)?(?:([0-9,.]*)D)?(?:T(?:([0-9,.]*)H)?(?:([0-9,.]*)M)?(?:([0-9,.]*)S)?)?|([0-9,.]*)W)$/,

        // format tokens
        formattingTokens = /(\[[^\[]*\])|(\\)?(Mo|MM?M?M?|Do|DDDo|DD?D?D?|ddd?d?|do?|w[o|w]?|W[o|W]?|Q|YYYYYY|YYYYY|YYYY|YY|gg(ggg?)?|GG(GGG?)?|e|E|a|A|hh?|HH?|mm?|ss?|S{1,4}|X|zz?|ZZ?|.)/g,
        localFormattingTokens = /(\[[^\[]*\])|(\\)?(LT|LL?L?L?|l{1,4})/g,

        // parsing token regexes
        parseTokenOneOrTwoDigits = /\d\d?/, // 0 - 99
        parseTokenOneToThreeDigits = /\d{1,3}/, // 0 - 999
        parseTokenOneToFourDigits = /\d{1,4}/, // 0 - 9999
        parseTokenOneToSixDigits = /[+\-]?\d{1,6}/, // -999,999 - 999,999
        parseTokenDigits = /\d+/, // nonzero number of digits
        parseTokenWord = /[0-9]*['a-z\u00A0-\u05FF\u0700-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+|[\u0600-\u06FF\/]+(\s*?[\u0600-\u06FF]+){1,2}/i, // any word (or two) characters or numbers including two/three word month in arabic.
        parseTokenTimezone = /Z|[\+\-]\d\d:?\d\d/gi, // +00:00 -00:00 +0000 -0000 or Z
        parseTokenT = /T/i, // T (ISO separator)
        parseTokenTimestampMs = /[\+\-]?\d+(\.\d{1,3})?/, // 123456789 123456789.123
        parseTokenOrdinal = /\d{1,2}/,

        //strict parsing regexes
        parseTokenOneDigit = /\d/, // 0 - 9
        parseTokenTwoDigits = /\d\d/, // 00 - 99
        parseTokenThreeDigits = /\d{3}/, // 000 - 999
        parseTokenFourDigits = /\d{4}/, // 0000 - 9999
        parseTokenSixDigits = /[+-]?\d{6}/, // -999,999 - 999,999
        parseTokenSignedNumber = /[+-]?\d+/, // -inf - inf

        // iso 8601 regex
        // 0000-00-00 0000-W00 or 0000-W00-0 + T + 00 or 00:00 or 00:00:00 or 00:00:00.000 + +00:00 or +0000 or +00)
        isoRegex = /^\s*(?:[+-]\d{6}|\d{4})-(?:(\d\d-\d\d)|(W\d\d$)|(W\d\d-\d)|(\d\d\d))((T| )(\d\d(:\d\d(:\d\d(\.\d+)?)?)?)?([\+\-]\d\d(?::?\d\d)?|\s*Z)?)?$/,

        isoFormat = 'YYYY-MM-DDTHH:mm:ssZ',

        isoDates = [
            ['YYYYYY-MM-DD', /[+-]\d{6}-\d{2}-\d{2}/],
            ['YYYY-MM-DD', /\d{4}-\d{2}-\d{2}/],
            ['GGGG-[W]WW-E', /\d{4}-W\d{2}-\d/],
            ['GGGG-[W]WW', /\d{4}-W\d{2}/],
            ['YYYY-DDD', /\d{4}-\d{3}/]
        ],

        // iso time formats and regexes
        isoTimes = [
            ['HH:mm:ss.SSSS', /(T| )\d\d:\d\d:\d\d\.\d+/],
            ['HH:mm:ss', /(T| )\d\d:\d\d:\d\d/],
            ['HH:mm', /(T| )\d\d:\d\d/],
            ['HH', /(T| )\d\d/]
        ],

        // timezone chunker '+10:00' > ['10', '00'] or '-1530' > ['-15', '30']
        parseTimezoneChunker = /([\+\-]|\d\d)/gi,

        // getter and setter names
        proxyGettersAndSetters = 'Date|Hours|Minutes|Seconds|Milliseconds'.split('|'),
        unitMillisecondFactors = {
            'Milliseconds' : 1,
            'Seconds' : 1e3,
            'Minutes' : 6e4,
            'Hours' : 36e5,
            'Days' : 864e5,
            'Months' : 2592e6,
            'Years' : 31536e6
        },

        unitAliases = {
            ms : 'millisecond',
            s : 'second',
            m : 'minute',
            h : 'hour',
            d : 'day',
            D : 'date',
            w : 'week',
            W : 'isoWeek',
            M : 'month',
            Q : 'quarter',
            y : 'year',
            DDD : 'dayOfYear',
            e : 'weekday',
            E : 'isoWeekday',
            gg: 'weekYear',
            GG: 'isoWeekYear'
        },

        camelFunctions = {
            dayofyear : 'dayOfYear',
            isoweekday : 'isoWeekday',
            isoweek : 'isoWeek',
            weekyear : 'weekYear',
            isoweekyear : 'isoWeekYear'
        },

        // format function strings
        formatFunctions = {},

        // default relative time thresholds
        relativeTimeThresholds = {
            s: 45,  // seconds to minute
            m: 45,  // minutes to hour
            h: 22,  // hours to day
            d: 26,  // days to month
            M: 11   // months to year
        },

        // tokens to ordinalize and pad
        ordinalizeTokens = 'DDD w W M D d'.split(' '),
        paddedTokens = 'M D H h m s w W'.split(' '),

        formatTokenFunctions = {
            M    : function () {
                return this.month() + 1;
            },
            MMM  : function (format) {
                return this.localeData().monthsShort(this, format);
            },
            MMMM : function (format) {
                return this.localeData().months(this, format);
            },
            D    : function () {
                return this.date();
            },
            DDD  : function () {
                return this.dayOfYear();
            },
            d    : function () {
                return this.day();
            },
            dd   : function (format) {
                return this.localeData().weekdaysMin(this, format);
            },
            ddd  : function (format) {
                return this.localeData().weekdaysShort(this, format);
            },
            dddd : function (format) {
                return this.localeData().weekdays(this, format);
            },
            w    : function () {
                return this.week();
            },
            W    : function () {
                return this.isoWeek();
            },
            YY   : function () {
                return leftZeroFill(this.year() % 100, 2);
            },
            YYYY : function () {
                return leftZeroFill(this.year(), 4);
            },
            YYYYY : function () {
                return leftZeroFill(this.year(), 5);
            },
            YYYYYY : function () {
                var y = this.year(), sign = y >= 0 ? '+' : '-';
                return sign + leftZeroFill(Math.abs(y), 6);
            },
            gg   : function () {
                return leftZeroFill(this.weekYear() % 100, 2);
            },
            gggg : function () {
                return leftZeroFill(this.weekYear(), 4);
            },
            ggggg : function () {
                return leftZeroFill(this.weekYear(), 5);
            },
            GG   : function () {
                return leftZeroFill(this.isoWeekYear() % 100, 2);
            },
            GGGG : function () {
                return leftZeroFill(this.isoWeekYear(), 4);
            },
            GGGGG : function () {
                return leftZeroFill(this.isoWeekYear(), 5);
            },
            e : function () {
                return this.weekday();
            },
            E : function () {
                return this.isoWeekday();
            },
            a    : function () {
                return this.localeData().meridiem(this.hours(), this.minutes(), true);
            },
            A    : function () {
                return this.localeData().meridiem(this.hours(), this.minutes(), false);
            },
            H    : function () {
                return this.hours();
            },
            h    : function () {
                return this.hours() % 12 || 12;
            },
            m    : function () {
                return this.minutes();
            },
            s    : function () {
                return this.seconds();
            },
            S    : function () {
                return toInt(this.milliseconds() / 100);
            },
            SS   : function () {
                return leftZeroFill(toInt(this.milliseconds() / 10), 2);
            },
            SSS  : function () {
                return leftZeroFill(this.milliseconds(), 3);
            },
            SSSS : function () {
                return leftZeroFill(this.milliseconds(), 3);
            },
            Z    : function () {
                var a = -this.zone(),
                    b = '+';
                if (a < 0) {
                    a = -a;
                    b = '-';
                }
                return b + leftZeroFill(toInt(a / 60), 2) + ':' + leftZeroFill(toInt(a) % 60, 2);
            },
            ZZ   : function () {
                var a = -this.zone(),
                    b = '+';
                if (a < 0) {
                    a = -a;
                    b = '-';
                }
                return b + leftZeroFill(toInt(a / 60), 2) + leftZeroFill(toInt(a) % 60, 2);
            },
            z : function () {
                return this.zoneAbbr();
            },
            zz : function () {
                return this.zoneName();
            },
            X    : function () {
                return this.unix();
            },
            Q : function () {
                return this.quarter();
            }
        },

        deprecations = {},

        lists = ['months', 'monthsShort', 'weekdays', 'weekdaysShort', 'weekdaysMin'];

    // Pick the first defined of two or three arguments. dfl comes from
    // default.
    function dfl(a, b, c) {
        switch (arguments.length) {
            case 2: return a != null ? a : b;
            case 3: return a != null ? a : b != null ? b : c;
            default: throw new Error('Implement me');
        }
    }

    function hasOwnProp(a, b) {
        return hasOwnProperty.call(a, b);
    }

    function defaultParsingFlags() {
        // We need to deep clone this object, and es5 standard is not very
        // helpful.
        return {
            empty : false,
            unusedTokens : [],
            unusedInput : [],
            overflow : -2,
            charsLeftOver : 0,
            nullInput : false,
            invalidMonth : null,
            invalidFormat : false,
            userInvalidated : false,
            iso: false
        };
    }

    function printMsg(msg) {
        if (moment.suppressDeprecationWarnings === false &&
                typeof console !== 'undefined' && console.warn) {
            console.warn('Deprecation warning: ' + msg);
        }
    }

    function deprecate(msg, fn) {
        var firstTime = true;
        return extend(function () {
            if (firstTime) {
                printMsg(msg);
                firstTime = false;
            }
            return fn.apply(this, arguments);
        }, fn);
    }

    function deprecateSimple(name, msg) {
        if (!deprecations[name]) {
            printMsg(msg);
            deprecations[name] = true;
        }
    }

    function padToken(func, count) {
        return function (a) {
            return leftZeroFill(func.call(this, a), count);
        };
    }
    function ordinalizeToken(func, period) {
        return function (a) {
            return this.localeData().ordinal(func.call(this, a), period);
        };
    }

    while (ordinalizeTokens.length) {
        i = ordinalizeTokens.pop();
        formatTokenFunctions[i + 'o'] = ordinalizeToken(formatTokenFunctions[i], i);
    }
    while (paddedTokens.length) {
        i = paddedTokens.pop();
        formatTokenFunctions[i + i] = padToken(formatTokenFunctions[i], 2);
    }
    formatTokenFunctions.DDDD = padToken(formatTokenFunctions.DDD, 3);


    /************************************
        Constructors
    ************************************/

    function Locale() {
    }

    // Moment prototype object
    function Moment(config, skipOverflow) {
        if (skipOverflow !== false) {
            checkOverflow(config);
        }
        copyConfig(this, config);
        this._d = new Date(+config._d);
    }

    // Duration Constructor
    function Duration(duration) {
        var normalizedInput = normalizeObjectUnits(duration),
            years = normalizedInput.year || 0,
            quarters = normalizedInput.quarter || 0,
            months = normalizedInput.month || 0,
            weeks = normalizedInput.week || 0,
            days = normalizedInput.day || 0,
            hours = normalizedInput.hour || 0,
            minutes = normalizedInput.minute || 0,
            seconds = normalizedInput.second || 0,
            milliseconds = normalizedInput.millisecond || 0;

        // representation for dateAddRemove
        this._milliseconds = +milliseconds +
            seconds * 1e3 + // 1000
            minutes * 6e4 + // 1000 * 60
            hours * 36e5; // 1000 * 60 * 60
        // Because of dateAddRemove treats 24 hours as different from a
        // day when working around DST, we need to store them separately
        this._days = +days +
            weeks * 7;
        // It is impossible translate months into days without knowing
        // which months you are are talking about, so we have to store
        // it separately.
        this._months = +months +
            quarters * 3 +
            years * 12;

        this._data = {};

        this._locale = moment.localeData();

        this._bubble();
    }

    /************************************
        Helpers
    ************************************/


    function extend(a, b) {
        for (var i in b) {
            if (hasOwnProp(b, i)) {
                a[i] = b[i];
            }
        }

        if (hasOwnProp(b, 'toString')) {
            a.toString = b.toString;
        }

        if (hasOwnProp(b, 'valueOf')) {
            a.valueOf = b.valueOf;
        }

        return a;
    }

    function copyConfig(to, from) {
        var i, prop, val;

        if (typeof from._isAMomentObject !== 'undefined') {
            to._isAMomentObject = from._isAMomentObject;
        }
        if (typeof from._i !== 'undefined') {
            to._i = from._i;
        }
        if (typeof from._f !== 'undefined') {
            to._f = from._f;
        }
        if (typeof from._l !== 'undefined') {
            to._l = from._l;
        }
        if (typeof from._strict !== 'undefined') {
            to._strict = from._strict;
        }
        if (typeof from._tzm !== 'undefined') {
            to._tzm = from._tzm;
        }
        if (typeof from._isUTC !== 'undefined') {
            to._isUTC = from._isUTC;
        }
        if (typeof from._offset !== 'undefined') {
            to._offset = from._offset;
        }
        if (typeof from._pf !== 'undefined') {
            to._pf = from._pf;
        }
        if (typeof from._locale !== 'undefined') {
            to._locale = from._locale;
        }

        if (momentProperties.length > 0) {
            for (i in momentProperties) {
                prop = momentProperties[i];
                val = from[prop];
                if (typeof val !== 'undefined') {
                    to[prop] = val;
                }
            }
        }

        return to;
    }

    function absRound(number) {
        if (number < 0) {
            return Math.ceil(number);
        } else {
            return Math.floor(number);
        }
    }

    // left zero fill a number
    // see http://jsperf.com/left-zero-filling for performance comparison
    function leftZeroFill(number, targetLength, forceSign) {
        var output = '' + Math.abs(number),
            sign = number >= 0;

        while (output.length < targetLength) {
            output = '0' + output;
        }
        return (sign ? (forceSign ? '+' : '') : '-') + output;
    }

    function positiveMomentsDifference(base, other) {
        var res = {milliseconds: 0, months: 0};

        res.months = other.month() - base.month() +
            (other.year() - base.year()) * 12;
        if (base.clone().add(res.months, 'M').isAfter(other)) {
            --res.months;
        }

        res.milliseconds = +other - +(base.clone().add(res.months, 'M'));

        return res;
    }

    function momentsDifference(base, other) {
        var res;
        other = makeAs(other, base);
        if (base.isBefore(other)) {
            res = positiveMomentsDifference(base, other);
        } else {
            res = positiveMomentsDifference(other, base);
            res.milliseconds = -res.milliseconds;
            res.months = -res.months;
        }

        return res;
    }

    // TODO: remove 'name' arg after deprecation is removed
    function createAdder(direction, name) {
        return function (val, period) {
            var dur, tmp;
            //invert the arguments, but complain about it
            if (period !== null && !isNaN(+period)) {
                deprecateSimple(name, 'moment().' + name  + '(period, number) is deprecated. Please use moment().' + name + '(number, period).');
                tmp = val; val = period; period = tmp;
            }

            val = typeof val === 'string' ? +val : val;
            dur = moment.duration(val, period);
            addOrSubtractDurationFromMoment(this, dur, direction);
            return this;
        };
    }

    function addOrSubtractDurationFromMoment(mom, duration, isAdding, updateOffset) {
        var milliseconds = duration._milliseconds,
            days = duration._days,
            months = duration._months;
        updateOffset = updateOffset == null ? true : updateOffset;

        if (milliseconds) {
            mom._d.setTime(+mom._d + milliseconds * isAdding);
        }
        if (days) {
            rawSetter(mom, 'Date', rawGetter(mom, 'Date') + days * isAdding);
        }
        if (months) {
            rawMonthSetter(mom, rawGetter(mom, 'Month') + months * isAdding);
        }
        if (updateOffset) {
            moment.updateOffset(mom, days || months);
        }
    }

    // check if is an array
    function isArray(input) {
        return Object.prototype.toString.call(input) === '[object Array]';
    }

    function isDate(input) {
        return Object.prototype.toString.call(input) === '[object Date]' ||
            input instanceof Date;
    }

    // compare two arrays, return the number of differences
    function compareArrays(array1, array2, dontConvert) {
        var len = Math.min(array1.length, array2.length),
            lengthDiff = Math.abs(array1.length - array2.length),
            diffs = 0,
            i;
        for (i = 0; i < len; i++) {
            if ((dontConvert && array1[i] !== array2[i]) ||
                (!dontConvert && toInt(array1[i]) !== toInt(array2[i]))) {
                diffs++;
            }
        }
        return diffs + lengthDiff;
    }

    function normalizeUnits(units) {
        if (units) {
            var lowered = units.toLowerCase().replace(/(.)s$/, '$1');
            units = unitAliases[units] || camelFunctions[lowered] || lowered;
        }
        return units;
    }

    function normalizeObjectUnits(inputObject) {
        var normalizedInput = {},
            normalizedProp,
            prop;

        for (prop in inputObject) {
            if (hasOwnProp(inputObject, prop)) {
                normalizedProp = normalizeUnits(prop);
                if (normalizedProp) {
                    normalizedInput[normalizedProp] = inputObject[prop];
                }
            }
        }

        return normalizedInput;
    }

    function makeList(field) {
        var count, setter;

        if (field.indexOf('week') === 0) {
            count = 7;
            setter = 'day';
        }
        else if (field.indexOf('month') === 0) {
            count = 12;
            setter = 'month';
        }
        else {
            return;
        }

        moment[field] = function (format, index) {
            var i, getter,
                method = moment._locale[field],
                results = [];

            if (typeof format === 'number') {
                index = format;
                format = undefined;
            }

            getter = function (i) {
                var m = moment().utc().set(setter, i);
                return method.call(moment._locale, m, format || '');
            };

            if (index != null) {
                return getter(index);
            }
            else {
                for (i = 0; i < count; i++) {
                    results.push(getter(i));
                }
                return results;
            }
        };
    }

    function toInt(argumentForCoercion) {
        var coercedNumber = +argumentForCoercion,
            value = 0;

        if (coercedNumber !== 0 && isFinite(coercedNumber)) {
            if (coercedNumber >= 0) {
                value = Math.floor(coercedNumber);
            } else {
                value = Math.ceil(coercedNumber);
            }
        }

        return value;
    }

    function daysInMonth(year, month) {
        return new Date(Date.UTC(year, month + 1, 0)).getUTCDate();
    }

    function weeksInYear(year, dow, doy) {
        return weekOfYear(moment([year, 11, 31 + dow - doy]), dow, doy).week;
    }

    function daysInYear(year) {
        return isLeapYear(year) ? 366 : 365;
    }

    function isLeapYear(year) {
        return (year % 4 === 0 && year % 100 !== 0) || year % 400 === 0;
    }

    function checkOverflow(m) {
        var overflow;
        if (m._a && m._pf.overflow === -2) {
            overflow =
                m._a[MONTH] < 0 || m._a[MONTH] > 11 ? MONTH :
                m._a[DATE] < 1 || m._a[DATE] > daysInMonth(m._a[YEAR], m._a[MONTH]) ? DATE :
                m._a[HOUR] < 0 || m._a[HOUR] > 23 ? HOUR :
                m._a[MINUTE] < 0 || m._a[MINUTE] > 59 ? MINUTE :
                m._a[SECOND] < 0 || m._a[SECOND] > 59 ? SECOND :
                m._a[MILLISECOND] < 0 || m._a[MILLISECOND] > 999 ? MILLISECOND :
                -1;

            if (m._pf._overflowDayOfYear && (overflow < YEAR || overflow > DATE)) {
                overflow = DATE;
            }

            m._pf.overflow = overflow;
        }
    }

    function isValid(m) {
        if (m._isValid == null) {
            m._isValid = !isNaN(m._d.getTime()) &&
                m._pf.overflow < 0 &&
                !m._pf.empty &&
                !m._pf.invalidMonth &&
                !m._pf.nullInput &&
                !m._pf.invalidFormat &&
                !m._pf.userInvalidated;

            if (m._strict) {
                m._isValid = m._isValid &&
                    m._pf.charsLeftOver === 0 &&
                    m._pf.unusedTokens.length === 0;
            }
        }
        return m._isValid;
    }

    function normalizeLocale(key) {
        return key ? key.toLowerCase().replace('_', '-') : key;
    }

    // pick the locale from the array
    // try ['en-au', 'en-gb'] as 'en-au', 'en-gb', 'en', as in move through the list trying each
    // substring from most specific to least, but move to the next array item if it's a more specific variant than the current root
    function chooseLocale(names) {
        var i = 0, j, next, locale, split;

        while (i < names.length) {
            split = normalizeLocale(names[i]).split('-');
            j = split.length;
            next = normalizeLocale(names[i + 1]);
            next = next ? next.split('-') : null;
            while (j > 0) {
                locale = loadLocale(split.slice(0, j).join('-'));
                if (locale) {
                    return locale;
                }
                if (next && next.length >= j && compareArrays(split, next, true) >= j - 1) {
                    //the next array item is better than a shallower substring of this one
                    break;
                }
                j--;
            }
            i++;
        }
        return null;
    }

    function loadLocale(name) {
        var oldLocale = null;
        if (!locales[name] && hasModule) {
            try {
                oldLocale = moment.locale();
                require('./locale/' + name);
                // because defineLocale currently also sets the global locale, we want to undo that for lazy loaded locales
                moment.locale(oldLocale);
            } catch (e) { }
        }
        return locales[name];
    }

    // Return a moment from input, that is local/utc/zone equivalent to model.
    function makeAs(input, model) {
        return model._isUTC ? moment(input).zone(model._offset || 0) :
            moment(input).local();
    }

    /************************************
        Locale
    ************************************/


    extend(Locale.prototype, {

        set : function (config) {
            var prop, i;
            for (i in config) {
                prop = config[i];
                if (typeof prop === 'function') {
                    this[i] = prop;
                } else {
                    this['_' + i] = prop;
                }
            }
        },

        _months : 'January_February_March_April_May_June_July_August_September_October_November_December'.split('_'),
        months : function (m) {
            return this._months[m.month()];
        },

        _monthsShort : 'Jan_Feb_Mar_Apr_May_Jun_Jul_Aug_Sep_Oct_Nov_Dec'.split('_'),
        monthsShort : function (m) {
            return this._monthsShort[m.month()];
        },

        monthsParse : function (monthName) {
            var i, mom, regex;

            if (!this._monthsParse) {
                this._monthsParse = [];
            }

            for (i = 0; i < 12; i++) {
                // make the regex if we don't have it already
                if (!this._monthsParse[i]) {
                    mom = moment.utc([2000, i]);
                    regex = '^' + this.months(mom, '') + '|^' + this.monthsShort(mom, '');
                    this._monthsParse[i] = new RegExp(regex.replace('.', ''), 'i');
                }
                // test the regex
                if (this._monthsParse[i].test(monthName)) {
                    return i;
                }
            }
        },

        _weekdays : 'Sunday_Monday_Tuesday_Wednesday_Thursday_Friday_Saturday'.split('_'),
        weekdays : function (m) {
            return this._weekdays[m.day()];
        },

        _weekdaysShort : 'Sun_Mon_Tue_Wed_Thu_Fri_Sat'.split('_'),
        weekdaysShort : function (m) {
            return this._weekdaysShort[m.day()];
        },

        _weekdaysMin : 'Su_Mo_Tu_We_Th_Fr_Sa'.split('_'),
        weekdaysMin : function (m) {
            return this._weekdaysMin[m.day()];
        },

        weekdaysParse : function (weekdayName) {
            var i, mom, regex;

            if (!this._weekdaysParse) {
                this._weekdaysParse = [];
            }

            for (i = 0; i < 7; i++) {
                // make the regex if we don't have it already
                if (!this._weekdaysParse[i]) {
                    mom = moment([2000, 1]).day(i);
                    regex = '^' + this.weekdays(mom, '') + '|^' + this.weekdaysShort(mom, '') + '|^' + this.weekdaysMin(mom, '');
                    this._weekdaysParse[i] = new RegExp(regex.replace('.', ''), 'i');
                }
                // test the regex
                if (this._weekdaysParse[i].test(weekdayName)) {
                    return i;
                }
            }
        },

        _longDateFormat : {
            LT : 'h:mm A',
            L : 'MM/DD/YYYY',
            LL : 'MMMM D, YYYY',
            LLL : 'MMMM D, YYYY LT',
            LLLL : 'dddd, MMMM D, YYYY LT'
        },
        longDateFormat : function (key) {
            var output = this._longDateFormat[key];
            if (!output && this._longDateFormat[key.toUpperCase()]) {
                output = this._longDateFormat[key.toUpperCase()].replace(/MMMM|MM|DD|dddd/g, function (val) {
                    return val.slice(1);
                });
                this._longDateFormat[key] = output;
            }
            return output;
        },

        isPM : function (input) {
            // IE8 Quirks Mode & IE7 Standards Mode do not allow accessing strings like arrays
            // Using charAt should be more compatible.
            return ((input + '').toLowerCase().charAt(0) === 'p');
        },

        _meridiemParse : /[ap]\.?m?\.?/i,
        meridiem : function (hours, minutes, isLower) {
            if (hours > 11) {
                return isLower ? 'pm' : 'PM';
            } else {
                return isLower ? 'am' : 'AM';
            }
        },

        _calendar : {
            sameDay : '[Today at] LT',
            nextDay : '[Tomorrow at] LT',
            nextWeek : 'dddd [at] LT',
            lastDay : '[Yesterday at] LT',
            lastWeek : '[Last] dddd [at] LT',
            sameElse : 'L'
        },
        calendar : function (key, mom) {
            var output = this._calendar[key];
            return typeof output === 'function' ? output.apply(mom) : output;
        },

        _relativeTime : {
            future : 'in %s',
            past : '%s ago',
            s : 'a few seconds',
            m : 'a minute',
            mm : '%d minutes',
            h : 'an hour',
            hh : '%d hours',
            d : 'a day',
            dd : '%d days',
            M : 'a month',
            MM : '%d months',
            y : 'a year',
            yy : '%d years'
        },

        relativeTime : function (number, withoutSuffix, string, isFuture) {
            var output = this._relativeTime[string];
            return (typeof output === 'function') ?
                output(number, withoutSuffix, string, isFuture) :
                output.replace(/%d/i, number);
        },

        pastFuture : function (diff, output) {
            var format = this._relativeTime[diff > 0 ? 'future' : 'past'];
            return typeof format === 'function' ? format(output) : format.replace(/%s/i, output);
        },

        ordinal : function (number) {
            return this._ordinal.replace('%d', number);
        },
        _ordinal : '%d',

        preparse : function (string) {
            return string;
        },

        postformat : function (string) {
            return string;
        },

        week : function (mom) {
            return weekOfYear(mom, this._week.dow, this._week.doy).week;
        },

        _week : {
            dow : 0, // Sunday is the first day of the week.
            doy : 6  // The week that contains Jan 1st is the first week of the year.
        },

        _invalidDate: 'Invalid date',
        invalidDate: function () {
            return this._invalidDate;
        }
    });

    /************************************
        Formatting
    ************************************/


    function removeFormattingTokens(input) {
        if (input.match(/\[[\s\S]/)) {
            return input.replace(/^\[|\]$/g, '');
        }
        return input.replace(/\\/g, '');
    }

    function makeFormatFunction(format) {
        var array = format.match(formattingTokens), i, length;

        for (i = 0, length = array.length; i < length; i++) {
            if (formatTokenFunctions[array[i]]) {
                array[i] = formatTokenFunctions[array[i]];
            } else {
                array[i] = removeFormattingTokens(array[i]);
            }
        }

        return function (mom) {
            var output = '';
            for (i = 0; i < length; i++) {
                output += array[i] instanceof Function ? array[i].call(mom, format) : array[i];
            }
            return output;
        };
    }

    // format date using native date object
    function formatMoment(m, format) {
        if (!m.isValid()) {
            return m.localeData().invalidDate();
        }

        format = expandFormat(format, m.localeData());

        if (!formatFunctions[format]) {
            formatFunctions[format] = makeFormatFunction(format);
        }

        return formatFunctions[format](m);
    }

    function expandFormat(format, locale) {
        var i = 5;

        function replaceLongDateFormatTokens(input) {
            return locale.longDateFormat(input) || input;
        }

        localFormattingTokens.lastIndex = 0;
        while (i >= 0 && localFormattingTokens.test(format)) {
            format = format.replace(localFormattingTokens, replaceLongDateFormatTokens);
            localFormattingTokens.lastIndex = 0;
            i -= 1;
        }

        return format;
    }


    /************************************
        Parsing
    ************************************/


    // get the regex to find the next token
    function getParseRegexForToken(token, config) {
        var a, strict = config._strict;
        switch (token) {
        case 'Q':
            return parseTokenOneDigit;
        case 'DDDD':
            return parseTokenThreeDigits;
        case 'YYYY':
        case 'GGGG':
        case 'gggg':
            return strict ? parseTokenFourDigits : parseTokenOneToFourDigits;
        case 'Y':
        case 'G':
        case 'g':
            return parseTokenSignedNumber;
        case 'YYYYYY':
        case 'YYYYY':
        case 'GGGGG':
        case 'ggggg':
            return strict ? parseTokenSixDigits : parseTokenOneToSixDigits;
        case 'S':
            if (strict) {
                return parseTokenOneDigit;
            }
            /* falls through */
        case 'SS':
            if (strict) {
                return parseTokenTwoDigits;
            }
            /* falls through */
        case 'SSS':
            if (strict) {
                return parseTokenThreeDigits;
            }
            /* falls through */
        case 'DDD':
            return parseTokenOneToThreeDigits;
        case 'MMM':
        case 'MMMM':
        case 'dd':
        case 'ddd':
        case 'dddd':
            return parseTokenWord;
        case 'a':
        case 'A':
            return config._locale._meridiemParse;
        case 'X':
            return parseTokenTimestampMs;
        case 'Z':
        case 'ZZ':
            return parseTokenTimezone;
        case 'T':
            return parseTokenT;
        case 'SSSS':
            return parseTokenDigits;
        case 'MM':
        case 'DD':
        case 'YY':
        case 'GG':
        case 'gg':
        case 'HH':
        case 'hh':
        case 'mm':
        case 'ss':
        case 'ww':
        case 'WW':
            return strict ? parseTokenTwoDigits : parseTokenOneOrTwoDigits;
        case 'M':
        case 'D':
        case 'd':
        case 'H':
        case 'h':
        case 'm':
        case 's':
        case 'w':
        case 'W':
        case 'e':
        case 'E':
            return parseTokenOneOrTwoDigits;
        case 'Do':
            return parseTokenOrdinal;
        default :
            a = new RegExp(regexpEscape(unescapeFormat(token.replace('\\', '')), 'i'));
            return a;
        }
    }

    function timezoneMinutesFromString(string) {
        string = string || '';
        var possibleTzMatches = (string.match(parseTokenTimezone) || []),
            tzChunk = possibleTzMatches[possibleTzMatches.length - 1] || [],
            parts = (tzChunk + '').match(parseTimezoneChunker) || ['-', 0, 0],
            minutes = +(parts[1] * 60) + toInt(parts[2]);

        return parts[0] === '+' ? -minutes : minutes;
    }

    // function to convert string input to date
    function addTimeToArrayFromToken(token, input, config) {
        var a, datePartArray = config._a;

        switch (token) {
        // QUARTER
        case 'Q':
            if (input != null) {
                datePartArray[MONTH] = (toInt(input) - 1) * 3;
            }
            break;
        // MONTH
        case 'M' : // fall through to MM
        case 'MM' :
            if (input != null) {
                datePartArray[MONTH] = toInt(input) - 1;
            }
            break;
        case 'MMM' : // fall through to MMMM
        case 'MMMM' :
            a = config._locale.monthsParse(input);
            // if we didn't find a month name, mark the date as invalid.
            if (a != null) {
                datePartArray[MONTH] = a;
            } else {
                config._pf.invalidMonth = input;
            }
            break;
        // DAY OF MONTH
        case 'D' : // fall through to DD
        case 'DD' :
            if (input != null) {
                datePartArray[DATE] = toInt(input);
            }
            break;
        case 'Do' :
            if (input != null) {
                datePartArray[DATE] = toInt(parseInt(input, 10));
            }
            break;
        // DAY OF YEAR
        case 'DDD' : // fall through to DDDD
        case 'DDDD' :
            if (input != null) {
                config._dayOfYear = toInt(input);
            }

            break;
        // YEAR
        case 'YY' :
            datePartArray[YEAR] = moment.parseTwoDigitYear(input);
            break;
        case 'YYYY' :
        case 'YYYYY' :
        case 'YYYYYY' :
            datePartArray[YEAR] = toInt(input);
            break;
        // AM / PM
        case 'a' : // fall through to A
        case 'A' :
            config._isPm = config._locale.isPM(input);
            break;
        // 24 HOUR
        case 'H' : // fall through to hh
        case 'HH' : // fall through to hh
        case 'h' : // fall through to hh
        case 'hh' :
            datePartArray[HOUR] = toInt(input);
            break;
        // MINUTE
        case 'm' : // fall through to mm
        case 'mm' :
            datePartArray[MINUTE] = toInt(input);
            break;
        // SECOND
        case 's' : // fall through to ss
        case 'ss' :
            datePartArray[SECOND] = toInt(input);
            break;
        // MILLISECOND
        case 'S' :
        case 'SS' :
        case 'SSS' :
        case 'SSSS' :
            datePartArray[MILLISECOND] = toInt(('0.' + input) * 1000);
            break;
        // UNIX TIMESTAMP WITH MS
        case 'X':
            config._d = new Date(parseFloat(input) * 1000);
            break;
        // TIMEZONE
        case 'Z' : // fall through to ZZ
        case 'ZZ' :
            config._useUTC = true;
            config._tzm = timezoneMinutesFromString(input);
            break;
        // WEEKDAY - human
        case 'dd':
        case 'ddd':
        case 'dddd':
            a = config._locale.weekdaysParse(input);
            // if we didn't get a weekday name, mark the date as invalid
            if (a != null) {
                config._w = config._w || {};
                config._w['d'] = a;
            } else {
                config._pf.invalidWeekday = input;
            }
            break;
        // WEEK, WEEK DAY - numeric
        case 'w':
        case 'ww':
        case 'W':
        case 'WW':
        case 'd':
        case 'e':
        case 'E':
            token = token.substr(0, 1);
            /* falls through */
        case 'gggg':
        case 'GGGG':
        case 'GGGGG':
            token = token.substr(0, 2);
            if (input) {
                config._w = config._w || {};
                config._w[token] = toInt(input);
            }
            break;
        case 'gg':
        case 'GG':
            config._w = config._w || {};
            config._w[token] = moment.parseTwoDigitYear(input);
        }
    }

    function dayOfYearFromWeekInfo(config) {
        var w, weekYear, week, weekday, dow, doy, temp;

        w = config._w;
        if (w.GG != null || w.W != null || w.E != null) {
            dow = 1;
            doy = 4;

            // TODO: We need to take the current isoWeekYear, but that depends on
            // how we interpret now (local, utc, fixed offset). So create
            // a now version of current config (take local/utc/offset flags, and
            // create now).
            weekYear = dfl(w.GG, config._a[YEAR], weekOfYear(moment(), 1, 4).year);
            week = dfl(w.W, 1);
            weekday = dfl(w.E, 1);
        } else {
            dow = config._locale._week.dow;
            doy = config._locale._week.doy;

            weekYear = dfl(w.gg, config._a[YEAR], weekOfYear(moment(), dow, doy).year);
            week = dfl(w.w, 1);

            if (w.d != null) {
                // weekday -- low day numbers are considered next week
                weekday = w.d;
                if (weekday < dow) {
                    ++week;
                }
            } else if (w.e != null) {
                // local weekday -- counting starts from begining of week
                weekday = w.e + dow;
            } else {
                // default to begining of week
                weekday = dow;
            }
        }
        temp = dayOfYearFromWeeks(weekYear, week, weekday, doy, dow);

        config._a[YEAR] = temp.year;
        config._dayOfYear = temp.dayOfYear;
    }

    // convert an array to a date.
    // the array should mirror the parameters below
    // note: all values past the year are optional and will default to the lowest possible value.
    // [year, month, day , hour, minute, second, millisecond]
    function dateFromConfig(config) {
        var i, date, input = [], currentDate, yearToUse;

        if (config._d) {
            return;
        }

        currentDate = currentDateArray(config);

        //compute day of the year from weeks and weekdays
        if (config._w && config._a[DATE] == null && config._a[MONTH] == null) {
            dayOfYearFromWeekInfo(config);
        }

        //if the day of the year is set, figure out what it is
        if (config._dayOfYear) {
            yearToUse = dfl(config._a[YEAR], currentDate[YEAR]);

            if (config._dayOfYear > daysInYear(yearToUse)) {
                config._pf._overflowDayOfYear = true;
            }

            date = makeUTCDate(yearToUse, 0, config._dayOfYear);
            config._a[MONTH] = date.getUTCMonth();
            config._a[DATE] = date.getUTCDate();
        }

        // Default to current date.
        // * if no year, month, day of month are given, default to today
        // * if day of month is given, default month and year
        // * if month is given, default only year
        // * if year is given, don't default anything
        for (i = 0; i < 3 && config._a[i] == null; ++i) {
            config._a[i] = input[i] = currentDate[i];
        }

        // Zero out whatever was not defaulted, including time
        for (; i < 7; i++) {
            config._a[i] = input[i] = (config._a[i] == null) ? (i === 2 ? 1 : 0) : config._a[i];
        }

        config._d = (config._useUTC ? makeUTCDate : makeDate).apply(null, input);
        // Apply timezone offset from input. The actual zone can be changed
        // with parseZone.
        if (config._tzm != null) {
            config._d.setUTCMinutes(config._d.getUTCMinutes() + config._tzm);
        }
    }

    function dateFromObject(config) {
        var normalizedInput;

        if (config._d) {
            return;
        }

        normalizedInput = normalizeObjectUnits(config._i);
        config._a = [
            normalizedInput.year,
            normalizedInput.month,
            normalizedInput.day,
            normalizedInput.hour,
            normalizedInput.minute,
            normalizedInput.second,
            normalizedInput.millisecond
        ];

        dateFromConfig(config);
    }

    function currentDateArray(config) {
        var now = new Date();
        if (config._useUTC) {
            return [
                now.getUTCFullYear(),
                now.getUTCMonth(),
                now.getUTCDate()
            ];
        } else {
            return [now.getFullYear(), now.getMonth(), now.getDate()];
        }
    }

    // date from string and format string
    function makeDateFromStringAndFormat(config) {
        if (config._f === moment.ISO_8601) {
            parseISO(config);
            return;
        }

        config._a = [];
        config._pf.empty = true;

        // This array is used to make a Date, either with `new Date` or `Date.UTC`
        var string = '' + config._i,
            i, parsedInput, tokens, token, skipped,
            stringLength = string.length,
            totalParsedInputLength = 0;

        tokens = expandFormat(config._f, config._locale).match(formattingTokens) || [];

        for (i = 0; i < tokens.length; i++) {
            token = tokens[i];
            parsedInput = (string.match(getParseRegexForToken(token, config)) || [])[0];
            if (parsedInput) {
                skipped = string.substr(0, string.indexOf(parsedInput));
                if (skipped.length > 0) {
                    config._pf.unusedInput.push(skipped);
                }
                string = string.slice(string.indexOf(parsedInput) + parsedInput.length);
                totalParsedInputLength += parsedInput.length;
            }
            // don't parse if it's not a known token
            if (formatTokenFunctions[token]) {
                if (parsedInput) {
                    config._pf.empty = false;
                }
                else {
                    config._pf.unusedTokens.push(token);
                }
                addTimeToArrayFromToken(token, parsedInput, config);
            }
            else if (config._strict && !parsedInput) {
                config._pf.unusedTokens.push(token);
            }
        }

        // add remaining unparsed input length to the string
        config._pf.charsLeftOver = stringLength - totalParsedInputLength;
        if (string.length > 0) {
            config._pf.unusedInput.push(string);
        }

        // handle am pm
        if (config._isPm && config._a[HOUR] < 12) {
            config._a[HOUR] += 12;
        }
        // if is 12 am, change hours to 0
        if (config._isPm === false && config._a[HOUR] === 12) {
            config._a[HOUR] = 0;
        }

        dateFromConfig(config);
        checkOverflow(config);
    }

    function unescapeFormat(s) {
        return s.replace(/\\(\[)|\\(\])|\[([^\]\[]*)\]|\\(.)/g, function (matched, p1, p2, p3, p4) {
            return p1 || p2 || p3 || p4;
        });
    }

    // Code from http://stackoverflow.com/questions/3561493/is-there-a-regexp-escape-function-in-javascript
    function regexpEscape(s) {
        return s.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
    }

    // date from string and array of format strings
    function makeDateFromStringAndArray(config) {
        var tempConfig,
            bestMoment,

            scoreToBeat,
            i,
            currentScore;

        if (config._f.length === 0) {
            config._pf.invalidFormat = true;
            config._d = new Date(NaN);
            return;
        }

        for (i = 0; i < config._f.length; i++) {
            currentScore = 0;
            tempConfig = copyConfig({}, config);
            if (config._useUTC != null) {
                tempConfig._useUTC = config._useUTC;
            }
            tempConfig._pf = defaultParsingFlags();
            tempConfig._f = config._f[i];
            makeDateFromStringAndFormat(tempConfig);

            if (!isValid(tempConfig)) {
                continue;
            }

            // if there is any input that was not parsed add a penalty for that format
            currentScore += tempConfig._pf.charsLeftOver;

            //or tokens
            currentScore += tempConfig._pf.unusedTokens.length * 10;

            tempConfig._pf.score = currentScore;

            if (scoreToBeat == null || currentScore < scoreToBeat) {
                scoreToBeat = currentScore;
                bestMoment = tempConfig;
            }
        }

        extend(config, bestMoment || tempConfig);
    }

    // date from iso format
    function parseISO(config) {
        var i, l,
            string = config._i,
            match = isoRegex.exec(string);

        if (match) {
            config._pf.iso = true;
            for (i = 0, l = isoDates.length; i < l; i++) {
                if (isoDates[i][1].exec(string)) {
                    // match[5] should be 'T' or undefined
                    config._f = isoDates[i][0] + (match[6] || ' ');
                    break;
                }
            }
            for (i = 0, l = isoTimes.length; i < l; i++) {
                if (isoTimes[i][1].exec(string)) {
                    config._f += isoTimes[i][0];
                    break;
                }
            }
            if (string.match(parseTokenTimezone)) {
                config._f += 'Z';
            }
            makeDateFromStringAndFormat(config);
        } else {
            config._isValid = false;
        }
    }

    // date from iso format or fallback
    function makeDateFromString(config) {
        parseISO(config);
        if (config._isValid === false) {
            delete config._isValid;
            moment.createFromInputFallback(config);
        }
    }

    function map(arr, fn) {
        var res = [], i;
        for (i = 0; i < arr.length; ++i) {
            res.push(fn(arr[i], i));
        }
        return res;
    }

    function makeDateFromInput(config) {
        var input = config._i, matched;
        if (input === undefined) {
            config._d = new Date();
        } else if (isDate(input)) {
            config._d = new Date(+input);
        } else if ((matched = aspNetJsonRegex.exec(input)) !== null) {
            config._d = new Date(+matched[1]);
        } else if (typeof input === 'string') {
            makeDateFromString(config);
        } else if (isArray(input)) {
            config._a = map(input.slice(0), function (obj) {
                return parseInt(obj, 10);
            });
            dateFromConfig(config);
        } else if (typeof(input) === 'object') {
            dateFromObject(config);
        } else if (typeof(input) === 'number') {
            // from milliseconds
            config._d = new Date(input);
        } else {
            moment.createFromInputFallback(config);
        }
    }

    function makeDate(y, m, d, h, M, s, ms) {
        //can't just apply() to create a date:
        //http://stackoverflow.com/questions/181348/instantiating-a-javascript-object-by-calling-prototype-constructor-apply
        var date = new Date(y, m, d, h, M, s, ms);

        //the date constructor doesn't accept years < 1970
        if (y < 1970) {
            date.setFullYear(y);
        }
        return date;
    }

    function makeUTCDate(y) {
        var date = new Date(Date.UTC.apply(null, arguments));
        if (y < 1970) {
            date.setUTCFullYear(y);
        }
        return date;
    }

    function parseWeekday(input, locale) {
        if (typeof input === 'string') {
            if (!isNaN(input)) {
                input = parseInt(input, 10);
            }
            else {
                input = locale.weekdaysParse(input);
                if (typeof input !== 'number') {
                    return null;
                }
            }
        }
        return input;
    }

    /************************************
        Relative Time
    ************************************/


    // helper function for moment.fn.from, moment.fn.fromNow, and moment.duration.fn.humanize
    function substituteTimeAgo(string, number, withoutSuffix, isFuture, locale) {
        return locale.relativeTime(number || 1, !!withoutSuffix, string, isFuture);
    }

    function relativeTime(posNegDuration, withoutSuffix, locale) {
        var duration = moment.duration(posNegDuration).abs(),
            seconds = round(duration.as('s')),
            minutes = round(duration.as('m')),
            hours = round(duration.as('h')),
            days = round(duration.as('d')),
            months = round(duration.as('M')),
            years = round(duration.as('y')),

            args = seconds < relativeTimeThresholds.s && ['s', seconds] ||
                minutes === 1 && ['m'] ||
                minutes < relativeTimeThresholds.m && ['mm', minutes] ||
                hours === 1 && ['h'] ||
                hours < relativeTimeThresholds.h && ['hh', hours] ||
                days === 1 && ['d'] ||
                days < relativeTimeThresholds.d && ['dd', days] ||
                months === 1 && ['M'] ||
                months < relativeTimeThresholds.M && ['MM', months] ||
                years === 1 && ['y'] || ['yy', years];

        args[2] = withoutSuffix;
        args[3] = +posNegDuration > 0;
        args[4] = locale;
        return substituteTimeAgo.apply({}, args);
    }


    /************************************
        Week of Year
    ************************************/


    // firstDayOfWeek       0 = sun, 6 = sat
    //                      the day of the week that starts the week
    //                      (usually sunday or monday)
    // firstDayOfWeekOfYear 0 = sun, 6 = sat
    //                      the first week is the week that contains the first
    //                      of this day of the week
    //                      (eg. ISO weeks use thursday (4))
    function weekOfYear(mom, firstDayOfWeek, firstDayOfWeekOfYear) {
        var end = firstDayOfWeekOfYear - firstDayOfWeek,
            daysToDayOfWeek = firstDayOfWeekOfYear - mom.day(),
            adjustedMoment;


        if (daysToDayOfWeek > end) {
            daysToDayOfWeek -= 7;
        }

        if (daysToDayOfWeek < end - 7) {
            daysToDayOfWeek += 7;
        }

        adjustedMoment = moment(mom).add(daysToDayOfWeek, 'd');
        return {
            week: Math.ceil(adjustedMoment.dayOfYear() / 7),
            year: adjustedMoment.year()
        };
    }

    //http://en.wikipedia.org/wiki/ISO_week_date#Calculating_a_date_given_the_year.2C_week_number_and_weekday
    function dayOfYearFromWeeks(year, week, weekday, firstDayOfWeekOfYear, firstDayOfWeek) {
        var d = makeUTCDate(year, 0, 1).getUTCDay(), daysToAdd, dayOfYear;

        d = d === 0 ? 7 : d;
        weekday = weekday != null ? weekday : firstDayOfWeek;
        daysToAdd = firstDayOfWeek - d + (d > firstDayOfWeekOfYear ? 7 : 0) - (d < firstDayOfWeek ? 7 : 0);
        dayOfYear = 7 * (week - 1) + (weekday - firstDayOfWeek) + daysToAdd + 1;

        return {
            year: dayOfYear > 0 ? year : year - 1,
            dayOfYear: dayOfYear > 0 ?  dayOfYear : daysInYear(year - 1) + dayOfYear
        };
    }

    /************************************
        Top Level Functions
    ************************************/

    function makeMoment(config) {
        var input = config._i,
            format = config._f;

        config._locale = config._locale || moment.localeData(config._l);

        if (input === null || (format === undefined && input === '')) {
            return moment.invalid({nullInput: true});
        }

        if (typeof input === 'string') {
            config._i = input = config._locale.preparse(input);
        }

        if (moment.isMoment(input)) {
            return new Moment(input, true);
        } else if (format) {
            if (isArray(format)) {
                makeDateFromStringAndArray(config);
            } else {
                makeDateFromStringAndFormat(config);
            }
        } else {
            makeDateFromInput(config);
        }

        return new Moment(config);
    }

    moment = function (input, format, locale, strict) {
        var c;

        if (typeof(locale) === 'boolean') {
            strict = locale;
            locale = undefined;
        }
        // object construction must be done this way.
        // https://github.com/moment/moment/issues/1423
        c = {};
        c._isAMomentObject = true;
        c._i = input;
        c._f = format;
        c._l = locale;
        c._strict = strict;
        c._isUTC = false;
        c._pf = defaultParsingFlags();

        return makeMoment(c);
    };

    moment.suppressDeprecationWarnings = false;

    moment.createFromInputFallback = deprecate(
        'moment construction falls back to js Date. This is ' +
        'discouraged and will be removed in upcoming major ' +
        'release. Please refer to ' +
        'https://github.com/moment/moment/issues/1407 for more info.',
        function (config) {
            config._d = new Date(config._i);
        }
    );

    // Pick a moment m from moments so that m[fn](other) is true for all
    // other. This relies on the function fn to be transitive.
    //
    // moments should either be an array of moment objects or an array, whose
    // first element is an array of moment objects.
    function pickBy(fn, moments) {
        var res, i;
        if (moments.length === 1 && isArray(moments[0])) {
            moments = moments[0];
        }
        if (!moments.length) {
            return moment();
        }
        res = moments[0];
        for (i = 1; i < moments.length; ++i) {
            if (moments[i][fn](res)) {
                res = moments[i];
            }
        }
        return res;
    }

    moment.min = function () {
        var args = [].slice.call(arguments, 0);

        return pickBy('isBefore', args);
    };

    moment.max = function () {
        var args = [].slice.call(arguments, 0);

        return pickBy('isAfter', args);
    };

    // creating with utc
    moment.utc = function (input, format, locale, strict) {
        var c;

        if (typeof(locale) === 'boolean') {
            strict = locale;
            locale = undefined;
        }
        // object construction must be done this way.
        // https://github.com/moment/moment/issues/1423
        c = {};
        c._isAMomentObject = true;
        c._useUTC = true;
        c._isUTC = true;
        c._l = locale;
        c._i = input;
        c._f = format;
        c._strict = strict;
        c._pf = defaultParsingFlags();

        return makeMoment(c).utc();
    };

    // creating with unix timestamp (in seconds)
    moment.unix = function (input) {
        return moment(input * 1000);
    };

    // duration
    moment.duration = function (input, key) {
        var duration = input,
            // matching against regexp is expensive, do it on demand
            match = null,
            sign,
            ret,
            parseIso,
            diffRes;

        if (moment.isDuration(input)) {
            duration = {
                ms: input._milliseconds,
                d: input._days,
                M: input._months
            };
        } else if (typeof input === 'number') {
            duration = {};
            if (key) {
                duration[key] = input;
            } else {
                duration.milliseconds = input;
            }
        } else if (!!(match = aspNetTimeSpanJsonRegex.exec(input))) {
            sign = (match[1] === '-') ? -1 : 1;
            duration = {
                y: 0,
                d: toInt(match[DATE]) * sign,
                h: toInt(match[HOUR]) * sign,
                m: toInt(match[MINUTE]) * sign,
                s: toInt(match[SECOND]) * sign,
                ms: toInt(match[MILLISECOND]) * sign
            };
        } else if (!!(match = isoDurationRegex.exec(input))) {
            sign = (match[1] === '-') ? -1 : 1;
            parseIso = function (inp) {
                // We'd normally use ~~inp for this, but unfortunately it also
                // converts floats to ints.
                // inp may be undefined, so careful calling replace on it.
                var res = inp && parseFloat(inp.replace(',', '.'));
                // apply sign while we're at it
                return (isNaN(res) ? 0 : res) * sign;
            };
            duration = {
                y: parseIso(match[2]),
                M: parseIso(match[3]),
                d: parseIso(match[4]),
                h: parseIso(match[5]),
                m: parseIso(match[6]),
                s: parseIso(match[7]),
                w: parseIso(match[8])
            };
        } else if (typeof duration === 'object' &&
                ('from' in duration || 'to' in duration)) {
            diffRes = momentsDifference(moment(duration.from), moment(duration.to));

            duration = {};
            duration.ms = diffRes.milliseconds;
            duration.M = diffRes.months;
        }

        ret = new Duration(duration);

        if (moment.isDuration(input) && hasOwnProp(input, '_locale')) {
            ret._locale = input._locale;
        }

        return ret;
    };

    // version number
    moment.version = VERSION;

    // default format
    moment.defaultFormat = isoFormat;

    // constant that refers to the ISO standard
    moment.ISO_8601 = function () {};

    // Plugins that add properties should also add the key here (null value),
    // so we can properly clone ourselves.
    moment.momentProperties = momentProperties;

    // This function will be called whenever a moment is mutated.
    // It is intended to keep the offset in sync with the timezone.
    moment.updateOffset = function () {};

    // This function allows you to set a threshold for relative time strings
    moment.relativeTimeThreshold = function (threshold, limit) {
        if (relativeTimeThresholds[threshold] === undefined) {
            return false;
        }
        if (limit === undefined) {
            return relativeTimeThresholds[threshold];
        }
        relativeTimeThresholds[threshold] = limit;
        return true;
    };

    moment.lang = deprecate(
        'moment.lang is deprecated. Use moment.locale instead.',
        function (key, value) {
            return moment.locale(key, value);
        }
    );

    // This function will load locale and then set the global locale.  If
    // no arguments are passed in, it will simply return the current global
    // locale key.
    moment.locale = function (key, values) {
        var data;
        if (key) {
            if (typeof(values) !== 'undefined') {
                data = moment.defineLocale(key, values);
            }
            else {
                data = moment.localeData(key);
            }

            if (data) {
                moment.duration._locale = moment._locale = data;
            }
        }

        return moment._locale._abbr;
    };

    moment.defineLocale = function (name, values) {
        if (values !== null) {
            values.abbr = name;
            if (!locales[name]) {
                locales[name] = new Locale();
            }
            locales[name].set(values);

            // backwards compat for now: also set the locale
            moment.locale(name);

            return locales[name];
        } else {
            // useful for testing
            delete locales[name];
            return null;
        }
    };

    moment.langData = deprecate(
        'moment.langData is deprecated. Use moment.localeData instead.',
        function (key) {
            return moment.localeData(key);
        }
    );

    // returns locale data
    moment.localeData = function (key) {
        var locale;

        if (key && key._locale && key._locale._abbr) {
            key = key._locale._abbr;
        }

        if (!key) {
            return moment._locale;
        }

        if (!isArray(key)) {
            //short-circuit everything else
            locale = loadLocale(key);
            if (locale) {
                return locale;
            }
            key = [key];
        }

        return chooseLocale(key);
    };

    // compare moment object
    moment.isMoment = function (obj) {
        return obj instanceof Moment ||
            (obj != null && hasOwnProp(obj, '_isAMomentObject'));
    };

    // for typechecking Duration objects
    moment.isDuration = function (obj) {
        return obj instanceof Duration;
    };

    for (i = lists.length - 1; i >= 0; --i) {
        makeList(lists[i]);
    }

    moment.normalizeUnits = function (units) {
        return normalizeUnits(units);
    };

    moment.invalid = function (flags) {
        var m = moment.utc(NaN);
        if (flags != null) {
            extend(m._pf, flags);
        }
        else {
            m._pf.userInvalidated = true;
        }

        return m;
    };

    moment.parseZone = function () {
        return moment.apply(null, arguments).parseZone();
    };

    moment.parseTwoDigitYear = function (input) {
        return toInt(input) + (toInt(input) > 68 ? 1900 : 2000);
    };

    /************************************
        Moment Prototype
    ************************************/


    extend(moment.fn = Moment.prototype, {

        clone : function () {
            return moment(this);
        },

        valueOf : function () {
            return +this._d + ((this._offset || 0) * 60000);
        },

        unix : function () {
            return Math.floor(+this / 1000);
        },

        toString : function () {
            return this.clone().locale('en').format('ddd MMM DD YYYY HH:mm:ss [GMT]ZZ');
        },

        toDate : function () {
            return this._offset ? new Date(+this) : this._d;
        },

        toISOString : function () {
            var m = moment(this).utc();
            if (0 < m.year() && m.year() <= 9999) {
                return formatMoment(m, 'YYYY-MM-DD[T]HH:mm:ss.SSS[Z]');
            } else {
                return formatMoment(m, 'YYYYYY-MM-DD[T]HH:mm:ss.SSS[Z]');
            }
        },

        toArray : function () {
            var m = this;
            return [
                m.year(),
                m.month(),
                m.date(),
                m.hours(),
                m.minutes(),
                m.seconds(),
                m.milliseconds()
            ];
        },

        isValid : function () {
            return isValid(this);
        },

        isDSTShifted : function () {
            if (this._a) {
                return this.isValid() && compareArrays(this._a, (this._isUTC ? moment.utc(this._a) : moment(this._a)).toArray()) > 0;
            }

            return false;
        },

        parsingFlags : function () {
            return extend({}, this._pf);
        },

        invalidAt: function () {
            return this._pf.overflow;
        },

        utc : function (keepLocalTime) {
            return this.zone(0, keepLocalTime);
        },

        local : function (keepLocalTime) {
            if (this._isUTC) {
                this.zone(0, keepLocalTime);
                this._isUTC = false;

                if (keepLocalTime) {
                    this.add(this._dateTzOffset(), 'm');
                }
            }
            return this;
        },

        format : function (inputString) {
            var output = formatMoment(this, inputString || moment.defaultFormat);
            return this.localeData().postformat(output);
        },

        add : createAdder(1, 'add'),

        subtract : createAdder(-1, 'subtract'),

        diff : function (input, units, asFloat) {
            var that = makeAs(input, this),
                zoneDiff = (this.zone() - that.zone()) * 6e4,
                diff, output, daysAdjust;

            units = normalizeUnits(units);

            if (units === 'year' || units === 'month') {
                // average number of days in the months in the given dates
                diff = (this.daysInMonth() + that.daysInMonth()) * 432e5; // 24 * 60 * 60 * 1000 / 2
                // difference in months
                output = ((this.year() - that.year()) * 12) + (this.month() - that.month());
                // adjust by taking difference in days, average number of days
                // and dst in the given months.
                daysAdjust = (this - moment(this).startOf('month')) -
                    (that - moment(that).startOf('month'));
                // same as above but with zones, to negate all dst
                daysAdjust -= ((this.zone() - moment(this).startOf('month').zone()) -
                        (that.zone() - moment(that).startOf('month').zone())) * 6e4;
                output += daysAdjust / diff;
                if (units === 'year') {
                    output = output / 12;
                }
            } else {
                diff = (this - that);
                output = units === 'second' ? diff / 1e3 : // 1000
                    units === 'minute' ? diff / 6e4 : // 1000 * 60
                    units === 'hour' ? diff / 36e5 : // 1000 * 60 * 60
                    units === 'day' ? (diff - zoneDiff) / 864e5 : // 1000 * 60 * 60 * 24, negate dst
                    units === 'week' ? (diff - zoneDiff) / 6048e5 : // 1000 * 60 * 60 * 24 * 7, negate dst
                    diff;
            }
            return asFloat ? output : absRound(output);
        },

        from : function (time, withoutSuffix) {
            return moment.duration({to: this, from: time}).locale(this.locale()).humanize(!withoutSuffix);
        },

        fromNow : function (withoutSuffix) {
            return this.from(moment(), withoutSuffix);
        },

        calendar : function (time) {
            // We want to compare the start of today, vs this.
            // Getting start-of-today depends on whether we're zone'd or not.
            var now = time || moment(),
                sod = makeAs(now, this).startOf('day'),
                diff = this.diff(sod, 'days', true),
                format = diff < -6 ? 'sameElse' :
                    diff < -1 ? 'lastWeek' :
                    diff < 0 ? 'lastDay' :
                    diff < 1 ? 'sameDay' :
                    diff < 2 ? 'nextDay' :
                    diff < 7 ? 'nextWeek' : 'sameElse';
            return this.format(this.localeData().calendar(format, this));
        },

        isLeapYear : function () {
            return isLeapYear(this.year());
        },

        isDST : function () {
            return (this.zone() < this.clone().month(0).zone() ||
                this.zone() < this.clone().month(5).zone());
        },

        day : function (input) {
            var day = this._isUTC ? this._d.getUTCDay() : this._d.getDay();
            if (input != null) {
                input = parseWeekday(input, this.localeData());
                return this.add(input - day, 'd');
            } else {
                return day;
            }
        },

        month : makeAccessor('Month', true),

        startOf : function (units) {
            units = normalizeUnits(units);
            // the following switch intentionally omits break keywords
            // to utilize falling through the cases.
            switch (units) {
            case 'year':
                this.month(0);
                /* falls through */
            case 'quarter':
            case 'month':
                this.date(1);
                /* falls through */
            case 'week':
            case 'isoWeek':
            case 'day':
                this.hours(0);
                /* falls through */
            case 'hour':
                this.minutes(0);
                /* falls through */
            case 'minute':
                this.seconds(0);
                /* falls through */
            case 'second':
                this.milliseconds(0);
                /* falls through */
            }

            // weeks are a special case
            if (units === 'week') {
                this.weekday(0);
            } else if (units === 'isoWeek') {
                this.isoWeekday(1);
            }

            // quarters are also special
            if (units === 'quarter') {
                this.month(Math.floor(this.month() / 3) * 3);
            }

            return this;
        },

        endOf: function (units) {
            units = normalizeUnits(units);
            return this.startOf(units).add(1, (units === 'isoWeek' ? 'week' : units)).subtract(1, 'ms');
        },

        isAfter: function (input, units) {
            units = normalizeUnits(typeof units !== 'undefined' ? units : 'millisecond');
            if (units === 'millisecond') {
                input = moment.isMoment(input) ? input : moment(input);
                return +this > +input;
            } else {
                return +this.clone().startOf(units) > +moment(input).startOf(units);
            }
        },

        isBefore: function (input, units) {
            units = normalizeUnits(typeof units !== 'undefined' ? units : 'millisecond');
            if (units === 'millisecond') {
                input = moment.isMoment(input) ? input : moment(input);
                return +this < +input;
            } else {
                return +this.clone().startOf(units) < +moment(input).startOf(units);
            }
        },

        isSame: function (input, units) {
            units = normalizeUnits(units || 'millisecond');
            if (units === 'millisecond') {
                input = moment.isMoment(input) ? input : moment(input);
                return +this === +input;
            } else {
                return +this.clone().startOf(units) === +makeAs(input, this).startOf(units);
            }
        },

        min: deprecate(
                 'moment().min is deprecated, use moment.min instead. https://github.com/moment/moment/issues/1548',
                 function (other) {
                     other = moment.apply(null, arguments);
                     return other < this ? this : other;
                 }
         ),

        max: deprecate(
                'moment().max is deprecated, use moment.max instead. https://github.com/moment/moment/issues/1548',
                function (other) {
                    other = moment.apply(null, arguments);
                    return other > this ? this : other;
                }
        ),

        // keepLocalTime = true means only change the timezone, without
        // affecting the local hour. So 5:31:26 +0300 --[zone(2, true)]-->
        // 5:31:26 +0200 It is possible that 5:31:26 doesn't exist int zone
        // +0200, so we adjust the time as needed, to be valid.
        //
        // Keeping the time actually adds/subtracts (one hour)
        // from the actual represented time. That is why we call updateOffset
        // a second time. In case it wants us to change the offset again
        // _changeInProgress == true case, then we have to adjust, because
        // there is no such time in the given timezone.
        zone : function (input, keepLocalTime) {
            var offset = this._offset || 0,
                localAdjust;
            if (input != null) {
                if (typeof input === 'string') {
                    input = timezoneMinutesFromString(input);
                }
                if (Math.abs(input) < 16) {
                    input = input * 60;
                }
                if (!this._isUTC && keepLocalTime) {
                    localAdjust = this._dateTzOffset();
                }
                this._offset = input;
                this._isUTC = true;
                if (localAdjust != null) {
                    this.subtract(localAdjust, 'm');
                }
                if (offset !== input) {
                    if (!keepLocalTime || this._changeInProgress) {
                        addOrSubtractDurationFromMoment(this,
                                moment.duration(offset - input, 'm'), 1, false);
                    } else if (!this._changeInProgress) {
                        this._changeInProgress = true;
                        moment.updateOffset(this, true);
                        this._changeInProgress = null;
                    }
                }
            } else {
                return this._isUTC ? offset : this._dateTzOffset();
            }
            return this;
        },

        zoneAbbr : function () {
            return this._isUTC ? 'UTC' : '';
        },

        zoneName : function () {
            return this._isUTC ? 'Coordinated Universal Time' : '';
        },

        parseZone : function () {
            if (this._tzm) {
                this.zone(this._tzm);
            } else if (typeof this._i === 'string') {
                this.zone(this._i);
            }
            return this;
        },

        hasAlignedHourOffset : function (input) {
            if (!input) {
                input = 0;
            }
            else {
                input = moment(input).zone();
            }

            return (this.zone() - input) % 60 === 0;
        },

        daysInMonth : function () {
            return daysInMonth(this.year(), this.month());
        },

        dayOfYear : function (input) {
            var dayOfYear = round((moment(this).startOf('day') - moment(this).startOf('year')) / 864e5) + 1;
            return input == null ? dayOfYear : this.add((input - dayOfYear), 'd');
        },

        quarter : function (input) {
            return input == null ? Math.ceil((this.month() + 1) / 3) : this.month((input - 1) * 3 + this.month() % 3);
        },

        weekYear : function (input) {
            var year = weekOfYear(this, this.localeData()._week.dow, this.localeData()._week.doy).year;
            return input == null ? year : this.add((input - year), 'y');
        },

        isoWeekYear : function (input) {
            var year = weekOfYear(this, 1, 4).year;
            return input == null ? year : this.add((input - year), 'y');
        },

        week : function (input) {
            var week = this.localeData().week(this);
            return input == null ? week : this.add((input - week) * 7, 'd');
        },

        isoWeek : function (input) {
            var week = weekOfYear(this, 1, 4).week;
            return input == null ? week : this.add((input - week) * 7, 'd');
        },

        weekday : function (input) {
            var weekday = (this.day() + 7 - this.localeData()._week.dow) % 7;
            return input == null ? weekday : this.add(input - weekday, 'd');
        },

        isoWeekday : function (input) {
            // behaves the same as moment#day except
            // as a getter, returns 7 instead of 0 (1-7 range instead of 0-6)
            // as a setter, sunday should belong to the previous week.
            return input == null ? this.day() || 7 : this.day(this.day() % 7 ? input : input - 7);
        },

        isoWeeksInYear : function () {
            return weeksInYear(this.year(), 1, 4);
        },

        weeksInYear : function () {
            var weekInfo = this.localeData()._week;
            return weeksInYear(this.year(), weekInfo.dow, weekInfo.doy);
        },

        get : function (units) {
            units = normalizeUnits(units);
            return this[units]();
        },

        set : function (units, value) {
            units = normalizeUnits(units);
            if (typeof this[units] === 'function') {
                this[units](value);
            }
            return this;
        },

        // If passed a locale key, it will set the locale for this
        // instance.  Otherwise, it will return the locale configuration
        // variables for this instance.
        locale : function (key) {
            var newLocaleData;

            if (key === undefined) {
                return this._locale._abbr;
            } else {
                newLocaleData = moment.localeData(key);
                if (newLocaleData != null) {
                    this._locale = newLocaleData;
                }
                return this;
            }
        },

        lang : deprecate(
            'moment().lang() is deprecated. Use moment().localeData() instead.',
            function (key) {
                if (key === undefined) {
                    return this.localeData();
                } else {
                    return this.locale(key);
                }
            }
        ),

        localeData : function () {
            return this._locale;
        },

        _dateTzOffset : function () {
            // On Firefox.24 Date#getTimezoneOffset returns a floating point.
            // https://github.com/moment/moment/pull/1871
            return Math.round(this._d.getTimezoneOffset() / 15) * 15;
        }
    });

    function rawMonthSetter(mom, value) {
        var dayOfMonth;

        // TODO: Move this out of here!
        if (typeof value === 'string') {
            value = mom.localeData().monthsParse(value);
            // TODO: Another silent failure?
            if (typeof value !== 'number') {
                return mom;
            }
        }

        dayOfMonth = Math.min(mom.date(),
                daysInMonth(mom.year(), value));
        mom._d['set' + (mom._isUTC ? 'UTC' : '') + 'Month'](value, dayOfMonth);
        return mom;
    }

    function rawGetter(mom, unit) {
        return mom._d['get' + (mom._isUTC ? 'UTC' : '') + unit]();
    }

    function rawSetter(mom, unit, value) {
        if (unit === 'Month') {
            return rawMonthSetter(mom, value);
        } else {
            return mom._d['set' + (mom._isUTC ? 'UTC' : '') + unit](value);
        }
    }

    function makeAccessor(unit, keepTime) {
        return function (value) {
            if (value != null) {
                rawSetter(this, unit, value);
                moment.updateOffset(this, keepTime);
                return this;
            } else {
                return rawGetter(this, unit);
            }
        };
    }

    moment.fn.millisecond = moment.fn.milliseconds = makeAccessor('Milliseconds', false);
    moment.fn.second = moment.fn.seconds = makeAccessor('Seconds', false);
    moment.fn.minute = moment.fn.minutes = makeAccessor('Minutes', false);
    // Setting the hour should keep the time, because the user explicitly
    // specified which hour he wants. So trying to maintain the same hour (in
    // a new timezone) makes sense. Adding/subtracting hours does not follow
    // this rule.
    moment.fn.hour = moment.fn.hours = makeAccessor('Hours', true);
    // moment.fn.month is defined separately
    moment.fn.date = makeAccessor('Date', true);
    moment.fn.dates = deprecate('dates accessor is deprecated. Use date instead.', makeAccessor('Date', true));
    moment.fn.year = makeAccessor('FullYear', true);
    moment.fn.years = deprecate('years accessor is deprecated. Use year instead.', makeAccessor('FullYear', true));

    // add plural methods
    moment.fn.days = moment.fn.day;
    moment.fn.months = moment.fn.month;
    moment.fn.weeks = moment.fn.week;
    moment.fn.isoWeeks = moment.fn.isoWeek;
    moment.fn.quarters = moment.fn.quarter;

    // add aliased format methods
    moment.fn.toJSON = moment.fn.toISOString;

    /************************************
        Duration Prototype
    ************************************/


    function daysToYears (days) {
        // 400 years have 146097 days (taking into account leap year rules)
        return days * 400 / 146097;
    }

    function yearsToDays (years) {
        // years * 365 + absRound(years / 4) -
        //     absRound(years / 100) + absRound(years / 400);
        return years * 146097 / 400;
    }

    extend(moment.duration.fn = Duration.prototype, {

        _bubble : function () {
            var milliseconds = this._milliseconds,
                days = this._days,
                months = this._months,
                data = this._data,
                seconds, minutes, hours, years = 0;

            // The following code bubbles up values, see the tests for
            // examples of what that means.
            data.milliseconds = milliseconds % 1000;

            seconds = absRound(milliseconds / 1000);
            data.seconds = seconds % 60;

            minutes = absRound(seconds / 60);
            data.minutes = minutes % 60;

            hours = absRound(minutes / 60);
            data.hours = hours % 24;

            days += absRound(hours / 24);

            // Accurately convert days to years, assume start from year 0.
            years = absRound(daysToYears(days));
            days -= absRound(yearsToDays(years));

            // 30 days to a month
            // TODO (iskren): Use anchor date (like 1st Jan) to compute this.
            months += absRound(days / 30);
            days %= 30;

            // 12 months -> 1 year
            years += absRound(months / 12);
            months %= 12;

            data.days = days;
            data.months = months;
            data.years = years;
        },

        abs : function () {
            this._milliseconds = Math.abs(this._milliseconds);
            this._days = Math.abs(this._days);
            this._months = Math.abs(this._months);

            this._data.milliseconds = Math.abs(this._data.milliseconds);
            this._data.seconds = Math.abs(this._data.seconds);
            this._data.minutes = Math.abs(this._data.minutes);
            this._data.hours = Math.abs(this._data.hours);
            this._data.months = Math.abs(this._data.months);
            this._data.years = Math.abs(this._data.years);

            return this;
        },

        weeks : function () {
            return absRound(this.days() / 7);
        },

        valueOf : function () {
            return this._milliseconds +
              this._days * 864e5 +
              (this._months % 12) * 2592e6 +
              toInt(this._months / 12) * 31536e6;
        },

        humanize : function (withSuffix) {
            var output = relativeTime(this, !withSuffix, this.localeData());

            if (withSuffix) {
                output = this.localeData().pastFuture(+this, output);
            }

            return this.localeData().postformat(output);
        },

        add : function (input, val) {
            // supports only 2.0-style add(1, 's') or add(moment)
            var dur = moment.duration(input, val);

            this._milliseconds += dur._milliseconds;
            this._days += dur._days;
            this._months += dur._months;

            this._bubble();

            return this;
        },

        subtract : function (input, val) {
            var dur = moment.duration(input, val);

            this._milliseconds -= dur._milliseconds;
            this._days -= dur._days;
            this._months -= dur._months;

            this._bubble();

            return this;
        },

        get : function (units) {
            units = normalizeUnits(units);
            return this[units.toLowerCase() + 's']();
        },

        as : function (units) {
            var days, months;
            units = normalizeUnits(units);

            if (units === 'month' || units === 'year') {
                days = this._days + this._milliseconds / 864e5;
                months = this._months + daysToYears(days) * 12;
                return units === 'month' ? months : months / 12;
            } else {
                // handle milliseconds separately because of floating point math errors (issue #1867)
                days = this._days + yearsToDays(this._months / 12);
                switch (units) {
                    case 'week': return days / 7 + this._milliseconds / 6048e5;
                    case 'day': return days + this._milliseconds / 864e5;
                    case 'hour': return days * 24 + this._milliseconds / 36e5;
                    case 'minute': return days * 24 * 60 + this._milliseconds / 6e4;
                    case 'second': return days * 24 * 60 * 60 + this._milliseconds / 1000;
                    // Math.floor prevents floating point math errors here
                    case 'millisecond': return Math.floor(days * 24 * 60 * 60 * 1000) + this._milliseconds;
                    default: throw new Error('Unknown unit ' + units);
                }
            }
        },

        lang : moment.fn.lang,
        locale : moment.fn.locale,

        toIsoString : deprecate(
            'toIsoString() is deprecated. Please use toISOString() instead ' +
            '(notice the capitals)',
            function () {
                return this.toISOString();
            }
        ),

        toISOString : function () {
            // inspired by https://github.com/dordille/moment-isoduration/blob/master/moment.isoduration.js
            var years = Math.abs(this.years()),
                months = Math.abs(this.months()),
                days = Math.abs(this.days()),
                hours = Math.abs(this.hours()),
                minutes = Math.abs(this.minutes()),
                seconds = Math.abs(this.seconds() + this.milliseconds() / 1000);

            if (!this.asSeconds()) {
                // this is the same as C#'s (Noda) and python (isodate)...
                // but not other JS (goog.date)
                return 'P0D';
            }

            return (this.asSeconds() < 0 ? '-' : '') +
                'P' +
                (years ? years + 'Y' : '') +
                (months ? months + 'M' : '') +
                (days ? days + 'D' : '') +
                ((hours || minutes || seconds) ? 'T' : '') +
                (hours ? hours + 'H' : '') +
                (minutes ? minutes + 'M' : '') +
                (seconds ? seconds + 'S' : '');
        },

        localeData : function () {
            return this._locale;
        }
    });

    moment.duration.fn.toString = moment.duration.fn.toISOString;

    function makeDurationGetter(name) {
        moment.duration.fn[name] = function () {
            return this._data[name];
        };
    }

    for (i in unitMillisecondFactors) {
        if (hasOwnProp(unitMillisecondFactors, i)) {
            makeDurationGetter(i.toLowerCase());
        }
    }

    moment.duration.fn.asMilliseconds = function () {
        return this.as('ms');
    };
    moment.duration.fn.asSeconds = function () {
        return this.as('s');
    };
    moment.duration.fn.asMinutes = function () {
        return this.as('m');
    };
    moment.duration.fn.asHours = function () {
        return this.as('h');
    };
    moment.duration.fn.asDays = function () {
        return this.as('d');
    };
    moment.duration.fn.asWeeks = function () {
        return this.as('weeks');
    };
    moment.duration.fn.asMonths = function () {
        return this.as('M');
    };
    moment.duration.fn.asYears = function () {
        return this.as('y');
    };

    /************************************
        Default Locale
    ************************************/


    // Set default locale, other locale will inherit from English.
    moment.locale('en', {
        ordinal : function (number) {
            var b = number % 10,
                output = (toInt(number % 100 / 10) === 1) ? 'th' :
                (b === 1) ? 'st' :
                (b === 2) ? 'nd' :
                (b === 3) ? 'rd' : 'th';
            return number + output;
        }
    });

    /* EMBED_LOCALES */

    /************************************
        Exposing Moment
    ************************************/

    function makeGlobal(shouldDeprecate) {
        /*global ender:false */
        if (typeof ender !== 'undefined') {
            return;
        }
        oldGlobalMoment = globalScope.moment;
        if (shouldDeprecate) {
            globalScope.moment = deprecate(
                    'Accessing Moment through the global scope is ' +
                    'deprecated, and will be removed in an upcoming ' +
                    'release.',
                    moment);
        } else {
            globalScope.moment = moment;
        }
    }

    // CommonJS module is defined
    if (hasModule) {
        module.exports = moment;
    } else if (typeof define === 'function' && define.amd) {
        define('moment', function (require, exports, module) {
            if (module.config && module.config() && module.config().noGlobal === true) {
                // release the global variable
                globalScope.moment = oldGlobalMoment;
            }

            return moment;
        });
        makeGlobal(true);
    } else {
        makeGlobal();
    }
}).call(this);

}).call(this,typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})
},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\parseuri\\index.js":[function(require,module,exports){
/**
 * Parses an URI
 *
 * @author Steven Levithan <stevenlevithan.com> (MIT license)
 * @api private
 */

var re = /^(?:(?![^:@]+:[^:@\/]*@)(http|https|ws|wss):\/\/)?((?:(([^:@]*)(?::([^:@]*))?)?@)?((?:[a-f0-9]{0,4}:){2,7}[a-f0-9]{0,4}|[^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/;

var parts = [
    'source', 'protocol', 'authority', 'userInfo', 'user', 'password', 'host', 'port', 'relative', 'path', 'directory', 'file', 'query', 'anchor'
];

module.exports = function parseuri(str) {
    var src = str,
        b = str.indexOf('['),
        e = str.indexOf(']');

    if (b != -1 && e != -1) {
        str = str.substring(0, b) + str.substring(b, e).replace(/:/g, ';') + str.substring(e, str.length);
    }

    var m = re.exec(str || ''),
        uri = {},
        i = 14;

    while (i--) {
        uri[parts[i]] = m[i] || '';
    }

    if (b != -1 && e != -1) {
        uri.source = src;
        uri.host = uri.host.substring(1, uri.host.length - 1).replace(/;/g, ':');
        uri.authority = uri.authority.replace('[', '').replace(']', '').replace(/;/g, ':');
        uri.ipv6uri = true;
    }

    return uri;
};

},{}],"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\underscore\\underscore.js":[function(require,module,exports){
module.exports=require("C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\node_modules\\underscore\\underscore.js")
},{"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\node_modules\\underscore\\underscore.js":"C:\\Users\\jaysc_000\\Documents\\GitHub\\QuoteFlow\\node_modules\\backbone\\node_modules\\underscore\\underscore.js"}]},{},["./QuoteFlow/Content/js/app/app.js"]);

//# sourceMappingURL=bundle.js.map