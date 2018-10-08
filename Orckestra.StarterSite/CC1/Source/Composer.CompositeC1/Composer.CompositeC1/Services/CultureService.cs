using System;
using System.Globalization;
using System.Linq;
using Composite.Data;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class CultureService : ICultureService
    {
        private readonly Lazy<CultureInfo> _defaultCulture;
        private readonly Lazy<CultureInfo[]> _allSupportedCultures;

        protected CultureInfo DefaultCulture
        {
            get { return _defaultCulture.Value; }
        }

        protected CultureInfo[] AllSupportedCultures
        {
            get { return _allSupportedCultures.Value; }
        }

        public CultureService()
        {
            _defaultCulture = new Lazy<CultureInfo>(() => DataLocalizationFacade.DefaultLocalizationCulture);
            _allSupportedCultures = new Lazy<CultureInfo[]>(() =>
            {
                return DataLocalizationFacade.ActiveLocalizationCultures.ToArray();
            });
        }

        public CultureInfo GetDefaultCulture()
        {
            return DefaultCulture;
        }

        public CultureInfo[] GetAllSupportedCultures()
        {
            return AllSupportedCultures;
        }

        public CultureInfo[] GetSupportedCulturesExcept(CultureInfo skipCulture)
        {
            return AllSupportedCultures.Except(new[] { skipCulture }).ToArray();
        }

        public bool IsCultureSupported(CultureInfo cultureToCheck)
        {
            return AllSupportedCultures.Contains(cultureToCheck);
        }
    }
}
