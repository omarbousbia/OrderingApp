using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;
using OrderingApp.WebApi.Application.Services.Discount;
using OrderingApp.WebApi.Domain;
using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDiscountService discountService;
        private readonly ILogger<OrderService> logger;

        public OrderService(ApplicationDbContext applicationDbContext,
                            IDiscountService discountService,
                            ILogger<OrderService> logger)
        {
            this._dbContext = applicationDbContext;
            this.discountService = discountService;
            this.logger = logger;
        }

        public async Task<GetOrderResponse> GetOrderDTOAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = new GetOrderResponse();
            var order = await _dbContext.Orders.AsNoTracking()
                                               .Include(o => o.Customer)
                                               .Include(o => o.OrderLines)
                                               .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (order is null)
            {
                response.HasErrors = true;
                response.Error = "Order was not found";
                response.ErrorCode = 404;
                return response;
            }

            response.Id = order.Id;
            response.DatePlaced = order.DatePlaced;
            response.DateConfirmed = order.DateConfirmed;
            response.CustomerName = order.Customer.Name;
            response.CustomerId = order.CustomerId;
            response.Discount = order.Discount;
            response.TotalAfterDiscount = order.TotalAfterDiscount;
            response.Total = order.Total;
            response.State = order.State.ToString();
            response.OrderLines = order.OrderLines.Select(o => new GetOrderLineResponse
            {
                Product = o.Product,
                Quantity = o.Quantity,
                UnitPrice = o.UnitPrice
            }).ToArray();

            return response;
        }

        public async Task<CreateOrderResponse> PlaceOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var response = new CreateOrderResponse();

            var validationResult = ValidateRequest(request);
            if (validationResult.Any())
            {
                response.HasError = true;
                response.Errors = validationResult;
                return response;
            }

            var order = CreateOrderFromRequest(request);
            var discountResult = discountService.ApplyDiscount(order, cancellationToken); // discount result can be traced/logged.
            order.TotalAfterDiscount = decimal.Round(order.Total - order.Discount, 2); // rounding can be configured

            //save order
            _dbContext.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Id = order.Id;
            return response;

        }

        public async Task<OrderTransitionResponse> OrderTransition(OrderTransitionRequest request, CancellationToken cancellationToken)
        {
            var response = new OrderTransitionResponse();
            if (request.OrderId is null)
            {
                response.HasErrors = true;
                response.Error = "Order id must be provided";
                response.ErrorCode = 400;
                return response;
            }

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
            if (order is null)
            {
                response.HasErrors = true;
                response.Error = "Order was not found";
                response.ErrorCode = 404;
                return response;
            }

            response.OrderId = request.OrderId;

            switch (order.State)
            {
                case OrderState.Draft:
                    if (request.Transition == OrderTransitionType.Confirm)
                    {
                        order.State = OrderState.Delivered;
                        order.DateConfirmed = DateTime.UtcNow;
                        await _dbContext.SaveChangesAsync();

                        response.NewOrderState = order.State;
                        return response;
                    }
                    else if (request.Transition == OrderTransitionType.Cancel)
                    {
                        order.State = OrderState.Cancelled;
                        order.DateCancelled = DateTime.UtcNow;
                        await _dbContext.SaveChangesAsync();

                        response.NewOrderState = order.State;
                        return response;

                    }
                    break;
                case OrderState.Delivered:
                    response.HasErrors = true;
                    response.Error = "Order cannot be modified once it has been delivered.";
                    response.ErrorCode = 400;
                    return response;
                case OrderState.Cancelled:
                    response.HasErrors = true;
                    response.Error = "Order cannot be modified once it has been cancelled.";
                    response.ErrorCode = 400;
                    return response;
                default:
                    response.Error = "Case not implemented.";
                    response.ErrorCode = 501;
                    break;
            }

            return response;
        }

        private Order CreateOrderFromRequest(CreateOrderRequest request)
        {
            var order = new Order()
            {
                CustomerId = request.CustomerId,
                OrderLines = request.OrderLines.Select(CreateOrderLine).ToList(),
            };
            decimal total = order.OrderLines.Sum(x => x.Quantity * x.UnitPrice);
            order.Total = decimal.Round(total, 2);
            order.DatePlaced = DateTime.UtcNow;
            return order;
        }

        private OrderLine CreateOrderLine(CreateOrderLineDTO oldto)
        {
            var ol = new OrderLine
            {
                Product = oldto.Product,
                Quantity = oldto.Quantity,
                UnitPrice = oldto.UnitPrice
            };
            return ol;
        }

        private IEnumerable<string> ValidateRequest(CreateOrderRequest request)
        {
            var errors = new List<string>(0);
            if (!request.OrderLines.Any())
            {
                errors.Add("order is empty");
            }
            if (request.OrderLines.Any(x => x.Quantity == 0m || string.IsNullOrWhiteSpace(x.Product)))
            {
                errors.Add("Make sure to provide Quantity and product name for each order line.");
            }
            return errors;
        }
    }
}
