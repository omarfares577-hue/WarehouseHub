using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
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

namespace WarehouseHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventorylogController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public InventorylogController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }


        [HttpGet]
        public async Task<ActionResult<Pagination<InventoryLogDto>>> GetAll([FromQuery] InventorylogSpecparam specparam)
        {
            var spec = new InventoryLogSpec(specparam);
            var inventorylogs = await _unitOfWork.InventoryLogs.GetAllWithSpecAsync(spec);
            var total = await _unitOfWork.InventoryLogs.GetCountAsync(spec);
            var mapped = _mapper.Map<IReadOnlyList<InventoryLog>, IReadOnlyList<InventoryLogDto>>(inventorylogs);
            return Ok(new Pagination<InventoryLogDto>(specparam.PageIndex, specparam.PageSize, total, mapped));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryLogDto>> GetLogById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });
            var spec = new InventoryLogSpec(id);
            var inventorylogs = await _unitOfWork.InventoryLogs.GetWithSpecById(spec);
            if (inventorylogs is null)
                return NotFound(new ApiResponse(404));
            var mapped = _mapper.Map<InventoryLogDto>(inventorylogs);
            return Ok(mapped);

        }
        [HttpPost()]
        public async Task<IActionResult> CreateLog([FromBody] CreateLogsDto dto)
        {
            if (dto.WarehouseItemId <= 0)
            {
                return BadRequest(new ApiResponse(400, "Invalid WarehouseItemId"));
            }
            var normalizedOperation = dto.OperationType.Trim().ToLower();
            Operation operation;
            var item = await _unitOfWork.WarehouseItems.GetById(dto.WarehouseItemId);
            if (item == null)
            {
                return NotFound(new ApiResponse(404));
            }
            if (normalizedOperation == "in")
                operation = Operation.In;
            else if (normalizedOperation == "out")
                operation = Operation.Out;
            else
                return BadRequest(new ApiResponse(400, "InValid Operation Use In or Out Only"));

            string AlertMessage = "";
            if (operation == Operation.Out)
            {
                if (item.StockQuantity < dto.QuantityChanged)
                    return BadRequest(new ApiResponse(400, "Stock Don't have enough Items for this Operation"));
                item.StockQuantity -= dto.QuantityChanged;
                if (item.MinStockLevel >= item.StockQuantity)
                    AlertMessage = $"Alert: Stock about to get empty";

            }
            else
            {
                item.StockQuantity += dto.QuantityChanged;
            }
            item.UpdatedAt = DateTime.UtcNow;
            var newLog = _mapper.Map<InventoryLog>(dto);
            newLog.OperationType = operation;
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.InventoryLogs.AddAsync(newLog);
                _unitOfWork.WarehouseItems.Update(item);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();

                return Created( AlertMessage, newLog);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, new { message = "An error occurred while processing the transaction.", error = ex.Message });
            }

        }

        [HttpGet("archive")]
        public async Task<ActionResult<Pagination<InventoryLogDto>>> GetArchiveLogs([FromQuery] InventorylogSpecparam specParam)
        {
            var spec = new InventoryLogArchiveSpec(specParam);

            var logs = await _unitOfWork.ArchivedInventoryLogs.GetAllWithSpecAsync(spec);

            var totalItems = await _unitOfWork.ArchivedInventoryLogs.GetCountAsync(spec);

            var data = _mapper.Map<IReadOnlyList<InventoryLogDto>>(logs);

            return Ok(new Pagination<InventoryLogDto>(specParam.PageIndex, specParam.PageSize, totalItems, data));
        }
    }
}
