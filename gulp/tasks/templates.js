var config = require('../config')
if (!config.tasks.templates) {
    return;
}

var path = require('path');
var merge = require('merge-stream');
var gulp = require('gulp');
var gulpif = require('gulp-if');
var bytediff = require('gulp-bytediff');
var concat = require('gulp-concat');
var handleErrors = require('../lib/handleErrors');
var uglify = require('gulp-uglify');
var handlebars = require('gulp-handlebars');
var declare = require('gulp-declare');
var wrap = require('gulp-wrap');
var browserSync = require('browser-sync');

const paths = {
    src: path.resolve(__dirname, "..", "..", path.join(config.root.src, config.tasks.templates.src, "**/[^_]*.{hbs, handlebars}")),
    srcPartials: path.resolve(__dirname, "..", "..", path.join(config.root.src, config.tasks.templates.src, "**/_*.hbs")),
    dest: path.resolve(__dirname, "..", "..", path.join(config.root.dest, config.tasks.js.dest))
};

var handlebarsFunc = function () {
    return handlebars({
        // outputType: 'bare',
        // wrapped: true,
        compilerOptions: {
            // knownHelpers: ['t', 'ifCond', 'debug', 'first', 'select'],
            knownHelpers: {
                "ifCond": true,
                "debug": true,
                "first": true,
                "select": true,
                "varLock": true,
                "dateFormat": true,
                "urlFriendly": true,
                "checkedIf": true,
                "disabledIfNoRole": true
            },
            knownHelpersOnly: false
        }
    });
};

var templatesTask = function () {
    // Assume all partials start with an underscore
    var partials = gulp.src(paths.srcPartials)
        .pipe(handlebarsFunc())
        // Report compile errors
        .on('error', handleErrors)
        .pipe(wrap('Handlebars.registerPartial(<%= processPartialName(file.relative) %>, Handlebars.template(<%= contents %>));', {},
            {
                imports: {
                    processPartialName: function (filePath) {
                        filePath = filePath.replace(/\\/g, "/"); // convert fwd-slash to backslash
                        return JSON.stringify(filePath.replace('.js', ''));
                    }
                }
            })
    );

    var templates = gulp.src(paths.src)
        .pipe(handlebarsFunc())
        // Report compile errors
        .on('error', handleErrors)
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
        .pipe(concat(global.production ? "templates.min.js" : "templates.js"))
        .pipe(gulpif(global.production, bytediff.start()))
        .pipe(gulpif(global.production, uglify()))
        .pipe(gulpif(global.production, bytediff.stop()))
        .pipe(gulp.dest(paths.dest))
        .pipe(browserSync.stream());
};

// JST's (should always be minified)
gulp.task('templates', templatesTask);
module.exports = templatesTask;
