QuoteFlow.Components.Query = function() {
    var fields = {
        catalog: "Catalog",
        manufacturer: "Manufacturer",
        creator: "Creator",
        description: "Description",
        created: "Created Date",
        updated: "Updated Date"
    };

    return {
        DEFAULT_CLAUSES: ["catalog", "manufacturer", "creator"],
        create: function(c) {
            c = _.defaults(c, {
                primaryClauses: this.DEFAULT_CLAUSES,
                without: [],
                style: "generic",
                layoutSwitcher: true,
                autocompleteEnabled: true,
                advancedAutoUpdate: false,
                basicOrderBy: false,
                basicAutoUpdate: true,
                preferredSearchMode: "basic"
            });
            c.primaryClauses = _.reject(c.primaryClauses, function(d) {
                return _.contains(c.without, d.id);
            });
            _.each(c.primaryClauses, function(e, d) {
                if (typeof e === "string") {
                    if (fields[e]) {
                        c.primaryClauses[d] = {
                            id: e,
                            name: fields[e]
                        }
                    } else {
                        console.error("QuoteFlow.Components.Query: You have specified clause [" + e + "], but no i18n string for it could be found. Instead use {id:" + e + ", name: '[NAME_HERE]'}");
                    }
                }
            });

            var queryModule = new QuoteFlow.Module.Asset.Query({
                primaryClauses: c.primaryClauses,
                searchers: c.searchers,
                queryStateModel: new QuoteFlow.Model.Asset.QueryState({
                    jql: c.jql,
                    without: c.without,
                    style: c.style,
                    layoutSwitcher: c.layoutSwitcher,
                    autocompleteEnabled: c.autocompleteEnabled,
                    advancedAutoUpdate: c.advancedAutoUpdate,
                    basicAutoUpdate: c.basicAutoUpdate,
                    preferredSearchMode: c.preferredSearchMode,
                    basicOrderBy: c.basicOrderBy
                })
            });

            if (c.jql || c.jql === "") {
                queryModule.resetToQuery(c.jql).always(function() {
                    //jQuery(c.el).addClass("ready");
                    queryModule.triggerInitialized(c.jql);
                });
            }

            return queryModule;
        }
    }
}();