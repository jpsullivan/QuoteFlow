"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');
var MarionetteViewManager = require('../../../mixins/marionette-viewmanager');

var AssetEditorController = require('../search/asset-editor/asset-editor');
var EmptyView = require('./views/empty');
var Pager = require('../../pager/pager');
var SimpleAssetList = require('../simple-asset-list/simple-asset-list');
var ViewContainer = require('./views/container');
var ViewLayout = require('./views/layout');
var ViewLoading = require('./views/loading');


/**
 * Utility method to create a debounced version of a function. The generated function will have
 * the properties:
 *
 *   - Initial call will happen after <initial> ms
 *   - After the initial call, it will postpone its execution until after <cooldown> milliseconds have elapsed
 *     since the last time it was invoked.
 *   - After the <cooldown> period, the function will be delayed <initial> ms again.
 *
 * Example timelines with values initial=100 and cooldown=500
 *   - Calling the function once:
 *          0ms - Function called
 *        100ms - Function executed
 *
 *   - Calling the function 4 times every 50ms, then again after 1s
 *          0ms - Function called
 *         50ms - Function called
 *        100ms - Function executed (1)
 *        100ms - Function called
 *        150ms - Function called
 *        650ms - Function executed (2)
 *       1000ms - Function called
 *       1100ms - Function executed (3)
 *
 * @param {Function} fn Function to execute
 * @param {number} initial Initial delay in ms.
 * @param {number} cooldown Subsequent delays, in ms.
 * @return {Function} The debounced function
 */
var variableDebounce = function (fn, initial, cooldown) {
    var timer;
    var currentTimer = initial;

    return function () {
        var args = arguments;
        clearTimeout(timer);
        timer = setTimeout(function () {
            fn.apply(this, args);
            currentTimer = initial;
        }, currentTimer);
        currentTimer = cooldown;
    };
};

