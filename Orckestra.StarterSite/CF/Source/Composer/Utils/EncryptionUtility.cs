using System;
using System.Text;
using System.Web.Security;

namespace Orckestra.Composer.Utils
{
    /// <summary>
    /// An encryption utility class.
    /// </summary>
    public class EncryptionUtility
    {
        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string Encrypt(string data)
        {
            //Encode
            byte[] plain = Encoding.UTF8.GetBytes(data);
            byte[] cipher = MachineKey.Protect(plain);

            //Base64 storable value
            string base64 = string.Empty;

            if (cipher != null)
            {
                base64 = Convert.ToBase64String(cipher);
            }

            return base64;
        }

        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string Decrypt(string data)
        {
            //Decode
            byte[] cipher = Convert.FromBase64String(data);
            byte[] plain = MachineKey.Unprotect(cipher);

            string decrypted = string.Empty;

            if (plain != null)
            {
                decrypted = Encoding.UTF8.GetString(plain);
            }

            return decrypted;
        }
    }
}
