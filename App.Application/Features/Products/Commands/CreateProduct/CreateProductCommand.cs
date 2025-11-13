using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Application.Common;

namespace App.Application.Features.Products.Commands.CreateProduct
{
        public record CreateProductCommand(
        string Name,
        string Description,
        string? ImageUrl,
        decimal Price,
        int StockQuantity,
        string SKU,
        int CategoryId
    ) : IRequest<Result<ProductDto>>;
}
