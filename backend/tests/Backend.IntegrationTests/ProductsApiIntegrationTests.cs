using System.Net;
using System.Net.Http.Json;
using Asisya.Application.DTOs;
using Asisya.Domain.Entities;
using Asisya.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Backend.IntegrationTests;

public class ProductsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Reemplazar la configuración de DbContext por una base de datos In-Memory para pruebas de integración aisladas
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbName = Guid.NewGuid().ToString();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                // Inicializar datos de prueba en la BD en memoria
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();

                var category = new Category
                {
                    CategoryId = 1,
                    CategoryName = "SERVIDORES",
                    Description = "Servidores de prueba"
                };
                db.Categories.Add(category);

                db.Products.Add(new Product
                {
                    ProductId = 1,
                    ProductName = "Integration Test Product",
                    CategoryId = 1,
                    UnitPrice = 1500m,
                    UnitsInStock = 20,
                    Discontinued = false
                });

                db.SaveChanges();
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessAndPagedResult()
    {
        // Act: Realizar petición HTTP GET a la API REST real
        var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=10");

        // Assert: Validar código HTTP 200 OK y estructura de respuesta JSON
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.First().ProductName.Should().Be("Integration Test Product");
        result.Items.First().CategoryName.Should().Be("SERVIDORES");
    }
}
