using Microsoft.Extensions.Logging;
using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;
using OrderingApp.WebApi.Application.Services.Discount.Rules;
using OrderingApp.WebApi.Domain;
using OrderingApp.WebApi.Domain.Models;
using System.Threading;

namespace OrderingApp.WebApi.Application.Services.Discount
{
    public class DiscountService : IDiscountService
    {
        private readonly IEnumerable<IDiscountRule> _discountRules;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(IEnumerable<IDiscountRule> discountRules,
                               ILogger<DiscountService> logger)
        {
            this._discountRules = discountRules;
            this._logger = logger;
        }

        public ApplyDiscountResult ApplyDiscount(Order order, CancellationToken cancellationToken)
        {
            var result = new ApplyDiscountResult();
            if (!_discountRules.Any())
            {
                _logger.LogInformation("Discount cannot be applied since there no discount rules provided.");
                return result;
            }

            //if (!order?.OrderLines?.Any() is true)
            //{
            //    _logger.LogWarning("Order has no order lines, discount cannot be applied.");
            //    return result;
            //}

            foreach (var rule in _discountRules)
            {
                if (rule.ApplyDiscount(order, cancellationToken))
                {
                    result.AppliedRules.Add(rule.GetDiscountRuleName());
                }
            }
            
            return result;
        }

    }

    public class ApplyDiscountResult
    {
        public List<string> AppliedRules { get; set; } = [];
    }
}
