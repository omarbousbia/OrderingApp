using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Application.Services.Discount
{
    public interface IDiscountService
    {
        ApplyDiscountResult ApplyDiscount(Order order, CancellationToken cancellationToken);

    }
}