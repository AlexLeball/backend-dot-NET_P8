using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Models;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;
using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;
    private readonly IRewardCentral _rewardCentral;

    public TourGuideController(ITourGuideService tourGuideService, IRewardCentral rewardCentral)
    {
        _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
        _rewardCentral = rewardCentral ?? throw new ArgumentNullException(nameof(rewardCentral));
    }

    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }

    // TODO: Change this method to no longer return a List of Attractions.
    // Instead: Get the closest five tourist attractions to the user - no matter how far away they are.
    // Return a new JSON object that contains:
    // Name of Tourist attraction, 
    // Tourist attractions lat/long, 
    // The user's location lat/long, 
    // The distance in miles between the user's location and each of the attractions.
    // The reward points for visiting each Attraction.
    // Note: Attraction reward points can be gathered from RewardsCentral
    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearbyAttractionDto>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = GetUser(userName);
        var visitedLocation = _tourGuideService.GetUserLocation(user);

        var nearbyAttractions = _tourGuideService
            .GetNearByAttractions(visitedLocation)
            .Select(attraction =>
            {
                var distance = GeoUtils.CalculateDistanceMiles(attraction, visitedLocation.Location);

                var rewardPoints = _rewardCentral.GetAttractionRewardPoints(
                    attraction.AttractionId,
                    user.UserId);

                return new NearbyAttractionDto
                {
                    AttractionName = attraction.AttractionName,

                    AttractionLatitude = attraction.Latitude,
                    AttractionLongitude = attraction.Longitude,

                    UserLatitude = visitedLocation.Location.Latitude,
                    UserLongitude = visitedLocation.Location.Longitude,

                    DistanceInMiles = distance,
                    RewardPoints = rewardPoints
                };
            })
            .ToList();

        return Ok(nearbyAttractions);
    }


    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
