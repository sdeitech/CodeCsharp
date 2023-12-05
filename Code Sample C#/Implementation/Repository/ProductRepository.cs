using AutoMapper;
using bestallningsportal.Application.Common.Interfaces.Repositories;
using bestallningsportal.Application.Common.Models.Response;
using bestallningsportal.Application.Interfaces;
using bestallningsportal.Application.Product.Queries.GetProduct;
using bestallningsportal.Domain.Entities;
using bestallningsportal.Infrastructure.Attributes;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Infrastructure.Implementation.Repositories
{
    /// <summary>
    /// This repository implements the methods defined in the implemented interface
    /// </summary>
    [ScopedService]
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #region Create
        /// <summary>
        /// This method is used to create a product
        /// </summary>
        /// <param name="application">Command containing properties to create new product</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Return Id of newly created product</returns>
        public async Task<int> AddProduct(ApplicationProduct application, CancellationToken cancellationToken)
        {
            var product = application.Adapt<ProductMaster>();
            await _dbContext.ProductMasters.AddAsync(product, cancellationToken);
            if (product.ProductListProductsMappings != null)
            {
                product.ProductListProductsMappings.Select(w => w.ProductId = product.Id).ToList();
                await _dbContext.ProductListProductsMapping.AddRangeAsync(product.ProductListProductsMappings, cancellationToken);
            }
            await _dbContext.SaveChangesAsync();

            return product.Id;
        }
        #endregion

        #region Return
        /// <summary>
        /// This method is used to get products based on supplied filters
        /// </summary>
        /// <param name="request">Contains properties to match with when applying filters</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns list of products and count as well</returns>
        public async Task<List<ApplicationProduct>> GetProducts(GetProducts request, CancellationToken cancellationToken)
        {
            var skip = request.PageNo != null ? (request.PageNo - 1) * request.PageSize : 0;
            var list = await _dbContext.ProductMasters
                .Include(a => a.ProductCategories)
            .Where(x => (x.ForSupplierOnly == request.ForSupplierOnly)
            && (request.Id != null ? x.Id == request.Id : true)
            && (!string.IsNullOrEmpty(request.Name) ? x.Name.ToLower().Trim().Contains(request.Name.ToLower().Trim()) : true)
            && (!string.IsNullOrEmpty(request.Comment) ? x.Comment.ToLower().Trim().Contains(request.Comment.ToLower().Trim()) : true)
            && (request.Plu != null && request.OnlyPluProd != true ? x.Plu.ToLower().Trim().Contains(request.Plu.ToLower().Trim()) : true)
            && (request.OnlyPluProd == true ? x.Plu.ToLower().Trim() == request.Plu.ToLower().Trim() : true)
            && (request.FromPrice != null ? request.FromPrice >= x.Price : true)
            && (request.ToPrice != null ? x.Price <= request.ToPrice : true)
            && (request.ProductCategory != null ? x.ProductCategoryId == request.ProductCategory : true)
            && (request.ProductType != null ? x.ProductType == request.ProductType : true)
            && (request.IsActive != null ? x.IsActive == request.IsActive : true)
            && (request.IsRepresentation != null ? x.IsRepresentation == request.IsRepresentation : true)
            )
            .Skip((int)skip).Take((int)(request.PageSize != null ? request.PageSize : await GetProductsCount(request, cancellationToken)))
            .OrderBy(a => a.CreatedOn)
            .ToListAsync(cancellationToken);
            return _mapper.Map<List<ApplicationProduct>>(list);
        }
        /// <summary>
        /// Returns products count
        /// </summary>
        /// <param name="request">Contains properties to match with when applying filters</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns products count</returns>
        public async Task<int> GetProductsCount(GetProducts request, CancellationToken cancellationToken)
        {
            var recordcount = await Task.FromResult(_dbContext.ProductMasters
                .Include(a => a.ProductCategories)
            .Where(x => (x.ForSupplierOnly == request.ForSupplierOnly)
            && (request.Id != null ? x.Id == request.Id : true)
            && (!string.IsNullOrEmpty(request.Name) ? x.Name.ToLower().Trim().Contains(request.Name.ToLower().Trim()) : true)
            && (!string.IsNullOrEmpty(request.Plu) ? x.Plu.ToLower().Trim().Contains(request.Plu.ToLower().Trim()) : true)
            && (request.FromPrice != null ? request.FromPrice >= x.Price : true)
            && (request.ToPrice != null ? x.Price <= request.ToPrice : true)
            && (request.ProductCategory != null ? x.ProductCategoryId == request.ProductCategory : true)
            && (request.ProductType != null ? x.ProductType == request.ProductType : true)
            && (request.IsActive != null ? x.IsActive == request.IsActive : true)
            && (request.IsRepresentation != null ? x.IsRepresentation == request.IsRepresentation : true)
            ).Count());
            return recordcount;
        }
        /// <summary>
        /// This method is used to get product based on supplied id
        /// </summary>
        /// <param name="id">Id to match and return product</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns a single product object</returns>
        public async Task<ApplicationProduct> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await _dbContext.ProductMasters.Include(x => x.ProductListProductsMappings).AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            return _mapper.Map<ApplicationProduct>(product);
        }
        #endregion

        #region Update
        /// <summary>
        /// This method is used to update a product based on supplied product object
        /// </summary>
        /// <param name="product">Contains values to update for a specific</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns id of updated product</returns>
        public async Task<int> UpdateProduct(ProductMaster product, CancellationToken cancellationToken)
        {
            _dbContext.ProductMasters.Update(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
        #endregion

        #region Delete
        /// <summary>
        /// This method is used to soft delete a product based on supplied id
        /// </summary>
        /// <param name="id">Id to match and soft delete product</param>
        /// <returns>Returns id of deleted product</returns>
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _dbContext.ProductMasters.FirstOrDefaultAsync(a => a.Id == id);
            _dbContext.ProductMasters.Remove(product);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
