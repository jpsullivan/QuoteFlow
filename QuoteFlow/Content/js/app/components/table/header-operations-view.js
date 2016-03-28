"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var Dropdown = require('@atlassian/aui/lib/js/aui/drop-down');
var Tipsy = require('@atlassian/aui/lib/js/aui/tipsy');

/**
 * Handles adding an ajax-backed dialog to the tools menu.
 */
var AssetTableHeaderOperationsView = Marionette.ItemView.extend({

    initialize: function(options) {
        _.bindAll(this,
            "_onSearchSuccess",
            "_updateFilterPageResources");

        _.extend(this, options);
        this.initTools();
        this.initViews();
        this.initShare();

        this._results = this.search.getResults();
        this._results.onNewPayload(this._onSearchSuccess);
    },

    initViews: function() {
        var searchPageModule = this.searchPageModule;
        searchPageModule.on("change", this._updateFilterPageResources);
        this._updateFilterPageResources();

        // new Tipsy({
        //     el: this.$el.find(".header-views"),
        //     showCondition: ":not(.active)"
        // });
        //
        // var spinnerTimeout = this._spinnerTimeoutKeeper(".header-views");
        //
        // Dropdown.create({
        //     styleClass: 'header-views-menu aui-style-default',
        //     trigger: AJS.$(".header-views", this.$el),
        //     ajaxOptions: function() {
        //         var data = {jql: searchPageModule.getEffectiveJql()},
        //             filter = searchPageModule.getQuote();
        //
        //         if (filter && !filter.getIsSystem()) {
        //             data.filterId = filter.getId();
        //             if (searchPageModule.isDirty()) {
        //                 data.jql = searchPageModule.getJql();
        //                 data.modified = true;
        //             }
        //         }
        //         spinnerTimeout.start();
        //
        //         return {
        //             url: contextPath + "/rest/issueNav/1/issueNav/operations/views",
        //             type: "POST",
        //             headers: JIRA.Issues.XsrfTokenHeader,
        //             data: data,
        //             dataType: "json",
        //             cache: false,
        //             formatSuccess: function(data) {
        //                 spinnerTimeout.end();
        //                 return AJS.$(JIRA.Templates.IssueNavTable.views({
        //                     sections: data,
        //                     contextPath: contextPath
        //                 }));
        //             }
        //         };
        //     }
        // });
    },

    initTools: function () {
        var instance = this,
            searchPageModule = this.searchPageModule;

        // override use-cols click to use user columns
        AJS.$("body").delegate("#use-cols", "click", function(e) {
            e.preventDefault();
            instance.searchPageModule.columnConfig.setCurrentColumnConfig("user");
            instance.searchPageModule.columnConfig.refreshSearchWithColumns();
        });

        // override use-filter-cols click to use filter columns
        AJS.$("body").delegate("#use-filter-cols", "click", function(e) {
            e.preventDefault();
            instance.searchPageModule.columnConfig.setCurrentColumnConfig("filter");
            instance.searchPageModule.columnConfig.refreshSearchWithColumns();
        });

        // new Tipsy({
        //     el: this.$el.find(".header-tools"),
        //     showCondition: ":not(.active)",
        //     tipsy: {
        //         gravity: "ne"
        //     }
        // });
        // var spinnerTimeout = this._spinnerTimeoutKeeper(".header-tools");
        //
        // Dropdown.create({
        //     styleClass: 'header-tools-menu',
        //     trigger: AJS.$(".header-tools", instance.$el),
        //     ajaxOptions: function() {
        //         var data,
        //             filter = searchPageModule.getQuote();
        //
        //         data = {
        //             jql: searchPageModule.getEffectiveJql(),
        //             searchResultsTotal: instance._results.getTotal(),
        //             searchResultsPages: instance._results.getNumberOfPages(),
        //             useColumns: instance.searchPageModule.columnConfig.getCurrentColumnConfig().getName()!="user",
        //             skipColumns: searchPageModule.getActiveLayout().id==="split-view" //Skip columns for split-view
        //         };
        //
        //         if (filter && !filter.getIsSystem()) {
        //             data.filterId = filter.getId();
        //             if (searchPageModule.isDirty()) {
        //                 data.jql = searchPageModule.getJql();
        //             }
        //         }
        //
        //         spinnerTimeout.start();
        //
        //         return {
        //             url: contextPath + "/rest/issueNav/1/issueNav/operations/tools",
        //             type: "POST",
        //             headers: JIRA.Issues.XsrfTokenHeader,
        //             data: data,
        //             dataType: "json",
        //             cache: false,
        //             formatSuccess: function(data) {
        //                 spinnerTimeout.end();
        //                 if(data && data.length === 0) {
        //                     return AJS.$('<div class="menu-empty-content"></span>' + AJS.I18n.getText("issue.nav.operations.tools.nooptions") + '<div>');
        //                 } else {
        //                     return AJS.$(JIRA.Templates.IssueNavTable.tools({
        //                         groups: data,
        //                         contextPath: contextPath
        //                     }));
        //                 }
        //             }
        //         };
        //     }
        // });
    },

    initShare: function() {
        //Init the tipsy tooltip for the share button
        // new Tipsy({
        //     el: this.$el.find(".issuenav-share"),
        //     showCondition: ":not(.active)"
        // });
    },

    _onSearchSuccess: function() {
        //JRADEV-18219 Only hide header-tools. header-views should always be visible, even for searches with 0 results
        AJS.$(".header-tools").toggle(this.search.getResults().getDisplayableTotal() > 0);
    },

    _updateFilterPageResources: function() {
        // These hidden input params are used by javascript in com.atlassian.jira.gadgets:searchrequestview-charts
        // to load custom dialogs for chart / dashboard view menu items
        // The '.operations-view-data .parameters' element updates AJS.params, which is a legacy bus for passing
        // variables around the page
        var $fieldset = this.$el.find('.operations-view-data > fieldset'),
            searchPageModule = this.searchPageModule;

        $fieldset.empty();

        if (searchPageModule.getQuote() && !searchPageModule.isDirty()) {
            AJS.$('<input type="hidden" id="filterId" />').val(searchPageModule.getQuote().getId()).appendTo($fieldset);
            AJS.$('<input type="hidden" id="jql" />').val("").appendTo($fieldset);
        }
        else {
            AJS.$('<input type="hidden" id="filterId" />').val("");
            AJS.$('<input type="hidden" id="jql" />').val(searchPageModule.getJql()).appendTo($fieldset);
        }

        // The gadgets and share plugins (possibly others) get the current
        // filter/JQL from this metadata. Both plugins ignore filter-jql if
        // filter-id is set, so we only set the latter for modified filters.
        var filter = searchPageModule.getQuote(),
            filterID = filter && filter.getId();

        if (filterID && !filter.getIsSystem() && !searchPageModule.isDirty()) {
            AJS.Meta.set("filter-id", filterID);
            AJS.Meta.set("filter-jql", undefined);
        } else {
            AJS.Meta.set("filter-id", undefined);
            AJS.Meta.set("filter-jql", searchPageModule.getEffectiveJql());
        }
    },

    /**
     * Utility timeout keeper container for starting and ending the spinner icon on dropdown menu
     */
    _spinnerTimeoutKeeper: function(element) {
        var spinnerTimeout;

        return {
            start: function() {
                if (spinnerTimeout) {
                    spinnerTimeout = clearTimeout(spinnerTimeout);
                }
                spinnerTimeout = setTimeout(function() {
                    spinnerTimeout = undefined;
                    AJS.$(element).addClass("spinner");
                }, 1000);
            },

            end: function() {
                spinnerTimeout = clearTimeout(spinnerTimeout);
                AJS.$(element).removeClass("spinner");
            }
        };
    }
});

module.exports = AssetTableHeaderOperationsView;
