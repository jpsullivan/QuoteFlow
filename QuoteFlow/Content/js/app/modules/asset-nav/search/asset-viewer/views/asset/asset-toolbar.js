"use strict";

var Marionette = require('backbone.marionette');

/**
 * This view renders the toolbar of an asset (i.e. the buttons and dropdowns in the header).
 * @extends {Marionette.ItemView}
 */
var AssetToolbarView = Marionette.ItemView.extend({
    template: Handlebars.partials["asset-viewer/asset/_toolbar"],

    ui: {
        shareButton: ".asset-share",
        exportButton: "#asset-export",
        commentButton: "#comment-asset"
    },

    /**
     * Extract the data from the model in the format needed by the template.
     * @returns {Object} Data to be rendered by the template
     */
    serializeData: function () {
        return {asset: this.model.getEntity()};
    },

    /**
     * Handler for render event, things to do after the template has been rendered
     */
    onRender: function () {
        //TODO Why do we need to mess with the DOM here? Can we move these changes to the template?

        // Ensure the comment button does not display the add comment dialog
        this.ui.commentButton.addClass("inline-comment");
    }
});

module.exports = AssetToolbarView;
