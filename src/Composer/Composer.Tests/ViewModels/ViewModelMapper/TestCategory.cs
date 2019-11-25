namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public class TestCategory : TestBaseCategory
    {
        public string Title { get; set; }
        public TestCategory ParentCategory { get; set; }
    }
}
