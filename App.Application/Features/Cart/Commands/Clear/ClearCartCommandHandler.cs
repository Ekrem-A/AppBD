using App.Application.Common;
using App.Domain.Entities;
using App.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Cart.Commands.Clear
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClearCartCommandHandler> _logger;

        public ClearCartCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ClearCartCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            ClearCartCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var cart = await _unitOfWork.Carts
                    .GetCartWithItemsAsync(request.UserId, cancellationToken);

                if (cart == null || !cart.CartItems.Any())
                {
                    return Result<bool>.Success(true);
                }

                var cartItemRepository = _unitOfWork.GetRepository<CartItem>() as IRepository<CartItem>;
                if (cartItemRepository == null)
                {
                    throw new InvalidOperationException("CartItem repository could not be resolved.");
                }

                foreach (var item in cart.CartItems.ToList())
                {
                    await cartItemRepository.DeleteAsync(item, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Sepet temizlendi: UserId: {UserId}", request.UserId);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sepet temizlenirken hata oluştu");
                return Result<bool>.Failure("Sepet temizlenirken bir hata oluştu.");
            }
        }
    }
}
