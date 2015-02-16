/* browserify task
   ---------------
   Bundle javascripty things with browserify!

   If the watch task is running, this uses watchify instead
   of browserify for faster bundling using caching.
*/

var browserify    = require('browserify');
var watchify      = require('watchify');
var bundleLogger  = require('../util/bundleLogger');
var gulp          = require('gulp');
var debug         = require('gulp-debug');
var bytediff      = require('gulp-bytediff');
var handleErrors  = require('../util/handleErrors');
var source        = require('vinyl-source-stream');
var buffer        = require('vinyl-buffer');
var gutil         = require('gulp-util');
var uglify        = require('gulp-uglify');
var sourcemaps    = require('gulp-sourcemaps');
var notify        = require("gulp-notify");

var cache = {};
var pkgCache = {};

var entryPoint = '../../QuoteFlow/Content/js/app/init.js';
var watchifyEntryPoint = './QuoteFlow/Content/js/app/init.js';

gulp.task('browserify', function() {
  var isProduction = gutil.env.type === 'production';

  var bundler = browserify({
    basedir: __dirname,
    // Required watchify args
    cache: cache, packageCache: pkgCache, fullPaths: false,
    // Specify the entry point of your app
    entries: [entryPoint],
    // Add file extentions to make optional in your requires
    extensions: ['.js'],
    // Don't parse massive libs like jquery to make bundling much faster
    //noparse: ['jquery', 'jquery-ui', 'backbone'],
    // Ignore __dirname__, __filename__, etc. Speeds up builds
    //insertGlobals : true,
    // Enable source maps!
    debug: !isProduction
  });

  var bundle = function() {
    // Log when bundling starts
    bundleLogger.start();

    return bundler
      .bundle()
      // Report compile errors
      .on('error', handleErrors)

      // Use vinyl-source-stream to make the
      // stream gulp compatible. Specifiy the
      // desired output filename here.
      .pipe(isProduction ? source('bundle.min.js') : source('bundle.js'))
      .pipe(buffer())
      .pipe(sourcemaps.init({loadMaps: true}))

      // Only display debug info when in dev mode
      .pipe(!isProduction ? debug({verbose: false}) : gutil.noop())

      // bytediff the stream output after minifying
      .pipe(bytediff.start())
      // Add transformation tasks to the pipeline here.
      .pipe(isProduction ? uglify({ preserveComments: "some"}) : gutil.noop())
      .pipe(bytediff.stop())

      //.pipe(sourcemaps.write('./QuoteFlow/Content/compiled/js'))
      .pipe(gulp.dest('./QuoteFlow/Content/compiled/js'))

      // Log when bundling completes!
      .on('end', bundleLogger.end);
  };

  if(global.isWatching) {
    bundler = watchify(browserify(watchifyEntryPoint, watchify.args));
    // Rebundle with watchify on changes.
    bundler.on('update', bundle);
  }

  return bundle();
});
