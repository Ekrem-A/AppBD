using App.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Application.Features.Products.Commands.CreateProduct;
using App.Application.Features.Cart.Queries;
using App.Application.Features.Cart.Commands.AddToCart;
using App.Application.Features.Cart.Commands.Update;
using App.Application.Features.Cart.Commands.Remove;
using App.Application.Features.Cart.Commands.Clear;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var query = new GetCartByUserIdQuery(userId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var command = new AddToCartCommand(userId, dto.ProductId, dto.Quantity);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItemQuantity(int itemId, [FromBody] UpdateCartItemDto dto)
        {
            var command = new UpdateCartItemCommand(itemId, dto.Quantity);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var command = new RemoveFromCartCommand(itemId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var command = new ClearCartCommand(userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
