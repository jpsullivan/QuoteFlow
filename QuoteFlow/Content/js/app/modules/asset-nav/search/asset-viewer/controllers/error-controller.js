"use strict";

var Marionette = require('backbone.marionette');

/**
 * Controller for the error pages.
 * @extends Marionette.Controller
 */
var ErrorController = Marionette.Controller.extend({
    /**
     * @event before:render
     * Fired before rendering an error
     */

    /**
     * @event render
     * Fired after rendering an error
     */

    /**
     * @constructor
     * @param {Object} options
     * @param {string} options.contextPath ContextPath used to access the REST resources
     * @param {boolean|function} [options.showReturnToSearchOnError=false] Whether the error views should display a 'Return to Search' link
     */
    initialize: function (options) {
        this.contextPath = options.contextPath;
        this.showReturnToSearchOnError = options.showReturnToSearchOnError;
    },

    /**
     * Renders an error
     *
     * @param {string} type Error type, valid values are: "auth", "forbidden", "notfound" or "generic"
     * @param {string} issueKey IssueKey, used in the message of "auth" error
     */
    render: function (type, issueKey) {
        switch (type) {
            case "auth":
                this._renderErrorAuth(issueKey);
                break;

            case "forbidden":
                this._renderErrorView(new JIRA.Components.IssueViewer.Views.ErrorNoPermission());
                break;

            case "notfound":
                this._renderErrorView(new JIRA.Components.IssueViewer.Views.ErrorNotFound({
                    showReturnToSearchOnError: this.showReturnToSearchOnError
                }));
                break;

            case "generic":
            default:
                this._renderErrorView(new JIRA.Components.IssueViewer.Views.ErrorGeneric());
                break;
        }
    },

    /**
     * Loads a view from server-rendered markup
     *
     * @param {string} type Error type, valid values are: "auth", "forbidden", "notfound" or "generic"
     * @param {string} issueKey IssueKey, used in the message of "auth" error
     */
    applyToDom: function (type, issueKey) {
        switch (type) {
            case "auth":
                this._renderErrorAuth(issueKey);
                break;

            case "forbidden":
                this._renderErrorViewFromDom(new JIRA.Components.IssueViewer.Views.ErrorNoPermission());
                break;

            case "notfound":
                this._renderErrorViewFromDom(new JIRA.Components.IssueViewer.Views.ErrorNotFound({
                    showReturnToSearchOnError: this.showReturnToSearchOnError
                }));
                break;

            case "generic":
            default:
                this._renderErrorViewFromDom(new JIRA.Components.IssueViewer.Views.ErrorGeneric());
                break;
        }
    },

    /**
     * Closes and deletes the view
     */
    close: function () {
        if (this.view) {
            this.view.close();
            delete this.view;
        }
    },

    /**
     * Changes the element where the view should be rendered
     *
     * @param {jQuery} element
     */
    setElement: function (element) {
        this._$el = element;
        if (this.view) {
            this.view.setElement(element);
        }
    },

    /**
     * Renders an authentication error. There is no view for this case, we just redirect the user to the login page
     *
     * @param {string} issueKey Issue key that raised the error
     * @private
     */
    _renderErrorAuth: function (issueKey) {
        // Redirect to the login page; will destroy stable search. This isn't ideal, but redirecting to
        // /browse/JRA-123 (as we were previously) results in a redirect loop in IE as we redirect to /i...
        var URL = this.contextPath + "/login.jsp?os_destination=" + encodeURIComponent("/browse/" + issueKey);
        window.location.replace(URL);
    },

    /**
     * Renders an error view.
     *
     * @param {JIRA.Components.IssueViewer.Views.Error} view View that needs to be rendered
     * @private
     */
    _renderErrorView: function (view) {
        this.trigger("before:render");

        JIRA.Components.IssueViewer.Utils.hideDropdown();

        this.view = view;
        this.view.setElement(this._$el);
        this.view.render();
        this.listenAndRethrow(this.view, "returnToSearch");

        this.trigger("render", {
            pager: this.view.$(this.view.pager.el)
        },{
            loadedFromDom: false,
            issueId: null
        });
    },

    /**
     * Renders an error view.
     *
     * @param {JIRA.Components.IssueViewer.Views.Error} view View that needs to be rendered
     * @private
     */
    _renderErrorViewFromDom: function (view) {
        this.trigger("before:render");

        JIRA.Components.IssueViewer.Utils.hideDropdown();

        this.view = view;
        this.view.setElement(this._$el);
        this.view.applyToDom();
        this.listenAndRethrow(this.view, "returnToSearch");

        this.trigger("render", {
            pager: this.view.$(this.view.pager.el)
        },{
            loadedFromDom: true,
            issueId: null
        });
    }
});

module.exports = ErrorController;
