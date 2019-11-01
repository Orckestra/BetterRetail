using System;
using System.Collections.Generic;
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
        protected IComposerRequestContext ComposerContext { get; private set; }

        public AddressController(ICountryService countryService, IComposerRequestContext composerContext)
        {
            if (countryService == null) { throw new ArgumentNullException("countryService"); }
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }

            CountryService = countryService;
            ComposerContext = composerContext;
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

            IEnumerable<RegionViewModel> regionsOrdered = null;

            if (regions != null)
            {
                regionsOrdered = regions.OrderBy(r => r.Name);
            }

            return Ok(regionsOrdered);
        }
    }
}
