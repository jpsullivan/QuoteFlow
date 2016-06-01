"use strict";

var Marionette = require('backbone.marionette');

var LoadingErrorView = Marionette.ItemView.extend({
    template: JST["asset-viewer/fields/errors-loading"],

    initialize: function (errorMessages, isTimeout) {
        this.errorMessages = errorMessages;
        this.isTimeout = isTimeout;
    },

    serializeData: function () {
        return {
            errorMessages: this.errorMessages,
            isTimeout: this.isTimeout
        };
    },

    onRender: function () {
        this.setElement(JIRA.Messages.showErrorMsg(this.$el.html(), {
            closeable: true,
            timeout: 0
        }));
    }
});

module.exports = LoadingErrorView;
