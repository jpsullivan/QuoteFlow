/*
  Notes:
   - gulp/tasks/browserify.js handles js recompiling with watchify
   - gulp/tasks/browserSync.js watches and reloads compiled files
*/

var gulp = require('gulp');

gulp.task('watch', ['setWatch', 'build'], function () {
  if(global.isWatching) {
    gulp.watch('./QuoteFlow/Content/less/**/*.less', ['less']);
    gulp.watch('./QuoteFlow/Content/views/**/*.{handlebars, hbs}', ['templates']);
  }
});
