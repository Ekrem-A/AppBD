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
                // Boş sepet döndür
                return Result<CartDto>.Success(new CartDto(
                    0,
                    request.UserId,
                    new List<CartItemDto>(),
                    0));
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return Result<CartDto>.Success(cartDto);
        }
    }
}
