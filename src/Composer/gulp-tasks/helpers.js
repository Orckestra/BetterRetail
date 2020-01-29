(function() {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        _ = require('lodash'),
        del = require('del'),
        merge = require('merge-stream'),
        path = require('path'),
        fs = require('fs'),
        through = require('through2'),
        config = require('../config.js'),
        execPathFinder = /(^.+\\source)/i,
        argv = require('yargs').argv,

        /**
         * Helper object containing multiple helpers functions
         */
        helpers = {
            /**
             * Logs a message in green.
             * @param message
             */
            log: function(message) {
                if (typeof(message) === 'object') {
                    // iterate the props of the object and log them
                    for (var item in message) {
                        if (message.hasOwnProperty(item)) {
                            $.util.log($.util.colors.green(message[item]));
                        }
                    }
                } else {
                    // log the message as is
                    $.util.log($.util.colors.green(message));
                }
            },

            /**
             * Cleans a folder
             * @param path {string} Path to clean.
             * @param done {function} Callback, called when done.
             */
            clean: function(path, done) {
                this.log('Cleaning: ' + $.util.colors.yellow(path));
                return del(path, done);
            },

            /**
             * Takes in a function that returns a stream.
             * The function will run with each blade in argument.
             * We then merge every resulting stream in a single one and return it.
             */
            eachBladeStreams: function(blades, fn) {
                var mergedStream = merge();

                blades.forEach(function bladeIteration(blade, index) {
                    var stream = fn(blade);
                    mergedStream.add(stream);
                });

                return mergedStream;
            },

            /**
             * Starts the unit tests
             * TODO: The config file should be configurable
             */
            startUnitTests: function(singleRun, done) {
                let karma = require('karma');
                let server = new karma.Server({
                    configFile: path.resolve(__dirname, 'karma.conf.js'),
                    singleRun: singleRun
                },
                function karmaCallback(karmaResult) {
                    helpers.log('Stopping tests with exit code : ' + karmaResult);

                    if (done !== undefined) {
                        done();
                    }
                });

                server.start();
            },

            fileStreamFromString: function(filename, string) {
                var src = require('stream').Readable({
                    objectMode: true
                })
                src._read = function() {
                    this.push(new $.util.File({
                        cwd: "",
                        base: "",
                        path: filename,
                        contents: new Buffer(string)
                    }))
                    this.push(null)
                }
                return src
            },

            directoryExists: function(pathToCheck) {
                try {
                    // Query the entry
                    var stats = fs.lstatSync(pathToCheck);

                    // Is it a directory?
                    if (stats.isDirectory()) {
                        return true;
                    } else {
                        return false;
                    }
                } catch (e) {
                    console.log(e);
                    return false;
                }
            },

            globFromRootFolder: function(options) {
                var defaults = {
                    root: './',
                    glob: null,
                    exclude: null
                };

                if (options) {
                    var options = _.assign(defaults, options);
                }

                var root = options.root;
                var builtGlobs = [];
                var rootPaths = fs.readdirSync(options.root);

                rootPaths = rootPaths.filter(function(item) {
                    var itemPath = path.join(options.root, item);

                    // if not a folder, exclude
                    if (!fs.statSync(itemPath).isDirectory()) {
                        return false;
                    }
                    // if we have exclusion pattern(s)
                    if (options.exclude) {
                        if (_.isArray(options.exclude)) {
                            var matches = false;
                            options.exclude.forEach(function eachExclusions(exclude){
                                // if path of exclusion matches current loop item
                                if (path.resolve(item) == path.resolve(exclude)) {
                                    matches = true;
                                }
                            });
                            // return false to filter out item if we have a match
                            return !matches;
                        } else {
                            // if item does matches exclude pattern filter out
                            return path.resolve(item) !== path.resolve(options.exclude);
                        }
                    }
                    // default keep item
                    return true;
                }).forEach(function(item) {
                    var itemPath = path.join(options.root, item);

                    /**
                     * if glob is string we just append it to current item
                     * if it is an array we loop on it to create multiple globs for each items
                     */
                    if (options.glob) {
                        if (_.isArray(options.glob)) {
                            options.glob.forEach(function eachGlobs(singleGlob){
                                builtGlobs.push(path.join(itemPath, singleGlob));
                            });
                        } else {
                            builtGlobs.push(path.join(itemPath, options.glob));
                        }
                    } else {
                        builtGlobs.push(itemPath);
                    }
                });

                return builtGlobs;
            },


            /**
             * TODO: Comment the options parameter object
             * TODO: also comment the inner workings of the function
             * debug
             * typescriptFilesGlob
             * generateSourceMaps
             * typeScriptProject
             */
            transpileTypeScriptToJs: function(options) {

                return gulp.src(options.typescriptFilesGlob)
                    .pipe($.if(argv.verbose, $.using()))
                    .pipe($.if(options.debug && options.generateSourceMaps, $.sourcemaps.init()))
                    .pipe($.tslint())
                    .pipe($.tslint.report('verbose', { emitError: true }))
                    .pipe($.typescript(options.typeScriptProject));
            },

            /**
             * TODO: Comment the options parameter object
             * TODO: also comment the inner workings of the function
             * debug
             * dtsBundleName
             * dtsOutputFolder
             * scriptBundleName
             * scriptOutputFolder
             * typingsReference
             */
            bundleTypescript: function(options) {

                var stream;
                helpers.log('Bundled scripts will be outputted as ' + options.scriptBundleName + ' to the following folders: ' + options.scriptOutputFolder);
                stream = helpers.transpileTypeScriptToJs(options);
                helpers.log(options.dtsBundleName);

                return merge([
                    stream.dts
                    .pipe($.if(argv.verbose, $.using()))
                    .pipe($.concat(options.dtsBundleName))
                    .pipe($.replace(/\/{3}\s+<reference\s+path="[^"]+"\s+\/>\n?/g, ''))
                    .pipe($.replace(/^/g, options.typingsReference))
                    .pipe(gulp.dest(options.dtsOutputFolder)),

                    stream.js
                    .pipe($.if(argv.verbose, $.using()))
                    .pipe($.concat(options.scriptBundleName))
                    .pipe($.if(options.debug, $.sourcemaps.write()))                    
                    .pipe(gulp.dest(options.scriptOutputFolder))
                ]);
            },
        };

    module.exports = helpers;
})();
