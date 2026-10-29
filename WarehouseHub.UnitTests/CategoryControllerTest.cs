using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseHub.Core;
using WarehouseHub.Domain;
using WarehouseHub.Api;
using Moq;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Core.Mapping;
using WarehouseHub.Api.Controllers;
using AutoMapper;
using Xunit;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace WarehouseHub.UnitTests
{
    public class CategoryControllerTest
    {
        private readonly Mock<IUnitOfWork> _unitofwork;
        private readonly Mock<IMapper> _mapper;
        private readonly CategoryController _controller;
        public CategoryControllerTest()
        {
            _unitofwork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _controller = new CategoryController(_unitofwork.Object, _mapper.Object);
        }
        [Fact]
        public async Task GetAllCategories_OKResult()
        {
            var category = new List<Category> { new Category { Id = 1 } };
            var categorydto = new List<CategoryDto> { new CategoryDto { Id = 1 } };
            _unitofwork.Setup(c => c.Categories.GetAllAsync()).ReturnsAsync(category);
            _mapper.Setup(c => c.Map<IReadOnlyList<CategoryDto>>(category)).Returns(categorydto);
            var result = await _controller.GetAllCategories();
            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(OkResult.Value);
        }
        [Fact]
        public async Task GetCategoryById_OkResult()
        {
            var existingcategory = new Category { Id = 1 };
            var categorydto = new CategoryDto { Id = 1 };
            _unitofwork.Setup(c => c.Categories.GetById(categorydto.Id)).ReturnsAsync(existingcategory);
            _mapper.Setup(c => c.Map<CategoryDto>(categorydto));
            var result = await _controller.GetCategoryById(categorydto.Id);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(OkResult.Value);
        }
        [Fact]
        public async Task GetCategoryById_NotFoundResult()
        {
            _unitofwork.Setup(c => c.Categories.GetById(99)).ReturnsAsync((Category?)null);
            var result = await _controller.GetCategoryById(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(NotFoundResult.Value);
        }
        [Fact]
        public async Task GetCategoryById_BadRequestResult()
        {
            var result = await _controller.GetCategoryById(-1);
            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadRequestResult.Value);
        }
        [Fact]
        public async Task DeleteCategory_OkResult()
        {
            var existingcategory = new Category { Id = 1 };
            _unitofwork.Setup(c => c.Categories.GetById(existingcategory.Id)).ReturnsAsync(existingcategory);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var result = await _controller.DeleteCategory(existingcategory.Id);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(OkResult.Value);
            _unitofwork.Verify(s => s.CompleteAsync(), Times.Once);

        }
        [Fact]
        public async Task DeleteCategory_NotFoundResult()
        {
            _unitofwork.Setup(c => c.Categories.GetById(99)).ReturnsAsync((Category?)null);
            var result = await _controller.DeleteCategory(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(NotFoundResult.Value);
        }
        [Fact]
        public async Task DeleteCategory_BadRequestResult()
        {
            var result = await _controller.DeleteCategory(-1);
            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadRequestResult.Value);
        }
        [Fact]
        public async Task CreateCategoy_OkResult()
        {
            var category = new Category { Name = "PCs" };
            var categorydto = new CreateCategoryDto { Name = "PCs" };
            _unitofwork.Setup(c => c.Categories.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(false);
            _mapper.Setup(c => c.Map<Category>(categorydto)).Returns(category);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var result = await _controller.AddCategory(categorydto);
            var CreatedResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(CreatedResult.Value);
            _unitofwork.Verify(s => s.CompleteAsync(), Times.Once);

        }

        [Fact]
        public async Task CreateCategoy_ExistResult()
        {
            var categorydto = new CreateCategoryDto { Name = "PCs" };
            _unitofwork.Setup(c => c.Categories.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(true);
            var result = await _controller.AddCategory(categorydto);
            var BadResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadResult.Value);

        }

    }
}
