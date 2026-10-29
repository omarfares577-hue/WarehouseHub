using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Api;
using WarehouseHub.Infrastructure.Data;
using WarehouseHub.Domain.Entities;
using Xunit;
using System.Net.Http.Json;
using System.Net;
namespace WarehouseHub.IntegrationTests
{
    public class SupplierIntegrationTests : IClassFixture<CustomWebApplicationFactory<ApiMarker>>
    {
        private readonly HttpClient _client;
        WebApplicationFactory<ApiMarker> _factory;
        public SupplierIntegrationTests(CustomWebApplicationFactory<ApiMarker> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            SeedDatabaseForTests(_factory);
        }
        private void SeedDatabaseForTests(WebApplicationFactory<ApiMarker> factory)
        {
            using var scope = factory.Services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<WarehouseContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Categories.Add(new Category { Id = 1, Name = "Electronics" });
            db.Suppliers.AddRange(
                new Supplier
                {
                    Id = 1,
                    Name = "Dell",
                },
                new Supplier
                {
                    Id = 2,
                    Name = "Samsung",

                },
                new Supplier
                {
                    Id = 3,
                    Name = "Sony",
                });
            db.WarehouseItems.Add(new WarehouseItem { Id = 2, Name = "Test Item", StockQuantity = 10, CategoryId = 1, Sku = "TST-02" });
            db.InventoryLogs.AddRange(
                new InventoryLog
                {
                    Id = 1,
                    OperationType = Operation.Out,
                    QuantityChanged = 2,
                    SupplierId = 1,
                    WarehouseItemId = 2
                    
                });
            db.SaveChanges();
        }
        [Fact]
        public async Task GetSuppliers()
        {
            var url = "/api/Supplier";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responseBody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
        }
        [Fact]
        public async Task GetSupplierById_ResturnsSuccess()
        {
            int id = 1;
            var url = $"/api/Supplier/{id}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responsebody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responsebody);
        }
        [Fact]
        public async Task GetSupplierById_ReturnsNotFound()
        {
            int id = 99;
            var url = $"/api/Supplier/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetSupplierById_ReturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/Supplier/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task DeleteSupplier_RturnsSuccess()
        {
            int id = 3;
            var url = $"/api/Supplier/{id}";
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task DeleteSupplier_RturnsBadRequest_WhileSupplierRelateTOAnyLog()
        {
            int id = 1;
            var url = $"/api/Supplier/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSupplier_ReturnsNotFound()
        {
            int id = 99;
            var url = $"/api/Supplier/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task DeleteSupplier_ReturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/Supplier/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task AddSupplier_ReturnsSuccess()
        {
            var Supplier = new Supplier { Name = "Hp" };
            var url = $"/api/Supplier";
            var response = await _client.PostAsJsonAsync(url, Supplier);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Supplier>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Hp", created.Name);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
            var dbSupplier = await db.Suppliers.FindAsync(created.Id);
            Assert.NotNull(dbSupplier);
            Assert.Equal("Hp", dbSupplier.Name);
        }
        [Fact]
        public async Task AddSupplier_ReturnsExistedname()
        {
            var Supplier = new Supplier { Name = "Dell" };
            var url = $"/api/Supplier";
            var response = await _client.PostAsJsonAsync(url, Supplier);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

    }
}
