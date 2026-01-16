using System;
using System.Collections.Generic;
using GpsUtil.Location;
using TripPricer;
using TourGuide.Users;

namespace TourGuide.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime LatestLocationTimestamp { get; set; }
        public List<VisitedLocation> VisitedLocations { get; set; } = new List<VisitedLocation>();
        public List<UserReward> UserRewards { get; set; } = new List<UserReward>();
        public UserPreferences UserPreferences { get; set; } = new UserPreferences();
        public List<Provider> TripDeals { get; set; } = new List<Provider>();
    }

    public static class UserMappings
    {
        public static UserDto ToDto(this TourGuide.Users.User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                EmailAddress = user.EmailAddress,
                LatestLocationTimestamp = user.LatestLocationTimestamp,
                VisitedLocations = new List<VisitedLocation>(user.VisitedLocations),
                UserRewards = new List<UserReward>(user.UserRewards),
                UserPreferences = user.UserPreferences,
                TripDeals = new List<Provider>(user.TripDeals)
            };
        }
    }
}