"use strict";

import $ from "jquery";
import Marionette from "backbone.marionette";

export default Marionette.ItemView.extend({
    el: "#header",

    ui: {
        quicksearch: ".aui-header .aui-header-secondary #quick-search-input",
        header: ".aui-header",
        searchIcon: "#mobile-search",
        fullSearchInput: "#quicksearchid",
        quickSearchInput: "#mobile-quicksearchid"
    },

    events: {
        "click @ui.searchIcon": "_onSearchIconClick"
    },

    initialize: function () {
        this.bindUIElements();
    },

    _onSearchIconClick (e) {
        if (this.ui.quicksearch.is(':hidden')) {
            this.ui.quickSearchInput.attr('placeholder', this.ui.fullSearchInput.attr('placeholder'));
            this.ui.searchIcon.css("color", "#cccccc");
            this.ui.quicksearch.slideToggle('medium', function () {
                $(this).css('display', 'inline-block');
            });
        } else {
            this.ui.searchIcon.css("color", "#fff");
            this.ui.quicksearch.slideToggle();
        }
    }
});
