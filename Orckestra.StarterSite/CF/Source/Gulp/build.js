(function() {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        merge = require('merge-stream'),
        path = require('path'),
        helpers = require('./common/helpers.js'),
        runSequence = require('run-sequence'),
        config = require('./config.js');


    gulp.task('build', function() {
        runSequence(
            'scripts',
            'syncToProjects'
        );
    });
})();

