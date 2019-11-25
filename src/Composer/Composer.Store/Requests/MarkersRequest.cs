using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Requests
{
    public class MarkersRequest
    {
        public int ZoomLevel { get; set; }
        public Bounds MapBounds { get; set; }
        public Coordinate SearchPoint { get; set; }
        public bool IsSearch { get; set; }
        public int PageSize { get; set; }
    }
}
