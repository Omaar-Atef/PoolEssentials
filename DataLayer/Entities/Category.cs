using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        public string Image { get; set; } 

        public ICollection<BrandCategory> BrandCategories { get; set; } 

        public ICollection<Product> Products { get; set; }

    }
}
