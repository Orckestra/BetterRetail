using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Builders
{
    public interface ICategoryAndNavigationBuilder
    {
        void ReBuildCategoriesAndMenu(Dictionary<string, string> localizedDisplayNames);
    }
}
