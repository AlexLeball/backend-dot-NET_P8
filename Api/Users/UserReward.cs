using GpsUtil.Location;

namespace TourGuide.Users;

public class UserReward
{
    public VisitedLocation VisitedLocation { get; }
    public Attraction Attraction { get; }
    public int RewardPoints { get; set; }

    // Primary constructor
    public UserReward(VisitedLocation visitedLocation, Attraction attraction, int rewardPoints)
    {
        VisitedLocation = visitedLocation;
        Attraction = attraction;
        RewardPoints = rewardPoints;
    }
}
