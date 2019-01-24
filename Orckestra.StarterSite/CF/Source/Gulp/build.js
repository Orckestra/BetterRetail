(function () {
    'use strict';

    var gulp = require('gulp');

    gulp.task('build', gulp.series(
        'scripts',
        'syncToProjects'
    ));
})();

