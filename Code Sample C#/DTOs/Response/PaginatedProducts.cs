using System.Collections.Generic;

namespace bestallningsportal.Application.Common.Models.Response.Product
{
    public class PaginatedProducts
    {
        public PaginatedProducts()
        {
            ProductsList = new List<ApplicationProduct>();
        }
        public int TotalCount { get; set; }
        public List<ApplicationProduct> ProductsList { get; set; }
    }
}
