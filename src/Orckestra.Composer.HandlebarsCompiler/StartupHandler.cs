using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Hosting;
using Orckestra.Composer.HandlebarsCompiler.Services;
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

        public static void OnInitialized(IFileWatcherService fileWatcherService, IHandlebarsCompileService compileService)
        {
            if (!HostingEnvironment.IsHosted) return;

            var templatesPath = HostingEnvironment.MapPath(HandlebarsCompileConfig.TemplatesPath);
            var compiledFile = HostingEnvironment.MapPath(HandlebarsCompileConfig.CompiledFilePath);

            if (!File.Exists(compiledFile))
            {
                compileService.PrecompileHandlebarsTemplate(compiledFile, templatesPath);
            }

            if (HandlebarsCompileConfig.IsEnabled)
            {
                fileWatcherService.WatchOnDirectory(templatesPath, () => compileService.PrecompileHandlebarsTemplate(compiledFile, templatesPath));
            }
        }
    }
}
