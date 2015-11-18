"use strict";

var PagerController = require('./controller');
var PagerModel = require('./model');

var PagerComponent = function(callbacks) {
    var pagerModel,
        pagerController;

    this.initialize = function() {
        pagerModel = new PagerModel();
    };

    this.update =  function(data) {
        if (data) {
            pagerModel.update(data);
        }else{
            pagerModel.clear();
        }
    };

    this.show = function(container, element) {
        if (!pagerController) {
            pagerController = new PagerController({
                model: pagerModel
            });
            pagerController.on("goBack", callbacks.goBack);
            pagerController.on("nextItem", callbacks.nextItem);
            pagerController.on("previousItem", callbacks.previousItem);
        }
        pagerController.show(container, element);
    };

    this.destroy = function() {
        if (pagerController) {
            pagerController.destroy();
        }
        pagerController = null;
        pagerModel.clear();
    }
};

module.exports = PagerComponent;
