var config = require('../config');
if (!config.tasks.js) {
    return;
}

var gulp = require('gulp');

var webpackProductionTask = function (callback) {
    if (!global.production) {
        return;
    }

    var logger = require('../lib/compileLogger');
    var config = require('../webpack/production');
    var webpack = require('webpack');

    webpack(config, function (err, stats) {
        logger(err, stats);
        callback();
    });
};

gulp.task('webpack:production', webpackProductionTask);
module.exports = webpackProductionTask;
