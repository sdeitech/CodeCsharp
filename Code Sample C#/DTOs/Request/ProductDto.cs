using bestallningsportal.DTO.Request.MediaManager;
using System.Collections.Generic;

namespace bestallningsportal.DTO.Request
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; } 
        public string Comment { get; set; }
        public int Vat { get; set; }
        public string Plu { get; set; }
        public string Description { get; set; }
        public bool IsPackagable { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutomaticDeposit { get; set; }
        public int ProductType { get; set; }
        public MediaModule ModuleName { get; set; } = MediaModule.Product;
        public int ProductCategoryId { get; set; }
        public bool ForSupplierOnly { get; set; }
        public bool IsRepresentation { get; set; } = false;
        public int? AutomaticDepositType { get; set; }
        public bool IsPant { get; set; }
        public bool IsPriceChanged { get; set; }
        public bool IsPriceEditable { get; set; }
        public int AccountNumber { get; set; }
        public int AlcoholType { get; set; }

        public int? RepTyp { get; set; }
        public List<ProductListProductDto> ProductListProductsMappings { get; set; } = null;
    }
}
