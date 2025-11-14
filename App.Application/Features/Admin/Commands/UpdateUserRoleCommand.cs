using App.Application.Common;
using App.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Commands
{
    public record UpdateUserRoleCommand(
     int UserId,
     UserRole NewRole
 ) : IRequest<Result<bool>>;
}
