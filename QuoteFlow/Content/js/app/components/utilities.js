"use strict";

var _ = require('underscore');
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
     * A debounce implementation the differs from the undercore one. This implementation:
     * 1. Executes the supplied method straight away
     * 2. Using underscore debounce, postpones its execution until 300ms since the last time it was invoked.
     * 3. After the debounced invocation, waits 500ms before restoring to the original state (step 1).
     *
     * @param {Object} ctx
     * @param {String} method
     * @param {...} arguments for first invocation.
     */
    debounce: function (ctx, method) {
        var args = Array.prototype.slice.call(arguments, 2);

        if (!ctx) {
            console.error("QuoteFlow custom debounce: ctx must be defined");
        }

        clearTimeout(ctx[method + "DebounceTimeout"]);

        // Invoke method, first time this will happen straight away. Subsequent times it will be calling the debounced
        // method.
        ctx[method].apply(ctx, args);

        if (!ctx[method + "DebounceTimeout"]) {
            ctx[method + "Original"] = ctx[method];
            ctx[method] = _.debounce(function () {
                return ctx[method + "Original"].apply(ctx, arguments);
            }, 300);
        }

        // After 500 ms or recieving input, get rid of debounced version.
        ctx[method + "DebounceTimeout"] = setTimeout(function () {
            ctx[method] = ctx[method + "Original"];
            ctx[method + "DebounceTimeout"] = null;
        }, 500);
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
        return prevdash ? sb.toString().substr(0, sb.length - 1) : sb.join("");
    }
};

module.exports = Utilities;
