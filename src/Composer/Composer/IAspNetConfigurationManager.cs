using System.Net.Http.Formatting;
using System.Web.Mvc;
using Autofac;

namespace Orckestra.Composer
{
    interface IAspNetConfigurationManager
    {
        void Configure(ILifetimeScope lifetimeScope,
            IViewEngine viewEngine,
            MediaTypeFormatter mediaTypeFormatter);


    }
}
