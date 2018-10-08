using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product
{
    [ViewModelMetadata(typeof(ProductDetailViewModel))]
    public interface IMondouProductDetailViewModelMetadata
    {
        //this is coming from OV entity bag (custom property)
        string Size { get; set; }   
    }

}