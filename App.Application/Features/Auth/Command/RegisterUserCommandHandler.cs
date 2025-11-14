using App.Application.Common;
using App.Application.Common.Interfaces;
using App.Application.DTOs;
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

namespace App.Application.Features.Auth.Command
{
    public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Email kontrolü
                var emailExists = await _unitOfWork.Users
                    .EmailExistsAsync(request.Email, cancellationToken);

                if (emailExists)
                {
                    return Result<AuthResponseDto>.Failure("Bu email adresi zaten kullanılıyor.");
                }

                // Kullanıcı oluştur
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = _passwordHasher.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Role = UserRole.Customer,
                    IsEmailConfirmed = false
                };

                await _unitOfWork.Users.AddAsync(user, cancellationToken);

                //// Kullanıcı için boş sepet oluştur
                //var cart = new Cart { UserId = user.Id };
                //await _unitOfWork.Carts.AddAsync(cart, cancellationToken);

                //await _unitOfWork.SaveChangesAsync(cancellationToken);

                //_logger.LogInformation("Yeni kullanıcı kaydedildi: {Email}", request.Email);

                // Token oluştur
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var userDto = _mapper.Map<UserDto>(user);
                var response = new AuthResponseDto(token, refreshToken, userDto);

                return Result<AuthResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı kaydedilirken hata oluştu");
                return Result<AuthResponseDto>.Failure("Kullanıcı kaydedilirken bir hata oluştu.");
            }
        }
    }
}
