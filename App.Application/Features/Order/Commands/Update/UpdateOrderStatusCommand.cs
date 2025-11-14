using App.Application.Common;
using App.Application.DTOs;
using App.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Commands.Update
{
    public record UpdateOrderStatusCommand(
    int OrderId,
    OrderStatus Status,
    string? TrackingNumber = null
) : IRequest<Result<OrderDto>>;

}
