"use strict";

var _ = require('underscore');

var AssetQueryModule = require('../modules/asset/queries/query');
var AssetQueryStateModel = require('../models/asset/query');

var QueryComponent = function () {
    var clauses = {
        catalog: "Catalog",
        manufacturer: "Manufacturer",
        creator: "Creator",
        description: "Description",
        created: "Created Date",
        updated: "Updated Date"
    };

    return {
        DEFAULT_CLAUSES: ["catalog", "manufacturer", "creator"],

        create: function (options) {

            options = _.defaults(options, {
                primaryClauses : this.DEFAULT_CLAUSES,
                without: [],
                style: "generic",
                /* This has to be true :( - If issue-nav-components is anything below 6.2, the layoutSwitcher option
                 * didn't exist when it was first consumed in 6.1. */
                layoutSwitcher: true,
                autocompleteEnabled: true,
                advancedAutoUpdate: false,
                basicOrderBy: false,
                basicAutoUpdate: true,
                preferredSearchMode: "basic"
            });


            options.primaryClauses = _.reject(options.primaryClauses, function (clause) {
                return _.contains(options.without, clause.id);
            });

            _.each(options.primaryClauses, function (clause, idx) {
                if (typeof clause === "string") {
                    if (clauses[clause]) {
                        options.primaryClauses[idx] = {id: clause, name: clauses[clause]};
                    } else {
                        console.error("QueryComponent: You have specified clause [" + clause + "]. " +
                            "But we do not have the i18n string for it, probably a custom field. Instead use {id:" + clause + ", name: '[NAME_HERE]'}");
                    }
                }
            });

            var queryModule = new AssetQueryModule({
                queryStateModel: new AssetQueryStateModel({
                    jql: options.jql,
                    without: options.without,
                    style: options.style,
                    layoutSwitcher: options.layoutSwitcher,
                    autocompleteEnabled: options.autocompleteEnabled,
                    advancedAutoUpdate: options.advancedAutoUpdate,
                    basicAutoUpdate: options.basicAutoUpdate,
                    preferredSearchMode: options.preferredSearchMode,
                    basicOrderBy: options.basicOrderBy

                }),
                primaryClauses: options.primaryClauses,
                searchers: options.searchers
            });

            jQuery(options.el).addClass("query-component " + options.style + "-styled");

            queryModule.createAndRenderView(options.el);

            if (options.jql || options.jql === "") {
                queryModule.resetToQuery(options.jql).always(function () {
                    jQuery(options.el).addClass("ready");
                    // Consumers of this component want to know when the jql (given at construction) is represented in the ui.
                    queryModule.triggerInitialized(options.jql);
                });
            }
            return queryModule;
        }
    };
};

module.exports = QueryComponent;
