(function() {
    'use strict';

    module.exports = function() {
        var mainConfiguration = require('./common/config'),
            _ = require('lodash'),
            path = require('path'),
            config = {};
            let thirdPartyJavaScriptPath = '../../../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/JavaScript/';

        config = _.merge(mainConfiguration, {

            typescriptFilesGlob: ['../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Typescript/**/*.ts'],
            typescriptFilesGlobForUnitTests: ['../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Typescript/**/*.ts', '!../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Typescript/App.ts'],
            dtsOutputFolder: '../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Typings',
            javascriptFolder: '../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/JavaScript',
            jsBundleName: 'orckestra.composer.js',
            dtsBundleName: 'orckestra.composer.d.ts',

            karma: {
                files: [
                    '../../3rdParty.ForTests/**/*.js',
                    path.join(thirdPartyJavaScriptPath, 'jquery-1.11.2.min.js'),
                    path.join(thirdPartyJavaScriptPath, 'lodash.min.js'),
                    path.join(thirdPartyJavaScriptPath, 'parsley.min.js'),
                    path.join(thirdPartyJavaScriptPath, 'handlebars.helpers.js'),
                    path.join(thirdPartyJavaScriptPath, 'q-1.2.0.js'),
                    path.join(thirdPartyJavaScriptPath, 'jquery.serialize-object.js'),
                    path.join(thirdPartyJavaScriptPath, 'typeahead.js'),
                    '../../../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Javascript/orckestra.composer.js',
                    path.join('../../', mainConfiguration.testsOutputFolder, '/**/*.js')
                ],
                filesToBuild: ['../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Tests/**/*.ts']
            },

            tokens: ['Product', 'Cart', 'MyAccount'],

            documentationSettings: {
                documentationName: 'Orckestra Composer',
                outputFolder: '../Composer.CompositeC1/Composer.CompositeC1.Mvc/UI.Package/Documentation',
                moduleType: 'commonjs',
                includeDeclarations: true
            },

            watch: {
                delays: {
                    composerAssemblies: 1000,
                },
            },

            composerAssemblies: ['./Packaging/bin/Debug/*.dll', '!./Packaging/bin/Debug/Packaging.dll'],
			composerc1Assemplies: [''],
			deployedWebsitePath: '../../deployment/WebSite'
        });

        return config;
    }();
})();
