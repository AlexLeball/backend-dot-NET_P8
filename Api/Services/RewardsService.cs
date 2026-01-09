using GpsUtil.Location;
using System.Linq;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;

    //default buffer for proximity is 10 miles this can be changed via the setProximityBuffer method
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    // Set proximity buffer to desired value
    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    // Reset proximity buffer to default value
    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    // Calculate rewards for a user based on their visited locations and nearby attractions
    public void CalculateRewards(User user)
    {
        var attractions = _gpsUtil.GetAttractions();

        // Snapshot of current rewards to prevent modifying collection while iterating
        var rewardedAttractions = user.UserRewards
            .Select(r => r.Attraction.AttractionId)
            .ToHashSet();

        // Take a snapshot of visited locations to avoid modification issues
        var visitedLocationsSnapshot = user.VisitedLocations.ToList();

        // List to collect rewards to add after iteration
        var rewardsToAdd = new List<UserReward>();

        foreach (var visitedLocation in visitedLocationsSnapshot)
        {
            foreach (var attraction in attractions)
            {
                if (rewardedAttractions.Contains(attraction.AttractionId))
                    continue;

                if (NearAttraction(visitedLocation, attraction))
                {
                    rewardsToAdd.Add(new UserReward(
                        visitedLocation,
                        attraction,
                        GetRewardPoints(attraction, user)
                    ));

                    rewardedAttractions.Add(attraction.AttractionId); // prevent duplicates
                }
            }
        }

        // Add all rewards after iteration to avoid modifying collection while enumerating
        foreach (var reward in rewardsToAdd)
        {
            user.AddUserReward(reward);
        }
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    private int GetRewardPoints(Attraction attraction, User user)
    {
        return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
