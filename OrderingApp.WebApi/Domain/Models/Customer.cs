namespace OrderingApp.WebApi.Domain.Models
{
    public class Customer : BaseEntity<int>
    {
        public string Name { get; set; }
        public DateTime? DateJoined { get; set; }
        public CustomerMembership Membership { get; set; }
    }

    public enum CustomerMembership
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2
    }
}