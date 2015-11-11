"use strict";

var _ = require('underscore');
var ErrorLayout = require('./error-layout');

/**
 * Renders the error for NotPermissions when viewing/editing an asset.
 * @extends ErrorLayout
 */
var ErrorNotFoundView = ErrorLayout.extend({
    template: JST["asset-viewer/error/does-not-exist"],

    /**
     * Initializes this view
     *
     * @param options
     * @param {boolean|function} [options.showReturnToSearchOnError=false] Whether the error views should display a 'Return to Search' link
     */
    initialize: function(options) {
        options = options || {};
        this.showReturnToSearchOnError = options.showReturnToSearchOnError;
    },

    triggers: {
        "click #return-to-search": "returnToSearch"
    },

    serializeData: function() {
        var showReturnToSearchOnError = this.showReturnToSearchOnError;
        if (_.isFunction(showReturnToSearchOnError)){
            showReturnToSearchOnError = showReturnToSearchOnError();
        }

        return {
            hideReturnToSearch: !showReturnToSearchOnError
        };
    }
});

module.exports = ErrorNotFoundView;
