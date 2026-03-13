using TourGuide.Models;
using TourGuide.Users;

namespace TourGuide.Services.Interfaces;

/// <summary>
/// Service for retrieving event ticket deals based on user rewards and preferences.
/// </summary>
public interface IEventTicketService
{
    List<EventTicket> GetEventTicketDeals(User user);
}