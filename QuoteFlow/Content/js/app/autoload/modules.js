"use strict";

var AutoloadModules = function (app) {
	require('../modules/asset-nav/module')(app);
	require('../modules/quote-status/module')(app);
};

module.exports = AutoloadModules;
