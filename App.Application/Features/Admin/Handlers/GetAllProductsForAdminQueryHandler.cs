using App.Application.Common;
using App.Application.Features.Admin.Queries.GetDashboardStats;
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
    public class GetAllProductsForAdminQueryHandler
    : IRequestHandler<GetAllProductsForAdminQuery, Result<PaginatedResult<AdminProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllProductsForAdminQueryHandler> _logger;

        public GetAllProductsForAdminQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllProductsForAdminQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<AdminProductDto>>> Handle(
            GetAllProductsForAdminQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
                var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

                var query = products.AsQueryable();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(p =>
                        p.Name.Contains(request.SearchTerm) ||
                        p.SKU.Contains(request.SearchTerm));
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == request.CategoryId.Value);
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == request.IsActive.Value);
                }

                if (request.LowStock == true)
                {
                    query = query.Where(p => p.StockQuantity < 10);
                }

                var totalCount = query.Count();

                // Sayfalama
                var items = query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // DTO'ya çevir ve satış bilgilerini ekle
                var adminProductDtos = items.Select(p => new AdminProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.ImageUrl,
                    p.Price,
                    p.StockQuantity,
                    p.SKU,
                    p.IsActive,
                    p.CategoryId,
                    p.Category.Name,
                    orders.SelectMany(o => o.OrderItems)
                          .Where(oi => oi.ProductId == p.Id)
                          .Sum(oi => oi.Quantity),
                    p.CreatedAt,
                    p.UpdatedAt 
                )).ToList();

                var result = new PaginatedResult<AdminProductDto>
                {
                    Items = adminProductDtos,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

                return Result<PaginatedResult<AdminProductDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin ürün listesi getirilirken hata oluştu");
                return Result<PaginatedResult<AdminProductDto>>.Failure(
                    "Ürün listesi alınamadı.");
            }
        }
    }
}
