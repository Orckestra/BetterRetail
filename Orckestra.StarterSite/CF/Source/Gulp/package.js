(function () {
    'use strict';

    var gulp = require('gulp'),
        argv = require('yargs').argv,
        autoprefixer = require('gulp-autoprefixer'),
        colors = require('ansi-colors'),
        config = require('./config'),
        filter = require('gulp-filter'),
        fs = require('fs'),
        fsSync = require('fs-sync'),
        gulpif = require('gulp-if'),
        helpers = require('./common/helpers'),
        path = require('path'),
        rename = require('gulp-rename'),
        sass = require('gulp-sass'),
        sassGlob = require('gulp-sass-glob'),
        through = require('through2'),
        using = require('gulp-using');

    // TODO: paths are hardcoded here, need to find a solution for global config
    var dest = 'UI.Package/';

    var changePath = function (file, env, callback) {
        // filter out directories
        // we don't need to copy and operate on empty directories
        if (fs.statSync(file.path).isDirectory()) {
            callback();
        } else {
            // remove the absolute portion of the path
            var sourcePath = path.join(path.resolve(path.join(__dirname, '../')), '/');
            file.path = file.path.replace(sourcePath, '');

            // split the path into an array of directories
            var parsedPath = path.parse(file.path),
                dirParts = parsedPath.dir.split(path.sep);

            /**
             * We assume array should have this kind of structure:
             * Bladeset/Blade/Source/Asset/*
             * (ex: Composer.Product.UI/ProductDetail/Source/Asset/*)
             *
             * Which will give us an array with the following contents
             * [0]: Bladeset (ex: Composer.Product.UI)
             * [1]: Blade (ex: ProductDetail)
             * [2]: Source (ex: Source)
             * [3]: Asset folder (ex: Sass)
             * [4]: *
             *
             */

            /**
             * If the second folder is not Source, we know it is a Blade.
             * Otherwise, it means it is a framework's (Composer.UI) folder,
             */
            if (dirParts[1] !== 'Source') {
                /**
                 * We insert the blade's name at position 4 (inside the asset folder)
                 * So we get (temporarily)
                 * Bladeset/Blade/Source/Asset/Blade/*
                 */
                dirParts.splice(4, 0, dirParts[1]);

                /**
                 * Then remove the directories at position 1 and 2.
                 * Bladeset/[Blade/Source/]Asset/Blade/*
                 * So we get
                 * Bladeset/Asset/Blade/*
                 */
                dirParts.splice(1, 2);

                /**
                 * If it is the Sass folder, we add a subfolder
                 */
                if (dirParts[1] === 'Sass') {
                    dirParts.splice(2, 0, 'Blades');
                }
            }
            // remove root folder
            dirParts.splice(0, 1);

            parsedPath.dir = dirParts.join(path.sep);

            file.path = path.join(path.format(parsedPath));

            callback(null, file);
        }
    };


    gulp.task('package-watch', function (callback) {
        var watch = [
            'Composer.UI/Source/**/*',
            'Composer.*.UI/**/Source/**/*'
        ];
        gulp.watch(watch, ['package']);
    });

    gulp.task('package-clean', function (callback) {
        return helpers.clean(dest, callback);
    });

    gulp.task('package-copy-thirdparty', function () {
        var thirdPartyScripts = '3rdParty/*.js';

        return gulp.src(thirdPartyScripts)
            .pipe(gulp.dest(path.join(dest, 'JavaScript')));
    });

    gulp.task('package-copy-mvc', function () {
        helpers.log('Copying ' + dest);
        var c1Site = path.join('C:/orckestra/composer-c1-cm-dev.develop.orckestra.cloud/WebSite', dest),
            c1MvcProject = path.join('../../CC1/Source/Composer.CompositeC1/Composer.CompositeC1.Mvc', dest);

        helpers.log('to: ' + c1Site);
        helpers.log('and to: ' + c1MvcProject);

        return gulp.src(dest + '**/*')
            .pipe(gulp.dest(c1Site))
            .pipe(gulp.dest(c1MvcProject));
    });

    gulp.task('package-copy-dll', function () {
        function copyAssembliesTo(projectLocation) {
            // Open packages.config file in sitecore mvc
            var composerPackagesConfigPath = path.join(projectLocation, 'packages.config'),
                composerPackagesConfigContent = fsSync.read(composerPackagesConfigPath);

            // Extract version number for Composer
            var match = composerPackagesConfigContent.match(/<package id="Composer" version="(.*?)" targetFramework=".*?" \/>/i);
            if (match.length < 2) {
                throw new Error('Cannot find Composer version in ' + composerPackagesConfigPath);
            }
            var version = match[1];

            // Copy assemblies to package destination
            var destinationFolder = path.join(projectLocation, '../', 'packages/Composer.' + version, 'lib/net452');

            helpers.log('Assemblies will be copied from ' + config.composerAssemblies + ' to ' + destinationFolder);

            return gulp.src(config.composerAssemblies)
                .pipe(gulpif(argv.verbose, using()))
                .pipe(gulp.dest(destinationFolder));
        }

        return copyAssembliesTo(config.c1MvcProject);
    });

    gulp.task('package-sass', function () {
        return gulp.src(dest + 'Sass/' + 'composer.scss')
            .pipe(gulpif(argv.verbose, using()))
            .pipe(sassGlob())
            .pipe(sass())
            .pipe(autoprefixer({
                browsers: [
                    'last 3 versions',
                    'Explorer >= 8',
                    'iOS 7',
                    'last 10 Chrome version',
                    'last 5 Firefox version'
                ]
            }))
            .pipe(gulp.dest(dest + '/css'));
    });

    /**
     * TODO: exclusion array in config
     */
    var filters = ['**/*',
        '!**/Typescript',
        '!**/Typescript/**/*',
        '!**/Tests',
        '!**/Tests/**/*',
        '!**/Javascript',
        '!**/Javascript/**/*',
        '!**/Templates/**/*'
    ];

    gulp.task('package-blades-sass', function () {
        return gulp.src('./*.UI/*/Source/Sass/*')
            .pipe(gulpif(argv.verbose, using()))
            .pipe(through.obj(changePath))
            .pipe(gulpif(argv.verbose, using()))
            .pipe(gulp.dest(dest));
    });

    gulp.task('package-framework', function () {
        var src = 'Composer.UI/Source/**/*',
            f = filter(filters);

        return gulp.src(src)
            .pipe(f)
            .pipe(gulp.dest(dest));
    });

    gulp.task('package-templates-merge-all-templates-to-one-folder', function () {
        helpers.log('Copy all handlebars templates to ' + colors.yellow(config.paths.rawTemplates));
        return gulp.src(['./*.UI/*/Source/Templates/**/*.hbs', './*.UI/Source/Templates/**/*.hbs'])
            .pipe(gulpif(argv.verbose, using()))
            .pipe(rename({
                dirname: ''
            }))
            .pipe(gulp.dest(config.paths.rawTemplates));
    });

    gulp.task('package-templates-merge-all-resx-to-one-folder', function () {
        helpers.log('Copy all resx files to ' + colors.yellow(config.paths.rawResourcesFolder));
        return gulp.src(['./*.UI/*/Source/LocalizedStrings/**/*.resx', './*.UI/Source/LocalizedStrings/**/*.resx'])
            .pipe(gulpif(argv.verbose, using()))
            .pipe(rename({
                dirname: ''
            }))
            .pipe(gulp.dest(config.paths.rawResourcesFolder));
    });

    gulp.task('package-templates-clean', function (callback) {
        return helpers.clean(config.paths.temporaryTemplatesFolder, callback);
    });


    gulp.task('package-templates-copy-hbs-to-ui-package', function () {
        var builtTemplatesFolder = path.join(__dirname, '../', config.paths.rawTemplates),
            uiPackageTemplatesFolder = path.join(__dirname, '../UI.Package/', config.paths.templates);

        helpers.log('Copy handlebars templates from ' + colors.yellow(builtTemplatesFolder) + ' to ' + colors.yellow(uiPackageTemplatesFolder));
        return gulp.src([builtTemplatesFolder + '/*.hbs'])
            .pipe(gulp.dest(uiPackageTemplatesFolder));
    });

    gulp.task('package-templates-copy-localized-strings-to-ui-package', function () {
        var builtResourcesFolder = path.join(__dirname, '../', config.paths.rawResourcesFolder),
            uiPackageLocalizedStringsFolder = path.join(__dirname, '../UI.Package/', config.paths.localizedStrings);

        helpers.log('Copy resx files from ' + colors.yellow(builtResourcesFolder) + ' to ' + colors.yellow(uiPackageLocalizedStringsFolder));
        return gulp.src([builtResourcesFolder + '/**/*.resx'])
            .pipe(gulp.dest(uiPackageLocalizedStringsFolder));
    });

    gulp.task('package-templates-compile-for-client-side', function () {
        return helpers.getClientSideTemplatesCompiler({
            templatesFolder: path.join(__dirname, '../', config.paths.rawTemplates),
            compiledTemplateDestinationFolder: path.join(__dirname, '../UI.Package/', config.paths.javascript),
            templatesBundleName: 'composer-templates'
        });
    });

    gulp.task('package-templates-if-any-exist', function (callback) {
        var fs = require('fs'),
            rawTemplatesFolder = path.join(__dirname, '../', config.paths.rawTemplates);

        if (fs.existsSync(rawTemplatesFolder)) {
            helpers.log('Handlebars templates found!');
            return gulp.series(
                'package-templates-compile-for-client-side',
                'package-templates-copy-hbs-to-ui-package',
                'package-templates-copy-localized-strings-to-ui-package'
            )(callback);
        } else {
            helpers.log('There are no templates to deploy.');
            callback();
        }
    });

    gulp.task('package-styles', gulp.series(
        'package-clean',
        'package-framework',
        'package-blades-sass',
        'package-sass'
    ));

    gulp.task('package-templates', gulp.series(
        'package-templates-clean',
        'package-templates-merge-all-templates-to-one-folder',
        'package-templates-merge-all-resx-to-one-folder',
        'package-templates-if-any-exist'
    ));


    gulp.task('package', gulp.series(
        'package-clean',
        'package-framework',
        'scripts',
        'package-copy-thirdparty',
        //'unitTests',
        'package-templates',
        'package-blades-sass',
        'package-sass'
    ));

    gulp.task('devPackage', gulp.series(
        'package',
        gulp.parallel(
            'package-copy-mvc',
            'package-copy-dll'
        )
    ));
})();
