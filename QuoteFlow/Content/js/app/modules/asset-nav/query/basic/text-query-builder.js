"use strict";

var $ = require('jquery');

var TextQueryBuilder = {
    BACKSLASH_PATTERN: /\\/g,
    MULTIPLE_WHITESPACE_PATTERN: /\s+/g,
    QUOTE_PATTERN: /\"/g,

    DOUBLE_BACKSLASH: "\\\\",

    buildJql: function(textQuery) {
        if (!textQuery) {
            return "";
        }
        textQuery = "" + textQuery;
        textQuery = $.trim(textQuery);
        textQuery = textQuery.replace(this.BACKSLASH_PATTERN, this.DOUBLE_BACKSLASH); // replace single literal backslash with two backslashes (escaped here)
        textQuery = textQuery.replace(this.MULTIPLE_WHITESPACE_PATTERN, " "); // remove multiple whitespaces
        textQuery = textQuery.replace(this.QUOTE_PATTERN, "\\\"");
        return this.createQueryClause(textQuery);
    },

    createQueryClause: function(searchTerm) {
        return 'text ~ "' + searchTerm + '"';
    }
};

module.exports = TextQueryBuilder;
