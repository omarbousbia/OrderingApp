using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Response
{
    public class GetAnalyticsResponse
    {
        public long AvgFulfillmentTime { get; set; }
        public decimal AvgOrderAmount { get; set; }

        public bool HasErrors { get; set; }
        public string? Error { get; set; }
        public int? ErrorCode { get; set; }

    }
}
