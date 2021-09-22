using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using Composite.Core;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Orckestra.Caching;
using Orckestra.Composer.CompositeC1.Builders;
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.CompositeC1.Installers
{
    public class PreConfigurationInstaller : BasePackageFragmentInstaller
    {
        public override IEnumerable<XElement> Install()
        {
            Log.LogInformation("PreConfigurationInstaller", "Start");


            // Default dependency is not registered in Console Mode
            var builder = new ContainerBuilder();
            builder.Register(c => ComposerOvertureClient.CreateFromConfig()).As<IOvertureClient>().SingleInstance();
            builder.RegisterType<NullCacheProvider>().As<ICacheProvider>();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>().SingleInstance();
            builder.RegisterType<CategoryAndNavigationBuilder>().As<ICategoryAndNavigationBuilder>().SingleInstance();
            var container = builder.Build();

            var categoryAndNavigationBuilder = container.Resolve<ICategoryAndNavigationBuilder>();

            var displayNames = new Dictionary<string, string>();

            Configuration.SingleOrDefault(f => f.Name == "MainMenuDisplayNames")?.Elements("add").ToList()
                .ForEach(d =>
            {
                var locale = d.Attributes("locale").Select(a => a.Value).FirstOrDefault();
                var displayName = d.Value;
                if (!string.IsNullOrWhiteSpace(locale))
                {
                    displayNames[locale] = displayName;
                    Log.LogInformation("PreConfigurationInstaller", $"{locale}: '{displayName}'");
                }
            });

            categoryAndNavigationBuilder.ReBuildCategoriesAndMenu(displayNames);

            Log.LogInformation("PreConfigurationInstaller", "End");
            yield break;
        }

        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            yield break;
        }
    }

    public class PreConfigurationUninstaller : BasePackageFragmentUninstaller
    {
        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            yield break;
        }

        public override void Uninstall()
        {
        }
    }
}
