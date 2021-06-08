using System;
using System.Threading;

namespace Distance.Service
{
    public class DistanceService : IDistanceService
    {
        private readonly static double ε = Math.Pow(2, -52);
        private const double MilesToMetersFactor = 1609.344;

        public DistanceService()
        {
        }

        /// <summary>
        /// Implementation of Vincenty formula for calculate distance between two points.
        /// </summary>
        /// <param name="latitudeFrom">Latitude of depature point. Must be from -90 to 90 degrees.</param>
        /// <param name="longitudeFrom">Longitude of depature point. Must be from -180 to 180 degrees.</param>
        /// <param name="latitudeTo">Latitude of destination point. Must be from -90 to 90 degrees.</param>
        /// <param name="longitudeTo">Longitude of destination point. Must be from -180 to 180 degrees.</param>
        /// <returns>Calculated distance in miles.</returns>
        public double CalulateDistance(double latitudeFrom, double longitudeFrom, double latitudeTo, double longitudeTo,
            CancellationToken cancellationToken)
        {
            double φ1 = DegreesToRadians(latitudeFrom);
            double λ1 = DegreesToRadians(longitudeFrom);
            double φ2 = DegreesToRadians(latitudeTo);
            double λ2 = DegreesToRadians(longitudeTo);

            double a = 6378137; //major semi-axes of the ellipsoid, meters, WGS84
            double b = 6356752.314245; //minor semi-axes of the ellipsoid, meters, WGS84
            double f = 1 / 298.257223563; //flattening, WGS84 

            double L = λ2 - λ1; // L = difference in longitude, U = reduced latitude, defined by tan U = (1-f)·tanφ.
            double tanU1 = (1 - f) * Math.Tan(φ1);
            double cosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1));
            double sinU1 = tanU1 * cosU1;
            double tanU2 = (1 - f) * Math.Tan(φ2);
            double cosU2 = 1 / Math.Sqrt((1 + tanU2 * tanU2));
            double sinU2 = tanU2 * cosU2;

            var antipodal = Math.Abs(L) > Math.PI / 2 || Math.Abs(φ2 - φ1) > Math.PI / 2;

            double λ = L; // λ = difference in longitude on an auxiliary sphere

            double σ = antipodal ? Math.PI : 0; // σ = angular distance P₁ P₂ on the sphere
            double sinσ = 0;
            double cosσ = antipodal ? -1 : 1;

            // σₘ = angular distance on the sphere from the equator to the midpoint of the line
            double cos2σₘ = 1;
            
            // α = azimuth of the geodesic at the equator
            double cosSqα = 1;

            double λʹ = 0;
            var iterations = 0;
            do
            {
                var sinλ = Math.Sin(λ);
                var cosλ = Math.Cos(λ);
                var sinSqσ = (cosU2 * sinλ) * (cosU2 * sinλ) + (cosU1 * sinU2 - sinU1 * cosU2 * cosλ) * (cosU1 * sinU2 - sinU1 * cosU2 * cosλ);

                if (Math.Abs(sinSqσ) < ε) break;  // co-incident/antipodal points (falls back on λ/σ = L)

                sinσ = Math.Sqrt(sinSqσ);
                cosσ = sinU1 * sinU2 + cosU1 * cosU2 * cosλ;
                σ = Math.Atan2(sinσ, cosσ);
                var sinα = cosU1 * cosU2 * sinλ / sinσ;
                cosSqα = 1.0 - sinα * sinα;
                cos2σₘ = (cosSqα != 0) ? (cosσ - 2.0 * sinU1 * sinU2 / cosSqα) : 0; // on equatorial line cos²α = 0
                double C = f / 16.0 * cosSqα * (4.0 + f * (4.0 - 3.0 * cosSqα));
                λʹ = λ;
                λ = L + (1.0 - C) * f * sinα * (σ + C * sinσ * (cos2σₘ + C * cosσ * (-1.0 + 2.0 * cos2σₘ * cos2σₘ)));

                double iterationCheck = antipodal ? Math.Abs(λ) - Math.PI : Math.Abs(λ);
                if (iterationCheck > Math.PI) throw new Exception("λ > π");
            } 
            while (Math.Abs(λ - λʹ) > 1e-12 && ++iterations < 1000 && !cancellationToken.IsCancellationRequested); //e.g. 10-12 ≈ 0.006mm

            if (iterations >= 1000) throw new Exception("Vincenty formula failed to converge");

            var uSq = cosSqα * (a * a - b * b) / (b * b);
            var A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            var B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            var Δσ = B * sinσ * (cos2σₘ + B / 4 * (cosσ * (-1 + 2 * cos2σₘ * cos2σₘ) -
                B / 6 * cos2σₘ * (-3 + 4 * sinσ * sinσ) * (-3 + 4 * cos2σₘ * cos2σₘ)));

            var s = b * A * (σ - Δσ); // s = length of the geodesic

            return s / MilesToMetersFactor; //To miles
        }

        private double DegreesToRadians(double degrees)
        {
            return (degrees * Math.PI / 180.0);
        }
    }
}
