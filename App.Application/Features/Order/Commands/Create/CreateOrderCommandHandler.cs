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
                

                // Kullanıcı kontrolü
                var userExists = await _unitOfWork.Users
                    .ExistsAsync(request.UserId, cancellationToken);

                if (!userExists)
                    return Result<OrderDto>.Failure("Kullanıcı bulunamadı.");

                // Sepeti getir
                var cart = await _unitOfWork.Carts
                    .GetCartWithItemsAsync(request.UserId, cancellationToken);

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                    return Result<OrderDto>.Failure("Sepette ürün bulunmuyor.");

                // Sipariş numarası
                var orderNumber = GenerateOrderNumber();

                // Sipariş entity
                var order = new App.Domain.Entities.Order
                {
                    OrderNumber = orderNumber,
                    UserId = request.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    ShippingAddress = request.ShippingAddress,
                    TotalAmount = 0m,
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0m;

                foreach (var cartItem in cart.CartItems)
                {
                    var product = cartItem.Product;

                    // Eğer navigation boşsa, garanti olsun diye DB'den çekelim
                    if (product == null)
                    {
                        product = await _unitOfWork.Products
                            .GetByIdAsync(cartItem.ProductId, cancellationToken);
                    }

                    if (product == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<OrderDto>.Failure($"Ürün bulunamadı: {cartItem.ProductId}");
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<OrderDto>.Failure(
                            $"Yetersiz stok: {product.Name} (Stok: {product.StockQuantity})");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * cartItem.Quantity
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;

                    // stok düş
                    product.StockQuantity -= cartItem.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                }

                order.TotalAmount = totalAmount;

                await _unitOfWork.Orders.AddAsync(order, cancellationToken);

                // Sepeti temizle (istersen tamamen sil)
                cart.CartItems.Clear();
                await _unitOfWork.Carts.UpdateAsync(cart, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);                

                _logger.LogInformation(
                    "Sepetten sipariş oluşturuldu: {OrderNumber}, Kullanıcı: {UserId}, Tutar: {Amount}",
                    order.OrderNumber,
                    request.UserId,
                    totalAmount);

                // 🔥 ORDER ENTITY -> OrderDto (DTO'yu elle oluşturuyoruz)
                var orderDto = new OrderDto(
                    Id: order.Id,
                    OrderNumber: order.OrderNumber,
                    OrderDate: order.OrderDate,
                    Status: order.Status.ToString(),
                    TotalAmount: order.TotalAmount,
                    ShippingAddress: order.ShippingAddress,
                    TrackingNumber: order.TrackingNumber,
                    Items: order.OrderItems.Select(oi =>
                        new OrderItemDto(
                            ProductId: oi.ProductId,
                            ProductName: oi.Product?.Name ?? string.Empty,
                            Quantity: oi.Quantity,
                            UnitPrice: oi.UnitPrice,
                            TotalPrice: oi.TotalPrice
                        )
                    ).ToList()
                );

                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Sepetten sipariş oluşturulurken hata oluştu");
                return Result<OrderDto>.Failure("Sepetten sipariş oluşturulurken bir hata oluştu.");
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
