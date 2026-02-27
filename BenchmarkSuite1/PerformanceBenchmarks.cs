using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using TourGuide.Services;
using TourGuide.Services.Interfaces;
using TourGuide.LibrairiesWrappers;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Utilities;
using TourGuide.Users;
using GpsUtil.Location;
using Microsoft.VSDiagnostics;

namespace TourGuide.Benchmarks
{
    [CPUUsageDiagnoser]
    public class PerformanceBenchmarks
    {
        private TourGuideService _tourGuideService;
        private RewardsService _rewardsService;
        private IGpsUtil _gpsUtil;
        private IRewardCentral _rewardCentral;
        private ILoggerFactory _loggerFactory;
        // Number of internal users to initialize for the benchmark. Adjust as needed (100_000 for target scenario).
        private const int InternalUserCount = 100000;
        [GlobalSetup]
        public void GlobalSetup()
        {
            // Ensure the internal test helper will create the requested number of users
            InternalTestHelper.SetInternalUserNumber(InternalUserCount);
            // Logger factory for services
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            // Instantiate wrappers and services
            _gpsUtil = new GpsUtilWrapper();
            _rewardCentral = new RewardCentralWrapper();
            _rewardsService = new RewardsService(_gpsUtil, _rewardCentral);
            // Create the TourGuideService which will initialize internal users in test mode
            var tourGuideLogger = _loggerFactory.CreateLogger<TourGuideService>();
            _tourGuideService = new TourGuideService(tourGuideLogger, _gpsUtil, _rewardsService, _loggerFactory);
        // At this point the internal users should be created by the service constructor
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            // Stop background tracker to avoid leaving tasks running after the benchmark
            _tourGuideService.Tracker.StopTracking();
        }

        [Benchmark(Description = "HighVolumeTrackLocation - Sequential")]
        public void HighVolumeTrackLocation()
        {
            List<User> allUsers = _tourGuideService.GetAllUsers();
            foreach (var user in allUsers)
            {
                _tourGuideService.TrackUserLocation(user);
            }
        }

        [Benchmark(Description = "HighVolumeGetRewards - Sequential")]
        public void HighVolumeGetRewards()
        {
            // Prepare: get an attraction and add a visited location to each user
            Attraction attraction = _gpsUtil.GetAttractions()[0];
            List<User> allUsers = _tourGuideService.GetAllUsers();
            foreach (var u in allUsers)
            {
                u.AddToVisitedLocations(new VisitedLocation(u.UserId, attraction, DateTime.UtcNow));
            }

            // Calculate rewards sequentially
            foreach (var u in allUsers)
            {
                _rewardsService.CalculateRewards(u);
            }
        }
    }
}