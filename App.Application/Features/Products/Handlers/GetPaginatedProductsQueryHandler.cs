using App.Application.Common;
using App.Application.DTOs;
using App.Application.Features.Products.Queries;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Products.Handlers
{
    public class GetPaginatedProductsQueryHandler
     : IRequestHandler<GetPaginatedProductsQuery, Result<PaginatedResult<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaginatedProductsQueryHandler> _logger;

        public GetPaginatedProductsQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetPaginatedProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<ProductDto>>> Handle(
            GetPaginatedProductsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _unitOfWork.Products.GetActiveProductsAsync(cancellationToken);
                var query = products.AsQueryable();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(p =>
                        p.Name.Contains(request.SearchTerm) ||
                        p.Description.Contains(request.SearchTerm));
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == request.CategoryId.Value);
                }

                if (request.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= request.MinPrice.Value);
                }

                if (request.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= request.MaxPrice.Value);
                }

                // Sıralama
                query = request.SortBy?.ToLower() switch
                {
                    "price" => request.IsDescending
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),
                    "date" => request.IsDescending
                        ? query.OrderByDescending(p => p.CreatedAt)
                        : query.OrderBy(p => p.CreatedAt),
                    _ => request.IsDescending
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name)
                };

                var totalCount = query.Count();

                // Sayfalama
                var items = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var productDtos = _mapper.Map<List<ProductDto>>(items);

                var paginatedResult = new PaginatedResult<ProductDto>
                {
                    Items = productDtos,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

                _logger.LogInformation(
                    "Sayfa {PageNumber}, Toplam {TotalCount} üründen {Count} ürün getirildi",
                    request.PageNumber,
                    totalCount,
                    productDtos.Count);

                return Result<PaginatedResult<ProductDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı ürün listesi getirilirken hata oluştu");
                return Result<PaginatedResult<ProductDto>>.Failure(
                    "Sayfalı ürün listesi getirilirken bir hata oluştu.");
            }
        }
    }
}
