QuoteFlow.UI.Asset.Navigator.Criteria = QuoteFlow.Views.Base.extend({
    events: {
        click: "hideTipsy",
        "click .criteria-selector": "_onClickCriteriaSelector",
        "click .remove-filter": "_onClickRemoveCriteria",
        "mousedown .remove-filter": "_preventFocusWhileDisabled",
        keydown: "_onKeydown"
    },

    initialize: function (a) {
        this.extended = a.extended;
        this.searcherCollection = a.searcherCollection;
        this.searcherCollection.onCollectionChanged(this.update, this._onKeydown);
        this.searcherCollection.onInteractiveChanged(this._handleInteractiveChanged, this);
        this.searcherCollection.bind("change:isSelected", this._onCriteriaSelectionChanged, this);

        this.dialog = new QuoteFlow.UI.Asset.Navigator.CriteriaDialog({
            el: $("#" + this.model.getId() + "-dropdown"),
            criteria: this.model.getId(),
            queryStateModel: this.searcherCollection.queryStateModel
        });
    },

    hideTipsy: function () {
        if (this.tipsy) {
            this.tipsy.hide();
        }
    },

    update: function () {
        var g = this.searcherCollection.length === 0;
        var f = this._getSearcher();
        var b = this._isValidSearcher();
        var e = !g && !f;
        var a = g || !b;
        var d = this.$("button");
        var c = this.$(".remove-filter");
        this.$el.toggleClass("hidden", e);
        d.attr("aria-disabled", a ? "true" : null).html(this.contentTemplate({
            name: f && f.getName() || this.model.getName(),
            viewHtml: f && f.getViewHtml(),
            extended: this.extended
        }));
        this.$el.toggleClass("invalid-searcher", !b).toggleClass("partial-invalid-searcher", (b && d.find(".invalid_sel").length !== 0));
        c.toggle(!b || !!this.extended);
        return this;
    },

    destroy: function () {
        this.hideTipsy();
        this.$el.remove();
    },

    _getSearcher: function () {
        return this.searcherCollection.get(this.model.getId());
    },

    _isValidSearcher: function () {
        var a = this._getSearcher();
        return !a || !(a.getValidSearcher() === false);
    },

    _containsInvalidValue: function (a) {
        return (AJS.$(a.getViewHtml()).find(".invalid_sel").length > 0);
    },

    _showDialog: function () {
        var searcherName = this.model.getId();
        AJS.$('[data-id="' + searcherName + '"] button').trigger("aui-button-invoke");
    },

    _removeCriteria: function (b) {
        var a = this.extended || !this._isValidSearcher();
        if (this.searcherCollection.isInteractive() && a) {
            this.searcherCollection.triggerBeforeCriteriaRemoved(this.model.getId(), b);
            _.defer(_.bind(function () {
                this.searcherCollection.clearClause(this.model.getId());
            }, this));
        }
    },

    _getTooltipText: function () {
        var b = this.searcherCollection.get(this.model.getId());
        var a;
        if (!this._isValidSearcher()) {
            a = "This criteria is not valid for the project and/or issue type";
        } else {
            if (this._containsInvalidValue(b)) {
                a = "This criteria contains invalid value(s)";
            } else {
                a = b.getTooltipText();
            }
        }
        return b && a || "";
    },

    _addTooltip: function () {
        this.tipsy = new JIRA.Issues.Tipsy({ el: this.$el, showCondition: this.searcherCollection.isInteractive, tipsy: { title: _.bind(this._getTooltipText, this) } });
    },

    _handleInteractiveChanged: function (a) {
        this.$("button, .remove-filter").attr("aria-disabled", (a) ? null : "true");
    },

    _onClickRemoveCriteria: function (a) {
        this._removeCriteria();
        a.preventDefault();
    },

    _onKeydown: function (a) {
        switch (a.which) {
            case AJS.$.ui.keyCode.DOWN:
                this._showDialog();
                break;
            case AJS.$.ui.keyCode.ESCAPE:
                this.$("button:focus").blur();
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
        a.preventDefault();
    },

    _onCriteriaDialogShow: function () {
//        var a = JIRA.Issues.SearcherDialog.instance.getCurrentSearcher();
        if (a == this._getSearcher()) {
            this.tipsy && this.tipsy.remove();
        }
    },

    _preventFocusWhileDisabled: function (a) {
        if (jQuery(a.target).closest("[aria-disabled=true]").length > 0) {
            a.preventDefault();
        }
    }
})