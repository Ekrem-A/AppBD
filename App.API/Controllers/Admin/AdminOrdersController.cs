using App.Application.Features.Admin.Queries.GetDashboardStats;
using App.Application.Features.Order.Commands.Cancel;
using App.Application.Features.Order.Commands.Update;
using App.Application.Features.Order.Queries;
using App.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] OrderStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetAllOrdersForAdminQuery(
                pageNumber,
                pageSize,
                searchTerm,
                status,
                fromDate,
                toDate
            );

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }

        //[HttpPut("{id}/status")]
        //public async Task<IActionResult> UpdateStatus(
        //    int id,
        //    [FromBody] UpdateOrderStatusDto dto)
        //{
        //    var command = new UpdateOrderStatusCommand(id, dto.Status, dto.TrackingNumber);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSuccess)
        //        return BadRequest(result.Error);

        //    return Ok(result.Data);
        //}

        //[HttpPost("{id}/cancel")]
        //public async Task<IActionResult> Cancel(
        //    int id,
        //    [FromBody] CancelOrderDto dto)
        //{
        //    var command = new CancelOrderCommand(id, dto.Reason);
        //    var result = await _mediator.Send(command);

        //    if (!result.IsSuccess)
        //        return BadRequest(result.Error);

        //    return Ok(new { message = "Sipariş iptal edildi" });
        //}
    }
}
