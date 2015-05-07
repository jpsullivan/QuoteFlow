"use strict";

window.$ = window.jQuery = require('jquery');
var aui = require('aui');
AJS.Meta = require('./util/meta');
//var auiExperimental = require('aui-experimental');
var App = require('./app');

$(document).ready(function () {
    App.start();
    console.log('Started');
});
