namespace GpsUtil.Location;

public class Attraction : Locations
{
    public string AttractionName { get; }
    public string City { get; }
    public string State { get; }
    public Guid AttractionId { get; }

    // Constructor to initialize an Attraction object with its name, city, state, latitude, and longitude
    public Attraction(string attractionName, string city, string state, double latitude, double longitude)
        : base(latitude, longitude)
    {
        AttractionName = attractionName;
        City = city;
        State = state;
        // Generate a unique ID for the attraction based on its name and location
        AttractionId = GuidUtility.Create($"{attractionName}|{city}|{state}");
    }

}
