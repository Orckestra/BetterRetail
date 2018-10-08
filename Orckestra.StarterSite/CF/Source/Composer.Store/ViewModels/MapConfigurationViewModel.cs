using Orckestra.Composer.Store.Models;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class MapConfigurationViewModel: BaseViewModel
    {
        public int ZoomLevel { get; set; }
        public Bounds Bounds { get; set; }
        public Coordinate Center { get; set; }
        public int MarkerPadding { get; set; }
    }
}