using App.Application.DTOs;
using App.Domain.Entities;
using AutoMapper;
using System.Linq;

namespace App.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.OrderItems));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name));

            // Cart mappings
            CreateMap<Cart, CartDto>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.CartItems))
                .ForMember(d => d.TotalAmount, opt => opt.MapFrom(
                    s => s.CartItems.Sum(ci => ci.Product.Price * ci.Quantity)   // ✔ Product.Price kullan
                ));

            // CartItem -> CartItemDto
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Product.Price))          // ✔
                .ForMember(d => d.SubTotal, opt => opt.MapFrom(s => s.Product.Price * s.Quantity));  // ✔

            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
        }
    }
}
