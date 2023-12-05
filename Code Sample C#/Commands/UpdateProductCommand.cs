using bestallningsportal.Application.Common.Interfaces.Services;
using bestallningsportal.DTO.Request;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Product.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
        public int Vat { get; set; }
        public bool IsRepresentation { get; set; }
        public string Plu { get; set; }
        public bool IsPackagable { get; set; }
        public string Description { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductType { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutomaticDeposit { get; set; }
        public int? AutomaticDepositType { get; set; }
        public bool IsPant { get; set; }
        public bool IsPriceChanged { get; set; }
        public bool IsPriceEditable { get; set; }
        public int AccountNumber { get; set; }
        public int AlcoholType { get; set; }

        public List<ProductListProductDto> ProductListProductsMappings { get; set; }
    }
    /// <summary>
    /// Command Handler to intercept and respond to the request made to controller by passing the request to service
    /// </summary>
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<int> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return await _productService.UpdateProduct(request, cancellationToken);
        }
    }
}
