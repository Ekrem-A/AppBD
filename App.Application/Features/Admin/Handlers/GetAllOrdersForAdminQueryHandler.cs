using App.Application.Common;
using App.Application.DTOs;
using App.Application.Features.Admin.Queries.GetDashboardStats;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Handler
{
    public class GetAllOrdersForAdminQueryHandler
     : IRequestHandler<GetAllOrdersForAdminQuery, Result<PaginatedResult<AdminOrderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllOrdersForAdminQueryHandler> _logger;

        public GetAllOrdersForAdminQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllOrdersForAdminQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<AdminOrderDto>>> Handle(
            GetAllOrdersForAdminQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
                var query = orders.AsQueryable();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(o =>
                        o.OrderNumber.Contains(request.SearchTerm) ||
                        o.User.Email.Contains(request.SearchTerm) ||
                        o.User.FirstName.Contains(request.SearchTerm) ||
                        o.User.LastName.Contains(request.SearchTerm));
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(o => o.Status == request.Status.Value);
                }

                if (request.FromDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= request.ToDate.Value);
                }

                var totalCount = query.Count();

                // Sayfalama
                var items = query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var adminOrderDtos = items.Select(o => new AdminOrderDto(
                    o.Id,
                    o.OrderNumber,
                    $"{o.User.FirstName} {o.User.LastName}",
                    o.User.Email,
                    o.TotalAmount,
                    o.Status,
                    o.ShippingAddress,
                    o.TrackingNumber,
                    o.OrderDate,
                    o.OrderItems.Count,
                    _mapper.Map<List<OrderItemDto>>(o.OrderItems)
                )).ToList();

                var result = new PaginatedResult<AdminOrderDto>
                {
                    Items = adminOrderDtos,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

                return Result<PaginatedResult<AdminOrderDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin sipariş listesi getirilirken hata oluştu");
                return Result<PaginatedResult<AdminOrderDto>>.Failure(
                    "Sipariş listesi alınamadı.");
            }
        }
    }
}
