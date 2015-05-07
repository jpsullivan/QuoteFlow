var gulp        = require('gulp');
var bytediff    = require('gulp-bytediff');
var newer       = require('gulp-newer');
var concat      = require('gulp-concat');
var uglify      = require('gulp-uglify');
var handlebars  = require('gulp-handlebars');
var declare     = require('gulp-declare');
var wrap        = require('gulp-wrap');
var config      = require('../config').templates;

// JST's (should always be minified)
gulp.task('templates', function () {
    return gulp.src(config.src)
        //.pipe(newer(paths.jsCompiled + '/templates.min.js'))
        .pipe(handlebars({
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
        }))
        // Wrap each template function in a call to Handlebars.template
        .pipe(wrap('Handlebars.template(<%= contents %>)'))
        .pipe(declare({
          namespace: 'JST',
          processName: function (filePath) {
            var lookup = 'QuoteFlow\\';
            filePath = filePath.substring((filePath.indexOf(lookup) + lookup.length), filePath.length)
            filePath = filePath.replace(/\\/g, "/"); // convert fwd-slash to backslash
            filePath = filePath.replace('QuoteFlow/Content/views/', '');
            filePath = filePath.replace('.js', '');
            return filePath;
          }
        }))
        .pipe(concat('templates.min.js'))
        .pipe(bytediff.start())
        .pipe(uglify())
        .pipe(bytediff.stop())
        .pipe(gulp.dest(config.dest))
});
