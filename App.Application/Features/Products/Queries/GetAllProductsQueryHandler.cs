using App.Application.Common;
using App.Application.DTOs;
using App.Application.Features.Products.Queries.GetAllProducts;
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
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<List<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<ProductDto>>> Handle(
            GetAllProductsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _unitOfWork.Products.GetActiveProductsAsync(cancellationToken);
                var productDtos = _mapper.Map<List<ProductDto>>(products);

                _logger.LogInformation("Toplam {Count} ürün getirildi", productDtos.Count);

                return Result<List<ProductDto>>.Success(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürünler getirilirken hata oluştu");
                return Result<List<ProductDto>>.Failure("Ürünler getirilirken bir hata oluştu.");
            }
        }
    }
}
