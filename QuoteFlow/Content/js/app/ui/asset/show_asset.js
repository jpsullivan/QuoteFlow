QuoteFlow.UI.Asset.ShowAsset = QuoteFlow.Views.Base.extend({

    el: ".asset-container",

    options: {
    },

    events: {
        "click #footer_comment_button": "showCommentModule"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function(options) {
        console.log(this.$el);
    },

    postRenderTemplate: function () {
    },

    showCommentModule: function() {
        $('#addcomment').addClass('active');
    }
})