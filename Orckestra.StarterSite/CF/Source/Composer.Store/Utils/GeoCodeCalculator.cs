using System;

namespace Orckestra.Composer.Store.Utils
{
    public static class GeoCodeCalculator
    {
        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static double ToRadian(double val)
        {
            return val*(Math.PI/180);
        }

        public static double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }

        /// <summary>
        /// Calculate Distance in Miles
        /// </summary>
        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return CalcDistance(lat1, lng1, lat2, lng2, EarthRadiusMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, double earthRadius)
        {
            double radius = earthRadius;

            return radius*2*
                   Math.Asin(Math.Min(1,
                       Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2))/2.0), 2.0) +
                                  Math.Cos(ToRadian(lat1))*Math.Cos(ToRadian(lat2))*
                                  Math.Pow(Math.Sin((DiffRadian(lng1, lng2))/2.0), 2.0)))));
        }
    }

    public struct EarthRadiusMeasurement
    {
        public const double Miles = 3956.0;
        public const double Kilometers = 6371.0;
    }
}