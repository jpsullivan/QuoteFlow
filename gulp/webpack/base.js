const camelCase = require('camelcase');
const path = require('path');
const pkg = require(path.join(process.cwd(), 'package.json'));
const autoprefixer = require('autoprefixer');
const webpack = require('webpack');
const config = require('../config');

const paths = {
    src: path.resolve(__dirname, "..", "..", config.root.src, config.tasks.js.src, config.tasks.js.entries.app),
    dest: path.resolve(__dirname, "..", "..", config.root.dest, config.tasks.js.dest)
};

const isDevelopment = !global.production;

/**
 * [defaultPackageMains description]
 * @return {[type]} [description]
 */
function defaultPackageMains () {
    const options = new webpack.WebpackOptionsDefaulter();
    options.process({});
    return options.defaults.resolve.packageMains;
}

function isExternal (module) {
    var userRequest = module.userRequest;

    if (typeof userRequest !== 'string') {
        return false;
    }

    return userRequest.indexOf('bower_components') >= 0 ||
         userRequest.indexOf('node_modules') >= 0 ||
         userRequest.indexOf('libraries') >= 0;
}

/**
 * Build a loader chain.
 *
 * @param {Object} spec -- {loader2: {}, loader1: {}, ...}
 *   The order of definition is significant. The prior example would return:
 *
 *       'loader1?{}!loader2?{}'
 */
const loaderChain = spec => Object.keys(spec)
  .map(key => `${key}?${JSON.stringify(spec[key])}`)
  .join('!');

const standardConfig = {
    entry: {
        app: [paths.src],
        vendor: []
    },
    output: {
        path: paths.dest,
        publicPath: "http://localhost:3000/Content/compiled/js/",
        // publicPath: "/js/",
        filename: "bundle.js",
        chunkFilename: "[chunkhash].js",
        libraryTarget: "umd"
        // This will be the name of the global in the UMD module.
        // library: camelCase(pkg.name)
    },
    resolve: {
        alias: {
            'jquery': require.resolve('jquery'),

            // jQuery-UI.. http://stackoverflow.com/a/32080299/1487071
            "jquery.ui.core": "jquery-ui/core",
            "jquery.ui.draggable": "jquery-ui/draggable",
            "jquery.ui.droppable": "jquery-ui/droppable",
            "jquery.ui.mouse": "jquery-ui/mouse",
            "jquery.ui.sortabl": "jquery-ui/sortable",
            "jquery.ui.widget": "jquery-ui/widget",

            // jquery.inputmask... https://gist.github.com/ksrb/47f9124c2f5be13f39dc645405a12608
            "inputmask.dependencyLib": "jquery.inputmask/dist/inputmask/inputmask.dependencyLib.jquery",
            "inputmask": "jquery.inputmask/dist/inputmask/inputmask",

            "handlebars": 'handlebars/runtime.js',

            "auiSelect2": path.resolve(config.root.src, "js", "lib", "aui-select2.js")
        },
        extensions: ['', '.webpack.js', '.web.js', '.ts', '.tsx', '.js', '.jsx']
        // packageMains: ['ak:webpack:raw', ...defaultPackageMains()]
    },
    module: {
        loaders: [
            {
                test: /\.json$/,
                loader: 'json'
            },
            {
                test: /\.less$/,
                loader: loaderChain({
                    "css-loader": {
                        camelCase: true,
                        importLoaders: 1,
                        mergeRules: false,
                        modules: true
                    },
                    "postcss-loader": {},
                    "less-loader": {}
                })
            },
            [
                {
                    loader: 'babel-loader',
                    test: /\.js$/,
                    include: path.resolve(__dirname, "..", "..", config.root.src, config.tasks.js.src, "app"),
                    exclude: /node_modules|bower_components/,
                    query: {
                        presets: ['es2015']
                    }
                }
            ],
            {
                // expose jQuery as an accessible Window object
                test: require.resolve("jquery"),
                loader: "expose?jQuery!expose?$"
            },
            {
                test: require.resolve("jquery-ui/widget"),
                loader: "imports-loader?dep1=jquery.ui.core&dep2=jquery.ui.widget"
            }
        ]
    },
    postcss: () => [
        autoprefixer({
            browsers: 'last 3 versions'
        })
    ],
    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            name: 'vendor',
            filename: isDevelopment ? "vendor.js" : "vendor.min.js",
            // children: true,
            minChunks: function (module) {
                return isExternal(module);
            }
        }),
        new webpack.ProvidePlugin({
            // Automtically detect jQuery and $ as free var in modules
            // and inject the jquery library
            // This is required by many jquery plugins
            $: "jquery",
            jQuery: "jquery",
            "window.jQuery": "jquery"
        }),
        // prevent moment from autoloading all locales, creating huge bundles
        new webpack.IgnorePlugin(/^\.\/locale$/, [/moment$/])
    ]
};

module.exports = standardConfig;
