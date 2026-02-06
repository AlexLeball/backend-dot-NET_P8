using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPricer.Helpers;

namespace TripPricer;

public class TripPricer
{
/// <summary>
/// trip pricer class to get price from providers for a given attraction
/// </summary>
    public List<Provider> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints, bool testMode = false)
    {
        // List to hold providers
        List<Provider> providers = new List<Provider>();

        // Test mode with fixed prices
        if (testMode)
        {
            for (int i = 0; i < 10; i++)
            {
                providers.Add(new Provider(attractionId, $"Provider {i + 1}", 1000));
            }
            return providers;
        }

        // Names of providers to choose from
        string[] allNames = new string[]
        {
        "Holiday Travels",
        "Enterprize Ventures Limited",
        "Sunny Days",
        "FlyAway Trips",
        "United Partners Vacations",
        "Dream Trips",
        "Live Free",
        "Dancing Waves Cruselines and Partners",
        "AdventureCo",
        "Cure-Your-Blues"
        };

        // Shuffle names via thread-safe random (class in TripPricer.Helpers)
        var shuffled = allNames.OrderBy(_ => ThreadLocalRandom.Current.Next()).ToList();

        // Generate prices and create providers
        for (int i = 0; i < 10; i++)
        {
            // Generate a random multiple for price calculation
            int multiple = ThreadLocalRandom.Current.Next(100, 700);

            // Calculate price based on adults, children, nights stay, and rewards points
            double childrenDiscount = children / 3.0;
            double price = multiple * adults + multiple * childrenDiscount * nightsStay + 0.99 - rewardsPoints;
            if (price < 0.0) price = 0.0;

            // Create provider with unique name from shuffled list
            string provider = shuffled[i];

            // Add provider to the list
            providers.Add(new Provider(attractionId, provider, price));
        }

        // Return the list of providers
        return providers;
    }

}
