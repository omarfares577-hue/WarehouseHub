using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Core.Dtos;
using WarehouseHub.Core.Dtos.Identity;
using WarehouseHub.Core.Errors;
using WarehouseHub.Core.Helpers;
using WarehouseHub.Core.Specification;
using WarehouseHub.Core.Specification.SpecParams;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;
using WarehouseHub.Infrastructure.Repositories;
using System.Threading.Tasks;
namespace WarehouseHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WarehouseController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<Pagination<WarehouseItemDto>>> GetAll([FromQuery] WarehouseitemSpecparam specparam)
        {
            var spec = new WarehouseSpec(specparam);
            var WarehouseItems = await _unitOfWork.WarehouseItems.GetAllWithSpecAsync(spec);
            var total = await _unitOfWork.WarehouseItems.GetCountAsync(spec);
            var mapped = _mapper.Map<IReadOnlyList<WarehouseItem>,IReadOnlyList<WarehouseItemDto>>(WarehouseItems);
            return Ok(new Pagination<WarehouseItemDto>(specparam.PageIndex, specparam.PageSize, total, mapped));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseItemDto>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });

            var spec = new WarehouseSpec(id);
            var Warehouseitem = await _unitOfWork.WarehouseItems.GetWithSpecById(spec);
            if (Warehouseitem is null)
                return NotFound(new ApiResponse(404));
            var mapped = _mapper.Map<WarehouseItemDto>(Warehouseitem);
            return Ok(mapped);
        }
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] CreateWarehouseItemDto dto)
        {
            bool nameExists = await _unitOfWork.WarehouseItems
                .AnyAsync(i => i.Name.ToLower() == dto.Name.ToLower());

            if (nameExists)
            {
                return BadRequest(new { message = "This Item Already Exist!" });
            }

            var item = new WarehouseItem
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Sku = dto.Sku,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };
            await _unitOfWork.WarehouseItems.AddAsync(item);
            await _unitOfWork.CompleteAsync();
            return  Created("Item has been Added Successfully",item);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });

            var item = await _unitOfWork.WarehouseItems.GetById(id);

            if (item == null)
                return NotFound(new ApiResponse(404));
            if (item.StockQuantity == 0)
            {
                _unitOfWork.WarehouseItems.Delete(item);
                await _unitOfWork.CompleteAsync();
                return Ok("Item has been Deleted Successfully");
            }
             return BadRequest(new { message = "Item still exist in Warehouse" });

        }
    }
}
