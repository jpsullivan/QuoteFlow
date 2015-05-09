"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

var AssetSearcherModel = require('../../models/asset/searcher');
var GroupDescriptor = require('../../components/list/group-descriptor');
var ItemDescriptor = require('../../components/list/item-descriptor');

/**
 *
 */
var AssetSearcherCollection = Brace.Collection.extend({
    model: AssetSearcherModel,

    /**
     * searchRequested: a user action has caused us to select a new search
     *
     * collectionChanged: the collection has changed. This is fired at most once per public interface method. Clients
     * requesting changes should listen to this method to receive updates instead of add, change, remove if they
     * want only a single notification of change.
     *
     * requestUpdateFromView: requests non-auto-submit views (eg text query view) to update the form values in their model
     *
     * beforeCriteriaRemoved: a user action caused a criteria to be removed from view. Args: id, direction
     */
    namedEvents: ["searchRequested", "collectionChanged", "jqlTooComplex", "textFieldChanged", "requestUpdateFromView", "interactiveChanged", "beforeCriteriaRemoved"],

    QUERY_PARAM: "text",
    QUERY_ID: "text",

    /**
     * Prefix used to send jql for invalid searchers to the server, because we don't generate editHtml for invalid searchers
     * (but we can generate jql)
     */
    JQL_INVALID_QUERY_PREFIX: "__jql_",

    initialize: function (models, options) {
        if (models && models.length) {
            this._searcherCache = models;
        }
        this.fixedLozenges = options && options.fixedLozenges ? options.fixedLozenges : [];
        this.queryStateModel = options && options.queryStateModel;
        this.initData = options && options.initData;
        this._interactive = true;
    },

    /**
     * Converts an array of SearcherModels to an array of ItemDescriptors.
     *
     * @param {Array<SearcherModel>} searchers
     * @return {Array<ItemDescriptor>}
     */
    _createItemDescriptors: function (searchers) {
        return _.map(searchers, function (searcher) {
            return new ItemDescriptor({
                meta: {
                    isShown: searcher.isShown
                },
                label: searcher.name,
                title: searcher.isShown ? searcher.name : searcher.name + " is not applicable for the current catalog and/or manufacturer.",
                selected: searcher.isSelected || searcher.jql,
                value: searcher.id
            });
        });
    },

    /**
     * Construct group and item descriptors for the "add criteria" menu.
     * <p/>
     * Returns valid non-primary searchers sorted by name (case insensitive).
     *
     * @return {Array} GroupDescriptors for the "add criteria" menu.
     */
    getAddMenuGroupDescriptors: function () {
        // Retrieve all valid, non-primary searchers.
        this._updateSearcherCache();

        var primarySearcherIds = this.fixedLozengeIds().concat(this.QUERY_ID);

        var searchers = _.filter(this._searcherCache, function (searcher) {
            var isPrimary = _.contains(primarySearcherIds, searcher.id);
            var isValid = !!searcher.groupName;
            return !isPrimary && isValid;
        });

        if (searchers.length) {
            var allSearchers = new GroupDescriptor({
                label: "All Criteria",
                items: this._createItemDescriptors(_.sortBy(searchers, function (searcher) {
                    return searcher.name.toLowerCase();
                }))
            });
            var recentSearchers = new GroupDescriptor({
                label: "Recent Criteria",
                items: this._createItemDescriptors(_.first(_.sortBy(_.filter(searchers, function (searcher) {
                    return searcher.lastViewed;
                }), function (searcher) {
                    return -searcher.lastViewed;
                }), AJS.Meta.getNumber("max-recent-searchers")))
            });
            return [recentSearchers, allSearchers];
        } else {
            return [];
        }
    },

    /**
     * Returns a searcher by id from either the model collection or the raw searcher cache.
     *
     * If a model is not found in the model collection it is added from the raw cache and returned.
     *
     * Assumes that a searcher being requested, if not present in the model collection, is in the raw searcher
     * collection. This is done as the raw searchers are used to generate the searcher list.
     *
     * @param id The Searcher ID
     */
    getSearcher: function (id) {
        var searcher = this.get(id);

        if (!searcher) {
            this.add(this._searcherCache[id]);
            searcher = this.get(id);
        }

        return searcher;
    },

    /**
     * Sets the jql for the model with the given id, creating one if it doesn't exist
     * @param id id of model
     * @param jql jql to set
     */
    setJql: function (id, jql) {
        this._addOrSet(id, {
            jql: jql
        });
    },

    /**
     * Returns a single jql string expressing all subclauses in this collection.
     */
    createJql: function () {
        var arr = this.pluck("jql");
        return _.filter(arr, _.isNotBlank).join(" AND ") || "";
    },

    /**
     * Has the user specified any clauses?
     */
    isDirty: function () {
        return this.any(function (lozenge) {
            // clauses are ultimately defined by a jql clause
            return lozenge.getJql() !== undefined && lozenge.getJql() !== ""; // TODO the stupid text field strikes again. it has a "" state after routing
        });
    },

    /**
     * Clears entire search state
     */
    clearSearchState: function () {
        this.each(function (searcherModel) {
            searcherModel.clearSearchState();
        });
        // TODO: optimise by only requerying server if search state changes
        this._querySearchersAndValues("");
    },

    /**
     * Clear the search state for a single searcher.
     *
     * If the searcher has a clause (i.e. it could be affecting search results),
     * the search will be re-performed.
     *
     * @param id The id of searcher.
     */
    clearClause: function (id) {
        // We don't need to requery here if invalid clauses
        var searcher = this.get(id);
        var hasClause = searcher && searcher.hasClause();

        if (searcher) {
            searcher.clearSearchState();
        }

        this.triggerCollectionChanged();
        this.triggerRequestUpdateFromView();

        // We only need to refresh results if the searcher had a clause.
        if (hasClause) {
            this.triggerSearchRequested(this.createJql());
        }
    },

    getTextQuery: function () {
        var model = this.get(this.QUERY_ID);
        return model ? model.getViewHtml() : "";
    },

    /**
     * Set the interactive flag to indicate whether searchers respond to user input.
     * @param {boolean} interactive
     */
    setInteractive: function (interactive) {
        if (interactive !== this._interactive) {
            this._interactive = interactive;
            this.triggerInteractiveChanged(interactive);
        }
    },

    /**
     * Determine whether searchers respond to user input.
     * @return {boolean}
     */
    isInteractive: function () {
        return this._interactive;
    },

    handleBasicViewSubmit: function () {
        this.triggerRequestUpdateFromView();
        this.triggerSearchRequested(this.createJql());
    },

    updateTextQuery: function (textQuery) {
        if (textQuery) {
            var textQueryHtml = AJS.escapeHtml(textQuery);
            this._addOrSet(this.QUERY_ID, {
                viewHtml: textQueryHtml,
                editHtml: textQueryHtml,
                jql: JIRA.Issues.TextQueryBuilder.buildJql(textQuery)
            });
        }
        else {
            this.remove(this.QUERY_ID);
        }
        this.triggerTextFieldChanged();
    },

    /**
     * Creates a queryString representing all querystring members
     * @return {string}
     */
    getQueryString: function () {
        var queryStrings = [];
        this.each(function (searcherModel) {
            var qs = searcherModel.getQueryString();
            if (qs) {
                queryStrings.push(qs);
            }
        });
        return queryStrings.join("&");
    },

    /**
     * Adds or sets parameters. If a model with the given id is found, the values in params are set. Otherwise a model is created
     * with the given id and params.
     * @param id id to of model to find.
     * @param params parameters
     * @param options
     */
    _addOrSet: function (id, params, options) {
        var model = this.get(id);
        if (model) {
            // Backbone doesn't support parse: true for set()
            if (options && options.parse && model.parse) {
                params = model.parse(params);
                delete options.parse;
            }
            if (params.jql) {
                params.isSelected = true;
            }
            model.set(params, options);
        } else {
            var paramsWithId = _.clone(params);
            paramsWithId.id = id;
            paramsWithId.isSelected = !!params.jql;
            this.add(paramsWithId, options);
            model = this.get(id);
        }
        return model;
    },

    /**
     * Update the basic mode query view to represent the given jql unless the jql can't fit into the basic view.
     * If the query is "too complex" a jqlTooComplex event is fired.
     * @param {queryStateModel} query
     */
    restoreFromQuery: function (jql, reset) {
        // We won't have JQL if a filter is private; just show empty searchers.
        this.queryStateModel.setJql(jql);

        var requestData = {
            jql: this.queryStateModel.getJql() || "",
            decorator: "none"
        };

        // We store a json blob on the page when we first load so we don't need to go to the server.
        if (this.initData) {
            if (!this.initData.errorMessages) {
                this._onQuerySearchersAndValues(this.initData);
                this.initData = null;
                return jQuery.Deferred().resolve();
            } else {
                // this json blob may also contain errors
                this._handleSearcherError(requestData.jql, this.initData);
                this.initData = null;
                return jQuery.Deferred().reject();
            }

        } else {
            var response = AJS.$.ajax({
                url: QuoteFlow.RootUrl + "/api/asset/QueryComponent",
                headers: { 'X-SITEMESH-OFF': true },
                data: requestData,
                type: "POST"
            });

            response.success(_.bind(function (data) {
                if (reset) {
                    this.clearExpectingUpdate(reset);
                }
                this._onQuerySearchersAndValues(data);
            }, this));

            response.error(_.bind(function (resp) {
                if (reset) {
                    this.clearExpectingUpdate();
                }
                try {
                    var json = JSON.parse(resp.responseText);
                    if (json) {
                        this._handleSearcherError(requestData.jql, json);
                    }
                } catch (e) {
                    console.log("search response error - not JSON?");
                }

            }, this));

            response.always(function () {
              console.log('quoteflow.search.searchers.updated');
            });

            return response;
        }
    },

    /**
     * Resets state, before we apply new values.
     */
    clearExpectingUpdate: function () {
        this.reset();
        this.updateTextQuery("");
    },

    /**
     *  Handles error when requesting searchers
     *
     * @param query
     * @param data
     * @private
     */
    _handleSearcherError: function (jql, data) {
        if (_.include(data.errorMessages, "jqlTooComplex") || _.include(data.errorMessages, "jqlInvalid")) {
            // I know I know. A bloody setTimeout.
            // This is because when we first load the page as we are not async (we are using json blob from page) this
            // event is triggered before switchToPreferred search is called. This means that we flick to advanced but
            // then back to the preferred search (which could be basic). If the jql is too complex we need to be sure

            window.setTimeout(_.bind(function() {
                this.triggerJqlTooComplex(jql);
            }, this), 0);
        }
    },

    searcherAffectsContext: function(id) {
        return "catalog" === id || "manufacturer" === id;
    },

    /**
     * Tells the clause to update from the values selected in its editHtml, creating or updating it as required.
     * This update involves an AJAX request to retrieve the jql and criteria content from the server.
     * @param id -- SearcherModel id
     * @param {Boolean} [forceUpdate=false] Force update of the JQL, even if autoupdate is disabled
     */
    createOrUpdateClauseWithQueryString: function (id, forceUpdate) {

        this.triggerRequestUpdateFromView();

        var deferred;
        if (this.searcherAffectsContext(id)) {
            // Requery all searchers and values
            deferred = this._querySearchersAndValues(this.getQueryString());
        }
        else {
            deferred = this._querySearchersByValue(id);
        }

        deferred.done(_.bind(function () {
            if ((this.queryStateModel.getBasicAutoUpdate() || forceUpdate) && !this.containsInvalidSearchers()) {
                this.triggerSearchRequested(this.createJql());
            }
        }, this));

        return deferred;

        // TODO: could optimise by only requesting all searchers and values when context changes (ie project or issue type)
        // and requesting only valuehtml for other cases. see _querySingleValue
    },

    _querySearchersByValue: function(id) {
        var model = this.get(id);
        var data = AJS.$.param({
            decorator: "none",
            jqlContext: this.queryStateModel.getJql()
        });

        if (model) {
            var modelString = model.getQueryString();
            if (modelString) {
                data = data + '&' + modelString;
            }
        }

        if (this._activeSearcherReq) {
            this._activeSearcherReq.abort();
        }

        return this._activeSearcherReq = JIRA.SmartAjax.makeRequest({
            type: "POST",
            data: data,
            processData: false,
            //url: QuoteFlow.ApplicationPath + "api/asset/FindAsset",
            url: contextPath + "/secure/QueryComponentRendererValue!Default.jspa",
            success:_.bind(function(data) {
                var model = this.get(id);
                if (model) {
                    if (data[id]) {
                        data[id].groupName = model.getGroupName();
                        data[id].groupId = model.getGroupId();
                    } else {
                        // If the searcher isn't present in the response, then
                        // it currently has no value and we need to reset it.
                        data[id] = _.extend(model.toJSON(), {
                            editHtml: null,
                            jql: null,
                            viewHtml: null
                        });
                    }
                }

                this._setSearchersFromData(data, true);
            }, this),
            dataType: "json",
            error: function(xhr) {
                //JIRA.Issues.displayFailSearchMessage(xhr);
            }
        }).always(_.bind(function () {
            this._activeSearcherReq = null;
        }, this));
    },

    /**
     * Returns a map by id of all searchers from the response
     * @param data response data
     */
    _parseSearcherGroups: function (data) {
        var searchers = {};
        var without = this.queryStateModel.getWithout();

        _.each(data.groups, function (group) {
            _.each(group.searchers, function (searcher) {
                if (!_.contains(without, searcher.id)) {
                    searcher.groupId = group.type;
                    searcher.groupName = group.title;
                    searchers[searcher.id] = searcher;
                }
            });
        });

        return searchers;
    },

    _querySearchersAndValues: function (queryString) {
        var data = "decorator=none";

        if (this._activeSearcherReq) {
            // If it is the same as the request we are currently waiting for we can just ignore.
            if (this._activeSearcherQuery === queryString) {
                return jQuery.Deferred().reject();
            }
            // Otherwise we will abort and issue a new request.
            this._activeSearcherReq.abort();
        }

        // store data for this request so we can use it to compare against new requests
        this._activeSearcherQuery = queryString;

        if (queryString) {
            data += "&" + queryString;
        }

        this._activeSearcherReq = AJS.$.ajax({
            url: QuoteFlow.ApplicationPath + "api/asset/QueryComponent",
            //url: contextPath + "/secure/QueryComponent!Default.jspa",
            type: "POST",
            data: data,
            processData: false
        });

        this._activeSearcherReq.done(_.bind(function (data) {
            this._onQuerySearchersAndValues(data);
        }, this));

        this._activeSearcherReq.fail(_.bind(function (xhr) {
            //JIRA.Issues.displayFailSearchMessage(xhr);
        }, this));

        this._activeSearcherReq.always(_.bind(function () {
            this._activeSearcherReq = null;
        }, this));

        return this._activeSearcherReq;
    },

    /**
     * Remove all searchers from the collection and clear the text query.
     */
    clear: function () {
        this.reset([], { silent: true });
        this.updateTextQuery("");
        this.triggerCollectionChanged();
    },

    /**
     * Wait any in flight updates to search collection.
     */
    searchersReady: function () {
        if (this._activeSearcherReq) {
            return this._activeSearcherReq;
        } else {
            return jQuery.Deferred().resolve();
        }
    },

    _onQuerySearchersAndValues: function (data) {

        // merge searchers and values from response
        var collection = this,
            searchers = this._parseSearcherGroups(data.searchers);
        _.each(data.values, _.bind(function (value, id) {
            // compose searcher and value from response
            var searcher = searchers[id];
            if (!searcher) {
                searchers[id] = value;
            }
            else {
                _.extend(searcher, value);
            }
        }, this));

        var modelsToRemove = [];
        this.each(function (searcher) {
            var searcherId = searcher.id;
            if (!_.any(searchers, function (newSearcher) { return newSearcher.id === searcherId; })) {
                if (searcher.getIsSelected()) {
                    searcher.setValidSearcher(false);
                } else {
                    modelsToRemove.push(searcher);
                }
            }
        });
        if (modelsToRemove.length) {
            this.remove(modelsToRemove);
        }

        this._setSearchersFromData(searchers);
    },

    /**
     * Updates the searcher cache to mirror the current state of the searcher collection.
     *
     * @private
     */
    _updateSearcherCache: function () {
        if (this._searcherCache) {
            this.each(_.bind(function (searcher) {
                this._searcherCache[searcher.id] = searcher.toJSON();
                this._searcherCache[searcher.id].id = searcher.id;
            }, this));
        }
    },

    /**
     * Sets the initial state of the searcher collection.
     *
     * Accepts a JSON object of raw searcher definitions and values and adds
     * real SearcherModels to SearcherCollection for each if they are primary or
     * they have viewHtml (i.e. in use).
     *
     * This is an optimisation to prevent every searcher model being instantiated
     * which is an incredibly slow process in IE8.
     *
     * @param searchers
     * @private
     */
    _setSearchersFromData: function (searchers, update) {
        _.each(searchers, _.bind(function (value, id) {
            this._addOrSet(id, {
                groupId: value.groupId,
                groupName: value.groupName,
                isShown: value.isShown,
                name: value.name,
                viewHtml: value.viewHtml,
                jql: value.jql,
                editHtml: value.editHtml,
                validSearcher: value.validSearcher,
                key: value.key,
                lastViewed: value.lastViewed
            }, { parse: true });
        }, this));

        if (update) {
            this._updateSearcherCache();
        } else {
            this._searcherCache = searchers;
        }

        this.triggerCollectionChanged();
    },

    /**
     * Return a list of extended criteria.
     * - the searcher has a value
     * - the searcher is not query text or a primary clause
     */
    getVariableClauses: function () {
        var variableClauses = [];
        this.each(_.bind(function (searcherModel) {
            if (!this.isFixed(searcherModel) && searcherModel.hasClause()) {
                variableClauses.push(searcherModel);
            }
        }, this));

        return variableClauses;
    },

    /**
     * @return {SearcherModel[]} All selected non-prime searchers, in ascending order of position.
     */
    getSelectedCriteria: function () {
        var instance = this;
        var isSelectedCriteria = function (searcher) {
            return !instance.isFixed(searcher) &&
                (searcher.hasClause() || searcher.getIsSelected());
        };

        return this.chain()
            .filter(isSelectedCriteria)
            .sortBy(function (searcher) { return searcher.getPosition(); })
            .value();
    },

    getAllSelectedCriteriaCount: function () {
        return this.getAllSelectedCriteria().length;
    },

    getAllSelectedCriteria: function () {
        var selectedCriteria = [];

        this.each(_.bind(function (searcherModel) {
            if (searcherModel.hasClause() || searcherModel.getIsSelected()) {
                selectedCriteria.push(searcherModel);
            }
        }, this));

        return selectedCriteria;
    },

    isFixed: function (searcherModel) {
        return _.contains(this.fixedLozengeIds(), searcherModel.getId()) || searcherModel.getId() === this.QUERY_ID;
    },

    fixedLozengeIds: function () {
        return _.pluck(this.fixedLozenges, "id");
    },

    /**
     * Extended criteria searchers have position values that are used to
     * determine their position in ExtendedCriteriaView; they appear in
     * ascending order. This method calculates the next position value.
     *
     * @return {number} The next position value.
     */
    getNextPosition: function () {
        return this.reduce(function (memo, searcher) {
            var max = Math.max(memo, searcher.getPosition());
            return isNaN(max) ? memo : max;
        }, -1) + 1;
    }
});

module.exports = AssetSearcherCollection;
