(function () {
    'use strict';

    var gulp = require('gulp'),
        colors = require('ansi-colors'),
        config = require('./config.js'),
        helpers = require('./common/helpers.js'),
        merge = require('merge-stream'),
        path = require('path'),
        plumber = require('gulp-plumber');


    /**
     * Copy directly source file to destination without any
     * other modifications.
     */
    function syncItem(watchItem) {
        return gulp.src(watchItem.srcGlob, {
            read: false
        })
            .pipe(plumber())
            .pipe(gulp.dest(watchItem.dst));
    }

    /**
     * Watch all 'watches' and copy directly source file to destination without any
     * other modifications.
     */
    gulp.task('syncToProjects', function () {

        //helpers.log('sync-projects: Loading...');

        // Check if needed vars are declared
        if (typeof config.tokens === 'undefined') {
            helpers.log(colors.yellow('"tokens" variable is undefined, skipping sync-project wire-up!'));
            helpers.log(colors.yellow('sync-project: Aborted!'));
            return;
        }
        if (typeof config.watchesTemplate === 'undefined') {
            helpers.log(colors.yellow('"watchesTemplate" variable is undefined, skipping sync-project wire-up!'));
            helpers.log(colors.yellow('sync-project: Aborted!'));
            return;
        }

        /**
         * Source and exclusions.
         */
        var src = [
            'Source/**/*',
            '!Source/Tests/',
            '!Source/Tests/**/*',
            '!Source/Typescript/',
            '!Source/Typescript/**/*',
        ];

        var sources = gulp.src(src);

        var merged = merge();
        config.tokens.forEach(function (token) {
            var name = 'Composer.{{TOKEN}}.UI';
            name = name.replace('{{TOKEN}}', token);

            helpers.log('../' + name + 'Framework');

            merged.add(
                sources
                    .pipe(gulp.dest(path.resolve('../', name, 'Framework')))
            );
        });

        return merged;
    });
})();
