"use strict";

var Marionette = require('backbone.marionette');

/**
 * The layout switcher control.
 */
var LayoutSwitcherView = Marionette.ItemView.extend({
    
    template: JST["asset/nav-search/layout-switcher"],

    /**
     * @param {object} options
     * @param {JIRA.Issues.SearchPageModule} options.searchPageModule
     */
    initialize: function (options) {
        _.bindAll(this, "_onLayoutSwitchClick");

        this.searchPageModule = options.searchPageModule;
        this.searchPageModule.on("change:currentLayout", this.render, this);
    },

    /**
     * @return {JIRA.Issues.LayoutSwitcherView} <tt>this</tt>
     */
    render: function () {
        this.$el.html(this.template({
            layouts: this.searchPageModule.getSortedLayouts(),
            activeLayout: this.searchPageModule.getActiveLayout()
        }));

        this._addLayoutSwitcherTooltip();

        // We can't use delegate events as the dropdown is appended to the body.
        this.$el.find(".aui-list-item-link").click(this._onLayoutSwitchClick);
        JIRA.trigger(JIRA.Events.NEW_CONTENT_ADDED, [this.$el, JIRA.CONTENT_ADDED_REASON.layoutSwitcherReady]);
        return this;
    },

    /**
     * @returns {JIRA.Issues.LayoutSwitcherView} <tt>this</tt>
     */
    enableLayoutSwitcher: function () {
        this.$el.find("#layout-switcher-button").removeClass("disabled").removeAttr('disabled');
        return this;
    },

    /**
     * @returns {JIRA.Issues.LayoutSwitcherView} <tt>this</tt>
     */
    disableLayoutSwitcher: function () {
        this.$el.find("#layout-switcher-button").addClass("disabled").attr('disabled', '');
        return this;
    },

    createHelptipForSwitchingToDetailView: function (weight) {
        var tip;
        if (this._shouldShowIntro() && this.$el.is(":visible")) {
            tip = new AJS.HelpTip({
                id: "split-view-intro",
                title: AJS.I18n.getText('issuenav.layoutswitcher.intro.title'),
                url: AJS.Meta.get('issue-search-help-url'),
                bodyHtml: AJS.I18n.getText('issuenav.layoutswitcher.intro.desc'),
                anchor: ".view-selector button",
                isSequence: true,
                weight: weight
            });
        }
        return tip;
    },

    _shouldShowIntro: function () {
        return this.searchPageModule.search.getResults().hasIssues();
    },

    /**
     * Adds a tooltip to the layout switcher button
     * @private
     */
    _addLayoutSwitcherTooltip: function () {
        function getTooltipMessage() {
            // If there is no shortcut for this action, just display the regular text. (i.e. without the 'Type X' part)
            var shortcut = AJS.KeyboardShortcut.getKeyboardShortcutKeys('switch.search.layouts');
            if (shortcut) {
                return AJS.I18n.getText("issuenav.layoutswitcher.button.tooltip", AJS.KeyboardShortcut.getKeyboardShortcutKeys('switch.search.layouts'));
            } else {
                return AJS.I18n.getText("issuenav.layoutswitcher.button.tooltip.whitout.kb.shortcut");
            }
        }

        new JIRA.Issues.Tipsy({
            el: this.$el.find("#layout-switcher-button"),
            showCondition: ":not(.active)",
            tipsy: {
                title: getTooltipMessage,
                gravity: 'ne'
            }
        });
    },

    /**
     * Tell the <tt>SearchPageModule</tt> to change layout.
     * <p/>
     * Called when a layout button is clicked.
     *
     * @param {object} e The click event.
     * @param {object} [options] Options used in tests.
     *
     * @private
     */
    _onLayoutSwitchClick: function (e, options) {
        if (JIRA.Issues.Application.request("issueEditor:canDismissComment")) {
            // HACK: Hover intent has a strange bug that when we click the layout switcher it triggers a mouseleave event on the filters panel
            // To get around this, we disable it whilst we are transitioning to new layout.
            var layoutKey = AJS.$(e.target).closest("[data-layout-key]").data("layout-key");
            e.preventDefault();
            this.searchPageModule.changeLayout(layoutKey, options);
        }
    }
})