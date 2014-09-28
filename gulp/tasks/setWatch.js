var gulp = require('gulp');
var gutil = require('gulp-util');

gulp.task('setWatch', function () {
  // isWatching only true when environment is set as dev
  var isProduction = gutil.env.type === 'production'
  global.isWatching = !isProduction;
});
