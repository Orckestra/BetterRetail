(function () {
    'use strict';

    var gulp = require('gulp'),
        colors = require('ansi-colors'),
        config = require('../config.js'),
        del = require('del'),
        fs = require('fs'),
        helpers = require('./helpers.js'),
        path = require('path'),
        replace = require('gulp-replace'),
        run = require('gulp-run-command').default,
        tslint = require('gulp-tslint');

    /**
     * Rename the node_modules/@types folder to prevent tsc-plus from reading them
     */
    gulp.task('rename-types-pre', function(done) {
        fs.rename('./node_modules/@types', './node_modules/@types_', function (err) {
            if (err) {
                helpers.log(colors.green('./node_modules/@types not found, skipping rename'));
            }
            done();
        });
    });
    gulp.task('rename-types-post', function(done) {
        fs.rename('./node_modules/@types_', './node_modules/@types', function (err) {
            if (err) {
                helpers.log(colors.green('./node_modules/@types_ not found, skipping rename'));
            }
            done();
        });
    });

    /**
     * Run TSLint on TypeScript files.
     * See tslint.json for configuration
     */
    gulp.task('tslint', function () {
        var bladeSourceDir = './*.UI/*/Source',
            bladeSetSourceDir = './*.UI/Source';
        var paths = [path.join(bladeSourceDir, 'Typescript/**/*.ts'), path.join(bladeSetSourceDir, 'Typescript/**/*.ts')];

        return gulp.src(paths)
            .pipe(tslint({
                formatter: 'verbose'
            }))
            .pipe(tslint.report({
                emitError: true
            }))
    });

    /**
     * Compile the TypeScript to JavaScript using the command-line of TypeScript-Plus.
     * It also generates the map file and typings.
     * See tsconfig.json for configuration.
     */
    gulp.task('compile-typeScript', run('tsc-plus'));

    /**
     *  Move the generated orckestra.composer.d.ts to the Typings folder
     */
    gulp.task('move-typings', function () {
        helpers.log('Move orckestra.composer.d.ts to the UI.Package/Typings folder');
        gulp.src('./UI.Package/Javascript/orckestra.composer.d.ts')
            .pipe(replace(/\/{3}\s+<reference\s+path="[^"]+"\s+\/>\n?/g, ''))
            .pipe(replace(/^/g, config.typingsReferenceForBundledDts))
            .pipe(gulp.dest('./UI.Package/Typings/'));

        return del('./UI.Package/Javascript/orckestra.composer.d.ts');
    });

    gulp.task('scripts', gulp.series('tslint', 'rename-types-pre', 'compile-typeScript', 'rename-types-post', 'move-typings'));
})();

