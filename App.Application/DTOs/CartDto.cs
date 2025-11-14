using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record CartDto(
        int Id,
        int UserId,
        List<CartItemDto> Items,
        decimal TotalAmount
    );

    public record CartItemDto(
        int Id,
        int ProductId,
        string ProductName,
        decimal Price,
        int Quantity,
        decimal SubTotal
    );

    public record AddToCartDto(
        int UserId,
        int ProductId,
        int Quantity
    );

    public record UpdateCartItemDto(
    int Quantity
    );

    public record RemoveFromCartDto(
        int CartItemId
    );
}
