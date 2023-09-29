using DG.Common.Exceptions;
using System;

namespace DG.Common
{
    /// <summary>
    /// <para>Provides functionality to convert byte arrays to an url safe version of base-64 encoded string.</para>
    /// <para>In the encoded version, '+' is replaced with '-', '/' is replaced with '_', and trailing '=' characters are trimmed.</para>
    /// </summary>
    public static class SafeBase64
    {
        /// <summary>
        /// Converts an array of <see cref="byte"/>s to its equivalent <see cref="string"/> representation that is encoded with url safe base-64 digits.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Encode(byte[] bytes)
        {
            ThrowIf.Parameter.IsNullOrEmpty(bytes, nameof(bytes));
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Converts the base-64 encoded <see cref="string"/> to an array of <see cref="byte"/>s.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static byte[] Decode(string base64)
        {
            ThrowIf.Parameter.IsNullOrEmpty(base64, nameof(base64));
            base64 = base64.Replace('_', '/').Replace('-', '+');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
