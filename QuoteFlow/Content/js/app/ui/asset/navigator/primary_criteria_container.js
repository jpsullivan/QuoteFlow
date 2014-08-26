﻿QuoteFlow.UI.Asset.Navigator.PrimaryCriteriaContainer = QuoteFlow.Views.Base.extend({
    el: '.search-criteria',

    initialize: function () {
        this._criteriaViews = _.map(this.collection.fixedLozenges, function (criteria) {
            var el = $('li.' + criteria.id, this.$el);
            AJS.$(el).scrollLock(".aui-list-scroll");

            return new QuoteFlow.UI.Asset.Navigator.Criteria({
                el: el,
                model: new QuoteFlow.Model.Asset.Criteria(criteria),
                searcherCollection: this.collection
            });
        }, this);
    },

//    render: function () {
//        _.each(this._criteriaViews, function (a) {
//            a.render();
//        });
//        this.$el.prepend(_.pluck(this._criteriaViews, "el"));
//    },

    getCriteriaViews: function () {
        return this._criteriaViews;
    },

    getFocusables: function () {
        return this.$(".criteria-selector, #searcher-query, .add-criteria, .search-button");
    },

    getFocusableForCriteria: function (a) {
        return this.$('.criteria-selector[data-id="' + a + '"]');
    }
})