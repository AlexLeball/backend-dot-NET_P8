using System.Security.Cryptography;

namespace GpsUtil.Helpers;

// Thread-safe random number generator for use in multi-threaded environments
internal static class ThreadLocalRandom
{
    private static readonly ThreadLocal<Random> threadLocal = new ThreadLocal<Random>(() =>
    {
        // seed each thread's Random with a cryptographically secure random number
        int seed = RandomNumberGenerator.GetInt32(int.MaxValue);
        return new Random(seed);
    });

    public static Random Current => threadLocal.Value;

    public static double NextDouble(double minValue, double maxValue)
    {
        return Current.NextDouble() * (maxValue - minValue) + minValue;
    }

    public static int Next(int minValue, int maxValue)
    {
        return Current.Next(minValue, maxValue);
    }
}

