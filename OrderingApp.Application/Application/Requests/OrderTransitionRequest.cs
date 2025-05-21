using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Requests
{
    public class OrderTransitionRequest
    {
        public Guid? OrderId { get; set; }
        public OrderTransitionType? Transition { get; set; }
    }

    public enum OrderTransitionType
    {
        Confirm = 0,
        Cancel = 1
    }
}
