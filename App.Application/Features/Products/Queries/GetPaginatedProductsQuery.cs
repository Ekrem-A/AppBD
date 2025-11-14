using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Products.Queries
{
    public record GetPaginatedProductsQuery(
     int PageNumber = 1,
     int PageSize = 10,
     string? SearchTerm = null,
     int? CategoryId = null,
     decimal? MinPrice = null,
     decimal? MaxPrice = null,
     string? SortBy = "name",
     bool IsDescending = false
 ) : IRequest<Result<PaginatedResult<ProductDto>>>;

}
