"use strict";

var path        = require('path');
var merge       = require('merge-stream');
var gulp        = require('gulp');
var bytediff    = require('gulp-bytediff');
var concat      = require('gulp-concat');
var uglify      = require('gulp-uglify');
var handlebars  = require('gulp-handlebars');
var declare     = require('gulp-declare');
var wrap        = require('gulp-wrap');
var config      = require('../config').templates;

var handlebarsFunc = function () {
    return handlebars({
      // outputType: 'bare',
      // wrapped: true,
      compilerOptions: {
        //knownHelpers: ['t', 'ifCond', 'debug', 'first', 'select'],
        knownHelpers: {
          "t": true,
          "ifCond": true,
          "debug": true,
          "first": true,
          "select": true,
          "varLock": true,
          "urlFriendly": true,
          "compare": true
        },
        knownHelpersOnly: false
      }
  });
};

// JST's (should always be minified)
gulp.task('templates', function () {
    // Assume all partials start with an underscore
    var partials = gulp.src(config.partials)
        .pipe(handlebars())
        .pipe(wrap('Handlebars.registerPartial(<%= processPartialName(file.relative) %>, Handlebars.template(<%= contents %>));', {},
        {
            imports: {
                processPartialName: function(filePath) {
                    filePath = filePath.replace(/\\/g, "/"); // convert fwd-slash to backslash
                    return JSON.stringify(filePath.replace('.js', ''));
                }
            }
        })
    );

    var templates = gulp.src(config.src)
        //.pipe(newer(paths.jsCompiled + '/templates.min.js'))
        .pipe(handlebarsFunc())
        // Wrap each template function in a call to Handlebars.template
        .pipe(wrap('Handlebars.template(<%= contents %>)'))
        .pipe(declare({
          namespace: 'JST',
          processName: function (filePath) {
              var lookup = 'QuoteFlow\\';
              filePath = filePath.substring((filePath.indexOf(lookup) + lookup.length), filePath.length);
              filePath = filePath.replace(/\\/g, "/"); // convert fwd-slash to backslash
              filePath = filePath.replace('QuoteFlow/Content/views/', '');
              filePath = filePath.replace('.js', '');
              return filePath;
          }
      }));

    return merge(partials, templates)
        .pipe(concat('templates.min.js'))
        .pipe(bytediff.start())
        .pipe(uglify())
        .pipe(bytediff.stop())
        .pipe(gulp.dest(config.dest));
});
