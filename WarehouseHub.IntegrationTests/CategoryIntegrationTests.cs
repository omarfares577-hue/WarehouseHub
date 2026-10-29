using Microsoft.AspNetCore.Mvc.Testing;
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
    public class CategoryIntegrationTests : IClassFixture<CustomWebApplicationFactory<ApiMarker>>
    {
        private readonly HttpClient _client;
        WebApplicationFactory<ApiMarker> _factory;
        public CategoryIntegrationTests(CustomWebApplicationFactory<ApiMarker> factory)
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

            db.Categories.AddRange(
                new Category
                {
                    Id = 1,
                    Name = "Electronics"
                },
                new Category
                {
                    Id = 2,
                    Name = "Pcs"
                }
                );


            db.SaveChanges();
        }
        [Fact]
        public async Task GetCategories()
        {
            string url = "/api/Category";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
        }
        [Fact]
        public async Task GetCategoryById_ResturnsSuccess()
        {
            int id = 1;
            var url = $"/api/Category/{id}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var responsebody = response.Content.ReadAsStringAsync();
            Assert.NotNull(responsebody);
        }
        [Fact]
        public async Task GetCategoryById_ReturnsNotFound()
        {
            int id = 99;
            var url = $"/api/Category/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetCategoryById_ReturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/Category/{id}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task DeleteCategory_RturnsSuccess()
        {
            int id = 2;
            var url = $"/api/Category/{id}";
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound()
        {
            int id = 99;
            var url = $"/api/Category/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task DeleteCategory_ReturnsBadRequest()
        {
            int id = -1;
            var url = $"/api/Category/{id}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task AddCategory_ReturnsSuccess()
        {
            var Category = new Category { Name = "Laptops" };
            var url = $"/api/Category";
            var response = await _client.PostAsJsonAsync(url, Category);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Category>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Laptops", created.Name);
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
            var dbCategory = await db.Categories.FindAsync(created.Id);
            Assert.NotNull(dbCategory);
            Assert.Equal("Laptops", dbCategory.Name);
        }
        [Fact]
        public async Task AddSupplier_ReturnsExistedname()
        {
            var Supplier = new Category { Name = "Pcs" };
            var url = $"/api/Category";
            var response = await _client.PostAsJsonAsync(url, Supplier);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

    }
}
