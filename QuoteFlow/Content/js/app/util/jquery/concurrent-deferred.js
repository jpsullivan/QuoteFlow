"use strict";

var jQuery = require('jquery');

var ConcurrentDeferred = function(initialDeferred) {
    var concurrentDeferred = new jQuery.Deferred();
    var activeDeferred;

    var attach = function(deferred) {
        activeDeferred = deferred;
        deferred.done(function() {
            if (activeDeferred === deferred) {
                concurrentDeferred.resolve.apply(concurrentDeferred, arguments);
            }
        }).fail(function() {
            if (activeDeferred === deferred) {
                concurrentDeferred.reject.apply(concurrentDeferred, arguments);
            }
        });
    };

    attach(initialDeferred);

    jQuery.extend(concurrentDeferred, {
        update: function(updatedDeferred) {
            if (!this.isPending()) {
                throw "Cannot update non-pending ConcurrentDeferred";
            }

            var oldDeferred = activeDeferred;
            attach(updatedDeferred);

            if (jQuery.isFunction(oldDeferred.abort)) {
                oldDeferred.abort();
            }

            return this;
        },
        isPending: function() {
            return this.state() === 'pending';
        }
    });

    return concurrentDeferred;
};

jQuery.ConcurrentDeferred = ConcurrentDeferred;

module.exports = ConcurrentDeferred;
