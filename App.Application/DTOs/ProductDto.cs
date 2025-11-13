using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.DTOs
{
    public record ProductDto(
         int Id,
         string Name,
         string Description,
         string? ImageUrl,
         decimal Price,
         int StockQuantity,
         string SKU,
         bool IsActive,
         int CategoryId,
         string CategoryName
     );

    public record CreateProductDto(
        string Name,
        string Description,
        string? ImageUrl,
        decimal Price,
        int StockQuantity,
        string SKU,
        int CategoryId
    );

    public record UpdateProductDto(
        int Id,
        string Name,
        string Description,
        string? ImageUrl,
        decimal Price,
        int StockQuantity,
        string SKU,
        int CategoryId,
        bool IsActive
    );
}
