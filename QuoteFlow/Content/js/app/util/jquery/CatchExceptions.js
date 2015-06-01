; (function ($) {
    var catchExceptions = function (fn) {
        return function (html) {
            try {
                fn.apply(this, arguments);
            } catch (e) {
                //                if (console && console.error && typeof html === 'string') {
                //                    console.error('Error while evaluating HTML: ' + e.message + ', in: ', html);
                //                }
            }
            return this;
        };
    };

    $.fn.htmlCatchExceptions = catchExceptions($.fn.html);
    $.fn.appendCatchExceptions = catchExceptions($.fn.append);
    $.fn.prependCatchExceptions = catchExceptions($.fn.prepend);

    $.catchExceptions = function (html) {
        // Done this way because $(html) doesn't evaluate scripts until elements are added to the document
        return $('<div>').htmlCatchExceptions(html).children();
    };
})(AJS.$);