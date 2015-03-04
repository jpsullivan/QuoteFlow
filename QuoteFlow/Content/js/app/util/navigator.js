"use strict";

var $ = require('jquery');
var _ = require('underscore');

/**
 * A module that wraps all the browser+os checks you'd want to do via 
 * checking the window.navigator properties.
 *
 * Code is borrowed (i.e., copy-pasted) from a Bower component.
 * 
 * Probably best to avoid this at all costs and rely on feature detection
 * instead.
 */
var Navigator = {
    userAgent: window.navigator.userAgent,
    platform: window.navigator.platform,

    _isTrident: function(userAgent) {
        return (/\bTrident\b/).test(userAgent);
    },

    isChrome: _.once(function() {
        return (/Chrome/).test(userAgent);
    }),

    isIE: _.once(function() {
        return _isTrident(userAgent);
    }),

    isMozilla: _.once(function() {
        return $.browser.mozilla;
    }),

    isSafari: _.once(function() {
        return $.browser.safari && !isChrome();
    }),

    isWebkit: _.once(function() {
        return $.browser.webkit;
    }),

    isOpera: _.once(function() {
        return $.browser.opera === true;
    }),

    majorVersion: _.once(function() {
        return parseInt($.browser.version, 10);
    }),

    isLinux: _.once(function() {
        return platform.indexOf('Linux') !== -1;
    }),

    isMac: _.once(function() {
        return platform.indexOf('Mac') !== -1;
    }),

    isWin: _.once(function() {
        return platform.indexOf('Win') !== -1;
    })
};

module.exports = Navigator;