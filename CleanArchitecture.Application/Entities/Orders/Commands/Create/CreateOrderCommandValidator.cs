namespace CleanArchitecture.Application.Entities.Orders.Commands.Create;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("CustomerId must be greater than or equal to 0");
    }
}