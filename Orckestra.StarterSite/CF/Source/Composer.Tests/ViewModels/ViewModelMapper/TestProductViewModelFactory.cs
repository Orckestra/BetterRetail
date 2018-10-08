namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public static class TestProductViewModelFactory
    {
        public static TestProductViewModel CreateValid()
        {
            var vm = new TestProductViewModel
            {
                Name = "Chair",
                Brand = "AmericanTire",
                Price = 9.99,
                Colour = "Brown",
                Description = "An exquisite chair.",
                Category = new TestCategoryViewModel
                {
                    Id = 42,
                    Title = "Sports"
                }
            };

            vm.Bag["CustomProperty"] = 5;
            vm.Bag["MappedViewModelBagProperty"] = "I'm a mapped field!";

            vm.Category.Bag["CustomName"] = "Custom sports";

            return vm;
        }
    }
}
