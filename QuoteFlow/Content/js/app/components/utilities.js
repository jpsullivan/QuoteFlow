﻿/**
    General utility functions

    @class Utilities
    @namespace QuoteFlow
    @module QuoteFlow
**/
QuoteFlow.Utilities = {

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
        var path = parsedUrl.path.replace(QuoteFlow.applicationPath, "");

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
    }
};