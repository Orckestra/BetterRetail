namespace Orckestra.Composer.Product.Parameters
{
    public class GetProductSpecificationsParam
    {
        public string VariantId { get; set; }
        public Overture.ServiceModel.Products.Product Product { get; set; }
        public Overture.ServiceModel.Metadata.ProductDefinition ProductDefinition { get; set; }
    }
}
