using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Queries
{
    public record GetOrdersByUserIdQuery(int UserId) : IRequest<Result<List<OrderDto>>>;
}
