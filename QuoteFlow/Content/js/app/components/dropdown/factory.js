"use strict";

var $ = require('jquery');
var Dropdown = require('./dropdown');

var DropdownFactory = {
    /**
     * Bind dropdowns that have no special behaviours.
     * @param {Element|jQuery} ctx the element to look in for dropdowns to bind
     */
    bindGenericDropdowns: function (ctx) {
        var trigger = $(".js-default-dropdown", ctx);
        trigger.each(function () {
            var $trigger = $(this),
                $content = $trigger.next(".aui-list"),
                alignment = $trigger.attr("data-alignment") || AJS.RIGHT,
                hasDropdown = !!$trigger.data("hasDropdown");

            if ($content.length === 0) {
                console.warn("Dropdown init failied. Could not find content. Printing culprit...");
                console.log($trigger);
            }

            if (!hasDropdown) {
                $trigger.data("hasDropdown", true);
                new Dropdown({
                    trigger: $trigger,
                    content: $content,
                    alignment: alignment,
                    setMaxHeightToWindow: $trigger.attr("data-contain-to-window"),
                    hideOnScroll: $trigger.attr("data-hide-on-scroll") || ".issue-container"
                });
            }
        });
    },

    /**
     * Binds issue action (cog) dropdowns
     * @param {Element|jQuery} ctx the element to look in for dropdowns to bind
     * @param {Object} options additional configuration for the dropdown to be created
     */
    bindIssueActionsDds: function (ctx, options) {
        var trigger = $(".issue-actions-trigger", ctx);
        trigger.each(function () {
            var $trigger = $(this);
            var dropdownConfig = {
                hideOnScroll: ".issue-container",
                trigger: $trigger,
                ajaxOptions: {
                    dataType: "json",
                    cache: false,
                    formatSuccess: JIRA.FRAGMENTS.issueActionsFragment
                },
                onerror: function (instance) {
                    // Sometimes the layerController is left in a initializing state (race condition?)
                    // Reset it here just in case.
                    instance.layerController.initialized = true;
                    instance.hide();
                }
            };
            dropdownConfig = $.extend(true, dropdownConfig, options);
            new Dropdown(dropdownConfig);
            $trigger.addClass("trigger-happy");
        });
    },

    /**
     * Binds dropdowns that control the views & columns in issue navigator
     */
    bindNavigatorOptionsDds: function () {
        var $navigatorOptions = $("#navigator-options");

        Dropdown.create({
            trigger: $navigatorOptions.find(".aui-dd-link"),
            content: $navigatorOptions.find(".aui-list"),
            alignment: AJS.RIGHT
        });
        $navigatorOptions.find("a.aui-dd-link").linkedMenu();
    },

    /**
     * Binds all the dropdowns that support the dashboard chrome
     */
    bindConfigDashboardDds: function () {
        $("#dashboard").find(".aui-dd-parent").dropDown("Standard", {
            trigger: "a.aui-dd-link"
        });
    }
};

module.exports = DropdownFactory;
