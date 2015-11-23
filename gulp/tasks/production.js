var gulp = require('gulp');
var gulpSequence = require('gulp-sequence');
var getEnabledTasks = require('../lib/getEnabledTasks');

var productionTask = function (cb) {
    global.production = true;
    var tasks = getEnabledTasks('production');
    gulpSequence(tasks.assetTasks, tasks.codeTasks, 'size-report', cb);
};

gulp.task('production', productionTask);
module.exports = productionTask;
