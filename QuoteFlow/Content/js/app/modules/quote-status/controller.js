"use strict";

var Marionette = require('backbone.marionette');

var RestfulTable = require('@atlassian/aui/lib/js/aui/restful-table');

/**
 * Contains callbacks for the asset module router.
 */
var QuoteStatusController = Marionette.Controller.extend({

    adminIndex: function () {
        new RestfulTable({
            autoFocus: true,
            el: jQuery("#quote_status_container"),
            allowReorder: true,
            resources: {
                all: QuoteFlow.RootUrl + "api/quotestatus",
                self: QuoteFlow.RootUrl + "api/quotestatus"
            },
            columns: [
                {
                    id: "name",
                    header: "Name"
                },
                {
                    id: "description",
                    header: "Description"
                },
                {
                    id: "rules",
                    header: "Edit Rules"
                }
            ]
        });
    }
});

module.exports = QuoteStatusController;
