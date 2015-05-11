"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var SearcherDialog = require('./searcher-dialog');

var CriteriaView = Brace.View.extend({

    tagName: 'li',

    template: JST["quote-builder/query/basic/criteria-button"],
    contentTemplate: JST["quote-builder/query/basic/criteria-button-content"],

    events: {
        'click' : 'hideTipsy',
        'click .criteria-selector': '_onClickCriteriaSelector',
        'click .remove-filter': '_onClickRemoveCriteria',
        'mousedown .remove-filter': '_preventFocusWhileDisabled',
        'keydown': '_onKeydown'
    },

    initialize: function(options) {
        this.extended = options.extended; // Indicates that a valid criteria can be removed from the UI
        this.searcherCollection = options.searcherCollection;
        this.searcherCollection.onCollectionChanged(this.update, this);
        this.searcherCollection.onInteractiveChanged(this._handleInteractiveChanged, this);
        this.searcherCollection.bind('change:isSelected', this._onCriteriaSelectionChanged, this);

        QuoteFlow.SearcherDialog.instance.onHide(_.bind(this._addTooltip, this));
        QuoteFlow.SearcherDialog.instance.onShow(_.bind(this._onCriteriaDialogShow, this));
    },

    /**
     * Initial render, should only be called once
     */
    render: function() {
        this.$el.html(this.template({
            id: this.model.getId()
        }));
        this.$el.attr('data-id', this.model.getId());
        if (this.extended) {
            this.$el.addClass('extended-searcher');
        }
        this.update();
        this.prepareForDisplay();
    },

    /**
     * Prepare the view before it is displayed: hook up event handlers, etc.
     */
    prepareForDisplay: function() {
        this._addTooltip();
        this.delegateEvents();
    },

    hideTipsy: function () {
        if (this.tipsy) {
            this.tipsy.hide();
        }
    },

    /**
     * Update view to reflect model changes
     */
    update: function() {

        var noSearchers = this.searcherCollection.length === 0; // Searchers have not loaded, but render them anyway default 'All' value
        var searcher = this._getSearcher();
        var validSearcher = this._isValidSearcher();
        var hidden = !noSearchers && !searcher; // Searchers have loaded, but this searcher is not present
        var disabled = noSearchers || !validSearcher;
        var $button = this.$('button');
        var $cross = this.$('.remove-filter');

        this.$el.toggleClass('hidden', hidden);

        $button
            .attr('aria-disabled', disabled ? 'true' : null)
            .html(this.contentTemplate({
                name: searcher && searcher.getName() || this.model.getName(),
                viewHtml: searcher && searcher.getViewHtml(),
                extended: this.extended
            }));

        // Validity
        this.$el
            .toggleClass('invalid-searcher', !validSearcher)
            .toggleClass('partial-invalid-searcher', (validSearcher && $button.find(".invalid_sel").length !== 0));

        $cross.toggle(!validSearcher || !!this.extended);

        return this;
    },

    /**
     * Destroys and cleans up the criteria view.
     * As the name implies, this is a destructive operation, and the View should not be used afterwards (construct a new one if needed).
     * Meant for use with extended criteria - primary criteria should only be hidden, not destroyed.
     */
    destroy: function() {
        this.hideTipsy();
        this.$el.remove();
    },

    _getSearcher: function() {
        return this.searcherCollection.get(this.model.getId());
    },

    /**
     * Searcher validity: returns true if there is no searcher, or searcher is valid
     */
    _isValidSearcher: function() {
        var searcher = this._getSearcher();
        return !searcher || !(searcher.getValidSearcher() === false); // Assume getValidSearcher()===undefined means valid searcher
    },

    /**
     * Value validity: returns true if there is an invalid value for the searcher
     */
    _containsInvalidValue: function(searcher) {
        return (AJS.$(searcher.getViewHtml()).find('.invalid_sel').length > 0);
    },

    _showDialog: function() {
        if (this.searcherCollection.isInteractive() && this._getSearcher() && this._isValidSearcher()) {
            QuoteFlow.SearcherDialog.instance.show(this._getSearcher());
        }
    },

    /**
     * Clear the searcher jql for this criteria
     *
     * direction is optional, and supplied when removing a criteria by
     * Backspace or Delete keys, which have an implicit direction associated with them.
     * The direction is passed along with the beforeCriteriaRemoved event so BasicQueryView can choose
     * an appropriate element to focus.
     *
     * If supplied, direction is either -1 (back) or 1 (forward)
     */
    _removeCriteria: function(direction) {
        var removable = this.extended || !this._isValidSearcher();
        if (this.searcherCollection.isInteractive() && removable) {
            this.searcherCollection.triggerBeforeCriteriaRemoved(this.model.getId(), direction);
            /*
             Need to defer otherwise InlineLayer will hide. This happens because the inline dialog chooses to close
             if the target element clicked is not a child element of the InlineLayer. Because we switch the content in the
             dialog, the back link is no longer in the InlineLayer therefor not a child element.  To rectify the problem
             we delay the toggling of content.
             */
            _.defer(_.bind(function () {
                this.searcherCollection.clearClause(this.model.getId());
            }, this));
        }
    },

    /**
     * @return the text to show in the searcher's tooltip.
     * @private
     */
    _getTooltipText: function() {
        var searcherModel = this.searcherCollection.get(this.model.getId());
        var tooltipText;

        if (!this._isValidSearcher()) {
            tooltipText = AJS.I18n.getText("issues.components.query.searcher.invalid.searcher");
        } else if (this._containsInvalidValue(searcherModel)) {
            tooltipText = AJS.I18n.getText("issues.components.query.searcher.invalid.view.value");
        } else {
            tooltipText = searcherModel.getTooltipText();
        }

        return searcherModel && tooltipText || "";
    },

    /**
     * Add a tooltip to the searcher.
     * <p/>
     * This method can safely be called multiple times.
     *
     * @private
     */
    _addTooltip: function () {
        // this.tipsy = new JIRA.Issues.Tipsy({
        //     el: this.$el,
        //     showCondition: this.searcherCollection.isInteractive,
        //     tipsy: {
        //         title: _.bind(this._getTooltipText, this)
        //     }
        // });
    },

    _handleInteractiveChanged: function(interactive) {
        this.$("button, .remove-filter").attr("aria-disabled", (interactive) ? null : "true");
    },

    _onClickCriteriaSelector: function(event) {
        debugger;
        if (this.searcherCollection.isInteractive() && this._getSearcher() && this._isValidSearcher()) {
            QuoteFlow.SearcherDialog.instance.toggle(this._getSearcher());
        }
        event.preventDefault();
    },

    _onClickRemoveCriteria: function(event) {
        this._removeCriteria();
        event.preventDefault();
    },

    _onKeydown: function(event) {
        switch (event.which) {
            case AJS.$.ui.keyCode.DOWN:
                this._showDialog();
                break;
            case AJS.$.ui.keyCode.ESCAPE:
                this.$('button:focus').blur();
                break;
            case AJS.$.ui.keyCode.BACKSPACE:
                this._removeCriteria(-1);
                break;
            case AJS.$.ui.keyCode.DELETE:
                this._removeCriteria(1);
                break;
            default:
                return;
        }
        event.preventDefault();
    },

    /**
     * Remove the searcher's tooltip if its dialog is showing.
     * <p/>
     * Called when SearcherDialog is shown.
     *
     * @private
     */
    _onCriteriaDialogShow: function () {
        var currentSearcher = QuoteFlow.SearcherDialog.instance.getCurrentSearcher();
        if (currentSearcher == this._getSearcher()) {
            this.tipsy && this.tipsy.remove();
        }
    },

    _preventFocusWhileDisabled: function(event) {
        if (jQuery(event.target).closest("[aria-disabled=true]").length > 0) {
            event.preventDefault();
        }
    }
});

module.exports = CriteriaView;
