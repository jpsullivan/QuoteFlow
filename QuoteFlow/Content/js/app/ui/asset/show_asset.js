QuoteFlow.UI.Asset.ShowAsset = QuoteFlow.Views.Base.extend({

    el: ".asset-container",

    options: {},

    events: {
        "click #footer_comment_button": "showCommentModule",
        "click #asset_comment_add_cancel": "hideCommentModule"
    },

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
        });
    },

    initialize: function(options) {},

    postRenderTemplate: function () {},

    /**
     * Shows the comment form.
     */
    showCommentModule: function() {
        $('#addcomment').addClass('active');
    },

    /**
     * Hides the comment form.
     */
    hideCommentModule: function() {
        $('#addcomment').removeClass('active');
    }
})