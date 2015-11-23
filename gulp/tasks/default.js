// var gulp = require('gulp');
//
// gulp.task('default', ['less', 'templates', 'fonts', 'webpack-default']);

var gulp = require('gulp');
var gulpSequence = require('gulp-sequence');
var getEnabledTasks = require('../lib/getEnabledTasks');

var defaultTask = function (cb) {
    global.production = false;
    var tasks = getEnabledTasks('watch');
    gulpSequence(tasks.assetTasks, tasks.codeTasks, 'watch', cb);
};

gulp.task('default', defaultTask);
module.exports = defaultTask;
