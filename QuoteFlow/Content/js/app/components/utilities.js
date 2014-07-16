/**
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
    },

    /**
        Produces optional, URL-friendly version of a title, "like-this-one".
        Totally copied from the UrlHelpers class and seems to work fine.
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