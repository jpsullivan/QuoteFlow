"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');
var State = require('./state');

/**
 * An object describing navigation parameters. Every property, unless noted, is passed with the stateChanged event.
 *
 * @typedef {Object} NavigationOptions
 * @property {boolean} reset - should the state be reseted before performing the navigation
 * @property {boolean} forceRefresh - should the stateChanged event be fired even when state does not change.
 * @property {boolean} fullPageLoad - is the state change a full page load navigation,
 *                                  meaning that it was the first navigation handled by controller
 * @property {boolean} routerEvent - is the navigation event coming from the router, meaning that it is a first
 *                                  application navigation, or it comes from browser history
 * @property {boolean} replace - should the URL in the address bar be replaced
 * @property {String} reason - reason for changing application state
 */

/**
 * Controller responsible for maintaining and modifying application state.
 * Every state change will result in stateChanged event being triggered.
 *
 * @class
 * @extends Marionette.Controller
 * @fires stateChanged
 */
var NavigationController = Marionette.Controller.extend({
    /**
     * @constructs
     * @param {Object} options - Configuration of the controller.
     * @param {boolean} [options.isFullPageLoad=true] - should the controller consider first navigation a full
     * page load. It will include this information in stateChanged event.
     * @param {State} [options.model] - the State object that the controller will use for storing state.
     */
    initialize: function initialize (options) {
        options = _.defaults(options, {isFullPageLoad: true});
        this.state = options.model || new State();
        this.isFullPageLoad = options.isFullPageLoad;
        this.eventQueue = [];
    },

    /**
     * Reset to blank state
     */
    reset: function reset () {
        this.state = new State();
    },
    /**
     * Process the application navigation to provided state.
     * It will check if navigation is possible and update application state accordingly.
     *
     * @param {Object} state - object containing state properties
     * @param {NavigationOptions} options - contains navigation properties
     */
    navigate: function navigate (state, options) {
        if (QuoteFlow.application.request("assetEditor:canDismissComment")) {
            this.updateState(state, options);
        }
    },

    /**
     * Process the application navigation to provided URL.
     *
     * @param {String} url - URL to which the navigation will be performed
     * @param {NavigationOptions} options - contains navigation properties
     * @param {Object} override - object containing state properties that will get overwritten in the state derived from the URL
     */
    navigateToUrl: function navigate (url, options, override) {
        override = override || {};
        var state = _.extend(this.state.getStateFromUrl(url), override);
        this.navigate(state, options);
    },

    /**
     * Update the application state to the one provided.
     *
     * @param {Object} state - object containing state properties
     * @param {NavigationOptions} options - contains navigation properties
     */
    updateState: function updateState (state, options) {
        options = options || {};

        var previousState = _.pick(this.state, _.keys(this.state));

        if (options.reset) {
            this.reset();
        }
        _.extend(this.state, state);

        if (!_.isEqual(previousState, _.pick(this.state, _.keys(this.state))) || options.forceRefresh) {
            this._triggerStateChangedEvent(options);
        }
    },

    _triggerStateChangedEvent: function _triggerStateChangedEvent (options) {
        if (this.isFullPageLoad) {
            _.extend(options, {fullPageLoad: true});
            this.isFullPageLoad = false;
        }
        options = _.omit(options, 'forceRefresh');
        this.eventQueue.push({state: _.clone(this.state), options: options});
        this._processEvents();
    },

    _processEvents: function _processEvents () {
        /**
         * Application state change event
         *
         * @event stateChanged
         * @property {State} state - state that controller navigated to
         * @property {NavigationOptions} options - array of options that describe state change
         */
        if (!this.processingEvent) {
            this.processingEvent = true;
            var event = this.eventQueue.shift();
            this.trigger('stateChanged', event.state, event.options);
            this.processingEvent = false;
            if (this.eventQueue.length) {
                this._processEvents();
            }
        }
    }
});

module.exports = NavigationController;
