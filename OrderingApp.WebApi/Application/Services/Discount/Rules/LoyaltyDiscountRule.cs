using Microsoft.EntityFrameworkCore;
using OrderingApp.WebApi.Domain;
using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Services.Discount.Rules
{
    public class LoyaltyDiscountRule : IDiscountRule
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<MembershipDiscountRule> _logger;

        public LoyaltyDiscountRule(ApplicationDbContext dbContext,
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

            if (customer.DateJoined is null)
            {
                return false;
            }
            else
            {
                // give 1% discount for each year
                var yearsSinceJoined = (customer.DateJoined.Value - DateTime.UtcNow).Days / 365;
                order.Discount += order.Total * 0.01m * yearsSinceJoined;
            }
            return true;
        }

        public string GetDiscountRuleName()
        {
            return "Loyalty discount";
        }
    }
}
