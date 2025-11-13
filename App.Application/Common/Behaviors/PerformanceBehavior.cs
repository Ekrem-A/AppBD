using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>
     : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly System.Diagnostics.Stopwatch _timer;

        public PerformanceBehavior(
            ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new System.Diagnostics.Stopwatch();
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500) // 500ms'den uzun süren işlemleri logla
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "Yavaş işlem tespit edildi: {RequestName} ({ElapsedMilliseconds}ms) {@Request}",
                    requestName,
                    elapsedMilliseconds,
                    request);
            }

            return response;
        }
    }

}
