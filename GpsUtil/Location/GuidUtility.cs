using System.Security.Cryptography;
using System.Text;

namespace GpsUtil.Location
{
    // Utility class to create a GUID from a string input using MD5 hashing
    public static class GuidUtility
    {
        public static Guid Create(string input)
        {
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
