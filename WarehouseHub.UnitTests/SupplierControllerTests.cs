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
    public class SupplierControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitofwork;
        private readonly Mock<IMapper> _mapper;
        private readonly SupplierController _controller;
        public SupplierControllerTests()
        {
            _unitofwork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _controller = new SupplierController(_unitofwork.Object, _mapper.Object);
        }
        [Fact]
        public async Task GetAll_ReturnOK()
        {
            var supplier = new List<Supplier> { new Supplier { Id = 1 } };
            var supplierdto = new List<SupplierDto> { new SupplierDto { Id = 1 } };
            _unitofwork.Setup(s => s.Suppliers.GetAllAsync()).ReturnsAsync(supplier);
            var mapped = _mapper.Setup(s => s.Map< IReadOnlyList<SupplierDto>>(supplier)).Returns(supplierdto);
            var result = await _controller.GetAllSuppliers();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
        [Fact]
        public async Task GetSupplierById_ReturnOK()
        {
            var existingsupplier = new Supplier { Id = 1 };
            var supplierdto = new SupplierDto { Id = 1 };
            _unitofwork.Setup(s => s.Suppliers.GetById(supplierdto.Id)).ReturnsAsync(existingsupplier);
            var mapped = _mapper.Setup(s => s.Map<Supplier, SupplierDto>(existingsupplier)).Returns(supplierdto);
            var result = await _controller.GetSupplierById(supplierdto.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

        }
        [Fact]
        public async Task GetSupplierById_ReturnNotFound()
        {
            _unitofwork.Setup(s => s.Suppliers.GetById(99)).ReturnsAsync((Supplier?)null);
            var result = await _controller.GetSupplierById(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(NotFoundResult.Value);

        }
        [Fact]
        public async Task GetSupplierById_ReturnBadRequest()
        {
            var result = await _controller.GetSupplierById(-1);
            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadRequestResult.Value);

        }
        [Fact]
        public async Task DeleteSupplier_ReturnOK()
        {
            var existingsupplier = new Supplier { Id = 1 };
            _unitofwork.Setup(s => s.Suppliers.GetById(existingsupplier.Id)).ReturnsAsync(existingsupplier);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            _unitofwork.Setup(u => u.InventoryLogs.AnyAsync(It.IsAny<Expression<Func<InventoryLog, bool>>>()))
                       .ReturnsAsync(false);
            var result = await _controller.DeleteSupplier(existingsupplier.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _unitofwork.Verify(s => s.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteSupplier_ReturnNotFound()
        {
            _unitofwork.Setup(s => s.Suppliers.GetById(99)).ReturnsAsync((Supplier?)null);
            var result = await _controller.GetSupplierById(99);
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(NotFoundResult.Value);

        }
        [Fact]
        public async Task DeleteSupplier_ReturnBadRequest()
        {
            var result = await _controller.DeleteSupplier(-1);
            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadRequestResult.Value);

        }
        [Fact]
        public async Task AddSupplier_OkResult()
        {
            var createDto = new CreateSupplierDto { Name = "New Supplier" };
            var supplierEntity = new Supplier { Id = 1, Name = "New Supplier" };
            _unitofwork.Setup(s => s.Suppliers.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>())).ReturnsAsync(false);
            _mapper.Setup(s => s.Map<Supplier>(createDto)).Returns(supplierEntity);
            _unitofwork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var result = await _controller.AddSupplier(createDto);
            var CreatedResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(CreatedResult.Value);
            _unitofwork.Verify(s => s.CompleteAsync(), Times.Once);
        }
    
            [Fact]
        public async Task AddSupplier_ExistResult()
        {
            var createDto = new CreateSupplierDto { Name = "New Supplier" };
            _unitofwork.Setup(s => s.Suppliers.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>())).ReturnsAsync(true);
            var result = await _controller.AddSupplier(createDto);
            var BadResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(BadResult.Value);
        }

    }
}