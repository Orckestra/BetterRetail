module.exports = function(config) {
    var path = require('path'),
        thirdPartyJavaScriptPath = '../src/Orckestra.Composer.Website/UI.Package/JavaScript/',
        testsOutputFolder = './.temp/Tests';
    
    //TODO: add ESLint and Coverage
    //TODO: check do we use all third party java scripts for tests
    //html reporter removed since its dependencies outdated and we have junit reporter to see file log
    
    config.set({
        frameworks: ['jasmine', 'sinon'],
        files: 
        [
            '../tests/3rdParty.ForTests/**/*.js',
            path.join(thirdPartyJavaScriptPath, 'jquery-1.11.2.min.js'),
            path.join(thirdPartyJavaScriptPath, 'lodash-4.17.21.min.js'),
            path.join(thirdPartyJavaScriptPath, 'parsley.min.js'),
            path.join(thirdPartyJavaScriptPath, 'q-1.2.0.js'),
            path.join(thirdPartyJavaScriptPath, 'jquery.serialize-object.js'),
            path.join(thirdPartyJavaScriptPath, 'typeahead.js'),
            '../src/Orckestra.Composer.Website/UI.Package/Javascript/orckestra.composer.tests.js',
            path.join(testsOutputFolder, '/**/*.js')],
        filesToBuild: ['../src/Orckestra.Composer.Website/UI.Package/Tests/**/*.ts'],
        reporters: 
        [
            'spec',
            'junit'
        ],

        testsOutputFolder: testsOutputFolder,
        colors: true,

        // level of logging	
        // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
        logLevel: config.LOG_INFO,
        
        browsers: ['PhantomJS'],
        
        plugins: 
        [
            'karma-jasmine',
            'karma-sinon',
            'karma-chrome-launcher',
            'karma-phantomjs-launcher',
            'karma-spec-reporter',
            'karma-junit-reporter'
        ],

        junitReporter: {
            outputDir: testsOutputFolder + '/test-results/',
            outputFile: 'karma.junit.xml'
        },

        singleRun: true      
    });
};