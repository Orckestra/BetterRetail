namespace Orckestra.Composer.Store.Models
{
    public class Bounds
    {
        public Bounds()
        {
            SouthWest = new Coordinate();
            NorthEast = new Coordinate();
        }

        public Bounds(Coordinate sw, Coordinate ne)
        {
            this._initialized = true;
            SouthWest = new Coordinate(sw);
            NorthEast = new Coordinate(ne);
        }

        public Coordinate NorthEast { get; set; }
        public Coordinate SouthWest { get; set; }

        public Coordinate GetCenter()
        {
            var lngSpan = SouthWest.Lng - NorthEast.Lng;
            var latSpan = NorthEast.Lat - SouthWest.Lat;
            return new Coordinate(NorthEast.Lat - latSpan / 2d, SouthWest.Lng - lngSpan / 2d);
        }

        public bool Contains(Coordinate point)
        {
            return Contains(point.Lat, point.Lng);
        }

        public bool Contains(double lat, double lng)
        {
            if (NorthEast.Lng > SouthWest.Lng)
            {
                if (lng < SouthWest.Lng || lng > NorthEast.Lng) return false;
            }
            else if (lng < SouthWest.Lng && lng > NorthEast.Lng) // from 149 to -3
            {
                return false;
            }

            return lat >= SouthWest.Lat && lat <= NorthEast.Lat;
        }

        public override string ToString()
        {
            return string.Format("SW:{0},{1} - NE:{2},{3}", SouthWest.Lat, SouthWest.Lng, NorthEast.Lat, NorthEast.Lng);
        }

        public void Extend(double latitude, double longitude)
        {
            if (_initialized)
            {
                if (latitude > NorthEast.Lat)
                    NorthEast.Lat = latitude;
                if (latitude < SouthWest.Lat)
                    SouthWest.Lat = latitude;

                if (longitude > NorthEast.Lng)
                    NorthEast.Lng = longitude;
                if (longitude < SouthWest.Lng)
                    SouthWest.Lng = longitude;
            }
            else
            {
                NorthEast.Lat = latitude;
                SouthWest.Lat = latitude;
                NorthEast.Lng = longitude;
                SouthWest.Lng = longitude;
                _initialized = true;
            }
        }

        public void Extend(Coordinate c)
        {
            Extend(c.Lat, c.Lng);
        }

        public void Extend(Bounds bounds)
        {
            Extend(bounds.NorthEast);
            Extend(bounds.SouthWest);
        }

        private bool _initialized = false;
    }
}