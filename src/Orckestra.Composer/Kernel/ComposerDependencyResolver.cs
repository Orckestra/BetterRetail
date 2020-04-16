using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orckestra.Composer.Kernel
{
    public class ComposerDependencyResolver : AutofacDependencyResolver
    {

        public ComposerDependencyResolver(ILifetimeScope container, IDependencyResolver innerResolver) : base(container) {
            InnerResolver = innerResolver;
        }

        public IDependencyResolver InnerResolver { get;}

        public override object GetService(Type serviceType)
        {
            return base.GetService(serviceType) ?? InnerResolver?.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return base.GetServices(serviceType) ?? InnerResolver?.GetServices(serviceType);
        }
    }
}
