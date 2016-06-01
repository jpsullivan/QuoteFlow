"use strict";

window.$ = window.jQuery = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Marionette = require('backbone.marionette');

var aui = require('aui');
AJS.Meta = require('./util/meta');
// var auiExperimental = require('aui-experimental');

if (window.__agent) {
    window.__agent.start(Backbone, Marionette);
}

var App = require('./app');



$(document).ready(function () {
    App.start();
    console.log('Started');
});
