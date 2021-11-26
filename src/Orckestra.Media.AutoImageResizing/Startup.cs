using Composite.Core.Application;
using Composite.Core.WebClient.Renderings.Page;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Media.AutoImageResizing
{
    [ApplicationStartup]
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPageContentFilter>(new ImageResizer());
        }
    }
}
