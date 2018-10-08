// Karma configuration
// Generated on Wed Mar 18 2015 13:15:38 GMT-0400 (Eastern Daylight Time)

module.exports = function(config) {

    var gulpConfig = require("../config");

    config.set({
        // base path that will be used to resolve all patterns (eg. files, exclude)
        basePath: '',

        // frameworks to use
        // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
        frameworks: gulpConfig.karma.frameworks,

        // list of files / patterns to load in the browser
        files: gulpConfig.karma.files,

        // list of files to exclude
        exclude: gulpConfig.karma.excludedFiles,

        // preprocess matching files before serving them to the browser
        // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
        preprocessors: gulpConfig.karma.preprocessors,

        // test results reporter to use
        // possible values: 'dots', 'progress'
        // available reporters: https://npmjs.org/browse/keyword/karma-reporter
        reporters: gulpConfig.karma.reporters,

        // web server port
        port: gulpConfig.karma.port,

        // enable / disable colors in the output (reporters and logs)
        colors: true,

        // level of logging
        // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
        logLevel: config.LOG_INFO,

        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: true,

        // start these browsers
        // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
        browsers: gulpConfig.karma.browsers,

        plugins: gulpConfig.karma.plugins,

        htmlReporter: gulpConfig.karma.htmlReporter,

        junitReporter: gulpConfig.karma.junitReporter,

        // Continuous Integration mode
        // if true, Karma captures browsers, runs the tests and exits
        singleRun: gulpConfig.karma.singleRun
    });
};
