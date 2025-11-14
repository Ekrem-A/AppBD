using App.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(
            IUnitOfWork unitOfWork,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Sadece Command'ler için transaction kullan
            var requestName = typeof(TRequest).Name;
            var isCommand = requestName.EndsWith("Command");

            if (!isCommand)
            {
                return await next();
            }

            _logger.LogInformation("Transaction başlatıldı: {RequestName}", requestName);

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var response = await next();
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Transaction başarılı: {RequestName}", requestName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction başarısız: {RequestName}", requestName);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
