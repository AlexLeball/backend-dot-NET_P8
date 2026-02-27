using GpsUtil.Location;
using System.Globalization;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services;

public class TourGuideService : ITourGuideService
{
    private readonly ILogger _logger;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardsService _rewardsService;
    private readonly TripPricer.TripPricer _tripPricer;

    // Internal user map for storing users by username
    private readonly Dictionary<string, User> _internalUserMap = new();
    private const string TripPricerApiKey = "test-server-api-key";

    // Test mode flag
    private readonly bool _testMode = true;

    // Public Tracker property to access the Tracker instance
    public Tracker Tracker { get; }

    // Bound for concurrent outbound GPS calls (bounded async handled here)
    private readonly SemaphoreSlim _gpsCallSemaphore = new SemaphoreSlim(100, 100);

    // Constructor 
    public TourGuideService(ILogger<TourGuideService> logger, IGpsUtil gpsUtil, IRewardsService rewardsService, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _tripPricer = new();
        _gpsUtil = gpsUtil;
        _rewardsService = rewardsService;

        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        // Initialize test mode users
        if (_testMode)
        {
            _logger.LogInformation("TestMode enabled");
            _logger.LogDebug("Initializing users");
            InitializeInternalUsers();
            _logger.LogDebug("Finished initializing users");
        }

        // Create logger for Tracker
        var trackerLogger = loggerFactory.CreateLogger<Tracker>();

        // Start the tracker
        Tracker = new Tracker(this, trackerLogger);

        // Termination hook to stop tracking on app shutdown 
        AddShutDownHook();
    }

    // method to retrieve user rewards
    public List<UserReward> GetUserRewards(User user)
    {
        return user.UserRewards;
    }

    // method to retrieve user location (synchronous façade)
    public VisitedLocation GetUserLocation(User user)
    {
        return user.VisitedLocations.Any()
            ? user.GetLastVisitedLocation()
            : GetUserLocationAsync(user).GetAwaiter().GetResult();
    }

    // New async GetUserLocation
    public Task<VisitedLocation> GetUserLocationAsync(User user)
    {
        return user.VisitedLocations.Any()
            ? Task.FromResult(user.GetLastVisitedLocation())
            : TrackUserLocationAsync(user);
    }

    // method to retrieve a user by username(change to retrieve by id?)
    public User GetUser(string userName)
    {
        // Retrieve user from internal user map or return null if not found
        return _internalUserMap.TryGetValue(userName, out var user) ? user : null!;
    }

    // method to retrieve all users
    public List<User> GetAllUsers()
    {
        // Return a list of all users in the internal user map
        return _internalUserMap.Values.ToList();
    }

    // method to add a new user
    // verify if this does not need more robust user criteria?
    public void AddUser(User user)
    {
        // Add the user to the internal user map if they do not already exist
        if (!_internalUserMap.ContainsKey(user.UserName))
        {
            _internalUserMap.Add(user.UserName, user);
        }
    }

    // method to get trip deals for a user based on their preferences and rewards.
    public List<Provider> GetTripDeals(User user)
    {
        // Calculate cumulative reward points from user's rewards
        int cumulativeRewardPoints = user.UserRewards.Sum(i => i.RewardPoints);

        // Retrieve trip deals from TripPricer API based on user details and reward points
        List<Provider> providers = _tripPricer.GetPrice(TripPricerApiKey, user.UserId,

            // User preferences from User object
            user.UserPreferences.NumberOfAdults, user.UserPreferences.NumberOfChildren,

            // User trip duration from User object
            user.UserPreferences.TripDuration, cumulativeRewardPoints);

        // Store the retrieved trip deals in the user's TripDeals property
        user.TripDeals = providers;

        // Return the list of trip deal providers
        return providers;
    }

    // Tracks the user's location, adds it to their visited locations, and calculates rewards (async, bounded)
    public async Task<VisitedLocation> TrackUserLocationAsync(User user)
    {
        await _gpsCallSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var visitedLocation = await _gpsUtil.GetUserLocationAsync(user.UserId).ConfigureAwait(false);
            user.AddToVisitedLocations(visitedLocation);
            await _rewardsService.CalculateRewardsAsync(user).ConfigureAwait(false);
            return visitedLocation;
        }
        finally
        {
            _gpsCallSemaphore.Release();
        }
    }

    // Synchronous TrackUserLocation to satisfy interface (facade)
    public VisitedLocation TrackUserLocation(User user)
    {
        return TrackUserLocationAsync(user).GetAwaiter().GetResult();
    }

    // Returns a list of the five nearest attractions to the given visited location
    public List<Attraction> GetNearByAttractions(VisitedLocation visitedLocation)
    {
        return _gpsUtil.GetAttractions()
            .OrderBy(a => _rewardsService.GetDistance(a, visitedLocation.Location))
            .Take(5)
            .ToList();
    }

    // Adds a shutdown hook to stop the tracker when the application exits
    private void AddShutDownHook()
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => Tracker.StopTracking();
    }

    /**********************************************************************************
    * 
    * Methods Below: For Internal Testing
    * 
    **********************************************************************************/

    private void InitializeInternalUsers()
    {
        for (int i = 0; i < InternalTestHelper.GetInternalUserNumber(); i++)
        {
            var userName = $"internalUser{i}";
            var user = new User(Guid.NewGuid(), userName, "000", $"{userName}@tourGuide.com");
            GenerateUserLocationHistory(user);
            _internalUserMap.Add(userName, user);
        }

        _logger.LogDebug($"Created {InternalTestHelper.GetInternalUserNumber()} internal test users.");
    }

    private void GenerateUserLocationHistory(User user)
    {
        for (int i = 0; i < 3; i++)
        {
            var visitedLocation = new VisitedLocation(user.UserId, new Locations(GenerateRandomLatitude(), GenerateRandomLongitude()), GetRandomTime());
            user.AddToVisitedLocations(visitedLocation);
        }
    }

    private static readonly Random random = new Random();

    private double GenerateRandomLongitude()
    {
        return new Random().NextDouble() * (180 - (-180)) + (-180);
    }

    private double GenerateRandomLatitude()
    {
        return new Random().NextDouble() * (90 - (-90)) + (-90);
    }

    private DateTime GetRandomTime()
    {
        return DateTime.UtcNow.AddDays(-new Random().Next(30));
    }
}
