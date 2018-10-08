using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Caching;
using Composite;
using Composite.C1Console.Security;
using Composite.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Orckestra.Composer.CompositeC1.Azure.OutputCache
{
    public class BlobOutputCacheProvider : OutputCacheProvider
    {
        private static readonly string LogTitle = typeof(BlobOutputCacheProvider).Name;

        private const int LoggedExceptionsLimit = 10000;
        private const int ContainerCreationAttemptsLimit = 10000;

        private const string DefaultProviderName = "AzureOutputCacheStore";
        private const string DefaultProviderDescription = "Azure Blob as a output cache data store";

        //todo: do it: make it extension method
        public static List<string> UrlPathsToIgnore = new List<string>(new[]
        {
            "/composite/",
            "/media/",
            "/renderers/",
            "/frontend/",
            "/UI.Package/",
            "/api/",
            "/Deployment/"
        });

        private CloudStorageAccount _storageAccount;
        private string _containerName;

        private static readonly object ContainerCreationLock = new object();

        private static readonly TimeSpan DelayBetweenContainerCreationAttempts = TimeSpan.FromSeconds(1);
        private DateTime? _lastFailContainerCreationAttemptTime;

        private static int _containerCreationAttemptsCounter;
        private static int _loggedExceptionsCounter;


        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            Verify.ArgumentNotNull(config, "config");

            Func<string, string> getRequiredAttribute = attributeName =>
            {
                var value = config[attributeName];
                Verify.That(!string.IsNullOrEmpty(value), "Missing required '{0}' provider attribute '{1}'",
                    GetType(), attributeName);
                return value;
            };

            string accountName = getRequiredAttribute("accountName");
            string accountKey = getRequiredAttribute("accountKey");
            string containerName = getRequiredAttribute("container");

            string description = config["description"];

            if (string.IsNullOrWhiteSpace(name))
            {
                name = DefaultProviderName;
            }

            if (String.IsNullOrEmpty(description))
            {
                config["description"] = DefaultProviderDescription;
            }

            base.Initialize(name, config);

            var credentials = new StorageCredentials(accountName, accountKey);
            _storageAccount = new CloudStorageAccount(credentials, useHttps: true);
            _containerName = containerName;
            var client = _storageAccount.CreateCloudBlobClient();

            try
            {
                client.GetContainerReference(_containerName).CreateIfNotExists();
            }
            catch (Exception ex)
            {
                if (IsHttpError(ex, HttpStatusCode.Conflict))
                {
                    // Container cannot be created as it is deleted at the moment
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Failed to create blob container '{0}'", containerName), ex);
                }
            }
        }


        public virtual bool ShouldCacheKeyBeIgnored(string key)
        {
            string urlPath = key.Substring(2); // Key usually starts with "a1" or "a2" 

            return UrlPathsToIgnore.Any(path => urlPath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                   || UserValidationFacade.IsLoggedIn();
        }


        /// <summary>
        /// Returns a reference to the specified entry in the output cache.
        /// </summary>
        /// <returns>
        /// The <paramref name="key"/> value that identifies the specified entry in the cache, or null if the specified entry is not in the cache.
        /// </returns>
        /// <param name="key">A unique identifier for a cached entry in the output cache. </param>
        public override object Get(string key)
        {
            if (ShouldCacheKeyBeIgnored(key))
            {
                return null;
            }

            try
            {
                var blob = GetBlockBlobReference(key);

                BlobCacheItem cacheItem;

                using (var ms = new MemoryStream())
                {
                    blob.DownloadToStream(ms);
                    ms.Position = 0;
                    cacheItem = (BlobCacheItem)GetObjectFromBytes(ms.ToArray());
                }

                if (cacheItem.UtcExpire > DateTime.UtcNow)
                {
                    var cachedRawResponse = cacheItem.Item;
                    return cachedRawResponse;
                }
            }
            catch (Exception ex)
            {
                HandleBlobException(ex);
            }

            return null;
        }

        /// <summary>
        /// Inserts the specified entry into the output cache. 
        /// </summary>
        /// <returns>
        /// A reference to the specified provider. 
        /// </returns>
        /// <param name="key">A unique identifier for <paramref name="entry"/>.</param>
        /// <param name="entry">The content to add to the output cache.</param>
        /// <param name="utcExpiry">The time and date on which the cached entry expires.</param>
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            try
            {
                var blob = GetBlockBlobReference(key);

                if (blob.Exists())
                {
                    return Get(key);
                }

                var cacheItem = new BlobCacheItem
                {
                    Item = entry,
                    UtcExpire = utcExpiry
                };

                using (var ms = new MemoryStream(GetBytesFromObject(cacheItem)))
                {
                    blob.UploadFromStream(ms);
                }

                return entry;
            }
            catch (Exception ex)
            {
                HandleBlobException(ex, true);
            }
            return null;
        }

        /// <summary>
        /// Inserts the specified entry into the output cache, overwriting the entry if it is already cached.
        /// </summary>
        /// <param name="key">A unique identifier for <paramref name="entry"/>.</param>
        /// <param name="entry">The content to add to the output cache.</param>
        /// <param name="utcExpiry">The time and date on which the cached <paramref name="entry"/> expires.</param>
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            try
            {
                var blob = GetBlockBlobReference(key);

                var cacheItem = new BlobCacheItem
                {
                    Item = entry,
                    UtcExpire = utcExpiry
                };

                using (var ms = new MemoryStream(GetBytesFromObject(cacheItem)))
                {
                    blob.UploadFromStream(ms);
                }

            }
            catch (Exception ex)
            {
                HandleBlobException(ex, true);
            }
        }

        /// <summary>
        /// Removes the specified entry from the output cache.
        /// </summary>
        /// <param name="key">The unique identifier for the entry to remove from the output cache. </param>
        public override void Remove(string key)
        {
            try
            {
                var blob = GetBlockBlobReference(key);

                blob.DeleteIfExists();
            }
            catch (Exception ex)
            {
                HandleBlobException(ex);
            }
        }


        private bool IsHttpError(Exception ex, HttpStatusCode statusCode)
        {
            var storageException = ex as StorageException;
            var webException = storageException == null ? null : storageException.InnerException as WebException;
            return webException != null
                   && webException.Status == WebExceptionStatus.ProtocolError
                   && webException.Response is HttpWebResponse
                   && ((HttpWebResponse)webException.Response).StatusCode == statusCode;
        }


        private void HandleBlobException(Exception ex, bool createContainerIf404 = false)
        {
            if (IsHttpError(ex, HttpStatusCode.NotFound))
            {
                if (createContainerIf404)
                {
                    TryCreateContainer();
                }
                return;
            }

            if (_loggedExceptionsCounter < LoggedExceptionsLimit
                && Interlocked.Increment(ref _loggedExceptionsCounter) <= LoggedExceptionsLimit)
            {
                Log.LogError(LogTitle, ex);
            }
        }

        private void TryCreateContainer()
        {
            if (_containerCreationAttemptsCounter >= ContainerCreationAttemptsLimit)
            {
                return;
            }

            DateTime? lastAttempt = _lastFailContainerCreationAttemptTime;
            if (lastAttempt != null && DateTime.Now - lastAttempt.Value < DelayBetweenContainerCreationAttempts)
            {
                return;
            }

            bool entered = false;
            try
            {
                Monitor.TryEnter(ContainerCreationLock, 0, ref entered);
                if (!entered) return;

                _containerCreationAttemptsCounter++;

                var client = _storageAccount.CreateCloudBlobClient();
                client.GetContainerReference(_containerName).CreateIfNotExists();

                _lastFailContainerCreationAttemptTime = null;
            }
            catch (Exception ex)
            {
                _lastFailContainerCreationAttemptTime = DateTime.Now;

                if (IsHttpError(ex, HttpStatusCode.Conflict))
                {
                    // Http 409 (Conflict) happends when the container is being deleted
                }
                else
                {
                    Log.LogError(LogTitle, String.Format("Failed to create container {0}", _containerName));
                    Log.LogError(LogTitle, ex);
                }
            }
            finally
            {
                if (entered)
                {
                    Monitor.Exit(ContainerCreationLock);
                }
            }
        }

        private CloudBlockBlob GetBlockBlobReference(string key)
        {
            var md5Key = CreateMD5Key(key);
            var client = _storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(_containerName);
            return container.GetBlockBlobReference(md5Key);
        }

        private string CreateMD5Key(string key)
        {
            using (var md5 = MD5.Create())
            {
                var sb = new StringBuilder();
                foreach (var b in md5.ComputeHash(Encoding.Unicode.GetBytes(key)))
                {
                    sb.Append(b.ToString("x2").ToLower());
                }
                return sb.ToString();
            }
        }

        private static byte[] GetBytesFromObject(object data)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, data);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private static object GetObjectFromBytes(byte[] dataAsBytes)
        {
            if (dataAsBytes == null)
            {
                return null;
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(dataAsBytes, 0, dataAsBytes.Length))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
