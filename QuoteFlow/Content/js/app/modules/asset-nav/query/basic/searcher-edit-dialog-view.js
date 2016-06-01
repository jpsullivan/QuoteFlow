"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

var ContentAddedReason = require('../../util/reasons');
var EventTypes = require('../../util/types');
var catchExceptions = require('jquery-catch-exceptions');

/**
 * The view for editing the value that one criterion has.
 */
var SearcherEditDialogView = Brace.View.extend({

    namedEvents: ["backRequested", "hideRequested"],

    autoUpdate: [".js-default-checkboxmultiselect", ".js-default-checkboxmultiselectstatuslozenge", ".js-user-checkboxmultiselect", ".js-usergroup-checkboxmultiselect", ".js-group-checkboxmultiselect", ".js-label-checkboxmultiselect"],

    events: {
        "click .cancel-update": "_onCancelClicked",
        "click": "_stopPropagation",
        "submit": "_onSubmit",
        'keydown': '_keyPressed'
    },

    template: JST["quote-builder/query/basic/lozenge-dropdown-content"],

    initialize: function (options) {
        this._formData = "";
        this.queryStateModel = options.queryStateModel;
        this.$el.scrollLock('.aui-list-scroll');

        // extend the events object since access to this.queryStateModel
        // is not valid after backbone 1.1.0 due to initialize() still not
        // being called.
        if (this.queryStateModel.getBasicAutoUpdate()) {
            this.events = _.extend(this.events, {
                ["selected " + this.autoUpdate.join(',')]: "applyChanges",
                ["unselect " + this.autoUpdate.join(',')]: "applyChanges"
            });
        }
    },

    renderDeferred: function () {
        var deferred = jQuery.Deferred();
        // Ask the searcher to retrieve html (which will trigger readyForDisplay immediately if the editHtml is cached)
        this.model.retrieveEditHtml().done(_.bind(function (editHtml) {
            deferred.resolve(this.render(editHtml));
        }, this));
        return deferred.promise();
    },

    hasAutoUpdate: function (editHtml) {
        return this.queryStateModel.getBasicAutoUpdate() && jQuery(editHtml).find(this.autoUpdate.join(',')).length !== 0;
    },

    render: function (editHtml) {
        var containsEditContent = !(/^\s*$/.test(editHtml));
        var renderedContent;
        if (containsEditContent) {
            renderedContent = AJS.$(this.template({
                displayBackButton: this.displayBackButton,
                displayUpdateCancel: !this.hasAutoUpdate(editHtml) && !jQuery(editHtml).hasClass("searchfilter-not-found")
            }));
        } else {
            renderedContent = AJS.$(JST["quote-builder/query/basic/lozenge-dropdown-cannot-edit"]({fieldName: this.model.getName()}));
        }

        this.$el.html(renderedContent);
        this.$el.find(".form-body").appendCatchExceptions(editHtml);
        this.$el.find("form").addClass(this.model.id + "-criteria");
        this.$el.find("label:first").remove(); // Server sends back label. todo: remove label on server
        // Trigger NEW_CONTENT_ADDED as searchers may need to add js to editHtml
        QuoteFlow.trigger(EventTypes.NEW_CONTENT_ADDED, [this.$el, ContentAddedReason.criteriaPanelRefreshed]);
        this._formData = this.$el.find("form").serialize();
        this.model.setInitParams(this._formData);
        return this.$el;
    },

    _stopPropagation: function (e) {
        e.stopPropagation();
    },

    _preventDefault: function (e) {
        e.preventDefault();
    },

    /**
     * @return {Boolean}
     */
    applyFilter: function () {
        var formData = this.$el.find("form").serialize();
        // Note: We can't compare formData to this.model.getSerializedParams() since this
        // is updated by the searcher HTML request, causing hasChanged to always evaluate
        // to true. @see JRADEV-14898
        var hasChanged = (formData !== this._formData);
        this._formData = formData;
        this.model.setSerializedParams(this._formData);
        return hasChanged;
    },

    applyChanges: function () {
        if (this.applyFilter()) {
            this.model.createOrUpdateClauseWithQueryString();
        }
    },

    // For non-auto-updating searchers only. This is different to clicking on "clear" within the CheckboxMultiSelect.
    _onCancelClicked: function (e) {
        e.preventDefault();
        this.triggerHideRequested(AJS.HIDE_REASON.cancelClicked);
    },

    _onSubmit: function (e) {
        e.preventDefault();
        this.triggerHideRequested(AJS.HIDE_REASON.submit);
    },

    _keyPressed: function (event) {
        if (event.keyCode === AJS.$.ui.keyCode.TAB) {
            var tabbableElements = AJS.$(":tabbable", this.$el);

            var noTabbableElements = (tabbableElements.length === 0);
            var shiftTabbingOnFirst = (event.shiftKey && (document.activeElement === tabbableElements.first()[0]));
            var tabbingOnLast = (!event.shiftKey && (document.activeElement === tabbableElements.last()[0]));

            if (noTabbableElements || shiftTabbingOnFirst || tabbingOnLast) {
                this.triggerHideRequested(AJS.HIDE_REASON.tabbedOut);
                event.preventDefault();
            }
        }
    }
});

module.exports = SearcherEditDialogView;
