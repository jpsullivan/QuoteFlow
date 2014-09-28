/* bundleLogger
   ------------
   Provides gulp style logs to the bundle method in browserify.js
*/

var gutil        = require('gulp-util');
var notify = require('gulp-notify');
var prettyHrtime = require('pretty-hrtime');
var startTime;

module.exports = {
  start: function() {
    startTime = process.hrtime();
    gutil.log('Running', gutil.colors.cyan("'bundle'") + '...');
  },

  end: function() {
    var taskTime = process.hrtime(startTime);
    var prettyTime = prettyHrtime(taskTime);
    gutil.log('Finished', gutil.colors.cyan("'bundle'"), 'in', gutil.colors.green(prettyTime));
    notify('donezo');
  }
};
