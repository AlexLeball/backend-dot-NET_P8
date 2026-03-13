namespace TripPricer;

/// <summary>
/// Represents an event ticket offer from a provider.
/// </summary>
public class EventTicketOffer
{
    public Guid TicketId { get; }
    public string EventName { get; }
    public string Venue { get; }
    public DateTime EventDate { get; }
    public double OriginalPrice { get; }
    public double DiscountedPrice { get; }
    public int RewardPointsCost { get; }
    public string ProviderName { get; }
    public int Attendees { get; }

    public EventTicketOffer(Guid ticketId, string eventName, string venue, DateTime eventDate,
        double originalPrice, double discountedPrice, int rewardPointsCost,
        string providerName, int attendees)
    {
        TicketId = ticketId;
        EventName = eventName;
        Venue = venue;
        EventDate = eventDate;
        OriginalPrice = originalPrice;
        DiscountedPrice = discountedPrice;
        RewardPointsCost = rewardPointsCost;
        ProviderName = providerName;
        Attendees = attendees;
    }
}