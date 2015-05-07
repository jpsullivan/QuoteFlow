"use strict";

var Brace = require('backbone-brace');

/**
 *
 */
var SwitcherModel = Brace.Model.extend({

    /**
     * id: id of the switcher
     * name: switcher name (displayed in switcher view)
     * view: backbone view object
     */
    namedAttributes: ["id", "name", "view", "text"]
});

module.exports = SwitcherModel;
