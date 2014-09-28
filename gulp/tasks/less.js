var gulp          = require('gulp');
var less          = require('gulp-less');
var filesize      = require('gulp-filesize');
var rename        = require('gulp-rename');

var paths = {
  cssCompiled: 'QuoteFlow/Content/compiled/css'
};

gulp.task('less', function () {
  return gulp.src('QuoteFlow/Content/less/bootstrap.less')
      .pipe(less())
      .pipe(rename('app.css'))
      .pipe(gulp.dest(paths.cssCompiled))
      .pipe(less({ compress: true }))
      .pipe(rename('app.min.css'))
      .pipe(gulp.dest(paths.cssCompiled))
      .pipe(filesize());
});
