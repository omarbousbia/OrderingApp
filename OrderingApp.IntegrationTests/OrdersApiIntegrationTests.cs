using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderingApp.WebApi;
using OrderingApp.WebApi.Application.Requests;
using OrderingApp.WebApi.Application.Response;
using OrderingApp.WebApi.Domain;
using System;
using System.Net.Http.Json;
using Testcontainers.MsSql;
using Xunit;

public class OrdersApiIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("YourStrong@Passw0rd")
        .WithPortBinding(1433, assignRandomHostPort: true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .Build();

    private HttpClient _client;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Override DB with TestContainer connection string
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(_dbContainer.GetConnectionString()));
                });
            });

        _client = factory.CreateClient();

        // ensure DB is created and seeded
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task PlaceOrder_ReturnsOK()
    {
        // Act
        CreateOrderRequest request = new() { CustomerId = 1, OrderLines = [new CreateOrderLineDTO { Product = "chair", Quantity = 2, UnitPrice = 55m }] };
        var response = await _client.PostAsJsonAsync("api/order", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var createOrderResponse = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        Assert.NotNull(createOrderResponse.Id);
    }

    [Fact]
    public async Task PlaceOrder_qty_error_Returns_badRequest()
    {
        // Act
        CreateOrderRequest request = new() { CustomerId = 1, OrderLines = [new CreateOrderLineDTO { Product = "chair", Quantity = 0, UnitPrice = 55m }] };
        var response = await _client.PostAsJsonAsync("api/order", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async Task DisposeAsync() => await _dbContainer.DisposeAsync();
}