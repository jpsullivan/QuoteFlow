"use strict";

window.$ = window.jQuery = require('jquery');
var App = require('./app');

$(document).ready(function () {
    App.start();
    console.log('Started');
});
