using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Country
{
    public class CountryService : ICountryService
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ICountryRepository CountryRepository { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public CountryService(
            ICountryRepository countryRepository,
            IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider)
        {
            if (viewModelMapper == null)
            {
                throw new ArgumentNullException("viewModelMapper");
            }
            if (countryRepository == null)
            {
                throw new ArgumentNullException("countryRepository");
            }
            if (localizationProvider == null)
            {
                throw new ArgumentNullException("localizationProvider");
            }

            ViewModelMapper = viewModelMapper;
            CountryRepository = countryRepository;
            LocalizationProvider = localizationProvider;
        }

        /// <summary>
        /// Retrieve the CountryViewModel
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<CountryViewModel> RetrieveCountryAsync(RetrieveCountryParam param)
        {
            if (string.IsNullOrWhiteSpace(param.IsoCode))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("IsoCode"), "param");
            }

            var country = await CountryRepository.RetrieveCountry(param).ConfigureAwait(false);
            var countryViewModel = ViewModelMapper.MapTo<CountryViewModel>(country, param.CultureInfo);

            return countryViewModel;
        }

        /// <summary>
        /// Retrieve the list of RegionViewModel for a specified Country
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<RegionViewModel>> RetrieveRegionsAsync(RetrieveCountryParam param)
        {
            if (string.IsNullOrWhiteSpace(param.IsoCode))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("IsoCode"), "param");
            }
            if (param.CultureInfo == null)
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param");
            }

            var regions = await CountryRepository.RetrieveRegions(param).ConfigureAwait(false);
            var regionsCountryModel =
                regions.Select(region => ViewModelMapper.MapTo<RegionViewModel>(region, param.CultureInfo));

            return regionsCountryModel;
        }

        /// <summary>
        /// Retrieve the display name for a specified region.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<string> RetrieveRegionDisplayNameAsync(RetrieveRegionDisplayNameParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo"); }
            if (string.IsNullOrWhiteSpace(param.IsoCode)) { throw new ArgumentException("param.IsoCode"); }
            if (string.IsNullOrWhiteSpace(param.RegionCode)) { throw new ArgumentException("param.RegionCode"); }

            var retrieveCountryParam = new RetrieveCountryParam
            {
                CultureInfo = param.CultureInfo,
                IsoCode = param.IsoCode
            };

            var regions = await RetrieveRegionsAsync(retrieveCountryParam).ConfigureAwait(false);
            var region = regions.SingleOrDefault(r => r.IsoCode == param.RegionCode);

            return region != null ? region.Name : string.Empty;
        }

        public virtual async Task<string> RetrieveCountryDisplayNameAsync(RetrieveCountryParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo"); }
            if (string.IsNullOrWhiteSpace(param.IsoCode)) { throw new ArgumentException("param.IsoCode"); }
            var country = await RetrieveCountryAsync(param).ConfigureAwait(false);
            return country != null ? country.CountryName : string.Empty;
        }
    }
}
