using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Response
{
    public class OrderTransitionResponse
    {
        public Guid? OrderId { get; set; }
        public OrderState? NewOrderState { get; set; }

        public bool HasErrors { get; set; }
        public string? Error { get; set; }
        public int? ErrorCode { get; set; }


    }
}
