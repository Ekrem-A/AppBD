using App.Application.DTOs;
using App.Application.Features.Admin.Queries.GetDashboardStats;
using App.Application.Features.Products.Commands.CreateProduct;
using App.Application.Features.Products.Commands.DeleteProduct;
using App.Application.Features.Products.Commands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class AdminProductsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AdminProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? lowStock = null)
        {
            var query = new GetAllProductsForAdminQuery(
                pageNumber,
                pageSize,
                searchTerm,
                categoryId,
                isActive,
                lowStock
            );

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var command = new CreateProductCommand(
                dto.Name,
                dto.Description,
                dto.ImageUrl,
                dto.Price,
                dto.StockQuantity,
                dto.SKU,
                dto.CategoryId
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID uyuşmazlığı");

            var command = new UpdateProductCommand(
                dto.Id,
                dto.Name,
                dto.Description,
                dto.ImageUrl,
                dto.Price,
                dto.StockQuantity,
                dto.SKU,
                dto.CategoryId,
                dto.IsActive
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(
            int id,
            [FromBody] UpdateStockDto dto)
        {
            var command = new UpdateProductStockCommand(id, dto.StockQuantity);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPost("bulk-update-status")]
        public async Task<IActionResult> BulkUpdateStatus(
            [FromBody] BulkUpdateStatusDto dto)
        {
            var command = new BulkUpdateProductStatusCommand(dto.ProductIds, dto.IsActive);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
    }
}
