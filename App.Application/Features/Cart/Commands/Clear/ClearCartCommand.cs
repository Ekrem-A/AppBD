using App.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Cart.Commands.Clear
{
    public record ClearCartCommand(int UserId) : IRequest<Result<bool>>;
}
