(function () {
    'use strict';

    module.exports = function () {
        var mainConfiguration = require('./common/config'),
            path = require('path'),
            _ = require('lodash'),
            config = {};

        config = _.merge(mainConfiguration, {

            dtsOutputFolder: './Source/Typings',
            javascriptFolder: './Source/JavaScript',
            jsBundleName: 'orckestra.composer.js',
            composerFrameworkJavascriptMinifiedFileName: 'orckestra.composer.min.js',
            dtsBundleName: 'orckestra.composer.d.ts',

            karma: {
                files: [
                    path.join(__dirname, '../../3rdParty.ForTests/**/*.js'),
                    path.join(__dirname, '../../3rdParty/q-1.2.0.js'),
                    path.join(__dirname, '../../3rdParty/jquery-1.11.2.min.js'),
                    path.join(__dirname, '../../3rdParty/lodash.min.js'),
                    path.join(__dirname, '../../3rdParty/parsley.min.js'),
                    path.join(__dirname, '../../3rdParty/handlebars.helpers.js'),
                    '../../Source/JavaScript/orckestra.composer.js',
                    path.join(__dirname, '../', mainConfiguration.testsOutputFolder) + '/**/*.spec.js'
                ],

                filesToBuild: [
                    './Source/Tests/unit/**/*.spec.ts'
                ]
            },

            tokens: ['Product', 'Cart', 'MyAccount']
        });

        return config;
    }();
})();
