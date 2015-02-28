"use strict";

var Mixins = {
    /**
     * Creates a camelCased method name
     */
    createMethodName: function(prefix, suffix) {
        return prefix + suffix.charAt(0).toUpperCase() + suffix.substr(1);
    }
};

module.exports = Mixins;