using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buss.Layer.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string Image { get; set; }
        public string? PDFLink { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
