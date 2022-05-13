using System.Globalization;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    public class BuildCategoryBrowsingUrlParam
    {
        public string CategoryId { get; set; }

        public SearchCriteria Criteria { get; set; }

        public CultureInfo CultureInfo
        {
            get
            {
                return Criteria?.CultureInfo;
            }
            set
            {
                if (Criteria == null)
                {
                    Criteria = new SearchCriteria();
                }
                Criteria.CultureInfo = value;
            }
        }

        public string BaseUrl
        {
            get
            {
                return Criteria?.BaseUrl;
            }
            set
            {
                if (Criteria == null)
                {
                    Criteria = new SearchCriteria();
                }
                Criteria.BaseUrl = value;
            }
        }

        public bool IsAllProductsPage { get; set; }
    }
}