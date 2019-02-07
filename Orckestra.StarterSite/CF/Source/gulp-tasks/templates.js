(function () {
    'use strict';

    var gulp = require('gulp'),
        config = require('../config'),
        helpers = require('./helpers'),
        argv = require('yargs').argv,
        path = require('path'),
        bladeTemplateConfiguration,
        log = require('fancy-log'),

        getBladeTemplateConfiguration = function (bladeName) {

            var bladeFolder;

            if (bladeName === void 0) {
                throw new Error('The bladename was not specified. Run the task like this: gulp templates --blade SomeBladeName');
            }

            // TODO: Composer.UI has the Source folder as a subfolder whereas all other blades have a blade name then the source folder
            // This is why we currently check if the name is composerui to treat it differently. Will need to check with Pat if it's safe to move this to
            // say Composer.UI/Composer/Source/... so that we can avoid the manipulation of the bladefolder.
            bladeFolder = path.join(__dirname, '../../', bladeName.toLowerCase() === 'composerui' ? '' : bladeName);

            return {
                bladeName: bladeName.toLowerCase(),
                temporaryFolder: path.join(bladeFolder, config.paths.temporaryTemplatesFolder),
                templatesFolder: path.join(bladeFolder, 'Source/Templates'),
                rawTemplatesFolder: path.join(bladeFolder, config.paths.rawTemplates),
                compiledTemplatesFolder: path.join(bladeFolder, 'Source/JavaScript'),
                localizedStringsFolder: path.join(bladeFolder, config.paths.rawResourcesFolder)
            };
        };

    if (argv.blade === void 0) {
        log('Loading Handlebars template tasks aborted as no --blade parameter was specified. This is not an error. It just means you ' +
            'will not have access to template tasks. To have access to template tasks run the task like this: gulp templates --blade SomeBladeName');
        return;
    }

    bladeTemplateConfiguration = getBladeTemplateConfiguration(argv.blade);

    gulp.task('templates-clean', function (callback) {

        return helpers.clean(bladeTemplateConfiguration.temporaryFolder, callback);
    });

    gulp.task('templates-compile-for-client-side', function () {

        return helpers.getClientSideTemplatesCompiler({
            templatesFolder: bladeTemplateConfiguration.rawTemplatesFolder,
            compiledTemplateDestinationFolder: bladeTemplateConfiguration.compiledTemplatesFolder,
            templatesBundleName: bladeTemplateConfiguration.bladeName + '-templates'
        });
    });


    gulp.task('templates', gulp.series(
        'templates-clean',
        'templates-compile-for-client-side',
        'templates-clean'
    ));
})();
