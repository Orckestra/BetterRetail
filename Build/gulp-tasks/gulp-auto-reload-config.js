(function() {
    'use strict';

    var gulp = require('gulp'),
        argv = require('yargs').argv,
        spawn = require('child-proc').spawn;

    /*
    *   Auto-reload gulp if it's gulpfile.js changes.
    *   
    *   @example gulp gulp-auto-reload name-of-your-task
     */
    gulp.task('gulp-auto-reload', function() {
        var process;

        gulp.watch(['gulpfile.js', '../gulp-tasks/**/*.js', './Gulp/**/*.js'], spawnChildren);
        spawnChildren();

        function spawnChildren(e) {
            if (process) {
                process.kill();
            }
            process = spawn('gulp', [argv.task], {
                stdio: 'inherit'
            });
        }
    });
})();
