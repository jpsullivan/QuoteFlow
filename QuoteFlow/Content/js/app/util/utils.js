"use strict";

var Utils = {
    tryIt: function(f, defaultVal) {
        try {
            return f();
        } catch (ex) {
            return defaultVal;
        }
    },

    regExpEscape: function (str) {
      // Note: This escapes str for regex literal sequences -- not within character classes
      var SPECIAL_CHARS = /[.*+?|^$()[\]{\\]/g;
        return str.replace(SPECIAL_CHARS, "\\$&");
    }
};

module.exports = Utils;
