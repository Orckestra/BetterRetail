(function () {
    'use strict';

    var gulp = require('gulp'),
        config = require('../config.js'),
        helpers = require('./helpers.js'),
        run = require('gulp-run-command').default;

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

    /**
     * Compile the TypeScript for the unit tests
     */
    gulp.task('compile-typeScript-karma', run('tsc-plus --p tsconfig.karma.json'));

    gulp.task('unitTests-build', gulp.series('tslint', 'rename-types-pre', 'compile-typeScript-karma', 'rename-types-post'));

    gulp.task('unitTests-build-js-framework', gulp.series('tslint', 'rename-types-pre', 'compile-typeScript', 'rename-types-post', 'move-typings'));

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
            helpers.startUnitTests(config.karma.singleRun, callback);
        })
    );
})();
