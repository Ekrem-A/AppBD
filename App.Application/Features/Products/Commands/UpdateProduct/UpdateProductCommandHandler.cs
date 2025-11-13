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

namespace App.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductDto>> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Ürünü bul
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
                if (product == null)
                {
                    return Result<ProductDto>.Failure("Ürün bulunamadı.");
                }

                // Kategori kontrolü
                var categoryExists = await _unitOfWork.Categories
                    .ExistsAsync(request.CategoryId, cancellationToken);

                if (!categoryExists)
                {
                    return Result<ProductDto>.Failure("Kategori bulunamadı.");
                }

                // SKU kontrolü (aynı SKU'ya sahip başka ürün var mı?)
                var existingProducts = await _unitOfWork.Products.GetAllAsync(cancellationToken);
                if (existingProducts.Any(p => p.SKU == request.SKU && p.Id != request.Id))
                {
                    return Result<ProductDto>.Failure("Bu SKU başka bir ürün tarafından kullanılıyor.");
                }

                // Ürünü güncelle
                product.Name = request.Name;
                product.Description = request.Description;
                product.ImageUrl = request.ImageUrl;
                product.Price = request.Price;
                product.StockQuantity = request.StockQuantity;
                product.SKU = request.SKU;
                product.CategoryId = request.CategoryId;
                product.IsActive = request.IsActive;

                await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Ürün başarıyla güncellendi: {ProductId}", product.Id);

                var productDto = _mapper.Map<ProductDto>(product);
                return Result<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün güncellenirken hata oluştu");
                return Result<ProductDto>.Failure("Ürün güncellenirken bir hata oluştu.");
            }
        }
    }
}
