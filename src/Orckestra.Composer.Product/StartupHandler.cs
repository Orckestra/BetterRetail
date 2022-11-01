using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Execution;
using Orckestra.Composer.Product.GraphQL;
using Orckestra.Composer.Product.GraphQL.Interfaces;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.GraphQL;

namespace Orckestra.Composer.Product
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IVariantColorDocumentExecuter, VariantColorDocumentExecuter>();
            collection.AddSingleton<IVariantColorDocumentWriter>(new VariantColorDocumentWriter(true, new ErrorInfoProvider(options =>
            {
                options.ExposeExceptionStackTrace = false;
                options.ExposeCode = true;
            })));

            collection.AddSingleton<ILookupService, LookupService>();
            collection.AddSingleton<IVariantColorSchema>(provider => new VariantColorSchema(new FuncServiceProvider(type => StartupHandlerHelper.Get(provider, type))));

        }

        public static void OnBeforeInitialize()
        {
        }

        public static void OnInitialized()
        {
        }
    }
}