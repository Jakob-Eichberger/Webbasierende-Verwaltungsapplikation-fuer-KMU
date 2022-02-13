using System;
using System.Text.RegularExpressions;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if a string has the correct Format to fit the RFC 2822 norm. This Function does not check if the top-level domain is correct.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Returns true if a string is a valid rfv 2822 format. Otherwhise it returns false.</returns>
        /// <exception cref="NullReferenceException">Throws <see cref="NullReferenceException"/> if <paramref name="e"/> is null.</exception>
        public static bool IsEmailValid(this string e) => Regex.IsMatch(string.IsNullOrWhiteSpace(e) ? "" : e, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        /// <summary>
        /// Checks if a string is a valid phone number.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Returns true if the string is a valid phone number.</returns>
        public static bool IsPhoneNumberValid(this string e) => Regex.IsMatch(string.IsNullOrWhiteSpace(e) ? "" : e, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
    }
}
