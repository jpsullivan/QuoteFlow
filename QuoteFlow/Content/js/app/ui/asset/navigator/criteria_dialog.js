QuoteFlow.UI.Asset.Navigator.CriteriaDialog = QuoteFlow.Views.Base.extend({
    events: {
        "click .check-list-item input": "applyChanges",
        keydown: "_onKeydown"
    },

    initialize: function (options) {
        this.criteria = options.criteria;
        this.formData = "";
        this.queryStateModel = options.queryStateModel;
    },

    setDropdownEventBindings: function (criteria) {
        AJS.$(this.el).on({
            "aui-dropdown2-show": function (k, i) {
                var currentTarget = $(k.currentTarget);
                var j = currentTarget.find(":input:not(submit):visible:first");
                j.focus();
                currentTarget.find(".aui-list-scroll").scrollTop(0);
            }
        });
    },

    _onKeydown: function (a) {
        switch (a.which) {
            case AJS.$.ui.keyCode.TAB:
                if (this.isTabbingOutOfDropdown()) {
                    // hide the dialog
                    this.$("button:focus").blur();
                    e.preventDefault();
                }
                break;
            default:
                return;
        }
        a.preventDefault();
    },

    applyFilter: function () {
        var b = this.$el.find("form").serialize();
        var a = (b !== this.formData);
        this.formData = b;
        this.model.setSerializedParams(this.formData);
        return a;
    },

    applyChanges: function () {
        if (this.applyFilter()) {
            this.model.createOrUpdateClauseWithQueryString();
        }
    },

    isTabbingOutOfDropdown: function () {
        var tabbableElement = AJS.$(":tabbable", this.$el);
        var isTabbable = (tabbableElement.length === 0);
        var d = (e.shiftKey && (document.activeElement === tabbableElement.first()[0]));
        var c = (!e.shiftKey && (document.activeElement === tabbableElement.last()[0]));

        return !!(isTabbable || d || c);
    }
})