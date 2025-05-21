using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Requests
{
    public class CreateOrderRequest
    {
        public int? CustomerId { get; set; }
        public IEnumerable<CreateOrderLineDTO> OrderLines { get; set; } = [];

    }

    public class CreateOrderLineDTO
    {
        public string Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
