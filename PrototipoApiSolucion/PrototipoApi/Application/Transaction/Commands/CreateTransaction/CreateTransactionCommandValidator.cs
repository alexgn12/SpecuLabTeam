using FluentValidation;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de la transacción no puede ser futura.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
    }
}
