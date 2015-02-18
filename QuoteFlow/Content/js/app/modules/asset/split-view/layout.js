"use strict";

var Marionette = require('backbone.marionette');

var Utilities = require('../../../components/utilities');

/**
 * Controller/view for detail view layout.
 */
var SplitScreenLayout = Marionette.ItemView.extend({

    /**
     * @param {object} options
     * @param {JIRA.Issues.FullScreenIssue} options.fullScreenIssue The application's <tt>FullScreenIssue</tt>.
     * @param {JIRA.Components.IssueViewer} options.issueModule The application's <tt>IssueModule</tt>.
     * @param {JIRA.Issues.SearchModule} options.search The layout's interface to the rest of the application.
     * @param {jQuery} options.searchContainer The element in which search results are to be rendered.
     */
    initialize: function (options) {
        _.bindAll(this,
            "_adjustHeight",
            "_adjustNoResultsMessageHeight",
            "_updateSidebarPosition",
            "applyResponsiveDesign");

        if (options.easeOff) {
            this.applyResponsiveDesign = jQuery.noop;
            _.debounce(this._adjustHeight, options.easeOff);
        }

        options = _.extend(options, {
            serverRendered: this._consumeServerRenderedSplitView(options)
        });

        this.search = options.search;
        this.fullScreenIssue = options.fullScreenIssue;
        this.navigatorContent = options.searchContainer.find(".navigator-content");
        this.searchResults = options.search.getResults();

        this.emptyResultsView = new JIRA.Issues.EmptyResultsView({
            searchResults: this.searchResults,
            el: this.navigatorContent
        });

        this.detailsView = new JIRA.Issues.SplitScreenDetailView(options);
        this.listView = new JIRA.Issues.SplitScreenListView(options);

        this.orderBy = JIRA.Components.OrderBy.create();
        this.orderBy.onSort(this._handleSort, this);

        Utilities.initializeResizeHooks();

        QuoteFlow.Interactive.onVerticalResize(this._adjustHeight);
        QuoteFlow.Interactive.onVerticalResize(this._adjustNoResultsMessageHeight);
        QuoteFlow.Interactive.onHorizontalResize(this._updateSidebarPosition);
        QuoteFlow.Interactive.onVerticalResize(this._updateSidebarPosition);
        this.addListener(options.search, "beforeSearch", this._showPending, this);
        this.addListener(this.searchResults, "issueDeleted", this._onIssueDeleted, this);
        this.addListener(this.searchResults, "newPayload", this._hidePending, this);
        this.addListener(this.searchResults, "newPayload", this._updateSortBy, this);
        this.addListener(this.searchResults, "newPayload", this.render, this);
        this.addListener(this.searchResults, "startIndexChange", this._onStartIndexChange, this);
        this.addListener(this.searchResults, "highlightedIssueChange", this._onHighlightedIssueChange, this);
        this.addListener(this.searchResults, "selectedIssueChange", this._onSelectedIssueChange, this);
        this.addListener(this.searchResults, "startIndexChange", this._renderEverythingExceptListView, this);
        this.search.onSearchError(this._onSearchFail, this);

        JIRA.Issues.Application.on("issueEditor:loadError", this._onIssueLoadError, this);
        this.listView.searchPromise.done(_.bind(function () {
            this._makeVisible();
        }, this));

        this.fullScreenIssue.hide();
        JIRA.Issues.overrideScrollIntoViewForSplit();

        this.setElement(this.navigatorContent);
        if (options.serverRendered) {
            this._activateSubviews();
        }

        this._scrollLayoutOnZoom = this._initScrollLayoutOnZoom();
        if (this._scrollLayoutOnZoom) {
            jQuery(window).on('resize', this._scrollLayoutOnZoom);
        }

        this._updateSortBy();
    },


    _updateSortBy: function () {
        this.orderBy.setJql(this.search.getEffectiveJql());
    },

    _handleSort: function (jql) {
        this.search.doSort(jql);
    },

    /**
     * Recalculate the list's height.
     *
     * @private
     */
    _adjustHeight: function () {
        var $listPanel = this.$el.find(".list-panel");
        var endOfStableSearchMessageHeight = 0,
            offsetTop = this.listView.$el.offset().top + $listPanel.scrollTop(),
            paginationHeight = this.$el.find(".pagination-container").outerHeight(), // NOTE: this is the pagination container, not the pagination view!
            windowHeight = window.innerHeight;

        if (this.endOfStableSearchView && this.endOfStableSearchView.$el) {
            endOfStableSearchMessageHeight = this.endOfStableSearchView.$el.outerHeight();
        }

        $listPanel.css("height", windowHeight - offsetTop - endOfStableSearchMessageHeight - paginationHeight);
    },

    /**
     * Adjust the height of the no results message so it fills the screen.
     *
     * @private
     */
    _adjustNoResultsMessageHeight: function () {
        var navigatorContentTop;
        if (this.searchResults.hasIssues()) {
            this.navigatorContent.css("height", "");
        } else {
            navigatorContentTop = this.navigatorContent.offset().top;
            this.navigatorContent.css("height", window.innerHeight - navigatorContentTop);
        }
    },

    // TF-38, JRA-34879 - Zooming in IE causes parts of page to disappear in split view.
    _initScrollLayoutOnZoom: function () {
        if (jQuery.browser.msie) {
            this.cachedDPI = screen.deviceXDPI;
            return _.bind(function () {
                if (this.cachedDPI !== screen.deviceXDPI) {
                    jQuery("body, html").scrollTop(0);
                }
                this.cachedDPI = screen.deviceXDPI;
            }, this);
        }
        return null;
    },

    /**
     * Consume server rendered split view markup, preventing it from being used again.
     *
     * @param {object} options
     * @param {jQuery} options.searchContainer
     * @return {boolean} <tt>true</tt> iff there was markup to consume.
     * @private
     */
    _consumeServerRenderedSplitView: function (options) {
        var hasConsumed = !!AJS.Meta.get("consumed-server-rendered-split-view"),
            hasExisting = !!options.searchContainer.find(".split-view").length;

        AJS.Meta.set("consumed-server-rendered-split-view", true);
        return !hasConsumed && hasExisting;
    },

    /**
     * Prepare to be removed, deactivating all subviews.
     */
    close: function () {
        QuoteFlow.Interactive.offVerticalResize(this._adjustHeight);
        QuoteFlow.Interactive.offVerticalResize(this._adjustNoResultsMessageHeight);
        QuoteFlow.Interactive.offHorizontalResize(this._updateSidebarPosition);
        QuoteFlow.Interactive.restoreScrollIntoViewForNormal();
        jQuery("body").removeClass("page-type-split");
        JIRA.Issues.Application.off("issueEditor:loadError", this._onIssueLoadError, this);
        this.navigatorContent.addClass("pending").css("height", "");
        this.orderBy.offSort(this._handleSort, this);
        this.search.offSearchError(this._onSearchFail, this);

        if (this._isIOS()) {
            this._deactivateIOSSpecificBehaviour();
        }

        // *first* deactivate the subviews. this ensures that they stop receiving change events, which
        // is important because we are about to modify the SearchResults after this.
        this.detailsView.deactivate();
        this.listView.deactivate();

        jQuery(window).off('resize', this.applyResponsiveDesign);
        if (this._scrollLayoutOnZoom) {
            jQuery(window).off('resize', this._scrollLayoutOnZoom);
            delete this._scrollLayoutOnZoom;
        }

        JIRA.Issues.BaseView.prototype.deactivate.apply(this, arguments);

        //If the selected issue is not in the list of downloaded issues go to first issue in page.
        if (!this.searchResults.hasIssue(this.searchResults.getSelectedIssue().getId())) {
            this.searchResults.selectFirstInPage();
        }
    },

    _handleInitialIssueSelection: function () {
        if (!this.searchResults.hasSelectedIssue()) {
            if (this.searchResults.hasHighlightedIssue()) {
                this.searchResults.selectIssueById(this.searchResults.getHighlightedIssue().getId(), { replace: true });
            } else {
                this.searchResults.selectFirstInPage({ replace: true });
            }
        }
    },

    _hidePending: function () {
        this.navigatorContent.removeClass("pending");
    },

    _isInitialRender: function () {
        return !this.navigatorContent.find(".split-view").length;
    },

    _applyWidthClass: function () {
        var width = this.detailsView.$el.width();
        this.$el.toggleClass("skinny", width < 900);
        this.$el.toggleClass("very-skinny", width < 600);
    },

    _makeVisible: function () {
        this.navigatorContent.removeClass("pending");
        if (this._isInitialRender()) {
            this.navigatorContent.html(this.$el);
        }
        this.$el.find(".list-results-panel").sidebar({
            id: "splitview",
            minWidth: function (ui) {
                return 250;
            },
            maxWidth: _.bind(function () {
                return this.$el[0].clientWidth - 500;
            }, this),
            resize: this.applyResponsiveDesign
        });
        jQuery(window).on('resize', this.applyResponsiveDesign);

        //TF-729: Ensure we put the appropriate width classes on first render to ensure the sidebar picks up the correct width.
        this._applyWidthClass();

        JIRA.trigger(JIRA.Events.LAYOUT_RENDERED);
        this.applyResponsiveDesign();
    },

    _updateSidebarPosition: function () {
        var $sidebar = this.$el.find(".list-results-panel");
        $sidebar.sidebar("updatePosition");
    },

    applyResponsiveDesign: function () {
        if (this.reapplyResponsive) {
            clearTimeout(this.reapplyResponsive);
        }
        this.reapplyResponsive = setTimeout(_.bind(function () {
            this._applyWidthClass();
            clearTimeout(this.reapplyResponsive);
            delete this.reapplyResponsive;
        }, this), 0);
    },

    /**
     * Synchronise the highlighted and selected issue.
     *
     * @param {JIRA.Issues.SimpleIssue} model The application's highlighted issue model.
     * @param {object} [options]
     * @param {boolean} [options.replace=true] Whether selecting the issue should be a "replace" operation.
     * @private
     */
    _onHighlightedIssueChange: function (model, options) {
        options = _.defaults({}, options, {
            replace: true
        });

        var highlightedIssueID = this.searchResults.getHighlightedIssue().getId();
        this.searchResults.selectIssueById(highlightedIssueID, options);
    },

    /**
     * Render the issue list or empty results message after issue deletion.
     *
     * @private
     */
    _onIssueDeleted: function (issue) {
        //Locally hide the issue to be deleted immediately so there is no delay in the UI before the new issue table
        this.listView.getIssueById(issue.id).hide();
        this._showPending();
        if (this.searchResults.hasIssues()) {
            this.listView.render();
            this._renderPagination();
            this._renderEndOfStableSearch();
            this._renderRefreshResults();
        } else {
            this._renderEmptyResults();
        }
    },

    /**
     * Handle the failure to load an issue.
     * <p/>
     * Update the issue list, etc.
     *
     * @param {object} entity
     * @param {number} entity.issueId The issue's ID.
     * @param {string} entity.issueKey The issue's key.
     * @param {object} entity.pager Pager data.
     * @param {object} entity.response The data returned from the cache/server.
     */
    _onIssueLoadError: function (entity) {
        if (entity.response.status === 404) {
            this.listView.markIssueInaccessible(entity.issueId);
        }
    },

    _onSearchFail: function () {
        this._renderEmptyResults();
        this._hidePending();
    },

    _onSelectedIssueChange: function () {
        this._setIssueModuleContainer();
    },

    _onStartIndexChange: function () {
        // If there are no results, the "No Results" message has already been rendered.
        if (this.searchResults.hasIssues()) {
            this._showPending();
        }
    },

    /**
     * Called when the 'j' keyboard shortcut is used and this layout is active.
     */
    nextIssue: function () {
        this.searchResults.selectNextIssue({ replace: true });
    },

    /**
     * Called when the 'k' keyboard shortcut is used and this layout is active.
     */
    prevIssue: function () {
        this.searchResults.selectPrevIssue({ replace: true });
    },

    /**
     * Called when a pagination link is clicked
     */
    goToPage: function (startIndex) {
        // TODO: Layout shouldn't use behaviour on this model. Should trigger event for Module to handle.
        this.searchResults.goToPage(startIndex);
    },

    refreshSearch: function () {
        if (JIRA.Issues.Application.request("issueEditor:canDismissComment")) {
            JIRA.Issues.Application.execute("analytics:trigger", "kickass.issueTableRefresh");
            JIRA.Issues.Application.execute("issueNav:refreshSearch");
        }
    },

    /**
     * Render the layout.
     * <p/>
     * Some subviews render asynchronously.
     *
     * @return {JIRA.Issues.SplitScreenLayout} <tt>this</tt>
     */
    render: function () {
        var hasIssues = this.searchResults.hasIssues(),
            isInitialRender = this._isInitialRender();

        jQuery("body").addClass("page-type-split");

        if (this._isIOS()) {
            this._activateIOSSpecificBehaviour();
        }

        if (hasIssues) {
            this._handleInitialIssueSelection();
            this.navigatorContent.removeClass("empty-results");

            // if this is the initial render then we need to create the structure for the list and details view
            // to render into before handling initial issue selection.
            if (isInitialRender) {
                this.$el.children().detach();
                this.$el.html(JIRA.Templates.SplitView.structure());
            }

            this._renderPagination();
            this._renderEndOfStableSearch();
            this._renderRefreshResults();

            this._activateSubviews();
            this.detailsView.render();
            this.listView.render();
        } else {
            this._hidePending();
            this._renderEmptyResults();
        }

        this._adjustHeight();
        this._adjustNoResultsMessageHeight();
        this.orderBy.render();
        return this;
    },

    _renderPagination: function () {
        if (this.paginationView) {
            this.paginationView.close();
        }
        this.paginationView = new JIRA.Components.IssueNavigator.Views.Pagination({
            startIndex: this.searchResults.getStartIndex(),
            pageSize: this.searchResults.getPageSize(),
            total: this.searchResults.getTotal(),
            currentSearch: JIRA.Issues.Application.request("issueNav:currentSearchRequest")
        });
        this.listenTo(this.paginationView, "goToPage", this.goToPage);
        this.paginationView.render();
        this.$(".pagination-container").empty().append(this.paginationView.$el);
    },

    _renderEndOfStableSearch: function () {
        if (this.endOfStableSearchView) {
            this.endOfStableSearchView.close();
        }
        this.endOfStableSearchView = new JIRA.Components.IssueNavigator.Views.EndOfStableMessage({
            total: this.searchResults.getTotal(),
            displayableTotal: this.searchResults.getDisplayableTotal(),
            pageNumber: this.searchResults.getPageNumber(),
            numberOfPages: this.searchResults.getNumberOfPages()
        });
        this.endOfStableSearchView.render();
        this.$(".end-of-stable-message-container").empty().append(this.endOfStableSearchView.$el);
    },


    _renderRefreshResults: function () {
        if (this.refreshResultsView) {
            this.refreshResultsView.close();
        }
        this.refreshResultsView = new JIRA.Components.IssueNavigator.Views.RefreshResults();
        this.listenTo(this.refreshResultsView, "refresh", this.refreshSearch);
        this.refreshResultsView.render();
        this.$(".refresh-container").empty().append(this.refreshResultsView.$el);
    },

    /**
     * Exists because list view listens for events that *just so happen* to mean that a new set of results came in.
     * But we want to know that, too! Oh well, we'll untangle that later.
     * TODO: Consolidate the re-render entry points. When searchresults fires its startIndexChanged and highlightedIssueChange events, those are smells.
     * @private
     */
    _renderEverythingExceptListView: function () {
        var hasIssues = this.searchResults.hasIssues();
        if (hasIssues) {
            this._renderPagination();
            this._renderEndOfStableSearch();
            this._renderRefreshResults();

            this._adjustHeight();
            this._adjustNoResultsMessageHeight();
        }
    },

    _renderEmptyResults: function () {
        this.emptyResultsView.render();
    },

    /**
     * Set the issue container element so issues render correctly.
     *
     * @private
     */
    _setIssueModuleContainer: function () {
        JIRA.Issues.Application.execute("issueEditor:setContainer", this.$(".split-view .detail-panel > div"));
    },

    _showPending: function () {
        this.navigatorContent.addClass("pending");
    },

    /**
     * Calls setElement on all the sub-views and activates them if necessary.
     *
     * @private
     */
    _activateSubviews: function () {
        this.detailsView.setElement(this.$(".detail-panel")).activate();
        this.orderBy.setElement(this.$('.list-ordering'));
        this.listView.setElement(this.$(".list-content"));
        this._setIssueModuleContainer();
    },

    handleLeft: function () {
        this.detailsView.blur();
    },

    handleRight: function () {
        this.detailsView.focus();
    },

    isIssueViewActive: function () {
        return this.detailsView.hasFocus();
    }
});

module.exports = SplitScreenLayout;