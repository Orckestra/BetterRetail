using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Hosting;
using Orckestra.Composer.HandlebarsCompiler.Services;
using Composite.Core;
using Orckestra.Composer.HandlebarsCompiler.Config;
using System.IO;
using Orckestra.Composer.CompositeC1.Services;

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
            collection.AddSingleton<IScheduler, Scheduler>();
        }

        public static void OnInitialized()
        {
            var templatesPath = HostingEnvironment.MapPath(HandlebarsCompileConfig.TemplatesPath);
            var compiledFile = HostingEnvironment.MapPath(HandlebarsCompileConfig.CompiledFilePath);

            if (!File.Exists(compiledFile))
            {
                StartCompileHadlebarsTask(compiledFile, templatesPath, 1);
            }

            if (HandlebarsCompileConfig.IsEnabled)
            {
                var fileWatcherService = ServiceLocator.GetService<IFileWatcherService>();
                fileWatcherService.WatchOnDirectory(templatesPath, () => StartCompileHadlebarsTask(compiledFile, templatesPath, 1));
            }
        }

        private static void StartCompileHadlebarsTask(string compiledFile, string templatesPath, int delayInSeconds)
        {
            var schedulerService = ServiceLocator.GetService<IScheduler>();
            const string jobName = "PrecompileHandlebars";
            schedulerService.ScheduleTask(() => {
                var compileService = ServiceLocator.GetService<IHandlebarsCompileService>();
                compileService.PrecompileHandlebarsTemplate(compiledFile, templatesPath);
            }, jobName, delayInSeconds);
        }
    }
}
