using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location;
//<summary>
// Represents a location visited by a user, including the user ID, location details, and the time of the visit.
// used to track user movements and locations visited.
//</summary>
public class VisitedLocation
{
    public Guid UserId { get; }
    public Locations Location { get; }
    public DateTime TimeVisited { get; }

    public VisitedLocation(Guid userId, Locations location, DateTime timeVisited)
    {
        UserId = userId;
        Location = location;
        TimeVisited = timeVisited;
    }

    // If timeVisited is not provided, default to current UTC time
    public VisitedLocation(Guid userId, Locations location) : this(userId, location, DateTime.UtcNow)
    {
    }
}
