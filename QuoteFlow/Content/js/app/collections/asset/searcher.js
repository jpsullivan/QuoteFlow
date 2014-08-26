QuoteFlow.Collection.Asset.Searcher = Brace.Collection.extend({
    model: QuoteFlow.Model.Asset.Searcher,

    namedEvents: ["searchRequested", "collectionChanged", "jqlTooComplex", "textFieldChanged", "requestUpdateFromView", "interactiveChanged", "beforeCriteriaRemoved"],
    QUERY_PARAM: "text",
    QUERY_ID: "text",
    JQL_INVALID_QUERY_PREFIX: "__jql_",

    initialize: function (b, a) {
        if (b && b.length) {
            this._searcherCache = b;
        }
        this.fixedLozenges = a && a.fixedLozenges ? a.fixedLozenges : [];
        this.queryStateModel = a && a.queryStateModel;
        this.initData = a && a.initData;
        this._interactive = true;
    },

    _createItemDescriptors: function (a) {
        return _.map(a, function (b) {
            return new AJS.ItemDescriptor({ meta: { isShown: b.isShown }, label: b.name, title: b.isShown ? b.name : b.name + " " + "is not applicable for the current project and/or issue type.", selected: b.isSelected || b.jql, value: b.id });
        });
    },

    getAddMenuGroupDescriptors: function () {
        this._updateSearcherCache();
        var d = this.fixedLozengeIds().concat(this.QUERY_ID);
        var a = _.filter(this._searcherCache, function (e) {
            var g = _.contains(d, e.id);
            var f = !!e.groupName;
            return !g && f;
        });
        if (a.length) {
            var b = new AJS.GroupDescriptor({
                label: "All Criteria", items: this._createItemDescriptors(_.sortBy(a, function (e) {
                    return e.name.toLowerCase();
                }))
            });
            var c = new AJS.GroupDescriptor({
                label: "Recent Criteria", items: this._createItemDescriptors(_.first(_.sortBy(_.filter(a, function (e) {
                    return e.lastViewed;
                }), function (e) {
                    return -e.lastViewed;
                }), AJS.Meta.getNumber("max-recent-searchers")))
            });
            return [c, b];
        } else {
            return [];
        }
    },

    getSearcher: function (b) {
        var a = this.get(b);
        if (!a) {
            this.add(this._searcherCache[b]);
            a = this.get(b);
        }
        return a;
    },

    setJql: function (b, a) {
        this._addOrSet(b, { jql: a });
    },

    createJql: function () {
        var a = this.pluck("jql");
        return _.filter(a, _.isNotBlank).join(" AND ") || "";
    },

    isDirty: function () {
        return this.any(function (a) {
            return a.getJql() !== undefined && a.getJql() !== "";
        });
    },

    clearSearchState: function () {
        this.each(function (a) {
            a.clearSearchState();
        });
        this._querySearchersAndValues("");
    },

    clearClause: function (c) {
        var b = this.get(c);
        var a = b && b.hasClause();
        if (b) {
            b.clearSearchState();
        }
        this.triggerCollectionChanged();
        this.triggerRequestUpdateFromView();
        if (a) {
            this.triggerSearchRequested(this.createJql());
        }
    },

    getTextQuery: function () {
        var a = this.get(this.QUERY_ID);
        return a ? a.getViewHtml() : "";
    },

    setInteractive: function (a) {
        if (a !== this._interactive) {
            this._interactive = a;
            this.triggerInteractiveChanged(a);
        }
    },

    isInteractive: function () {
        return this._interactive;
    },

    handleBasicViewSubmit: function () {
        this.triggerRequestUpdateFromView();
        this.triggerSearchRequested(this.createJql());
    },

    updateTextQuery: function (a) {
        if (a) {
            var b = AJS.escapeHtml(a);
            this._addOrSet(this.QUERY_ID, { viewHtml: b, editHtml: b, jql: JIRA.Issues.TextQueryBuilder.buildJql(a) });
        } else {
            this.remove(this.QUERY_ID);
        }
        this.triggerTextFieldChanged();
    },

    getQueryString: function () {
        var a = [];
        this.each(function (c) {
            var b = c.getQueryString();
            if (b) {
                a.push(b);
            }
        });
        return a.join("&");
    },

    _addOrSet: function (e, d, b) {
        var a = this.get(e);
        if (a) {
            if (b && b.parse && a.parse) {
                d = a.parse(d);
                delete b.parse;
            }
            if (d.jql) {
                d.isSelected = true;
            }
            a.set(d, b);
        } else {
            var c = _.clone(d);
            c.id = e;
            c.isSelected = !!d.jql;
            this.add(c, b);
            a = this.get(e);
        }
        return a;
    },

    restoreFromQuery: function (a, d) {
        this.queryStateModel.setJql(a);
        var c = { jql: this.queryStateModel.getJql() || "", decorator: "none" };
        if (this.initData) {
            if (!this.initData.errorMessages) {
                this._onQuerySearchersAndValues(this.initData);
                this.initData = null;
                return jQuery.Deferred().resolve();
            } else {
                this._handleSearcherError(c.jql, this.initData);
                this.initData = null;
                return jQuery.Deferred().reject();
            }
        } else {
            var b = AJS.$.ajax({
                url: AJS.contextPath() + "/secure/QueryComponent!Jql.jspa",
                headers: { "X-SITEMESH-OFF": true },
                data: c,
                type: "POST"
            });
            b.success(_.bind(function (e) {
                if (d) {
                    this.clearExpectingUpdate(d);
                }
                this._onQuerySearchersAndValues(e);
            }, this));
            b.error(_.bind(function (h) {
                if (d) {
                    this.clearExpectingUpdate();
                }
                try {
                    var f = JSON.parse(h.responseText);
                    if (f) {
                        this._handleSearcherError(c.jql, f);
                    }
                } catch (g) {
                    console.log("search response error - not JSON?");
                }
            }, this));
            b.always(function () {
                JIRA.trace("jira.search.searchers.updated");
            });
            return b;
        }
    },

    clearExpectingUpdate: function () {
        this.reset();
        this.updateTextQuery("");
    },

    _handleSearcherError: function (a, b) {
        if (_.include(b.errorMessages, "jqlTooComplex") || _.include(b.errorMessages, "jqlInvalid")) {
            window.setTimeout(_.bind(function () {
                this.triggerJqlTooComplex(a);
            }, this), 0);
        }
    },

    searcherAffectsContext: function (a) {
        return "project" === a || "issuetype" === a;
    },

    createOrUpdateClauseWithQueryString: function (c, b) {
        this.triggerRequestUpdateFromView();
        var a;
        if (this.searcherAffectsContext(c)) {
            a = this._querySearchersAndValues(this.getQueryString());
        } else {
            a = this._querySearchersByValue(c);
        }
        a.done(_.bind(function () {
            if ((this.queryStateModel.getBasicAutoUpdate() || b) && !this.containsInvalidSearchers()) {
                this.triggerSearchRequested(this.createJql());
            }
        }, this));
        return a;
    },

    containsInvalidSearchers: function () {
        return this.any(function (a) {
            return a.hasErrorInEditHtml();
        });
    },

    _querySearchersByValue: function (d) {
        var a = this.get(d);
        var c = AJS.$.param({ decorator: "none", jqlContext: this.queryStateModel.getJql() });
        if (a) {
            var b = a.getQueryString();
            if (b) {
                c = c + "&" + b;
            }
        }
        if (this._activeSearcherReq) {
            this._activeSearcherReq.abort();
        }
        return this._activeSearcherReq = JIRA.SmartAjax.makeRequest({
            type: "POST",
            data: c,
            processData: false,
            url: contextPath + "/secure/QueryComponentRendererValue!Default.jspa",
            success: _.bind(function(f) {
                var e = this.get(d);
                if (e) {
                    if (f[d]) {
                        f[d].groupName = e.getGroupName();
                        f[d].groupId = e.getGroupId();
                    } else {
                        f[d] = _.extend(e.toJSON(), { editHtml: null, jql: null, viewHtml: null });
                    }
                }
                this._setSearchersFromData(f, true);
            }, this),
            dataType: "json",
            error: function(e) {
                JIRA.Issues.displayFailSearchMessage(e);
            }
        }).always(_.bind(function() {
            this._activeSearcherReq = null;
        }, this));
    },

    _parseSearcherGroups: function (c) {
        var a = {};
        var b = this.queryStateModel.getWithout();
        _.each(c.groups, function (d) {
            _.each(d.searchers, function (e) {
                if (!_.contains(b, e.id)) {
                    e.groupId = d.type;
                    e.groupName = d.title;
                    a[e.id] = e;
                }
            });
        });
        return a;
    },

    _querySearchersAndValues: function (b) {
        var a = "decorator=none";
        if (this._activeSearcherReq) {
            if (this._activeSearcherQuery === b) {
                return jQuery.Deferred().reject();
            }
            this._activeSearcherReq.abort();
        }
        this._activeSearcherQuery = b;
        if (b) {
            a += "&" + b;
        }
        this._activeSearcherReq = AJS.$.ajax({ url: contextPath + "/secure/QueryComponent!Default.jspa", headers: { "X-SITEMESH-OFF": true }, type: "POST", data: a, processData: false });
        this._activeSearcherReq.done(_.bind(function (c) {
            this._onQuerySearchersAndValues(c);
        }, this));
        this._activeSearcherReq.fail(_.bind(function (c) {
            JIRA.Issues.displayFailSearchMessage(c);
        }, this));
        this._activeSearcherReq.always(_.bind(function () {
            this._activeSearcherReq = null;
        }, this));
        return this._activeSearcherReq;
    },

    clear: function () {
        this.reset([], { silent: true });
        this.updateTextQuery("");
        this.triggerCollectionChanged();
    },

    searchersReady: function () {
        if (this._activeSearcherReq) {
            return this._activeSearcherReq;
        } else {
            return jQuery.Deferred().resolve();
        }
    },

    _onQuerySearchersAndValues: function (c) {
        var d = this, a = this._parseSearcherGroups(c.searchers);
        _.each(c.values, _.bind(function (f, g) {
            var e = a[g];
            if (!e) {
                a[g] = f;
            } else {
                _.extend(e, f);
            }
        }, this));
        var b = [];
        this.each(function (f) {
            var e = f.id;
            if (!_.any(a, function (g) {
                return g.id === e;
            })) {
                if (f.getIsSelected()) {
                    f.setValidSearcher(false);
                } else {
                    b.push(f);
                }
            }
        });
        if (b.length) {
            this.remove(b);
        }
        this._setSearchersFromData(a);
    },

    _updateSearcherCache: function () {
        if (this._searcherCache) {
            this.each(_.bind(function (a) {
                this._searcherCache[a.id] = a.toJSON();
                this._searcherCache[a.id].id = a.id;
            }, this));
        }
    },

    _setSearchersFromData: function (a, b) {
        _.each(a, _.bind(function (c, d) {
            this._addOrSet(d, { groupId: c.groupId, groupName: c.groupName, isShown: c.isShown, name: c.name, viewHtml: c.viewHtml, jql: c.jql, editHtml: c.editHtml, validSearcher: c.validSearcher, key: c.key, lastViewed: c.lastViewed }, { parse: true });
        }, this));
        if (b) {
            this._updateSearcherCache();
        } else {
            this._searcherCache = a;
        }
        this.triggerCollectionChanged();
        JIRA.trace("jira.search.searchers.updated");
    },

    getVariableClauses: function () {
        var a = [];
        this.each(_.bind(function (b) {
            if (!this.isFixed(b) && b.hasClause()) {
                a.push(b);
            }
        }, this));
        return a;
    },

    getSelectedCriteria: function () {
        var a = this;
        var b = function (c) {
            return !a.isFixed(c) && (c.hasClause() || c.getIsSelected());
        };
        return this.chain().filter(b).sortBy(function (c) {
            return c.getPosition();
        }).value();
    },

    getAllSelectedCriteriaCount: function () {
        return this.getAllSelectedCriteria().length;
    },

    getAllSelectedCriteria: function () {
        var a = [];
        this.each(_.bind(function (b) {
            if (b.hasClause() || b.getIsSelected()) {
                a.push(b);
            }
        }, this));
        return a;
    },

    isFixed: function (a) {
        return _.contains(this.fixedLozengeIds(), a.getId()) || a.getId() === this.QUERY_ID;
    },

    fixedLozengeIds: function () {
        return _.pluck(this.fixedLozenges, "id");
    },

    getNextPosition: function () {
        return this.reduce(function (b, c) {
            var a = Math.max(b, c.getPosition());
            return isNaN(a) ? b : a;
        }, -1) + 1;
    }
})