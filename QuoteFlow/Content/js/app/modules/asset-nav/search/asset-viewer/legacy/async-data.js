"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Brace = require('backbone-brace');

var AsyncData = Brace.Evented.extend({
    namedEvents: ["change"],

    /**
     * @param options
     * @param {Boolean} options.disableCache A flag to indicate whether to cache the data
     */
    initialize: function(options) {
        this.data = {};
        this.pendings = {};
        this.accessIndex = 0;

        if (options) {
            this.disableCache = options.disableCache;
            this.maxCacheSize = options.maxCacheSize;
        }
    },

    /**
     * Get item from the cache if possible, otherwise fetch it.
     *
     * If forceFetch is true, always fetch the latest data and override any pending request.
     *
     * If the fetch succeeds, updates the item and metadata in cache.
     * If the fetch fails, the last successful value is kept (null if there is no previous success),
     * and metadata is updated to indicate an error.
     *
     * @return {$.Promise} resolves when fetch() succeeds or fails, and cache has been updated.
     */
    get: function(id, forceFetch, options) {
        var deferred = new $.Deferred();
        var data = this.disableCache ? {} : this.data; // Keep a reference to the data object at the time the get() is called

        var resolveWith = _.bind(function(meta, additionalMeta) {
            var changedData = this._set(data, id, _.defaults(meta, {accessIndex: this.accessIndex++}), true, options);

            // combinedMeta is what is passed to the deferred callbacks
            var combinedMeta = _.defaults({changed: changedData}, additionalMeta, data[id]);
            if (options && options.mergeIntoCurrent) {
                combinedMeta.mergeIntoCurrent = true;
            }

            if (data[id] && !data[id].error) {
                deferred.resolve(data[id].value, combinedMeta, options);
            } else {
                deferred.reject(data[id] && data[id].value, combinedMeta, options);
            }
        }, this);

        var resolveAfterFetch = _.bind(function(initialLoad) {
            var fetchPromise = this.fetch(id, options);

            this.setPending(id, fetchPromise, function(value) {
                resolveWith({
                    value: value,
                    error: false,
                    updated: new Date()
                }, {initialLoad: initialLoad});
            }, function(value) {
                var meta = {
                    error: true,
                    updated: new Date()
                };
                if (value !== undefined) {
                    meta.value = value;
                }
                resolveWith(meta, {
                    initialLoad: initialLoad,
                    error: true
                });
            });
        }, this);

        var resolveFromCache = function() {
            resolveWith({}, {fromCache: true});
        };

        var resolveAfterPending = _.bind(function() {
            this.pendings[id].always(_.bind(resolveWith, this, {}, {
                // resolveAfterPending is used to resolve deferreds that have piggy-backed onto a fetch() call that's
                // already in progress (i.e. a "pending"). therefore, the correct thing is to resolve the deferred with
                // meta.initialLoad=true, since it is also getting the data returned from the initial cache load.
                initialLoad: true
            }));
        }, this);

        // if caching is disabled then always fetch.
        if (forceFetch || this.disableCache) {
            resolveAfterFetch(false);
        } else if (this.hasCached(id)) {
            resolveFromCache();
        } else if (this.hasPending(id)) {
            resolveAfterPending();
        } else {
            resolveAfterFetch(true);
        }

        return deferred.promise();
    },

    setPending: function(id, task, doneCallback, failCallback) {
        if (this.hasPending(id)) {
            this.pendings[id].update(task);
        } else {
            this.pendings[id] = new $.ConcurrentDeferred(task);
        }
        return this.pendings[id].done(function(value) {
            doneCallback(value);
        }).fail(function(value) {
            failCallback(value);
        });
    },

    /**
     * Get id by value
     *
     * @return {String|null} id of the value. Can be null to indicate not found.
     */
    getIdByValue: function(value) {
        var id;
        for (id in this.data) {
            if (this.data[id].value === value) {
                return id;
            }
        }
        return null;
    },

    /**
     * Retrieves item as stored in the cache, with attached metadata.
     * @return {
     *     value: value
     *     updated: {Date},
     *     accessIndex: {Number},
     *     error: {Boolean}
     * }
     */
    getMeta: function(id) {
        return this.data[id] || {};
    },

    getAllCached: function() {
        var obj = {};
        _.each(this.data, function(value, id) {
            obj[id] = value.value;
        });
        return obj;
    },

    /**
     * @return {Boolean} whether or not a value/error for the given id is already in the cache
     */
    hasCached: function(id) {
        var meta = this.getMeta(id);
        return meta.value !== undefined || meta.error === true;
    },

    /**
     * @return {Boolean} whether or not the given id has an error set on it
     */
    hasError: function(id) {
        var meta = this.getMeta(id);
        return meta.error === true;
    },

    /**
     * @return {Boolean} whether or not a request is currently in progress for the given id
     */
    hasPending: function(id) {
        var pending = this.pendings[id];
        return pending ? pending.isPending() : false;
    },

    /**
     * Sets a value in the cache.
     * Triggers a change event if the value or the error state has changed.
     *
     * @return {Boolean} whether the value changed
     */
    set: function(id, value) {
        return this._set(this.data, id, {
            value: value,
            updated: new Date()
        }, true);
    },

    /**
     * Remove item from the cache
     * @param id
     * @return {Boolean} whether an item was removed
     */
    remove: function(id) {
        if (this.data[id]) {
            delete this.data[id];
            this.triggerChange();
            return true;
        }
        return false;
    },

    /**
     * Sets the error flag and optionally the value for the given item in the cache
     * Triggers a change event if the error state has changed.
     *
     * @param id
     * @param {Object|null} value - optionally change the value at the same time
     *
     * @return {Boolean} whether the error state or value changed
     */
    setError: function(id, value) {
        var meta = {
            error: true,
            updated: new Date()
        };
        if (value !== undefined) {
            meta.value = value;
        }
        return this._set(this.data, id, meta, true);
    },

    /**
     * Sets multiple values at once, triggering at most one change event
     * @param {Object} map of id:meta, where:
     *     id: {String} id
     *     meta: {Object} meta, containing at least one of:
     *         value: {Object}
     *         error: {Object}
     * @return {Boolean} whether any value changed
     */
    setMultiple: function(map) {
        var updated = new Date();
        var changed = false;
        _.each(map, _.bind(function(meta, id) {
            meta = _.pick(meta, 'value', 'error');
            meta.updated = updated;
            if (this._set(this.data, id, meta, false)) {
                changed = true;
            }
        }, this));
        if (changed) {
            this.triggerChange();
        }
        return changed;
    },

    /**
     * Resets the cached data. Optionally, ids and values can be passed in to set initial values.
     */
    reset: function(map) {
        this.data = {};
        this.pendings = {};
        this.accessIndex = 0;
        if (map) {
            this.setMultiple(map);
        } else {
            this.triggerChange();
        }
    },

    /**
     * @param {Object} data
     * @param {String} id
     * @param {Object} metaValues
     * @param {Boolean} triggerChange - trigger change event if:
     *    - value or error is changed, AND
     *    - data === this.data
     * @return {Boolean} whether the value changed
     *
     * Note: Why are we passing in `data` and not just using `this.data`?
     *  - get() is asynchronous, so by the time it resolves, reset() may have been called
     *  - reset() replaces `this.data` with a new object
     *  - when get() resolves it shouldn't add its results to the new `data`,
     *    but it should have access to the `data` at the time it was initiated.
     */
    _set: function(data, id, metaValues, triggerChange, options) {
        if (this.disableCache && data === this.data) {
            data = {}; // Don't modify the cache
        }

        var item = data[id] ? _.clone(data[id]) : {};

        var changedData = this.mergeFetchedAndCached(item, metaValues, options ? options : {}, data[id]);

        if (triggerChange && (!data[id] || changedData || data[id].error !== item.error) && data === this.data) {
            this.triggerChange();
        }

        if (!data[id] && this.maxCacheSize && _.keys(data).length === this.maxCacheSize) {
            if (metaValues.accessIndex) {
                this._dropItem(data);
                data[id] = item;
            }
        } else {
            data[id] = item;
        }

        return changedData;
    },

    _dropItem: function(data) {
        // Drop the least-recently accessed item
        var leastRecentKey = null;
        for (var key in data) {
            if (!data[key].accessIndex) {
                leastRecentKey = key;
                break;
            } else if (leastRecentKey === null || data[key].accessIndex < data[leastRecentKey].accessIndex) {
                leastRecentKey = key;
            }
        }
        if (leastRecentKey !== null) {
            delete data[leastRecentKey];
        }
    },

    /**
     * Override to specify fetch behaviour.
     * @return {jQuery.Promise}
     */
    fetch: function(id) {
        var meta = this.getMeta(id);
        var deferred = new jQuery.Deferred();
        if (!this.hasCached(id) || meta.error) {
            deferred.reject(meta.value);
        } else {
            deferred.resolve(meta.value);
        }
        return deferred.promise();
    },

    mergeFetchedAndCached: function(toResolve, fetched, options, inCache) {
        _.extend(toResolve, fetched);

        return !inCache || inCache.value !== toResolve.value || inCache.error !== toResolve.error;
    }
});

module.exports = AsyncData;
