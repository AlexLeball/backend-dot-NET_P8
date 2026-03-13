using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{

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

    public async Task CalculateRewardsAsync(User user)
    {
        var attractions = await _gpsUtil.GetAttractionsAsync().ConfigureAwait(false);

        var rewardedAttractions = user.UserRewards
            .Select(r => r.Attraction.AttractionId)
            .ToHashSet();

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
                    var points = await GetRewardPointsAsync(attraction, user).ConfigureAwait(false);
                    rewardsToAdd.Add(new UserReward(visitedLocation, attraction, points));
                    rewardedAttractions.Add(attraction.AttractionId);
                }
            }
        }

        foreach (var reward in rewardsToAdd)
        {
            user.AddUserReward(reward);
        }
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    // Check if a visited location is near an attraction based on the proximity buffer
    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    public async Task<int> GetRewardPointsAsync(Attraction attraction, User user)
    {
        return await _rewardsCentral.GetAttractionRewardPointsAsync(
            attraction.AttractionId,
            user.UserId
        ).ConfigureAwait(false);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        return GeoUtils.CalculateDistanceMiles(loc1, loc2);
    }
}
