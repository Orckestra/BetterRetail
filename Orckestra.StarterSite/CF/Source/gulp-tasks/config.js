var path = require('path');

(function() {
    'use strict';

    module.exports = function() {

        var argv = require('yargs').argv,
            ecmascriptTarget = 'ES5',
            tempFolder = './.temp',
            outputFolder = './output',
            testsOutputFolder = tempFolder + '/Tests',
            report = './report',
            temporaryTemplatesFolder = tempFolder + '/templates',
            rawTemplates = temporaryTemplatesFolder + '/raw-templates',
            rawResourcesFolder = tempFolder + '/raw-resources',
            isRelease = argv.release !== void 0,


            config = {
                debug: true,
                outputFolder: outputFolder,
                tempFolder: tempFolder,
                dtsOutputFolder: '',
                ecmascriptTarget: ecmascriptTarget,
                testsOutputFolder: testsOutputFolder,

                /**
                 * Filenames
                 */
                filenames: {},

                /**
                 * Globs
                 */
                globs: {
                    all: '**/*',
                    sass: '**/*.scss',
                    javascript: '**/*.js',
                    css: '**/*.css'
                },

                /**
                 * Paths
                 */
                paths: {
                    blades: [],
                    dest: 'Assets/',
                    framework: 'Framework/',
                    images: 'Images/',
                    sass: 'Sass/',
                    css: 'Css/',
                    temp: tempFolder,
                    source: 'Source/',
                    templates: 'Templates/',
                    localizedStrings: 'LocalizedStrings/',
                    tests: 'Tests/',
                    typescript: 'Typescript/',
                    javascript: 'Javascript/',
                    workbench: 'Workbench/',
                    mvcSample: '../Composer.Mvc.Sample',
                    temporaryTemplatesFolder: temporaryTemplatesFolder,
                    rawTemplates: rawTemplates,
                    rawResourcesFolder: rawResourcesFolder
                },

                // Template for source-to-destination copy for each concern
                watchesTemplate: [{
                        srcGlob: 'Source/Images/**/*',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/Images'
                    }, {
                        srcGlob: 'Source/Resources/**/*',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/Resources'
                    }, {
                        srcGlob: 'Source/Sass/**/*.scss',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/Sass'
                    },

                    // I don't think we'll have any in the Composer.UI framework itself, but leaving it for now.
                    {
                        srcGlob: 'Source/Templates/**/*.hbs',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/Templates'
                    }, {
                        srcGlob: 'Source/Javascript/**/*.js',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/Javascript'
                    }, {
                        srcGlob: 'Source/Typings/**/*.d.ts',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/Typings'
                    }, {
                        srcGlob: 'Source/LocalizedStrings/**/*.resx',
                        dst: '../Composer.{{TOKEN}}.UI/Framework/LocalizedStrings'
                    }
                ],

                // Concern-specific Composer UI folders to auto-provision from Composer.UI on changes
                tokens: [],

                typescriptFilesGlob: ['./Source/Typescript/**/*.ts'],
                javascriptFolder: '',
                jsBundleName: '',
                composerFrameworkJavascriptMinifiedFileName: '',
                dtsBundleName: '',
                typingsReferenceForBundledDts: '/// <reference path="./tsd.d.ts" />\n',

                typingsFolder: './Typings',

                karma: (function() {
                    var options = {
                        port: 9876,
                        testsOutputFolder: testsOutputFolder,

                        frameworks: ['jasmine', 'sinon'],

                        files: [],

                        filesToBuild: [],

                        excludedFiles: [],
                        singleRun: true,

                        coverage: {
                            dir: report + 'coverage',
                            reporters: [{
                                type: 'html',
                                subdir: 'report-html'
                            }, {
                                type: 'lcov',
                                subdir: 'report-lcov'
                            }, {
                                type: 'text-summary'
                            }]
                        },

                        reporters: [
                            //'progress', // Gulp output list style report
                            'spec', // Gulp output bdd style report
                            //'coverage', // Generates an html coverage report
                            'html', // Generates an html test report
                            'junit' // Generates xml for Jenkins/Sonar
                        ],

                        junitReporter: {
                            outputDir: path.join('../../', testsOutputFolder, '/test-results/'),
                            outputFile: 'karma.junit.xml', // if included, results will be saved as $outputDir/$browserName/$outputFile
                        },

                        preprocessors: {},

                        browserify: {
                            debug: true,
                            transform: ['brfs']
                        },

                        browsers: ['PhantomJS'],

                        plugins: [
                            'karma-jasmine',
                            'karma-sinon',
                            'karma-chrome-launcher',
                            'karma-phantomjs-launcher',
                            'karma-junit-reporter',
                            'karma-spec-reporter',
                            'karma-html-reporter',
                            'karma-coverage'
                        ],

                        htmlReporter: {
                            outputDir: testsOutputFolder + '/test-results/'
                        }
                    };

                    options.preprocessors[testsOutputFolder + '/js/output.js'] = ['coverage'];

                    return options;
                })(),


                defaultTypescriptSettings: {
                    declarationFiles: true,
                    allowUnusedLabels: false,
                    noResolve: false,
                    noImplicitAny: false,
                    noImplicitReturns: true,
                    noLib: false,
                    removeComments: false,
                    target: ecmascriptTarget
                    // Exclude definition types from node_modules/@types for TypeScript 2+
                    //types: []
                }
            };

            config.debug = argv.debug || true;

            if (argv.debug) {
                config.karma.singleRun = false;
                config.karma.browsers = ['Chrome'];
            }

        return config;
    }();
})();
