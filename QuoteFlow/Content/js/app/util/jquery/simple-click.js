(function ($) {
    $.event.special.simpleClick = {
        add: function (handleObj) {
            handleObj._clickHandler = function (event) {
                if (!event.ctrlKey && !event.metaKey && !event.shiftKey) {
                    return handleObj.handler.apply(this, arguments);
                }
            };
            $(this).on('click', handleObj.selector, handleObj._clickHandler);
        },

        remove: function (handleObj) {
            $(this).off('click', handleObj.selector, handleObj._clickHandler);
        }
    };
})(jQuery);
