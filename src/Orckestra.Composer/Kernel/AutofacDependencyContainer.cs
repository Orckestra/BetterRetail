using System;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Orckestra.Overture;

namespace Orckestra.Composer.Kernel
{
    internal class AutofacDependencyContainer
    {
        private readonly ContainerBuilder _containerBuilder;

        internal AutofacDependencyContainer()
        {
            _containerBuilder = new ContainerBuilder();
        }

        public ILifetimeScope Build()
        {
            return _containerBuilder.Build();
        }

        public void Register<T>(T instance)
        {
            _containerBuilder.RegisterInstance((object)instance).As<T>().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
        }

        public void Register<TAs>(object instance)
        {
            _containerBuilder.RegisterInstance(instance).As<TAs>().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
        }

        public void RegisterAs<T, TAs>() where T : TAs
        {
            RegisterAs<T, TAs>(ComponentLifestyle.Transient);
        }

        public void RegisterAs<T, TAs>(ComponentLifestyle lifestyle) where T : TAs
        {
            if (lifestyle == ComponentLifestyle.PerRequest)
            {
                _containerBuilder.RegisterType<T>().As<TAs>().InstancePerLifetimeScope().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
            }
            else if (lifestyle == ComponentLifestyle.Singleton)
            {
                _containerBuilder.RegisterType<T>().As<TAs>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
            }
            else if (lifestyle == ComponentLifestyle.Transient)
            {
                _containerBuilder.RegisterType<T>().As<TAs>().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lifestyle), lifestyle, null);
            }
        }

        public void Register(Type implementationType, ComponentLifestyle lifestyle, params Type[] asType)
        {
            var builder = _containerBuilder.RegisterType(implementationType);

            if (asType.Length != 0)
            {
                builder = builder.As(asType);
            }

            if (lifestyle == ComponentLifestyle.PerRequest)
            {
                builder.InstancePerLifetimeScope();
            }
            else if (lifestyle == ComponentLifestyle.Singleton)
            {
                builder.SingleInstance();
            }
        }

        public void Register<T>(ComponentLifestyle lifestyle)
        {
            Register(typeof(T), lifestyle);
        }

        public void Register<T>()
        {
            Register(typeof(T), ComponentLifestyle.Transient);
        }

        public void Register(Type implementationType)
        {
            Register(implementationType, ComponentLifestyle.Transient);
        }

        public void Register(Type implementationType, ComponentLifestyle lifestyle)
        {
            Register(implementationType, lifestyle, new Type[] { });
        }

        public void Register(Type implementationType, Type asType)
        {
            Register(implementationType, asType, ComponentLifestyle.Transient);
        }

        public void Register(Type implementationType, Type asType, ComponentLifestyle lifestyle)
        {
            Register(implementationType, lifestyle, new[] { asType });
        }

        public void Register<T, TAs>(ComponentLifestyle lifestyle) where T : TAs
        {
            Register(typeof(T), lifestyle, new[] { typeof(TAs) });
        }

        public void Register<T, TAs>() where T : TAs
        {
            Register(typeof(T), ComponentLifestyle.Transient, new[] { typeof(TAs) });
        }

        public void RegisterControllers(Assembly assembly)
        {
            _containerBuilder.RegisterControllers(assembly);
        }

        public void RegisterApiControllers(Assembly assembly)
        {
            _containerBuilder.RegisterApiControllers(assembly);
        }

        public void RegisterExceptionFiltersForApiControllers(params Type[] filterTypes)
        {
            _containerBuilder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            foreach (var type in filterTypes)
            {
                _containerBuilder.RegisterType(type)
                                 .AsWebApiExceptionFilterFor<ApiController>()
                                 .InstancePerRequest();
            }
        }

        public void RegisterModule(IModule mo)
        {
            _containerBuilder.RegisterModule(mo);
        }
    }
}
