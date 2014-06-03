/// <reference path="../lib/jquery.d.ts"/>
/// <reference path="../lib/underscore.d.ts"/>
/// <reference path="../lib/backbone.d.ts"/>
/// <reference path="quoteflow.ts"/>

declare var JST: any;

class BaseView extends Backbone.View<FakeModel> {

    template: (data: any) => string;
    templateName: string;
    subviews: any;
    
    constructor(options) {
        super(options);
        this.setupRenderEvents();
    }

    setupRenderEvents() {
        if (this.model) {
            this.model.bind('remove', this.remove, this);
        }
    }

    presenter() {
        return this.defaultPresenter();
    }

    // automatically plugs-in the model attributes into the JST, as well as
    // some site-wide attributes that we define below
    defaultPresenter() {
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
    }

    render() {
        this.renderTemplate();
        this.renderSubviews();

        return this;
    }

    renderTemplate() {
        var presenter = _.isFunction(this.presenter) ? this.presenter() : this.presenter;
        this.template = JST[this.templateName];
        if (!this.template) {
            console.log(!_.isUndefined(this.templateName) ? ("no template for " + this.templateName) : "no templateName specified");
        }

        this.$el
            .html(this.template(presenter))
            .attr("data-template", _.last(this.templateName.split("/")));
        this.postRenderTemplate();
    }

    postRenderTemplate() {
        $.noop; // hella callbax yo
    }

    renderSubviews() {
        _.each(this.subviews, (property, selector) => {
            //var view = _.isFunction(this[property]) ? this[property]() : this[property];
            var view = new function() {};
            if (view) {
                if (_.isArray(view)) {
                    // If we pass an array of views into the subviews, append each to the selector.
                    // This should generally only be used when dealing with parent-specific class issues.
                    // For example, if you need to supply a "span*" class on the parent to prevent the
                    // box model from breaking.
                    _.each(view, function (arrayView: any) {
                        var subView = _.isFunction(arrayView) ? arrayView() : arrayView;
                        if (arrayView) {
                            this.$(selector).append(subView.render().el);
                            subView.delegateEvents();
                        }
                    });
                } else {
                    // drop the view directly into the selector
                    this.$(selector).html(view.render().el);
                    view.delegateEvents();
                }
            }
        });
    }

    remove() {
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
    }

    removeSubviews() {
        var children = this.subviews,
            childViews = [];

        if (!children) {
            return this;
        }

        _.each(children, (property, selector) => {
            //var view = _.isFunction(this[property]) ? this[property]() : this[property];
            var view = new function () { };
            if (view) {
                if (_.isArray(view)) {
                    // ensure that subview arrays are also properly disposed of
                    _.each(view, (arrayView: any) => {
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
    }
}

class FakeModel extends Backbone.Model { }
