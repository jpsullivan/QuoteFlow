"use strict";

var _ = require('underscore');

/* module-key = 'com.atlassian.jira.jira-issue-nav-plugin:common', location = 'content/js/util/Sidebar.js' */
/*!
 * jQuery UI Sidebar
 * http://jqueryui.com
 *
 * Depends:
 *   jquery.ui.widget.js
 */
(function(a) {
    a.widget("ui.sidebar", {

        version: "0.1",

        options: {
            minWidth: function(b) {
                return 50;
            },
            maxWidth: function(b) {
                return window.innerWidth;
            },
            resize: a.noop,
            easeOff: 0
        },

        _create: function () {
            debugger;
            _.bindAll(this, "_handleDrag", "_persist", "_setContainment", "_setHandlePosition", "updatePosition");
            if (this.options.easeOff) {
                this.updatePosition = _.debounce(this.updatePosition, this.options.easeOff);
            }

            if (!this.options.id) {
                console.error("ui.sidebar: You must specify an id");
            }

            this._addHandle();
            this._restore();
            a(window).resize(_.debounce(this.updatePosition, 30));
        },

        _restore: function () {
            if (window.localStorage) {
                var b = localStorage.getItem("ui.sidebar." + this.options.id);
                if (b) {
                    this._setWidth(b);
                }
            }
        },

        _persist: function () {
            if (window.localStorage) {
                localStorage.setItem("ui.sidebar." + this.options.id, this.element.outerWidth());
            }
        },

        _setContainment: function () {
            var b = window.innerHeight;
            this._elementLeft = this.element.offset().left;
            this._minLeft = this._elementLeft + this.options.minWidth(this);
            this._maxLeft = Math.max(this._minLeft, this._elementLeft + this.options.maxWidth(this));
            this.handle.draggable({ containment: [this._minLeft, b, this._maxLeft, b] });
        },

        _handleDrag: function (d, b) {
            var c = b.position.left - this._elementLeft;
            this._setWidth(c, true);
        },

        _setWidth: function (e, d) {
            if (!d) {
                var c = this.options.maxWidth(this);
                var b = this.options.minWidth(this);
                if (e > c) {
                    e = c;
                } else {
                    if (e < b) {
                        e = b;
                    }
                }
            }
            e -= this.element.outerWidth() - this.element.width();
            this.element.width(e);
            this.options.resize(e);
        },

        _addHandle: function () {
            var b = document.createElement("div");
            b.setAttribute("class", "ui-sidebar");
            this.handle = a(b).appendTo(this.element);
            this.handle.draggable({ axis: "x", drag: this._handleDrag, stop: this._persist });
            this.handle.mousedown(this._setContainment);
            _.defer(this._setHandlePosition);
        },

        _setHandlePosition: function () {
            this._setContainment();
            if (this._minLeft === this._maxLeft) {
                this.handle.hide();
            } else {
                var c = this.element.offset();
                var b = c.left + this.element.outerWidth();
                this.handle.css({ top: c.top, left: b, height: this.element.outerHeight() }).show();
            }
        },

        updatePosition: function() {
            debugger;
            this._setHandlePosition();
            this._setWidth(this.handle.offset().left - this._elementLeft);
            this._persist();
        }
    });
})(window.jQuery);
