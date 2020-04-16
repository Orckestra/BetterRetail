using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.ForTests.Factories
{
    public static class ProductFactory
    {
        private static LocalizedString _localizedDisplayName;
        private static LocalizedString _localizedDescription;
        private static Product _product;

        public static Product Create()
        {
            _localizedDisplayName = new LocalizedString();
            _localizedDescription = new LocalizedString();

            _localizedDisplayName.Add(GetRandom.String(120), GetRandom.String(GetRandom.Int(3, short.MaxValue)));
            _localizedDescription.Add(GetRandom.String(10), GetRandom.String(GetRandom.Int(3, short.MaxValue)));

            _product = Builder<Product>.CreateNew()
                .With(d => d.DisplayName = _localizedDisplayName)
                .With(d => d.Description = _localizedDescription)
                .Build();

            return _product;
        }
    }
}