using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Auth.Command
{
    public record RegisterUserCommand(
     string Email,
     string Password,
     string FirstName,
     string LastName,
     string? PhoneNumber
 ) : IRequest<Result<AuthResponseDto>>;
}
