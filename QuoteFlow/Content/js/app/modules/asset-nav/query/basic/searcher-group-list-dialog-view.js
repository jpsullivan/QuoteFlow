"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var CheckboxMultiSelectSuggestHandler = require('../../../../components/select/checkbox-multi-select-suggest-handler');
var GroupDescriptor = require('../../../../components/list/group-descriptor');
var Reasons = require('../../util/reasons');
var SearcherDialog = require('./searcher-dialog');
var SuggestHelper = require('../../../../components/select/suggest-helper');

var $moreCriteriaFooter;

/**
 * List of searchers that can be added to a search
 */
var SearcherGroupListDialogView = Brace.View.extend({

    template: JST["quote-builder/query/basic/searcher-dropdown-content"],

    /**
     * searcherSelected(id): a searcher has been selected. id is the id of the searcher
     * hideRequested: dialog close has been requested
     */
    namedEvents: ["searcherSelected", "hideRequested"],

    events: {
        "keydown": "_keyPressed"
    },

    initialize: function(options) {
        this.searcherCollection = options.searcherCollection;
        this.$el.scrollLock('.aui-list-scroll');
    },

    render: function() {

        var descriptors = this.searcherCollection.getAddMenuGroupDescriptors(),
            tooManySearchers = descriptors.length && descriptors[0].properties.items.length > SearcherGroupListDialogView.CRITERIA_DISPLAY_LIMIT;

        var select = jQuery(AJS.Templates.queryableSelect({
            descriptors: descriptors,
            id:"criteria"
        }));

        // Even though it works, weird stuff happens if you call .html(select) since select is a jQuery object.
        this.$el.empty().append(select);

        var options = {
                element: select,
                suggestionsHandler: SearcherGroupListDialogView.SuggestHandler,
                hideFooterButtons: true
            };

        var searchersHiddenMessage = AJS.I18n.getText("issues.components.query.searcher.hidden.global"),
            searchersHiddenDetails = AJS.I18n.getText("issues.components.query.searcher.hidden.global.desc");

        // Performance optimisation. When appending more than 100 custom fields performance suffers dramatically.
        if (tooManySearchers) {
            options.maxInlineResultsDisplayed = SearcherGroupListDialogView.CRITERIA_DISPLAY_LIMIT;
            searchersHiddenMessage = AJS.I18n.getText("issues.components.query.searcher.hidden.global.too.many.searchers");
            searchersHiddenDetails = AJS.I18n.getText("issues.components.query.searcher.hidden.global.desc.too.many.searchers");
        }

        var multiselect = new AJS.CheckboxMultiSelect(options);

        $moreCriteriaFooter = jQuery("<div class='more-criteria-footer' />").html(searchersHiddenMessage);
        this.$el.append($moreCriteriaFooter);
        new JIRA.Issues.Tipsy({
            el: $moreCriteriaFooter,
            tipsy: {
                title: function(){
                    return searchersHiddenDetails;
                },
                className: "tipsy-front"
            }
        });

        // this.$el is an element owned by AJS.InlineLayer and is detached from the dom each time we switch
        // from basic to advanced. Thus, we need to rebind when we render rather than use the backbone way of
        // binding.
        this.$el.unbind("selected").bind("selected", _.bind(this._searcherSelected, this));
        this.$el.unbind("unselect").bind("unselect", _.bind(this._searcherUnselected, this));

        return this.$el;
    },

    _searcherUnselected: function (e, descriptor) {
        this.searcherCollection.clearClause(descriptor.properties.value);
    },

    _searcherSelected: function(e, descriptor) {
        var searcher = this.searcherCollection.getSearcher(descriptor.properties.value);
        searcher.select();
        SearcherDialog.instance.hide();
        SearcherDialog.instance.show(searcher);
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
}, {
    CRITERIA_DISPLAY_LIMIT: 100
});

SearcherGroupListDialogView.SuggestHandler = CheckboxMultiSelectSuggestHandler.extend({
    formatSuggestions: function (groups, query) {
        var numberHidden = 0,
            selectedItems = SuggestHelper.removeDuplicates(this.model.getDisplayableSelectedDescriptors());

        // Prepend a group containing all selected items.
        groups.splice(0, 0, new GroupDescriptor({
            actionBarHtml: selectedItems.length > 1 ? this.createClearAll() : null,
            items: selectedItems,
            styleClass: "selected-group"
        }));

        _.each(groups, function (group) {
            if (query.length === 0) {
                var items = _.filter(group.items(), function (item, index) {
                    var meta = item.meta();
                    return meta && meta.isShown && index < SearcherGroupListDialogView.CRITERIA_DISPLAY_LIMIT;
                });

                numberHidden += group.items().length - items.length;
                group.items(items);
            } else {
                _.each(group.items(), function (item) {
                    var meta = item.meta();
                    item.disabled(!(meta && meta.isShown));
                });
            }
        });

        _.defer(function () {
            if ($moreCriteriaFooter) {
                if (numberHidden) {
                    $moreCriteriaFooter.show().find(".hidden-no").text(numberHidden);
                } else {
                    $moreCriteriaFooter.hide();
                }
            }
        });

        return groups;
    }
});

module.exports = SearcherGroupListDialogView;
