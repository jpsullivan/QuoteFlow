"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

/**
 * Controls edits for the currently viewed asset.
 * Note: the actual saving of edits is don by the SaveInProgressManager.
 * @extends Brace.Model
 */
var EditAssetController = Brace.Model.extend({
    namedAttributes: [
        /**
        * @type number
        */
        "assetId",

        /**
        * Issue Key
        * @type string
        */
        "assetSku",

        /**
        * jQuery element that contains the view asset html
        * @type jQuery
        */
        "assetViewContext",

        /**
        * Collection of JIRA.Components.IssueEditor.Models.Field
        * @type JIRA.Components.IssueEditor.Collections.Fields
        */
        "fields",

        /**
        * @type JIRA.Components.IssueViewer.Legacy.IssueEventBus
        */
        "assetEventBus"
    ],

    namedEvents: [
        /**
        * @event save
        * Fired when the fields are ready to be saved. The actual save action is performed by other objects
        * listening to this event (SaveInProgressManager).
        *
        * @param {number} issueId The ID of the issue being saved
        * @param {string} issueKey The Key of the issue being saved
        * @param {string[]} toSaveIds List of fields ids being saved
        * @param {object} params
        * @param {object} ajaxProperties
        */
        "save",

        /**
        * @event editField
        * Fired when the user is editing a field
        * @param {object} options
        * @param {string} options.fieldId Id of the field being saved
        */
        "editField"
    ],

    /**
    * @constructor
    */
    initialize: function() {
        _.bindAll(this,
            "_handleSaveError",
            "_handleSaveSuccess",
            "_handleSavingStarted",
            "cancelUneditedFields",
            "createFieldView",
            "handleFieldUpdate",
            "save");

            this.set({
                fields: new JIRA.Components.IssueEditor.Collections.Fields()
            }, {silent: true});

            this.getFields()
            .bind("add", this.createFieldView)
            .bind("updated", this.handleFieldUpdate)
            .bind("save", this.save);

            this.getIssueEventBus().onSavingStarted(this._handleSavingStarted);
            this.getIssueEventBus().onSaveSuccess(this._handleSaveSuccess);
            this.getIssueEventBus().onSaveError(this._handleSaveError);
            this.getIssueEventBus().onSave(this.save);
            this.getIssueEventBus().onSave(this.cancelUneditedFields);
        },

        _saveById: function(id) {
            var model = this.getFields().get(id);
            if (model) {
                model.blurEdit();
            }
        },

        /**
        * Handles case where the JIRA.Components.IssueEditor.Services.SaveInProgressManager returns server/validation errors for issue.
        *
        * @param {Number} issueId
        * @param {Array} attemptedSavedIds
        * @param {Object} response
        * ... {Array} errorMessages
        * ... {Object} errors - Validation errors
        */
        _handleSaveError: function(issueId, attemptedSavedIds, response) {
            var instance = this;
            if (response) {
                this.applyErrors(response);
            } else {
                _.each(attemptedSavedIds, function(id) {
                    var model = instance.getFields().get(id);
                    if (model) {
                        model.handleSaveError();
                    }
                });
            }
        },

        /**
        * Lets all the models know that saving has started
        *
        * @param savingIds
        * @private
        */
        _handleSavingStarted: function(savingIds) {
            this.getFields().each(function(model) {
                if (_.include(savingIds, model.id)) {
                    model.handleSaveStarted();
                }
            });
        },

        /*
        * Handles the situation where a field becomes visible but doesn't have a
        * view associated with it, meaning it's not possible to inline-edit it.
        *
        * @param {Object} fieldModel The field model that was updated.
        */
        handleFieldUpdate: function(fieldModel) {
            // If a view has been created for the field, its trigger element (or
            // one of its descendants) will have the "editable-field" class.
            var trigger = jQuery(JIRA.Components.IssueViewer.Legacy.IssueFieldUtil.getFieldSelector(fieldModel.id));
            if (!trigger.hasClass("editable-field")) {
                this.createFieldView(fieldModel);
            }
        },

        /**
        * Applies an error collection to the current issue page. Useful when restoring an issues state after navigating away.
        *
        * @param errorCollection
        */
        applyErrors: function(lastEditData, focusFirst) {
            var errorCollection = lastEditData.errorCollection;
            if (errorCollection && errorCollection.errors) {
                this.getFields().each(function(model) {
                    if (errorCollection.errors[model.id]) {
                        var updatedField = _.find(lastEditData.fields, function(field) {
                            return field.id === model.id;
                        });
                        if (updatedField) {
                            model.setValidationError(updatedField.editHtml, errorCollection.errors[model.id], focusFirst);
                            focusFirst = false;
                        }
                    }
                });
            }

            // In the case of error messages we pin up a global error message
            if (errorCollection.errorMessages && errorCollection.errorMessages.length) {
                var html = JIRA.Templates.IssueEditor.Fields.saveErrorMessage({
                    errors: errorCollection.errorMessages,
                    issueKey: this.getIssueKey()
                });
                JIRA.Messages.showErrorMsg(html, {
                    closeable: true
                });
            }
        },

        /**
        * Removes all field models and edital views
        */
        reset: function() {
            this.getFields().reset();
        },

        /**
        * Cancels any edit is progress
        */
        cancelEdit: function() {
            this.getFields().each(function(model) {
                model.cancelEdit();
            });
        },

        /**
        * Handles case where the JIRA.Components.IssueEditor.Services.SaveInProgressManager saves successfully for issue
        *
        * @param {Number} issueId
        * @param {Array} savedFieldIds - Ids for successfully saved fields
        */
        _handleSaveSuccess: function(issueId, issueKey, savedFieldIds) {
            var savedFieldModels = this.getFields().filter(function(fieldModel) {
                return _.indexOf(savedFieldIds, fieldModel.id) >= 0;
            });
            _.each(savedFieldModels, function(model) {
                model.handleSaveSuccess();
            });
        },

        /**
        * Gets the ids of fields in edit mode that need to be saved
        *
        * @return Array<String>
        */
        getDirtyEditsInProgress: function() {
            return _.pluck(this.getFields().filter(function(model) {
                return model.getEditing() && model.isDirty();
            }), "id");
        },

        /**
        * Gets the ids of fields in edit mode
        *
        * @return Array<String>
        */
        getEditsInProgress: function() {
            return _.pluck(this.getFields().filter(function(model) {
                return model.getEditing();
            }), "id");
        },

        /**
        * Saves all the fields that are currently in edit mode with dirty (changed) values.
        * Note: The actual save is delegated to the JIRA.Components.IssueEditor.Services.SaveInProgressManager
        *
        * @param model
        * @param ajaxProperties
        */
        save: function(model, ajaxProperties) {

            var params = {}, toSaveIds = [];

            var toSave = [model];
            if (!model) {
                toSave = this.getFields().filter(function(model) {
                    return !model.getSaving() && model.getEditing() && model.isDirty();
                });
            } else if (!model.getEditing() || model.getSaving()) {
                return;
            }

            _.each(toSave, function(model) {
                toSaveIds.push(model.getId());
                _.extend(params, model.getCurrentParams());
            });

            if (toSaveIds.length > 0) {
                this.triggerSave(this.getIssueId(), this.getIssueKey(), toSaveIds, params, ajaxProperties);
            }
        },

        /**
        * Cancels any fields which are not dirty (have not been edited) and have no validation errors.
        */
        cancelUneditedFields: function() {
            this.getFields().each(function(model) {
                if (model.getEditing() && !model.isDirty() && !model.hasValidationError()) {
                    model.cancelEdit();
                }
            });
        },

        /**
        * Updates the data of this controller
        *
        * @param {Object}   data
        * @param {number}   [data.issueId]
        * @param {string}   [data.issueKey]
        * @param {Object[]} [data.fields]
        */
        update: function(data, props) {
            if (data.fields) {
                if (props && props.fieldsInProgress) {
                    _.each(data.fields, function(fieldData) {
                        if (_.contains(props.fieldsInProgress, fieldData.id)) {
                            fieldData.editing = true;
                        }
                    });
                }
                this.getFields().update(data.fields, props);
            }

            if (data.issueId) {
                this.setIssueId(data.issueId);
            }

            if (data.issueKey) {
                this.setIssueKey(data.issueKey);
            }
        },

        /**
        * Creates fields view
        *
        * @param {JIRA.Components.IssueEditor.Models.Field} fieldModel
        */
        createFieldView: function(fieldModel) {
            var editableFieldTrigger = jQuery(JIRA.Components.IssueViewer.Legacy.IssueFieldUtil.getFieldSelector(fieldModel.id), this.getIssueViewContext());
            if (editableFieldTrigger.length === 1) {
                var field = new JIRA.Components.IssueEditor.Views.FieldView({
                    model: fieldModel,
                    el: editableFieldTrigger,
                    issueEventBus: this.getIssueEventBus()
                });

                field.on("editField", _.bind(function(parameters) {
                    this.trigger("editField", parameters);
                }, this));
            }
        }
    });
});m,
