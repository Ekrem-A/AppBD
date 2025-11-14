using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir kullanıcı ID'si gereklidir");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ürün ID'si gereklidir");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Miktar 0'dan büyük olmalıdır")
                .LessThanOrEqualTo(50)
                .WithMessage("Bir seferde en fazla 50 adet ekleyebilirsiniz");
        }
    }
}
