using App.Application.Common;
using App.Application.DTOs;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Products.Queries
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<List<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchProductsQueryHandler> _logger;

        public SearchProductsQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SearchProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<ProductDto>>> Handle(
            SearchProductsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    return Result<List<ProductDto>>.Failure("Arama terimi boş olamaz.");
                }

                var products = await _unitOfWork.Products
                    .SearchAsync(request.SearchTerm, cancellationToken);

                var productDtos = _mapper.Map<List<ProductDto>>(products);

                _logger.LogInformation(
                    "Arama terimi '{SearchTerm}' için {Count} ürün bulundu",
                    request.SearchTerm,
                    productDtos.Count);

                return Result<List<ProductDto>>.Success(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün arama sırasında hata oluştu");
                return Result<List<ProductDto>>.Failure("Ürün arama sırasında bir hata oluştu.");
            }
        }
    }

}
