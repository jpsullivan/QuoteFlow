QuoteFlow.Module.Asset.BasicQuery = Brace.Evented.extend({
    namedEvents: ["jqlTooComplex", "searchRequested", "basicModeCriteriaCountWhenSearching", "verticalResize"],

    initialize: function(a) {
        this._queryStateModel = a.queryStateModel;
        this.searcherCollection = new QuoteFlow.Collection.Asset.Searcher([], {
            fixedLozenges: a.primaryClauses,
            queryStateModel: a.queryStateModel,
            initData: a.initialSearcherCollectionState
        });

//        this.view = new JIRA.Issues.BasicQueryView({
//                queryStateModel: a.queryStateModel,
//                searcherCollection: this.searcherCollection
//            })
//            .onVerticalResize(this.triggerVerticalResize, this)
//            .onSearchRequested(this.triggerSearchRequested, this);

        var searcherCollection = this.searcherCollection;
        this.searcherCollection.onSearchRequested(_.bind(function(b) {
            this.triggerBasicModeCriteriaCountWhenSearching({ count: searcherCollection.getAllSelectedCriteriaCount() });
            var c = this._attachOrderByClause(b);
            this.triggerSearchRequested(c);
        }, this));
        this.searcherCollection.onJqlTooComplex(_.bind(function(b) {
            this.triggerJqlTooComplex(b);
        }, this));
    },

    hasErrors: function () {
        var a = this.searcherCollection.any(function(b) {
            return b.hasErrorInEditHtml();
        });
        return a;
    },

    clear: function () {
        this.searcherCollection.clear();
    },

    queryChanged: function () {
        this.searcherCollection.restoreFromQuery(this._queryStateModel.getJql());
    },

    queryReset: function (a) {
        this.searcherCollection.setInteractive(false);
        return this.searcherCollection.restoreFromQuery(a, true).always(_.bind(function() {
            this.searcherCollection.setInteractive(true);
        }, this));
    },

    searchersReady: function () {
        return this.searcherCollection.searchersReady();
    },

    getSelectedCriteria: function () {
        return this.searcherCollection.getAllSelectedCriteria();
    },

    _attachOrderByClause: function (a) {
        var b = /\bORDER\s+BY\b.*$/i;
        var c = b.exec(this._queryStateModel.getJql());
        if (c && b.exec(a) === null) {
            a = a ? a + " " + c[0] : c[0];
        }
        return a;
    }
});