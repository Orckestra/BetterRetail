using GraphQL;
using GraphQL.Execution;
using GraphQL.NewtonsoftJson;

namespace Orckestra.Composer.Product.GraphQL.Interfaces
{
    public interface IVariantColorDocumentWriter: IDocumentWriter
    {
    }

    public class VariantColorDocumentWriter: DocumentWriter, IVariantColorDocumentWriter
    {
        public VariantColorDocumentWriter(bool indent, IErrorInfoProvider errorInfoProvider) : base(
            indent, errorInfoProvider)
        {
        }
    }
}
