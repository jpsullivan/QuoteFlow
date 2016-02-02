"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var LineItemsCollection = require('./entities/line-items');
var LineItemsListController = require('./list/list');

/**
 * This controller is the main entry point for the quote sidebar component.
 * @extends {Marionette.Controller}
 */
var QuoteSidebarController = Marionette.Controller.extend({
    initialize: function(options) {
        //this._initializeDialogController();
        this.searchPageModule = options.searchPageModule;

        this._initializeCollections(options);
        this._initializeLineItemsController();
        //this._initializeFilterHeaderController();

        // QuoteFlow.application.on("assetEditor:loadComplete", function (model, props) {
        //     console.warning("hook asset events into sidebar");
        // }, this);
    },

    _initializeCollections: function(options) {
        this.lineItemsCollection = new LineItemsCollection(options.lineItems);
        this.listenTo(this.lineItemsCollection, {
            "change:favourite": function (filterModel, isFavourite) {
                if (!isFavourite) {
                    this.lineItemsCollection.remove(filterModel);
                }
            },
            "remove": function(filterModel) {
                this.trigger('filterRemoved', { filterId: filterModel.getId() });
            }
        });
    },

    _initializeLineItemsController: function() {
        this.lineItemsController = new LineItemsListController({
            collection: this.lineItemsCollection,
            className: "system-filters",
            errorMessage: "Failed to retrieve the line items from the server",
            loadingMessage: "Loading line items...",
            emptyMessage: "There are no line items."
            //loginMessage: "You must be {0}logged in{1} to view the line items", '<a class="login-link" href="#"', '</a>')
        });

        this.lineItemsController.on("selectFilter", function (filterModel) {
            this.headerController.closeDetails();
            this.trigger("filterSelected", filterModel.id);
        }, this);
    },

    _initializeFilterHeaderController: function() {
        this.headerController = new JIRA.Components.Filters.Controllers.Header();

        this.listenTo(this.headerController, "saveAs", function(filterModel) {
            this.showSaveAsDialog(filterModel);
        });

        this.listenTo(this.headerController, "save", function(filterModel) {
            var filterName = AJS.escapeHtml(filterModel.getName());

            filterModel.saveFilter(this.searchPageModule.getEffectiveJql())
                .done(_.bind(function() {
                    JIRA.Messages.showSuccessMsg(
                        AJS.I18n.getText('issuenav.filters.save.success.msg', filterName),
                        JIRA.Issues.getDefaultMessageOptions()
                    );

                    this.trigger("savedFilter", filterModel);
                },this))
                .fail(function() {
                    JIRA.Messages.showErrorMsg(
                        AJS.I18n.getText('issuenav.filters.save.error.msg', filterName),
                        JIRA.Issues.getDefaultMessageOptions()
                    );
                });
        });

        this.listenTo(this.headerController, "discard", function() {
            this.trigger("filterDiscarded");
        });

        this.listenTo(this.headerController, "favourite", function(filterModel) {
            filterModel = this._addFavouriteFilter(filterModel);
            this.highlightLineItem(filterModel);
            this.trigger("fitlerFavourited", filterModel);
        });
    },

    _addFavouriteFilter: function(filterModel) {
        var isFavourite = !!filterModel.getFavourite();
        var isInFavouriteCollection = !!this.favouriteFiltersCollection.get(filterModel.getId());

        if ( isFavourite && !isInFavouriteCollection) {
            this.favouriteFiltersCollection.add(filterModel);
        }

        return this.favouriteFiltersCollection.get(filterModel.getId());
    },

    getFilterById: function (filterId) {
        var filter = this.lineItemsCollection.get(filterId) || this.favouriteFiltersCollection.get(filterId);
        var deferred = jQuery.Deferred();

        if (filter) {
            deferred.resolve(filter);
        } else {
            var model = new JIRA.Components.Filters.Models.Filter({ id: filterId });
            model.fetch({
                success: _.bind(function() {
                    if (model.getFavourite()) {
                        this.favouriteFiltersCollection.add(model, {merge: true});

                        // We need to return the model in the collection, because 'add' will only merge the attributes,
                        // not the 'cid'. The 'cid' is used to identify the views associated with this model.
                        deferred.resolve(this.favouriteFiltersCollection.get(model.id));
                    } else if (model.getIsSystem()) {
                        this.lineItemsCollection.add(model, {merge: true});
                        deferred.resolve(this.lineItemsCollection.get(model.id));
                    } else {
                        deferred.resolve(model);
                    }
                },this),
                error: function() {
                    model.setIsValid(false);
                    deferred.reject.apply(this, arguments);
                }
            });
        }

        return deferred.promise();
    },

    showDeleteDialog: function(filterId) {
        this.getFilterById(filterId).done(_.bind(function(filterModel) {
            this.dialogController.showDeleteDialog(filterModel);
        }, this));
    },

    showRenameDialog: function(filterId) {
        this.getFilterById(filterId).done(_.bind(function(filterModel) {
            this.dialogController.showRenameDialog(filterModel);
        }, this));
    },

    showCopyDialog: function(filterId) {
        this.getFilterById(filterId).done(_.bind(function(filterModel) {
            this.dialogController.showCopyDialog(filterModel);
        }, this));
    },

    showSaveAsDialog: function(filterModel) {
        this.searchPageModule.getJqlDeferred()
            .done(_.bind(function(jql) {
                this.dialogController.showSaveAsDialog(filterModel, jql);
            },this));
    },

    showLineItems: function(el) {
        this.lineItemsController.show(el);
    },

    showFilterHeader: function(options) {
        this.headerController.show({
            el: options.el,
            model: options.model,
            isDirty: options.isDirty
        });
    },

    updateFilterHeader: function(options) {
        // this.headerController.update({
        //     model: options.model,
        //     isDirty: options.isDirty
        // });
    },

    highlightLineItem: function(filterModel) {
        this.lineItemsController.highlightLineItem(filterModel);
    },

    markFilterHeaderAsInvalid: function() {
        this.headerController.markAsInvalid();
    },

    fetchSystemFilters: function() {
        return this.lineItemsCollection.fetch();
    },
});

module.exports = QuoteSidebarController;
