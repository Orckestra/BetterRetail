(function () {
    'use strict';

    var gulp = require('gulp'),
        _ = require('lodash'),
        argv = require('yargs').argv,
        colors = require('ansi-colors'),
        concat = require('gulp-concat'),
        declare = require('gulp-declare'),
        del = require('del'),
        gulpif = require('gulp-if'),
        handlebars = require('gulp-handlebars'),
        log = require('fancy-log'),
        path = require('path'),
        uglify = require('gulp-uglify'),
        using = require('gulp-using'),
        wrap = require('gulp-wrap'),

        /**
         * Helper object containing multiple helpers functions
         */
        helpers = {
            /**
             * Logs a message in green.
             * @param message
             */
            log: function (message) {
                if (typeof(message) === 'object') {
                    // iterate the props of the object and log them
                    for (var item in message) {
                        if (message.hasOwnProperty(item)) {
                            log(colors.green(message[item]));
                        }
                    }
                } else {
                    // log the message as is
                    log(colors.green(message));
                }
            },

            /**
             * Cleans a folder
             * @param path {string} Path to clean.
             * @param done {function} Callback, called when done.
             */
            clean: function (path, done) {
                this.log('Cleaning: ' + colors.yellow(path));
                return del(path, done);
            },

            /**
             * Starts the unit tests
             * TODO: The config file should be configurable
             */
            startUnitTests: function (singleRun, done) {
                var karma = require('karma').server;
                karma.start({
                        configFile: path.resolve(__dirname, 'karma.conf.js'),
                        singleRun: singleRun
                    },
                    function karmaCallback(karmaResult) {
                        helpers.log('Stopping tests with exit code : ' + karmaResult);

                        if (done !== undefined) {
                            done();
                        }
                    }
                );
            },

            getClientSideTemplatesCompiler: function (compilerOptions) {

                var templatesFolder = compilerOptions.templatesFolder,
                    compiledTemplateDestinationFolder = compilerOptions.compiledTemplateDestinationFolder,
                    templatesBundleName = compilerOptions.templatesBundleName,
                    templateNamespace = compilerOptions.templateNamespace || 'Orckestra.Composer.Templates',
                    templateFilesGlob = templatesFolder + '/*.hbs',
                    packagedTemplatesName = templatesBundleName + '.js',
                    minifiedPackagedTemplatesName = templatesBundleName + '.min.js';

                helpers.log('Compiling handlebars templates from ' + colors.yellow(templatesFolder));

                return gulp.src([templateFilesGlob])
                    .pipe(gulpif(argv.verbose, using()))
                    .pipe(handlebars())
                    .pipe(wrap('Handlebars.template(<%= contents %>)'))
                    .pipe(declare({
                        namespace: templateNamespace,
                        noRedeclare: true // Avoid duplicate declarations
                    }))
                    .pipe(concat(packagedTemplatesName))
                    .on('end', function () {
                        helpers.log('Outputting compiled handlebars templates to ' + colors.yellow(compiledTemplateDestinationFolder));
                    })
                    .pipe(gulp.dest(compiledTemplateDestinationFolder))
                    .on('end', function () {
                        helpers.log('Create minified version of the compiled handlebars templates');
                    })
                    .pipe(uglify())
                    .pipe(concat(minifiedPackagedTemplatesName))
                    .pipe(gulp.dest(compiledTemplateDestinationFolder));
            }
        };

    module.exports = helpers;
})();
