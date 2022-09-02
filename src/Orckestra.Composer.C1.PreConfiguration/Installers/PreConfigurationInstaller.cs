extern alias occ;
using Autofac;
using Composite.Core;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Orckestra.Composer.CompositeC1.Builders;
using Orckestra.Composer.OutputCache;
using Orckestra.Composer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using occ::Orckestra.Overture.Caching;

namespace Orckestra.Composer.CompositeC1.Installers
{
    public class PreConfigurationInstaller : BasePackageFragmentInstaller
    {
        public override IEnumerable<XElement> Install()
        {
            Log.LogInformation(nameof(PreConfigurationInstaller), "Start");

            try
            {
                // Default dependency is not registered in Console Mode
                var builder = new ContainerBuilder();
                builder.Register(c => ComposerOvertureClient.CreateFromConfig()).As<IComposerOvertureClient>().SingleInstance();
                builder.RegisterType<NullCacheProvider>().As<ICacheProvider>();
                builder.RegisterType<CategoryRepository>().As<ICategoryRepository>().SingleInstance();
                builder.RegisterType<CategoryAndNavigationBuilder>().As<ICategoryAndNavigationBuilder>().SingleInstance();
                var container = builder.Build();

                var categoryAndNavigationBuilder = container.Resolve<ICategoryAndNavigationBuilder>();

                Dictionary<string, string> displayNames = null;

                var mainMenuDisplayNames = Configuration.SingleOrDefault(f => f.Name == "MainMenuDisplayNames");

                if (mainMenuDisplayNames != null)
                {
                    displayNames = new Dictionary<string, string>();
                    mainMenuDisplayNames.Elements("add").ToList()
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
                }


                categoryAndNavigationBuilder.ReBuildCategoriesAndMenu(displayNames);

            }
            catch (Exception ex) {
                Log.LogError(nameof(PreConfigurationInstaller), ex);
            }
            Log.LogInformation(nameof(PreConfigurationInstaller), "End");
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
