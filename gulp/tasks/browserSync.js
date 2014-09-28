var browserSync = require('browser-sync');
var gulp        = require('gulp');

gulp.task('browserSync', ['build'], function () {
  browserSync({
    server: {
      // src is included for use with less source maps
      baseDir: ['build', 'src']
    },
    files: [
      // watch everything in compiled
      "compiled/**",
      // exclude sourcemap files
      "!compiled/**.map"
    ]
  })
});
