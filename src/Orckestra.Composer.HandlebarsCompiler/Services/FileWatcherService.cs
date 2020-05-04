using System;
using System.IO;
using System.Linq;
using Composite.Core.Collections.Generic;

namespace Orckestra.Composer.HandlebarsCompiler.Services
{
    class FileWatcherService: IFileWatcherService
    {
        private static readonly Hashtable<string, CachedDirectoryInfo> directoryInfo = new Hashtable<string, CachedDirectoryInfo>();

        private readonly string fileWatcherMask = "*.hbs";

        public virtual DateTime GetCachedFolderLastUpdateDateUtc(string directory, string fileMask)
        {
            var cacheRecord = directoryInfo[directory] ?? WatchOnDirectory(directory, fileMask, null);

            var cachedResult = cacheRecord.LastTimeUpdatedUtc;
            if (cachedResult.HasValue) return cachedResult.Value;

            var result = GetLastTimeUpdatedUtc(directory, fileMask);
            cacheRecord.LastTimeUpdatedUtc = result;
            return result;
        }

        private DateTime GetLastTimeUpdatedUtc(string directory, string fileMask)
        {
            var files = Directory.GetFiles(directory, fileMask, SearchOption.AllDirectories);

            if (files.Length == 0) return DateTime.UtcNow;

            DateTime result = File.GetLastWriteTimeUtc(files[0]);
            foreach (var file in files.Skip(1))
            {
                DateTime lastUpdatedUtc = File.GetLastWriteTimeUtc(file);
                if (lastUpdatedUtc > result)
                {
                    result = lastUpdatedUtc;
                }
            }

            return result;
        }

        private void OnFileChanged(CachedDirectoryInfo cachedDirectoryInfo)
        {
            cachedDirectoryInfo.LastTimeUpdatedUtc = DateTime.UtcNow;
            cachedDirectoryInfo.OnChange?.Invoke();
        }

        private CachedDirectoryInfo WatchOnDirectory(string directory, string fileMask, Action onChangeAction)
        {
            lock (directoryInfo)
            {
                var cacheRecord = directoryInfo[directory];
                if (cacheRecord == null)
                {
                    cacheRecord = new CachedDirectoryInfo
                    {
                        FileSystemWatcher = new FileSystemWatcher(directory, fileMask)
                        {
                            IncludeSubdirectories = true,
                        },
                        OnChange = onChangeAction 
                    };
                    cacheRecord.FileSystemWatcher.Created += (a, b) => OnFileChanged(cacheRecord);
                    cacheRecord.FileSystemWatcher.Changed += (a, b) => OnFileChanged(cacheRecord);
                    cacheRecord.FileSystemWatcher.Deleted += (a, b) => OnFileChanged(cacheRecord);
                    cacheRecord.FileSystemWatcher.Renamed += (a, b) => OnFileChanged(cacheRecord);
                    cacheRecord.FileSystemWatcher.EnableRaisingEvents = true;

                    directoryInfo[directory] = cacheRecord;
                }

                return cacheRecord;
            }
        }

        public virtual void WatchOnDirectory(string directory, Action onChangeAction)
        {
            WatchOnDirectory(directory, fileWatcherMask, onChangeAction);
        }

        private class CachedDirectoryInfo
        {
            public FileSystemWatcher FileSystemWatcher { get; set; }
            public DateTime? LastTimeUpdatedUtc { get; set; }
            public Action OnChange { get; set; }
        }
    }
}