namespace OrderingApp.WebApi.Application.Response
{
    public class CreateOrderResponse
    {
        public Guid? Id { get; set; }
        public bool HasError { get; set; } = false;
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
