"use strict";

var SelectSuggestHelper = require('./select-suggest-handler');

/**
 * Special handler for share dialog pickers.
 * @class UserListSuggestHandler
 * @extends SelectSuggestHandler
 */
var UserListSuggestHandler = SelectSuggestHelper.extend({
    /**
     * Tests valid email address
     */
    emailExpression: /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/,

    /**
     * Only mirror user input if it is a valid email address
     * @param {String} query
     * @return {Boolean}
     */
    validateMirroring: function (query) {
        return this.options.freeEmailInput && query.length > 0 && this.emailExpression.test(query);
    }
});

module.exports = UserListSuggestHandler;