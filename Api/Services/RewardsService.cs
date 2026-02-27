using GpsUtil.Location;
using System.Linq;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using System.Threading.Tasks;

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

    // Constructor to initialize GPS utility and Rewards Central dependencies
    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    // Proximity buffer setter to allow for changes. Checks that user has visited location within the given buffer
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
        // Get all attractions from GPS utility
        var attractions = _gpsUtil.GetAttractions();

        // Existing rewards linked to user (no modifying during iteration)
        var rewardedAttractions = user.UserRewards
            .Select(r => r.Attraction.AttractionId)
            .ToHashSet();

        // Utilisez la méthode sécurisée pour obtenir un snapshot des locations visitées
        var visitedLocationsSnapshot = user.GetVisitedLocationsSnapshot();

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

                    // Mark this attraction as rewarded to prevent duplicates
                    rewardedAttractions.Add(attraction.AttractionId);
                }
            }
        }

        // Add all reward points to the user after processing to avoid modifying the collection during iteration
        foreach (var reward in rewardsToAdd)
        {
            user.AddUserReward(reward);
        }
    }

    // Async wrapper for CalculateRewards to keep API async-friendly
    public async Task CalculateRewardsAsync(User user)
    {
        await Task.Run(() => CalculateRewards(user)).ConfigureAwait(false);
    }

    // Check if a location is within the attraction proximity range
    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    // Check if a visited location is near an attraction based on the proximity buffer
    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    // Get reward points for a given attraction and user from Rewards Central
    public int GetRewardPoints(Attraction attraction, User user)
    {
        return _rewardsCentral.GetAttractionRewardPoints(
            attraction.AttractionId,
            user.UserId
        );
    }

    // Calculate distance between an attraction and a location in miles via the GeoUtils class
    public double GetDistance(Locations loc1, Locations loc2)
    {
        return GeoUtils.CalculateDistanceMiles(loc1, loc2);
    }
}
