namespace TourGuide.Models;

/// <summary>
/// Represents an event ticket reward that a user can redeem using reward points.
/// </summary>
public class EventTicket
{
    public Guid TicketId { get; }
    public string EventName { get; }
    public string Venue { get; }
    public DateTime EventDate { get; }
    public double OriginalPrice { get; }
    public double DiscountedPrice { get; }
    public int RewardPointsCost { get; }
    public string ProviderName { get; }

    public EventTicket(Guid ticketId, string eventName, string venue, DateTime eventDate,
        double originalPrice, double discountedPrice, int rewardPointsCost, string providerName)
    {
        TicketId = ticketId;
        EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
        Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        EventDate = eventDate;
        OriginalPrice = originalPrice;
        DiscountedPrice = discountedPrice;
        RewardPointsCost = rewardPointsCost;
        ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
    }
}