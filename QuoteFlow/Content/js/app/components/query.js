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
                autocompleteEnabled: true,
                orderBy: false
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

            return c;
        }
    }
}();