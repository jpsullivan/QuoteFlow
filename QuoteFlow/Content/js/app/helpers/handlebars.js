"use strict";

import JustHandlebarsHelpers from "just-handlebars-helpers";
import moment from "moment";
import Routes from "../components/routes";

JustHandlebarsHelpers.registerHelpers(Handlebars);

/**
 * Determines a @method eachKeys @for Handlebars
 **/
Handlebars.registerHelper('eachkeys', function (context, options) {
    var fn = options.fn,
        inverse = options.inverse,
        ret = "",
        empty = true;

    for (key in context) {
        empty = false;
        break;
    }

    if (empty) {
        ret = inverse(this);
    } else {
        for (key in context) {
            ret = ret + fn({
                'key': key,
                'value': context[key]
            });
        }
    }
    return ret;
});

/**
 * Allows an inline if statement to be performed within the markup
 * @method ifCond
 * @for Handlebars
 **/
Handlebars.registerHelper('ifCond', function (v1, v2, options) {
    if (v1 == v2) {
        return options.fn(this);
    }
    return options.inverse(this);
});

// debug helper
// usage: {{debug}} or {{debug someValue}}
// from: @commondream (http://thinkvitamin.com/code/handlebars-js-part-3-tips-and-tricks/)
Handlebars.registerHelper('debug', function (optionalValue) {
    console.log("Current Context");
    console.log("====================");
    console.log(this);

    if (optionalValue) {
        console.log("Value");
        console.log("====================");
        console.log(optionalValue);
    }
});

//  return the first item of a list only
// usage: {{#first items}}{{name}}{{/first}}
Handlebars.registerHelper('first', function (context, block) {
    return block(context[0]);
});

// Gravatar thumbnail
// Usage: {{#gravatar email size="64"}}{{/gravatar}} [depends on md5.js]
// Author: Makis Tracend (@tracend)
Handlebars.registerHelper('gravatar', function (context, options) {

    var email = context;
    var size = (typeof (options.hash.size) === "undefined") ? 32 : options.hash.size;

    return "http://www.gravatar.com/avatar/" + md5(email) + "?s=" + size;
});

// Converts a decimal to a percentage (including the % sign)
// Usage: {{#percentage 0.15}}{{/percentage}}
Handlebars.registerHelper('percentage', function (value) {
    return (value * 100).toFixed(2) + " %";
});

// Wraps a "select" element in a handlebars template with {{#select foo}}.
// usage: {{#select foo}} <option value="bar">Baz</option> {{/select}}
Handlebars.registerHelper('select', function (value, options) {
    // Create a select element
    var select = document.createElement('select');

    // Populate it with the option HTML
    select.innerHTML = options.fn(this);

    // Set the value
    select.value = value;

    // Find the selected node, if it exists, add the selected attribute to it
    if (select.children[select.selectedIndex]) {
        select.children[select.selectedIndex].setAttribute('selected', 'selected');
    } else { // select first option if that exists
        if (select.children[0]) {
            select.children[0].setAttribute('selected', 'selected');
        }
    }

    return select.innerHTML;
});

/**
 * Produces a url based on the route name and its values. The route name
 * is just the name of the function within the routes.js file. The routeValues
 * argument is taken in via an options hash (arg1=arg1) and sent through as a
 * key-value object.
 * Usage: {{#routeUrl 'showAsset' arg1=arg1 arg2=arg2 }}
 */
Handlebars.registerHelper('routeUrl', function (routeName, routeValues) {
    return Routes[routeName](routeValues);
});


var dateFormats = {
    "short": "DD MMMM - YYYY",
    "long": "dddd DD.MM.YYYY HH:mm",
    "comment": "DD/MMM/YY hh:mm A"
};

Handlebars.registerHelper("formatDate", function (datetime, format) {
    if (moment) {
        if (format === "relative") {
            return moment(datetime).fromNow();
        }

        var f = dateFormats[format];
        return moment(datetime).format(f);
    } else {
        return datetime;
    }
});

Handlebars.registerHelper("price", function (number) {
    // adapted from http://stackoverflow.com/questions/2901102/how-to-print-number-with-commas-as-thousands-separators-in-javascript
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
});

Handlebars.registerHelper('compare', function (lvalue, operator, rvalue, options) {
    var operators, result;

    if (arguments.length < 3) {
        throw new Error("Handlerbars Helper 'compare' needs 2 parameters");
    }

    if (options === undefined) {
        options = rvalue;
        rvalue = operator;
        operator = "===";
    }

    operators = {
        '==': function (l, r) {
            return l == r;
        },
        '===': function (l, r) {
            return l === r;
        },
        '!=': function (l, r) {
            return l != r;
        },
        '!==': function (l, r) {
            return l !== r;
        },
        '<': function (l, r) {
            return l < r;
        },
        '>': function (l, r) {
            return l > r;
        },
        '<=': function (l, r) {
            return l <= r;
        },
        '>=': function (l, r) {
            return l >= r;
        },
        'typeof': function (l, r) {
            return typeof l == r;
        }
    };

    if (!operators[operator]) {
        throw new Error("Handlerbars Helper 'compare' doesn't know the operator " + operator);
    }

    result = operators[operator](lvalue, rvalue);

    if (result) {
        return options.fn(this);
    }

    return options.inverse(this);
});

Handlebars.registerHelper("math", function (lvalue, operator, rvalue, options) {
    lvalue = parseFloat(lvalue);
    rvalue = parseFloat(rvalue);

    return {
        "+": lvalue + rvalue,
        "-": lvalue - rvalue,
        "*": lvalue * rvalue,
        "/": lvalue / rvalue,
        "%": lvalue % rvalue
    }[operator];
});
