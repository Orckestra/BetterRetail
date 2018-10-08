using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Kernel;

namespace Orckestra.Composer
{
    internal class AspNetConfigurationManager : IAspNetConfigurationManager
    {
        public void Configure(ILifetimeScope lifetimeScope,
            IViewEngine viewEngine,
            MediaTypeFormatter mediaTypeFormatter)
        {

            if (viewEngine == null)
            {
                throw new ArgumentNullException("viewEngine");
            }

            if (mediaTypeFormatter == null)
            {
                throw new ArgumentNullException("mediaTypeFormatter");
            }
            
            RegisterMvc(lifetimeScope, viewEngine);
            RegisterWebApi(lifetimeScope, mediaTypeFormatter);
        }

        private void RegisterMvc(ILifetimeScope lifetimeScope, IViewEngine viewEngine)
        {
            DependencyResolver.SetResolver(new ComposerDependencyResolver(lifetimeScope, DependencyResolver.Current));
            ViewEngines.Engines.Add(viewEngine);

            lifetimeScope.Resolve<ISearchUrlProvider>().RegisterRoutes(RouteTable.Routes);
            lifetimeScope.Resolve<IProductUrlProvider>().RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterWebApi(ILifetimeScope lifetimeScope, MediaTypeFormatter mediaTypeFormatter)
        {
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(lifetimeScope);

            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Clear();
            formatters.Add(mediaTypeFormatter);
        }
    }
}
