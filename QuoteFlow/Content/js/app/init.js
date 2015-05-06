"use strict";

window.$ = window.jQuery = require('jquery');
var aui = require('aui');
//var auiExperimental = require('aui-experimental');
var App = require('./app');

$(document).ready(function () {
    App.start();
    console.log('Started');
});
