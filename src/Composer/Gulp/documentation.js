/* Do we use it? */
(function() {
    'use strict';

    var gulp = require("gulp"),
        $ = require('gulp-load-plugins')(),
        runSequence = require('run-sequence'),
        helpers = require('./common/helpers'),        
        config = require('./config');

    /*
     * Cleans the documentation folder.
     */
    gulp.task('documentation-clean', function() {

        helpers.clean(config.documentationSettings.outputFolder);
    });

    /*
     * Builds the client-side documentation.
     */
    gulp.task('documentation-build', function() {

        return gulp.src(config.typescriptFilesGlob)
            .pipe($.typedoc({
                module: config.documentationSettings.moduleType,
                out: config.documentationSettings.outputFolder,
                name: config.documentationSettings.documentationName,
                target: config.ecmascriptTarget,
                includeDeclarations: config.documentationSettings.includeDeclarations
            }))
            .pipe(gulp.dest(config.documentationSettings.outputFolder));
    });

    /*
     * Cleans and packages the client-side documentation.
     */
    gulp.task('documentation', function(callback) {
        runSequence(
            'documentation-clean',
            'documentation-build',
            callback
        );
    });
})();
