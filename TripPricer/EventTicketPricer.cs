using TripPricer.Helpers;

namespace TripPricer;

/// <summary>
/// Generates event ticket offers with pricing based on user reward points and preferences.
/// </summary>
public class EventTicketPricer
{
    private static readonly string[] EventNames =
    {
        "City Walking Tour",
        "Sunset Cruise",
        "Wine Tasting Experience",
        "Jazz Night Live",
        "Cultural Heritage Festival",
        "Mountain Adventure Day",
        "Food Truck Festival",
        "Outdoor Movie Night",
        "Art Gallery Opening",
        "Local Brewery Tour"
    };

    private static readonly string[] Venues =
    {
        "Grand Plaza",
        "Riverside Park",
        "Historic Downtown Theater",
        "Oceanfront Pavilion",
        "Hilltop Convention Center",
        "Harbor Pier",
        "Central Square",
        "Botanical Gardens",
        "Lakeside Amphitheater",
        "Rooftop Terrace"
    };

    private static readonly string[] ProviderNames =
    {
        "EventBrite Local",
        "TicketMaster Experiences",
        "Viator Picks",
        "GetYourGuide Select",
        "Klook Adventures"
    };

    /// <summary>
    /// Generates a list of event ticket offers for a user based on their reward points and group size.
    /// </summary>
    public List<EventTicketOffer> GetEventTickets(string apiKey, Guid userId, int adults,
        int children, int rewardPoints)
    {
        var offers = new List<EventTicketOffer>();
        var random = ThreadLocalRandom.Current;

        int ticketCount = Math.Min(5, EventNames.Length);

        var shuffledEvents = EventNames.OrderBy(_ => random.Next()).Take(ticketCount).ToList();
        var shuffledVenues = Venues.OrderBy(_ => random.Next()).Take(ticketCount).ToList();

        for (int i = 0; i < ticketCount; i++)
        {
            int totalAttendees = adults + children;
            double basePrice = random.Next(20, 150) * totalAttendees;
            int pointsCost = random.Next(50, 500);

            // Apply discount based on available reward points
            double discount = Math.Min(rewardPoints * 0.05, basePrice * 0.75);
            double discountedPrice = Math.Max(basePrice - discount, 0.0);

            var eventDate = DateTime.UtcNow.AddDays(random.Next(7, 90));
            var providerName = ProviderNames[random.Next(ProviderNames.Length)];

            offers.Add(new EventTicketOffer(
                ticketId: Guid.NewGuid(),
                eventName: shuffledEvents[i],
                venue: shuffledVenues[i],
                eventDate: eventDate,
                originalPrice: Math.Round(basePrice, 2),
                discountedPrice: Math.Round(discountedPrice, 2),
                rewardPointsCost: pointsCost,
                providerName: providerName,
                attendees: totalAttendees
            ));
        }

        return offers;
    }
}