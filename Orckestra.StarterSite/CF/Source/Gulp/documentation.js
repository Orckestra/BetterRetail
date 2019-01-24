(function () {
    'use strict';

    var gulp = require("gulp"),
        typedoc = require('gulp-typedoc/index'),
        helpers = require('./common/helpers'),
        config = require('./config');

    /*
     * Cleans the documentation folder.
     */
    gulp.task('documentation-clean', function () {
        return helpers.clean(config.documentationSettings.outputFolder);
    });

    /*
     * Builds the client-side documentation.
     */
    gulp.task('documentation-build', function () {
        return gulp
            .src(config.typescriptFilesGlob)
            .pipe(typedoc({
                module: config.documentationSettings.moduleType,
                out: config.documentationSettings.outputFolder,
                name: config.documentationSettings.documentationName,
                target: config.ecmascriptTarget,
                includeDeclarations: config.documentationSettings.includeDeclarations,
                ignoreCompilerErrors: config.documentationSettings.ignoreCompilerErrors
            }))
            .pipe(gulp.dest(config.documentationSettings.outputFolder));
    });

    /*
     * Cleans and packages the client-side documentation.
     */
    gulp.task('documentation', gulp.series(
        'documentation-clean',
        'documentation-build'
    ));
})();
