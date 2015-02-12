"use strict";

var Marionette = require('backbone.marionette');

/**
 * The asset table rendered in the full-screen layout.
 */
var TableView = Marionette.ItemView.extend({
    syncSystemModeMessage: function () {
        var currentMessage = $("#system-mode-warning-msg");
        var isSystemMode = this.columnConfig.isSystemMode();
        if (isSystemMode && !currentMessage.length) {
            var $msg = JIRA.Messages.showWarningMsg(
                //TODO Move to a template
                AJS.I18n.getText("issues.components.column.config.system.warning.post"), {
                    id: "system-mode-warning-msg",
                    closeable: false,
                    timeout: 0 //Don't close the message automatically
                }
            );

            $msg.find(".exit").click(_.bind(function (e) {
                e.preventDefault();
                this.columnConfig.setCurrentColumnConfig("user");
            }, this));

        } else {
            currentMessage.remove();
        }
    },

    /**
     * @param {object} options
     * @param {SearchModule} options.search The application's <tt>SearchModule</tt> instance.
     * @param {SearchResults} options.searchResults The application's <tt>SearchResults</tt> instance.
     */
    initialize: function (options) {
        this.columnConfig = options.columnConfig;
        this.columnConfig.on("change:columnConfig", this.syncSystemModeMessage, this);
        QuoteFlow.Interactive.onVerticalResize(_.bind(this.adjustColumnConfigHeight, this));
    },

    /**
     * Prepare to be removed, unbinding all event handlers, etc.
     */
    onBeforeDestroy: function () {
        QuoteFlow.Interactive.offVerticalResize(this.adjustColumnConfigHeight);
    },

    /**
     * Render the issue table after a search completes.
     * Called when an operation in <tt>searchPromise</tt> completes.
     *
     * @param {object} table The search payload.
     * @private
     */
    _onSearchDone: function (el) {
        this.columnConfig.setElement(el.find(".column-picker-trigger-container")).render();
    },

    adjustColumnConfigHeight: function () {
        this.columnConfig.adjustHeight();
    }
});

module.exports = TableView;