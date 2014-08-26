QuoteFlow.Model.Asset.QueryState = Brace.Model.extend({
    BASIC_SEARCH: "basic",
    ADVANCED_SEARCH: "advanced",

    namedAttributes: [
        "style",
        "searchMode",
        "preferredSearchMode",
        "jql",
        "without",
        "layoutSwitcher",
        "autocompleteEnabled",
        "advancedAutoUpdate",
        "basicAutoUpdate",
        "basicOrderBy"
    ],

    defaults: {
        searchMode: "basic",
        preferredSearchMode: "basic"
    },

    switchToSearchMode: function (a) {
        this.setSearchMode(a);
    },

    switchPreferredSearchMode: function (a) {
        this.switchToSearchMode(a);
        this.setPreferredSearchMode(a);
        this._savePreferredSearchMode();
    },

    switchToPreferredSearchMode: function () {
        this.switchToSearchMode(this.getPreferredSearchMode());
    },

    hasSearchButton: function () {
        return this.getStyle() !== "field";
    },

    hasSubtleMoreCriteria: function () {
        return this.getStyle() !== "field";
    },

    _savePreferredSearchMode: function () {
        jQuery.ajax({
            url: AJS.contextPath() + "/rest/querycomponent/latest/userSearchMode",
            type: "POST",
            headers: { "X-Atlassian-Token": "nocheck" },
            data: { searchMode: this.getPreferredSearchMode() },
            error: _.bind(function(a) {
                if (JIRA.Issues.displayFailSearchMessage) {
                    JIRA.Issues.displayFailSearchMessage(a);
                }
            }, this),
            success: function() {
                JIRA.trace("jira.search.mode.changed");
            }
        });
    }
});