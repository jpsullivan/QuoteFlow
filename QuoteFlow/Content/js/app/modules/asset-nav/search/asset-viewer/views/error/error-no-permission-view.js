"use strict";

var ErrorLayout = require('./error-layout');

/**
 * Renders the error for NotPermissions when viewing/editing an asset.
 * @extends ErrorLayout
 */
var ErrorNoPermissionView = ErrorLayout.extend({
    template: JST["asset-viewer/error/no-permission"]
});

module.exports = ErrorNoPermissionView;
