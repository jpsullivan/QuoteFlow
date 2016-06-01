"use strict";

var AppModule = require('../../../base/app-module');
var NavigationController = require('./navigation-controller');

var NavigationAppModule = AppModule.extend({
    name: "navigation",
    create: function create () {
        return new NavigationController();
    },
    commands: {
        'reset': true,
        'updateState': true,
        'navigate': true,
        'navigateToUrl': true
    },
    events: [
        'stateChanged'
    ]
});

module.exports = NavigationAppModule;
