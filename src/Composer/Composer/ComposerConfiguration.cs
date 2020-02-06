using Orckestra.Composer.Enums;
using System.Collections.Generic;
using System.Web;

namespace Orckestra.Composer
{
    public static class ComposerConfiguration
    {
        static ComposerConfiguration()
        {

            ValidateCsrfTokenForWebApi = false;

            LocalizationCacheOptions = new OutputCacheOptions
            {
                Duration = 10800,
                HttpCacheability = HttpCacheability.Public,
                SetValidUntilExpires = true,
                VaryByHeaders = "Accept-Language"
            };
        }

        /// <summary>
        /// The LocalizationCache parameters for the localization
        /// </summary>
        public static OutputCacheOptions LocalizationCacheOptions { get; set; }

        /// <summary>
        /// The list on InventoryStatus considered available to sell. (Used to display AddToCart button)
        /// </summary>
        public static List<InventoryStatusEnum> AvailableStatusForSell = new List<InventoryStatusEnum>
        {
            InventoryStatusEnum.InStock
        };

        public static bool ValidateCsrfTokenForWebApi { get; set; }

        public static class HandlebarsViewEngineConfiguration
        {
            private static IList<string> _viewLocationFormats = new List<string>
            {
                "~/UI.Package/Templates/{1}/{0}.hbs",
                "~/UI.Package/Templates/{0}.hbs",
                "~/Views/{1}/{0}.hbs",
                "~/Views/Shared/{0}.hbs",
            };

            /// <summary>
            /// {0} = Action name
            /// {1} = Controller name
            /// </summary>
            public static IList<string> ViewLocationFormats
            {
                get { return _viewLocationFormats ?? new List<string>(); }
                set { _viewLocationFormats = value; }
            }

            /// <summary>
            /// Directory relative to the AppDomainAppPath where to find the .hbs files
            /// </summary>
            public static string TamplateHbsDirectory { get; set; } = "~/UI.Package/Templates";
        }

        public static class ResxLocalizationRepositoryConfiguration
        {
            private static IList<string> _patternsForPossibleSources = new List<string>
            {
                "{category}_Custom.{cultureName}.resx",
                "{category}_Custom.{twoLetterISOLanguageName}.resx",
                "{category}_Custom.resx",
                "{category}.{cultureName}.resx",
                "{category}.{twoLetterISOLanguageName}.resx",
                "{category}.resx",
            };

            /// <summary>
            /// Directory relative to the AppDomainAppPath where to find the resx files
            /// used by the ResxLocalizationProvider
            /// </summary>
            public static string LocalizationResxDirectory { get; set; } = @"~/UI.Package/LocalizedStrings";

            /// <summary>
            /// Sorted list of filename patterns that could possibly hold
            /// a value for the given Category and Culture.
            /// 
            /// This is a list of fallback
            /// Most significant first
            /// 
            /// Possible replacement tokens are 
            ///   {cultureName}              the parms.CultureInfo.Name
            ///   {twoLetterISOLanguageName} the parms.CultureInfo.TwoLetterISOLanguageName
            ///   {category}                 the parms.Category
            /// </summary>
            public static IList<string> PatternsForPossibleSources
            {
                get { return _patternsForPossibleSources ?? new List<string>(); }
                set { _patternsForPossibleSources = value; }
            }

            /// <summary>
            /// Regex used to extract the category part of a filename
            /// </summary>
            public static string RegexSource { get; set; } = "^(?<category>[^_\\.]+)(?<customTag>_Custom){0,1}(?<culture>\\.[^\\.]+){0,1}\\.resx$";
        }
    }
}
