QuoteFlow.UI.Asset.Navigator.Toolbar = QuoteFlow.Views.Base.extend({
    el: ".navigator-search",

    options: {},

    events: {},

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function (options) {
        this.initManufacturersFilter();
    },

    postRenderTemplate: function () { },

    /**
     * Initializes the Manufacturers multi select
     * dropdown to automatically filter results based 
     * on the input (yet without having to use select2)
     */
    initManufacturersFilter: function() {
        var worker = $("#manufacturers_filter .check-list-field"),
            container = $("#manufacturers_filter .aui-list-section"),
            manufacturers = $("#manufacturers_filter .check-list-item");

        worker.keyup(function() {
            var text = $(this).val();
            if (text === "") {
                // show all manufacturers
                manufacturers.each(function (i, v) {
                    var $el = $(v);
                    $el.removeClass('hidden');
                });
                return;
            }

            // perform the filter logic
            manufacturers.each(function (i, v) {
                var $el = $(v);
                var title = $('.item-label', $el).attr('title');

                // if this manufacturer doesn't contain the search text, hide it.
                if (title.toLowerCase().indexOf(text.toLowerCase()) === -1) {
                    $el.addClass('hidden');
                } else {
                    $el.removeClass('hidden');
                }
            });
            return;
        });
    }
})