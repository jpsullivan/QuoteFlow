"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var AssetFieldUtil = require('../../asset-viewer/legacy/asset-field-util');

var FieldModel = Brace.Model.extend({

    namedEvents: [
        /**
         * Triggered when editing is started. Lets the field view know to switch to use editHTML
         */
        "editingStarted",

        /**
         * Triggered when editing is desired but the field is already in edit mode.
         */
        "focusRequested",

        /**
         * Triggered when editing is cancelled. Lets the field view know to switch to use viewHTML
         */
        "editingCancelled",

        /**
         * Triggered when a save is required. This lets the JIRA.Components.IssueEditor.Controllers.EditIssue know to trigger a save
         */
        "save",
        /**
         * Triggered when an kind of save error has occurred (including server/validation errors).
         * This lets the field view know to hide loading indicator.
         */
        "saveError",

        /**
         * Triggered when saving started
         */
        "savingStarted",

        /**
         * Triggered when save has been completed. Successful or not. Lets the field view know to give indication to user.
         */
        "saveComplete",

        /**
         * Triggered when save has been successful. Lets the field view know to give indication to user.
         */
        "saveSuccess",
        /**
         * Triggered when we need to update the [params] property with the serialized value of the edit html
         */
        "updateRequired",
        /**
         * Triggered when there is a validation error on the server
         */
        "validationError",
        /**
         * Triggered when edit permissions are revoked
         */
        "modelDestroyed"
    ],

    namedAttributes: [
        /**
         * Field id
         * @type String
         */
        "id",

        /**
         * serialized input field parameters retrieved from server (ie editHtml).
         * @type object
         */
        "initParams",

        /**
         * Serialized input field parameters that the user has changed.
         * @type object
         */
        "params",

        /**
         * Html to displayed when editing this field
         * @type String
         */
        "editHtml",

        /**
         * Html to displayed when viewing this field
         * @type String
         */
        "viewHtml",

        /**
         * Is this field required
         * @type Boolean
         */
        "required",

        /**
         * The key the field type (complete key) for custom fields (for system fields this value is undefined).
         * The value is set upon initialisation of the view.
         * @type String
         */
        "fieldType",

        /**
         * True if the field should be displayed in edit mode
         * @type Boolean
         */
        "editing",

        /**
         * True if the field is currently being saved
         * @type Boolean
         */
        "saving",

        /**
         * Validation errors returned from server on attempted save
         * @type Object
         */
        "error",

        /**
         * Human readable localised name.
         * @type String
         */
        "label",

        /**
         * Current field content identifier token..
         * @type String
         */
        "contentId"
    ],

    /**
     * @constructor
     */
    initialize: function () {
        if (this.collection) {
            var instance = this;
            this.collection.on("reset", function () {
                instance.triggerModelDestroyed();
            });
        }
        this.on("change:viewHtml", function () {
            var $previous = this.previous("viewHtml");
            if ($previous) {
                // We may have view html with event handlers still attached. As we use detach instead of remove.
                // This can cause memory leaks unless we destroy properly
                jQuery.cleanData($previous);
            }
        });
        this.onModelDestroyed(function () {
            var $view = this.getViewHtml();
            if ($view) {
                // We may have view html with event handlers still attached. As we use detach instead of remove.
                // This can cause memory leaks unless we destroy properly
                jQuery.cleanData($view);
            }
        });
    },

    /**
     * Handling for when the field is trying to be programmatically switched out of edit mode.
     *
     * @see JIRA.Components.IssueEditor.Collections.Fields._handleEditingStarted to see how this works
     */
    blurEdit: function () {
        if (this.getEditing()) {
            if (this.isDirty() || this.hasValidationError()) {
                this.save();
            } else {
                this.cancelEdit();
            }
        }
    },

    /**
     * Sets validation error
     * @param {String} editHtml - The new edit html for the field
     * @param {String}  error - Error Message
     */
    setValidationError: function (errorHtml, error, focus) {
        this.setSaving(false);
        this.setError(error);
        // other fields shouldn't care about this field showing a validation error
        this.edit({ignoreBlur: true});
        this.triggerSaveError();
        this.triggerValidationError(errorHtml, focus);
    },

    /**
     * Lets everyone know we have an error
     */
    handleSaveError: function () {
        this.setSaving(false);
        this.triggerSaveError();
    },

    /**
     * Cancels edit. Will switch view out of edit mode
     */
    cancelEdit: function (reason) {
        if (!this.cancelLocked) {
            var event = new AJS.$.Event(JIRA.Events.BEFORE_INLINE_EDIT_CANCEL);
            QuoteFlow.application.trigger(event, [this.getId(), this.getFieldType(), reason]);
            if (!event.isDefaultPrevented()) {
                this.cancelLocked = true;
                if (this.getEditing() || this.getSaving()) {
                    this.setEditing(false);
                    this.setSaving(false);
                    this.unset("params");
                    this.unset("initParams");
                    this.triggerEditingCancelled();
                    this.unset("viewHtml");
                }
                delete this.cancelLocked;
            }
        }
    },

    /**
     * Starts editing. Will switch view into edit mode.
     * @param {Object} props
     * ... {Boolean} ignoreBlur - Usually we trigger a save or cancel on the an other fields in edit mode when we
     * start editing another field. This flag prevents that. This is useful to avoid focus theft by an in-flight error.
     */
    edit: function (props) {
        if (!this.getEditing()) {
            this.setEditing(true);
            this.triggerEditingStarted(this, props);
        } else if (!props || !props.ignoreBlur) {
            this.triggerFocusRequested(this, props);
        }
    },

    /**
     * Gets the current serialized value of the field(s)
     * @return {Object}
     */
    getCurrentParams: function () {
        this.triggerUpdateRequired();
        var params = {};
        // TODO: only return this.getParams() when we can change quick edit endpoint to not require *all* values
        if (this.getParams()) {
            params = this.getParams();
        }
        else if (this.getInitParams()) {
            params = this.getInitParams();
        }
        else {
            var $el = AJS.$('<div />').html(this.getEditHtml());
            this.setInitParams(this._serializeObject($el));
            params = this.getInitParams();
        }
        var fp = {};
        _.each(params, function (value, key) {
            fp[key] = value;
        });
        return fp;
    },

    /**
     * Looks to see if the edit html actually has a field the user can edit. For example if we have not components
     * configured for a project there will be no field to edit so the field is not editable.
     *
     * @return {Boolean}
     */
    isEditable: function () {
        if (this.getEditHtml()) {
            // TODO: JRADEV-9709: replace with flag that comes back from quick edit
            return AJS.$(this.getEditHtml()).find("textarea, :text,:radio,:checkbox, select").length !== 0;
        }
        return false;
    },

    /**
     * Do we have a validation error
     * @return {Boolean}
     */
    hasValidationError: function () {
        return Boolean(this.getError());
    },

    /**
     * Popluates element with editHTML. Stores the $el's html as the view html, so that if we cancelEdit we can
     * restore it.
     *
     * @param {jQuery} $el
     */
    switchElToEdit: function ($el) {
        if (!this.getViewHtml()) {
            this.setViewHtml($el.contents());
        }

        $el.contents().detach();

        $el.html(JST["asset-viewer/fields/field"]({
            asset: this.toJSON(),
            accessKey: AssetFieldUtil.getAccessKeyModifier()
        }));
        if (!this.getInitParams()) {
            this.setInitParams(this._serializeObject($el));
        }
        this.setEditing(true);
    },

    /**
     * Serializes fields of el into params property
     * @param {jQuery} $el
     */
    update: function ($el) {
        if (this.getEditing()) {
            this.setParams(this._serializeObject($el));
        }
    },

    handleSaveSuccess: function () {
        this.setSaving(false);
        this.setEditing(false);
        this.unset("viewHtml");
        this.unset("initParams");
        this.setError(null);
        this.triggerSaveSuccess();
    },

    handleSaveStarted: function () {
        this.setSaving(true);
        this.triggerSavingStarted();
    },

    /**
     * Initiates a save. This lets the JIRA.Components.IssueEditor.Controllers.EditIssue know to trigger a save.
     */
    save: function () {
        if (!this.getSaving()) {
            // Get the view to update model with serialized form field
            this.triggerUpdateRequired();
            this.triggerSave(this);
        }
    },

    /**
     * Has the field been modified
     * @return {Boolean}
     */
    isDirty: function () {
        this.triggerUpdateRequired();
        // Get the view to update model with serialized form field
        return this._isDirty(this.getCurrentParams());
    },

    /**
     * Has the field been modified
     * @return {Boolean}
     */
    _isDirty: function (params) {
        return JSON.stringify(params) !== JSON.stringify(this.getInitParams());
    },

    matchesFieldSelector: function () {
        return AssetFieldUtil.matchesFieldSelector(this.id);
    },

    /**
     * Serialize the inputs in an element to an object.
     *
     * The object's keys are the inputs' names, its values their values.
     *
     * @param {element} element A DOM element.
     * @private
     * @returns {Object} An object representation of `element`'s inputs.
     */
    _serializeObject: function (element) {
        var data = {};

        var dataArray = AJS.$(element).find(":input").serializeArray();

        jQuery.each(dataArray, function () {
            if (data[this.name]) {
                if (!data[this.name].push) {
                    data[this.name] = [data[this.name]];
                }
                data[this.name].push(this.value || '');
            } else {
                data[this.name] = this.value || '';
            }
        });

        return data;
    }
}, {
    /**
     * @param {FieldModel} fieldModel The FieldModel to test.
     * @return A boolean indicating whether fieldModel is editable.
     */
    IS_EDITABLE: function (fieldModel) {
        return fieldModel.isEditable();
    }
});

module.exports = FieldModel;
