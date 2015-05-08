"use strict";

var _ = require('underscore');

/**
 * A vertical resize event is triggered when the page changes in such a way that elements may need to resize themselves
 * vertically (e.g. an element is added/removed, or an element's height changes).
 */
var iife = (function () {
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
}());

module.exports = iife;
