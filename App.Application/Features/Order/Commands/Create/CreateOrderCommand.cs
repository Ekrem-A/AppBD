using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Commands.Create
{
    public record CreateOrderCommand(
    int UserId,
    string ShippingAddress,
    List<CreateOrderItemDto> Items
) : IRequest<Result<OrderDto>>;
}
