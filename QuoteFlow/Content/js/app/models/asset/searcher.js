QuoteFlow.Model.Asset.Searcher = Brace.Model.extend({
    namedAttributes: ["id", "name", "isShown", "viewHtml", "editHtml", "groupId", "groupName", "initParams", "isSelected", "jql", "position", "serializedParams", "validSearcher", "key", "lastViewed"],

    namedEvents: ["readyForDisplay"],

    initialize: function () {
        this._onEditHtmlUpdated();
        this.bind("change:editHtml", this._onEditHtmlUpdated, this);
    },

    parse: function (a) {
        if (a.viewHtml) {
            a.viewHtml = this._cleanViewHtml(a.viewHtml);
        }
        return a;
    },

    _cleanViewHtml: function (b) {
        var a = AJS.$("<div/>").appendCatchExceptions(b);
        a.find(".fieldLabel").remove();
        a.find("a").replaceWith(function () {
            return AJS.$(this).html();
        });
        a.find(".searcherValue").slice(0, -1).each(function () {
            AJS.$(this).after("-");
        });
        a.find("*").removeAttr("title");
        return a.html();
    },

    createOrUpdateClauseWithQueryString: function (a) {
        return this.collection.createOrUpdateClauseWithQueryString(this.id, a);
    },

    getQueryString: function () {
        var a = {};
        if (this.collection.QUERY_ID === this.getId()) {
            if (this.getViewHtml()) {
                a[this.collection.QUERY_PARAM] = this.getDisplayText();
                return AJS.$.param(a);
            }
            return null;
        }
        if ((!this.getValidSearcher() || /^\s*$/.test(this.getEditHtml())) && this.getJql()) {
            a = {};
            a[this.collection.JQL_INVALID_QUERY_PREFIX + this.getId()] = this.getJql();
            return AJS.$.param(a);
        }
        return this.getSerializedParams();
    },

    getDisplayText: function () {
        var a = this.getViewHtml();
        var c = "";
        if (a) {
            var b = AJS.$("<div>").appendCatchExceptions(a);
            c = AJS.$.trim(b.text()).replace(/[\n\r\s]+/g, " ");
        }
        return c;
    },

    hasClause: function () {
        if (this.collection.QUERY_ID === this.getId()) {
            return !!this.getViewHtml();
        } else {
            if (!this.getValidSearcher()) {
                return !!this.getJql();
            } else {
                return !!this.getQueryString();
            }
        }
    },

    clearSearchState: function () {
        this.set({ viewHtml: null, editHtml: null, jql: null, validSearcher: null, isSelected: false });
    },

    _now: Date.now || function () {
        return new Date().getTime();
    },

    select: function () {
        this.set({ isSelected: true, position: this.collection.getNextPosition(), validSearcher: true, lastViewed: this._now() });
    },

    hasErrorInEditHtml: function () {
        var a = jQuery("<div>").htmlCatchExceptions(this.getEditHtml());
        if (!a) {
            return false;
        } else {
            return a.find(".error, .has-errors").length > 0;
        }
    },

    clearEditHtml: function () {
        this.setEditHtml(null);
    },

    _onEditHtmlUpdated: function () {
        this.setSerializedParams(jQuery("<form/>").appendCatchExceptions(this.getEditHtml()).serialize());
    },

    getTooltipText: function () {
        if (this.getValidSearcher !== false) {
            var a = this.getDisplayText() || "All";
            return this.getName() + ": " + a;
        } else {
            return "This criteria is not valid for the project and/or issue type";
        }
    },

    searchersReady: function () {
        return this.collection.searchersReady();
    }
})