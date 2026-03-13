using GpsUtil.Location;
using TripPricer;
using TourGuide.Users;

namespace TourGuide.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime LatestLocationTimestamp { get; set; }
        public List<VisitedLocation> VisitedLocations { get; set; } = new();
        public List<UserReward> UserRewards { get; set; } = new();
        public UserPreferences UserPreferences { get; set; } = new();
        public List<Provider> TripDeals { get; set; } = new();
    }

    public static class UserMappings
    {
        public static UserDto ToDto(this User user)
        {
            if (user == null) return null!;

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                EmailAddress = user.EmailAddress,
                LatestLocationTimestamp = user.LatestLocationTimestamp,
                VisitedLocations = user.GetVisitedLocationsSnapshot().ToList(),
                UserRewards = new List<UserReward>(user.UserRewards),
                UserPreferences = user.UserPreferences,
                TripDeals = new List<Provider>(user.TripDeals)
            };
        }
    }
}