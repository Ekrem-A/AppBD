using App.Application.Common;
using App.Application.Features.Admin.Commands;
using App.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Admin.Handlers
{
    public class UpdateUserRoleCommandHandler
     : IRequestHandler<UpdateUserRoleCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserRoleCommandHandler> _logger;

        public UpdateUserRoleCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateUserRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            UpdateUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    return Result<bool>.Failure("Kullanıcı bulunamadı.");
                }

                user.Role = request.NewRole;
                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Kullanıcı rolü güncellendi: UserId: {UserId}, Yeni Rol: {Role}",
                    request.UserId,
                    request.NewRole);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı rolü güncellenirken hata oluştu");
                return Result<bool>.Failure("Kullanıcı rolü güncellenemedi.");
            }
        }
    }
}
