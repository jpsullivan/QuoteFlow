﻿/**
  Translates a specific string

  @method t
  @for Handlebars
**/
Handlebars.registerHelper('t', function (str) {
    var text = Handlebars.Utils.escapeExpression((intra.language != undefined ? intra.language.t(str) : str));
    return new Handlebars.SafeString(text);
});

/**
  Determines a 

  @method eachKeys
  @for Handlebars
**/
Handlebars.registerHelper('eachkeys', function (context, options) {
    var fn = options.fn, inverse = options.inverse, ret = "", empty = true;

    for (key in context) { empty = false; break; }

    if (!empty) {
        for (key in context) {
            ret = ret + fn({ 'key': key, 'value': context[key] });
        }
    } else {
        ret = inverse(this);
    }
    return ret;
});

/**
  Allows an inline if statement to be performed within the markup

  @method ifCond
  @for Handlebars
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