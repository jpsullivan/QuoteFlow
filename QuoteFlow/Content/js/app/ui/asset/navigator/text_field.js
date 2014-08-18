QuoteFlow.UI.Asset.Navigator.TextField = QuoteFlow.Views.Base.extend({
    el: '.text-query-container input',

    events: {
        keypress: 'handleKeypress'
    },

    initialize: function() {
        _.bindAll(this, "updateSearcherCollectionTextField", "render");

        this.collection.on("remove change add", _.bind(function (a) {
            if (a.getId() === this.collection.QUERY_ID) {
                this.render();
            }
        }, this));

        this.collection.onTextFieldChanged(this.render);
        this.collection.onRequestUpdateFromView(this._updateSearcherCollectionTextField);
        this.collection.onInteractiveChanged(this._handleInteractiveChanged);
    },

    handleKeypress: function (e) {
        // enter key detection
        if (e.keyCode === 13) {
            this.collection.handleBasicViewSubmit();
            e.preventDefault();
        }
    },

    setQuery: function(query) {
        this.$el.valueOf(query);
    },

    updateSearcherCollectionTextField: function() {
        if (this.$el.is('input')) {
            var query = AJS.$.trim(this.$el.val());
            this.collection.updateTextQuery(query);
        }
    }
});