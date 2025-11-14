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
    public record SearchProductsQuery(string SearchTerm) : IRequest<Result<List<ProductDto>>>;

}
