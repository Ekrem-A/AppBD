using App.Application.Common;
using App.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Commands.Create
{
    public class CreateOrderCommand : IRequest<Result<OrderDto>>
    {
        public int UserId { get; }
        public string ShippingAddress { get; }

        public CreateOrderCommand(int userId, string shippingAddress)
        {
            UserId = userId;
            ShippingAddress = shippingAddress;
        }
    }
}