var DetailsLayout = MarionetteViewManager.extend({
    _buildIssueEditor: function () {
        var editor = this.assetEditor = new AssetEditorController();
        var options = this.options;

        this._loadAssetInEditor = variableDebounce(_.bind(function (issueData) {
            editor.loadAsset({
                id: issueData.id,
                sku: issueData.sku,
                detailView: !options.shouldUpdateCurrentProject
            }).always(_.bind(function () {
                this.hideLoading();
            }, this));
        }, this), 100, 500);

        this.listenTo(editor, {
            "loadError": function (issueData) {
                this.simpleIssueList.disableIssue(issueData.issueId);
                this.trigger("editorError");
            },
            "loadComplete": function (issue, options) {
                this.adjustSize();

                // The editor model does not provide info about the issue type, we can't
                // update that value on the list.
                this.simpleIssueList.updateIssue(issue.id, {
                    sku: issue.get('entity').sku,
                    summary: issue.get('entity').summary,
                    status: issue.get('entity').status.name
                });

                var eventPayload = {
                    assetSku: issue.get('entity').sku,
                    assetId: issue.id,
                    issueEditorOptions: options
                };
                if (options && options.loadReason === "issues-cache-refresh") {
                    this.trigger("editorLoadedFromCache", eventPayload);
                } else {
                    this.trigger("editorLoaded", eventPayload);
                }
            },
            "saveSuccess": function (event) {
                this.simpleIssueList.refreshIssue(event.issueId);
                this.trigger("editor:saveSuccess", {
                    event: event.issueId,
                    savedFieldIds: event.savedFieldIds,
                    duration: event.duration
                });
            },
            "linkToIssue": function (event) {
                this.trigger("linkToIssue", event);
            },
            "refineViewer": function (event) {
                event.preventDefault();
                this.updateEditor(event.query);
            },
            "linkInErrorMessage": function (event) {
                event.preventDefault();
                this.simpleIssueList.selectIssue(event.issueData.id);
            }
        });
    },

    _buildSimpleIssueList: function () {
        this.simpleIssueList = new SimpleAssetList({
            baseURL: this.baseURL,
            displayInlineIssueCreate: this.options.displayInlineIssueCreate
        });
        this.listenTo(this.simpleIssueList, {
            "select": function (assetData) {
                this.trigger('select', assetData);
            },
            "update": function () {
                this.trigger("list:update");
            },
            "refresh": function () {
                this.trigger("list:refresh");
            },
            "sort": function (jql) {
                this.trigger("list:sort", jql);
            },
            "list:select": function (event) {
                this.trigger("list:select", {
                    id: event.id,
                    sku: event.sku,
                    absolutePosition: event.absolutePosition,
                    relativePosition: event.relativePosition
                });
            },
            "list:pagination": function () {
                this.trigger("list:pagination");
            },
            "before:loadpage": function () {
                this.showLoading();
            },
            "error:loadpage": function (errorInfo) {
                this.hideLoading();
                this.trigger("error:loadpage", errorInfo);
            },
            "assetCreated": function (issueInfo) {
                this.trigger("assetCreated", issueInfo);
            }
        });
    },

    _buildPager: function () {
        this.pager = new Pager();
        this.listenTo(this.pager, {
            "next": function () {
                this.selectNext();
            },
            "previous": function () {
                this.selectPrevious();
            }
        });
    },

    _buildContainerView: function () {
        this.containerView = this.buildView("containerView", function () {
            var view = new ViewContainer();
            this.listenTo(view, {
                "render": function () {
                    this.getView("layoutView").render();
                    this.getView("emptyView").render();
                    this.getView("loadingView").render();
                    view.$el.prepend(this.getView("layoutView").$el);
                }
            });
            return view;
        });
    },

    _buildEmptyView: function () {
        this.emptyView = this.buildView("emptyView", function () {
            var view;
            if (typeof this.options.emptyViewFactory === "function") {
                view = this.options.emptyViewFactory();
            } else {
                view = new EmptyView();
            }

            this.listenTo(view, {
                "render": function () {
                    this.containerView.$el.append(view.$el);
                    this.adjustSize();
                    this.trigger("empty");
                }
            });
            return view;
        });
    },

    _showEmptyView: function () {
        this.layoutView.$el.detach();
        this.containerView.$el.empty();
        this.containerView.showView(this.emptyView);
        this.adjustSize();
        this.trigger("empty");
    },

    _buildLayoutView: function () {
        this.layoutView = this.buildView("layoutView", function () {
            var view = new ViewLayout();
            this.listenTo(view, {
                "render": function () {
                    view.assetEditor._ensureElement();
                    this.assetEditor.setContainer(view.assetEditor.$el);

                    view.issuesList._ensureElement();
                    this.simpleIssueList.show(view.issuesList.$el);

                    view.pager._ensureElement();
                    this.pager.show(view.pager.$el);
                },
                "resize": function () {
                    this.adjustSize();
                }
            });
            return view;
        });
    },

    _buildLoadingView: function () {
        this.loadingView = this.buildView("loadingView", function () {
            return new ViewLoading();
        });
    },

    _buildComponents: function () {
        this._buildIssueEditor();
        this._buildSimpleIssueList();
        this._buildPager();
    },

    _buildViews: function () {
        this._buildLayoutView();
        this._buildEmptyView();
        this._buildLoadingView();
        this._buildContainerView();

        this.containerView.render();
    },

    initialize: function (options) {
        this.baseURL = options.baseURL;

        this._buildComponents();
        this._buildViews();
    },

    _showLoadingView: function () {
        this.containerView.showLoading(this.loadingView);
    },

    _hideLoadingView: function () {
        this.containerView.hideLoading(this.loadingView);
    },

    _showLayoutView: function () {
        this.emptyView.$el.detach();
        this.containerView.showView(this.layoutView);
        this.layoutView.showDraggable();
        this.adjustSize();
    },

    show: function (el) {
        el.replaceWith(this.containerView.$el);
        return this.containerView.$el;
    },

    onDestroy: function () {
        Marionette.ViewManager.prototype.onDestroy.call(this);
        this.assetEditor.close();
        this.simpleIssueList.destroy();
        this.hideView("layoutView");
        this.hideView("loadingView");
        this.hideView("emptyView");
        this.hideView("containerView");
    },

    showLoading: function () {
        this._showLoadingView();
    },

    hideLoading: function () {
        this._hideLoadingView();
    },

    load: function (searchResults, issueIdOrKey) {
        if (this.searchResults) {
            this.stopListening(this.searchResults);
            delete this.searchResults;
        }
        this.searchResults = searchResults;
        this.listenTo(this.searchResults, {
            "issueDeleted": function () {
                if (!this.searchResults.length) {
                    this._showEmptyView();
                }
            },
            "select selectIssueNotInList": function (issueModel) {
                this.showLoading();
                this._loadAssetInEditor({
                    id: issueModel.id,
                    sku: issueModel.get('sku')
                });
            }
        });

        if (this.searchResults.isEmptySearch()) {
            this._showEmptyView();
        } else {
            this.simpleIssueList.load(this.searchResults, issueIdOrKey);
            this.pager.load(this.searchResults);
            this._showLayoutView();
        }

        this.trigger("list:render");
    },

    adjustSize: function () {
        _.defer(_.bind(function () {
            if (this.getView("layoutView")) {
                this.simpleIssueList.adjustSize();
                this.getView("layoutView").maximizeDetailPanelHeight();
                this.assetEditor.applyResponsiveDesign();
                this.getView("layoutView").updateDraggable();
            }

            if (this.getView("emptyView")) {
                var emptyViewContainer = this.getView("emptyView").$el;
                var issueContainerTop = emptyViewContainer.length && emptyViewContainer.offset().top;
                emptyViewContainer.css("height", window.innerHeight - issueContainerTop);
            }
        }, this));
    },

    refreshIssue: function (issueId) {
        this.simpleIssueList.refreshIssue(issueId);
        return this.assetEditor.refreshIssue();
    },

    removeIssue: function (issueId) {
        this.showLoading();
        return this.simpleIssueList.removeIssue(issueId)
                .always(_.bind(function () {
                    this.hideLoading();
                }, this))
                .done(_.bind(function (listLength) {
                    if (listLength === 0) {
                        this._showEmptyView();
                    }
                }, this));
    },

    getActiveIssueId: function () {
        return this.assetEditor.getIssueId();
    },

    getActiveIssueKey: function () {
        return this.assetEditor.getIssueKey();
    },

    selectNext: function () {
        return this.simpleIssueList.selectNext();
    },

    selectPrevious: function () {
        return this.simpleIssueList.selectPrevious();
    },

    updateEditor: function (params) {
        this.showLoading();
        this.listenToOnce(this.assetEditor, "loadComplete", function () {
            this.hideLoading();
        });
        this.assetEditor.updateIssueWithQuery(params);
    },

    isLoading: function () {
        return this.assetEditor.isCurrentlyLoading();
    },

    hasSavesInProgress: function () {
        return this.assetEditor.hasSavesInProgress();
    },

    canDismissComment: function () {
        return this.assetEditor.canDismissComment();
    },

    getEditorFields: function () {
        return this.assetEditor.getFields();
    },

    abortPending: function () {
        return this.assetEditor.abortPending();
    },

    beforeHide: function () {
        return this.assetEditor.beforeHide();
    },

    beforeShow: function () {
        return this.assetEditor.beforeShow();
    },

    removeIssueMetadata: function () {
        return this.assetEditor.removeIssueMetadata();
    },

    editField: function (field) {
        return this.assetEditor.editField(field);
    }
});

module.exports = DetailsLayout;
