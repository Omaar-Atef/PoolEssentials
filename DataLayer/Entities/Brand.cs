using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Brand
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string Image {  get; set; }
        public ICollection<BrandCategory> BrandCategories { get; set; } = new List<BrandCategory>();
        public ICollection<Product> Products { get; set; }


    }
}
