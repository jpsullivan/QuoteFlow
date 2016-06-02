"use strict";

var $ = require('jquery');
var Dropdown = require('./dropdown');
var Objects = require('../../util/objects');

var DropdownCreateFactory = {
    /**
     * Static factory method to create multiple dropdowns at one time.
     *
     * @method Dropdown.createDropdown
     * @param {Dropdown.OptionsDescriptor} options
     * @return {Array}
     */
    createDropdown: function (options) {
        var dropdowns = [];

        if (options.content && !options.trigger) {
            options.content = $(options.content);

            $.each(options.content, function () {
                var instanceOptions = Objects.copyObject(options);
                instanceOptions.content = $(this);
                dropdowns.push(new Dropdown(instanceOptions));
            });
        } else if (!options.content && options.trigger) {
            options.trigger = $(options.trigger);

            $.each(options.trigger, function () {
                var instanceOptions = Objects.copyObject(options);
                instanceOptions.trigger = $(this);
                dropdowns.push(new Dropdown(instanceOptions));
            });
        } else if (options.content && options.trigger) {
            options.content = $(options.content);
            options.trigger = $(options.trigger);

            if (options.content.length === options.trigger.length) {
                options.trigger.each(function (i) {
                    var instanceOptions = Objects.copyObject(options);
                    instanceOptions.trigger = $(this);
                    instanceOptions.content = options.content.eq(i);
                    dropdowns.push(new Dropdown(instanceOptions));
                });
            } else {
                throw new Error("Dropdown.create: Expected the same number of content elements as trigger elements");
            }
        }

        return dropdowns;
    }
};

module.exports = DropdownCreateFactory;
