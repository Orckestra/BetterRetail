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
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
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
            object result = null;

            return _container.TryResolve(type, out result) ? result : null;
        }

        public T TryResolve<T>() where T : class
        {
            object result = null;
            return _container.TryResolve(typeof(T), out result) ? (T)result : null;
        }
    }
}
