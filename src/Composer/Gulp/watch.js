
(function() {
    'use strict';

    var gulp = require("gulp"),
         $ = require('gulp-load-plugins')(),
         runSequence = require('run-sequence'),
         config = require('./config'),
         path = require('path'),
		 plumber = require('gulp-plumber');

    gulp.task('watch', function(callback){
        var baseGlobDirectory = './*.UI/**/';

        plumber();

        $.watch([path.join(baseGlobDirectory, 'Typescript/*.ts'), path.join(baseGlobDirectory, 'Typescript/**/*.ts')], function(){
            runSequence('package-clean', 'package-scripts', 'package-copy-mvc');
        });

        $.watch(config.composerAssemblies, $.batch({ timeout: config.watch.delays.composerAssemblies }, function(done) {
            runSequence('package-clean', 'package-copy-dll', function() {
                // Must call done in an anonymous function, because runSequence only accept a function.
                done();
            });
        }));
    });
})();
