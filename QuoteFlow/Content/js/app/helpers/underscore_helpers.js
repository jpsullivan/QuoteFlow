_.mixin({
    lambda: function (x) {
        return function () {
            return x;
        };
    },

    isNotBlank: function (object) {
        return !!object;
    },

    bindObjectTo: function (obj, context) {
        _.map(obj, function (value, key) {
            if (_.isFunction(value)) {
                obj[key] = _.bind(value, context);
            }
        });
    }
});