using App.Application.Common;
using App.Domain.Enums;
using App.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>;

    public record DashboardStatsDto(
        int TotalProducts,
        int TotalOrders,
        int TotalUsers,
        int TotalCategories,
        decimal TotalRevenue,
        int PendingOrders,
        int LowStockProducts,
        List<RecentOrderDto> RecentOrders,
        List<TopSellingProductDto> TopSellingProducts,
        List<MonthlyRevenueDto> MonthlyRevenue
    );

    public record RecentOrderDto(
        int Id,
        string OrderNumber,
        string CustomerName,
        decimal TotalAmount,
        string Status,
        DateTime OrderDate
    );

    public record TopSellingProductDto(
        int ProductId,
        string ProductName,
        int TotalSold,
        decimal TotalRevenue
    );

    public record MonthlyRevenueDto(
        string Month,
        decimal Revenue,
        int OrderCount
    );
}
