using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers;

public class GpsUtilWrapper : IGpsUtil
{
    private readonly GpsUtil.GpsUtil _gpsUtil;

    public GpsUtilWrapper()
    {
        _gpsUtil = new();
    }

    public VisitedLocation GetUserLocation(Guid userId)
    {
        // Appel synchrone à partir de la méthode asynchrone
        return _gpsUtil.GetUserLocationAsync(userId).GetAwaiter().GetResult();
    }

    public async Task<VisitedLocation> GetUserLocationAsync(Guid userId)
    {
        return await _gpsUtil.GetUserLocationAsync(userId).ConfigureAwait(false);
    }

    public List<Attraction> GetAttractions()
    {
        // Appel synchrone à partir de la méthode asynchrone
        return _gpsUtil.GetAttractionsAsync().GetAwaiter().GetResult();
    }
}
