using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Orckestra.Composer.HttpModules;

namespace Orckestra.Composer
{
    /// <summary>
    /// Entry point to load HTTP modules dynamically.
    /// </summary>
    public class PreApplicationStartCode
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(RequestBranderModule));
            DynamicModuleUtility.RegisterModule(typeof(AntiCookieTamperingModule));
        }
    }
}
