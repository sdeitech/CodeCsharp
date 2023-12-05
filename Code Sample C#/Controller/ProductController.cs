using bestallningsportal.Application.Common.Models.Response.Product;
using bestallningsportal.Application.Product.Commands.CreateProduct;
using bestallningsportal.Application.Product.Commands.DeleteProduct;
using bestallningsportal.Application.Product.Commands.UpdateProduct;
using bestallningsportal.Application.Product.Queries.GetProduct;
using bestallningsportal.Application.Product.Queries.GetProductById;
using bestallningsportal.DTO.Request;
using bestallningsportal.DTO.Request.Product;
using bestallningsportal.Presentation.Authorization;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bestallningsportal.Presentation.Controllers
{
    /// <summary>
    /// This controller is used to perform operations on Products
    /// Only users having SuperAdmin, Supplier, Reviewer and Client are able to make requests to the actions of this controller
    /// </summary>
    [Produces("application/json")]
    [AuthorizeRole(Role.SuperAdmin, Role.Supplier, Role.Reviewer, Role.Client)]
    public class ProductController : ApiControllerBase
    {
        #region Create
        /// <summary>
        /// This action is used to create new product with supplied details
        /// </summary>
        /// <param name="productDto">Contains details to be used to create a new product</param>
        /// <returns>Returns status code along with id of newly created product</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            var command = productDto.Adapt<CreateProductCommand>();
            return Ok(await Mediator.Send(command));
        }
        #endregion

        #region Return

        /// <summary>
        /// This action is used to get all the products based on provided filters
        /// </summary>
        /// <param name="filtersDTO">Contains properties to match with when applying filters</param>
        /// <returns>Returns a paginated result with list of products and count as well</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProduct([FromQuery] ProductFiltersDTO filtersDTO)
        {
            var result = await Mediator.Send(filtersDTO.Adapt<GetProducts>());
            return Ok(result.Adapt<PaginatedProducts>());
        }

        /// <summary>
        /// This action is used to get single product based on supplied id
        /// Only users having SuperAdmin, Supplier and Client are able to make requests to this action
        /// </summary>
        /// <param name="id">Id to match and return product</param>
        /// <returns>Returns an object with details of product</returns>
        [AuthorizeRole(Role.SuperAdmin, Role.Supplier, Role.Client)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await Mediator.Send(new GetProductByIdQuery { Id = id });
            return Ok(result.Adapt<ProductDto>());
        }
        #endregion

        #region Update
        /// <summary>
        /// This action is used to update a product based on supplied product object
        /// Only users having SuperAdmin and Supplier are able to make requests to this action
        /// </summary>
        /// <param name="productDto">Contains values to update for a specific</param>
        /// <returns>Returns status code along with id of updated product</returns>
        [AuthorizeRole(Role.SuperAdmin, Role.Supplier)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            var command = productDto.Adapt<UpdateProductCommand>();
            return Ok(await Mediator.Send(command));
        }
        #endregion

        #region Delete
        /// <summary>
        /// This action is used to soft delete a product based on supplied id
        /// Only users having SuperAdmin, Supplier and Client are able to make requests to this action
        /// </summary>
        /// <param name="id">>Id to match and soft delete product</param>
        /// <returns>Returns status code along with id of deleted product</returns>
        [AuthorizeRole(Role.SuperAdmin, Role.Supplier, Role.Client)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await Mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }
        #endregion
    }
}
