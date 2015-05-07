"use strict";

var Brace = require('backbone-brace');

/**
 * A CriteriaModel is a simple representation of a SearcherModel of the same id and name
 * so that the criterion can still be rendered before the SearcherModel has been constructed.
 * This happens in the case of primary criteria since they are rendered on page load, but their values
 * still need to be retrieved.
 *
 * To get the actual searcher values, the SearcherModel should be accessed from SearcherCollection, e.g:
 *
 *     var searcherModel = searchCollection.get(criteriaModel.getId())
 *
 */
var CriteriaModel = Brace.Model.extend({
    namedAttributes: ["id", "name"]
});

module.exports = CriteriaModel;
