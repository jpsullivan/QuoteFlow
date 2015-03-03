"use strict";

var Utils = {
    tryIt: function(f, defaultVal) {
        try {
            return f();
        } catch (ex) {
            return defaultVal;
        }
    }
};

module.exports = Utils;