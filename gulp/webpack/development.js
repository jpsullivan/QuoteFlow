if (global.production) {
    return;
}

const webpack = require('webpack');
const WebpackNotifierPlugin = require('webpack-notifier');
const standardConfig = require('./base.js');

// prepend the HMR entries to the base entry array
standardConfig.entry.app = [
    "webpack/hot/dev-server",
    "webpack-hot-middleware/client?path=//localhost:3000/__webpack_hmr",
    ...standardConfig.entry.app
];

standardConfig.cache = true;
standardConfig.devtool = "inline-source-map";
standardConfig.debug = true;
standardConfig.plugins.push(new webpack.DefinePlugin({
    'process.env': {
        NODE_ENV: JSON.stringify('development')
    }
}));
standardConfig.plugins.push(new webpack.optimize.DedupePlugin());
standardConfig.plugins.push(new webpack.optimize.OccurenceOrderPlugin());
standardConfig.plugins.push(new webpack.NoErrorsPlugin());
standardConfig.plugins.push(new webpack.HotModuleReplacementPlugin());
standardConfig.plugins.push(new WebpackNotifierPlugin({title: 'Webpack'}));

module.exports = standardConfig;
