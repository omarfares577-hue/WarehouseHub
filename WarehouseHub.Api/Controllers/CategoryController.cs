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
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;
using WarehouseHub.Infrastructure.Repositories;

namespace WarehouseHub.Api.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryController( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var mapped = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
            return Ok(mapped);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)

        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });

            var category = await _unitOfWork.Categories.GetById(id);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto categoryDto)
        {
            bool nameExists = await _unitOfWork.Categories
                 .AnyAsync(i => i.Name.ToLower() == categoryDto.Name.ToLower());

            if (nameExists)
            {
                return BadRequest(new { message = "This Category Already Exist!" });
            }

            var category = _mapper.Map<Category>(categoryDto);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return Created("Category has been added Successfully", category );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid ID" });

            var category = await _unitOfWork.Categories.GetById(id);

            if (category == null)
                return NotFound(new ApiResponse(404));

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Category has been deleted Successfully" });
        }
    }
}
