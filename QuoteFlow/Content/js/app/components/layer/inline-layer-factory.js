"use strict";

var InlineLayerFactory = {

	/**
     * Static factory method to create multiple dropdowns at one time.
     *
     * @method create
     * @param options - {@see InlineLayer.OptionsDescriptor}
     * @return {InlineLayer | InlineLayer[]}
     */
	createInlineLayers: function (options) {
        var inlineLayers = [];

        if (options.content) {
            options.content = jQuery(options.content);
            jQuery.each(options.content, function () {
                var instanceOptions = Objects.copyObject(options);
                instanceOptions.content = jQuery(this);
                inlineLayers.push(new InlineLayer(instanceOptions));
            });
        }

        if (inlineLayers.length == 1) {
            return inlineLayers[0];
        } else {
            return inlineLayers;
        }
    }
};

module.exports = InlineLayerFactory;
