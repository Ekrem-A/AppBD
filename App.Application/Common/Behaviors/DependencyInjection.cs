using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace App.Application.Common.Behaviors
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);

                // Pipeline behaviors ekle (sıralama önemli!)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
                // TransactionBehavior'u handler içinde manuel yönetiyoruz
            });

            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);           

            // AutoMapper
            services.AddAutoMapper(assembly);

            return services;
        }
    }
}
