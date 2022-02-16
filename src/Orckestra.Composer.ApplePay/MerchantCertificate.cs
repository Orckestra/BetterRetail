// Copyright (c) Just Eat, 2016. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Composite.Core;
using Orckestra.Composer.ApplePay.Options;

namespace Orckestra.Composer.ApplePay
{
    public class MerchantCertificate
    {
        private readonly ApplePayOptions _options;

        public MerchantCertificate(ApplePayOptions options)
        {
            _options = options;
        }

        public X509Certificate2 GetCertificate()
        {
            return LoadCertificateFromDisk(_options.MerchantCertificateFileName, _options.MerchantCertificatePassword);

            /* // Get the merchant certificate for two-way TLS authentication with the Apple Pay server.
             if (!string.IsNullOrWhiteSpace(_options.MerchantCertificate))
             {
                 return LoadCertificateFromBase64String(_options.MerchantCertificate, _options.MerchantCertificatePassword);
             }
             else if (_options.UseCertificateStore)
             {
                 return LoadCertificateFromStore(_options.MerchantCertificateThumbprint);
             }
             else
             {
                 return LoadCertificateFromDisk(_options.MerchantCertificateFileName, _options.MerchantCertificatePassword);
             }*/
        }

        public string GetMerchantIdentifier()
        {
            try
            {
                //using var merchantCertificate = GetCertificate();
                var merchantCertificate = GetCertificate();
                return GetMerchantIdentifier(merchantCertificate);
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }
        }

        private static string GetMerchantIdentifier(X509Certificate2 certificate)
        {
            // This OID returns the ASN.1 encoded merchant identifier
            var extension = certificate.Extensions["1.2.840.113635.100.6.32"];

            if (extension == null)
            {
                return string.Empty;
            }

            // Convert the raw ASN.1 data to a string containing the ID
            return Encoding.ASCII.GetString(extension.RawData).Substring(2);
        }

      /*  private static X509Certificate2 LoadCertificateFromBase64String(string encoded, string? password)
        {
            byte[] rawData = Convert.FromBase64String(encoded);

            return X509Certificate2.GetCertContentType(rawData) switch
            {
                X509ContentType.Pfx => new X509Certificate2(rawData, password),
                _ => throw new NotSupportedException("The format of the encoded Apple Pay merchant certificate is not supported."),
            };
        }*/

        private static X509Certificate2 LoadCertificateFromDisk(string fileName, string password)
        {
            var path = Directory.GetParent(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath).Parent.FullName;
            try
            {
                return new X509Certificate2(Path.Combine(path, fileName) ?? string.Empty, password, X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load Apple Pay merchant certificate file from '{fileName}'.", ex);
            }
        }

        /*
       private static X509Certificate2 LoadCertificateFromStore(string thumbprint)
        {
            // Load the certificate from the current user's certificate store. This
            // is useful if you do not want to publish the merchant certificate with
            // your application, but it is also required to be able to use an X.509
            // certificate with a private key if the user profile is not available,
            // such as when using IIS hosting in an environment such as Microsoft Azure.
            X509Store userCaStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                userCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = userCaStore.Certificates;
                X509Certificate2Collection findResult = certificatesInStore.Find(X509FindType.FindByThumbprint, thumbprint, false);
                X509Certificate2 clientCertificate = null;
                if (findResult.Count == 1)
                {
                    clientCertificate = findResult[0];
                }
                else
                {
                    throw new Exception("Unable to locate the correct client certificate.");
                }
                return clientCertificate;
            }
            catch
            {
                throw;
            }
            finally
            {
                userCaStore.Close();
            }
        }
        */
    }
}
