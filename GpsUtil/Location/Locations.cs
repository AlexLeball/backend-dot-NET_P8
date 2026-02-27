namespace GpsUtil.Location;

// Basic class to represent geographical locations with latitude and longitude
public class Locations
{
    public double Longitude { get; }
    public double Latitude { get; }

    public Locations(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
