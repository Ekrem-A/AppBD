using App.Application.Common;
using App.Application.DTOs;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Cart.Queries
{
    public class GetCartByUserIdQueryHandler
      : IRequestHandler<GetCartByUserIdQuery, Result<CartDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCartByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CartDto>> Handle(
            GetCartByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.Carts
                .GetCartWithItemsAsync(request.UserId, cancellationToken);

            if (cart == null)
            {

                var emptyCart = new CartDto
                {
                    Id = 0,
                    UserId = request.UserId,
                    Items = new List<CartItemDto>(),
                    TotalAmount = 0m
                };

                return Result<CartDto>.Success(emptyCart);
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return Result<CartDto>.Success(cartDto);
        }
    }
}
