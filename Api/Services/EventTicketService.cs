using TourGuide.Models;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Services;

/// <summary>
/// Retrieves event ticket deals for a user by leveraging their cumulative reward points.
/// </summary>
public class EventTicketService : IEventTicketService
{
    private readonly EventTicketPricer _eventTicketPricer;
    private const string EventTicketApiKey = "test-event-api-key";

    public EventTicketService()
    {
        _eventTicketPricer = new EventTicketPricer();
    }

    public List<EventTicket> GetEventTicketDeals(User user)
    {
        int cumulativeRewardPoints = user.UserRewards.Sum(r => r.RewardPoints);

        var offers = _eventTicketPricer.GetEventTickets(
            EventTicketApiKey,
            user.UserId,
            user.UserPreferences.NumberOfAdults,
            user.UserPreferences.NumberOfChildren,
            cumulativeRewardPoints
        );

        var tickets = offers.Select(o => new EventTicket(
            o.TicketId,
            o.EventName,
            o.Venue,
            o.EventDate,
            o.OriginalPrice,
            o.DiscountedPrice,
            o.RewardPointsCost,
            o.ProviderName
        )).ToList();

        return tickets;
    }
}