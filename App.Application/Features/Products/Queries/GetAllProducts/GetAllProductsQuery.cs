using App.Application.DTOs;
using App.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Application.Common;

namespace App.Application.Features.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery : IRequest<Result<List<ProductDto>>>;

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<List<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.GetActiveProductsAsync(cancellationToken);
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return Result<List<ProductDto>>.Success(productDtos);
        }
    }
}
