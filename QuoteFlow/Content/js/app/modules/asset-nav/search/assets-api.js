"use strict";

var $ = require('jquery');


var AssetsApi = {
    searchPageModule: null,

    /**
     * Initialize the API.
     *
     * @param {object} options
     * @param {SearchPageModule} options.searchPageModule
     */
    initialize: function (options) {
        this.searchPageModule = options.searchPageModule;
    },

    /**
     * Initiate editing the value of a field on the selected issue.
     *
     * It's not always possible for the user to edit a field, for example if the issue is currently not editable, or
     * if the user doesn't have the correct permissions.
     *
     * For visible fields inline editing is used. For hidden fields, a modal dialog is used.
     *
     * @param fieldId the ID of the field to edit
     * @return {boolean} true if the field editing did/will happen, otherwise false.
     */
    editFieldOnSelectedIssue: function (fieldId) {
        var fields = JIRA.Issues.Api.getFieldsOnSelectedIssue(),
            field = fields && fields.get(fieldId),
            permitted = field && field.isEditable();

        if (permitted && this.searchPageModule) {
            JIRA.Issues.Application.execute("issueEditor:editField", field);
        }

        return permitted;
    },

    /**
     * Focus the search controls.
     * <p/>
     * In basic mode, the project criteria; in advanced mode, the JQL input.
     */
    focusSearch: function () {
        AJS.$(".criteria-selector:first, #advanced-search").focus().select();
    },

    /**
     * @return {SimpleIssue} the currently selected issue.
     */
    getSelectedIssue: function () {
        return this.searchPageModule.getEffectiveIssue();
    },

    /**
     * @return {null|number} the ID of the selected issue or null.
     */
    getSelectedIssueId: function () {
        return this.searchPageModule.getEffectiveIssueId();
    },

    /**
     * @return {null|string} the key of the selected issue or <tt>null</tt>.
     */
    getSelectedIssueKey: function () {
        return this.searchPageModule.getEffectiveIssueKey();
    },

    /**
     * Returns the fields on the selected issue.
     *
     * @return {undefined|Backbone.Collection} collection of {JIRA.Components.IssueEditor.Models.Field} objects
     */
    getFieldsOnSelectedIssue: function () {
        var fields = QuoteFlow.application.request("assetEditor:fields");
        return fields.length ? fields : undefined;
    },

    /**
     * @return {Boolean} whether there are saves in progress.
     */
    hasSavesInProgress: function () {
        return QuoteFlow.application.request("assetEditor:hasSavesInProgress");
    },

    /**
     * @return {boolean} whether an issue is currently being loaded.
     */
    isCurrentlyLoadingIssue: function () {
        return this.searchPageModule.isCurrentlyLoadingIssue();
    },

    /**
     * @return {boolean|null} whether the selected issue can be opened, or <tt>null</tt> if no issue is selected.
     */
    isSelectedIssueAccessible: function () {
        return this.searchPageModule.isHighlightedIssueAccessible();
    },

    /**
     * @return {boolean} whether an issue is visible.
     */
    issueIsVisible: function () {
        return this.searchPageModule.isIssueVisible();
    },

    /**
     * Select the next issue.
     * <p/>
     * When in issue search, the next issue is highlighted; when viewing an
     * issue, the next one is loaded. No-op if an overlay is visible.
     */
    nextIssue: function () {
        this.searchPageModule.nextIssue();
    },

    /**
     * Open the focus shifter.
     */
    openFocusShifter: function () {
        if (this.searchPageModule.isIssueVisible()) {
            this.searchPageModule.openFocusShifter();
        }
    },

    /**
     * Select the previous issue.
     * 
     * When in issue search, the previous issue is highlighted; when viewing
     * an issue, the previous one is loaded. No-op if an overlay is visible.
     */
    prevIssue: function () {
        this.searchPageModule.prevIssue();
    },

    /**
     * Refresh the content of the selected issue, by merging changes from the server.
     *
     * @param {object} [options] Extra options to include in the internal triggerRefreshIssue() call
     * @returns {jQuery.Promise}
     *
     * The returned promise is:
     * - resolved when the selected issue is refreshed, or if there is no selected issue
     * - rejected *only* when refreshing the selected issue fails
     */
    refreshSelectedIssue: function (options) {
        return QuoteFlow.application.request("assetEditor:refreshAsset", options);
    },

    /**
     * Return to issue search.
     * <p/>
     * If a form is dirty, we ask the user to confirm navigation.
     *
     * @param {boolean} ignoreDirtiness Whether we should ignore dirtiness
     *     (used, for example, to force return after deleting an issue).
     */
    returnToSearch: function (ignoreDirtiness) {
        this.searchPageModule.returnToSearch({
            ignoreDirtiness: ignoreDirtiness
        });
    },

    /**
     * @param {Object|null} issueProps - if null/undefined, use currently selected issue
     */
    showInlineIssueLoadError: function (issueProps) {
        this.searchPageModule.showInlineIssueLoadError(issueProps);
    },

    /**
     * Switch to the next search layout.
     * <p/>
     * We cycle through layouts in the order they appear in the layout switcher.
     */
    switchLayouts: function (options) {
        var currentIndex = -1,
            currentLayout = this.searchPageModule.getCurrentLayout(),
            layouts = this.searchPageModule.getSortedLayouts();

        _.find(layouts, function (layout, index) {
            if (currentLayout instanceof layout.view) {
                currentIndex = index;
                return true;
            }
        });

        var newLayout = layouts[(currentIndex + 1) % layouts.length];
        this.searchPageModule.changeLayout(newLayout.id, options);
    },

    /**
     * Return if the current query is valid.
     * @returns {boolean}
     */
    isQueryValid: function () {
        return this.searchPageModule.queryModule.isQueryValid();
    },

    /**
     * @returns {*|boolean}
     */
    isFullScreenIssueVisible: function () {
        return this.searchPageModule.isFullScreenIssueVisible();
    },

    /**
     * View the issue that is currently highlighted in the issue table.
     */
    viewSelectedIssue: function () {
        if (!JIRA.Issues.Api.isSelectedIssueAccessible()) return;

        var issueKey = JIRA.Issues.Api.getSelectedIssueKey();

        if (!this.searchPageModule.isIssueVisible()) {
            this.searchPageModule.update({
                selectedIssueKey: issueKey
            });
        }
    },

    /**
     * Undocks/Docks filter panel
     */
    toggleFilterPanel: function () {
        this.searchPageModule.toggleFilterPanel();
    },

    /**
     * Returns a deferred that is resolved once all inline edits are complete.
     * Or straight away if you have no inline edits pending.
     *
     * @return jQuery.Deferred
     */
    waitForSavesToComplete: function () {
        var d = jQuery.Deferred();
        window.setTimeout(function () {
            if (!JIRA.Issues.Api.hasSavesInProgress()) {
                d.resolve();
            } else {
                JIRA.bind(JIRA.Events.INLINE_EDIT_SAVE_COMPLETE, function (e) {
                    if (!JIRA.Issues.Api.hasSavesInProgress()) {
                        d.resolve();
                    }
                });
            }
        }, 10);
    },

    updateIssue: function (issue, message) {
        message = message || "thanks_issue_updated";

        return this.searchPageModule.updateIssue({
            key: issue.key,
            id: issue.id,
            action: JIRA.Issues.Actions.UPDATE,
            message: message,
            meta: {}
        });
    }
};

module.exports = AssetsApi;