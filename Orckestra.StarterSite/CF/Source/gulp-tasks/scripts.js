(function () {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        config = require('../config.js'),
        helpers = require('./helpers.js'),
        typeScriptProject = $.typescript.createProject(config.defaultTypescriptSettings);

    gulp.task('scripts', function () {

        return helpers.bundleTypescript({
            typescriptFilesGlob: config.typescriptFilesGlob,
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
})();

