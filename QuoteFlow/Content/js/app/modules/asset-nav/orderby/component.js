"use strict";

var OrderByModel = require('./model');
var OrderByView = require('./view');

var OrderByComponent = {
    create: function (options) {
        options = options || {};

        var model = new OrderByModel({
            sortBy: options.sortBy,
            jql: options.jql
        });
        var view = new OrderByView({ model: model });

        // publish public api
        return {
            onSort: function () {
                model.onSort.apply(model, arguments);
                return this;
            },
            offSort: function (method) {
                model.off("sort", method);
                return this;
            },
            render: function () {
                view.render();
                return this;
            },
            setElement: function (el) {
                view.setElement(el);
                return this;
            },
            setJql: function (jql) {
                model.setJql(jql);
                return this;
            },
            setSortBy: function (options) {
                model.setSortBy(options);
                return this;
            }
        };
    }
};

/**
 * Factory to create order by controls
 */
QuoteFlow.Events.ISSUE_TABLE_REORDER = "issueTableReorder";

module.exports = OrderByComponent;