using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Cart.Commands.Update
{
    public record UpdateCartItemCommand(
    int CartItemId,
    int Quantity
) : IRequest<Result<CartDto>>;

}
