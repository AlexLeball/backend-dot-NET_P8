using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
