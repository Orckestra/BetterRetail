using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public class OrderHistoryViewModelFactory : IOrderHistoryViewModelFactory
    {
        protected virtual ILocalizationProvider LocalizationProvider { get; private set; }
        protected virtual IViewModelMapper ViewModelMapper { get; private set; }
        protected virtual IShippingTrackingProviderFactory ShippingTrackingProviderFactory { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHistoryViewModelFactory" /> class.
        /// </summary>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <param name="viewModelMapper">The view model mapper.</param>
        /// <param name="shippingTrackingProviderFactory"></param>
        public OrderHistoryViewModelFactory(ILocalizationProvider localizationProvider,
            IViewModelMapper viewModelMapper,
            IShippingTrackingProviderFactory shippingTrackingProviderFactory)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            ShippingTrackingProviderFactory = shippingTrackingProviderFactory ?? throw new ArgumentNullException(nameof(shippingTrackingProviderFactory));
        }

        /// <summary>
        /// Creates the view model.
        /// </summary>
        /// <param name="param">The parameters required to create the view model.</param>
        /// <returns />
        public virtual OrderHistoryViewModel CreateViewModel(GetOrderHistoryViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.OrderStatuses == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.OrderStatuses)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OrderDetailBaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderDetailBaseUrl)), nameof(param)); }

            OrderHistoryViewModel viewModel = null;

            if (param.OrderResult?.Results != null)
            {
                viewModel = new OrderHistoryViewModel
                {
                    Orders = new List<LightOrderDetailViewModel>(),
                    Pagination = BuildOrderHistoryPagination(new OrderHistoryPaginationParam
                    {
                        CultureInfo = param.CultureInfo,
                        CurrentPage = param.Page,
                        TotalNumberOfOrders = param.OrderResult.TotalCount
                    })
                };

                foreach (var rawOrder in param.OrderResult.Results)
                {
                    var order = BuildLightOrderDetailViewModel(rawOrder, param);
                    viewModel.Orders.Add(order);
                }
            }

            return viewModel;
        }

        /// <summary>
        /// Builds the order history pagination.
        /// </summary>
        /// <param name="param">The parameter parameters required to create the pagination view model.</param>
        /// <returns></returns>
        protected virtual OrderHistoryPaginationViewModel BuildOrderHistoryPagination(OrderHistoryPaginationParam param)
        {
            var totalCount = param.TotalNumberOfOrders;
            var itemsPerPage = OrderHistoryConfiguration.MaxItemsPerPage;
            var totalNumberOfPages = (int)Math.Ceiling((double)totalCount / itemsPerPage);

            var pagination = new OrderHistoryPaginationViewModel
            {
                PreviousPage = GetPreviousPage(param),
                NextPage = GetNextPage(param, totalNumberOfPages),
                Pages = GetPages(param, OrderHistoryConfiguration.MaximumNumberOfPages, totalNumberOfPages)
            };

            return pagination;
        }

        /// <summary>
        /// Gets the next page.
        /// </summary>
        /// <param name="param">The parameters required to create a view model.</param>
        /// <param name="totalNumberOfPages">The total number of pages.</param>
        /// <returns></returns>
        protected virtual OrderHistoryPageViewModel GetNextPage(OrderHistoryPaginationParam param, int totalNumberOfPages)
        {
            var nextPage = new OrderHistoryPageViewModel
            {
                DisplayName = LocalizationProvider
                    .GetLocalizedString(new GetLocalizedParam
                    {
                        Category = "List-Search",
                        Key = "B_Next",
                        CultureInfo = param.CultureInfo
                    })
            };

            if (param.CurrentPage < totalNumberOfPages)
            {
                nextPage.PageNumber = param.CurrentPage + 1;
            }
            else
            {
                nextPage.IsCurrentPage = true;
            }

            return nextPage;
        }

        /// <summary>
        /// Gets the previous page.
        /// </summary>
        /// <param name="param">The parameters required to create a view model.</param>
        /// <returns></returns>
        protected virtual OrderHistoryPageViewModel GetPreviousPage(OrderHistoryPaginationParam param)
        {
            var previousPage = new OrderHistoryPageViewModel
            {
                DisplayName = LocalizationProvider
                    .GetLocalizedString(new GetLocalizedParam
                    {
                        Category = "List-Search",
                        Key = "B_Previous",
                        CultureInfo = param.CultureInfo
                    })
            };

            if (param.CurrentPage > 1)
            {
                previousPage.PageNumber = param.CurrentPage - 1;
            }
            else
            {
                previousPage.IsCurrentPage = true;
            }

            return previousPage;
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <param name="param">The parameters required to create a view model.</param>
        /// <param name="maxPages">The maximum pages.</param>
        /// <param name="totalNumberOfPages">The total number of pages.</param>
        /// <returns></returns>
        protected virtual IEnumerable<OrderHistoryPageViewModel> GetPages(OrderHistoryPaginationParam param, int maxPages, int totalNumberOfPages)
        {
            var pages = new List<OrderHistoryPageViewModel>();
            int endPage = 0;
            int startPage = 0;

            if (totalNumberOfPages < maxPages)
            {
                startPage = 1;
                endPage = totalNumberOfPages;
            }
            else if (maxPages <= totalNumberOfPages)
            {
                int maxPagesSplit = (int)Math.Floor((double)maxPages / 2);
                int potentialStartPage = param.CurrentPage - maxPagesSplit;

                if (potentialStartPage < 1)
                {
                    startPage = 1;
                    endPage = maxPages;
                }
                else
                {
                    int potentialEndPage = param.CurrentPage + maxPagesSplit - (maxPages % 2 == 0 ? 1 : 0);

                    if (potentialEndPage > totalNumberOfPages)
                    {
                        startPage = totalNumberOfPages - maxPages + 1;
                        endPage = totalNumberOfPages;
                    }
                    else
                    {
                        startPage = potentialStartPage;
                        endPage = potentialEndPage;
                    }
                }
            }
            else if (maxPages > totalNumberOfPages)
            {
                startPage = 1;
                endPage = totalNumberOfPages;
            }

            for (var index = startPage; index <= endPage; index++)
            {
                var displayName = index.ToString(CultureInfo.InvariantCulture);
                var searchPage = new OrderHistoryPageViewModel
                {
                    DisplayName = displayName,
                    PageNumber = index,
                    IsCurrentPage = index == param.CurrentPage
                };

                pages.Add(searchPage);
            }

            return pages;
        }

        /// <summary>
        /// Builds the compact order view model.
        /// </summary>
        /// <param name="rawOrder">The raw order.</param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual LightOrderDetailViewModel BuildLightOrderDetailViewModel(OrderItem rawOrder,
           GetOrderHistoryViewModelParam param)
        {
            var lightOrderVm = new LightOrderDetailViewModel();
            var orderInfo = ViewModelMapper.MapTo<OrderDetailInfoViewModel>(rawOrder, param.CultureInfo);

            orderInfo.OrderStatus = param.OrderStatuses[rawOrder.OrderStatus];
            orderInfo.OrderStatusRaw = rawOrder.OrderStatus;

            var orderDetailUrl = UrlFormatter.AppendQueryString(param.OrderDetailBaseUrl, new NameValueCollection
                {
                    {"id", rawOrder.OrderNumber}
                });
            lightOrderVm.Url = orderDetailUrl;
            
            lightOrderVm.OrderInfos = orderInfo;

            lightOrderVm.ShipmentSummaries = new List<OrderShipmentSummaryViewModel>();
            if (rawOrder.ShipmentItems.Count > 0)
            {
                foreach (var shipment in rawOrder.ShipmentItems)
                {
                    var shipmentSummary = new OrderShipmentSummaryViewModel();

                    if (shipment.FulfillmentScheduledTimeBeginDate.HasValue)
                    {
                        shipmentSummary.ScheduledShipDate = LocalizationHelper.LocalizedFormat("General", "ShortDateFormat", shipment.FulfillmentScheduledTimeBeginDate.Value, param.CultureInfo);
                    }

                    if (param.ShipmentsTrackingInfos != null && param.ShipmentsTrackingInfos.ContainsKey(shipment.Id))
                    {
                        var trackingInfo = param.ShipmentsTrackingInfos[shipment.Id];
                        shipmentSummary.TrackingInfo = trackingInfo;
                    }

                    lightOrderVm.ShipmentSummaries.Add(shipmentSummary);
                }
            }

            return lightOrderVm;
        }
    }
}