(function () {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        config = require('../config.js'),
        helpers = require('./helpers.js'),
        //argv = require('yargs').argv,
        typeScriptProject = $.typescript.createProject(config.defaultTypescriptSettings);

    /*
     * Cleans the folder where temporary tests files are outputted to.
     */
    gulp.task('unitTests-clean', function (callback) {
        return helpers.clean(config.karma.testsOutputFolder, callback);
    });

    /*
     * Cleans the folder where temporary tests files are outputted to.
     */
    gulp.task('unitTests-clean-reporting', function (callback) {
        return helpers.clean(config.karma.htmlReporter.outputDir, callback);
    });

    /*
     * Builds the unit tests into JavaScript so that they can run with the test runner.
     */
    gulp.task('unitTests-build', function () {
        return helpers.transpileTypeScriptToJs({
            typescriptFilesGlob: config.karma.filesToBuild,
            typeScriptProject: typeScriptProject,
            debug: config.debug
        })
            .js
            .pipe($.if(config.debug, $.using()))
            .pipe(gulp.dest(config.karma.testsOutputFolder));
    });

    gulp.task('unitTests-build-js-framework', function () {
        return helpers.bundleTypescript({
            typescriptFilesGlob: config.typescriptFilesGlobForUnitTests,
            scriptBundleName: config.jsBundleName,
            dtsBundleName: config.dtsBundleName,
            scriptOutputFolder: config.javascriptFolder,
            dtsOutputFolder: config.dtsOutputFolder,
            typeScriptProject: typeScriptProject,
            typingsReference: config.typingsReferenceForBundledDts,
            generateSourceMaps: true,
            debug: config.debug
        });
    });

    /*
     * Cleans and packages unit tests and any client-side code that the unit tests depend on.
     */
    gulp.task('unitTests-prepare', gulp.series(
        'unitTests-clean',
        'unitTests-clean-reporting',
        'unitTests-build-js-framework',
        'unitTests-build'
    ));

    /*
     * Runs the unit tests, if any in the test runner.
     */
    gulp.task('unitTests',
        gulp.series('unitTests-prepare', function (callback) {
            return helpers.startUnitTests(config.karma.singleRun, callback);
        })
    );
})();
