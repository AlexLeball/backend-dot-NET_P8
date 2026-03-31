using Microsoft.Extensions.Logging;
using TourGuide.LibrairiesWrappers;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services;
using TourGuide.Services.Interfaces;
using TourGuide.Utilities;

namespace TourGuideTest;

public class DependencyFixture
{
    public IGpsUtil GpsUtil { get; private set; } = null!;
    public IRewardCentral RewardCentral { get; private set; } = null!;
    public IRewardsService RewardsService { get; private set; } = null!;
    public ITourGuideService TourGuideService { get; private set; } = null!;

    public DependencyFixture()
    {
        Initialize();
    }

    public void Cleanup()
    {
        Initialize();
    }

    public void Initialize(int internalUserNumber = 100)
    {
        InternalTestHelper.SetInternalUserNumber(internalUserNumber);

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var tourGuideLogger = loggerFactory.CreateLogger<TourGuideService>();

        GpsUtil = new GpsUtilWrapper();
        RewardCentral = new RewardCentralWrapper();
        RewardsService = new RewardsService(GpsUtil, RewardCentral);
        TourGuideService = new TourGuideService(tourGuideLogger, GpsUtil, RewardsService, loggerFactory);
    }
}
