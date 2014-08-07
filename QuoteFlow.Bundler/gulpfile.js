// Include gulp
var gulp = require('gulp');

// Include our plugins
var newer = require('gulp-newer');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var handlebars = require('gulp-handlebars');
var declare = require('gulp-declare');
var rename = require('gulp-rename');
var filesize = require('gulp-filesize');
var streamqueue = require('streamqueue');

var paths = {
    templates: ['../QuoteFlow/Content/views/**/*.hbs'],

    jsCompiled: '../QuoteFlow/Content/compiled/js',
};

// JST's (should always be minified)
gulp.task('templates', function () {
    return gulp.src(paths.templates)
        //.pipe(newer(paths.jsCompiled + '/templates.min.js'))
        .pipe(handlebars({
            outputType: 'bare',
            wrapped: true,
            compilerOptions: {
                knownHelpers: ['t', 'eachkeys', 'ifCond', 'debug', 'first', 'routeUrl'],
                knownHelpersOnly: false
            }
        }))
        .pipe(declare({
            processName: function (filePath) {
                filePath = filePath.replace(/\\/g, "/"); // convert fwd-slash to backslash
                filePath = filePath.replace('../QuoteFlow/Content/views/', '');
                filePath = filePath.replace('.js', '');
                return filePath;
            },
            namespace: 'JST'
        }))
        .pipe(concat('templates.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest(paths.jsCompiled))
        .pipe(filesize());
});

// Watch Files For Changes
gulp.task('watch', function () {
    gulp.watch(paths.templates, ['templates']);
});

gulp.task('default', [
    'templates',
    //'watch'
]);
