using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record OrderDto(
      int Id,
      string OrderNumber,
      DateTime OrderDate,
      string Status,
      decimal TotalAmount,
      string ShippingAddress,
      string? TrackingNumber,
      List<OrderItemDto> Items
 );

    public record OrderItemDto(
        int ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
    public record CreateOrderDto(
       int UserId,
       string ShippingAddress
   );

}
