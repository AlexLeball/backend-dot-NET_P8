namespace GpsUtil.Location;

public static class GeoUtils
{
    private const double EarthRadiusMiles = 3958.8;

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

    // Haversine formula to calculate the distance between two locations in miles then applied to location and attractions visited by users
    public static double CalculateDistanceMiles(Locations a, Locations b)
    {
        var dLat = ToRadians(b.Latitude - a.Latitude);
        var dLon = ToRadians(b.Longitude - a.Longitude);

        var lat1 = ToRadians(a.Latitude);
        var lat2 = ToRadians(b.Latitude);

        var sinDLat = Math.Sin(dLat / 2);
        var sinDLon = Math.Sin(dLon / 2);

        var hav = sinDLat * sinDLat + Math.Cos(lat1) * Math.Cos(lat2) * sinDLon * sinDLon;
        var c = 2 * Math.Atan2(Math.Sqrt(hav), Math.Sqrt(1 - hav));

        return EarthRadiusMiles * c;
    }
}