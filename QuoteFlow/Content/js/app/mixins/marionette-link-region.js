"use strict";

var Marionette = require('backbone.marionette');

/**
 * This type of region will render the view's children and set the view root element to the region itself.
 *
 *     <body>
 *       <div id="region"></div>
 *     </body>
 *
 *     var MainView = Marionette.ItemView.extend({
 *        template: function() {
 *            return '<div><p>Content</p></div>';
 *        }
 *     });
 *     var ContainerView = Marionette.LayoutView.extend({
 *      regions: {
 *         main: "#region", regionType: Marionette.LinkRegion,
 *     });
 *     var view = new ContainerView();
 *     view.main(new MainView());
 *
 *     //It will generate
 *     <body>
 *       <div id="region">
 *         <p>Content</p>
 *       </div>
 *     </body>
 *
 *     //And the follow is true:
 *     view.$el.is(jQuery("#region"))
 *
 *
 * You can thin about this region as a container for views 'without container'. It will remove the view's root element
 * from the output, plus it will set the view's main element to the region element.
 *
 * For example, it is used by BodyView. Here the body template contains the regions for the left/right columns, where
 * the panels will be rendered. The list of panels is a view with a <div> as the root element, but we don't want that
 * <div> in the final output.
 *
 * This region is like {@link ReplaceRegion} but here we remove the view's root markup.
 * @extends Marionette.Region
 */
var LinkRegion = Marionette.Region.extend({
    /**
     * Method used to open (i.e. inject a view) this region.
     * @param {Backbone.View} view View to inject
     */
    open: function(view) {
        this.$el.append(view.$el.children());
        view.setElement(this.$el);
    }
});

Marionette.LinkRegion = LinkRegion;

module.exports = LinkRegion;
