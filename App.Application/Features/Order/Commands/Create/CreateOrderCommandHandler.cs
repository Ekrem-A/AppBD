using App.Application.Common;
using App.Application.DTOs;
using App.Application.Features.Order.Commands;
using App.Application.Features.Order.Commands.Create;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Transaction başlat
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Kullanıcı kontrolü
                var userExists = await _unitOfWork.Users.ExistsAsync(request.UserId, cancellationToken);
                if (!userExists)
                {
                    return Result<OrderDto>.Failure("Kullanıcı bulunamadı.");
                }

                // Sipariş numarası oluştur
                var orderNumber = GenerateOrderNumber();

                // Sipariş oluştur
                var order = new App.Domain.Entities.Order
                {
                    OrderNumber = orderNumber,
                    UserId = request.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    ShippingAddress = request.ShippingAddress,
                    TotalAmount = 0
                };

                await _unitOfWork.Orders.AddAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                decimal totalAmount = 0;

                // Sipariş kalemlerini oluştur
                foreach (var item in request.Items)
                {
                    // Ürün kontrolü ve stok kontrolü
                    var product = await _unitOfWork.Products
                        .GetByIdAsync(item.ProductId, cancellationToken);

                    if (product == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<OrderDto>.Failure($"Ürün bulunamadı: {item.ProductId}");
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<OrderDto>.Failure(
                            $"Yetersiz stok: {product.Name} (Stok: {product.StockQuantity})");
                    }

                    // Sipariş kalemi oluştur
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * item.Quantity
                    };

                    order.OrderItems.Add(orderItem);

                    // Stok güncelle
                    product.StockQuantity -= item.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                }

                // Toplam tutarı güncelle
                order.TotalAmount = totalAmount;
                await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "Sipariş oluşturuldu: {OrderNumber}, Kullanıcı: {UserId}, Tutar: {Amount}",
                    orderNumber,
                    request.UserId,
                    totalAmount);

                // Siparişi tekrar getir (ilişkilerle birlikte)
                var createdOrder = await _unitOfWork.Orders
                    .GetByOrderNumberAsync(orderNumber, cancellationToken);

                var orderDto = _mapper.Map<OrderDto>(createdOrder);
                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Sipariş oluşturulurken hata oluştu");
                return Result<OrderDto>.Failure("Sipariş oluşturulurken bir hata oluştu.");
            }
        }

        private string GenerateOrderNumber()
        {
            // Format: ORD-YYYYMMDD-RANDOM6
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random().Next(100000, 999999);
            return $"ORD-{date}-{random}";
        }
    }
}
