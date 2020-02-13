(function() {
    'use strict';

    module.exports = function() {
        var mainConfiguration = require('./common/config'),
            _ = require('lodash'),
            path = require('path'),
            config = {};
            let thirdPartyJavaScriptPath = '../../../src/Orckestra.Composer.Website/UI.Package/JavaScript/';

        config = _.merge(mainConfiguration, {

            typescriptFilesGlob: ['../src/Orckestra.Composer.Website/UI.Package/Typescript/**/*.ts'],
            typescriptFilesGlobForUnitTests: ['../src/Orckestra.Composer.Website/UI.Package/Typescript/**/*.ts', '!../src/Orckestra.Composer.Website/UI.Package/Typescript/App.ts'],
            dtsOutputFolder: '../src/Orckestra.Composer.Website/UI.Package/Typings',
            javascriptFolder: '../src/Orckestra.Composer.Website/UI.Package/JavaScript',
            jsBundleName: 'orckestra.composer.js',
            dtsBundleName: 'orckestra.composer.d.ts',

            karma: {
                files: [
                    '../../../tests/3rdParty.ForTests/**/*.js',
                    path.join(thirdPartyJavaScriptPath, 'jquery-1.11.2.min.js'),
                    path.join(thirdPartyJavaScriptPath, 'lodash.min.js'),
                    path.join(thirdPartyJavaScriptPath, 'parsley.min.js'),
                    //no such js, this js should be added or this line to deleted
                    path.join(thirdPartyJavaScriptPath, 'handlebars.helpers.js'),
                    path.join(thirdPartyJavaScriptPath, 'q-1.2.0.js'),
                    path.join(thirdPartyJavaScriptPath, 'jquery.serialize-object.js'),
                    path.join(thirdPartyJavaScriptPath, 'typeahead.js'),
                    '../../../src/Orckestra.Composer.Website/UI.Package/Javascript/orckestra.composer.js',
                    path.join('../../', mainConfiguration.testsOutputFolder, '/**/*.js')
                ],
                filesToBuild: ['../src/Orckestra.Composer.Website/UI.Package/Tests/**/*.ts']
            },

            tokens: ['Product', 'Cart', 'MyAccount'],

            documentationSettings: {
                documentationName: 'Orckestra Composer',
                outputFolder: '../src/Orckestra.Composer.Website/UI.Package/Documentation',
                moduleType: 'commonjs',
                includeDeclarations: true
            },

            watch: {
                delays: {
                    composerAssemblies: 1000,
                },
            }
        });

        return config;
    }();
})();
