namespace OrderingApp.WebApi.Domain.Models
{
    public class Order : BaseEntity<Guid>
    {
        public DateTime DatePlaced { get; set; }
        public Customer Customer { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public DateTime? DateCancelled { get; set; }
        public OrderState State { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
    }
}
