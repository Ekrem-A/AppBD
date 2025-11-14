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

namespace App.Application.Features.Cart.Commands.Update
{
    public class UpdateCartItemCommandHandler
    : IRequestHandler<UpdateCartItemCommand, Result<CartDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCartItemCommandHandler> _logger;

        public UpdateCartItemCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateCartItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CartDto>> Handle(
            UpdateCartItemCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var cartItem = await _unitOfWork.GetRepository<CartItem>()
                    .GetByIdAsync(request.CartItemId, cancellationToken);

                if (cartItem == null)
                {
                    return Result<CartDto>.Failure("Sepet öğesi bulunamadı.");
                }

                // Stok kontrolü
                var product = await _unitOfWork.Products
                    .GetByIdAsync(cartItem.ProductId, cancellationToken);

                if (product == null)
                {
                    return Result<CartDto>.Failure("Ürün bulunamadı.");
                }

                if (product.StockQuantity < request.Quantity)
                {
                    return Result<CartDto>.Failure(
                        $"Yetersiz stok. Mevcut: {product.StockQuantity}");
                }

                cartItem.Quantity = request.Quantity;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Sepet öğesi güncellendi: CartItemId: {CartItemId}, Yeni Miktar: {Quantity}",
                    request.CartItemId,
                    request.Quantity);

                // Güncel sepeti getir
                var cart = await _unitOfWork.Carts
                    .GetCartWithItemsAsync(cartItem.Cart.UserId, cancellationToken);

                var cartDto = _mapper.Map<CartDto>(cart);
                return Result<CartDto>.Success(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sepet öğesi güncellenirken hata oluştu");
                return Result<CartDto>.Failure("Sepet öğesi güncellenirken bir hata oluştu.");
            }
        }
    }
}
