using System.Linq;
using System.Reflection;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.DynamicTypes;

namespace Orckestra.Composer.Grocery
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void Start() { }

        public static void OnInitialized()
        {
            EnsureCreateStore();
        }

        private static void EnsureCreateStore()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dataTypes = assembly.GetExportedTypes().Where(typeof(IData).IsAssignableFrom);

            foreach (var type in dataTypes)
            {
                DynamicTypeManager.EnsureCreateStore(type);
            }
        }
    };
}