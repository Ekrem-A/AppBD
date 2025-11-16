using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record CartDto
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public List<CartItemDto> Items { get; init; } = new();
        public decimal TotalAmount { get; init; }
    }

    public record CartItemDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int Quantity { get; init; }
        public decimal SubTotal { get; init; }
    }

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
