(function() {
    'use strict';

var gulp = require('gulp'),
    $ = require('gulp-load-plugins')(),
    config = require('../config.js'),
    helpers = require('./helpers.js');

gulp.task('scripts', function () {

    var conf = config.defaultTypescriptSettings;
    conf.removeComments = true;
    return helpers.bundleTypescript({
        typescriptFilesGlob: config.typescriptFilesGlob,
        scriptBundleName: config.jsBundleName,
        dtsBundleName: config.dtsBundleName,
        scriptOutputFolder: config.javascriptFolder,
        dtsOutputFolder: config.dtsOutputFolder,
        typeScriptProject: $.typescript.createProject(conf),
        typingsReference: config.typingsReferenceForBundledDts,
        generateSourceMaps: false,
        debug: config.debug
    });
});
})();

