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
    public class InventoryLogControllerTest
    {
        private readonly Mock<IUnitOfWork> _unitofwork;
        private readonly Mock<IMapper> _mapper;
        private readonly InventorylogController _controller;
        public InventoryLogControllerTest()
        {
            _unitofwork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _controller = new InventorylogController(_unitofwork.Object, _mapper.Object);
        }
        [Fact]
        public async Task GetAlllogs()
        {
            var specparam = new InventorylogSpecparam { PageSize = 6 };
            var log = new List<InventoryLog> { new InventoryLog { Id = 1,QuantityChanged = 2, OperationType = Operation.In } };
            var logdto = new List<InventoryLogDto> { new InventoryLogDto { Id = 1, QuantityChanged = 2, OperationType = "in" } };
            _unitofwork.Setup(i => i.InventoryLogs.GetAllWithSpecAsync(It.IsAny<ISpecification<InventoryLog>>())).ReturnsAsync(log);
            _mapper.Setup(i => i.Map<IReadOnlyList<InventoryLogDto>>(log)).Returns(logdto);
            var result = await _controller.GetAll(specparam);
            var OkResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(OkResult);
        }
        [Fact]
        public async Task GetItemById_ReturnsOk()
        {
            var log =  new InventoryLog { Id = 1, QuantityChanged = 2, OperationType = Operation.In } ;
            var logdto =  new InventoryLogDto { Id = 1, QuantityChanged = 2, OperationType = "in" };
            _unitofwork.Setup(i => i.InventoryLogs.GetWithSpecById(It.IsAny<ISpecification<InventoryLog>>())).ReturnsAsync(log);
            _mapper.Setup(i => i.Map<InventoryLogDto>(log)).Returns(logdto);
            var result = await _controller.GetLogById(log.Id);
            var OkResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(OkResult);
        }
        [Fact]
        public async Task GetlogById_ReturnsNotFound()
        {
            _unitofwork.Setup(i => i.InventoryLogs.GetWithSpecById(It.IsAny<ISpecification<InventoryLog>>())).ReturnsAsync((InventoryLog?)null);
            var result = await _controller.GetLogById(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(NotFoundResult);
        }
        [Fact]
        public async Task GetlogById_ReturnsBadRequest()
        {
            _unitofwork.Setup(i => i.InventoryLogs.GetWithSpecById(It.IsAny<ISpecification<InventoryLog>>())).ReturnsAsync((InventoryLog?)null);
            var result = await _controller.GetLogById(-1);
            var BadeRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(BadeRequestResult);
        }
        [Fact]
        public async Task CreateLog_ReturnOkOutWIthAlert()
        {
            var dto = new CreateLogsDto { WarehouseItemId = 1, OperationType = "out", QuantityChanged = 5 };
            var item = new WarehouseItem { Id = 1, StockQuantity = 10, MinStockLevel = 8 };
            var log = new InventoryLog();
            _unitofwork.Setup(i => i.WarehouseItems.GetById(dto.WarehouseItemId)).ReturnsAsync(item);
            _mapper.Setup(m => m.Map<InventoryLog>(dto)).Returns(log);

            _unitofwork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitofwork.Setup(u => u.InventoryLogs.AddAsync(log)).Returns(Task.CompletedTask);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            _unitofwork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            var result = await _controller.CreateLog(dto);
            var CreatedResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(CreatedResult.Value);
        }
        [Fact]
        public async Task CreateLog_ReturnsOk_WhenInOperationSuccess()
        {
            var dto = new CreateLogsDto { WarehouseItemId = 1, OperationType = "IN", QuantityChanged = 20 };
            var item = new WarehouseItem { Id = 1, StockQuantity = 10, MinStockLevel = 5 };
            var log = new InventoryLog();

            _unitofwork.Setup(u => u.WarehouseItems.GetById(dto.WarehouseItemId)).ReturnsAsync(item);
            _mapper.Setup(m => m.Map<InventoryLog>(dto)).Returns(log);

            _unitofwork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitofwork.Setup(u => u.InventoryLogs.AddAsync(log)).Returns(Task.CompletedTask);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            _unitofwork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            var result = await _controller.CreateLog(dto);
            var CreatedResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(CreatedResult.Value); 
            Assert.Equal(30, item.StockQuantity);
        }
            [Fact]
        public async Task CreateLog_ReturnsBadRequest_WhenWarehouseItemIdIsInvalid()
        {
            var dto = new CreateLogsDto { WarehouseItemId = 0, OperationType = "In" };
            var result = await _controller.CreateLog(dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }
        [Fact]
        public async Task CreateLog_ReturnsNotFound_WhenItemDoesNotExist()
        {
            var dto = new CreateLogsDto { WarehouseItemId = 99, OperationType = "In" };
            _unitofwork.Setup(u => u.WarehouseItems.GetById(dto.WarehouseItemId)).ReturnsAsync((WarehouseItem?)null);
            var result = await _controller.CreateLog(dto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }
        [Fact]
        public async Task CreateLog_ReturnsBadRequest_WhenStockNotHaveEnoughItemsForOut()
        {
            var dto = new CreateLogsDto { WarehouseItemId = 1, OperationType = "Out", QuantityChanged = 10 };
            var item = new WarehouseItem { Id = 1, StockQuantity = 5 };
            _unitofwork.Setup(u => u.WarehouseItems.GetById(dto.WarehouseItemId)).ReturnsAsync(item);
            var result = await _controller.CreateLog(dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }
        [Fact]
        public async Task CreateLog_ReturnsStatusCode500_AndRollsBack()
        {
            var dto = new CreateLogsDto { WarehouseItemId = 1, OperationType = "In", QuantityChanged = 10 };
            var item = new WarehouseItem { Id = 1, StockQuantity = 10 };
            var log = new InventoryLog();
            _unitofwork.Setup(u => u.WarehouseItems.GetById(dto.WarehouseItemId)).ReturnsAsync(item);
            _mapper.Setup(m => m.Map<InventoryLog>(dto)).Returns(log);
            _unitofwork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitofwork.Setup(u => u.InventoryLogs.AddAsync(log)).Returns(Task.CompletedTask);

            _unitofwork.Setup(u => u.CompleteAsync()).ThrowsAsync(new Exception("Database connection failed"));
            var result = await _controller.CreateLog(dto);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            _unitofwork.Verify(u => u.RollbackAsync(), Times.Once);
        }
        [Fact]
        public async Task GetArchivedLogs()
        {
            var specparam = new InventorylogSpecparam { PageIndex = 1 };
            var dto = new List<InventoryLogDto> { new InventoryLogDto { Id = 1 } };
            var log = new List<ArchivedInventoryLog> { new ArchivedInventoryLog { Id = 1 } };
            _unitofwork.Setup(u => u.ArchivedInventoryLogs.GetAllWithSpecAsync(It.IsAny<ISpecification<ArchivedInventoryLog>>())).ReturnsAsync(log);
            _mapper.Setup(l => l.Map<IReadOnlyList<InventoryLogDto>>(log)).Returns(dto);
            var result = await _controller.GetArchiveLogs(specparam);
            var OkReult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(OkReult.Value);
        }
    }
}
