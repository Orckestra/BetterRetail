using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreLocatorViewModelParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string BaseUrl { get; set; }
        public Bounds MapBounds { get; set; }
        public int ZoomLevel { get; set; }
        public Coordinate SearchPoint { get; set; }
        public bool IncludeMarkers { get; set; } = false;
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        /// <summary>
        /// To return or not to return the coordinate of the nearest available
        /// store from all stores in the scope, not depending on map bounds param
        /// </summary>
        public bool IncludeNearestStoreCoordinate { get; set; }
        public GetStoreLocatorViewModelParam Clone()
        {
            var param = (GetStoreLocatorViewModelParam)MemberwiseClone();
            param.MapBounds = this.MapBounds?.Clone();
            param.SearchPoint = this.SearchPoint?.Clone();
            return param;
        }
    }
}
