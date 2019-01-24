(function () {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        config = require('./config'),
        plumber = require('gulp-plumber'),
        baseGlobDirectory = './*.UI/**/';

    gulp.task('watch:styles', function () {
        var watched = [baseGlobDirectory + 'Sass/*.scss', baseGlobDirectory + 'Sass/**/*.scss'];
        gulp.watch(watched, gulp.series('package-styles', 'package-copy-mvc'));
    });

    gulp.task('watch:templates', function () {
        var watched = [baseGlobDirectory + 'Templates/*.hbs', baseGlobDirectory + 'Typescript/**/*.ts'];
        gulp.watch(watched, gulp.series('package-clean', 'package-templates', 'package-copy-mvc'));
    });

    gulp.task('watch:scripts', function () {
        var watched = [baseGlobDirectory + 'Typescript/*.ts', baseGlobDirectory + 'Typescript/**/*.ts'];
        gulp.watch(watched, gulp.series('package-clean', 'package-scripts', 'package-copy-mvc'));
    });

    gulp.task('watch:assemblies', function () {
        gulp.watch(config.composerAssemblies, $.batch(

            {timeout: config.watch.delays.composerAssemblies},
            gulp.series('package-clean', 'package-copy-dll'))
        );
    });

    gulp.task('watch', gulp.parallel('watch:styles', 'watch:templates', 'watch:scripts', 'watch:assemblies'));
})();
