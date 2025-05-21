using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;

namespace OrderingApp.WebApi.Application.Services
{
    public interface IAnalyticsService
    {
        Task<GetAnalyticsResponse> Get(CancellationToken cancellationToken);

    }
}