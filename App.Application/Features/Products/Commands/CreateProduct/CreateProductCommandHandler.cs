using App.Application.Common;
using App.Application.DTOs;
using App.Domain.Entities;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductDto>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Kategori kontrolü
                var categoryExists = await _unitOfWork.Categories
                    .ExistsAsync(request.CategoryId, cancellationToken);

                if (!categoryExists)
                {
                    return Result<ProductDto>.Failure("Kategori bulunamadı.");
                }

                // SKU kontrolü
                var existingProducts = await _unitOfWork.Products.GetAllAsync(cancellationToken);
                if (existingProducts.Any(p => p.SKU == request.SKU))
                {
                    return Result<ProductDto>.Failure("Bu SKU zaten kullanılıyor.");
                }

                // Yeni ürün oluştur
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    SKU = request.SKU,
                    CategoryId = request.CategoryId,
                    IsActive = true
                };

                await _unitOfWork.Products.AddAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Ürün başarıyla oluşturuldu: {ProductId}", product.Id);

                var productDto = _mapper.Map<ProductDto>(product);
                return Result<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün oluşturulurken hata oluştu");
                return Result<ProductDto>.Failure("Ürün oluşturulurken bir hata oluştu.");
            }
        }
    }
}
