"use strict";

var $ = require('jquery');
var _ = require('underscore');

var EventTypes = require('../util/types');
var ContentAddedReason = require('../util/reasons');
var CheckboxMultiSelect = require('../../../components/select/checkbox-multi-select');

var InitSparklers = {

	initSparklers: function ($ctx) {
		$ctx.find(".js-default-checkboxmultiselect").each(function () {
            var $select = AJS.$(this);
            if (!$select.data("checkboxmultiselect")) {
                $select.data("checkboxmultiselect",  new CheckboxMultiSelect({
                    element: this
                }));
            } else {
                $select.data("checkboxmultiselect").render();
            }
        });
	},

	register: function () {
		QuoteFlow.bind(EventTypes.NEW_CONTENT_ADDED, _.bind(function (e, $context, reason) {
	        if (reason === ContentAddedReason.criteriaPanelRefreshed) {
	            this.initSparklers($context);
	            // initStatusLozengeSparklers($context);
	            // initLabelSparkler($context);
	        }
	    }, this));

		/**
		 * Adds the class 'checkboxmultiselect-container' to the parent form-body div of a sparker.
		 * This removes the implicit padding from the container as all other types of searchers
		 * have padding by default.
		 */
		QuoteFlow.bind(EventTypes.CHECKBOXMULTISELECT_READY, function (e, $select) {
		    $select.closest(".form-body").addClass("checkboxmultiselect-container");
		});
	}
};

module.exports = InitSparklers;
