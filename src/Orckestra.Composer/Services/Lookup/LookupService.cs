using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Services.Lookup
{
    public class LookupService : ILookupService
    {
        private readonly ILookupRepositoryFactory _lookupRepositoryFactory;

        public LookupService(ILookupRepositoryFactory lookupRepositoryFactory)
        {
            _lookupRepositoryFactory = lookupRepositoryFactory;
        }

        //TODO: Delete
        public virtual async Task<List<Overture.ServiceModel.Metadata.Lookup>> GetLookupsAsync(LookupType lookupType)
        {
            ILookupRepository repository = _lookupRepositoryFactory.CreateLookupRepository(lookupType);

            var lookups = await repository.GetLookupsAsync().ConfigureAwait(false);

            return lookups;
        }

        public async Task<string> GetLookupDisplayNameAsync(GetLookupDisplayNameParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var splitLookups = param.Value.Split('|');
            var lookupStringBuilder = new StringBuilder();

            for (int i = 0; i < splitLookups.Length; i++)
            {
                var response = await GetDistinctLookup(param.LookupType, param.LookupName, splitLookups[i], param.CultureInfo).ConfigureAwait(false);
                if (response != null)
                {
                    // don't add a space before the first lookup
                    if (i != 0)
                    {
                        lookupStringBuilder.Append(param.Delimiter);
                    }
                    lookupStringBuilder.Append(response);
                }
            }

            return lookupStringBuilder.ToString();
        }

        public async Task<Dictionary<string, string>> GetLookupDisplayNamesAsync(GetLookupDisplayNamesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var lookupDisplayNames = new Dictionary<string, string>();

            ILookupRepository repository = _lookupRepositoryFactory.CreateLookupRepository(param.LookupType);
            var lookup = await repository.GetLookupAsync(param.LookupName).ConfigureAwait(false);
            if (lookup == null)
            {
                throw new InvalidOperationException("No lookup found for lookup name " + param.LookupName);
            }

            foreach (var lookupValue in lookup.Values)
            {
                var valueDisplayName = lookupValue.DisplayName.GetLocalizedValue(param.CultureInfo.Name);

                if (string.IsNullOrWhiteSpace(valueDisplayName))
                {
                    //TODO: Add log here as a warning.
                    valueDisplayName = string.Format("[{0}:{1}.{2}]", param.LookupType, param.LookupName, lookupValue.Value);
                }

                lookupDisplayNames.Add(lookupValue.Value, valueDisplayName);
            }

            return lookupDisplayNames;
        }

        /// <summary>
        /// Performs lookups for a single value.
        /// Because Overture stores multiple lookup values a pipe (|) delimited strings,
        /// this method should only be called from code that explicitly splits on |
        /// </summary>
        /// <param name="lookupType"></param>
        /// <param name="lookupName"></param>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private async Task<string> GetDistinctLookup(LookupType lookupType, string lookupName, string value,
            CultureInfo cultureInfo)
        {
            ILookupRepository repository = _lookupRepositoryFactory.CreateLookupRepository(lookupType);
            var lookup = await repository.GetLookupAsync(lookupName).ConfigureAwait(false);
            if (lookup == null)
            {
                throw new InvalidOperationException("No lookup found for lookup name " + lookupName);
            }
            var lookupValue = lookup.Values.SingleOrDefault(v => v.Value == value);

            return lookupValue?.DisplayName.GetLocalizedValue(cultureInfo.Name);
        }
    }
}