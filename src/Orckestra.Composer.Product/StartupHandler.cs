using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Core.Routing.Foundation.PluginFacades;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Execution;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;
using Orckestra.Composer.Product.GraphQL;
using Orckestra.Composer.Product.GraphQL.Interfaces;
using Orckestra.Composer.Services.Lookup;

namespace Orckestra.Composer.Product
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            //GraphQL, NOTE: may be move to IRecipeDocumentExecuter and IRecipeDocumentWriter
            collection.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            collection.AddSingleton<IDocumentWriter>(new DocumentWriter(true, new ErrorInfoProvider(options =>
            {
                options.ExposeExceptionStackTrace = false;
                options.ExposeCode = true;
            })));

            collection.AddSingleton<ILookupService, LookupService>();
            collection.AddSingleton<IVariantColorSchema>(provider => new VariantColorSchema(new FuncServiceProvider(type => Get(provider, type))));

        }

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




        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {
            EnsureCreateStore();


        }


        private static void EnsureCreateStore()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dataInterface = typeof(IData);
            var types = assembly.GetExportedTypes();

            foreach (var type in types)
            {
                if (!dataInterface.IsAssignableFrom(type)) continue;
                DynamicTypeManager.EnsureCreateStore(type);
            }
        }
    }
}