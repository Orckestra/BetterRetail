using System.Linq;
using System.Reflection;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.DynamicTypes;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void Start()
        {
        }

        public static void OnInitialized()
        {
            EnsureCreateStore();
            DataTypesEventRegistrator.Initialize();
        }

        private static void EnsureCreateStore()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var dataInterface = typeof(IData);
            var types = assembly.GetExportedTypes()
                .Where(dataInterface.IsAssignableFrom)
                .ToList();

            foreach (var type in types)
            {
                DynamicTypeManager.EnsureCreateStore(type);
            }
        }

    };
}
