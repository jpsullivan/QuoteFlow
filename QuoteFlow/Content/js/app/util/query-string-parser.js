"use strict";

var QueryStringParser = {
    parser: /(?:^|&)([^&=]*)=?([^&]*)/g,

    parse: function(queryString) {
        if (!queryString && queryString !== 0) {
            return {};
        }
        queryString = "" + queryString;
        if ("?" === queryString.charAt(0)) {
            queryString = queryString.substring(1);
        }
        var params = {};
        queryString.replace(this.parser, function ($0, $1, $2) {
            var v = decodeURIComponent(($2 || "").replace(/\+/g, " "));
            params[decodeURIComponent($1)] = v;
        });
        return params;
    }
};

module.exports = QueryStringParser;
