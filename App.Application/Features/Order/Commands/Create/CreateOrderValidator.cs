using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Order.Commands.Create
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir kullanıcı ID'si gereklidir");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .WithMessage("Teslimat adresi boş olamaz")
                .MaximumLength(500)
                .WithMessage("Teslimat adresi en fazla 500 karakter olabilir");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Sipariş en az bir ürün içermelidir");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .GreaterThan(0)
                    .WithMessage("Geçerli bir ürün ID'si gereklidir");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Miktar 0'dan büyük olmalıdır")
                    .LessThanOrEqualTo(100)
                    .WithMessage("Miktar 100'den küçük olmalıdır");
            });
        }
    }
}
