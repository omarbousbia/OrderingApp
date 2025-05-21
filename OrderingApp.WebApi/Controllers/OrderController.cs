using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderingApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        //[ProducesResponseType("application/json")]
        [SwaggerOperation("create a new order", "more details here.")]
        [HttpPost]
        public IActionResult Post(/*CreateOrderRequest*/)
        {
            return Ok();
        }

        
        [SwaggerOperation("Retrieve order", "Get order details by id.")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(); 
        }

        [SwaggerOperation("Update order state", "You can go forward, or cancel the order.")]
        [HttpPut("{id}")]
        public IActionResult Transition(/*OrderTransitionRequest*/)
        {
            return Ok();
        }
    }
}
