using System;

namespace Orckestra.Composer.HandlebarsCompiler.Services
{
    public interface IFileWatcherService
    {
        DateTime GetCachedFolderLastUpdateDateUtc(string directory, string fileMask);
        void WatchOnDirectory(string directory, Action onChangeAction);
    }
}
