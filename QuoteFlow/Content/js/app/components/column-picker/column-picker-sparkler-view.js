"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

/**
 * This object wraps a Sparkler. It is used by the ColumnPicker to render sparklers for select columns
 * (it will render one sparkler per column layout).
 *
 * @constructs
 * @param {Object} config Configuration object for this object
 * @param {jQuery} config.el Element where the sparkler should be rendered
 * @param {ColumnConfigModel} config.model Model used for this view
 * @param {number} [config.maxResults=25] Max number of items to display
 */
var ColumnPickerSparklerView = Brace.View.extend({
    initialize: function (config) {
        //Don't use || to get the default value, it will fail with 0
        this._maxResults = (typeof config.maxResults == "number") ? config.maxResults : this.MAX_DISPLAYED_ITEMS;
        this._autoUpdate = !!config.autoUpdate;
        this.$el.scrollLock('.aui-list-scroll');
        if (!this._autoUpdate) {
            this.model.on("change:columns", _.bind(this._setSelectedColumns, this));
        }
    },

    /**
     * Default for max displayed columns
     * @type {Number}
     * @default
     * @private
     * @constant
     */
    MAX_DISPLAYED_ITEMS: 25,

    /**
     * Stores the actual AJS.CheckboxMultiSelect object that represents the sparkler
     * @type {AJS.CheckboxMultiSelect}
     * @private
     */
    _sparkler: null,

    activate: function () {
        this.model.setIsActive(true);
    },

    deactivate: function () {
        this.model.setIsActive(false);
    },

    /**
     * Hides the sparkler
     */
    hide: function () {
        this.model.revertUnsavedColumns({ silent: true });

        this.$el.addClass("hidden");

        //Clear the sparkler search field
        var input = this.$el.find("input[id *= column-sparkler-input]");
        input.val('');
    },

    /**
     * Shows the sparkler
     */
    show: function () {
        this.$el.removeClass("hidden");

        //Set the selected columns
        this._setSelectedColumns();

        //Focus the sparkler input
        var input = this.$el.find("input[id *= column-sparkler-input]");
        if (!input.is(":disabled")) {
            input.focus();
        }
    },

    addMessage: function (message) {
        if (this.$el.find(".aui-message." + message.type).length === 0) {
            AJS.messages[message.type](this.$el, {
                body: message.content,
                closeable: false,
                insert: "prepend",
                id: message.id
            });
        }
    },

    /**
     * Disables the sparkler whenever the edit is disabled in the model
     *
     * @private
     */
    _disableEditIfNeeded: function () {
        if (this.model.isEditDisabled()) {
            this._disableEdit();
        } else {
            this._enableEdit();
        }
    },

    /**
     * Shows the EditDisabled message
     *
     * @private
     */
    _showEditDisabledMessage: function () {
        this.addMessage({
            content: AJS.I18n.getText("issues.components.column.config.filter.disabled"),
            type: "warning",
            id: this.model.getName() + '-edit-disabled-message'
        });
    },

    /**
     * Destroys the EditDisabled message
     *
     * @private
     */
    _hideEditDisabledMessage: function () {
        this.$el.find("#" + this.model.getName() + '-edit-disabled-message').remove();
    },

    /**
     * Disables the sparkler interactions.
     *
     * This method is used when the user does not have permission to change the columns.
     *
     * @private
     */
    _disableEdit: function () {
        //Disable the search input
        this._sparkler.disable();

        //Disable the checkboxes
        this.$el.find("input[type='checkbox']").attr("disabled", "disabled");  //Disable checkboxes
        this.$el.find(".check-list-item").addClass("disabled");                //Disable labels
        this.$el.find(".no-suggestions").addClass("disabled");                 //Disable "more" message

        //Disable the action bar
        this.$el.find(".restore-defaults").attr("aria-disabled", "true");

        //Show reason to user
        this._showEditDisabledMessage();
    },

    /**
     * Enables the sparkler interactions.
     *
     * This method is used when the user have permission to change the columns.
     *
     * @private
     */
    _enableEdit: function () {
        //Enable the search input
        this._sparkler.enable();

        //Enable the checkboxes
        this.$el.find("input[type='checkbox']").removeAttr("disabled");  //Enable checkboxes
        this.$el.find(".check-list-item").removeClass("disabled");       //Enable labels
        this.$el.find(".no-suggestions").removeClass("disabled");        //Enable "more" message

        //Enable the action bar
        this.$el.find(".restore-defaults").removeAttr("aria-disabled");

        //Remove editDisabled message
        this._hideEditDisabledMessage();
    },

    /**
     * Creates the internal sparkler
     *
     * @param {Array.<{label: string, value: string}>} items Items to include in the sparkler
     */
    createSparklerControl: function (items) {
        //Make sure we destroy the actual sparkler. 
        //This should not happen unless some event is being fired twice
        if (this._sparkler) {
            this._sparkler.remove();
        }

        var selectElement = this._buildQueryableSelect(items);
        this.$el.append(selectElement);

        //Create the sparkler control
        this._sparkler = new AJS.CheckboxMultiSelect({
            element: selectElement,
            maxInlineResultsDisplayed: this._maxResults,
            //suggestionsHandler: JIRA.Issues.ColumnPickerSuggestHandler,
            actionBar: this.model.getActionBarText()
        });

        //Set the selected columns
        this._setSelectedColumns();

        if (this._autoUpdate) {
            selectElement.bind("selected", _.bind(this.saveColumns, this));
            selectElement.bind("unselect", _.bind(this.saveColumns, this));
        }

        this._sparkler.$field.focus();

        // Intercept column reset and use our own method
        this.$el.on("actionclick", _.bind(function (ev) {
            ev.preventDefault();
            this.model.unset("columns");
            this.model.destroy();
        }, this));

        this.model.on("change:editDisabled", _.bind(this._disableEditIfNeeded, this));
    },

    /**
     * Saves the sparkler's columns in our model and persists them
     */
    saveColumns: function () {
        this.model.setUnsortedColumns(this._sparkler.model.getSelectedValues());
        this.model.save(null, { wait: false });
    },

    /**
     * Sets the selected items in the sparkler
     * @private
     */
    _setSelectedColumns: function () {
        var selectedColumns = this.model.getColumns();
        var sparkler = this._sparkler;

        //If the sparkler has already been built, and we hae selectedColumns...
        if (sparkler && selectedColumns && selectedColumns.length) {
            _.each(sparkler.model.getDisplayableUnSelectedDescriptors(), function (descriptor) {
                if (_.contains(selectedColumns, descriptor.value())) {
                    sparkler.selectItem(descriptor);
                }
            });
            _.each(sparkler.model.getDisplayableSelectedDescriptors(), function (descriptor) {
                if (!_.contains(selectedColumns, descriptor.value())) {
                    sparkler.unselectItem(descriptor);
                }
            });

            sparkler.render();
            this._disableEditIfNeeded();
        }
    },

    /**
     * Builds the queryableSelect with all the available items
     *
     * @param {Array.<{label: string, value: string}>} columns Columns to include in the sparkler
     * @returns {jQuery} A select element containing all the options
     * @private
     */
    _buildQueryableSelect: function (columns) {
        var instance = this;
        var availableItems = _.map(columns, function (column) {
            return new AJS.ItemDescriptor(_.extend(column, {
                title: column.label
            }));
        });

        return jQuery(AJS.Templates.queryableSelect({
            id: instance.model.getName() + "-column-sparkler",
            descriptors: [
                new AJS.GroupDescriptor({
                    items: availableItems
                })
            ]
        }));
    }
});

module.exports = ColumnPickerSparklerView;