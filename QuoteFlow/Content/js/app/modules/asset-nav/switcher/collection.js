"use strict";

var Brace = require('backbone-brace');
var SwitcherModel = require('./model.js');

/**
 *
 */
var SwitcherCollection = Brace.Collection.extend({
    model: SwitcherModel
});

module.exports = SwitcherCollection;
