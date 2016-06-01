"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var SimpleAssetList = Marionette.Object.extend({
    initialize: function () {
        if (this.options.displayInlineIssueCreate) {
            this.inlineIssueCreate = new InlineIssueCreate();
            this.listenTo(this.inlineIssueCreate, {
                "issueCreated": function (issueInfo) {
                    this.trigger("issueCreated", issueInfo);
                },
                "activated deactivated": function () {
                        // We need to defer this because the Inline Issue Create component
                        // trigger those events *before* it is actually activated or deactivated
                    _.defer(_.bind(function () {
                        this.list.adjustSize();
                    }, this));
                }
            });
        }

        this.list = new ControllerList({
            baseURL: this.options.baseURL,
            inlineIssueCreate: this.inlineIssueCreate
        });
        this.listenTo(this.list, {
            "goToPreviousPage": function () {
                this.trigger("list:pagination");
                this.searchResults.jumpToPage("prev");
            },
            "goToNextPage": function () {
                this.trigger("list:pagination");
                this.searchResults.jumpToPage("next");
            },
            "goToPage": function (page) {
                this.trigger("list:pagination");
                this.searchResults.jumpToPage(page);
            },
            "refresh": function () {
                this.trigger('refresh');
            },
            "selectIssue": function (event) {
                this.trigger("list:select", {
                    id: event.id,
                    key: event.key,
                    absolutePosition: this.searchResults.getPositionOfIssueInSearchResults(event.id),
                    relativePosition: this.searchResults.getPositionOfIssueInPage(event.id)
                });
                this.searchResults.select(event.id);
            },
            "sort": function (jql) {
                this.trigger('sort', jql);
            },
            "update": function () {
                this.trigger("update");
            }
        });
        ServiceAPI.init(this);
    },

    load: function (searchResults, issueIdOrKey) {
        if (this.searchResults) {
            this.stopListening(this.searchResults);
            delete this.searchResults;
        }

        this.searchResults = searchResults;
        if (this.options.displayInlineIssueCreate) {
            this.inlineIssueCreate.setJQL(this.searchResults.jql);
        }

        this.listenTo(this.searchResults, {
            "before:loadpage": function () {
                this.trigger("before:loadpage");
            },
            "error:loadpage": function (errorInfo) {
                this.trigger("error:loadpage", errorInfo);
            },
            "reset": function () {
                this.list.update(this.searchResults);
            },
            "unselect": function (unselectedModel) {
                this.list.unselectIssue(unselectedModel.get("id"));
            },
            "select": function (selectedModel) {
                var modelId = selectedModel.get("id") || null;
                if (modelId) {
                    this.list.selectIssue(selectedModel.get("id"));
                }
                this.trigger("select", {
                    id: modelId,
                    key: selectedModel.get('key')
                });
            },
            "change": function (model) {
                this.list.updateIssue(model);
            }
        });

        if (issueIdOrKey) {
                // If we are looking for a specific key, jump to the page containing that key
            searchResults.jumpToPageForIssue(issueIdOrKey);
        } else {
                // If we are not looking for a specific key, just load the first page
            searchResults.jumpToPage("first");
        }
    },

    show: function (el) {
        this.list.render({
            el: el
        });
    },

    selectNext: function () {
        if (!this.searchResults) {
            return;
        }
        this.searchResults.selectNext();
    },

    selectPrevious: function () {
        if (!this.searchResults) {
            return;
        }
        this.searchResults.selectPrev();
    },

    selectIssue: function (issueId) {
        this.searchResults.select(issueId);
    },

    _getIssueById: function (issueId) {
        if (!this.searchResults) {
            return;
        }
        return this.searchResults.get(issueId);
    },

    refreshIssue: function (issueId) {
        var model = this._getIssueById(issueId);
        if (!model) return;

        model.fetch();
    },

    updateIssue: function (issueId, data) {
        var model = this._getIssueById(issueId);
        if (!model) {
            return;
        }
        model.set(data);
    },

    adjustSize: function () {
        this.list.adjustSize();
    },

    disableIssue: function (issueId) {
        var model = this._getIssueById(issueId);
        if (!model) {
            return;
        }
        model.set("inaccessible", true);
    },

    removeIssue: function (issueId) {
        var model = this._getIssueById(issueId);
        if (!model) {
            return new jQuery.Deferred().reject().promise();
        }

        return this.searchResults.removeAndUpdateSelectionIfNeeded(model);
    },

    onDestroy: function () {
        this.list.destroy();
    }
});

module.exports = SimpleAssetList;
