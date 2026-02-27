using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers
{
    public class RewardCentralWrapper : IRewardCentral
    {
        private readonly RewardCentral.RewardCentral _rewardCentral;

        public RewardCentralWrapper()
        {
            _rewardCentral = new ();
        }

        public int GetAttractionRewardPoints(Guid attractionId, Guid userId)
        {
            // Appel de la méthode asynchrone et attente du résultat de façon synchrone
            return _rewardCentral.GetAttractionRewardPointsAsync(attractionId, userId).GetAwaiter().GetResult();
        }
    }
}
