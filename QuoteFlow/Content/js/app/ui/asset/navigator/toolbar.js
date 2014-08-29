QuoteFlow.UI.Asset.Navigator.Toolbar = QuoteFlow.Views.Base.extend({
    el: ".navigator-search",

    options: {},

    events: {},

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function (options) {
        _.bindAll(this, 'manufacturerClickHandler');

        var query = QuoteFlow.Components.Query.create({
            searchers: {
                searchers: {
                    groups: [
                        {
                            searchers: [
                                {
                                    id: "manufacturer",
                                    isShown: true,
                                    key: "asset.field.manufacturer",
                                    name: "Manufacturer"
                                }
                            ],
                            title: "Details",
                            type: "DETAILS"
                        }
                    ]
                },
                values: {
                    manufacturer: {
                        isShown: true,
                        name: "Manufacturer",
                        validSearcher: true
                    }
                }
            },
            preferredSearchMode: "basic",
            layoutSwitcher: true,
            basicAutoUpdate: true
        });

        query._basicQueryModule.queryChanged();

        if (_.isUndefined(this.model)) {
            // initialize an empty model anyway
            this.model = new QuoteFlow.Model.Asset.Navigator();
        }

        this.initManufacturerDropdownClickHandlers();
        this.initManufacturersFilter();

        this.primaryCriteriaContainerView = new QuoteFlow.UI.Asset.Navigator.PrimaryCriteriaContainer({ collection: query._basicQueryModule.searcherCollection });
        this.textFieldView = new QuoteFlow.UI.Asset.Navigator.TextField({ collection: query._basicQueryModule.searcherCollection });

        this.listenTo(this.model, 'change:selectedManufacturers', this.updateManufacturerButtonTitle);
    },

    postRenderTemplate: function () { },

    /**
     * Initialize the click handler for dropdown items. This occurs here
     * instead of in the 'events' delegate due to the fact that dropdown
     * items are out of element scope from the rest of the DOM.
     */
    initManufacturerDropdownClickHandlers: function() {
        $('#manufacturers-dropdown .check-list-item').on('change', this.manufacturerClickHandler);
    },

    /**
     * Sets up the functionality for the manufacturers filter
     * and subsequent popup window (search auto-filter, selected mfg's).
     */
    initManufacturersFilter: function() {
        this.addSelectedManufacturersListener();
        this.addManufacturersSearchFilter();
    },

    /**
     * Adds the listener for AUI's aui-dropdown2-show
     * trigger which when called, adds the selected manufacturers
     * to the top of the popup window.
     */
    addSelectedManufacturersListener: function() {
        AJS.$('#manufacturers-dropdown').on({
            "aui-dropdown2-show": function() {
                console.log('show it');
            }
        });
    },

    /**
     * Initializes the Manufacturers multi select
     * dropdown to automatically filter results based 
     * on the input (yet without having to use select2)
     */
    addManufacturersSearchFilter: function() {
        var worker = $("#manufacturers_filter .check-list-field"),
            manufacturers = $("#manufacturers_filter .check-list-item");

        worker.keyup(function () {
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
    },

    /**
     * Handles when a manufacturer from the popup is selected (or deselected).
     */
    manufacturerClickHandler: function (e) {
        var el = $('input', $(e.currentTarget)), parent = $(e.currentTarget);
        var manufacturer = parent.attr('title');

        if(el.is(':checked')) {
            // add the mfg to the selected list
            this.model.addManufacturer(manufacturer);
            return;
        }

        // otherwise remove it
        this.model.removeManufacturer(manufacturer);
        return;
    },

    /**
     * Called after adding an item to the array of
     * selected mfg's. This takes those mfg's and takes their
     * names, adding them to the button text. If none are selected, 
     * the text just says, "All".
     */
    updateManufacturerButtonTitle: function () {
        var title = "All", button = $('button[data-id="manufacturers"]');

        var selectedManufacturers = this.model.get('selectedManufacturers');
        if (!_.isEmpty(selectedManufacturers)) {
            title = selectedManufacturers.join(',');
        }

        $('.selected-criteria', button).html(title);
    }
})