using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Country;
using Orckestra.Composer.Services;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class AddressController : ApiController
    {
        protected ICountryService CountryService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public AddressController(ICountryService countryService, IComposerContext composerContext)
        {
            CountryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        /// <summary>
        /// Get the regions for the current Customer country.
        /// </summary>
        /// <returns>A Json representation of the regions</returns>
        [HttpGet]
        [ActionName("regions")]
        public async Task<IHttpActionResult> GetRegions()
        {
            var regions = await CountryService.RetrieveRegionsAsync(new RetrieveCountryParam
            {
                IsoCode = ComposerContext.CountryCode,
                CultureInfo = ComposerContext.CultureInfo,
            }).ConfigureAwait(false);

            return Ok(regions?.OrderBy(r => r.Name));
        }
    }
}