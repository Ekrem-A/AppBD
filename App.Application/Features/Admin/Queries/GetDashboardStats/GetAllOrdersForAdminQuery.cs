using App.Application.Common;
using App.Application.DTOs;
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
    public record GetAllOrdersForAdminQuery(
     int PageNumber = 1,
     int PageSize = 20,
     string? SearchTerm = null,
     OrderStatus? Status = null,
     DateTime? FromDate = null,
     DateTime? ToDate = null
 ) : IRequest<Result<PaginatedResult<AdminOrderDto>>>;

    public record AdminOrderDto(
        int Id,
        string OrderNumber,
        string CustomerName,
        string CustomerEmail,
        decimal TotalAmount,
        OrderStatus Status,
        string ShippingAddress,
        string? TrackingNumber,
        DateTime OrderDate,
        int ItemCount,
        List<OrderItemDto> Items
    );
}
