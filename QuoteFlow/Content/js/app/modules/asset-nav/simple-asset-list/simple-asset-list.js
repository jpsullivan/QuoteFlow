"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var AssetsApi = require('./services/api');
var ControllerList = require('./controllers/list');

var SimpleAssetList = Marionette.Object.extend({
    initialize: function () {
        if (this.options.displayInlineAssetCreate) {
            this.inlineAssetCreate = new InlineAssetCreate();
            this.listenTo(this.inlineAssetCreate, {
                "assetCreated": function (assetInfo) {
                    this.trigger("assetCreated", assetInfo);
                },
                "activated deactivated": function () {
                        // We need to defer this because the Inline Asset Create component
                        // trigger those events *before* it is actually activated or deactivated
                    _.defer(_.bind(function () {
                        this.list.adjustSize();
                    }, this));
                }
            });
        }

        this.list = new ControllerList({
            baseURL: this.options.baseURL,
            inlineAssetCreate: this.inlineAssetCreate
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
            "selectAsset": function (event) {
                this.trigger("list:select", {
                    id: event.id,
                    sku: event.sku,
                    absolutePosition: this.searchResults.getPositionOfAssetInSearchResults(event.id),
                    relativePosition: this.searchResults.getPositionOfAssetInPage(event.id)
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
        AssetsApi.init(this);
    },

    load: function (searchResults, assetIdOrSku) {
        if (this.searchResults) {
            this.stopListening(this.searchResults);
            delete this.searchResults;
        }

        this.searchResults = searchResults;
        if (this.options.displayinlineAssetCreate) {
            this.inlineAssetCreate.setJQL(this.searchResults.jql);
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
                this.list.unselectAsset(unselectedModel.get("id"));
            },
            "select": function (selectedModel) {
                var modelId = selectedModel.get("id") || null;
                if (modelId) {
                    this.list.selectAsset(selectedModel.get("id"));
                }
                this.trigger("select", {
                    id: modelId,
                    sku: selectedModel.get('sku')
                });
            },
            "change": function (model) {
                this.list.updateAsset(model);
            }
        });

        if (assetIdOrSku) {
            // If we are looking for a specific sku, jump to the page containing that sku
            searchResults.jumpToPageForAsset(assetIdOrSku);
        } else {
            // If we are not looking for a specific sku, just load the first page
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

    selectAsset: function (assetId) {
        this.searchResults.select(assetId);
    },

    _getAssetById: function (assetId) {
        if (!this.searchResults) {
            return;
        }
        return this.searchResults.get(assetId);
    },

    refreshAsset: function (assetId) {
        var model = this._getAssetById(assetId);
        if (!model) {
            return;
        }

        model.fetch();
    },

    updateAsset: function (assetId, data) {
        var model = this._getAssetById(assetId);
        if (!model) {
            return;
        }
        model.set(data);
    },

    adjustSize: function () {
        this.list.adjustSize();
    },

    disableAsset: function (assetId) {
        var model = this._getAssetById(assetId);
        if (!model) {
            return;
        }
        model.set("inaccessible", true);
    },

    removeAsset: function (assetId) {
        var model = this._getAssetById(assetId);
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
