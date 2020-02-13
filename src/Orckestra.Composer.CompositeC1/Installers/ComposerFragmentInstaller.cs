using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Composite.Core.Logging;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;
using Composite.Data.Types;
using Composite.C1Console.Trees.Foundation;
using System.Linq;
using Composite.C1Console.Security;

namespace Orckestra.Composer.CompositeC1.Installers
{
    public class ComposerFragmentInstaller : BasePackageFragmentInstaller
    {
        public override IEnumerable<XElement> Install()
        {
            // Sitemap Perspective
            IUserGroupActivePerspective sitemapPerspective = DataFacade.BuildNew<IUserGroupActivePerspective>();
            var userGroup = DataFacade.GetData<IUserGroup>().FirstOrDefault(u => u.Name == "Administrator");
            if (userGroup != null)
            {
                sitemapPerspective.UserGroupId = userGroup.Id;
                EntityToken entityToken = new TreePerspectiveEntityToken("SitemapElement");
                sitemapPerspective.SerializedEntityToken = EntityTokenSerializer.Serialize(entityToken);
                sitemapPerspective.Id = Guid.NewGuid();
                DataFacade.AddNew(sitemapPerspective);
                LoggingService.LogInformation("ComposerExperience", string.Format("Access to the Sitemap Perspective granted for group {0}.", userGroup.Name));
            }

            FixDefaultLanguageUrlMapping();

            yield break;
        }

        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            yield break;
        }

        public void FixDefaultLanguageUrlMapping() {
            using(var conn = new DataConnection())
            {
                var language = conn.Get<ISystemActiveLocale>().FirstOrDefault(l => l.IsDefault);
                if(string.IsNullOrEmpty(language.UrlMappingName))
                {
                    language.UrlMappingName = language.CultureName;
                }
                conn.Update(language);
            }
        }
    }
}

