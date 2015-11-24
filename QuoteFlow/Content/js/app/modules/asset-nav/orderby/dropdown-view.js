"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Brace = require('backbone-brace');
var Marionette = require('backbone.marionette');

var CheckboxMultiSelect = require('../../../components/select/checkbox-multi-select');
var EventTypes = require('../util/types');
var GroupDescriptor = require('../../../components/list/group-descriptor');
var InlineLayer = require('../../../components/layer/inline-layer');
var ItemDescriptor = require('../../../components/list/item-descriptor');
var SelectSuggestHandler = require('../../../components/select/select-suggest-handler');
/**
 * A pop-up where we display the new sort options.
 * @type {OrderByDropdownView}
 * @extends {Brace.View}
 */
var OrderByDropdownView = Brace.View.extend({
    /**
     * Creates a new OrderByDropDownView.
     *
     * @param {object} options
     * @param {SearchModule} options.search the SearchModule
     * @param {Element} options.offsetTarget the element that the drop-down will be positioned under
     */
    initialize: function (options) {
        _.bindAll(this, '_onDropdownHidden', '_onDropdownShown');
        this.offsetTarget = options.offsetTarget;
        this._onHideCallback = options.onHideCallback;
        this._pending = null;
        this._inlineLayer = null;
    },

    /**
     * Hides this OrderByDropDownView and cancels any pending AJAX requests.
     */
    deactivate: function() {
        this._inlineLayer && this._inlineLayer.hide();
        Marionette.prototype.deactivate.apply(this, arguments);
    },

    /**
     * Toggles the visibility of this OrderByDropDownView, cancelling any pending AJAX requests when appropriate.
     */
    toggle: function() {
        // depending on whether the InlineLayer is visible or not we either
        // cancel pending requests or kick off new ones.
        if (!this._inlineLayer) {
            this._pending = $.ajax(this._ajaxDefaults());
            this._inlineLayer = this._createInlineLayer(this._pending);
            this._inlineLayer.bind(InlineLayer.EVENTS.show, this._onDropdownShown);
            this._inlineLayer.bind(InlineLayer.EVENTS.hide, this._onDropdownHidden);
            this._inlineLayer.show();
        } else {
            this._inlineLayer.hide();
        }
    },

    /**
     * Creates the drop-down InlineLayer with the single select inside of it.
     *
     * @param deferred
     * @return {AJS.InlineLayer}
     * @private
     */
    _createInlineLayer: function(deferred) {
        var renderSelectAndAddTipsy = _.bind(function(data) {
            this._deferredAddTipsyToFooter(data);
            return this._renderInitialContent(data);
        }, this);

        var inlineLayer = new InlineLayer({
            offsetTarget: this.offsetTarget,
            alignment: "left",
            content: function() { return deferred.pipe(renderSelectAndAddTipsy);  },
            width: 218
        });

        if (this._onHideCallback) {
            inlineLayer.bind(InlineLayer.EVENTS.hide, this._onHideCallback);
        }

        return inlineLayer;
    },

    /**
     * Enhances the order by single select to use a sparkler instead of a plain old HTML select list.
     *
     * @param {AJS.InlineLayer} layer the layer that contains the select
     * @private
     */
    _initSingleSelect: function (layer) {
        var orderBySelect = layer.find('select');

        // we are using a sparkler as if it were a single select here. to do this we
        // programatically hide the sparkler whenever the user clicks one of the options.
        orderBySelect.bind("selected", _.bind(this._onFieldSelected, this));

        this.selectControl = new CheckboxMultiSelect({
            element: orderBySelect,
            itemAttrDisplayed: "label",
            suggestionsHandler: SelectSuggestHandler,
            ajaxOptions: this._ajaxDefaults({
                data: _.bind(function(query) {
                    return JSON.stringify(this._ajaxOptionsData(query));
                }, this),
                query: true, // keep going back to the sever for each keystroke
                minQueryLength: 0,
                formatResponse: _.bind(function (response) {
                    this._removeTipsy();

                    var sortFields = new GroupDescriptor();

                    // add each returned field
                    _.each(response.fields, function(option) {
                        sortFields.addItem(new ItemDescriptor({
                            title: option.fieldName,
                            value: option.fieldId,
                            label: option.fieldName,
                            meta: { sortJql: option.sortJql }
                        }));
                    });

                    // add a footer showing how many more fields there are
                    var footerText = this._getFooterText(response);
                    if (footerText) {
                        sortFields.footerText(footerText);
                        this._deferredAddTipsyToFooter(response);
                    }

                    // we only have 1 group
                    return [ sortFields ];
                }, this)
            })
        });

        // focus the 'Search' area in the multi-select after rendering it
        var orderByInput = layer.find('#order-by-options-input');
        orderByInput.focus();
    },

    /**
     * Returns the default AJAX options for requesting suggestions.
     *
     * @param {object} ajaxOpts additional options that override the defaults
     */
    _ajaxDefaults: function(ajaxOpts) {

        var defaults = {
            url: QuoteFlow.RootUrl + "/api/orderby/orderByOptions",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(this._ajaxOptionsData())
        };

        return _.extend(defaults, ajaxOpts);
    },

    /**
     * Returns the additional data to send back in the options request.
     *
     * @param {string} query the query
     * @return {Object} data to send
     * @private
     */
    _ajaxOptionsData: function(query) {
        var jql = this.model.getJql();
        var sortBy = this.model.getSortBy();
        return {
            query: query,
            jql: jql ? jql : undefined,
            sortBy: sortBy ? sortBy.fieldId : undefined
        };
    },

    /**
     * Renders the initial content for the InlineLayer.
     *
     * @param orderByOptions the options returned from the server
     * @return {jQuery} a div with the initial content
     * @private
     */
    _renderInitialContent: function(orderByOptions) {
        // the 'meta' needs to be stringified for inclusion in the select
        var optionsWithMeta = _.map(orderByOptions.fields, function(field) {
            return _.extend({}, field, {
                meta: JSON.stringify({ sortJql: field.sortJql })
            });
        });

        return $('<div></div>')
                .addClass('order-dropdown')
                .html(JST["quote-builder/split-view/orderby-options"]({ options: optionsWithMeta, footer: this._getFooterText(orderByOptions) }));
    },

    /**
     * Initialises the single select control after the data has been inserted into the DOM.
     *
     * @param e
     * @param layer
     * @param id
     *
     * @private
     */
    _onDropdownShown: function(e, layer, id) {
        this._initSingleSelect(layer);
    },

    /**
     * Cleans up after the drop-down when it gets hidden. This includes removing the drop-down from the DOM.
     *
     * @param e
     * @param {AJS.InlineLayer} layer the layer
     * @param id
     * @private
     */
    _onDropdownHidden: function(e, layer, id) {
        this._removeTipsy();

        // remove element
        $('div.order-dropdown').remove();
        this._inlineLayer = null;
    },

    /**
     * Hides the drop-down and sorts by the selected field
     */
    _onFieldSelected: function(e, descriptor) {
        // it seems we can't just hide the InlineLayer because it is still trying to do things
        // *after* this callback has run and will fail if we've hidden it. so we defer.
        var event = new $.Event(EventTypes.ASSET_TABLE_REORDER);
        QuoteFlow.trigger(event);
        if (!event.isDefaultPrevented()) {
            _.defer(_.bind(function () {
                // handling for bug in QueryableDropdownSelect - it incorrectly fires a request on enter key.
                if (this.selectControl.outstandingRequest) {
                    this.selectControl.outstandingRequest.reject();
                }
                InlineLayer.current.hide();
                // sort using the provided sort JQL
                var meta = descriptor.meta();
                this.model.doSort(meta.sortJql);
            }, this));
        }
    },


    /**
     * Returns the footer string to render in the dropdown, or undefined. If there are more fields to show than were
     * returned, the footer will read along the lines of "23 more fields not shown".
     *
     * @param {object} orderByOptions
     * @param {number} orderByOptions.maxResults the maximum number of returned fields
     * @param {number} orderByOptions.matchesCount the number of matched fields
     * @return {string}
     * @private
     */
    _getFooterText: function(orderByOptions) {
        if (typeof orderByOptions.matchesCount === 'number' && typeof orderByOptions.maxResults === 'number') {
            var notShown = orderByOptions.matchesCount - orderByOptions.maxResults;
            if (notShown > 0) {
                return notShown + " more fields...";
            }
        }

        return "";
    },

    /**
     * Adds a tipsy to the footer telling users how to find fields that are not in the list. The actual adding of the
     * tipsy is deferred.
     *
     * @param {object} orderByOptions
     * @param {number} orderByOptions.maxResults the maximum number of returned fields
     * @param {number} orderByOptions.matchesCount the number of matched fields
     * @return {*}
     * @private
     */
    _deferredAddTipsyToFooter: function(orderByOptions) {
        if (this._inlineLayer) {
            var matched = orderByOptions.matchesCount;
            var shown = Math.min(orderByOptions.matchesCount, orderByOptions.maxResults);
            var tipsyText = "Showing " + shown + " of " + matched + " matching fields. Find more fields by typing in the search box.";

            var instance = this;
            // _.defer(function() {
            //     instance._tipsyTrigger = new JIRA.Issues.Tipsy({
            //         el: instance._inlineLayer.$layer.find('.aui-list-section-footer'),
            //         tipsy: {
            //             title: function() { return tipsyText; },
            //             className: "tipsy-front"
            //         }
            //     });
            // });
        }
    },

    /**
     * Removes the footer tipsy.
     *
     * @return {*}
     * @private
     */
    _removeTipsy: function() {
        if (this._tipsyTrigger) {
            this._tipsyTrigger.hide();
            this._tipsyTrigger = null;
        }
    }
});

module.exports = OrderByDropdownView;
