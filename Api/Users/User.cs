using GpsUtil.Location;
using TripPricer;

namespace TourGuide.Users;

public class User
{
    private readonly object _rewardLock = new();
    private readonly HashSet<Guid> _rewardedAttractions = new();

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
        UserName = userName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public void AddToVisitedLocations(VisitedLocation visitedLocation)
    {
        VisitedLocations.Add(visitedLocation);
    }

    public void ClearVisitedLocations()
    {
        VisitedLocations.Clear();
    }

    public void AddUserReward(UserReward userReward)
    {
        lock (_rewardLock)
        {
            // utiliser AttractionId pour la déduplication (consistant avec RewardsService)
            if (_rewardedAttractions.Add(userReward.Attraction.AttractionId))
            {
                UserRewards.Add(userReward);
            }
        }
    }

    public VisitedLocation GetLastVisitedLocation()
    {
        return VisitedLocations[^1];
    }
}
