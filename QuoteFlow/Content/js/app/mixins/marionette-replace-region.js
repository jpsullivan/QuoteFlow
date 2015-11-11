"use strict";

var Marionette = require('backbone.marionette');

/**
 * This type of region will replace the region itself with a view. This Region is useful when the region markup should
 * not be present in the final DOM.
 *
 * With Marionette.Region:
 *
 *     <body>
 *       <div id="region"></div>
 *     </body>
 *
 *     var MainView = Marionette.ItemView.extend({
 *        template: function() {
 *            return '<section id="main"></section>';
 *        }
 *     });
 *     var ContainerView = Marionette.LayoutView.extend({
 *      regions: {
 *         main: "#region"
 *     });
 *     var view = new ContainerView();
 *     view.main(new MainView());
 *
 *     //Fails, it will generate
 *     <body>
 *       <div id="region">
 *         <section id="main"></section>
 *       </div>
 *     </body>
 *
 *
 * With Marionette.ReplaceRegion:
 *
 *     var ContainerView = Marionette.LayoutView.extend({
 *      regions: {
 *         main: {selector: "#region", regionType: Marionette.AppendRegion},
 *     });
 *
 *     //Works, it will generate
 *     <body>
 *       <section id="main"></section>
 *     </body>
 *
 *
 * Of course, this will destroy the original region markup. If you call addRegion() to add the same region again, it
 * won't work unless you restore the original markup.
 *
 * You can thin about this region as a 'transparent container' for views, as the markup for the region will disappear
 * when a view is rendered inside. For example, it is used by IssueView to render the body and headers. The IssueView
 * template contains placeholders for the Body/Header views, but we want those placeholders to be removed from the final
 * output.
 *
 * This region is like {@link LinkRegion} but here we remove the regions' markup.
 * @extends Marionette.Region
 */
var ReplaceRegion = Marionette.Region.extend({
    open: function(view){
        this.$el.replaceWith(view.el);
    }
});

Marionette.ReplaceRegion = ReplaceRegion;

module.exports = ReplaceRegion;
