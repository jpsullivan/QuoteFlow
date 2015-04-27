var gulp          = require('gulp');
var less          = require('gulp-less');
var filesize      = require('gulp-filesize');
var rename        = require('gulp-rename');
var config        = require('../config').less;

gulp.task('less', function () {
  return gulp.src(config.entryPoint)
      .pipe(less())
      .pipe(rename('app.css'))
      .pipe(gulp.dest(config.dest))
      .pipe(less({ compress: true }))
      .pipe(rename('app.min.css'))
      .pipe(gulp.dest(config.dest))
      .pipe(filesize());
});
