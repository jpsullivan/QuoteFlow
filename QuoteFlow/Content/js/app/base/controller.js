"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;
var Marionette = require('backbone.marionette');

var Mixins = require('./mixins');

/**
 * Extends a marionette controller with the added sugar that 
 * Backbone.Brace provides.
 */
var BaseController = Marionette.Controller.extend({
    
});
_.extend(BaseController.prototype, Mixins);

module.exports = BaseController;
