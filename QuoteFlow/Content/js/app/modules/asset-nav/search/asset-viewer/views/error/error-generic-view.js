"use strict";

var ErrorLayout = require('./error-layout');

/**
 * Renders a generic error
 * @extends ErrorLayout
 */
var ErrorGenericView = ErrorLayout.extend({
    template: JST["asset-viewer/error/generic"]
});

module.exports = ErrorGenericView;
