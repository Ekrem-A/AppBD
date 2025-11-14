using App.Application.Common;
using App.Application.DTOs;
using App.Application.Features.Admin.Queries.GetDashboardStats;
using App.Application.Features.Products.Queries;
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
    public class GetAllUsersForAdminQueryHandler
    : IRequestHandler<GetAllUsersForAdminQuery, Result<PaginatedResult<AdminUserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllUsersForAdminQueryHandler> _logger;

        public GetAllUsersForAdminQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllUsersForAdminQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<AdminUserDto>>> Handle(
            GetAllUsersForAdminQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
                var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

                var query = users.AsQueryable();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(u =>
                        u.Email.Contains(request.SearchTerm) ||
                        u.FirstName.Contains(request.SearchTerm) ||
                        u.LastName.Contains(request.SearchTerm));
                }

                if (request.Role.HasValue)
                {
                    query = query.Where(u => u.Role == request.Role.Value);
                }

                var totalCount = query.Count();

                // Sayfalama
                var items = query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var adminUserDtos = items.Select(u => new AdminUserDto(
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.PhoneNumber,
                    u.Role,
                    u.IsEmailConfirmed,
                    orders.Count(o => o.UserId == u.Id),
                    orders.Where(o => o.UserId == u.Id && o.Status != OrderStatus.Cancelled)
                          .Sum(o => o.TotalAmount),
                    u.CreatedAt
                )).ToList();

                var result = new PaginatedResult<AdminUserDto>
                {
                    Items = adminUserDtos,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

                return Result<PaginatedResult<AdminUserDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin kullanıcı listesi getirilirken hata oluştu");
                return Result<PaginatedResult<AdminUserDto>>.Failure(
                    "Kullanıcı listesi alınamadı.");
            }
        }
    }
}
