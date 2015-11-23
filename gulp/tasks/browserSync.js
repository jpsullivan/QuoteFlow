var gulp = require('gulp');
var browserSync = require('browser-sync');
var webpack = require('webpack');
var config = require('../config');
var pathToUrl = require('../lib/pathToUrl');

var browserSyncTask = function () {
    if (global.production) {
        return;
    }

    var webpackConfig = require('../webpack/development');
    var compiler = webpack(webpackConfig);
    var proxyConfig = config.tasks.browserSync.proxy || null;

    if (typeof (proxyConfig) === 'string') {
        config.tasks.browserSync.proxy = {
            target: proxyConfig
        };
    }

    var server = config.tasks.browserSync.proxy || config.tasks.browserSync.server;
    server.middleware = [
        require('webpack-dev-middleware')(compiler, {
            publicPath: webpackConfig.output.publicPath,
            stats: {
                colors: true
            }
        }),
        require('webpack-hot-middleware')(compiler)
    ];
    server.port = 3000;

    browserSync.init(config.tasks.browserSync);
};

gulp.task('browserSync', browserSyncTask);
module.exports = browserSyncTask;
