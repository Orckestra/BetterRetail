using System;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Cart.Utils
{
    public static class SharedWishListTokenizer
    {
        private const string TokenSeparator = ":";

        /// <summary>
        /// Gets an encrypted token that will be used to identify an wish list.
        /// </summary>
        /// <param name="token">Parameters used to generate the token.</param>
        /// <returns></returns>
        public static string GenerateToken(SharedWishListToken token)
        {
            if (token == null) { throw new ArgumentNullException("token"); }
            if (token.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), "token"); }
            if (String.IsNullOrWhiteSpace(token.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "token"); }

            var encryptor = new EncryptionUtility();

            var rawToken = string.Join(TokenSeparator, token.CustomerId.ToString("N"), token.Scope);
            var encryptedToken = encryptor.Encrypt(rawToken);

            byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(encryptedToken);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// Gets the original token data.
        /// </summary>
        /// <param name="token">Encrypted token.</param>
        /// <returns></returns>
        public static SharedWishListToken DecryptToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("token"), "token"); }

            if (!token.IsBase64String()) { return null; }

            var tokenAsBytes = Convert.FromBase64String(token);
            var encryptedToken = System.Text.Encoding.ASCII.GetString(tokenAsBytes);

            var decryptor = new EncryptionUtility();

            var rawToken = decryptor.Decrypt(encryptedToken);
            var splittedTokens = rawToken.Split(new[] { TokenSeparator }, 2, StringSplitOptions.None);

            //Invalid token.
            if (splittedTokens.Length != 2)
            {
                return null;
            }

            Guid customerId;
            if (Guid.TryParse(splittedTokens[0], out customerId))
            {
                return new SharedWishListToken
                {
                    CustomerId = customerId,
                    Scope = splittedTokens[1]
                };
            }

            return null;
        }
    }
}
