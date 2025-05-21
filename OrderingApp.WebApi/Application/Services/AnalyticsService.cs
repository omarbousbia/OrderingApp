using Microsoft.EntityFrameworkCore;
using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;
using OrderingApp.WebApi.Domain;
using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AnalyticsService> logger;

        public AnalyticsService(ApplicationDbContext dbContext,
                                ILogger<AnalyticsService> logger)
        {
            this._dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<GetAnalyticsResponse> Get(CancellationToken cancellationToken)
        {
            var response = new GetAnalyticsResponse();

            var values = await _dbContext.Orders.Where(o => o.State == OrderState.Delivered && o.DateConfirmed.HasValue)
                                            .Select(o => o.DateConfirmed - o.DatePlaced).ToListAsync();

            response.AvgFulfillmentTime = (long)values.Select(v => v.Value.Seconds).Average();


            response.AvgOrderAmount = await _dbContext.Orders.Where(o => o.State != OrderState.Cancelled)
                                                            .AverageAsync(o => o.TotalAfterDiscount);

            return response;
        }
    }
}