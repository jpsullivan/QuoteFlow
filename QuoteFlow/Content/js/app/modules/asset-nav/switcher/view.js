"use strict";

var Brace = require('backbone-brace');

/**
 * View that allows switching between querying modes
 */
var SwitcherView = Brace.View.extend({

    namedEvents: ["verticalResize"],

    tagName: "div",

    events: {
        "click .switcher-item": "_onSwitcherClick"
    },

    initialize: function(options) {
        this.containerClass = options.containerClass;
        this.model = options.model;
        this.template = options.template;
        this.model.onSelectionChanged(this._onSelect, this);
        this.switchEl = AJS.$();
        this.model.on("change:disabled", _.bind(this._setSwitching, this));
    },

    /**
     * Render the switcher and the currently selected item.
     */
    render: function() {
        this._onSelect();
        this._render();
    },

    /**
     * Render the switcher.
     *
     * @private
     */
    _render: function () {
        var selected = this.model.getSelected();

        this.switchEl = this.$el.find(".mode-switcher");
        this.switchEl.html(this.template({
            items: this.model.getCollection().toJSON(),
            selectedId: selected && selected.id
        }));

        // new JIRA.Issues.Tipsy({
        //     el: this.$el.find(".switcher-item.active")
        // });
    },

    getSwitcherTrigger: function() {
        return this.switchEl.find('.switcher-item.active');
    },

    _onSelect: function() {
        var container = this.$el.find(this.containerClass).empty(),
            selected = this.model.getSelected();
        if (selected) {
            container.attr("data-mode",selected.id);
            selected.getView().setElement(container).render();
        }

        this._render();
        this.triggerVerticalResize();
    },

    _onSwitcherClick: function(event) {
        event.preventDefault();
        if (!this.model.getDisabled()) {
            this.model.next();

            var selectedView = this.model.getSelected().getView();
            selectedView.focus && selectedView.focus();
        }
    },

    _setSwitching: function() {
        if (this.model.getDisabled()) {
            this.disableSwitching()
        } else {
            this.enableSwitching();
        }
    },

    disableSwitching: function() {
        this.switchEl.addClass("disabled");
        this.getSwitcherTrigger().attr("original-title", AJS.I18n.getText("jira.jql.query.too.complex"))
    },

    enableSwitching: function() {
        this.switchEl.removeClass("disabled");
        var selected = this.model.getSelected();
        //if we're currently in advanced mode and we're re-enabling the switcher we need to restore the original title to indicate
        //that the link allows you to switch back to basic mode.
        if(selected && selected.id === "advanced") {
            this.getSwitcherTrigger().attr("original-title", AJS.I18n.getText("issues.components.query.switchto.basic.description"));
        }
    },

    /**
     * Hide or show the switcher trigger.
     *
     * @param isVisible Whether the switcher should be visible.
     */
    setVisible: function(isVisible) {
        this.switchEl.toggleClass("hidden", !isVisible);
    }
});

module.exports = SwitcherView;
