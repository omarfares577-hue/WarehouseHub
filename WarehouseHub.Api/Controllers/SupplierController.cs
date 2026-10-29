using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Core.Dtos;
using WarehouseHub.Core.Errors;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;

namespace WarehouseHub.Api.Controllers
{
    public class SupplierController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SupplierController( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var mapped = _mapper.Map<IReadOnlyList<SupplierDto>>(suppliers);
            return Ok(mapped);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });
            var supplier = await _unitOfWork.Suppliers.GetById(id);

            if (supplier == null)
                return NotFound(new { message = "Supplier not found" });
            var mapped = _mapper.Map<SupplierDto>(supplier);

            return Ok(supplier);
        }
        [HttpPost()]
        public async Task<IActionResult> AddSupplier([FromBody] CreateSupplierDto supplierDto)
        {
            bool nameExists = await _unitOfWork.Suppliers
             .AnyAsync(i => i.Name.ToLower() == supplierDto.Name.ToLower());

            if (nameExists)
            {
                return BadRequest(new { message = "This Supplier Already Exist!" });
            }

            var supplier = _mapper.Map<Supplier>(supplierDto);

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();
            
            return Created("Supplier has been added Successfully", supplier );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });
            var supplier = await _unitOfWork.Suppliers.GetById(id);
            if (supplier == null)
                return NotFound(new ApiResponse(404));
            bool hasLogs = await _unitOfWork.InventoryLogs.AnyAsync(l => l.SupplierId == id);
            if (hasLogs)
            {
                return BadRequest(new { message = "Supplier is related to logs and cannot be deleted." });
            }
            _unitOfWork.Suppliers.Delete(supplier);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Supplier has been deleted Successfully" });
        }
    }
}
