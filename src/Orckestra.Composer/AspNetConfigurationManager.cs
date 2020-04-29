using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;
using Orckestra.Composer.Kernel;

namespace Orckestra.Composer
{
    internal class AspNetConfigurationManager : IAspNetConfigurationManager
    {
        public void Configure(ILifetimeScope lifetimeScope, IViewEngine viewEngine, MediaTypeFormatter mediaTypeFormatter)
        {
            if (viewEngine == null) { throw new ArgumentNullException(nameof(viewEngine));}
            if (mediaTypeFormatter == null) { throw new ArgumentNullException(nameof(mediaTypeFormatter)); }
            
            RegisterMvc(lifetimeScope, viewEngine);
            RegisterWebApi(lifetimeScope, mediaTypeFormatter);
        }

        private void RegisterMvc(ILifetimeScope lifetimeScope, IViewEngine viewEngine)
        {
            DependencyResolver.SetResolver(new ComposerDependencyResolver(lifetimeScope, DependencyResolver.Current));
            ViewEngines.Engines.Add(viewEngine);

            //TODO: ISearchUrlProvider and IProductUrlProvider is PerRequest now
            //lifetimeScope.Resolve<ISearchUrlProvider>().RegisterRoutes(RouteTable.Routes);
            //lifetimeScope.Resolve<IProductUrlProvider>().RegisterRoutes(RouteTable.Routes);
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
