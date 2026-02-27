using System;
using System.Collections.Generic;
using GpsUtil.Location;
using TourGuide.Models;
using TripPricer;

namespace TourGuide.Users;

public class User
{
    private readonly object _rewardLock = new();
    private readonly HashSet<Guid> _rewardedAttractionIds = new();
    private readonly object _visitedLocationsLock = new();
    public Guid UserId { get; }
    public string UserName { get; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public DateTime LatestLocationTimestamp { get; set; }
    public List<VisitedLocation> VisitedLocations { get; } = new List<VisitedLocation>();
    public List<UserReward> UserRewards { get; } = new List<UserReward>();
    public UserPreferences UserPreferences { get; set; } = new UserPreferences();
    public List<Provider> TripDeals { get; set; } = new List<Provider>();

    public User(Guid userId, string userName, string phoneNumber, string emailAddress)
    {
        UserId = userId;
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public IReadOnlyList<VisitedLocation> GetVisitedLocationsSnapshot()
    {
        lock (_visitedLocationsLock)
        {
            return VisitedLocations.ToList();
        }
    }

    // Add a visited location and update the latest location timestamp
    public void AddToVisitedLocations(VisitedLocation visitedLocation)
    {
        lock (_visitedLocationsLock)
        {
            VisitedLocations.Add(visitedLocation);
        }
    }

    // Clear all visited locations
    public void ClearVisitedLocations()
    {
        VisitedLocations.Clear();
    }

    // Add a user reward if it doesn't already exist
    public void AddUserReward(UserReward userReward)
    {
        if (userReward is null) throw new ArgumentNullException(nameof(userReward));

        lock (_rewardLock)
        {
            // Check if the attraction has already been rewarded to avoid duplicates
            if (_rewardedAttractionIds.Add(userReward.Attraction.AttractionId))
            {
                UserRewards.Add(userReward);
            }
        }
    }

    // Get the most recent visited location by date visited (changed)
    public VisitedLocation GetLastVisitedLocation()
    {
        // snapshot to avoid threading issues
        var snapshot = VisitedLocations.ToList();

        if (snapshot.Count == 0)
            throw new InvalidOperationException("No visited locations available for this user.");

        // Return the visited location with the maximum TimeVisited value
        return snapshot.MaxBy(v => v.TimeVisited)!;
    }
}
