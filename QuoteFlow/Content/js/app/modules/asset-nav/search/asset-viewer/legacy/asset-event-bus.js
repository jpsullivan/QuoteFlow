"use strict";

var Brace = require('backbone-brace');

var AssetEventBus = Brace.Model.extend({

    namedAttributes: ["assetId"],

    namedEvents: [
        /*
         * Triggered when a user saves a field on the view asset screen. Should cause state to be saved
         * on the server.
         */
        "save",
        /*
         * Triggered when a save successfully returns from the server.
         */
        "saveSuccess",
        /**
         * Triggered when save request has been issued to server
         */
        "savingStarted",
        /*
         * Triggered when a save failed on the server.
         */
        "saveError",
        /*
         * Issue panels will fire this event when they've finished rendering after an update.
         */
        "panelRendered",
        /**
         * The asset panel that previously had focus was replaced.
         */
        "replacedFocusedPanel",
        /**
         * Field has been submitted by user.
         */
        "fieldSubmitted",
        /*
         * Fires when an asset view is closed or we go back to search.
         */
        "dismiss",
        /**
         * Triggered when the asset view needs to be refreshed.
         */
        "refreshAsset",
        /**
         * Triggered when the asset view has finished refreshing
         */
        "assetRefreshed",
        /**
         * Triggers views/models to update from a pre-existing DOM.
         */
        "updateFromDom",
        /**
         * Lets interested objects know to update status color
         */
        "updateStatusColor",
        /**
         * Triggered when a key is pressed whilst holding the tab key.
        */
        "quickEditKeyPressed",
        /**
         * Opens the focus shifter.
         */
        "openFocusShifter"
    ]
});

module.exports = AssetEventBus;
