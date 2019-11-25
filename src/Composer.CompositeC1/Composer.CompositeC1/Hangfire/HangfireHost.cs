using System;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;
using Autofac;
using Autofac.Core;
using Hangfire;
using Hangfire.CompositeC1;

namespace Orckestra.Composer.CompositeC1.Hangfire
{
    public class HangfireHost : IDisposable, IRegisteredObject
    {
        public static HangfireHost Current { get; set; } = new HangfireHost();

        public static bool IsEnabled
        {
            get
            {
                // Switch to disable hangfire, for example per environment 
                // Use App Setting in web.config like <add key="hangfire:AutomaticAppStartup" value="false" />
                var autoStartConfig = (ConfigurationManager.AppSettings["hangfire:AutomaticAppStartup"] ?? "true");

                return (autoStartConfig.ToLower() != "false");
            }
        }

        public IContainer Container { get; protected set; }

        protected BackgroundJobServer _backgroundJobServer;

        protected HangfireHost()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public virtual void Init(params IModule[] modules)
        {
            // Hangfire
            // NOTE (SIMON.BERUBE):
            // By using HangfireAspNet.Use we ensure that the host will be correctly disposed on application shutdown.
            // HangfireAsp.Use will only call the delegate once
            // So the method will be called from the Application_Start or the 
            // HangfireHostApplicationPreload entry point.
            HangfireAspNet.Use(() =>
            {
                // NOTE (SIMON.BERUBE):
                // Hangfire needs to use an IoC container to recreate the scheduled job
                // We can't reuse the one used the ComposerHost because we don't access to it
                // So we are creating a totally new container and some dependencies in it are
                // based on the ComposerHost container.
                var builder = new ContainerBuilder();
                modules.ToList().ForEach(module => builder.RegisterModule(module));
                Container = builder.Build();

                GlobalConfiguration.Configuration.UseLogProvider(new HangfireLogProvider());
                GlobalConfiguration.Configuration.UseCompositeC1Storage();
                GlobalConfiguration.Configuration.UseAutofacActivator(Container);

                _backgroundJobServer = new BackgroundJobServer();

                return new[] { this };
            });
        }

        public virtual void Dispose()
        {
            if (_backgroundJobServer != null)
            {
                _backgroundJobServer.Dispose();
            }
        }

        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
        }
    }
}