(function() {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        config = require('../config.js'),
        helpers = require('./helpers.js'),
        argv = require('yargs').argv,
        runSequence = require('run-sequence').use(gulp);

    /*
     * Cleans the folder where temporary tests files are outputted to.
     */
    gulp.task('unitTests-clean', function(callback) {

        return helpers.clean(config.karma.testsOutputFolder, callback);
    });

    /*
     * Cleans the folder where temporary tests files are outputted to.
     */
    gulp.task('unitTests-clean-reporting', function(callback) {

        return helpers.clean(config.karma.htmlReporter.outputDir, callback);
    });

    /*
     * Builds the unit tests into JavaScript so that they can run with the test runner.
     */
    gulp.task('unitTests-build', function() {

        return helpers.transpileTypeScriptToJs({
                typescriptFilesGlob: config.karma.filesToBuild,
                typeScriptProject: $.typescript.createProject(config.defaultTypescriptSettings),
                debug: config.debug
            })
            .js
            .pipe($.if(config.debug, $.using()))
            .pipe(gulp.dest(config.karma.testsOutputFolder));
    });

    /*
     * Cleans and packages unit tests and any client-side code that the unit tests depend on.
     */
    gulp.task('unitTests-prepare', function(callback) {

        return runSequence(
            'unitTests-clean',
            'unitTests-clean-reporting',
            'unitTests-build-js-framework',
            'unitTests-build',
            callback
        );
    });

    gulp.task('unitTests-build-js-framework', function () {

        return helpers.bundleTypescript({
            typescriptFilesGlob: config.typescriptFilesGlobForUnitTests,
            scriptBundleName: config.jsBundleName,
            dtsBundleName: config.dtsBundleName,
            scriptOutputFolder: config.javascriptFolder,
            dtsOutputFolder: config.dtsOutputFolder,
            typeScriptProject: $.typescript.createProject(config.defaultTypescriptSettings),
            typingsReference: config.typingsReferenceForBundledDts,
            generateSourceMaps: true,
            debug: config.debug
        });
    });

    /*
     * Runs the unit tests, if any in the test runner.
     */
    gulp.task('unitTests', ['unitTests-prepare'], function(callback) {
        return helpers.startUnitTests(config.karma.singleRun, callback);
    });
})();
