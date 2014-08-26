QuoteFlow.Module.Asset.JqlQuery = Brace.Evented.extend({
    namedEvents: ["searchRequested", "verticalResize"],

    initialize: function(a) {
        this._queryStateModel = a.queryStateModel;

//        this.view = new JIRA.Issues.JqlQueryView({
//            queryStateModel: a.queryStateModel
//        })
//        .onVerticalResize(this.triggerVerticalResize, this)
//        .onSearchRequested(this.triggerSearchRequested, this);
//
//        JIRA.bind(JIRA.Events.NEW_CONTENT_ADDED, _.bind(function (d, b, c) {
//            if (c === JIRA.CONTENT_ADDED_REASON.returnToSearch) {
//                this.setQuery();
//            }
//        }, this));
    },

    search: function () {
        //var a = this.view.readJql();
        this.triggerSearchRequested(a);
    },

    setQuery: function () {
        this.view.setQuery();
    }
});