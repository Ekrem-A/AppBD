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

namespace App.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<CartDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddToCartCommandHandler> _logger;

        public AddToCartCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AddToCartCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CartDto>> Handle(
            AddToCartCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Ürün kontrolü
                var product = await _unitOfWork.Products
                    .GetByIdAsync(request.ProductId, cancellationToken);

                if (product == null)
                {
                    return Result<CartDto>.Failure("Ürün bulunamadı.");
                }

                if (!product.IsActive)
                {
                    return Result<CartDto>.Failure("Bu ürün şu anda satışta değil.");
                }

                if (product.StockQuantity < request.Quantity)
                {
                    return Result<CartDto>.Failure(
                        $"Yetersiz stok. Mevcut: {product.StockQuantity}");
                }

                // Sepeti getir veya oluştur
                var cart = await _unitOfWork.Carts
                    .GetCartWithItemsAsync(request.UserId, cancellationToken);

                if (cart == null)
                {
                    cart = new App.Domain.Entities.Cart { UserId = request.UserId };
                    await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                // Aynı ürün sepette var mı kontrol et
                var existingItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == request.ProductId);

                if (existingItem != null)
                {
                    // Varsa miktarı artır
                    var newQuantity = existingItem.Quantity + request.Quantity;

                    if (product.StockQuantity < newQuantity)
                    {
                        return Result<CartDto>.Failure(
                            $"Yetersiz stok. Sepetteki miktar: {existingItem.Quantity}, " +
                            $"Mevcut stok: {product.StockQuantity}");
                    }

                    existingItem.Quantity = newQuantity;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    // Yoksa yeni item ekle
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity
                    };

                    cart.CartItems.Add(cartItem);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInformation(
                    "Ürün sepete eklendi: UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}",
                    request.UserId,
                    request.ProductId,
                    request.Quantity);

                // Güncel sepeti getir
                cart = await _unitOfWork.Carts
                    .GetCartWithItemsAsync(request.UserId, cancellationToken);

                var cartDto = _mapper.Map<CartDto>(cart);
                return Result<CartDto>.Success(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün sepete eklenirken hata oluştu");
                return Result<CartDto>.Failure("Ürün sepete eklenirken bir hata oluştu.");
            }
        }
    }
}
