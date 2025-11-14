using App.Application.Common;
using App.Application.Features.Products.Queries;
using App.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Queries.GetDashboardStats
{
    public record GetAllUsersForAdminQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    UserRole? Role = null
) : IRequest<Result<PaginatedResult<AdminUserDto>>>;

    public record AdminUserDto(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        UserRole Role,
        bool IsEmailConfirmed,
        int TotalOrders,
        decimal TotalSpent,
        DateTime CreatedAt
    );
}
