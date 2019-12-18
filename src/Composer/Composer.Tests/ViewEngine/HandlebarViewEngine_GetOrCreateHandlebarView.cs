using System.Collections.Generic;
using System.Web.Mvc;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.ViewEngine;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Tests.ViewEngine
{
    // ReSharper disable once InconsistentNaming
    public class HandlebarViewEngine_GetOrCreateHandlebarView
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void WHEN_Passing_Any_TemplateName_Resulting_View_SHOULD_Be_Cached_ByTemplateName()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            var cachedView = new HandlebarsView((w, o) => { }, GetRandom.String(32), new Dictionary<string, HandlebarsView>());
            var monitor = new CacheProviderFactory.CacheHistMonitor();
            var cacheProvider = CacheProviderFactory.CreateForHandlebarsWithMonitor(monitor, cachedView, cachedView);
            var viewEngine = new UnitTestableHandlebarsViewEngine(cacheProvider.Object);

            string templateNameA = "SubLevel2";
            string templateNameB = "OtherLeaf";

            //Act
            monitor.Reset();
            monitor.CacheMissCount.ShouldBeEquivalentTo(0, "Otherwise this test is irrelevent");
            monitor.CacheHitCount.ShouldBeEquivalentTo(0, "Otherwise this test is irrelevent");

            var result1A = viewEngine.FindPartialView(controllerContext.Object, templateNameA, false);
            monitor.CacheMissCount.ShouldBeEquivalentTo(1, "First attempt to load the TemplateA should cache miss");

            var result2A = viewEngine.FindPartialView(controllerContext.Object, templateNameA, false);
            monitor.CacheHitCount.ShouldBeEquivalentTo(1, "Second attempt to load the TemplateA should cache hit");

            monitor.Reset();
            for (int i = 0; i < 10; i++)
            {
                var result3A = viewEngine.FindPartialView(controllerContext.Object, templateNameA, false);

            }
            monitor.CacheMissCount.ShouldBeEquivalentTo(0, "Subsequent attempt to load the TemplateA should not cache miss");
            monitor.CacheHitCount.Should().BeGreaterOrEqualTo(10, "Subsequent attempt to load the TemplateA should cache hit");

            //--
            monitor.Reset();
            var result1B = viewEngine.FindPartialView(controllerContext.Object, templateNameB, false);
            monitor.CacheMissCount.ShouldBeEquivalentTo(1, "First attempt to load the CultureB should cache miss, key is culture dependant");
            monitor.CacheHitCount.ShouldBeEquivalentTo(0, "First attempt to load the CultureB should not cache hit, key is culture dependant");
        }

        [Test]
        public void WHEN_Template_Use_SubTemplate_Dependencies_SHOULD_Contains_All_TemplateNames_From_the_Tree()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
            var cachedView = new HandlebarsView((w, o) => { }, GetRandom.String(32), new Dictionary<string, HandlebarsView>());
            cachedView.Dependencies.Add("SubLevel1", cachedView);
            cachedView.Dependencies.Add("SubLevel2", cachedView);
            cachedView.Dependencies.Add("OtherLeaf", cachedView);

            var cacheProvider = CacheProviderFactory.CreateForHandlebars(cachedView);
            var viewEngine = new UnitTestableHandlebarsViewEngine(cacheProvider.Object);

            viewEngine.ViewLocationFormats = new string[] { "~/ViewEngine/Assets/{0}.hbs" };
            viewEngine.PartialViewLocationFormats = new string[] { "~/ViewEngine/Assets/{0}.hbs" };

            //Act
            ViewEngineResult result = viewEngine.FindPartialView(controllerContext.Object, "Root", false);

            //Assert
            result.View.Should().BeOfType<HandlebarsView>();
            result.View.As<HandlebarsView>().CompiledTemplate.Should().NotBeNull();
            result.View.As<HandlebarsView>().VirtualPath.Should().NotBeNull();
            result.View.As<HandlebarsView>().Dependencies.Should().ContainKey("SubLevel1");
            result.View.As<HandlebarsView>().Dependencies.Should().ContainKey("SubLevel2");
            result.View.As<HandlebarsView>().Dependencies.Should().ContainKey("OtherLeaf");
        }

        /// <summary>
        /// Grant access to a protected method 
        /// (mostly to bypass the hard-to-mock VirtualPathProviderViewEngine overhead)
        /// </summary>
        private class UnitTestableHandlebarsViewEngine: HandlebarsViewEngine
        {
            public UnitTestableHandlebarsViewEngine(ICacheProvider cacheProvider) : base(cacheProvider)
            {
                var viewLocationCache = new Mock<IViewLocationCache>(MockBehavior.Strict);
                var composerEnvironment = ComposerEnvironmentFactory.Create();

                VirtualPathProvider = composerEnvironment.Object.VirtualPathProvider;
                ViewLocationCache   = viewLocationCache.Object;
            }

            public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
            {
                IView view = CreateView(controllerContext, string.Format("~/UI.Package/Template/{0}.hbs", viewName), "");
                return new ViewEngineResult(view, this);
            }

            public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialName, bool useCache)
            {
                IView view = CreatePartialView(controllerContext, string.Format("~/UI.Package/Template/{0}.hbs", partialName));
                return new ViewEngineResult(view, this);
            }
        }
    }
}
