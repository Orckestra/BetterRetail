using System;
using Composite.Data;
using Composite.Data.Types;

namespace Orckestra.Composer.CompositeC1.Tests.Mocks
{
    public class PageMock: IPage
    {
        public DataSourceId DataSourceId { get; } = null;

        public DateTime ChangeDate { get; set; }

        public string ChangedBy { get; set; }

        public string PublicationStatus { get; set; }

        public string SourceCultureName { get; set; }

        public Guid Id { get; set; }

        public Guid TemplateId { get; set; }

        public Guid PageTypeId { get; set; }

        public string Title { get; set; }

        public string MenuTitle { get; set; }

        public string UrlTitle { get; set; }

        public string FriendlyUrl { get; set; }

        public string Description { get; set; }

        public Guid ParentPageId { get; set; }

        public string Url { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public Guid VersionId { get; set; }
    }
}
