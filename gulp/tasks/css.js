var config = require('../config');
if (!config.tasks.css) {
    return;
}

var gulp = require('gulp');
var gulpif = require('gulp-if');
var browserSync = require('browser-sync');
var less = require('gulp-less');
var sourcemaps = require('gulp-sourcemaps');
var handleErrors = require('../lib/handleErrors');
var autoprefixer = require('gulp-autoprefixer');
var path = require('path');
var rename = require('gulp-rename');
var cssnano = require('gulp-cssnano');

const paths = {
    src: path.resolve(__dirname, "..", "..", path.join(config.root.src, config.tasks.css.src, config.tasks.css.less.entryPoint)),
    dest: path.resolve(__dirname, "..", "..", path.join(config.root.dest, config.tasks.css.dest))
};

var cssTask = function() {
    return gulp.src(paths.src)
        .pipe(gulpif(!global.production, sourcemaps.init()))
        .pipe(less(config.tasks.css.less))
        .on('error', handleErrors)
        .pipe(autoprefixer(config.tasks.css.autoprefixer))
        .pipe(gulpif(global.production, cssnano({
            autoprefixer: false
        })))
        .pipe(gulpif(!global.production, sourcemaps.write()))
        .pipe(rename(global.production ? "app.min.css" : "app.css"))
        .pipe(gulp.dest(paths.dest))
        .pipe(browserSync.stream());
}

gulp.task('css', cssTask);
module.exports = cssTask;
