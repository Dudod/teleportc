using System.Threading;

namespace Distance.Service
{
    public interface IDistanceService
    {
        double CalulateDistance(double latitudeFrom, double longitudeFrom, double latitudeTo, double longitudeTo, CancellationToken cancellationToken);
    }
}
