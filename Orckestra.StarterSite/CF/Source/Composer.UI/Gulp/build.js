'use strict';

// Imports
    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        merge = require('merge-stream'),
        path = require('path'),
        helpers = require('./common/helpers.js'),
        runSequence = require('run-sequence'),
        config = require('./config.js');

/*
* Builds an entire bladeset, e.g. scripts, images, SASS etc.
 */
gulp.task('build', function (callback) {
    runSequence(
        'scripts',
        'styles',
        'unitTests',
        callback
    );
});
