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
    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<List<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductsByCategoryQueryHandler> _logger;

        public GetProductsByCategoryQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetProductsByCategoryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<ProductDto>>> Handle(
            GetProductsByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Kategori var mı kontrol et
                var categoryExists = await _unitOfWork.Categories
                    .ExistsAsync(request.CategoryId, cancellationToken);

                if (!categoryExists)
                {
                    return Result<List<ProductDto>>.Failure("Kategori bulunamadı.");
                }

                var products = await _unitOfWork.Products
                    .GetByCategoryIdAsync(request.CategoryId, cancellationToken);

                var productDtos = _mapper.Map<List<ProductDto>>(products);

                _logger.LogInformation(
                    "Kategori {CategoryId} için {Count} ürün getirildi",
                    request.CategoryId,
                    productDtos.Count);

                return Result<List<ProductDto>>.Success(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori ürünleri getirilirken hata oluştu");
                return Result<List<ProductDto>>.Failure("Kategori ürünleri getirilirken bir hata oluştu.");
            }
        }
    }
}
