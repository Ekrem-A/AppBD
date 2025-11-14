using App.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Cart.Commands.Remove
{
    public record RemoveFromCartCommand(int CartItemId) : IRequest<Result<bool>>;
}
