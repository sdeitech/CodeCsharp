using bestallningsportal.Application.Common.Interfaces.Services;
using bestallningsportal.DTO.Request;
using bestallningsportal.DTO.Request.Product;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Product.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
        public int Vat { get; set; }
        public string Plu { get; set; }
        public bool IsPackagable { get; set; }
        public string Description { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductType ProductType { get; set; }
        public bool ForSupplierOnly { get; set; }
        public bool IsActive { get; set; }
        public bool IsRepresentation { get; set; }
        public bool IsAutomaticDeposit { get; set; }
        public int? AutomaticDepositType { get; set; }
        public bool IsPant { get; set; }
        public int AccountNumber { get; set; }
        public int AlcoholType { get; set; }
        public List<ProductListProductDto> ProductListProductsMappings { get; set; }
    }
    /// <summary>
    /// Command Handler to intercept and respond to the request made to controller by passing the request to service
    /// </summary>
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductService _productService;
        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return await _productService.AddProduct(request, cancellationToken);
        }
    }
}
