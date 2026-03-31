using RewardCentral.Helpers;
using System.Collections.Concurrent;

namespace RewardCentral;

public class RewardCentral
{
    private static readonly ConcurrentDictionary<(Guid, Guid), int> rewardCache = new();

    public async Task<int> GetAttractionRewardPointsAsync(Guid attractionId, Guid userId)
    {
        if (rewardCache.TryGetValue((attractionId, userId), out var cachedPoints))
        {
            return cachedPoints;
        }

        // Simulate delay with Task.Delay
        await Task.Delay(ThreadLocalRandom.Current.Next(1, 100));

        int rewardPoints = ThreadLocalRandom.Current.Next(1, 1000);
        rewardCache[(attractionId, userId)] = rewardPoints;

        return rewardPoints;
    }
}
