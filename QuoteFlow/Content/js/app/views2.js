/// <reference path="../lib/jquery.d.ts"/>
/// <reference path="../lib/underscore.d.ts"/>
/// <reference path="../lib/backbone.d.ts"/>
/// <reference path="quoteflow.ts"/>
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};

var BaseView = (function (_super) {
    __extends(BaseView, _super);
    function BaseView(options) {
        _super.call(this, options);
        this.setupRenderEvents();
    }
    BaseView.prototype.setupRenderEvents = function () {
        if (this.model) {
            this.model.bind('remove', this.remove, this);
        }
    };

    BaseView.prototype.presenter = function () {
        return this.defaultPresenter();
    };

    // automatically plugs-in the model attributes into the JST, as well as
    // some site-wide attributes that we define below
    BaseView.prototype.defaultPresenter = function () {
        var modelJson = this.model && this.model.attributes ? _.clone(this.model.attributes) : {};

        var imageUrl;
        if (QuoteFlow.RootUrl === "/") {
            imageUrl = "/Content/images/";
        } else {
            imageUrl = QuoteFlow.RootUrl + "Content/images/";
        }

        return _.extend(modelJson, {
            RootUrl: QuoteFlow.RootUrl,
            ImageUrl: imageUrl
        });
    };

    BaseView.prototype.render = function () {
        this.renderTemplate();
        this.renderSubviews();

        return this;
    };

    BaseView.prototype.renderTemplate = function () {
        var presenter = _.isFunction(this.presenter) ? this.presenter() : this.presenter;
        this.template = JST[this.templateName];
        if (!this.template) {
            console.log(!_.isUndefined(this.templateName) ? ("no template for " + this.templateName) : "no templateName specified");
        }

        this.$el.html(this.template(presenter)).attr("data-template", _.last(this.templateName.split("/")));
        this.postRenderTemplate();
    };

    BaseView.prototype.postRenderTemplate = function () {
        $.noop; // hella callbax yo
    };

    BaseView.prototype.renderSubviews = function () {
        var _this = this;
        _.each(this.subviews, function (property, selector) {
            //var view = _.isFunction(this[property]) ? this[property]() : this[property];
            var view = new function () {
            };
            if (view) {
                if (_.isArray(view)) {
                    // If we pass an array of views into the subviews, append each to the selector.
                    // This should generally only be used when dealing with parent-specific class issues.
                    // For example, if you need to supply a "span*" class on the parent to prevent the
                    // box model from breaking.
                    _.each(view, function (arrayView) {
                        var subView = _.isFunction(arrayView) ? arrayView() : arrayView;
                        if (arrayView) {
                            this.$(selector).append(subView.render().el);
                            subView.delegateEvents();
                        }
                    });
                } else {
                    // drop the view directly into the selector
                    _this.$(selector).html(view.render().el);
                    view.delegateEvents();
                }
            }
        });
    };

    BaseView.prototype.remove = function () {
        if (this.subviews) {
            this.removeSubviews();
        }

        // remove this from the debug array if it exists
        if (QuoteFlow.Debug.Views.length > 0) {
            var debugIndex = _.indexOf(QuoteFlow.Debug.Views, this);
            if (debugIndex > -1) {
                QuoteFlow.Debug.Views.splice(debugIndex, 1);
            }
        }

        // completely unbind the view
        this.undelegateEvents();
        this.off(); // Kills off remaining events

        // Remove the view from the DOM
        return Backbone.View.prototype.remove.apply(this, arguments);
    };

    BaseView.prototype.removeSubviews = function () {
        var children = this.subviews, childViews = [];

        if (!children) {
            return this;
        }

        _.each(children, function (property, selector) {
            //var view = _.isFunction(this[property]) ? this[property]() : this[property];
            var view = new function () {
            };
            if (view) {
                if (_.isArray(view)) {
                    // ensure that subview arrays are also properly disposed of
                    _.each(view, function (arrayView) {
                        var subView = _.isFunction(arrayView) ? arrayView() : arrayView;
                        if (arrayView) {
                            childViews.push(subView);
                        }
                    });
                } else {
                    childViews.push(view);
                }
            }
        });

        _(childViews).invoke("remove");

        this.subviews = {};
        return this;
    };
    return BaseView;
})(Backbone.View);

var FakeModel = (function (_super) {
    __extends(FakeModel, _super);
    function FakeModel() {
        _super.apply(this, arguments);
    }
    return FakeModel;
})(Backbone.Model);
//# sourceMappingURL=views2.js.map
