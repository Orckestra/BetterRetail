using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Providers.Order;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services.Order
{
    public class OrderHistoryViewService : IOrderHistoryViewService
    {
        public enum PickingResult
        {
            Substituted,
            NotAvailable,
            Available
        }

        protected virtual IOrderHistoryViewModelFactory OrderHistoryViewModelFactory { get; private set; }
        protected virtual IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected virtual ILookupService LookupService { get; private set; }
        protected virtual IOrderRepository OrderRepository { get; private set; }
        protected virtual ICartRepository CartRepository { get; private set; }
        protected virtual IOrderDetailsViewModelFactory OrderDetailsViewModelFactory { get; private set; }
        protected virtual IImageService ImageService { get; private set; }
        protected virtual IShippingTrackingProviderFactory ShippingTrackingProviderFactory { get; private set; }
        protected virtual ICustomerRepository CustomerRepository { get; private set; }
        protected virtual IComposerContext ComposerContext { get; private set; }
        protected virtual ICartUrlProvider CartUrlProvider { get; private set; }
        protected IComposerJsonSerializer ComposerJsonSerializer { get; }
        public ILineItemViewModelFactory LineItemViewModelFactory { get; }
        protected virtual IEditingOrderProvider EditingOrderProvider { get; private set; }
        protected virtual ICheckoutService CheckoutService { get; private set; }
        protected virtual ICartService CartService { get; private set; }


        public OrderHistoryViewService(
            IOrderHistoryViewModelFactory orderHistoryViewModelFactory,
            IOrderRepository orderRepository,
            IOrderUrlProvider orderUrlProvider,
            ILookupService lookupService,
            IOrderDetailsViewModelFactory orderDetailsViewModelFactory,
            IImageService imageService,
            IShippingTrackingProviderFactory shippingTrackingProviderFactory,
            ICustomerRepository customerRepository,
            IComposerJsonSerializer composerJsonSerializer,
            ILineItemViewModelFactory lineItemViewModelFactory,
            ICartRepository cartRepository,
            IComposerContext composerContext, 
            ICartUrlProvider cartUrlProvider,
            IEditingOrderProvider editingOrderProvider,
            ICheckoutService checkoutService,
            ICartService cartService)
        {
            OrderHistoryViewModelFactory = orderHistoryViewModelFactory ?? throw new ArgumentNullException(nameof(orderHistoryViewModelFactory));
            OrderUrlProvider = orderUrlProvider ?? throw new ArgumentNullException(nameof(orderUrlProvider));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            OrderDetailsViewModelFactory = orderDetailsViewModelFactory ?? throw new ArgumentNullException(nameof(orderDetailsViewModelFactory));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            ShippingTrackingProviderFactory = shippingTrackingProviderFactory ?? throw new ArgumentNullException(nameof(shippingTrackingProviderFactory));
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            ComposerJsonSerializer = composerJsonSerializer ?? throw new ArgumentNullException(nameof(composerJsonSerializer));
            LineItemViewModelFactory = lineItemViewModelFactory ?? throw new ArgumentNullException(nameof(lineItemViewModelFactory));
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
            EditingOrderProvider = editingOrderProvider ?? throw new ArgumentNullException(nameof(editingOrderProvider));
            CheckoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
            CartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        /// <summary>
        /// Gets the OrderHistory ViewModel, containing a list of Orders.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<OrderHistoryViewModel> GetOrderHistoryViewModelAsync(GetCustomerOrdersParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var orderQueryResult = await OrderRepository.GetCustomerOrdersAsync(param).ConfigureAwait(false);
            if (orderQueryResult == null)
            {
                return null;
            }

            var orderDetailBaseUrl = OrderUrlProvider.GetOrderDetailsBaseUrl(param.CultureInfo);
            var orderStatuses = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "OrderStatus",
            }).ConfigureAwait(false);

           
            var shipmentsTrackingInfos = new Dictionary<Guid, TrackingInfoViewModel>();
            var ordersDetails = new List<Overture.ServiceModel.Orders.Order>();
            var orderCartDrafts = new List<CartSummary>();
            var orderEditingInfos = new Dictionary<Guid, bool>();
            var orderCancellationInfos = new Dictionary<Guid, CancellationStatus>();
            if (orderQueryResult.Results != null && param.OrderTense == OrderTense.CurrentOrders)
            {
                ordersDetails = await GetOrders(orderQueryResult, param).ConfigureAwait(false);
                orderCartDrafts = await GetOrderCartDrafts(param.Scope, param.CustomerId, param.CultureInfo).ConfigureAwait(false);
                orderEditingInfos = await GetOrderEditingInfos(ordersDetails).ConfigureAwait(false);
                orderCancellationInfos = await GetCancellationStatus(ordersDetails).ConfigureAwait(false);
                shipmentsTrackingInfos = GetShipmentsTrackingInfoViewModels(ordersDetails, param);
            }

            Guid.TryParse(EditingOrderProvider.GetCurrentEditingCartName(), out Guid currentlyEditingOrder);
            var getOrderHistoryViewModelParam = new GetOrderHistoryViewModelParam
            {
                CultureInfo = param.CultureInfo,
                OrderResult = orderQueryResult,
                OrderCartDrafts = orderCartDrafts,
                OrderStatuses = orderStatuses,
                Page = param.Page,
                OrderDetailBaseUrl = orderDetailBaseUrl,
                ShipmentsTrackingInfos = shipmentsTrackingInfos,
                OrderEditingInfos = orderEditingInfos,
                Orders = ordersDetails,
                OrderCancellationStatusInfos = orderCancellationInfos,
                CurrentlyEditedOrderId = currentlyEditingOrder
            };

            var viewModel = OrderHistoryViewModelFactory.CreateViewModel(getOrderHistoryViewModelParam);

            return viewModel;
        }

        private async Task<Dictionary<Guid, bool>> GetOrderEditingInfos(List<Overture.ServiceModel.Orders.Order> orders)
        {
            var orderEditingInfos = new Dictionary<Guid, bool>();

            foreach (var order in orders)
            {
                orderEditingInfos.Add(Guid.Parse(order.Id), await EditingOrderProvider.CanEdit(order).ConfigureAwait(false));
            }

            return orderEditingInfos;
        }

        private async Task<Dictionary<Guid, CancellationStatus>> GetCancellationStatus(List<Overture.ServiceModel.Orders.Order> orders)
        {
            var cancellationStatusInfo = new Dictionary<Guid, CancellationStatus>();

            foreach (var order in orders)
            {
                cancellationStatusInfo.Add(Guid.Parse(order.Id), await EditingOrderProvider.GetCancellationStatus(order).ConfigureAwait(false));
            }

            return (cancellationStatusInfo);
        }

        protected virtual Task<List<CartSummary>> GetOrderCartDrafts(string scope, Guid customerId, CultureInfo cultureInfo)
        {
            return CartRepository.GetCartsByCustomerIdAsync(new GetCartsByCustomerIdParam
            {
                Scope = scope,
                CustomerId = customerId,
                CultureInfo = cultureInfo,
                CartType = CartConfiguration.OrderDraftCartType
            });
        }

        protected virtual async Task<List<Overture.ServiceModel.Orders.Order>> GetOrders(OrderQueryResult orderQueryResult,
            GetCustomerOrdersParam param)
        {
            var getOrderTasks = orderQueryResult.Results.Select(order => OrderRepository.GetOrderAsync(new GetCustomerOrderParam
            {
                OrderNumber = order.OrderNumber,
                Scope = param.Scope
            }));

            var orders =await Task.WhenAll(getOrderTasks).ConfigureAwait(false);
            return orders.ToList();
        }

        protected virtual Dictionary<Guid, TrackingInfoViewModel> GetShipmentsTrackingInfoViewModels(
            List<Overture.ServiceModel.Orders.Order> orders,
            GetCustomerOrdersParam param)
        {
            var shipmentsTrackingInfos = new Dictionary<Guid, TrackingInfoViewModel>();

            foreach (var order in orders)
            {
                foreach (var shipment in order.Cart.Shipments)
                {
                    var shippingTrackingProvider = ShippingTrackingProviderFactory.ResolveProvider(shipment.FulfillmentMethod.Name);
                    var trackingInfoVm = shippingTrackingProvider.GetTrackingInfoViewModel(shipment, param.CultureInfo);
                    shipmentsTrackingInfos.Add(shipment.Id, trackingInfoVm);
                }
            }

            return shipmentsTrackingInfos;
        }

        /// <summary>
        /// Gets an OrderDetailViewModel, containing all information about an order and his shipments.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<OrderDetailViewModel> GetOrderDetailViewModelAsync(GetCustomerOrderParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CustomerId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CustomerId)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderNumber)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CountryCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CountryCode)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var order = await OrderRepository.GetOrderAsync(param).ConfigureAwait(false);

            //Check if order is one of the current customer.
            if (order == null || Guid.Parse(order.CustomerId) != param.CustomerId) { return null; }

            var viewModel = await BuildOrderDetailViewModelAsync(order, param).ConfigureAwait(false);

            return viewModel;
        }

        /// <summary>
        /// Gets an OrderDetailViewModel for a guest customer, containing all information about an order and his shipments.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<OrderDetailViewModel> GetOrderDetailViewModelForGuestAsync(GetOrderForGuestParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderNumber)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CountryCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CountryCode)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Email)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Email)), nameof(param)); }

            var order = await OrderRepository.GetOrderAsync(param).ConfigureAwait(false);

            if (order == null || order.Cart.Customer.Email != param.Email) { return null; }

            var viewModel = await BuildOrderDetailViewModelAsync(order, param).ConfigureAwait(false);

            return viewModel;
        }

        /// <summary>
        /// Update Order Customer Id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<Overture.ServiceModel.Orders.Order> UpdateOrderCustomerAsync(UpdateOrderCustomerParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo))); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope))); }
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderNumber))); }
            if (param.CustomerId == default) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId))); }

            var order = await OrderRepository.GetOrderAsync(param).ConfigureAwait(false);
            if (order == null)
            {
                return null;
            }

            var customer = await CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
            {
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            if (customer == null || order.Cart.Customer.Email != customer.Email)
            {
                return null;
            }

            order.CustomerId = customer.Id.ToString();
            var updatedOrder = await OrderRepository.UpdateOrderAsync(new UpdateOrderParam
            {
                Scope = param.Scope,
                Order = order,
                OrderId = Guid.Parse(order.Id)
            });

            return updatedOrder;
        }

        protected virtual async Task<OrderDetailViewModel> BuildOrderDetailViewModelAsync(
            Overture.ServiceModel.Orders.Order order,
            GetOrderParam getOrderParam)
        {
            Helper.LineItemsHelper.PrepareGiftLineItems(order.Cart);

            var shipmentsNotes = await GetShipmentsNotes(order.Cart.Shipments, getOrderParam.Scope).ConfigureAwait(false);

            var orderChanges = await OrderRepository.GetOrderChangesAsync(new GetOrderChangesParam
            {
                OrderNumber = getOrderParam.OrderNumber,
                Scope = getOrderParam.Scope
            }).ConfigureAwait(false);

            var orderCartDrafts = await GetOrderCartDrafts(getOrderParam.Scope, Guid.Parse(order.CustomerId), getOrderParam.CultureInfo).ConfigureAwait(false);

            var orderStatuses = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = getOrderParam.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "OrderStatus",
            }).ConfigureAwait(false);

            var shipmentStatuses = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = getOrderParam.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "ShipmentStatus",
            }).ConfigureAwait(false);

            var productImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(order.Cart.GetLineItems()).ConfigureAwait(false)
            };

            var viewModel = OrderDetailsViewModelFactory.CreateViewModel(new CreateOrderDetailViewModelParam
            {
                Order = order,
                OrderChanges = orderChanges,
                OrderStatuses = orderStatuses,
                ShipmentStatuses = shipmentStatuses,
                CultureInfo = getOrderParam.CultureInfo,
                CountryCode = getOrderParam.CountryCode,
                ProductImageInfo = productImageInfo,
                BaseUrl = getOrderParam.BaseUrl,
                ShipmentsNotes = shipmentsNotes,
                OrderCartDrafts = orderCartDrafts
            });

            viewModel.OrderInfos.IsOrderEditable = await EditingOrderProvider.CanEdit(order).ConfigureAwait(false);
            viewModel.OrderInfos.IsBeingEdited = EditingOrderProvider.IsBeingEdited(order);

            var orderCancellationStatus = await EditingOrderProvider.GetCancellationStatus(order).ConfigureAwait(false);
            viewModel.OrderInfos.IsOrderCancelable = orderCancellationStatus.CanCancel;
            viewModel.OrderInfos.IsOrderPendingCancellation = orderCancellationStatus.CancellationPending;

            if (order.Cart.PropertyBag.TryGetValue("PickedItems", out var pickedItemsObject))
            {
                var pickedItemsList = ComposerJsonSerializer.Deserialize<List<PickedItemViewModel>>(pickedItemsObject.ToString());
                var shipment = viewModel.Shipments.First();
                shipment.LineItems = await ProcessPickedLineItemsAsync(pickedItemsList, shipment.LineItems, getOrderParam.CultureInfo).ConfigureAwait(false);
            };

            return viewModel;
        }

        protected virtual async Task<List<LineItemDetailViewModel>> ProcessPickedLineItemsAsync(List<PickedItemViewModel> pickedItemsList,
            List<LineItemDetailViewModel> lineItemsList,
            CultureInfo culture)
        {
            var itemsToProcess = pickedItemsList
               .Where(x => x.PickingResult == PickingResult.Substituted.ToString() || x.PickingResult == PickingResult.NotAvailable.ToString())
               .Select(x => (x.ProductId, x.VariantId)).ToList();

            if (!itemsToProcess.Any()) return lineItemsList;

            var result = new List<LineItemDetailViewModel>();
            var imgDictionary = await CreateImageDictionary(itemsToProcess).ConfigureAwait(false);
            foreach (var pickedItem in pickedItemsList)
            {
                var itemPickingStatus = (PickingResult)Enum.Parse(typeof(PickingResult), pickedItem.PickingResult, true);
                LineItemDetailViewModel toAddVM = null;

                if (itemPickingStatus == PickingResult.Available)
                {
                    foreach (var lineItem in lineItemsList)
                    {
                        if (lineItem.ProductId != pickedItem.ProductId || lineItem.VariantId != pickedItem.VariantId) continue;
                        if (lineItem.Quantity == pickedItem.Quantity)
                        {
                            toAddVM = lineItem;
                        }
                        else
                        {
                            var createdLineItem = CreateLineItemFromPickedItem(pickedItem);
                            toAddVM = LineItemViewModelFactory.GetLineItemDetailViewModel(new CreateLineItemDetailViewModelParam
                            {
                                LineItem = createdLineItem,
                                CultureInfo = culture
                            });
                            toAddVM.ImageUrl = lineItem.ImageUrl;
                            toAddVM.FallbackImageUrl = lineItem.FallbackImageUrl;
                        }
                        break;
                    }
                }
                else if (itemPickingStatus == PickingResult.NotAvailable || itemPickingStatus == PickingResult.Substituted)
                {
                    LineItem toAddLineItem = CreateLineItemFromPickedItem(pickedItem);

                    toAddVM = LineItemViewModelFactory.GetLineItemDetailViewModel(new CreateLineItemDetailViewModelParam
                    {
                        LineItem = toAddLineItem,
                        CultureInfo = culture,
                        ImageDictionary = imgDictionary
                    });

                    if (itemPickingStatus == PickingResult.Substituted)
                    {
                        toAddVM.IsSubstituted = true;
                    }
                    else if (itemPickingStatus == PickingResult.NotAvailable)
                    {
                        toAddVM.IsUnavailable = true;
                    }
                }
                result.Add(toAddVM);
            }

            return result;
        }

        private async Task<IDictionary<(string ProductId, string VariantId), ProductMainImage>> CreateImageDictionary(List<(string ProductId, string VariantId)> itemsToProcess)
        {
            var productImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(itemsToProcess).ConfigureAwait(false)
            };
            var imgDictionary = LineItemHelper.BuildImageDictionaryFor(productImageInfo.ImageUrls);
            return imgDictionary;
        }

        protected virtual LineItem CreateLineItemFromPickedItem(PickedItemViewModel pickedItem)
        {
            return new LineItem
            {
                ProductId = pickedItem.ProductId,
                Quantity = pickedItem.DisplayQuantity,
                CurrentPrice = pickedItem.DisplayPrice,
                DefaultPrice = pickedItem.DisplayPrice,
                Total = pickedItem.DisplayTotalPrice,
                VariantId = pickedItem.VariantId,
                ProductSummary = new CartProductSummary
                {
                    DisplayName = pickedItem.ProductTitle
                }
            };
        }

        protected virtual async Task<Dictionary<Guid, List<string>>> GetShipmentsNotes(List<Shipment> shipments, string scope)
        {
            var shipmentsNotes = new Dictionary<Guid, List<string>>();

            foreach (var shipmentId in shipments.Select(s => s.Id))
            {
                var notes = await OrderRepository.GetShipmentNotesAsync(new GetShipmentNotesParam
                {
                    ShipmentId = shipmentId,
                    Scope = scope
                }).ConfigureAwait(false);

                if (notes != null)
                {
                    shipmentsNotes.Add(shipmentId, notes.Select(n => n.Content).ToList());
                }
            }
            return shipmentsNotes;
        }

        public virtual async Task<EditingOrderViewModel> CreateEditingOrderViewModel(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(orderNumber)));

            var getOrderParam = new GetCustomerOrderParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                OrderNumber = orderNumber,
                Scope = Constants.GlobalScopeName
            };

            var order = await OrderRepository.GetOrderAsync(getOrderParam).ConfigureAwait(false);

            if (order == null)
            {
                throw new InvalidOperationException($"Cannot edit order #${orderNumber} as it doesn't exist.");
            }

            var isOrderEditable = await EditingOrderProvider.CanEdit(order).ConfigureAwait(false);
            if (!isOrderEditable) throw new InvalidOperationException($"Cannot edit this order #${orderNumber}");

 
            if (EditingOrderProvider.IsBeingEdited(order))
            {
                return GetEditingOrderViewModel();
            }

            await EditingOrderProvider.StartEditOrderModeAsync(order).ConfigureAwait(false);

            return GetEditingOrderViewModel();
        }

        public virtual async Task CancelEditingOrderAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(orderNumber)));

            var getOrderParam = new GetCustomerOrderParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                OrderNumber = orderNumber,
                Scope = Constants.GlobalScopeName
            };

            var order = await OrderRepository.GetOrderAsync(getOrderParam).ConfigureAwait(false);

            if(order == null)
            {
                throw new InvalidOperationException($"Cannot cancel editing order #${orderNumber} as it doesn't exist.");
            }

            await EditingOrderProvider.CancelEditOrderAsync(order).ConfigureAwait(false);
        }

        private EditingOrderViewModel GetEditingOrderViewModel()
        {
            return new EditingOrderViewModel
            {
                CartUrl = CartUrlProvider.GetCartUrl(new BaseUrlParameter
                {
                    CultureInfo = ComposerContext.CultureInfo
                })
            };
        }

        public virtual async Task<CompleteCheckoutViewModel> SaveEditedOrderAsync(string orderNumber, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(orderNumber)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(orderNumber)));

            var getOrderParam = new GetCustomerOrderParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                OrderNumber = orderNumber,
                Scope = Constants.GlobalScopeName
            };

            var order = await OrderRepository.GetOrderAsync(getOrderParam).ConfigureAwait(false);

            if (order == null)
            {
                throw new InvalidOperationException($"Cannot save edited order #${orderNumber} as it doesn't exist.");
            }

            var removeInvalidLineItemsParam = new RemoveInvalidLineItemsParam
            {
                CartName = order.Id.Replace("-", "").ToLower(),
                CartType = CartConfiguration.OrderDraftCartType,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                BaseUrl = baseUrl
            };
            await CartService.RemoveInvalidLineItemsAsync(removeInvalidLineItemsParam).ConfigureAwait(false);

            var orderResult = await EditingOrderProvider.SaveEditedOrderAsync(order).ConfigureAwait(false);

            var resultViewModel = await CheckoutService.MapOrderToCompleteCheckoutViewModel(orderResult, new CompleteCheckoutParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = baseUrl
            });

            resultViewModel.NextStepUrl = CartUrlProvider.GetCheckoutConfirmationPageUrl(
                new BaseUrlParameter { CultureInfo = ComposerContext.CultureInfo });

            return resultViewModel;
        }

        public virtual async Task<OrderFulfillmentState> CancelOrder(CancelOrderParam param)
        {
            if(param == null) throw new ArgumentNullException(nameof(param));
            if (param.CultureInfo == null) throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)));
            if (string.IsNullOrWhiteSpace(param.OrderNumber)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.OrderNumber)));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)));

            var order = await OrderRepository.GetOrderAsync(new GetCustomerOrderParam
            {
                Scope = Constants.GlobalScopeName,
                OrderNumber = param.OrderNumber,
                CustomerId = param.CustomerId
            }).ConfigureAwait(false);
            
            if (order == null) throw new InvalidOperationException($"Order {param.OrderNumber} cannot be received.");

            await EditingOrderProvider.CancelOrder(order);

            var orderFulfillmentState = await OrderRepository.GetOrderFulfillmentStateAsync(new GetOrderFulfillmentStateParam
            {
                OrderId = order.Id,
                ScopeId = order.ScopeId
            }).ConfigureAwait(false);

            return orderFulfillmentState;
        }
    }
}
