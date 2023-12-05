using bestallningsportal.Application.Common.Interfaces.Services;
using bestallningsportal.Application.Common.Models.Response.Product;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Product.Queries.GetProduct
{
    public class GetProducts: IRequest<PaginatedProducts>
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
        public string Plu { get; set; }
        public int? ProductCategory { get; set; }
        public int? ProductType { get; set; }
        public bool ForSupplierOnly { get; set; }
        public bool? IsActive { get; set; }
        public bool? OnlyPluProd { get; set; }
        public bool? IsRepresentation { get; set; }
        //Pagination
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetProductQueryHandler : IRequestHandler<GetProducts, PaginatedProducts>
    {
        private readonly IProductService _productService;
        public GetProductQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<PaginatedProducts> Handle(GetProducts request, CancellationToken cancellationToken)
        {
            return await _productService.GetProducts(request, cancellationToken);
        }
    }
}
