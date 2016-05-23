"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var PagerView = Marionette.ItemView.extend({
    template: function (data) {
        return _.isEmpty(data) ? "" : JST["asset-viewer/asset/pager"];
    },

    events: {
        "simpleClick #return-to-search": "_onReturnToSearchClick"
    },

    triggers: {
        "simpleClick #next-asset": "nextItem",
        "simpleClick #previous-asset": "previousItem"
    },

    ui: {
        nextIssue: "#next-asset",
        previousIssue: "#previous-asset",
        returnToSearch: "#return-to-search"
    },

    modelEvents: {
        "change": "render"
    },

    onRender: function () {
        if (AJS.activeShortcuts) {
            AJS.activeShortcuts["j"] && AJS.activeShortcuts["j"]._addShortcutTitle(this.$el.find(this.ui.nextAsset));
            AJS.activeShortcuts["k"] && AJS.activeShortcuts["k"]._addShortcutTitle(this.$el.find(this.ui.previousAsset));
            AJS.activeShortcuts["u"] && AJS.activeShortcuts["u"]._addShortcutTitle(this.$el.find(this.ui.returnToSearch));
        }
    },

    _onReturnToSearchClick: function (e) {
        e.preventDefault();
        this.trigger("goBack");

        // IE8 and IE9 do not return focus back to the body after pressing the Return to Search link
        // Thus any subsequent shortcut operations will not work because the focus is wrong
        // This is to return focus back to body
        $("body").focus();
    }
});

module.exports = PagerView;
