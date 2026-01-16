using System.Diagnostics;
using TourGuide.Services;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Utilities;

/// <summary>
/// Tracker class tracks users' locations at regular intervals 
/// </summary>
public class Tracker
{
    private readonly ILogger<Tracker> _logger;
    private static readonly TimeSpan TrackingPollingInterval = TimeSpan.FromMinutes(5);
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly ITourGuideService _tourGuideService;

    // Constructor
    public Tracker(ITourGuideService tourGuideService, ILogger<Tracker> logger)
    {
        _tourGuideService = tourGuideService;
        _logger = logger;
        // Start the tracking process immediately upon instantiation
        Task.Run(() => Run(), _cancellationTokenSource.Token);
    }

    // Assures to shut down the Tracker thread
    public void StopTracking()
    {
        _cancellationTokenSource.Cancel();
    }

    // Main tracking loop
    public async Task Run()
    {
        // Stopwatch to measure tracking duration
        var stopwatch = new Stopwatch();

        // Token to monitor cancellation requests
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Retrieve all users to track (location)
            List<User> users = _tourGuideService.GetAllUsers();

            // Log the number of users being tracked
            _logger.LogDebug($"Begin Tracker. Tracking {users.Count} users.");

            // Start measuring time taken to track users
            stopwatch.Start();

            // Track each user's location
            users.ForEach(u => _tourGuideService.TrackUserLocation(u));

            // Stop measuring time
            stopwatch.Stop();

            // Log the time taken for tracking
            _logger.LogDebug($"Tracker Time Elapsed: {stopwatch.ElapsedMilliseconds / 1000.0} seconds.");

            // Reset stopwatch for the next iteration
            stopwatch.Reset();

            // Sleep for the defined polling interval or until cancellation is requested
            try
            {
                // Log sleeping state
                _logger.LogDebug("Tracker sleeping");
                await Task.Delay(TrackingPollingInterval, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Exit the loop if cancellation is requested
                break;
            }
        }

        _logger.LogDebug("Tracker stopping");
    }
}
