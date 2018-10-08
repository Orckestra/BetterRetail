namespace Orckestra.Composer.Store.Models
{
    public class Coordinate
    {
        public Coordinate() { }


        public Coordinate(Coordinate c)
        {
            Lat = c.Lat;
            Lng = c.Lng;
        }

        public Coordinate(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public double Lat { get; set; }
        public double Lng { get; set; }

        public override string ToString()
        {
            return Lat + "," + Lng;
        }
    }
}