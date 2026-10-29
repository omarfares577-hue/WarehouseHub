using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class InventoryLogsIntegrationTests : IClassFixture<CustomWebApplicationFactory<ApiMarker>>
    {
        private readonly HttpClient _client;
        WebApplicationFactory<ApiMarker> _factory;
        public InventoryLogsIntegrationTests(CustomWebApplicationFactory<ApiMarker> factory)
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
            db.Suppliers.AddRange(
                new Supplier
                {
                    Id = 1,
                    Name = "Dell",
               
                });
            db.InventoryLogs.AddRange(
                new InventoryLog
                {
                    Id = 1,
                    OperationType = Operation.Out,
                    QuantityChanged = 2,
                    SupplierId = 1,
                    WarehouseItemId = 1

                },
                new InventoryLog
                {
                    Id = 2,
                    OperationType = Operation.Out,
                    QuantityChanged = 2,
                    SupplierId = 1,
                    WarehouseItemId = 1

                });
            db.WarehouseItems.Add(new WarehouseItem { Id = 1, Name = "Test Item", StockQuantity = 10, CategoryId = 1, Sku = "TST-02" });
            db.Categories.Add(new Category { Id = 1, Name = "Electronics" });

            db.SaveChanges();
        }
        [Fact]
        public async Task GetInventoryLog()
        {
            var url = "/api/InventoryLog";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responseBody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
        }
        [Fact]
        public async Task GetInventoryLogById_ResturnsSuccess()
        {
            int id = 1;
            var url = $"/api/InventoryLog/{id}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responsebody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responsebody);
        }
        [Fact]
        public async Task GetInventoryLogById_ReturnsNotFound()
        {
            int id = 99;
            var url = $"/api/InventoryLog/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetInventoryLogById_ReturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/InventoryLog/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetArchivedInventoryLog()
        {
            var url = "/api/InventoryLog/archive";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responseBody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
        }
        [Fact]
        public async Task AddInventoryLog_ReturnsSuccess()
        {
            var InventoryLog = new 
            {
                OperationType ="out",
                QuantityChanged = 2,
                SupplierId = 1,
                WarehouseItemId = 1
            };
            var url = $"/api/InventoryLog";
            var response = await _client.PostAsJsonAsync(url, InventoryLog);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<InventoryLog>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(1, created.SupplierId);
            Assert.Equal(1, created.WarehouseItemId);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
            var dbInventoryLog = await db.InventoryLogs.FindAsync(created.Id);
            Assert.NotNull(dbInventoryLog);
            Assert.Equal(1, dbInventoryLog.SupplierId);
            Assert.Equal(1, dbInventoryLog.WarehouseItemId);
        }

        [Fact]
        public async Task AddInventoryLog_ReturnsErrorr_Rollback()
        {
            var InventoryLog = new 
            {
                OperationType = "out",
                QuantityChanged = 2,
                SupplierId = 99,
                WarehouseItemId = 1
            };
            var url = $"/api/InventoryLog";
            var response = await _client.PostAsJsonAsync(url, InventoryLog);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<WarehouseContext>();

                var itemInDb = await db.WarehouseItems.FindAsync(1);
                Assert.NotNull(itemInDb);
                Assert.Equal(10, itemInDb.StockQuantity);
                var logExists = await db.InventoryLogs.AnyAsync(l => l.WarehouseItemId == 1 && l.SupplierId == 99);
                Assert.False(logExists, "Transaction Rollback failed: The log was saved despite the error!");
            }
        }


    }
}
