using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    string? ImageUrl,
    decimal Price,
    int StockQuantity,
    string SKU,
    int CategoryId,
    bool IsActive
) : IRequest<Result<ProductDto>>;
}
