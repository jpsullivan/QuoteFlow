"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var InlineLayer = require('../../../../components/layer/inline-layer');

/**
 * View that handles "add criteria" and "clear criteria" buttons
 */
var ClauseButtonView = Brace.View.extend({

    template: JST["quote-builder/query/basic/clause-button"],

    addCriteriaButton: ".add-criteria",

    events: {
        "click .add-criteria": "_showDialog",
        "keydown .add-criteria": "_onKeydown"
    },

    initialize: function(options) {
        _.bindAll(this,
            "_handleInteractiveChanged",
            "_showOrHideClauseButtons");

        var instance = this;

        this.queryStateModel = options.queryStateModel;
        this.searcherCollection = options.searcherCollection;
        this.searcherCollection.onCollectionChanged(this._showOrHideClauseButtons);
        this.searcherCollection.onInteractiveChanged(this._handleInteractiveChanged);

        var dialog = this.dialog = new InlineLayer({
            width: "auto",
            alignment: AJS.LEFT,
            // Lazilly get offset target as it isn't in the DOM at this time
            offsetTarget:function () {
                return instance.$el.find(instance.addCriteriaButton);
            },
            // Each time we open our dialog this function will be called to retrieve content
            content: function() {
                return listView.render();
            }
        });

        dialog.bind(InlineLayer.EVENTS.hide, function (e, layer, reason) {
            if (reason === AJS.HIDE_REASON.escPressed || reason === AJS.HIDE_REASON.toggle
                || reason == AJS.HIDE_REASON.tabbedOut) {
                instance.$(instance.addCriteriaButton).focus();
            }
        });

        // Contents of InlineLayer
        var listView = new JIRA.Issues.SearcherGroupListDialogView({
            searcherCollection: this.searcherCollection,
            dialog: dialog
        });

        // Allow the view inside of the InlineLayer to trigger hiding
        listView.onHideRequested(function(reason) {
            dialog.hide(reason);
        });

        dialog.bind(AJS.InlineLayer.EVENTS.show, function(event, $layer) {
            jQuery("#criteria-input").focus();
            // List.js also resets the scrollTop but because the dialog is still hidden at that point, the browser won't actually do any scrolling.
            // @see JRADEV-15097
            $layer.find(".aui-list-scroll").scrollTop(0);
        });
    },

    render: function() {
        this.$el.html(this.template({
            isSubtle: this.queryStateModel.hasSubtleMoreCriteria()
        }));
        this._showOrHideClauseButtons();
        this._addToolTip();
        return this.$el;
    },

    _addToolTip : function() {
        new JIRA.Issues.Tipsy({
            el: this.$el.find(this.addCriteriaButton),
            showCondition: ":not(.active)"
        });
    },

    _showOrHideClauseButtons: function() {
        var addFiltersButton = this.$(this.addCriteriaButton);
        if (this.searcherCollection.getAddMenuGroupDescriptors().length > 0) {
            addFiltersButton.show();
        } else {
            addFiltersButton.hide();
        }
    },

    _showDialog: function (event) {
        if (this.searcherCollection.isInteractive()) {
            this.dialog.toggle();
        }
        event.preventDefault();
    },

    _handleInteractiveChanged: function(interactive) {
        this.$(this.addCriteriaButton).attr("aria-disabled", (interactive) ? null : "true");
    },

    _onKeydown: function(event) {
        switch (event.which) {
            case AJS.$.ui.keyCode.DOWN:
                this._showDialog(event);
                break;
            case AJS.$.ui.keyCode.ESCAPE:
                jQuery(event.target).blur();
                break;
            case AJS.$.ui.keyCode.BACKSPACE:
                // Prevent Backspace on the Add Criteria button from navigating back in history
                break;
            default:
                return;
        }
        event.preventDefault();
    }
});

module.exports = ClauseButtonView;
