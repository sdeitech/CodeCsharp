using FluentValidation;

namespace bestallningsportal.Application.Product.Commands.CreateProduct
{
    /// <summary>
    /// This code is used to validate the requests to command based on rules specified
    /// </summary>
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Namn krävs.")
                .MaximumLength(200);
            RuleFor(x => x.Unit).NotEmpty().WithMessage("Enhet krävs");
            RuleFor(x => x.ProductCategoryId).NotEmpty().WithMessage("Produktkategori krävs");
        }
    }
}
