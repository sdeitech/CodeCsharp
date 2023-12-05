using FluentValidation;

namespace bestallningsportal.Application.Product.Commands.UpdateProduct
{
    /// <summary>
    /// This code is used to validate the requests to command based on rules specified
    /// </summary>
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Namn krävs.")
                .MaximumLength(200);
            RuleFor(x => x.Unit).NotEmpty().WithMessage("Enhet krävs");
            RuleFor(x => x.ProductCategoryId).NotEmpty().WithMessage("Produktkategori krävs");
        }
    }
}
