using FluentValidation;

namespace Payment.Application.Commands.Charges.ChargeOrderPayment;

public class ChargeOrderPaymentCommandValidator : AbstractValidator<ChargeOrderPaymentCommand>
{
    public ChargeOrderPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.BasketTotal).GreaterThan(0);
        RuleFor(x => x.PaidTotal).GreaterThan(0);
        RuleFor(x => x.BasketItems).NotEmpty();

        RuleFor(x => x.Card.HolderName).NotEmpty();
        RuleFor(x => x.Card.CardNumber).NotEmpty();
        RuleFor(x => x.Card.ExpireMonth).Matches(@"^(0[1-9]|1[0-2])$");
        RuleFor(x => x.Card.ExpireYear).Matches(@"^\d{4}$");
        RuleFor(x => x.Card.Cvc).Matches(@"^\d{3,4}$");

        RuleFor(x => x.Buyer.Name).NotEmpty();
        RuleFor(x => x.Buyer.Surname).NotEmpty();
        RuleFor(x => x.Buyer.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Buyer.IdentityNumber).Matches(@"^\d{11}$");
        RuleFor(x => x.Buyer.PhoneNumber).NotEmpty();
        RuleFor(x => x.Buyer.Ip).NotEmpty();

        RuleFor(x => x.Address.Description).NotEmpty();
        RuleFor(x => x.Address.City).NotEmpty();
        RuleFor(x => x.Address.Country).NotEmpty();
        RuleFor(x => x.Address.ZipCode).NotEmpty();
    }
}
