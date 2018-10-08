(function() {
    'use strict';

    var gulp = require('gulp'),
        $ = require('gulp-load-plugins')(),
        del = require('del'),
        requireDir = require('require-dir'),
        fs = require('fs-sync'),
        commonTasksFolder ='../gulp-tasks',
        commonTasksFolderToCopyTo = './Gulp/common';


    /**
     * Clean and populate the common gulp tasks
     */
    $.util.log($.util.colors.green('Syncing tasks from "' + commonTasksFolder + '"'));
    del.sync(commonTasksFolderToCopyTo)
    fs.copy(commonTasksFolder, commonTasksFolderToCopyTo, { force: true });
    $.util.log($.util.colors.green('Done!'));

    /**
     * Require gulp tasks
     */
    requireDir('./Gulp', {recurse: true});


    /**
     * Default task, just list tasks
     */
    gulp.task('default', $.taskListing);

})();
