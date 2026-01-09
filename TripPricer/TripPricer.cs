using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPricer.Helpers;

namespace TripPricer;

public class TripPricer
{
    public List<Provider> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints, bool testMode = false)
    {
        List<Provider> providers = new List<Provider>();

        if (testMode)
        {
            for (int i = 0; i < 10; i++)
            {
                providers.Add(new Provider(attractionId, $"Provider {i + 1}", 1000));
            }
            return providers;
        }

        // Original names
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

        // Shuffle names
        var shuffled = allNames.OrderBy(_ => ThreadLocalRandom.Current.Next()).ToList();

        for (int i = 0; i < 10; i++)
        {
            int multiple = ThreadLocalRandom.Current.Next(100, 700);
            double childrenDiscount = children / 3.0;
            double price = multiple * adults + multiple * childrenDiscount * nightsStay + 0.99 - rewardsPoints;
            if (price < 0.0) price = 0.0;

            string provider = shuffled[i];  // pick unique provider
            providers.Add(new Provider(attractionId, provider, price));
        }

        return providers;
    }

}
