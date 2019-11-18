using System.Globalization;
using System.Threading.Tasks;
namespace Orckestra.Composer.Services
{
    public interface IHomeViewService
    {
        /// <summary>
        /// Get the copyright
        /// </summary>
        /// <param name="culture">culture for optional links, urls and alt texts</param>
        /// <returns>string corresponding to the copyright</returns>
        Task<string> GetCopyright(CultureInfo culture);
    }
}
