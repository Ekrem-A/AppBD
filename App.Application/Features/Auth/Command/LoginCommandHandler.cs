using App.Application.Common;
using App.Application.Common.Interfaces;
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

namespace App.Application.Features.Auth.Command
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<LoginCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .GetByEmailAsync(request.Email, cancellationToken);

                if (user == null)
                {
                    return Result<AuthResponseDto>.Failure("Email veya şifre hatalı.");
                }

                var isPasswordValid = _passwordHasher
                    .VerifyPassword(request.Password, user.PasswordHash);

                if (!isPasswordValid)
                {
                    return Result<AuthResponseDto>.Failure("Email veya şifre hatalı.");
                }

                _logger.LogInformation("Kullanıcı giriş yaptı: {Email}", request.Email);

                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var userDto = _mapper.Map<UserDto>(user);
                var response = new AuthResponseDto(token, refreshToken, userDto);

                return Result<AuthResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş yapılırken hata oluştu");
                return Result<AuthResponseDto>.Failure("Giriş yapılırken bir hata oluştu.");
            }
        }
    }
}
