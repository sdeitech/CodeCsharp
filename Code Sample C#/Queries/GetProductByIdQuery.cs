using bestallningsportal.Application.Common.Interfaces.Services;
using bestallningsportal.Application.Common.Models.Response;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Product.Queries.GetProductById
{ 
    public class GetProductByIdQuery: IRequest<ApplicationProduct>
    {
        public int Id { get; set; }
    }
    /// <summary>
    /// Query Handler to intercept and respond to the request made to controller by passing the request to service
    /// </summary>
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApplicationProduct>
    {
        private readonly IProductService  _productService;

        public GetProductByIdQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<ApplicationProduct> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetProductById(request.Id, cancellationToken);
        }

    }
}
