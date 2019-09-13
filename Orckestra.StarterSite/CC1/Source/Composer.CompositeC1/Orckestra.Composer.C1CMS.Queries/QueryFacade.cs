using Orckestra.Composer.Configuration;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.Repositories;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Orckestra.Composer.C1CMS.Queries
{
    public class QueryFacade
    {
        public static List<string> GetSearchQueryList(string type)
        {
            SearchQueryType queryType;
            Enum.TryParse(type, out queryType);

            var searchQueryRepository = ComposerHost.Current.Resolve<ISearchQueryRepository>();
            var composerConfigurationSection = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;
            if (composerConfigurationSection == null)
            {
                throw new InvalidOperationException();
            }

            var queries = searchQueryRepository.GetSearchQueriesAsync(new GetSearchQueriesParam()
            {
                Scope = "BetterRetailCanada", // TODO: SiteConfiguration.GetScopeId(),
                QueryType = queryType
            }).Result;

            return queries.SearchQueries.Select(d => d.Name).ToList();
        }
    }
}
