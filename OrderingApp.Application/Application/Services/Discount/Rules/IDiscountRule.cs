using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Services.Discount.Rules
{
    public interface IDiscountRule
    {
        string GetDiscountRuleName();
        bool ApplyDiscount(Order order, CancellationToken cancellationToken);
    }
}
