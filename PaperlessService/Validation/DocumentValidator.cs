using FluentValidation;
using PaperlessService.Entities;

namespace PaperlessService.Validation;

public class DocumentValidator : AbstractValidator<BlDocument>
{
    public DocumentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FilePath)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.DateUploaded)
            .NotEmpty()
            .Must(date => date <= DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Upload date cannot be in the future");
    }
}