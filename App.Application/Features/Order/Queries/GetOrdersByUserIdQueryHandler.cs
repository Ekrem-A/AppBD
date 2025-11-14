using App.Application.Common;
using App.Application.DTOs;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Queries
{
    public class GetOrdersByUserIdQueryHandler
    : IRequestHandler<GetOrdersByUserIdQuery, Result<List<OrderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrdersByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<OrderDto>>> Handle(
            GetOrdersByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders
                .GetByUserIdAsync(request.UserId, cancellationToken);

            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            return Result<List<OrderDto>>.Success(orderDtos);
        }
    }
}
