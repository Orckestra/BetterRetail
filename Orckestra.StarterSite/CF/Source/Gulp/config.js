(function() {
    'use strict';

    module.exports = function() {
        var mainConfiguration = require('./common/config'),
            _ = require('lodash'),
            path = require('path'),
            config = {};

        config = _.merge(mainConfiguration, {

            typescriptFilesGlob: ['./*.UI/*/Source/Typescript/**/*.ts', './*.UI/Source/Typescript/**/*.ts'],
            typescriptFilesGlobForUnitTests: ['./*.UI/*/Source/Typescript/**/*.ts', './*.UI/Source/Typescript/**/*.ts', '!./Composer.UI/Source/Typescript/App.ts'],
            dtsOutputFolder: './UI.Package/Typings',
            javascriptFolder: './UI.Package/JavaScript',
            jsBundleName: 'orckestra.composer.js',
            dtsBundleName: 'orckestra.composer.d.ts',

            karma: {
                files: [
                    '../../3rdParty.ForTests/**/*.js',
                    '../../3rdParty/jquery-1.11.2.min.js',
                    '../../3rdParty/bootstrap.min.js',
                    '../../3rdParty/lodash.min.js',
                    '../../3rdParty/parsley.min.js',
                    '../../3rdParty/handlebars.helpers.js',
					'../../3rdParty/q-1.2.0.js',
					'../../3rdParty/jquery.serialize-object.js',
                    '../../UI.Package/JavaScript/orckestra.composer.js',
                    path.join('../../', mainConfiguration.testsOutputFolder, '/**/*.js')
                ],
                filesToBuild: [
                    'Composer.UI/Source/Tests/unit/**/*.ts',
                    'Composer.*.UI/*/Source/Tests/unit/**/*.ts'
                ]
            },

            tokens: ['Product', 'Cart', 'MyAccount'],

            documentationSettings: {
                documentationName: 'Orckestra Composer',
                outputFolder: './UI.Package/Documentation',
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
			deployedWebsitePath: 'C:/orckestra/composer-c1-cm-dev.develop.orckestra.cloud/WebSite',	
            c1MvcProject: '../../CC1/Source/Composer.CompositeC1/Composer.CompositeC1.Mvc',
        });

        return config;
    }();
})();
