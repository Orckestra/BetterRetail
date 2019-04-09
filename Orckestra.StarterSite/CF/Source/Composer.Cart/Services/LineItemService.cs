using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

namespace Orckestra.Composer.Cart.Services
{
    public class LineItemService : ILineItemService
    {
        protected IDamProvider DamProvider { get; private set; }
        public ILineItemValidationProvider LineItemValidationProvider { get; set; }

        public LineItemService(IDamProvider damProvider, ILineItemValidationProvider lineItemValidationProvider)
        {
            if (damProvider == null) { throw new ArgumentNullException("damProvider"); }
            if (lineItemValidationProvider == null) { throw new ArgumentNullException("lineItemValidationProvider"); }

            DamProvider = damProvider;
            LineItemValidationProvider = lineItemValidationProvider;
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems)
        {
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = CartConfiguration.ThumbnailImageSize,
                ProductImageRequests = lineItems
                    .Select(li => new ProductImageRequest
                    {
                        ProductId = li.ProductId,
                        Variant = new VariantKey
                        {
                            Id = li.VariantId,
                            KeyVariantAttributeValues = li.KvaValues

                        },
                        PropertyBag = li.PropertyBag,
                        ProductDefinitionName = li.ProductDefinitionName
                    })
                    .ToList()
            };
            return DamProvider.GetProductMainImagesAsync(getImageParam);
        }

        public List<LineItem> GetInvalidLineItems(ProcessedCart cart)
        {
            if (cart == null) { throw new ArgumentNullException("cart"); }

            var lineItems = cart.GetLineItems();

            var invalidLineItems = lineItems.Where(lineItem => !LineItemValidationProvider.ValidateLineItem(cart, lineItem)).ToList();
            return invalidLineItems;
        }

        public virtual Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list)
        {
            var getImageParam = new GetProductMainImagesParam
            {
                ImageSize = CartConfiguration.ThumbnailImageSize,
                ProductImageRequests = list.RecurringOrderLineItems
                    .Select(li => new ProductImageRequest
                    {
                        ProductId = li.ProductId,
                        Variant = new VariantKey
                        {
                            Id = li.VariantId,
                        },
                    })
                    .ToList()
            };
            return DamProvider.GetProductMainImagesAsync(getImageParam);
        }
    }
}
