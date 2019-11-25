using System.Web;

namespace Orckestra.Composer.HttpModules
{
    public interface IAntiCookieTamperingExcluder
    {
        /// <summary>
        /// Determines if the AntiCookieTemperingModule should handle the request.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>True if should handle, false otherwise.</returns>
        bool ShouldHandleRequest(HttpContextBase httpContext);
    }
}
