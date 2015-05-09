"use strict";

var objects = {
	/**
     * @function begetObject
     * @return {Object} cloned object
     */
    begetObject: function begetObject(obj) {
        var f = function() {};
        f.prototype = obj;
        return new f;
    },

    /**
     * @param {Object} object - to copy
     * @param {Boolean} deep - weather to copy objects within object
     */
    copyObject: function copyObject(object, deep) {

        var copiedObject = $.isArray(object) ? [] : {};

            $.each(object, function(name, property) {
                if (typeof property !== "object" || property === null || property instanceof $) {
                    copiedObject[name] = property;
                } else if (deep !== false) {
                    copiedObject[name] = exports.copyObject(property, deep);
                }
            });

        return copiedObject;
    }
}

module.exports = objects;
