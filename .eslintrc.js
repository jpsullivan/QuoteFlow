const path = require('path');

module.exports = {
    root: true,
    extends: "google",
    installedEsLint: true,
    plugins: [
        "import"
    ],
    rules: {
        "indent": ["error", 4, {
            "SwitchCase": 1
        }],
        "linebreak-style": ["error", "windows"],
        "max-len": ["warn", 130],
        "no-lonely-if": 0,
        "object-curly-spacing": 0,
        "quote-props": 0,
        "space-before-function-paren": ["error", "always"]
    },
    globals: {
        "QuoteFlow": false,
        "JST": false,
        "jQuery": true,
        "Handlebars": true,
        "AJS": false,
        "define": false,
        "require": false,
        "jasmine": false,
        "describe": false,
        "it": false,
        "expect": false,
        "beforeEach": false,
        "afterEach": false,
        "spyOn": false,
        "window": false,
        "document": false
    },
    settings: {
        'import/resolver': {
            webpack: {
                config: path.join(__dirname, 'gulp', 'webpack', 'development.js'),
            }
        }
    }
};
