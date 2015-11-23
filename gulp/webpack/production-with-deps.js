const Visualizer = require('webpack-visualizer-plugin');
const productionConfig = require('./production.js');

delete productionConfig.externals;

productionConfig.entry = {
    'dist/bundle-with-deps.js': productionConfig.entry['dist/bundle.js'],
    'dist/bundle-with-deps.min.js': productionConfig.entry['dist/bundle.js']
};

productionConfig.plugins.push(new Visualizer({
    filename: './stats/with-deps.html'
}));

module.exports = productionConfig;
