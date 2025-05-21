using Microsoft.EntityFrameworkCore;
using OrderingApp.WebApi.Domain;
using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Services.Discount.Rules
{
    public class MembershipDiscountRule : IDiscountRule
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<MembershipDiscountRule> _logger;

        public MembershipDiscountRule(ApplicationDbContext dbContext,
                                      ILogger<MembershipDiscountRule> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }
        public bool ApplyDiscount(Order order, CancellationToken cancellationToken)
        {
            if (order.CustomerId is null)
            {
                return false;
            }

            var customer = _dbContext.Customers.AsNoTracking().FirstOrDefault(c => c.Id == order.CustomerId);
            if (customer == null)
            {
                _logger.LogError("Customer with id: {0}, was not found.", order.CustomerId);
                return false;
            }

            switch (customer.Membership)
            {
                case CustomerMembership.None:
                    break;
                case CustomerMembership.Bronze:
                    order.Discount += order.Total * 0.02m; // 2% discount
                    break;
                case CustomerMembership.Silver:
                    order.Discount += order.Total * 0.04m; // 4% discount
                    break;
                case CustomerMembership.Gold:
                    order.Discount += order.Total * 0.07m; // 7% discount
                    break;
                default:
                    break;
            }
            return true;
        }

        public string GetDiscountRuleName()
        {
            return "Membership discount";
        }
    }
}
