using bestallningsportal.Application.Common.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Product.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
    /// <summary>
    /// Command Handler to intercept and respond to the request made to controller by passing the request to service
    /// </summary>
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductService  _productService;

        public DeleteProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, System.Threading.CancellationToken cancellationToken)
        {
            return await _productService.DeleteProduct(request.Id);
        }
    }
}



