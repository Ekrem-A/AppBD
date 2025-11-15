using App.Application.DTOs;
using App.Application.Features.Products.Commands.CreateProduct;
using App.Application.Features.Products.Commands.DeleteProduct;
using App.Application.Features.Products.Commands.UpdateProduct;
using App.Application.Features.Products.Queries;
using App.Application.Features.Products.Queries.GetAllProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace App.API.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        //public async Task<IActionResult> GetAll()
        //{
        //    var query = new GetAllProductsQuery();
        //    var result = await _mediator.Send(query);

        //    if (!result.IsSuccess)
        //        return BadRequest(result.Error);

        //    return Ok(result.Data);
        //}

        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllProductsQuery();
                var result = await _mediator.Send(query);

                if (result == null)
                    return StatusCode(500, new { message = "Handler null result döndürdü." });

                if (!result.IsSuccess)
                    return BadRequest(result.Error);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                // Geçici olarak gerçek hatayı gör:
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var query = new GetProductsByCategoryQuery(categoryId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var query = new SearchProductsQuery(searchTerm);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
    }
}
