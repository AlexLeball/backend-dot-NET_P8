using GpsUtil.Location;
using TourGuide.Models;
using TripPricer;

namespace TourGuide.Users;

public class User
{
    private readonly object _visitedLocationsLock = new();
    private readonly object _rewardLock = new();
    private readonly HashSet<Guid> _rewardedAttractionIds = new();

    public Guid UserId { get; }
    public string UserName { get; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public DateTime LatestLocationTimestamp { get; set; }
    public List<VisitedLocation> VisitedLocations { get; } = new();
    public List<UserReward> UserRewards { get; } = new();
    public UserPreferences UserPreferences { get; set; } = new();
    public List<Provider> TripDeals { get; set; } = new();
    public List<EventTicket> EventTickets { get; set; } = new();

    public User(Guid userId, string userName, string phoneNumber, string emailAddress)
    {
        UserId = userId;
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    /// <summary>
    /// Thread-safe snapshot to prevent InvalidOperationException during concurrent enumeration.
    /// </summary>
    public IReadOnlyList<VisitedLocation> GetVisitedLocationsSnapshot()
    {
        lock (_visitedLocationsLock)
        {
            return VisitedLocations.ToList();
        }
    }

    public void AddToVisitedLocations(VisitedLocation visitedLocation)
    {
        lock (_visitedLocationsLock)
        {
            VisitedLocations.Add(visitedLocation);
            LatestLocationTimestamp = visitedLocation.TimeVisited;
        }
    }

    /// <summary>
    /// Thread-safe: prevents duplicate rewards per attraction.
    /// </summary>
    public void AddUserReward(UserReward userReward)
    {
        if (userReward is null) throw new ArgumentNullException(nameof(userReward));

        lock (_rewardLock)
        {
            if (_rewardedAttractionIds.Add(userReward.Attraction.AttractionId))
            {
                UserRewards.Add(userReward);
            }
        }
    }

    /// <summary>
    /// Returns the most recently visited location. Must lock to avoid
    /// InvalidOperationException if another thread adds concurrently.
    /// </summary>
    public VisitedLocation GetLastVisitedLocation()
    {
        lock (_visitedLocationsLock)
        {
            if (VisitedLocations.Count == 0)
                throw new InvalidOperationException("No visited locations available for this user.");

            return VisitedLocations.MaxBy(v => v.TimeVisited)!;
        }
    }
}
