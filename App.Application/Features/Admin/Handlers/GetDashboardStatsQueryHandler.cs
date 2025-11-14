using App.Application.Common;
using App.Application.Features.Admin.Queries.GetDashboardStats;
using App.Domain.Enums;
using App.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Handlers
{
    public class GetDashboardStatsQueryHandler
    : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDashboardStatsQueryHandler> _logger;

        public GetDashboardStatsQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetDashboardStatsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<DashboardStatsDto>> Handle(
            GetDashboardStatsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
                var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
                var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
                var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);

                // Temel istatistikler
                var totalProducts = products.Count();
                var totalOrders = orders.Count();
                var totalUsers = users.Count();
                var totalCategories = categories.Count();
                var totalRevenue = orders
                    .Where(o => o.Status != OrderStatus.Cancelled)
                    .Sum(o => o.TotalAmount);
                var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
                var lowStockProducts = products.Count(p => p.StockQuantity < 10 && p.IsActive);

                // Son siparişler (son 10)
                var recentOrders = orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .Select(o => new RecentOrderDto(
                        o.Id,
                        o.OrderNumber,
                        $"{o.User.FirstName} {o.User.LastName}",
                        o.TotalAmount,
                        o.Status.ToString(),
                        o.OrderDate
                    ))
                    .ToList();

                // En çok satan ürünler
                var topSellingProducts = orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                    .Select(g => new TopSellingProductDto(
                        g.Key.ProductId,
                        g.Key.Name,
                        g.Sum(oi => oi.Quantity),
                        g.Sum(oi => oi.TotalPrice)
                    ))
                    .OrderByDescending(p => p.TotalSold)
                    .Take(10)
                    .ToList();

                // Aylık gelir (son 6 ay)
                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
                var monthlyRevenue = orders
                    .Where(o => o.OrderDate >= sixMonthsAgo && o.Status != OrderStatus.Cancelled)
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new MonthlyRevenueDto(
                        $"{g.Key.Year}-{g.Key.Month:D2}",
                        g.Sum(o => o.TotalAmount),
                        g.Count()
                    ))
                    .OrderBy(m => m.Month)
                    .ToList();

                var stats = new DashboardStatsDto(
                    totalProducts,
                    totalOrders,
                    totalUsers,
                    totalCategories,
                    totalRevenue,
                    pendingOrders,
                    lowStockProducts,
                    recentOrders,
                    topSellingProducts,
                    monthlyRevenue
                );

                return Result<DashboardStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard istatistikleri getirilirken hata oluştu");
                return Result<DashboardStatsDto>.Failure("Dashboard istatistikleri alınamadı.");
            }
        }
    }
}
