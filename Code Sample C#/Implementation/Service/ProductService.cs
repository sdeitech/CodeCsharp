using AutoMapper;
using bestallningsportal.Application.CheckListMaster.Commands.CreateRestaurant;
using bestallningsportal.Application.Common.Interfaces.Repositories;
using bestallningsportal.Application.Common.Interfaces.Services;
using bestallningsportal.Application.Common.Models.Response;
using bestallningsportal.Application.Common.Models.Response.Product;
using bestallningsportal.Application.Product.Commands.ActivateProduct;
using bestallningsportal.Application.Product.Commands.CreateProduct;
using bestallningsportal.Application.Product.Commands.ImportProduct;
using bestallningsportal.Application.Product.Commands.UpdateProduct;
using bestallningsportal.Application.Product.Queries.GetProduct;
using bestallningsportal.Domain.Entities;
using bestallningsportal.Domain.Entities.OrderTables;
using bestallningsportal.DTO.Request.MediaManager;
using bestallningsportal.DTO.Request.Product;
using bestallningsportal.Infrastructure.Attributes;
using ExcelDataReader;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bestallningsportal.Infrastructure.Implementation.Services
{
    /// <summary>
    /// This service implements the methods defined in the implemented interface
    /// </summary>
    [ScopedService]
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMediaManagerFileRepository _mediaManagerRepo;
        private readonly ILogger<IRequest> logger;
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment _env;

        public ProductService(IProductRepository productRepository, IMapper mapper,
            IMediaManagerFileRepository mediaManagerRepo, ILogger<IRequest> logger, IConfiguration configuration, IOrderRepository orderRepository,
            IWebHostEnvironment env)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _mediaManagerRepo = mediaManagerRepo;
            this.logger = logger;
            _configuration = configuration;
            _orderRepository = orderRepository;
            _env = env;
        }

        #region Create
        /// <summary>
        /// This method is used to create a product
        /// </summary>
        /// <param name="createProductCommand">Command containing properties to create new product</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Return Id of newly created product</returns>
        public async Task<int> AddProduct(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
        {
            return await _productRepository.AddProduct(createProductCommand.Adapt<ApplicationProduct>(), cancellationToken);
        }
        #endregion

        #region Return
        /// <summary>
        /// This method is used to get products based on supplied filters
        /// </summary>
        /// <param name="request">Contains properties to match with when applying filters</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns list of products and count as well</returns>
        public async Task<PaginatedProducts> GetProducts(GetProducts request, CancellationToken cancellationToken)
        {
            var paginatedResult = new PaginatedProducts();
            List<ApplicationMediaManagerFile> mediaManagers = null;
            var moduleName = MediaModule.Product.ToString();
            var result = await _productRepository.GetProducts(request, cancellationToken);
            if (result != null && result.Any())
            {
                mediaManagers = await AttachMediaFiles(result, moduleName);
                if (mediaManagers != null && mediaManagers.Any())
                {
                    result.ForEach(x => x.MediaManagerFiles = mediaManagers.Where(y => y.ModuleId == x.Id).ToList());
                }
            }
            paginatedResult.ProductsList = result != null && result.Any() ? result.ToList() : new List<ApplicationProduct>();
            paginatedResult.TotalCount = result != null ? await _productRepository.GetProductsCount(request, cancellationToken) : 0;
            return paginatedResult;
        }
        /// <summary>
        /// This method is used to get product based on supplied id
        /// </summary>
        /// <param name="id">Id to match and return product</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns a single product object</returns>
        public async Task<ApplicationProduct> GetProductById(int id, CancellationToken cancellationToken)
        {
            List<ApplicationMediaManagerFile> mediaManagers = null;
            var moduleName = MediaModule.Product.ToString();
            var result = await _productRepository.GetProductById(id, cancellationToken);
            if (result != null)
            {
                mediaManagers = await AttachMediaFiles(result, moduleName);
            }
            if (mediaManagers.Any())
            {
                result.MediaManagerFiles = mediaManagers;
            }
            return result;
        }
        #endregion

        #region Update
        /// <summary>
        /// This method is used to update a product based on supplied product object
        /// </summary>
        /// <param name="updateProductCommand">Contains values to update for a specific</param>
        /// <param name="cancellationToken">To notify the application if the operation need to be canceled</param>
        /// <returns>Returns id of updated product</returns>
        public async Task<int> UpdateProduct(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken)
        {
            try
            {
                var productToUpdate = updateProductCommand.Adapt<ProductMaster>();
                var updatedProductID = await _productRepository.UpdateProduct(productToUpdate, cancellationToken);
                if (updatedProductID > 0 && updateProductCommand.IsPriceChanged)
                {
                    var orderProductMappings = await _orderRepository.GetFilteredOrderProductMappings(updateProductCommand.Id, cancellationToken);
                    orderProductMappings.ForEach(x => x.Price = updateProductCommand.Price);
                    var isUpdated = await _orderRepository.UpdateOrderProductMappings(orderProductMappings, cancellationToken);
                }

                return updatedProductID;
            }
            catch (Exception ex)
            {
                return 0;
            }
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
            return await _productRepository.DeleteProduct(id);
        }
        #endregion

        #region Helper Methods
        private async Task<List<ApplicationMediaManagerFile>> AttachMediaFiles(ApplicationProduct product, string moduleName)
        {
            List<MediaManager> mediaManagers = await _mediaManagerRepo.GetFileByModule(moduleName, product.Id);
            return _mapper.Map<List<ApplicationMediaManagerFile>>(mediaManagers);
        }

        private async Task<List<ApplicationMediaManagerFile>> AttachMediaFiles(List<ApplicationProduct> product, string moduleName)
        {
            var prodIds = product.Select(x => x.Id).ToList();
            var batches = BuildChunksWithRange(prodIds, 500);
            var mediaManagers = new List<MediaManager>();
            foreach (var batch in batches)
            {
                var result = await _mediaManagerRepo.GetFileByModuleIds(moduleName, batch.ToList());
                mediaManagers.AddRange(result.ToList());
            }
            return _mapper.Map<List<ApplicationMediaManagerFile>>(mediaManagers);
        }
        private IEnumerable<IEnumerable<T>> BuildChunksWithRange<T>(List<T> fullList, int batchSize)
        {
            List<List<T>> chunkedList = new List<List<T>>();
            int index = 0;

            while (index < fullList.Count)
            {
                int rest = fullList.Count - index;
                if (rest >= batchSize)
                {
                    chunkedList.Add(fullList.GetRange(index, batchSize));
                }
                else
                {
                    chunkedList.Add(fullList.GetRange(index, rest));
                }
                index += batchSize;
            }

            return chunkedList;
        }
        #endregion
    }
}
