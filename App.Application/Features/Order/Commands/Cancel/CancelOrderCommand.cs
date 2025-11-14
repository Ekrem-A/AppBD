using App.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Commands.Cancel
{
    public record CancelOrderCommand(
     int OrderId,
     string Reason
    ) : IRequest<Result<bool>>;
}
