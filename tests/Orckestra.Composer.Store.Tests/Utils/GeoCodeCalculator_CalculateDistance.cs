using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Store.Utils;

namespace Orckestra.Composer.Store.Tests.Utils
{
    [TestFixture]
    public class GeoCodeCalculator_CalculateDistance
    {

        [TestCase(50.394486, 30.440753, 50.394486, 30.440753, 0, 0)]
        [TestCase(50.394486, 30.440753, 50.391422, 30.460194, 1.42, 0.88)]
        [TestCase(45.368254, -73.688230, 50.394486, 30.440753, 7112.34, 4416.33)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        public void WHEN_coordinates_SHOULD_return_expected_distance(double lat1, double lng1, double lat2, double lng2, double expectedDistanceInKilometers, double expectedDistanceInMiles)
        {
            var distanceInMiles = Math.Round(GeoCodeCalculator.CalcDistance(lat1, lng1, lat2, lng2, EarthRadiusMeasurement.Miles), 2);
            var distanceInKilometers = Math.Round(GeoCodeCalculator.CalcDistance(lat1, lng1, lat2, lng2, EarthRadiusMeasurement.Kilometers), 2);
            distanceInKilometers.ShouldBeEquivalentTo(expectedDistanceInKilometers);
            distanceInMiles.ShouldBeEquivalentTo(expectedDistanceInMiles);
        }
    }
}