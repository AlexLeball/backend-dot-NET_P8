namespace TourGuide.Models;

/// <summary>
/// DTO representing a nearby attraction with distance and reward information.
/// </summary>
public class NearbyAttractionDto
{
    public string AttractionName { get; set; } = string.Empty;

    public double AttractionLatitude { get; set; }
    public double AttractionLongitude { get; set; }

    public double UserLatitude { get; set; }
    public double UserLongitude { get; set; }

    public double DistanceInKilometers { get; set; }

    public int RewardPoints { get; set; }
}

