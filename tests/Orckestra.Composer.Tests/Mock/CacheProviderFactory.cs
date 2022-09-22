using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Orckestra.Composer.Caching;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewEngine;

namespace Orckestra.Composer.Tests.Mock
{
    internal class CacheProviderFactory
    {
        internal static Mock<ICacheProvider> CreateForLocalizationTree()
        {
            var cacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);

            //3.8 upgrade
            cacheProvider
                .Setup(provider => provider.Get<bool>(
                    It.IsNotNull<CacheKey>()))
                .Returns(false);

            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<LocalizationTree>>>(),
                    It.IsAny<Func<LocalizationTree, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<LocalizationTree>>, Func<LocalizationTree, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func());

            cacheProvider
                .Setup(provider => provider.Set(
                    It.IsNotNull<CacheKey>(), 
                    It.IsAny<object>(), 
                    It.IsNotNull<CacheKey>()));

            cacheProvider
                .Setup(provider => provider.SetAsync(
                    It.IsNotNull<CacheKey>(), 
                    It.IsAny<object>(), 
                    It.IsNotNull<CacheKey>()));

            return cacheProvider;
        }

        internal class CacheHistMonitor
        {
            public HashSet<string> CacheKeys = new HashSet<string>();
            public int CacheMissCount = 0;
            public int CacheHitCount  = 0;

            public void Reset()
            {
                CacheMissCount = 0;
                CacheHitCount  = 0;
            }
        }
        internal static Mock<ICacheProvider> CreateWithMonitor<T>(CacheHistMonitor monitor, T mockedOutput)
        {
            Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);

            //3.8 upgrade
            cacheProvider
                .Setup(provider => provider.Get<object>(
                    It.IsNotNull<CacheKey>()))
                .Returns<CacheKey,object>((c, o) =>
                         {
                             if (monitor.CacheKeys.Contains(c.GetFullCacheKey()))
                             {
                                 //Note: output object will be trash in test only.
                                 monitor.CacheHitCount++;
                                 return true;
                             }
                             else
                             {
                                 monitor.CacheMissCount++;
                                 return false;
                             }
                         });

            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<LocalizationTree>>>(),
                    It.IsAny<Func<LocalizationTree, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<LocalizationTree>>, Func<LocalizationTree, Task>,
                    CacheKey>((key, func, arg3, arg4) => 
                        {
                            if (monitor.CacheKeys.Contains(key.GetFullCacheKey()))
                            {
                                //Note: output object will be trash in test only.
                                monitor.CacheHitCount++;
                                return func();
                            }
                            else
                            {
                                monitor.CacheKeys.Add(key.GetFullCacheKey());
                                monitor.CacheMissCount++;
                                return func(); 
                            }

                            
                        }); 

            cacheProvider.Setup(provider => provider.SetAsync(
                It.IsNotNull<CacheKey>(), 
                It.IsAny<object>(), 
                It.IsAny<CacheKey>()))
                         .Callback<CacheKey,object, CacheKey>((c, o, k) =>
                         {
                             monitor.CacheKeys.Add(c.GetFullCacheKey());
                         });

            return cacheProvider;
        }

        internal static Mock<ICacheProvider> CreateForHandlebarsWithMonitor<T>(CacheHistMonitor monitor, T mockedOutput, HandlebarsView cachedView)
        {
            Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);
            cacheProvider
                .Setup(provider => provider.GetOrAdd(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<HandlebarsView>>(),
                    null,
                    null))
                .Returns<CacheKey, object, object, CacheKey>((c, o, a, b) =>
                 {
                     if (monitor.CacheKeys.Contains(c.GetFullCacheKey()))
                     {
                         //Note: output object will be trash in test only.
                         monitor.CacheHitCount++;
                         return cachedView;
                     }
                     else
                     {
                         monitor.CacheMissCount++;
                         monitor.CacheKeys.Add(c.GetFullCacheKey());
                         return cachedView;
                     }
                 });

            return cacheProvider;
        }

        internal static Mock<ICacheProvider> CreateForHandlebars(HandlebarsView cachedView)
        {
            Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);
            cacheProvider
                .Setup(provider => provider.GetOrAdd(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<HandlebarsView>>(),
                    null,
                    null))
                .Returns(cachedView);

            return cacheProvider;
        }
    }
}
