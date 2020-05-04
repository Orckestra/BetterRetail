using System.Text.RegularExpressions;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
// ReSharper disable once CheckNamespace

namespace System
{
    public static class StringExtensions
    {
        private static readonly Regex Base64Regex = new Regex("^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool IsBase64String(this string str)
        {
            if(string.IsNullOrWhiteSpace(str)) { return false; }

            var s = str.Trim();
            var isMatch = s.Length%4 == 0 && Base64Regex.IsMatch(s);

            return isMatch;
        }

        public static Guid ToGuid(this string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(guid)); }
            return Guid.Parse(guid);
        }
    }
}