namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public class TestCategoryViewModel : TestBaseCategoryViewModel
    {
        public string Title { get; set; }
        public TestCategoryViewModel ParentCategory { get; set; }
    }
}
