using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
