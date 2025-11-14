using App.Application.Common;
using App.Application.DTOs;
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

namespace App.Application.Features.Order.Commands.Update
{
    public class UpdateOrderStatusCommandHandler
     : IRequestHandler<UpdateOrderStatusCommand, Result<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

        public UpdateOrderStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateOrderStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(
            UpdateOrderStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var order = await _unitOfWork.Orders
                    .GetByIdAsync(request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result<OrderDto>.Failure("Sipariş bulunamadı.");
                }

                // Durum geçişi kontrolü
                if (!IsValidStatusTransition(order.Status, request.Status))
                {
                    return Result<OrderDto>.Failure(
                        $"Geçersiz durum geçişi: {order.Status} -> {request.Status}");
                }

                order.Status = request.Status;

                // Kargo takip numarası varsa güncelle
                if (!string.IsNullOrEmpty(request.TrackingNumber))
                {
                    order.TrackingNumber = request.TrackingNumber;
                }

                await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Sipariş durumu güncellendi: {OrderNumber}, Yeni Durum: {Status}",
                    order.OrderNumber,
                    request.Status);

                var orderDto = _mapper.Map<OrderDto>(order);
                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş durumu güncellenirken hata oluştu");
                return Result<OrderDto>.Failure("Sipariş durumu güncellenirken bir hata oluştu.");
            }
        }

        private bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
        {
            // Basit durum geçiş kontrolü
            return (current, next) switch
            {
                (OrderStatus.Pending, OrderStatus.PaymentReceived) => true,
                (OrderStatus.PaymentReceived, OrderStatus.Processing) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (_, OrderStatus.Cancelled) => true, // Her durumdan iptal edilebilir
                _ => false
            };
        }
    }
}
