QuoteFlow.UI.Asset.Navigator.PrimaryCriteriaContainer = QuoteFlow.Views.Base.extend({
    el: '.search-criteria',

    initialize: function () {
        this._criteriaViews = _.map(this.collection.fixedLozenges, function (criteria) {
            this.setDropdownEventBindings(criteria.id);

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
    },

    setDropdownEventBindings: function (criteria) {
        AJS.$('#' + criteria + '-dropdown').on({
            "aui-dropdown2-show": function (k, i) {
                var currentTarget = $(k.currentTarget);
                var j = currentTarget.find(":input:not(submit):visible:first");
                j.focus();
                currentTarget.find(".aui-list-scroll").scrollTop(0);
            }
        });
    }
})