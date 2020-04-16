using System;
using HashidsNet;

namespace URLShortenerAPI
{
    public static class HashIDGenerator
    {
        private static readonly string SALT_VALUE = "SF_URLSHORTENERAPI";
        // Exclude lowercase o, l and uppercase I.
        private static readonly string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private static readonly int MIN_HASH_LENGTH = 5;
        
        private static Hashids hashIds;

        public static void Initialize()
        {
            // Limit hash to 5 characters.
            hashIds = new Hashids(SALT_VALUE, MIN_HASH_LENGTH, ALPHABET);
        }
        public static string EncodeSeedValue(int seedValue)
        {
            return hashIds.Encode(seedValue);
        }

        public static int DecodeSeedValue(string hash)
        {
            int[] decodedValueDigits = hashIds.Decode(hash);
            return JoinDigitArray(decodedValueDigits);
        }

        private static int JoinDigitArray(int[] digits)
        {
            int result = 0;
            for (int i = 0; i < digits.Length; i++)
            {
                result += digits[i] * Convert.ToInt32(Math.Pow(10, digits.Length - i - 1));
            }
            return result;
        }
    }
}
