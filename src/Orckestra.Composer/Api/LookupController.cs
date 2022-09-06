using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orckestra.Composer.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class LookupController : ApiController
    {
        private ILookupService LookupService { get; set; }
        public IComposerContext ComposerContext { get; set; }

        public LookupController(ILookupService lookupService,
           IComposerContext composerContext)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        [HttpPost]
        [ActionName("localizedLookupValues")]
        public virtual async Task<IHttpActionResult> GetLocalizedLookupValues(GetLookupDisplayNamesParam param)
        {
            var lookup = await LookupService.GetLookupAsync(param.LookupType, param.LookupName).ConfigureAwait(false);

            var lookupValuesResult = lookup.Values.OrderBy(lookupValue => lookupValue.SortOrder)
             .Where(lookupValue => lookupValue.IsActive)
             .ToDictionary(key => key.Value, value => value.DisplayName.GetLocalizedValue(ComposerContext.CultureInfo.Name));

            return Ok(lookupValuesResult);
        }
    }
}
