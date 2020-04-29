using System;
using Autofac;
using Orckestra.Overture;

namespace Orckestra.Composer.Kernel
{
    internal sealed class AutofacDependancyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _container;

        public AutofacDependancyResolver(ILifetimeScope container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public object TryResolve(Type type)
        {
            return _container.TryResolve(type, out object result) ? result : null;
        }

        public T TryResolve<T>() where T : class
        {
            return _container.TryResolve(typeof(T), out object result) ? (T)result : null;
        }
    }
}
