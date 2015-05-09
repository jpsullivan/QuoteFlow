"use strict";

var $ = require('jquery');
var Class = require('../class/class');

/**
 * An abstract class used to maintain complex descriptors.
 */
var Descriptor = Class.extend({

    init: function(options) {
        if (this._validate(options)) {
            this.properties = $.extend(this._getDefaultOptions(), options);
        }
    },

    /**
     * Gets all properties.
     *
     * @method allProperties
     * @return {Object}
     */
    allProperties: function () {
        return this.properties;
    },

    /**
     * Ensures all required properites are defined otherwise throws error.
     *
     * @method _validate
     * @protected
     */
    _validate: function (properties) {
        if (this.REQUIRED_PROPERTIES) {
            $.each(this.REQUIRED_PROPERTIES, function (name) {
                if (typeof properties[name] === "undefined") {
                    throw new Error("Descriptor: expected property [" + name + "] but was undefined");
                }
            });
        }
        return true;
    }
});

module.exports = Descriptor;
