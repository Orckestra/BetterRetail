using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using Moq;
using Orckestra.Composer.Kernel;

namespace Orckestra.Composer.Tests.Mock
{
    internal sealed class ComposerEnvironmentFactory
    {
        public static Mock<IComposerEnvironment> Create()
        {
            Mock<IComposerEnvironment> environment = new Mock<IComposerEnvironment>(MockBehavior.Strict);

            string testAppDomainAppPath = Regex.Replace(AppDomain.CurrentDomain.BaseDirectory, @"bin\\[^\\]+$", "");
            var testVirtualPathProvider = new TestAssetsVirtualPathProvider();

            environment
                .SetupGet(c => c.AppDomainAppPath)
                .Returns(testAppDomainAppPath);

            environment
                .SetupGet(c => c.VirtualPathProvider)
                .Returns(testVirtualPathProvider);

            return environment;
        }


        /// <summary>
        /// Mock a VirtualPathProvider to load test assets from the ViewEngine/Assets folder
        /// </summary>
        internal class TestAssetsVirtualPathProvider : VirtualPathProvider
        {
            private readonly string _resxTestAssetsFolder;
            private readonly string _hbsTestAssetsFolder;
            public TestAssetsVirtualPathProvider()
                : base()
            {
                _resxTestAssetsFolder = Path.Combine(Regex.Replace(AppDomain.CurrentDomain.BaseDirectory, @"bin\\[^\\]+$", ""), "Localization\\Assets");
                _hbsTestAssetsFolder  = Path.Combine(Regex.Replace(AppDomain.CurrentDomain.BaseDirectory, @"bin\\[^\\]+$", ""), "ViewEngine\\Assets");
            }

            private string MapPath(string virtualPath)
            {
                string physicalPath = null;

                if (physicalPath == null)
                {
                    physicalPath = ResxTestMapPath(virtualPath);
                }

                if (physicalPath == null)
                {
                    physicalPath = HbsTesMapPath(virtualPath);
                }

                if (physicalPath == null)
                {
                    throw new NotSupportedException("This virtualPath is not mocked");
                }

                return physicalPath;
            }

            private string ResxTestMapPath(string virtualPath)
            {
                if (virtualPath.StartsWith("~/UI.Package/LocalizedStrings"))
                {
                    return virtualPath.Replace("~/UI.Package/LocalizedStrings", _resxTestAssetsFolder)
                                      .Replace('/', '\\');
                }
                return null;
            }

            private string HbsTesMapPath(string virtualPath)
            {
                if (virtualPath.StartsWith("~/UI.Package/Template"))
                {
                    return virtualPath.Replace("~/UI.Package/Template", _hbsTestAssetsFolder)
                                      .Replace('/', '\\');
                }
                return null;
            }

            public override bool FileExists(string virtualPath)
            {
                return File.Exists(MapPath(virtualPath));
            }

            public override bool DirectoryExists(string virtualPath)
            {
                string physicalPath = MapPath(virtualPath);
                return Directory.Exists(physicalPath);
            }

            public override VirtualFile GetFile(string virtualPath)
            {
                var realPath = MapPath(virtualPath);

                return File.Exists(realPath) 
                    ? new TestAssetVirtualFile(realPath) 
                    : null;
            }

            public override VirtualDirectory GetDirectory(string virtualPath)
            {
                var realPath = MapPath(virtualPath);
                
                return Directory.Exists(realPath) 
                    ? new TestAssetVirtualDirectory(realPath) 
                    : null;
            }
        }

        private class TestAssetVirtualFile : VirtualFile
        {
            public TestAssetVirtualFile(string physicalPath) : base(physicalPath) { }

            public override Stream Open()
            {
                return File.OpenRead(VirtualPath);
            }
        }
        private class TestAssetVirtualDirectory : VirtualDirectory
        {
            public TestAssetVirtualDirectory(string physicalPath) : base(physicalPath) { }


            public override IEnumerable Directories
            {
                get
                {
                    return Directory.EnumerateDirectories(VirtualPath, "*")
                                    .Select(path => new TestAssetVirtualDirectory(path));
                }
            }

            public override IEnumerable Files
            {
                get 
                {
                    return Directory.EnumerateFiles(VirtualPath, "*")
                                    .Select(path => new TestAssetVirtualFile(path));
                }
            }

            public override IEnumerable Children
            {
                get
                {
                    return Directory.EnumerateFileSystemEntries(VirtualPath, "*", SearchOption.AllDirectories)
                        .Select(
                            path =>
                                Directory.Exists(path)
                                    ? (object)new TestAssetVirtualDirectory(path)
                                    : (object)new TestAssetVirtualFile(path));
                }
            }
        }
    }
}
