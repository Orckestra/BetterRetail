(function() {
    'use strict';

    module.exports = function() {
        var mainConfiguration = require('./common/config'),
            _ = require('lodash'),
            path = require('path'),
            config = {};

        config = _.merge(mainConfiguration, {

            filenames: {
                sassImports: '_blades.scss'
            },

            typescriptFilesGlob: ['./*/Source/Typescript/**/*.ts'],
            javascriptFolder: './Source/JavaScript',
            dtsOutputFolder: './Source/Typings',
            jsBundleName: 'bladeset.js',
            composerFrameworkJavascriptMinifiedFileName: 'bladeset.min.js',
            dtsBundleName: 'bladeset.d.ts',

            karma: {
                files: [
                    path.join(__dirname, '../../3rdParty.ForTests/**/*.js'),
                    path.join(__dirname, '../../3rdParty/q-1.2.0.js'),
                    path.join(__dirname, '../../3rdParty/jquery-1.11.2.min.js'),
                    path.join(__dirname, '../../3rdParty/lodash.min.js'),
                    path.join(__dirname, '../../3rdParty/parsley.min.js'),
                    path.join(__dirname, '../../3rdParty/handlebars.helpers.js'),
                    path.join(__dirname, '../Framework/JavaScript/orckestra.composer.js'),
                    path.join(__dirname, '../Source/JavaScript/bladeset.js'),
                    path.join(__dirname, '../', mainConfiguration.testsOutputFolder) + '/**/*.spec.js'
                ],

                filesToBuild: [
                    './*/Source/Tests/unit/**/*.spec.ts'
                ]
            },

            paths: {
                blades: [
                    'ProductDetail',
                    'ProductMerchandiser',
                ],
            },

            tokens: ["ProductDetail", "ProductTile", "ProductSearch", "ProductMerchandiser", "ProductPicker"]
        });

        return config;
    }();
})();
