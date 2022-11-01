using System;
using System.Linq;
using System.Reflection;

namespace Orckestra.Composer.GraphQL
{
    public class StartupHandlerHelper
    {

        public static object Get(IServiceProvider provider, Type serviceType)
        {
            var type = provider.GetService(serviceType);
            if (type != null)
            {
                return type;
            }

            if (!serviceType.GetTypeInfo().IsAbstract)
            {
                return CreateInstance(provider, serviceType);
            }

            throw new InvalidOperationException("No registration for " + serviceType);
        }

        private static object CreateInstance(IServiceProvider provider, Type implementationType)
        {
            var ctor = implementationType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            var parameterTypes = ctor?.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes?.Select(d => Get(provider, d)).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
