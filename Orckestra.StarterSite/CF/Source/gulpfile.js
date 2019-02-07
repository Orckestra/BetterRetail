'use strict';

var gulp = require('gulp'),
    colors = require('ansi-colors'),
    del = require('del'),
    fs = require('fs-sync'),
    taskListing = require('gulp-task-listing'),
    commonTasksFolder = './gulp-tasks',
    commonTasksFolderToCopyTo = './Gulp/common';

/**
 * Clean and populate the common gulp tasks
 */
console.log(colors.green('Syncing tasks from "' + commonTasksFolder + '"'));
del.sync(commonTasksFolderToCopyTo);
fs.copy(commonTasksFolder, commonTasksFolderToCopyTo, {force: true});
console.log(colors.green('Done!'));

/**
 * Require gulp tasks (the order is important)
 */
require('./Gulp/common/scripts');
require('./Gulp/common/unitTests');
require('./Gulp/sync-projects');
require('./Gulp/package');
require('./Gulp/build');
require('./Gulp/documentation');
require('./Gulp/watch');

/**
 * Default task, just list tasks
 */
gulp.task('default', taskListing);