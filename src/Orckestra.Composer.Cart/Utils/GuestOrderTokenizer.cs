using System;
using Orckestra.Composer.Utils;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Utils
{
    public static class GuestOrderTokenizer
    {
        private const string TokenSeparator = "|";

        /// <summary>
        /// Gets an encrypted token that will be used to identify an order.
        /// </summary>
        /// <param name="token">Parameters used to generate the token.</param>
        /// <returns></returns>
        public static string GenerateOrderToken(OrderToken token)
        {
            if (token == null) { throw new ArgumentNullException(nameof(token)); }
            if (string.IsNullOrWhiteSpace(token.OrderNumber)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(token.OrderNumber)), nameof(token)); }
            if (string.IsNullOrWhiteSpace(token.Email)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(token.Email)), nameof(token)); }

            var encryptor = new EncryptionUtility();

            var rawToken = string.Join(TokenSeparator, token.OrderNumber, token.Email);
            var encryptedToken = encryptor.Encrypt(rawToken);

            return encryptedToken;
        }

        /// <summary>
        /// Gets the original token data.
        /// </summary>
        /// <param name="token">Encrypted token.</param>
        /// <returns></returns>
        public static OrderToken DecypherOrderToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(token)); }

            if (!token.IsBase64String()) { return null; }

            var decryptor = new EncryptionUtility();

            var rawToken = decryptor.Decrypt(token);
            var splittedTokens = rawToken.Split(new[] { TokenSeparator }, 2, StringSplitOptions.None);

            //Invalid token.
            if (splittedTokens.Length != 2) { return null; }

            var data = new OrderToken
            {
                OrderNumber = splittedTokens[0],
                Email = splittedTokens[1]
            };
            return data;
        }
    }
}