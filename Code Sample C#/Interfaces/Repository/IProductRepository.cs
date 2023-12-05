using bestallningsportal.Application.Common.Models.Response;
using bestallningsportal.Application.Product.Queries.GetProduct;
using bestallningsportal.Domain.Entities;
using bestallningsportal.Domain.Entities.Product;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Application.Common.Interfaces.Repositories
{
    /// <summary>
    /// Interface that defines what methods will the repository have
    /// </summary>
    public interface IProductRepository
    {
        #region Create
        Task<int> AddProduct(ApplicationProduct application, CancellationToken cancellationToken);
        #endregion

        #region Return
        Task<List<ApplicationProduct>> GetProducts(GetProducts request, CancellationToken cancellationToken);
        Task<int> GetProductsCount(GetProducts request, CancellationToken cancellationToken);
        Task<ApplicationProduct> GetProductById(int id, CancellationToken cancellationToken);
        #endregion

        #region Update
        Task<int> UpdateProduct(ProductMaster applicationProduct, CancellationToken cancellationToken);
        #endregion

        #region Delete
        Task<bool> DeleteProduct(int id);
        #endregion
    }
}
