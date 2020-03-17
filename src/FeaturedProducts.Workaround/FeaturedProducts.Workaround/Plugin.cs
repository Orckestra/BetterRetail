using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer;
using Orckestra.Composer.Search.Repositories;

namespace FeaturedProducts.Workaround
{
    public class Plugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<WorkaroundProductRequestFactory, IProductRequestFactory>();
        }
    }
}
