
using Microsoft.EntityFrameworkCore;
using OrderingApp.WebApi.Application.Services;
using OrderingApp.WebApi.Application.Services.Discount;
using OrderingApp.WebApi.Application.Services.Discount.Rules;
using OrderingApp.WebApi.Domain;

namespace OrderingApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("sqlserver") ?? throw new ArgumentNullException("Cannot find connection string");
            builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString));

            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

            builder.Services.AddScoped<IDiscountRule, LoyaltyDiscountRule>();
            builder.Services.AddScoped<IDiscountRule, MembershipDiscountRule>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o => o.EnableAnnotations());

            builder.Services.AddResponseCaching();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseResponseCaching();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
