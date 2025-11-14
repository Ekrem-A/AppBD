using App.Application.Common;
using App.Domain.Enums;
using App.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Commands.Cancel
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CancelOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            CancelOrderCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var order = await _unitOfWork.Orders
                    .GetByIdAsync(request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result<bool>.Failure("Sipariş bulunamadı.");
                }

                // İptal edilebilir mi kontrol et
                if (order.Status == OrderStatus.Delivered)
                {
                    return Result<bool>.Failure("Teslim edilmiş siparişler iptal edilemez.");
                }

                if (order.Status == OrderStatus.Cancelled)
                {
                    return Result<bool>.Failure("Sipariş zaten iptal edilmiş.");
                }

                // Stokları geri yükle
                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.Products
                        .GetByIdAsync(item.ProductId, cancellationToken);

                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                    }
                }

                // Siparişi iptal et
                order.Status = OrderStatus.Cancelled;
                await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "Sipariş iptal edildi: {OrderNumber}, Sebep: {Reason}",
                    order.OrderNumber,
                    request.Reason);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Sipariş iptal edilirken hata oluştu");
                return Result<bool>.Failure("Sipariş iptal edilirken bir hata oluştu.");
            }
        }
    }
}
