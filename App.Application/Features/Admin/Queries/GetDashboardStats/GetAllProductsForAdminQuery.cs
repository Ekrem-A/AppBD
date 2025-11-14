using App.Application.Common;
using App.Application.Features.Products.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Queries.GetDashboardStats
{
    public record GetAllProductsForAdminQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    int? CategoryId = null,
    bool? IsActive = null,
    bool? LowStock = null
) : IRequest<Result<PaginatedResult<AdminProductDto>>>;

    public record AdminProductDto(
        int Id,
        string Name,
        string Description,
        string? ImageUrl,
        decimal Price,
        int StockQuantity,
        string SKU,
        bool IsActive,
        int CategoryId,
        string CategoryName,
        int TotalSold,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
