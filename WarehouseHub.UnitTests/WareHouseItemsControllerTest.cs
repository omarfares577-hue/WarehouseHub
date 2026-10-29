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
using WarehouseHub.Core.Specification;
using WarehouseHub.Core.Specification.SpecParams;


namespace WarehouseHub.UnitTests
{
    public class WareHouseItemsControllerTest
    {
        private readonly Mock<IUnitOfWork> _unitofwork;
        private readonly Mock<IMapper> _mapper;
        private readonly WarehouseController _controller;
        public WareHouseItemsControllerTest()
        {
            _unitofwork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _controller = new WarehouseController(_unitofwork.Object, _mapper.Object);
        }
        [Fact]
        public async Task GetAllItems()
        {
            var specparam = new WarehouseitemSpecparam { PageSize = 6 };
            var item = new List<WarehouseItem> { new WarehouseItem { Id = 1, StockQuantity = 10 } };
            var itemdto = new List<WarehouseItemDto> { new WarehouseItemDto { Id = 1, StockQuantity = 10 } };
            _unitofwork.Setup(i => i.WarehouseItems.GetAllWithSpecAsync(It.IsAny<ISpecification<WarehouseItem>>())).ReturnsAsync(item);
            _mapper.Setup(i => i.Map<IReadOnlyList<WarehouseItemDto>>(item)).Returns(itemdto);
            var result = await _controller.GetAll(specparam);
            var OkResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(OkResult);
        }

        [Fact]
        public async Task GetItemById_ReturnsOk()
        {
            var item = new WarehouseItem { Id = 1, StockQuantity = 10 };
            var itemdto = new WarehouseItemDto { Id = 1, StockQuantity = 10 };
            _unitofwork.Setup(i => i.WarehouseItems.GetWithSpecById(It.IsAny<ISpecification<WarehouseItem>>())).ReturnsAsync(item);
            _mapper.Setup(i => i.Map<WarehouseItemDto>(item)).Returns(itemdto);
            var result = await _controller.GetById(item.Id);
            var OkResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(OkResult);
        }
        [Fact]
        public async Task GetItemById_ReturnsNotFound()
        {
            _unitofwork.Setup(i => i.WarehouseItems.GetWithSpecById(It.IsAny<ISpecification<WarehouseItem>>())).ReturnsAsync((WarehouseItem?)null);
            var result = await _controller.GetById(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(NotFoundResult);
        }
        [Fact]
        public async Task GetItemById_ReturnsBadRequest()
        {
            _unitofwork.Setup(i => i.WarehouseItems.GetWithSpecById(It.IsAny<ISpecification<WarehouseItem>>())).ReturnsAsync((WarehouseItem?)null);
            var result = await _controller.GetById(-1);
            var BadeRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(BadeRequestResult);
        }
        [Fact]
        public async Task DeleteItem_ReturnsOk()
        {
            var item = new WarehouseItem { Id = 1, StockQuantity = 0 };
            _unitofwork.Setup(i => i.WarehouseItems.GetById(item.Id)).ReturnsAsync(item);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            var result = await _controller.DeleteItem(item.Id);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(OkResult);

        }
        [Fact]
        public async Task DeleteItem_ReturnsNotFound()
        {
            _unitofwork.Setup(i => i.WarehouseItems.GetById(99)).ReturnsAsync((WarehouseItem?)null);
            var result = await _controller.DeleteItem(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(NotFoundResult);
        }
        [Fact]
        public async Task DeleteItem_ReturnsBadRequest()
        {
            _unitofwork.Setup(i => i.WarehouseItems.GetById(-1)).ReturnsAsync((WarehouseItem?)null);
            var result = await _controller.DeleteItem(-1);
            var BadeRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadeRequestResult);
        }
        [Fact]
        public async Task DeleteItem_StockIsnotEmpty_ReturnsBadRequest()
        {
            var item = new WarehouseItem { Id = 1, StockQuantity = 10 };

            _unitofwork.Setup(i => i.WarehouseItems.GetById(1)).ReturnsAsync(item);
            var result = await _controller.DeleteItem(1);
            var BadeRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadeRequestResult);
            _unitofwork.Verify(c => c.WarehouseItems.Delete(item), Times.Never);
        }
        [Fact]
        public async Task CreateItem_OkResult()
        {
            var item = new WarehouseItem{ Name = "PCs" };
            var itemdto = new CreateWarehouseItemDto { Name = "PC4" };
            _unitofwork.Setup(c => c.WarehouseItems.AnyAsync(It.IsAny<Expression<Func<WarehouseItem, bool>>>())).ReturnsAsync(false);
            _mapper.Setup(c => c.Map<WarehouseItem>(itemdto)).Returns(item);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var result = await _controller.AddItem(itemdto);
            var CreateResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(CreateResult.Value);
            _unitofwork.Verify(s => s.CompleteAsync(), Times.Once);

        }

        [Fact]
        public async Task CreateItem_ExistResult()
        {
            var item = new CreateWarehouseItemDto { Name = "PCs" };
            _unitofwork.Setup(c => c.WarehouseItems.AnyAsync(It.IsAny<Expression<Func<WarehouseItem, bool>>>())).ReturnsAsync(true);
            var result = await _controller.AddItem(item);
            var BadResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadResult.Value);

        }

    }

}