using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Api;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;
using Xunit;

namespace WarehouseHub.IntegrationTests
{
    public class WarehouseItemIntegrationTest : IClassFixture<CustomWebApplicationFactory<ApiMarker>>
    {
        private readonly HttpClient _client;
        WebApplicationFactory<ApiMarker> _factory;
        public WarehouseItemIntegrationTest(CustomWebApplicationFactory<ApiMarker> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            SeedDatabaseForTests(_factory);
        }
        private void SeedDatabaseForTests(
            WebApplicationFactory<ApiMarker> factory)
        {
            using var scope = factory.Services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<WarehouseContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Categories.Add(
                new Category
                {
                    Id = 1,
                    Name = "Electronics"
                });

            db.WarehouseItems.AddRange(
                new WarehouseItem
                {
                    Id = 1,
                    Name = "Laptop",
                    StockQuantity = 10,
                    CategoryId = 1,
                    Sku = "LAP-001"
                },
                new WarehouseItem
                {
                    Id = 2,
                    Name = "Mouse",
                    StockQuantity = 30,
                    CategoryId = 1,
                    Sku = "MOU-001"
                },
                new WarehouseItem
                {
                    Id = 3,
                    Name = "GamePad",
                    StockQuantity = 0,
                    CategoryId = 1,
                    Sku = "PAD-001"
                });

            db.SaveChanges();
        }
        [Fact]
        public async Task GetItems_ReturnsSuccess()
        {
            var url = "/api/Warehouse";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responseBody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody); 
        
        }
        [Fact]
        public async Task GetItemsById_ResturnsSuccess()
        {
            int id = 1;
            var url = $"/api/Warehouse/{id}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responseBody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);

        }
        [Fact]
        public async Task GetItemsById_ResturnsNotFound()
        {
            int id = 99;
            var url = $"/api/Warehouse/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
        [Fact]
        public async Task GetItemsById_ResturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/Warehouse/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        [Fact]
        public async Task DeleteItems_ResturnsSuccess()
        {
            int id = 3;
            var url = $"/api/Warehouse/{id}";
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            
        }
        [Fact]
        public async Task DeleteItems_ResturnsNotFound()
        {
            int id = 99;
            var url = $"/api/Warehouse/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
        [Fact]
        public async Task DeleteItems_ResturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/Warehouse/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        [Fact]
        public async Task DeleteItems_ResturnsStockIsNotEMpty()
        {
            int id = 1;
            var url = $"/api/Warehouse/{id} ";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        [Fact]
        public async Task AddItem_ReturnsSucess()
        {
            var newItem = new { Name = "Ps4", StockQuantity =10 , CategoryId = 1, Sku = "PS4-999", Price = 300 }; 
            var url = $"/api/Warehouse";
            var response = await _client.PostAsJsonAsync(url, newItem);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<WarehouseItem>();
            Assert.NotNull(created);
            Assert.Equal("Ps4", created.Name);
            Assert.True(created.Id > 0);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
            var dbitem = await db.WarehouseItems.FindAsync(created.Id);
            Assert.NotNull(dbitem);
            Assert.Equal("Ps4", dbitem.Name);
        }
        [Fact]
        public async Task AddItem_ReturnsExistedName()
        {
            var newitem = new WarehouseItem { Name = "GamePad", StockQuantity = 10 };
            var url = $"/api/Warehouse";
            var response = await _client.PostAsJsonAsync(url, newitem);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}

