using System;
using System.Collections.Generic;
using System.Web;
using Composite.Data;
using Composite.Data.Types;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;

// ReSharper disable InconsistentNaming

namespace Orckestra.Composer.CompositeC1.Tests.Services.FacetConfigurationContext
{
    [TestFixture]
    public class FacetConfigurationContext_GetFacetSettings
    {
        private Mock<HttpContextBase> _httpContextMoq;
        private Mock<IPage> _iPageMoq;
        private CompositeC1.Services.FacetConfigurationContext _facetConfigContext;
        private const string PageKey = "PageRenderer.IPage";

        [SetUp]
        public void SetUp()
        {
            _httpContextMoq = new Mock<HttpContextBase>();
            _iPageMoq = new Mock<IPage>();
            _iPageMoq
                .Setup(x => x.Id)
                .Returns(Guid.NewGuid());

            _httpContextMoq
                .Setup(x => x.Items)
                .Returns(() => new Dictionary<string, object>
                {
                    [PageKey] = _iPageMoq.Object,
                });

            _facetConfigContext = new CompositeC1.Services.FacetConfigurationContext(_httpContextMoq.Object);
        }

        [Test]
        public void WHEN_pageId_is_give_SHOULD_loaded_corresponding_facetSettings()
        {
        }

        [Test]
        public void WHEN_pageId_is_null_SHOULD_loaded_default_facetSettings()
        {
        }

        [Test]
        public void WHEN_no_default_facetSettings_SHOULD_loaded_any_facetSettings()
        {
        }
    };
}
