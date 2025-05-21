using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderingApp.WebApi.Application.Response;
using OrderingApp.WebApi.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
        }

        [ProducesResponseType<CreateOrderResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Get order analytics", "Get order analytics like average value, fulfillment time.")]
        [ResponseCache(Duration = 5 * 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = ["*"])] // response cache set to 5min
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var resp = await analyticsService.Get(cancellationToken);
            if (resp.HasErrors)
            {
                return BadRequest(new
                {
                    message = "One or more errors has occurred.",
                    resp.Error
                });
            }

            return Ok(resp);
        }
    }
}
