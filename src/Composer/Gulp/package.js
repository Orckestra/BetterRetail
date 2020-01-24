(function () {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        merge = require('merge-stream'),
        path = require('path'),
        fs = require('fs'),
        fsSync = require('fs-sync'),
        handlebars = require('handlebars'),
        glob = require('glob'),
        helpers = require('./common/helpers'),
        config = require('./config'),
        runSequence = require('run-sequence').use(gulp),
        through = require('through2'),
        argv = require('yargs').argv;

    // TODO: paths are hardcoded here, need to find a solution for global config
    var dest = 'UI.Package/';
	
    var changePath = function (file, env, callback) {
        // filter out directories
        // we don't need to copy and operate on empty directories
        if (fs.statSync(file.path).isDirectory()) {
            callback();
        } else {
            // remove the absolute portion of the path
            var sourcePath = path.join(path.resolve(path.join(__dirname, '../')), '/');

            var filePath = file.path.replace(sourcePath, '');
            file.path = filePath;

            // split the path into an array of directories
            var parsedPath = path.parse(file.path);
            var dirParts = parsedPath.dir.split(path.sep);

            /**
             * We assume array should have this kind of structure:
             * Bladeset/Blade/Source/Asset/*
             * (ex: Composer.Product.UI/ProductDetail/Source/Asset/*)
             *
             * Which will give us an array with the following contents
             * [0]: Bladeset (ex: Composer.Product.UI)
             * [1]: Blade (ex: ProductDetail)
             * [2]: Source (ex: Source)
             * [3]: Asset folder (ex: Sass)
             * [4]: *
             *
             */

            /**
             * If the second folder is not Source, we know it is a Blade.
             * Otherwise, it means it is a framework's (Composer.UI) folder,
             */
            if (dirParts[1] !== 'Source') {
                /**
                 * We insert the blade's name at position 4 (inside the asset folder)
                 * So we get (temporarily)
                 * Bladeset/Blade/Source/Asset/Blade/*
                 */
                dirParts.splice(4, 0, dirParts[1]);

                /**
                 * Then remove the directories at position 1 and 2.
                 * Bladeset/[Blade/Source/]Asset/Blade/*
                 * So we get
                 * Bladeset/Asset/Blade/*
                 */
                dirParts.splice(1, 2);

                /**
                 * If it is the Sass folder, we add a subfolder
                 */
                if (dirParts[1] === 'Sass') {
                    dirParts.splice(2, 0, 'Blades');
                }
            }
            // remove root folder
            dirParts.splice(0, 1);

            var newDir = dirParts.join(path.sep);
            parsedPath.dir = newDir;

            file.path = path.join(path.format(parsedPath));

            callback(null, file);
        }
    };

    gulp.task('package-watch', function (callback) {
        var watch = [
            'Composer.UI/Source/**/*',
            'Composer.*.UI/**/Source/**/*'
        ];
        gulp.watch(watch, ['package']);
    });

    gulp.task('package-clean', function (callback) {
        return helpers.clean(dest, callback);
    });

     gulp.task('package-copy-mvc', function() {
         helpers.log('Copying ' + dest);
         var c1Site = path.join(config.deployedWebsitePath, dest);

         return gulp.src(path.join(dest, '**/*'))
             .pipe(gulp.dest(c1Site));
    });

    gulp.task('package-scripts', function (callback) {
        var bladeSourceDir = './*.UI/*/Source',
            bladeSetSourceDir = './*.UI/Source';
       var paths = [path.join(bladeSourceDir, 'Typescript/**/*.ts'), path.join(bladeSetSourceDir, 'Typescript/**/*.ts')];
        var typeScriptProject = $.typescript.createProject(config.defaultTypescriptSettings);

        return helpers.bundleTypescript({
            typescriptFilesGlob: paths,
            scriptBundleName: config.jsBundleName,
            dtsBundleName: config.dtsBundleName,
            scriptOutputFolder: config.javascriptFolder,
            dtsOutputFolder: config.dtsOutputFolder,
            typeScriptProject: typeScriptProject,
            typingsReference: config.typingsReferenceForBundledDts,
            generateSourceMaps: true,
            debug: config.debug
        });
    });


    /**
     * TODO: exclusion array in config
     */
    var filters = ['**/*',
        '!**/Typescript',
        '!**/Typescript/**/*',
        '!**/Tests',
        '!**/Tests/**/*',
        '!**/Javascript',
        '!**/Javascript/**/*',
    ];


    gulp.task('package-framework', function () {
        var src = 'Composer.UI/Source/**/*';

        var filter = $.filter(filters);

        return gulp.src(src)
            .pipe(filter)
            .pipe(gulp.dest(dest));
    });

    /*
     * Runs the unit tests, if any in the test runner.
     */
    gulp.task('package-run-unit-tests', ['unitTests-prepare'], function (callback) {
        helpers.startUnitTests(config.karma.singleRun, callback);
    });

    gulp.task('package', function (callback) {
        runSequence(
            'package-clean',
            'package-framework',
            'package-scripts',
            //'package-run-unit-tests',
            callback
        );
    });

    gulp.task('devPackage', function (callback) {
        runSequence(
            'package',
            'package-copy-mvc',
            callback
        );
    });
})();
