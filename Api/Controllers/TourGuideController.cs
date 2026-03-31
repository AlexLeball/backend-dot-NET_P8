using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Models;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.LibrairiesWrappers.Interfaces;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private const double MilesToKilometers = 1.609344;

    private readonly ITourGuideService _tourGuideService;
    private readonly IRewardCentral _rewardCentral;

    public TourGuideController(ITourGuideService tourGuideService, IRewardCentral rewardCentral)
    {
        _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
        _rewardCentral = rewardCentral ?? throw new ArgumentNullException(nameof(rewardCentral));
    }

    [HttpGet("getLocation")]
    public async Task<ActionResult<VisitedLocation>> GetLocationAsync([FromQuery] string userName)
    {
        var location = await _tourGuideService.GetUserLocationAsync(GetUser(userName));
        return Ok(location);
    }

    /// <summary>
    /// Returns the 5 closest attractions to the user with distance (km) and reward points.
    /// </summary>
    [HttpGet("getNearbyAttractions")]
    public async Task<ActionResult<List<NearbyAttractionDto>>> GetNearbyAttractionsAsync([FromQuery] string userName)
    {
        var user = GetUser(userName);
        var visitedLocation = await _tourGuideService.GetUserLocationAsync(user);
        var nearbyAttractions = await _tourGuideService.GetNearByAttractionsAsync(visitedLocation);

        var result = new List<NearbyAttractionDto>();
        foreach (var attraction in nearbyAttractions)
        {
            var distanceMiles = GeoUtils.CalculateDistanceMiles(attraction, visitedLocation.Location);
            var rewardPoints = await _rewardCentral.GetAttractionRewardPointsAsync(attraction.AttractionId, user.UserId);

            result.Add(new NearbyAttractionDto
            {
                AttractionName = attraction.AttractionName,
                AttractionLatitude = attraction.Latitude,
                AttractionLongitude = attraction.Longitude,
                UserLatitude = visitedLocation.Location.Latitude,
                UserLongitude = visitedLocation.Location.Longitude,
                DistanceInKilometers = Math.Round(distanceMiles * MilesToKilometers, 2),
                RewardPoints = rewardPoints
            });
        }

        return Ok(result);
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
