"use strict";

var StandardPositioning = require('./inline-layer-standard-positioning');

/**
 * An {@see InlineLayer} positioning controller that ensures the layer doesn't overflow the bottom of the window.
 *
 * @class WindowPositioning
 * @extends StandardPositioning
 */
var WindowPositioning = StandardPositioning.extend({

    /**
     * @param {object} offset The offset from which to calculate the overflow.
     * @return {Number} The amount by which the layer overflows the window.
     * @private
     */
    _calculateOverflow: function (offset) {
        var isFixed = this.layer().css("position") === "fixed",
                layerBottom = offset.top + this.layer().outerHeight(true),
                windowHeight = jQuery(window).outerHeight(),
                windowScroll = jQuery(window).scrollTop();

        if (isFixed) {
            return Math.max(0, layerBottom - windowHeight);
        } else {
            return Math.max(0, layerBottom - windowScroll - windowHeight);
        }
    },

    left: function () {
        var offset = this._super(),
                overflow = this._calculateOverflow(offset);

        if (overflow > 0) {
            offset.left += this.offsetTarget().outerWidth();
            offset.top -= overflow;
        }

        return offset;
    },

    right: function () {
        var offset = this._super(),
                overflow = this._calculateOverflow(offset);

        if (overflow > 0) {
            offset.left -= this.offsetTarget().outerWidth();
            offset.top -= overflow;
        }

        return offset;
    }
});

module.exports = WindowPositioning;
