using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;
using OrderingApp.WebApi.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderingApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService,
                               ILogger<OrderController> logger)
        {
            this._orderService = orderService;
            _logger = logger;
        }

        [ProducesResponseType<CreateOrderResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("create a new order", "more details here.")]
        [HttpPost]
        public async Task<IActionResult> Post(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.PlaceOrderAsync(request, cancellationToken);
            if (result.HasError)
            {
                return BadRequest(new
                {
                    message = "One or more errors has occurred.",
                    result.Errors
                });
            }
            return Ok(result);
        }


        [ProducesResponseType<CreateOrderResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Retrieve order", "Get order details by id.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderDTOAsync(id, cancellationToken);
            if (result.HasErrors)
            {
                return StatusCode(result.ErrorCode ?? StatusCodes.Status400BadRequest, new { result.Error });
            }

            return Ok(result);
        }


        [SwaggerOperation("Update order state", "You can only confirm, or cancel the order.")]
        [ProducesResponseType<OrderTransitionResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut]
        public async Task<IActionResult> Transition(OrderTransitionRequest request, CancellationToken cancellationToken)
        {
            OrderTransitionResponse result = await _orderService.OrderTransition(request, cancellationToken);
            if (result.HasErrors)
            {
                return StatusCode(result.ErrorCode ?? StatusCodes.Status400BadRequest, new { result.Error });
            }

            return Ok(result);
        }
    }
}
