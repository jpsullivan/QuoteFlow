/**
    Backbone.js helpers

    @class Backbone
    @namespace QuoteFlow
    @module QuoteFlow
**/
QuoteFlow.Backbone = {

    /**
        Determines if a jQuery element is equal to a 
        specific Backbone.js view. 

        @param {$el} el The jQuery element to compare.
        @param {Backbone.View} view The Backbone View to compare the jQuery
                                    element against.
        @return {boolean} True if successful match.
    */
    viewsMatch: function (el, view) {
        return $(el)[0] === view.$el[0];
    },

    /**
        Needle and haystack method for determining if a 
        jQuery element is contained within a list of Views.

        @param {$el} el The jQuery element to search for.
        @param {Array|Backbone.Collection} views The collection of Views to search within.
        @return {Backbone.View|int} Backbone view if found, otherwise -1.
    */
    findMatchedView: function (el, views) {
        if (_.isUndefined(views))
            return false;

        var result = -1;

        var self = this;
        _.each(views, function (view) {
            if (self.viewsMatch(el, view)) {
                result = view;
            }
        });

        return result;
    }
};