using FluentValidation;
using PaperlessService.Entities;

namespace PaperlessService.Validation;

public class BlValidation : AbstractValidator<BlDocument>
{
    public BlValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FilePath)
            .NotEmpty();

        RuleFor(x => x.DateUploaded)
            .NotEmpty()
            .Must(date => date <= DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Upload date cannot be in the future");
    }
}
