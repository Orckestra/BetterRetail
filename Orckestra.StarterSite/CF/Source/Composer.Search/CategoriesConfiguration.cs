using System.Collections.Generic;

namespace Orckestra.Composer.Search
{
    public static class CategoriesConfiguration
    {
        public static int LandingPageMaxLevel { get; set; }

        public static bool GenerateAllProductsPage { get; set; }

        public static Dictionary<string, object> CategoriesSyncConfiguration { get; set; }

        static CategoriesConfiguration()
        {
            LandingPageMaxLevel = 1;
            GenerateAllProductsPage = true;
            CategoriesSyncConfiguration = new Dictionary<string, object>();
        }
    }
}
