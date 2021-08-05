using System;
using System.IO;
using System.Text;
using Composite.Core.Configuration;
using Composite.Core.IO;

namespace Orckestra.Composer.Utils
{
    /// <summary>
    /// Allows creating a "write" lock on a local file.
    /// Can be used for multi-instance deployments with a shared network drive, to ensure only one instance is running a certain operation.
    /// </summary>
    public class FileWriteLock : IDisposable
    {
        private FileStream _fileStream;

        private FileWriteLock(FileStream fileStream)
        {
            _fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
        }

        public void Dispose()
        {
            if (_fileStream == null) return;

            var content = Encoding.UTF8.GetBytes($"Released at {DateTime.Now}\r\n");

            _fileStream.Write(content, 0, content.Length);
            _fileStream.Flush();
            _fileStream.Dispose();
            _fileStream = null;
        }

        /// <summary>
        /// Tries to get a write access to a specific lock. If not successful, returns <value>null</value>.
        /// </summary>
        /// <param name="lockName">The name of the lock, should also be a valid filename. Could be the name of the class, that uses it.</param>
        /// <returns></returns>
        public static IDisposable TryAcquire(string lockName)
        {
            var filePath = Path.Combine(PathUtil.Resolve(GlobalSettingsFacade.TempDirectory), $"{lockName}.writelock");

            try
            {
                var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

                var content = Encoding.UTF8.GetBytes($"Acquired at {DateTime.Now}\r\n");

                fileStream.Write(content, 0, content.Length);
                fileStream.Flush();

                return new FileWriteLock(fileStream);
            }
            catch
            {
                return null;
            }
        }
    }
}
