"use strict";

var $ = require('jquery');
var _ = require('underscore');

var EventTypes = require('../util/types');
var ContentAddedReason = require('../util/reasons');
var CheckboxMultiSelect = require('../../../components/select/checkbox-multi-select');
var GroupDescriptor = require('../../../components/list/group-descriptor');
var ItemDescriptor = require('../../../components/list/item-descriptor');

var InitUserSparkler = {

    _formatUserGroupResponse: function (response) {
        var users = _formatUserResponse(response.users, true);
        var groups = _formatGroupResponse(response.groups, true);
        var items = [].concat(users).concat(groups);
        return [new GroupDescriptor({items: items})];
    },

    _formatUserResponse: function (response, prefix) {
        return _.map(response.users, function (item) {
            return new ItemDescriptor({
                highlighted: true,
                html: item.html,
                icon: item.avatarUrl,
                label: item.displayName,
                value: (prefix ? "user:" : "") + item.name
            });
        });
    },

    _formatGroupResponse: function (response, prefix) {
        return _.map(response.groups, function (item) {
            return new ItemDescriptor({
                highlighted: true,
                html: item.html,
                icon: QuoteFlow.RootUrl + "/images/icons/icon_groups_16.png",
                label: item.name,
                value: (prefix ? "group:" : "")+ item.name
            });
        });
    },

	initSparklers: function ($ctx) {
		$ctx.find(".js-default-checkboxmultiselect").each(function () {
            var $select = $(this);
            if (!$select.data("checkboxmultiselect")) {
                $select.data("checkboxmultiselect",  new CheckboxMultiSelect({
                    element: this
                }));
            } else {
                $select.data("checkboxmultiselect").render();
            }
        });
	},

	register: function () {
		QuoteFlow.bind(EventTypes.NEW_CONTENT_ADDED, _.bind(function (e, $ctx) {
            var self = this;

            // jQuery(".js-usergroup-checkboxmultiselect", $ctx).each(function () {
            //     var ajaxData = {};
            //     // grab additional parameters from fieldset
            //     AJS.$("fieldset.user-group-searcher-params", $ctx).each(function(){
            //         ajaxData = JIRA.parseOptionsFromFieldset(AJS.$(this))
            //     });
            //     ajaxData.showAvatar = true;
            //     new AJS.CheckboxMultiSelect({
            //         element: this,
            //         maxInlineResultsDisplayed: 10,
            //         content: "mixed",
            //         ajaxOptions: {
            //             url: QuoteFlow.RootUrl + "/rest/api/latest/groupuserpicker",
            //             data: ajaxData,
            //             query: true,
            //             formatResponse: self._formatUserGroupResponse
            //         }
            //     });
            // });

            jQuery(".js-user-checkboxmultiselect", $ctx).each(function () {
                new CheckboxMultiSelect({
                    element: this,
                    maxInlineResultsDisplayed: 5,
                    content: "mixed",
                    ajaxOptions: {
                        url: QuoteFlow.RootUrl + "/rest/api/latest/user/picker",
                        data: {
                            showAvatar: true
                        },
                        query: true,
                        formatResponse: function (items) {
                            return self._formatUserResponse(items, false);
                        }
                    }
                });
            });

            jQuery(".js-group-checkboxmultiselect", $ctx).each(function () {
                new CheckboxMultiSelect({
                    element: this,
                    maxInlineResultsDisplayed: 5,
                    content: "mixed",
                    ajaxOptions: {
                        url: QuoteFlow.RootUrl + "/rest/api/latest/groups/picker",
                        data: {
                            showAvatar: true
                        },
                        query: true,
                        formatResponse: function (items) {
                            return self._formatGroupResponse(items, false);
                        }
                    }
                });
            });
	    }, this));
	}
};

module.exports = InitUserSparkler;
