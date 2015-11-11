"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');
var MarionetteMixins = require('../mixins/marionette');

/**
 * Base class for all AppModules. It provides a declarative way to set the
 * commands, requests and events, decoupled from the actual application where
 * this module is being installed.
 *
 * An AppModule is a Marionette.Module that encapsulates a "native" module
 * (i.e. a module designed to be constructed using the new operator).
 * The recommendation is to set command/requests for the module API, so others
 * Marionette.Modules's can use it in a decoupled way.
 *
 * @abstract
 * @constructor
 */
var AppModule = function () {
    // ensure definition function is bound to the module
    this.definition = _.bind(this.definition, this);
};

// add extend() functionality
AppModule.extend = Marionette.extend;

// add methods to our prototype
_.extend(AppModule.prototype, {

    /**
     * Creates the internal module. This method must be implemented by the descendant of this class.
     *
     * @abstract
     */
    create: function() {
        throw new Error("create() must be implemented by AppModules");
    },

    /**
     * whether this module will create a request that will return the internal module. This should be used only
     * for testing or for code that has not been completely migrated to Marionette Applications and Modules yet.
     *
     * @type {boolean}
     */
    generateMasterRequest: false,

    /**
     * Module name. Will be used as a prefix for all the commands/requests/events. For example, if the name is
     * 'myModule', all commands will be in the form 'myModule:*'. This property must be defined by each extending
     * module.
     *
     * @type {string}
     */
    name: "",

    /**
     * List of commands implemented by this module.
     *
     * The usual form is {commandName: function}. In this case, this module will implement a command named
     * '<this.name>:commandName', and function will be the handler.
     *
     *     MyModule = JIRA.Marionette.AppModule.extend({
     *          name: "myModule",
     *          commands: {
     *              "sayHello": function() {alert("Hello world");}
     *          }
     *     })
     *     MyApp.module("MyModule", new MyModule().definition);
     *     MyApp.execute("myModule:sayHello") //alerts "Hello world"
     *
     *
     * Calling the internal module
     * ---------------------------
     * This list can be an object, or a function that returns an object. In the function case, this function will
     * receive the internal module (created in create()) as argument.
     *
     *     myInternalModule = function() {}
     *     myInternalModule.prototype.getName = function() {return "Charlie";}
     *
     *     MyModule = JIRA.Marionette.AppModule.extend({
     *          name: "myModule",
     *          create: function() { return new myInternalModule(); },
     *          commands: function(internalModule) {
     *              return {
     *                  "sayHello": function() {alert(internalModule.getName());}
     *              }
     *          }
     *     })
     *     MyApp.module("MyModule", new MyModule().definition);
     *     MyApp.execute("myModule:sayHello") //alerts "Charlie"
     *
     *
     * Calling the internal module API
     * -------------------------------
     * If a command is just a call to the internal module API with the same name, AppModule provides a shortcut so you
     * don't need all the boilerplate. Just use 'true' as the value of the command, and it will call the method in
     * the internal module with the same name than the command.
     *
     *
     *     myInternalModule = function() {}
     *     myInternalModule.prototype.getName = function() {return alert("Charlie");}
     *
     *     MyModule = JIRA.Marionette.AppModule.extend({
     *          name: "myModule",
     *          create: function() { return new myInternalModule(); },
     *          commands: {
     *             "getName": true
     *          }
     *     })
     *
     *     MyApp.module("MyModule", new MyModule().definition);
     *     MyApp.execute("myModule:getName") //alerts "Charlie"
     *                                       //equivalent to invoking myInternalModule.getName()
     *
     * @type {Object|function}
     */
    commands: {},

    /**
     * Just like {@link commands}, but for defining requests.
     * @type {Object|function}
     */
    requests: {},

    /**
     * List of events this module will fire.
     *
     * This module will listen for the events named here fired in the internal module, and re-fire them with the prefix
     * this.name. All the events will include the original event arguments.
     *
     *     myInternalModule = function() {} //... your internal module
     *
     *     MyModule = JIRA.Marionette.AppModule.extend({
     *          name: "myModule",
     *          create: function() { return new myInternalModule(); },
     *          events: [
     *             "loaded"
     *          ]
     *     })
     *
     *     //MyModule will fire the event 'myModule:loaded' when myInternalModule fires the event 'loaded'
     *
     *
     * If the event name is an empty string, the event fired by MyModule will be just {@link name}
     *
     * @type {string[]}
     */
    events: [],

    /**
     * Definition for this module. This module will create the internal module and set all the events/commands/requests
     * used by this module. This is the function you need to pass to Application.module() to define a new module:
     *
     *      MyApp = new JIRA.Marionette.Application();
     *      MyApp.module("myModule", new MyModule().definition)
     *
     *
     * @param {JIRA.Libs.Marionette.Module} mod The module itself
     * @param {JIRA.Libs.Marionette.Application} app The Parent module, or Application object that .module was called from
     */
    definition: function(mod, app) {
        var prefix = this.name;
        var instance = this;
        if (!prefix) {
            throw new Error("'name' must be defined by AppModules");
        }

        // Generates the initializer used for this module. It will install all the commands and requests in the
        // application, and set the proper events.
        mod.addInitializer(function(options){
            // Create the internal module
            var internalModule = instance.create.apply(instance, _.toArray(arguments));

            function generateCommands(commands) {
                if (_.isFunction(commands)) {
                    commands = commands(internalModule);
                }

                var events = {};
                _.each(commands, function(def, name) {
                    if (def === true) {
                        def = _.bind(internalModule[name], internalModule);
                    }
                    if (name && prefix) {
                        name = prefix+":"+name;
                    } else {
                        name = prefix;
                    }
                    events[name] = def;
                });

                return events;
            }
            function generateEvents(app, events) {
                if (_.isFunction(events)) {
                    events = events(internalModule);
                }

                _.each(events, function(originalName) {
                    var newName = prefix+":"+originalName;
                    app.listenTo(internalModule, originalName, function() {
                        this.trigger.apply(this, [newName].concat(_.toArray(arguments)));
                    });
                });
            }

            // Generate the commands and requests for this module.
            var commands = generateCommands(instance.commands);
            var requests = generateCommands(instance.requests);

            // Generate the master request for this module.
            if (instance.generateMasterRequest) {
                _.extend(requests, generateCommands({
                    "": function() { return internalModule; }
                }));
            }

            app.commands.setHandlers(commands);
            app.reqres.setHandlers(requests);
            generateEvents(app, instance.events);
        });
    }
});

_.extend(AppModule.prototype, MarionetteMixins);

module.exports = AppModule;
