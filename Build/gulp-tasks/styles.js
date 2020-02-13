//Do we use this source?
(function() {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        config = require('../config.js'),
        helpers = require('./helpers.js'),
        path = require('path'),
        fs = require('fs');


    /**
     * Clean the temp folder.
     * Temp folder is used to bring together all the sass.
     */
    gulp.task('clean-bladeset-temp', function(callback) {
        helpers.clean(config.paths.temp, callback);
    });

    /**
     * Compile styles and delete temp folder after that.
     */
    gulp.task('styles', ['styles-compile'], function(callback) {
        helpers.clean(config.paths.temp, callback);
    });

    /**
     * Compile sass to css and copy it to each workbenches.
     */
    gulp.task('styles-compile', ['styles-bladeset-import'], function() {
        var compiledSass = gulp.src(path.join(config.paths.temp, '**/*.scss'))
            .pipe($.plumber())
            .pipe($.sass());

        return compiledSass
            .pipe(gulp.dest(path.join(config.paths.source, config.paths.css)));
    });

    /**
     * Take the sass in temp folder and run the glob importer to generate ready to compile sass.
     * The glob import is to allow the use of globbing in @import rules.
     */
    gulp.task('styles-bladeset-import', ['styles-blades-to-temp'], function() {

        return gulp.src(path.join(config.paths.temp, '**/*.scss'))
            .pipe($.plumber())
            .pipe($.sassGlobImport())
            .pipe(gulp.dest(config.paths.temp));
    });

    /**
     * Populate a file that @import's all the sass from the blade.
     * Write it to blade's the temp folder.
     */
    gulp.task('styles-blades-to-temp', ['styles-framework-to-temp'], function(cb) {

        /**
         * TODO: clean and comment this function
         * We generate paths from a read of the root folders, because root glob is slow as hell.
         */
        var paths = fs.readdirSync('./');
        paths = paths.filter(function(item) {
            return fs.statSync(item).isDirectory();
        }).map(function(item) {
            var glob = path.join('./', item, config.paths.source, config.paths.sass, '**/*.scss');
            return glob;
        });

        return gulp.src(paths, {
                base: './'
            })
            .pipe(gulp.dest(path.join(config.paths.temp, 'Blades')));
    });

    /**
     * Copy framework to temp folder
     */
    gulp.task('styles-framework-to-temp', ['clean-bladeset-temp'], function() {
        var frameworkSass = path.join(config.paths.framework, config.paths.sass, '**/*.scss');

        return gulp.src(frameworkSass)
            .pipe(gulp.dest(config.paths.temp));
    });
})();
