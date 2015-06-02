"use strict";

var _ = require('underscore');
var InlineLayer = require('../../../../components/layer/inline-layer');
var Reasons = require('../../util/reasons');
var SearcherEditDialogView = require('./searcher-edit-dialog-view');

/**
 * A singleton that reuses the same InlineLayer for many different criteria selectors.
 */
var SearcherDialog = function () {

    return {
        initialize: function(options) {
            if (!this.instance) {
                this.instance = initSearcherDialog(options);
            }
        }
    };

    function initSearcherDialog(options) {
        AJS.HIDE_REASON.switchLozenge = "switchLozenge";

        var reasonsToFocusCriteria = [
            AJS.HIDE_REASON.escPressed,
            AJS.HIDE_REASON.toggle,
            AJS.HIDE_REASON.cancelClicked,
            AJS.HIDE_REASON.submit,
            AJS.HIDE_REASON.tabbedOut
        ];

        function findLozenge(searcher) {
            return searcher ? jQuery(".criteria-selector[data-id='" + searcher.getId() + "']") : jQuery();
        }

        var promise;
        var _currentSearcher; // JIRA.Issues.SearcherModel
        var _currentView;

        var _dialog = new InlineLayer({
            width: "auto",
            alignment: "left",
            offsetTarget: function () {
                // Don't cache the lozenge under the current implementation since it can get reblatted
                return findLozenge(_currentSearcher);
            },
            // Uses AJS.DeferredContentRetriever.
            content: function () {
                _currentView = new SearcherEditDialogView({
                    model: _currentSearcher,
                    queryStateModel: options.queryStateModel
                });
                _currentView.onHideRequested(function(reason) {
                    _dialog.hide(reason);
                });
                return _currentView.renderDeferred();
            }
        });

        _dialog.bind(InlineLayer.EVENTS.show, function(e, $layer) {
            var input = $layer.find(":input:not(submit):visible:first");
            input.focus();

            // List.js also resets the scrollTop but because the dialog is still hidden at that point, the browser won't actually do any scrolling.
            // @see JRADEV-15097
            $layer.find(".aui-list-scroll").scrollTop(0);
        });

        _dialog.bind(InlineLayer.EVENTS.beforeHide, function (e, layer, reason, id, originalTarget) {
            if (reason === AJS.HIDE_REASON.clickOutside && originalTarget && jQuery(originalTarget).closest(".calendar").length) {
                e.preventDefault();
            }
        });

        _dialog.bind(InlineLayer.EVENTS.hide, function (e, layer, reason) {
            var _searcher = _currentSearcher;

            if (_.contains(reasonsToFocusCriteria, reason)) {
                findLozenge(_searcher).focus();
            }

            function doSearch() {
                // A searcher has been submitted. Create a clause and add it to the clause collection
                promise = _currentSearcher.createOrUpdateClauseWithQueryString(reason === AJS.HIDE_REASON.submit);
                promise.done(function() {
                    // Check for an "error" class in the searcher's editHtml.
                    // If so, leave the dialog open and rerender to the the updated editHtml.
                    // Otherwise close the dialog.
                    if (_searcher.hasErrorInEditHtml()) {
                        // Prevent displaying the error dialog if the mode has switched to advanced as a
                        // result of the click outside (i.e. on the Switch to Advanced link)
                        if (options.queryStateModel.getSearchMode() === "basic") {
                            SearcherDialog.instance.show(_searcher);
                        }
                    }

                    if (InlineLayer.current) {
                        // page layout might have changed as a result of updating a lozenge content (need to make sure dialog is in correct position)
                        InlineLayer.current.setPosition();
                    }
                });

                promise.always(function() {
                    promise = null;
                });

                _searcher.clearEditHtml();
            }

            if (reason === AJS.HIDE_REASON.submit) {
                // If this is a submit, always do de search
                _currentView.applyFilter();
                doSearch();
            } else if (reason !== AJS.HIDE_REASON.cancelClicked && reason !== AJS.HIDE_REASON.escPressed &&
                reason !== AJS.HIDE_REASON.tabbedOut) {
                // If the dialog has not been cancelled, do the search only if the selector has not changed.
                if (_currentView.applyFilter()) {
                    doSearch();
                }
            } else {
                console.log("quoteflow.search.searchers.hiddenWithoutUpdate");
            }

            _currentView.$el.remove();
            _currentView = null;
            _currentSearcher = null;
        });

        /**
         * JRADEV-15697 - Ensure all the dialog is closed when we switch search modes.
         */
        options.queryStateModel.on("change:searchMode", function() {
            _dialog.hide();
        });

        return  {
            /**
             * @return {SearcherModel} the searcher for which the dialog was last shown.
             */
            getCurrentSearcher: function () {
                return _currentSearcher;
            },

            /**
             * Hide and show dialog
             */
            toggle: function (searcher) {
                if (_currentSearcher != null) {
                    if (_currentSearcher !== searcher) {
                        _dialog.hide(AJS.HIDE_REASON.switchLozenge);
                        this.show(searcher);
                    } else {
                        _dialog.hide(AJS.HIDE_REASON.toggle);
                    }
                } else {
                    this.show(searcher);
                }
            },

            _show: function (searcher) {
                _.defer(function() {
                    _currentSearcher = searcher;
                    _dialog.show();
                    _dialog.setPosition();
                });
            },

            /**
             * shows dialog with correct searcher
             * @param {SearcherModel} searcher
             */
            show: function (searcher) {
                var waitingToShow;
                if (SearcherDialog.waitingToShow) {
                    waitingToShow = true;
                }

                var instance = this;
                SearcherDialog.waitingToShow = function () {
                    instance._show(searcher);
                    SearcherDialog.waitingToShow = null;
                };

                if (!waitingToShow) {
                    // Defer showing until all searchers are ready.
                    searcher.searchersReady().done(function () {
                        if (promise) {
                            promise.done(function () {
                                SearcherDialog.waitingToShow();
                            });
                        } else {
                            SearcherDialog.waitingToShow();
                        }
                    });
                }
            },

            /**
             * Bind a handler to be called when the dialog is shown.
             *
             * @param handler The function to call when the dialog is shown.
             */
            onShow: function (handler) {
                _dialog.bind(InlineLayer.EVENTS.show, handler);
            },

            /**
             * Hide the current dialog. Drrrrr!
             */
            hide: function () {
                _dialog.hide();
            },

            /**
             * Bind a handler to be called when the dialog is hidden.
             *
             * @param handler The function to call when the dialog is hidden.
             */
            onHide: function (handler) {
                _dialog.bind(InlineLayer.EVENTS.hide, handler);
            }
        }
    }
}();

module.exports = SearcherDialog;
