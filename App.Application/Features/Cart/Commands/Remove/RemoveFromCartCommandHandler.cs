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

namespace App.Application.Features.Cart.Commands.Remove
{
    public class RemoveFromCartCommandHandler
        : IRequestHandler<RemoveFromCartCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveFromCartCommandHandler> _logger;

        public RemoveFromCartCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<RemoveFromCartCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            RemoveFromCartCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<CartItem>() as IRepository<CartItem>;
                if (repository == null)
                {
                    return Result<bool>.Failure("CartItem repository could not be resolved.");
                }

                var cartItem = await repository.GetByIdAsync(request.CartItemId, cancellationToken);

                if (cartItem == null)
                {
                    return Result<bool>.Failure("Sepet öğesi bulunamadı.");
                }

                await repository.DeleteAsync(cartItem, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Sepet öğesi silindi: CartItemId: {CartItemId}",
                    request.CartItemId);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sepet öğesi silinirken hata oluştu");
                return Result<bool>.Failure("Sepet öğesi silinirken bir hata oluştu.");
            }
        }
    }
}
