using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Response
{
    public class GetOrderResponse
    {
        public Guid? Id { get; set; }
        public DateTime? DatePlaced { get; set; }
        public string? CustomerName { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Total { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TotalAfterDiscount { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public DateTime? DateCancelled { get; set; }
        public string? State { get; set; }
        public ICollection<GetOrderLineResponse> OrderLines { get; set; }

        public bool HasErrors { get; set; }
        public string? Error { get; set; }
        public int? ErrorCode { get; set; }

    }

    public class GetOrderLineResponse
    {
        public string Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

    }
}