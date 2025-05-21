using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;

namespace OrderingApp.WebApi.Application.Services
{
    public interface IOrderService
    {
        Task<GetOrderResponse> GetOrderDTOAsync(Guid id, CancellationToken cancellationToken);
        Task<CreateOrderResponse> PlaceOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
        Task<OrderTransitionResponse> OrderTransition(OrderTransitionRequest request, CancellationToken cancellationToken);

    }
}