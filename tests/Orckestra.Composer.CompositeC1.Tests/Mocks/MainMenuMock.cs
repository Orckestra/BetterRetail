using System;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;

namespace Orckestra.Composer.CompositeC1.Tests.Mocks
{
    public class MainMenuMock: MainMenu, Footer, StickyHeader, FooterOptionalLink, HeaderOptionalLink
    {
        public DataSourceId DataSourceId { get; }
        public Guid PageId { get; set; }
        public string PublicationStatus { get; set; }
        public string SourceCultureName { get; set; }
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public Guid? ParentId { get; set; }
        public string Url { get; set; }
        public Guid Target { get; set; }
        public int Order { get; set; }
        public Guid? CssStyle { get; set; }
        public string CssClassName { get; set; }
    }
}
