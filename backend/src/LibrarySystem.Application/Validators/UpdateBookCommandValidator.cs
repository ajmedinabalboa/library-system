
using FluentValidation;
using LibrarySystem.Application.Commands.Books;

namespace LibrarySystem.Application.Validators;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Book ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.Authors)
            .NotEmpty().WithMessage("At least one author is required")
            .Must(authors => authors != null && authors.Count > 0)
            .WithMessage("At least one author is required");
    }
}
