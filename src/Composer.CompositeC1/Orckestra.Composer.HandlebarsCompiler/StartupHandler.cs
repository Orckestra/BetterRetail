using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Hosting;
using Orckestra.Composer.HandlebarsCompiler.Services;
using Composite.Core;
using Orckestra.Composer.HandlebarsCompiler.Config;
using System.IO;

namespace Orckestra.Composer.HandlebarsCompiler
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize()
        {

        }

        public static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IFileWatcherService, FileWatcherService>();
            collection.AddTransient<IHandlebarsCompileService, HandlebarsCompileService>();
        }

        public static void OnInitialized()
        {
            var templatesPath = HostingEnvironment.MapPath(HandlebarsCompileConfig.TemplatesPath);
            var compiledFile = HostingEnvironment.MapPath(HandlebarsCompileConfig.CompiledFilePath);

            if (!File.Exists(compiledFile))
            {
                var handlebarsCompileService = ServiceLocator.GetService<IHandlebarsCompileService>();
                handlebarsCompileService.PrecompileHandlebarsTemplate(compiledFile, templatesPath);
            }

            if (HandlebarsCompileConfig.IsEnabled)
            {
                var fileWatcherService = ServiceLocator.GetService<IFileWatcherService>();
                fileWatcherService.WatchOnDirectory(templatesPath, () => {
                    var compileService = ServiceLocator.GetService<IHandlebarsCompileService>();
                    compileService.PrecompileHandlebarsTemplate(compiledFile, templatesPath);
                });
            }
        }
    }
}
