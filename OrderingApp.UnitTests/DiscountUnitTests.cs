using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderingApp.WebApi.Application.Services.Discount;
using OrderingApp.WebApi.Application.Services.Discount.Rules;
using OrderingApp.WebApi.Domain;
using OrderingApp.WebApi.Domain.Models;
using System;

namespace OrderingApp.UnitTests
{
    public static class TestLogger
    {
        public static ILogger<T> Create<T>() => Mock.Of<ILogger<T>>();
    }

    public class DiscountUnitTests
    {
        [Fact]
        public void MembershipRule_apply_for_gold_customer()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            using var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            // Arrange
            var mockLogger = new Mock<ILogger<DiscountService>>();
            var memberShipRule = new MembershipDiscountRule(dbContext, TestLogger.Create<MembershipDiscountRule>());
            var discountService = new DiscountService([memberShipRule], mockLogger.Object);

            var order = new Order
            {
                CustomerId = 2, // customer '2' has Gold membership
                OrderLines = [new OrderLine { Product = "eggs", Quantity = 2, UnitPrice = 10 },
                            new OrderLine { Product = "milk", Quantity = 1, UnitPrice = 30 },],
                Total = 2 * 10 + 30
            };

            // act
            var dresult = discountService.ApplyDiscount(order, default); // apply 7% discount

            // assert
            const int totalBeforeDiscount = (2 * 10 + 30);
            var totalAfterDiscount = totalBeforeDiscount * 0.93m;

            //Assert.Equal(totalAfterDiscount, order.TotalAfterDiscount);
            Assert.Equal(totalBeforeDiscount * 0.07m, order.Discount);
        }

        [Fact]
        public void Multiple_Rules_apply_for_gold_customer()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            using var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            // Arrange
            var mockLogger = new Mock<ILogger<DiscountService>>();
            var memberShipRule = new MembershipDiscountRule(dbContext, TestLogger.Create<MembershipDiscountRule>());
            var loyaltyRule = new LoyaltyDiscountRule(dbContext, TestLogger.Create<LoyaltyDiscountRule>());
            var discountService = new DiscountService([memberShipRule, loyaltyRule], mockLogger.Object);

            var order = new Order
            {
                CustomerId = 3, // customer '3' has Silver membership
                OrderLines = [new OrderLine { Product = "eggs", Quantity = 2, UnitPrice = 10 },
                            new OrderLine { Product = "milk", Quantity = 1, UnitPrice = 30 },],
                Total = 2 * 10 + 30
            };

            // act
            // customer has silver membership, he gets 4% off + 2% for loyalty (1% for each year since joining) => total is 6%.
            var dresult = discountService.ApplyDiscount(order, default); // apply 7% discount

            // assert
            const int totalBeforeDiscount = (2 * 10 + 30);
            var totalAfterDiscount = totalBeforeDiscount * 0.94m;

            //Assert.Equal(totalAfterDiscount, order.TotalAfterDiscount);
            Assert.Equal(totalBeforeDiscount * 0.06m, order.Discount);
        }


        [Fact]
        public void MembershipRule_dont_apply_for_customer_with_no_membership()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            using var dbContext = new ApplicationDbContext(options);

            // Arrange
            var mockLogger = new Mock<ILogger<DiscountService>>();
            var memberShipRule = new MembershipDiscountRule(dbContext, TestLogger.Create<MembershipDiscountRule>());
            var discountService = new DiscountService([memberShipRule], mockLogger.Object);

            var order = new Order
            {
                CustomerId = 1, // customer '1' has no membership
                OrderLines = [new OrderLine { Product = "eggs", Quantity = 2, UnitPrice = 10 },
                            new OrderLine { Product = "milk", Quantity = 1, UnitPrice = 30 },],
                Total = 2 * 10 + 30
            };

            // act
            var dresult = discountService.ApplyDiscount(order, default);

            // assert
            var expetedTotal = (2 * 10 + 30);
            Assert.Equal(expetedTotal, order.Total);
            Assert.Equal(0m, order.Discount);
        }
    }
}