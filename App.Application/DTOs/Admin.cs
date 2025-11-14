using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record UpdateStockDto(int StockQuantity);
    public record BulkUpdateStatusDto(List<int> ProductIds, bool IsActive);
    public record UpdateOrderStatusDto(OrderStatus Status, string? TrackingNumber);
    public record CancelOrderDto(string Reason);
    public record UpdateUserRoleDto(UserRole Role);
    public record CreateCategoryDto(string Name, string? Description, int? ParentCategoryId);
    public record UpdateCategoryDto(int Id, string Name, string? Description, int? ParentCategoryId);
}
