using bestallningsportal.Application.Common.Models.Response;
using bestallningsportal.Application.Common.Models.Response.Product;
using bestallningsportal.Application.Product.Commands.ActivateProduct;
using bestallningsportal.Application.Product.Commands.CreateProduct;
using bestallningsportal.Application.Product.Commands.ImportProduct;
using bestallningsportal.Application.Product.Commands.UpdateProduct;
using bestallningsportal.Application.Product.Queries.GetProduct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Common.Interfaces.Services
{
    /// <summary>
    /// Interface that defines what methods will the service have
    /// </summary>
    public interface IProductService
    {
        #region Create
        Task<int> AddProduct(CreateProductCommand createProductCommand, CancellationToken cancellationToken);
        #endregion

        #region Return
        Task<PaginatedProducts> GetProducts(GetProducts request, CancellationToken cancellationToken);
        Task<ApplicationProduct> GetProductById(int id, CancellationToken cancellationToken);
        #endregion

        #region Update
        Task<int> UpdateProduct(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken);
        #endregion

        #region Delete
        Task<bool> DeleteProduct(int id);
        #endregion
    }
}
