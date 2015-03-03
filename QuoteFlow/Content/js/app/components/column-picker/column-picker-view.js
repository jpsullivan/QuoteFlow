"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

var ColumnPickerModel = require('./column-picker-model');
var ColumnPickerSparklerView = require('./column-picker-sparkler-view');

/**
 * This view contains the column picker trigger, the dialog and one subview ({@link ColumnPickerSparklerView})
 * per each column config. Its job is to open/close the dialog, display buttons to select the column config, and
 * display the corresponding subview based on those buttons.  It also handles the 'Save' and 'Cancel' buttons at
 * the bottom of the dialog.
 *
 * The model of this view is {@link JIRA.Issues.columnPickerModel}
 *
 * @constructs
 * @extends Brace.View
 * @param {Object} options
 * @param {ColumnPickerModel} options.columnPickerModel Model to use in this view
 */
var ColumnPickerView = Brace.View.extend({
    initialize: function (options) {
        _.extend(this, options);
        _.bindAll(this,
            "_generateInlineDialogContent",
            "_onDOMCloseDialogClick",
            "_onDOMConfigChooserClick",
            "_onDOMDialogHide",
            "_onDOMFormSubmit",
            "_onDOMTriggerClick",
            "_onModelChangeAvailableColumns",
            "_onModelChangeColumnConfig",
            "_onModelChangeColumnConfigDisabled",
            "_onModelDestroyColumnConfig");

        /**
         * Stores all the subviews handled by this view, indexed by name
         * @type {Object.<string, ColumnPickerSparklerView>}
         */
        this.subViews = {};
    },

    /**
     * Stores the inline dialog used by this view. Populated by {@link JIRA.Issues.ColumnPickerView#render}
     * @type {AJS.InlineDialog}
     */
    dialog: null,

    /**
     * Stores a reference to the active subview. Populated by {@link JIRA.Issues.ColumnPickerView#_activateNewSubView}
     * @type {ColumnPickerSparklerView}
     */
    activeSubview: null,

    /**
     * Handles the click event on config chooser
     *
     * This method sets the columnConfig in our model and closes the dialog
     *
     * @param ev (jQuery.Event)
     * @private
     */
    _onDOMConfigChooserClick: function (ev) {
        var $target = jQuery(ev.target);

        // If the button is disabled, do nothing
        if ($target.attr("aria-disabled")) {
            return;
        }

        this.columnPickerModel.setColumnConfig($target.data("value") || "user");
    },

    /**
     * Activate a new subview. Each column config has a different logic, so all the decisions are delegated
     * to the columnConfig models.
     *
     * @param {ColumnPickerSparklerView} subview View being activated
     * @private
     */
    _activateNewSubView: function (subview) {
        subview.activate();

        if (this.columnPickerModel.shouldCloseOnActivation()) {
            this.dialog.hide();
        } else {
            this.activeSubview = subview;
            this.activeSubview.show();
            this.adjustHeight();
        }

        if (this.columnPickerModel.shouldRefreshSearchOnActivation()) {
            this.columnPickerModel.refreshSearchWithColumns();
        }

        if (this.columnPickerModel.shouldLoadDefaultsOnActivation()) {
            this.columnPickerModel.loadDefaultColumns();
        }
    },

    /**
     * Handles the form submit event
     *
     * This method saves the columns in the active columnConfig and closes the dialog
     *
     * @param ev (jQuery.Event)
     * @private
     */
    _onDOMFormSubmit: function (ev) {
        // Stops the form submission
        ev.preventDefault();

        this.activeSubview.saveColumns();
        this.dialog.hide();
    },

    /**
     * Handles click event on the close dialog link
     *
     * This method hides the dialog
     * @param ev (jQuery.Event)
     * @private
     */
    _onDOMCloseDialogClick: function (ev) {
        this.dialog.hide();
    },

    /**
     * Handles the click on the dialog trigger
     *
     * @param ev (jQuery.Event)
     * @private
     */
    _onDOMTriggerClick: function (ev) {
        //Display the dialog
        this.dialog.show();

        // Display the active sparkler
        this.activeSubview = this.subViews[this.columnPickerModel.getCurrentColumnConfig().getName()];
        this.activeSubview.show();
        this._activateConfigChooserButton(this.columnPickerModel.getCurrentColumnConfig().getName());

        // Send request to get the available columns
        this.columnPickerModel.fetchAvailableColumnsIfNeeded();
    },

    _activateConfigChooserButton: function (newButtonName) {
        this.dialog.find(".config-chooser.active").removeClass("active");
        this.dialog.find("#columns-chooser-" + newButtonName).addClass("active");
    },

    /**
     * Handles the close event on the InlineDialog
     *
     * @param ev (jQuery.Event)
     * @private
     */
    _onDOMDialogHide: function (ev) {
        this.columnPickerModel.revertColumnConfig();
        this.activeSubview.hide();
    },

    /**
     * Handler for change:columnConfig
     *
     * @param {JIRA.Issues.columnPickerModel} columnPickerModel Model that has changed
     * @param {string} columnConfig New value for columnConfig
     * @private
     */
    _onModelChangeColumnConfig: function (columnPickerModel, columnConfig) {
        var newSubView = this.subViews[columnConfig];
        this._activateConfigChooserButton(newSubView.model.getName());

        var oldSubView = this.subViews[columnPickerModel.previous("columnConfig")];
        if (oldSubView != newSubView) {
            oldSubView.hide();
            oldSubView.deactivate();
            this._activateNewSubView(newSubView);
        }
    },

    /**
     * Handler for change:availableColumns
     *
     * @param {JIRA.Issues.columnPickerModel} columnPickerModel Model that has changed
     * @param {Array.<{label: string, value: string}>} availableColumns New value for availableColumns
     * @private
     */
    _onModelChangeAvailableColumns: function (columnPickerModel, availableColumns) {
        _.each(this.subViews, function (columnPickerView) {
            columnPickerView.createSparklerControl(availableColumns);
        });
    },

    /**
     * Handler for destroy
     * @private
     */
    _onModelDestroyColumnConfig: function () {
        this.dialog.hide();
    },

    /**
     * Handler for change:isDisabled
     *
     * @param {ColumnConfigModel} model Model that has changed
     * @param {boolean} isDisabled New value for isDisabled
     * @private
     */
    _onModelChangeColumnConfigDisabled: function (model, isDisabled) {
        var button = this.dialog.find("#columns-chooser-" + model.getName());
        if (isDisabled) {
            button.attr("aria-disabled", true);
        } else {
            button.removeAttr("aria-disabled");
        }
    },

    /**
     * Renders the trigger and the dialog
     */
    render: function () {
        if (!this.$trigger) {
            this.$trigger = jQuery(JIRA.Templates.Dialogs.ColumnPicker.trigger());
        }
        this.$el.append(this.$trigger);
        this.$trigger.click(this._onDOMTriggerClick);

        this._renderInlineDialog();
    },

    /**
     * Adjust the height of the dialog based on window height
     *
     * @returns {Boolean}
     * @private
     */
    adjustHeight: function () {
        if (AJS.InlineDialog.current && AJS.InlineDialog.current.id === "column-picker-dialog" && this.activeSubview) {
            var scrollList = jQuery(this.activeSubview.el).find(".aui-list-scroll");
            //Maximum available height = ((window height - trigger y position) - dialog bottom padding)
            var maxDialogHeight = ((window.innerHeight - this.$trigger.offset().top) - 90);
            //Delta = max available height - actual height
            var heightDelta = maxDialogHeight - this.dialog.height();
            //Desired scroll list height = actual height + Delta
            //Height confined to: 80 pixel < Scroll list height < 270 pixel
            var scrollListHeight = Math.max(Math.min(scrollList.height() + heightDelta, 270), 80);
            scrollList.css("height", scrollListHeight);
            return true;
        }
        return false;
    },

    /**
     * Generates the dialog's content
     *
     * @param {jQuery} $content The div element that will contain the custom content
     * @param {jQuery} $trigger The element of your dialog trigger
     * @param {Function} done Callback to run when the content is ready to be displayed
     * @private
     */
    _generateInlineDialogContent: function ($content, $trigger, done) {
        if (!this.dialogContent) {
            // Injects the template into the dialog
            $content.html(JIRA.Templates.Dialogs.ColumnPicker.popup({
                modifierKey: AJS.Navigator.modifierKey(),
                isAutoUpdate: this.columnPickerModel.getAutoUpdate(),
                columns: _.map(this.columnPickerModel.columnsData, _.bind(function (col) {
                    return {
                        name: col.getName(),
                        description: col.getDescription(),
                        selected: false,
                        isDisabled: col.isDisabled(),
                        isActive: this.columnPickerModel.getCurrentColumnConfig() == col
                    };
                }, this))
            }));

            // Bind DOM handlers
            $content.find(".config-chooser").click(this._onDOMConfigChooserClick);
            $content.find("form").submit(this._onDOMFormSubmit);
            $content.find(".close-dialog").click(this._onDOMCloseDialogClick);

            // Avoid dialog being closed on click
            $content.click(function (e) {
                e.stopPropagation();
            });

            // Create the sparklers
            this._createSubViews($content);

            this.dialogContent = $content.children();
        } else {
            $content.append(this.dialogContent);
        }

        done();
    },

    /**
     * Renders the InlineDialog
     *
     * @private
     */
    _renderInlineDialog: function () {
        // If the dialog has been already rendered, do nothing
        if (this.dialog) {
            return;
        }

        // Build the dialog
        this.dialog = AJS.InlineDialog(
            this.$trigger,
            "column-picker-dialog",
            this._generateInlineDialogContent,
            {
                offsetY: 15,
                addActiveClass: true,
                hideDelay: null,
                noBind: true,
                initCallback: _.bind(function () {
                    //This is called when the inline dialog has finished rendering
                    //Using a timeout to adjust the height because when this is called, the column picker
                    //has not been completely rendered into the page yet
                    var instance = this;
                    var timeoutAdjust = function () {
                        if (!instance.adjustHeight()) {
                            //noinspection DynamicallyGeneratedCodeJS
                            setTimeout(timeoutAdjust, 100);
                        }
                    };
                    timeoutAdjust();
                }, this),
                hideCallback: this._onDOMDialogHide
            }
        );
    },

    /**
     * Creates the sparklers subViews
     *
     * @param {jQuery} $content
     * @private
     */
    _createSubViews: function ($content) {
        _.each(this.columnPickerModel.columnsData, _.bind(function (columnConfigModel) {
            this.subViews[columnConfigModel.getName()] = new ColumnPickerSparklerView({
                el: $content.find("." + columnConfigModel.getName() + "-column-sparkler"),
                model: columnConfigModel,
                autoUpdate: this.columnPickerModel.getAutoUpdate()
            });
        }, this));

        //Create sparkler controls if/when we have the available columns 
        if (this.columnPickerModel.has("availableColumns")) {
            this._onModelChangeAvailableColumns(this.columnPickerModel, this.columnPickerModel.getAvailableColumns());
        }
        this.columnPickerModel.on("change:availableColumns", this._onModelChangeAvailableColumns);

        //Bind model events
        this.columnPickerModel.on("change:columnConfig", this._onModelChangeColumnConfig);
        this.columnPickerModel.onChangeColumnConfigDisabled(this._onModelChangeColumnConfigDisabled);
        this.columnPickerModel.onDestroyColumnConfig(this._onModelDestroyColumnConfig);
    }
});

module.exports = ColumnPickerView;