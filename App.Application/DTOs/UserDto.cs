using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record UserDto(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string Role
    );

    public record RegisterUserDto(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string? PhoneNumber
    );

    public record LoginDto(
        string Email,
        string Password
    );

    public record AuthResponseDto(
        string Token,
        string RefreshToken,
        UserDto User
    );
}
