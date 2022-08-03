using System;

namespace webapi.Models.ProductService
{
    public class ProductServiceModel
    {
        public Guid uid { get; set; }
        public string Name { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
