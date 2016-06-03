"use strict";

var Brace = require('backbone-brace');

var QuoteModel = Brace.Model.extend({
    namedAttributes: [
        "id",
        "name",
        "description",
        "status",
        "responded",
        "totalPrice",
        "totalLineItems",
        "creatorId"
    ]
});

module.exports = QuoteModel;
