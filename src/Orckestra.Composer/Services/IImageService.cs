﻿using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Services
{
    public interface IImageService
    {
        Task<List<ProductMainImage>> GetImageUrlsAsync(IEnumerable<LineItem> lineItems);
        Task<List<ProductMainImage>> GetImageUrlsAsync(ListOfRecurringOrderLineItems list);
        Task<List<ProductMainImage>> GetImageUrlsAsync(RecurringOrderLineItem lineItem);
        Task<List<ProductMainImage>> GetImageUrlsAsync(ICollection<(string productId, string variantId, string imageUrl)> products);
        Task<List<ProductMainImage>> GetImageUrlsFromProperty(List<ProductDocument> documents);
        Task<List<ProductMainImage>> GetImageUrlsFromProperty(List<LineItem> documents);
    }
}
