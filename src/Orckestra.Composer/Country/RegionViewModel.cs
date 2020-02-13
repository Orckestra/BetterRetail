using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Country
{
    public sealed class RegionViewModel : BaseViewModel
    {
        /// <summary>
        /// Two letter code that identifies Region uniquely
        /// </summary>
        public string IsoCode { get; set; }

        /// <summary>
        /// The official political name given for the Region
        /// </summary>
        public string Name { get; set; }
    }
}
    