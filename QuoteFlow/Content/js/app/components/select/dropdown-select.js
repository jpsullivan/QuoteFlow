"use strict";

var $ = require('jquery');
var Control = require('../control/control');
var InlineLayerFactory = require('../layer/inline-layer-factory');
var List = require('../list/list');
var SelectModel = require('./select-model');

/**
 * @class DropdownSelect
 * @extends Control
 */
var DropdownSelect = Control.extend({

    init: function (options) {
        var instance = this;

        this.model = new SelectModel(options);
        this.model.$element.hide();
        this._createFurniture();

        this.dropdownController = InlineLayerFactory.createInlineLayers({
            alignment: "left",
            width: 200,
            hideOnScroll: ".asset-container",
            content: jQuery(".aui-list", this.$container)
        });

        this.dropdownController.layer().addClass("select-menu");

        this.listController = new List({
            containerSelector: jQuery(".aui-list", this.$container),
            groupSelector: "ul.opt-group",
            itemSelector: "li:not(.no-suggestions)",
            selectionHandler: function (e) {
                instance._selectionHandler(this.getFocused(), e);
                e.preventDefault();
            }
        });

        this._assignEventsToFurniture();
    },

    show: function() {
        this.listController.generateListFromJSON(this.model.getAllDescriptors());
        this.dropdownController.show();
        this.listController.index = 0;
        this.listController.focus();
        this.listController.enable();
    },

    _assignEventsToFurniture: function () {
        this._assignEvents("trigger", this.$trigger);
    },

    _createFurniture: function () {
        var id = this.model.$element.attr("id");

        this.$container = this._render("container", id);
        this.$trigger = this.model.$element.prev("a").appendTo(this.$container);
        this.$container.append(this._render("suggestionsContainer", id));
        this.$container.insertBefore(this.model.$element);
    },


    _renders: {
        container : function (idPrefix) {
            return jQuery('<div class="select-menu" id="' + idPrefix +'-multi-select">');
        },
        suggestionsContainer : function (idPrefix) {
            return jQuery('<div class="aui-list aui-list-checked" id="' + idPrefix +'-suggestions" tabindex="-1"></div>');
        }
    },

    _selectionHandler: function (selected) {
        var instance = this,
            intCount = 0;

        this.model.setSelected(selected.data("descriptor"));

        this.dropdownController.content().find(".aui-checked").removeClass(".aui-checked");

        selected.addClass(".aui-checked");

        var myInterval = window.setInterval(function () {
            intCount++;
            selected.toggleClass(".aui-checking");
            if (intCount > 2) {
                clearInterval(myInterval);
                instance.dropdownController.hide();
            }

        }, 80);
    },

    _events: {
        trigger:  {
            click: function (e) {
                this.show();
                e.preventDefault();
                e.stopPropagation();
            }
        }
    }
});
