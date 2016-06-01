"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var FieldModel = require('./field-model');

var FieldsCollection = Brace.Collection.extend({
    model: FieldModel,

    initialize: function () {
        this.on("editingStarted", this._handleEditingStarted, this);
    },

    _handleEditingStarted: function (editModel, props) {
        props = props || {};
        if (props.ignoreBlur) {
            return;
        }
        // Let any field in edit mode know we want to edit another
        this.each(function (model) {
            if (editModel !== model && model.getEditing()) {
                model.blurEdit();
            }
        });
    },

    update: function (data, props) {
        var instance = this;

        if (props && props.changed) {
            var changedData = props.changed;
            var updatedFields = changedData.updated ? changedData.updated.fields : [];
            var addedFields = changedData.added ? changedData.added.fields : [];

            _.each(data, function (modelData) {
                if (_.contains(updatedFields, modelData.id)) {
                    var existingModel = instance.get(modelData.id);

                    if (existingModel) {
                        // JRADEV-11518 Don't update the model's editHtml if it's an error,
                        // since it will override the 'real' editHtml
                        if (existingModel.hasValidationError()) {
                            delete modelData.editHtml;
                        }
                        existingModel.set(modelData, {silent: true});
                        instance.trigger("updated", existingModel);
                    } else {
                        instance.add(modelData);
                    }
                } else if (_.contains(addedFields, modelData.id)) {
                    instance.add(modelData);
                }
            });
        } else {
            _.each(data, function (modelData) {
                var existingModel = instance.get(modelData.id);
                if (existingModel) {
                    // JRADEV-11518 Don't update the model's editHtml if it's an error,
                    // since it will override the 'real' editHtml
                    if (existingModel.hasValidationError()) {
                        delete modelData.editHtml;
                    }
                    existingModel.set(modelData, {silent: true});
                    instance.trigger("updated", existingModel);
                } else {
                    instance.add(modelData);
                }
            });
        }
    },

    cancelEdit: function () {
        this.each(function (model) {
            model.cancelEdit();
        });
    },

    isDirty: function () {
        return this.any(function (item) {
            return item.isDirty();
        });
    },
    getDirtyFields: function () {
        return this.filter(function (item) {
            return item.isDirty();
        });
    }
});

module.exports = FieldsCollection;
