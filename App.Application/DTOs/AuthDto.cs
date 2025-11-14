using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record LoginDto(
    string Email,
    string Password
    );
    public record RegisterUserDto(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string? PhoneNumber
    );

    public record AuthResponseDto(
        string Token,
        string RefreshToken,
        UserDto User
    );

    public record RefreshTokenDto(
        string RefreshToken
    );
    public record ChangePasswordDto(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    );
    public record ForgotPasswordDto(
        string Email
    );

    public record ResetPasswordDto(
        string Email,
        string Token,
        string NewPassword,
        string ConfirmPassword
    );
    public record ConfirmEmailDto(
        string Email,
        string Token
    );
    public record UserDto(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string Role,
        bool IsEmailConfirmed
    );
    public record UpdateProfileDto(
        string FirstName,
        string LastName,
        string? PhoneNumber
    );
}
