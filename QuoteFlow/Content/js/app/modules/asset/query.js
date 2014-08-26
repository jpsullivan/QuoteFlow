QuoteFlow.Module.Asset.Query = Brace.Evented.extend({
    namedEvents: ["jqlChanged", "jqlTooComplex", "jqlError", "jqlSuccess", "searchRequested", "queryTooComplexSwitchToAdvanced", "changedPreferredSearchMode", "basicModeCriteriaCountWhenSearching", "verticalResize", "initialized"],

    initialize: function (options) {
        this.queryStateModel = options.queryStateModel;
        var queryState = this.queryStateModel;

        this.queryStateModel.on("change:preferredSearchMode", _.bind(function () {
            this.triggerChangedPreferredSearchMode(queryState.getPreferredSearchMode());
        }, this));

        this._jqlQueryModule = new QuoteFlow.Module.Asset.JqlQuery({
             queryStateModel: this.queryStateModel
        })
        .onSearchRequested(this.handleAdvancedSearchRequested, this)
        .onVerticalResize(this.triggerVerticalResize, this);

        this._errors = {};
        this._errors[this.queryStateModel.BASIC_SEARCH] = [];
        this._errors[this.queryStateModel.ADVANCED_SEARCH] = [];
        this.queryStateModel.on("change:searchMode", this.showSearchErrors, this);

        this._basicQueryModule = new QuoteFlow.Module.Asset.BasicQuery({
            queryStateModel: this.queryStateModel,
            primaryClauses: options.primaryClauses,
            initialSearcherCollectionState: options.searchers
        })
        .onSearchRequested(this.clearSearchErrors, this)
        .onJqlTooComplex(this.handleJqlTooComplex, this)
        .onSearchRequested(this.handleSearchRequested, this)
        .onVerticalResize(this.triggerVerticalResize, this)
        .onBasicModeCriteriaCountWhenSearching(this.triggerBasicModeCriteriaCountWhenSearching, this);
    },

    refreshLayout: function () {
        this._jqlQueryModule.setQuery();
    },

    handleAdvancedSearchRequested: function (a) {
        this.handleSearchRequested(a);
        this._basicQueryModule.queryChanged();
    },

    handleSearchRequested: function (a) {
        this.queryStateModel.setJql(a);
        this.clearSearchErrors();
    },

    handleJqlTooComplex: function (a) {
        if (this.getSearchMode() !== this.queryStateModel.ADVANCED_SEARCH) {
            this.triggerQueryTooComplexSwitchToAdvanced();
        }
        this.setSearchMode(this.queryStateModel.ADVANCED_SEARCH);
        this.triggerJqlTooComplex(a);
        if (this._queryView) {
            this._queryView.switcherViewModel.disableSwitching();
        }
    },

    getJql: function () {
        return this.queryStateModel.getJql();
    },

    getSearcherCollection: function () {
        return this._basicQueryModule.searcherCollection;
    },

    isBasicMode: function () {
        return this.queryStateModel.getSearchMode() === this.queryStateModel.BASIC_SEARCH;
    },

    resetToQuery: function (a, b) {
        this.clearSearchErrors();
        return this._basicQueryModule.queryReset(a).always(_.bind(function () {
            this.queryStateModel.switchToPreferredSearchMode();
            this._jqlQueryModule.setQuery();
            if (b && b.focusQuery === true) {
                this._queryView.getView().focus();
            }
            this._basicQueryModule.off("searchRequested", this.publishJqlChanges);
            this._jqlQueryModule.off("searchRequested", this.publishJqlChanges);
            this._basicQueryModule.onSearchRequested(this.publishJqlChanges, this);
            this._jqlQueryModule.onSearchRequested(this.publishJqlChanges, this);
        }, this));
    },

    publishJqlChanges: function (a) {
        this.triggerJqlChanged(a);
    },

    setVisible: function (a) {
        this._queryView.setVisible(a);
    },

    queryChanged: function () {
        this.clearSearchErrors();
        this._basicQueryModule.queryChanged();
    },

    onSearchSuccess: function (a) {
        if (this._queryView) {
            this._queryView.showWarnings(a);
        }
        this.triggerJqlSuccess();
    },

    searchersReady: function () {
        return this._basicQueryModule.searchersReady();
    },

    onSearchError: function (b) {
        this._errors.renderFunction = "showErrors";
        var c = (b.errorMessages) ? b.errorMessages.concat() : [];
        var a = [];
        _.each(b.errors, function (e, d) {
            if (d === "jql") {
                a.push(e);
            } else {
                c.push(e);
            }
        });
        if (this.getSearchMode() === this.queryStateModel.BASIC_SEARCH && !this._basicQueryModule.hasErrors() && a.length > 0) {
            this.setSearchMode(this.queryStateModel.ADVANCED_SEARCH);
        }
        this._errors[this.queryStateModel.BASIC_SEARCH] = c;
        this._errors[this.queryStateModel.ADVANCED_SEARCH] = a.concat(c);
        this.showSearchErrors();
        this.triggerJqlError();
    },

    showSearchErrors: function () {
        if (this._queryView) {
            this._queryView.clearNotifications();
            var a = this._errors.renderFunction || "showErrors";
            this._queryView[a](this._errors[this.getSearchMode()]);
        }
    },

    clearSearchErrors: function () {
        if (this._queryView) {
            this._queryView.clearNotifications();
            this._queryView.switcherViewModel.enableSwitching();
        }
        this._errors[this.queryStateModel.BASIC_SEARCH].length = 0;
        this._errors[this.queryStateModel.ADVANCED_SEARCH].length = 0;
    },

    getSearchMode: function () {
        return this.queryStateModel.getSearchMode();
    },

    getActiveBasicModeSearchers: function () {
        return this._basicQueryModule.getSelectedCriteria();
    },

    setSearchMode: function (a) {
        if (this.getSearchMode() !== a) {
            this.queryStateModel.switchToSearchMode(a);
            return true;
        }
        return false;
    },

    createAndRenderView: function (a) {
        this._queryView = new JIRA.Issues.QueryView({ el: a, queryStateModel: this.queryStateModel, basicQueryModule: this._basicQueryModule, jqlQueryModule: this._jqlQueryModule }).onVerticalResize(this.triggerVerticalResize, this);
        this._queryView.render();
    },

    isQueryValid: function () {
        return (this._errors && this._errors[this.queryStateModel.BASIC_SEARCH].length === 0 && this._errors[this.queryStateModel.ADVANCED_SEARCH].length === 0);
    }
})